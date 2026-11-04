using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using core;
using guild;

// 帮会系统冒烟测试：需要 Redis。
//   - 设置 GUILD_TEST_REDIS=127.0.0.1:6379 使用已有 Redis（只用独立 key 前缀，不 flush）；
//   - 否则自动启动仓库自带的 dependences/windows/redis/redis-server.exe；
//   - 两者都没有时打印 SKIP 并返回 0，不阻塞 CI。
//
// 覆盖 store 层的完整语义（hub 的编排都建立在这些原子操作上）：
//   创建 / 重名 / 一人一会、邀请与接受、申请与审批、成员上限、权限拒绝、
//   踢人与职位、转让、退出、解散清理、任务发布/领取/进度/完成、任务目录。

var external = Environment.GetEnvironmentVariable("GUILD_TEST_REDIS");
var redisExe = string.IsNullOrWhiteSpace(external) ? FindBundledRedis() : null;
if (string.IsNullOrWhiteSpace(external) && redisExe == null)
{
    Console.WriteLine("SKIP: 未找到 Redis（设置 GUILD_TEST_REDIS 或放入 dependences/windows/redis）");
    return 0;
}

Process? redisProcess = null;
string? tempDir = null;
var port = 0;
if (!string.IsNullOrWhiteSpace(external))
{
    var parts = external.Split(':', 2);
    port = int.Parse(parts[1]);
}
else
{
    port = FreePort();
    tempDir = Path.Combine(AppContext.BaseDirectory, "guild-smoke-" + Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(tempDir);
    redisProcess = Process.Start(new ProcessStartInfo
    {
        FileName = redisExe!,
        Arguments = $"--port {port} --bind 127.0.0.1 --save \"\" --appendonly no --dir \"{tempDir}\" --logfile \"{Path.Combine(tempDir, "redis.log")}\"",
        UseShellExecute = false,
        CreateNoWindow = true,
    });
    if (redisProcess == null || !await WaitPort(port, 8000))
    {
        Console.WriteLine("SKIP: Redis 启动失败");
        return 0;
    }
}

try
{
    var prefix = "gd-smoke-" + Guid.NewGuid().ToString("N") + ":";
    var redis = new RedisHandle($"127.0.0.1:{port}", "");
    var settings = new GuildSettings
    {
        KeyPrefix = prefix,
        MaxMembers = 3,
        MaxOfficers = 1,
        MaxApplications = 5,
        MaxInvites = 3,
        NameMinLength = 2,
        NameMaxLength = 8,
        DefaultTaskIds = { "daily_kill" },
    };
    var store = new GuildStore(redis, settings);

    var passed = 0;
    void Check(bool ok, string name)
    {
        if (!ok)
        {
            throw new Exception("FAILED: " + name);
        }

        passed++;
        Console.WriteLine("  ok  " + name);
    }

    void CheckErr(GuildResult result, string code, string name)
    {
        if (result.Ok || result.Code != code)
        {
            throw new Exception($"FAILED: {name} (expect {code}, got ok={result.Ok} code={result.Code})");
        }

        passed++;
        Console.WriteLine($"  ok  {name}  [{result.Code}]");
    }

    Console.WriteLine("== 1. 任务目录 ==");
    var catalog = new GuildTaskCatalog();
    catalog.Register(new GuildTaskDefinition("daily_kill", "讨伐守卫", Target: 10));
    catalog.Register(new GuildTaskDefinition("guild_boss", "帮会首领", Target: 1, PerMember: false,
        OfficerCanPublish: true, RewardKey: "reward_boss"));
    Check(catalog.TryGet("daily_kill", out var daily) && daily!.Target == 10, "注册并查到默认任务");
    Check(catalog.TryGet("guild_boss", out var boss) && !boss!.PerMember, "共享任务标记正确");
    Check(!catalog.TryGet("nope", out _), "未知任务查不到");
    Check(catalog.All.Count == 2, "目录里共 2 条");

    Console.WriteLine("== 2. 名字校验 ==");
    Check(settings.NormalizeName("  铁血盟  ", out var normalized) == null && normalized == "铁血盟",
        "去首尾空白后合法");
    Check(settings.NormalizeName("a", out _) == GuildCodes.NameInvalid, "太短被拒");
    Check(settings.NormalizeName("123456789", out _) == GuildCodes.NameInvalid, "太长被拒");
    Check(settings.NormalizeName("bad{a}", out _) == GuildCodes.NameInvalid, "花括号被拒");
    Check(GuildSettings.NameKey("铁血盟") == "铁血盟" && GuildSettings.NameKey("ABC") == "abc",
        "名字索引大小写不敏感");

    Console.WriteLine("== 3. 创建 / 重名 / 一人一会 ==");
    var g1 = GuildStore.NewGuildId();
    Check((await store.CreateAsync(g1, "铁血盟", "leader")).Ok, "创建帮会");
    var info = await store.GetInfoAsync(g1);
    Check(info != null && info.Name == "铁血盟" && info.LeaderId == "leader", "帮会信息正确");
    Check(await store.GetRoleAsync(g1, "leader") == GuildRole.Leader, "创建者是帮主");
    Check(await store.GetPlayerGuildIdAsync("leader") == g1, "玩家帮会索引指向新帮会");
    Check((await store.GetMembersAsync(g1)).Count == 1, "成员数为 1");

    CheckErr(await store.CreateAsync(GuildStore.NewGuildId(), "铁血盟", "other"), GuildCodes.NameTaken,
        "重名被拒");
    CheckErr(await store.CreateAsync(GuildStore.NewGuildId(), "别的会", "leader"), GuildCodes.AlreadyInGuild,
        "一人只能在一个帮会");

    Console.WriteLine("== 4. 邀请 → 接受 ==");
    CheckErr(await store.InviteAsync(g1, "m1", "invitee1"), GuildCodes.NoPermission, "普通成员不能邀请");
    Check((await store.InviteAsync(g1, "leader", "invitee1")).Ok, "帮主邀请成功");
    Check((await store.GetInvitesAsync("invitee1")).ContainsKey(g1), "被邀请人收到邀请");
    Check((await store.AcceptInviteAsync(g1, "invitee1")).Ok, "接受邀请入帮");
    Check(await store.GetRoleAsync(g1, "invitee1") == GuildRole.Member, "接受邀请后成为成员");
    Check((await store.GetInvitesAsync("invitee1")).Count == 0, "接受后邀请被清掉");
    CheckErr(await store.AcceptInviteAsync(g1, "invitee1"), GuildCodes.NoRequest, "同一个邀请不能接受两次");

    Console.WriteLine("== 5. 申请 → 审批 / 上限 ==");
    Check((await store.ApplyAsync(g1, "applier1")).Ok, "申请加入");
    Check((await store.GetAppliesAsync(g1)).ContainsKey("applier1"), "申请进入待审批列表");
    CheckErr(await store.ApproveAsync(g1, "invitee1", "applier1", true), GuildCodes.NoPermission,
        "普通成员不能审批");
    Check((await store.ApproveAsync(g1, "leader", "applier1", true)).Ok, "帮主审批通过");
    Check(await store.GetRoleAsync(g1, "applier1") == GuildRole.Member, "申请人成为成员");
    Check((await store.GetAppliesAsync(g1)).Count == 0, "审批后申请被清掉");

    Check((await store.ApplyAsync(g1, "applier2")).Ok, "第二个人申请");
    CheckErr(await store.ApproveAsync(g1, "leader", "applier2", true), GuildCodes.MemberFull,
        "已满员（leader + invitee1 + applier1 = 3/3），审批被拒");
    var (count, capacity) = await store.GetCapacityAsync(g1);
    Check(count == 3 && capacity == 3, "成员数达到上限");

    Console.WriteLine("== 6. 上限可扩展 ==");
    Check(await store.SetMemberCapExtraAsync(g1, 2), "给帮会增加 2 个附加位");
    var (_, capacity2) = await store.GetCapacityAsync(g1);
    Check(capacity2 == 5, "上限变成 5");
    Check((await store.ApproveAsync(g1, "leader", "applier2", true)).Ok, "扩容后可以继续审批（4/5）");

    Console.WriteLine("== 7. 职位 / 踢人 / 权限 ==");
    Check((await store.SetRoleAsync(g1, "leader", "invitee1", GuildRole.Officer)).Ok, "升副帮主");
    Check(await store.GetRoleAsync(g1, "invitee1") == GuildRole.Officer, "职位已更新");
    CheckErr(await store.SetRoleAsync(g1, "leader", "applier1", GuildRole.Officer), GuildCodes.OfficerFull,
        "副帮主名额已满");

    Check((await store.KickAsync(g1, "invitee1", "applier2")).Ok, "副帮主可以踢普通成员");
    Check(await store.GetPlayerGuildIdAsync("applier2") == null, "被踢成员的帮会索引被清掉");
    CheckErr(await store.KickAsync(g1, "invitee1", "leader"), GuildCodes.NoPermission, "不能踢帮主");
    CheckErr(await store.KickAsync(g1, "applier1", "leader"), GuildCodes.NoPermission, "普通成员不能踢人");

    Console.WriteLine("== 8. 退出 / 转让 / 解散 ==");
    CheckErr(await store.LeaveAsync(g1, "leader"), GuildCodes.LeaderMustTransfer, "帮主不能直接退出");
    Check((await store.LeaveAsync(g1, "applier1")).Ok, "普通成员可以退出");
    Check(await store.GetPlayerGuildIdAsync("applier1") == null, "退出后索引被清掉");

    Check((await store.TransferAsync(g1, "leader", "invitee1")).Ok, "转让帮主");
    Check(await store.GetRoleAsync(g1, "invitee1") == GuildRole.Leader, "新帮主生效");
    Check(await store.GetRoleAsync(g1, "leader") == GuildRole.Officer, "老帮主降为副帮主");
    Check((await store.GetInfoAsync(g1))!.LeaderId == "invitee1", "info 里的 leaderId 已更新");

    CheckErr(await store.DisbandAsync(g1, "leader").ContinueWith(t => t.Result.Result), GuildCodes.NoPermission,
        "非帮主不能解散");

    var disband = await store.DisbandAsync(g1, "invitee1");
    Check(disband.Result.Ok, "帮主解散帮会");
    Check(disband.Members.Count == 2, "返回剩余成员名单");
    Check(await store.GetInfoAsync(g1) == null, "帮会信息已删除");
    Check(await store.GetPlayerGuildIdAsync("invitee1") == null && await store.GetPlayerGuildIdAsync("leader") == null,
        "所有成员的帮会索引都被清掉");
    Check((await store.ListGuildIdsAsync()).Count == 0, "帮会索引里没有残留");

    Console.WriteLine("== 9. 帮会任务 ==");
    var g3 = GuildStore.NewGuildId();
    Check((await store.CreateAsync(g3, "任务会", "t_leader")).Ok, "新建帮会");

    var publish = await store.PublishTaskAsync(g3, "t_leader",
        new GuildTaskState("daily_kill", "t_leader", 1000), officerAllowed: false);
    Check(publish.Ok, "帮主发布任务");
    Check((await store.GetTasksAsync(g3)).ContainsKey("daily_kill"), "任务进入已发布列表");
    CheckErr(await store.PublishTaskAsync(g3, "t_leader",
            new GuildTaskState("daily_kill", "t_leader", 2000), false), GuildCodes.TaskPublished,
        "重复发布被拒");

    Check((await store.AcceptTaskAsync(g3, "t_leader", "daily_kill")).Ok, "成员领取任务");
    CheckErr(await store.AcceptTaskAsync(g3, "t_leader", "daily_kill"), GuildCodes.TaskAlreadyAccepted,
        "重复领取被拒");

    var (progressResult, progress, completed) =
        await store.AddTaskProgressAsync(g3, "daily_kill", "t_leader", 4, 10, shared: false);
    Check(progressResult.Ok && progress == 4 && !completed, "进度累加到 4/10");
    var (_, progress2, completed2) =
        await store.AddTaskProgressAsync(g3, "daily_kill", "t_leader", 6, 10, shared: false);
    Check(progress2 == 10 && completed2, "进度达到目标，标记完成");

    CheckErr((await store.AddTaskProgressAsync(g3, "daily_kill", "outsider", 1, 10, false)).Result,
        GuildCodes.TaskNotAccepted, "没领取的人不能上报进度");

    var finished = (await store.GetTaskAsync(g3, "daily_kill"))!;
    await store.MarkTaskFinishedAsync(g3, "daily_kill", finished with { Status = GuildTaskStatus.Finished });
    Check((await store.GetTaskAsync(g3, "daily_kill"))!.Status == GuildTaskStatus.Finished, "任务状态置为完成");

    Check((await store.CancelTaskAsync(g3, "t_leader", "daily_kill")).Ok, "取消任务");
    Check((await store.GetTasksAsync(g3)).Count == 0, "任务从已发布列表移除");
    Check((await store.GetTaskProgressAsync(g3, "daily_kill")).Count == 0, "任务进度一并清理");

    // 清理本次测试写入的 key
    foreach (var key in await redis.GetKeys(prefix + "*"))
    {
        redis.DelData(key);
    }

    Console.WriteLine();
    Console.WriteLine($"全部通过：{passed} 项断言");
    return 0;
}
finally
{
    if (redisProcess is { HasExited: false })
    {
        try
        {
            redisProcess.Kill(entireProcessTree: true);
            redisProcess.WaitForExit(3000);
        }
        catch
        {
            // ignore
        }
    }

    if (tempDir != null && Directory.Exists(tempDir))
    {
        try
        {
            Directory.Delete(tempDir, true);
        }
        catch
        {
            // ignore
        }
    }
}

static string? FindBundledRedis()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir != null)
    {
        var candidate = Path.Combine(dir.FullName, "dependences", "windows", "redis", "redis-server.exe");
        if (File.Exists(candidate))
        {
            return candidate;
        }

        dir = dir.Parent;
    }

    return null;
}

static int FreePort()
{
    var listener = new TcpListener(IPAddress.Loopback, 0);
    listener.Start();
    var port = ((IPEndPoint)listener.LocalEndpoint).Port;
    listener.Stop();
    return port;
}

static async Task<bool> WaitPort(int port, int timeoutMs)
{
    var deadline = Environment.TickCount64 + timeoutMs;
    while (Environment.TickCount64 < deadline)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, port);
            return true;
        }
        catch
        {
            await Task.Delay(100);
        }
    }

    return false;
}
