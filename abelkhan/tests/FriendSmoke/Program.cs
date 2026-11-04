using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using core;
using friend;

// 好友系统冒烟测试：需要 Redis。
//   - 设置 FRIEND_TEST_REDIS=127.0.0.1:6379 使用已有 Redis（只用独立 key 前缀，不 flush）；
//   - 否则自动启动仓库自带的 dependences/windows/redis/redis-server.exe；
//   - 两者都没有时打印 SKIP 并返回 0，不阻塞 CI。
//
// 覆盖的是 store 层的完整语义（hub 的编排逻辑都建立在这些原子操作上）：
//   申请 → 同意占位 → 跨服确认 → 好友双向可见；上限拦截；占位超时回收；删除；好友位扩展。

var external = Environment.GetEnvironmentVariable("FRIEND_TEST_REDIS");
var redisExe = string.IsNullOrWhiteSpace(external) ? FindBundledRedis() : null;
if (string.IsNullOrWhiteSpace(external) && redisExe == null)
{
    Console.WriteLine("SKIP: 未找到 Redis（设置 FRIEND_TEST_REDIS 或放入 dependences/windows/redis）");
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
    tempDir = Path.Combine(AppContext.BaseDirectory, "friend-smoke-" + Guid.NewGuid().ToString("N"));
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
    var prefix = "fr-smoke-" + Guid.NewGuid().ToString("N") + ":";
    var redis = new RedisHandle($"127.0.0.1:{port}", "");
    var settings = new FriendSettings
    {
        KeyPrefix = prefix,
        MaxFriends = 3,
        MaxPendingRequests = 5,
        ReserveTimeoutMs = 1000,
        PresenceTtlMs = 15_000,
        RouteTtlMs = 60_000,
    };
    var store = new FriendStore(redis, settings);

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

    void CheckErr(FriendResult result, string code, string name)
    {
        if (result.Ok || result.Code != code)
        {
            throw new Exception($"FAILED: {name} (expect {code}, got ok={result.Ok} code={result.Code})");
        }

        passed++;
        Console.WriteLine($"  ok  {name}  [{result.Code}]");
    }

    Console.WriteLine("== 1. 申请 → 同意占位 → 确认 ==");
    var submit = await store.SubmitRequestAsync("bob", "alice");
    Check(submit.Ok, "bob 申请加 alice");
    Check((await store.ListInboxAsync("alice")).Contains("bob"), "申请进入 alice 的收件箱（离线消息）");
    Check((await store.ListSentAsync("bob")).Contains("alice"), "bob 的已发列表记录这条申请");

    CheckErr(await store.SubmitRequestAsync("bob", "alice"), FriendCodes.Duplicate, "重复申请被拦");
    CheckErr(await store.SubmitRequestAsync("alice", "alice"), FriendCodes.Self, "不能加自己");

    var (accept, deadline) = await store.AcceptReserveAsync("alice", "bob");
    Check(accept.Ok, "alice 同意 → 占位");
    Check(deadline > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), "占位带截止时间");
    var limitAfterAccept = await store.GetLimitAsync("alice");
    Check(limitAfterAccept.Reserved == 1 && limitAfterAccept.Used == 1, "alice 已用 = 0 好友 + 1 占位");

    var confirm = await store.ConfirmAddAsync("bob", "alice");
    Check(confirm.Ok, "bob 侧确认加好友");
    Check((await store.ListFriendsAsync("alice")).Contains("bob"), "alice 的好友列表有 bob");
    Check((await store.ListFriendsAsync("bob")).Contains("alice"), "bob 的好友列表有 alice");
    Check((await store.ListInboxAsync("alice")).Count == 0, "确认后收件箱清空");
    Check((await store.GetLimitAsync("alice")).Reserved == 0, "确认后占位清零");

    CheckErr(await store.SubmitRequestAsync("alice", "bob"), FriendCodes.AlreadyFriend, "已是好友不能再申请");

    Console.WriteLine("== 2. 好友上限（全局 3） ==");
    foreach (var other in new[] { "c1", "c2" })
    {
        await store.SubmitRequestAsync(other, "alice");
        await store.AcceptReserveAsync("alice", other);
        Check((await store.ConfirmAddAsync(other, "alice")).Ok, $"alice 再加 {other}（共 3 人）");
    }

    var full = await store.SubmitRequestAsync("c3", "alice");
    CheckErr(full, FriendCodes.TargetFull, "alice 已满 → 申请直接拒绝");

    Console.WriteLine("== 3. 好友位扩展（extra） ==");
    await store.SetExtraLimitAsync("alice", 1);
    Check((await store.GetLimitAsync("alice")).Capacity == 4, "额外好友位生效，上限 3 → 4");
    Check((await store.SubmitRequestAsync("c3", "alice")).Ok, "扩展后可以继续申请");

    Console.WriteLine("== 4. 申请方自己满了也要拦 ==");
    await store.RemoveFriendAsync("alice", "c3");
    // d1 先把 bob 之外的位置占满
    await store.SetExtraLimitAsync("d1", 0);
    foreach (var other in new[] { "e1", "e2", "e3" })
    {
        await store.SubmitRequestAsync(other, "d1");
        await store.AcceptReserveAsync("d1", other);
        await store.ConfirmAddAsync(other, "d1");
    }

    CheckErr(await store.SubmitRequestAsync("d1", "bob"), FriendCodes.RequesterFull,
        "申请方好友已满 → 不允许主动申请");

    Console.WriteLine("== 5. 占位超时回收 ==");
    await store.SubmitRequestAsync("f2", "f1");
    Check((await store.AcceptReserveAsync("f1", "f2")).Result.Ok, "f1 同意 → 占位");
    Check((await store.GetLimitAsync("f1")).Reserved == 1, "占位为 1");

    await Task.Delay(settings.ReserveTimeoutMs + 300);
    var expired = await store.SweepExpiredReservesAsync(100);
    Check(expired.Any(e => e.UserId == "f1" && e.OtherId == "f2"), "超期占位被扫描到");
    Check((await store.GetLimitAsync("f1")).Reserved == 0, "超期占位已被回收");

    Console.WriteLine("== 6. 拒绝 / 删除 ==");
    await store.SubmitRequestAsync("g2", "g1");
    Check(await store.RejectRequestAsync("g1", "g2"), "g1 拒绝 g2 的申请");
    Check((await store.ListInboxAsync("g1")).Count == 0, "拒绝后收件箱清空");
    Check((await store.ListSentAsync("g2")).Count == 0, "拒绝后对方已发列表清空");

    Check(await store.RemoveFriendAsync("alice", "bob"), "alice 删除 bob");
    Check(!(await store.ListFriendsAsync("alice")).Contains("bob"), "alice 列表里没有 bob 了");
    Check(!(await store.ListFriendsAsync("bob")).Contains("alice"), "bob 列表也同步解除");
    CheckErr(await store.ConfirmAddAsync("bob", "alice"), FriendCodes.NoRequest, "删除后确认会失败（申请已不存在）");

    Console.WriteLine("== 7. presence / 路由 ==");
    await store.SetPresenceAsync("alice", "gate-1", "entity-alice");
    await store.SetPresenceAsync("bob", "gate-2", "entity-bob");
    var presence = await store.GetPresenceAsync(new[] { "alice", "bob", "nobody" });
    Check(presence.Count == 2 && presence["alice"].EntityId == "entity-alice" &&
          presence["alice"].GateId == "gate-1", "presence 带 gateId + entityId");
    Check(!presence.ContainsKey("nobody"), "离线玩家不出现在 presence 里");

    await store.ReleasePresenceAsync("alice", "gate-1", "entity-alice");
    Check(!(await store.GetPresenceAsync(new[] { "alice" })).ContainsKey("alice"), "释放 presence");
    await store.ReleasePresenceAsync("bob", "gate-9", "entity-bob");
    Check((await store.GetPresenceAsync(new[] { "bob" })).ContainsKey("bob"), "gateId 不匹配时不误删");

    await store.SetRouteAsync("alice", "hub-1");
    Check(await store.GetRouteAsync("alice") == "hub-1", "路由写入 / 读取");
    Check(await store.GetRouteAsync("nobody") == null, "无路由返回 null");

    Console.WriteLine("== 8. 跨 hub 指令队列 ==");
    await store.PushHubCommandAsync("hub-1", new byte[] { 1, 2, 3 });
    await store.PushHubCommandAsync("hub-1", new byte[] { 4, 5 });
    var commands = await store.PopHubCommandsAsync("hub-1", 8);
    Check(commands.Count == 2 && commands[0].SequenceEqual(new byte[] { 1, 2, 3 }),
        "hub 指令按投递顺序出队");
    Check((await store.PopHubCommandsAsync("hub-1", 8)).Count == 0, "队列已空");

    // 清理本次测试写入的 key，避免反复运行把共享 Redis 撑大
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
