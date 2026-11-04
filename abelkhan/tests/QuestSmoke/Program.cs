using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using core;
using quest;

// 任务系统冒烟测试：需要 Redis。
//   - 设置 QUEST_TEST_REDIS=127.0.0.1:6379 使用已有 Redis（只用独立 key 前缀，不 flush）；
//   - 否则自动启动仓库自带的 dependences/windows/redis/redis-server.exe；
//   - 两者都没有时打印 SKIP 并返回 0，不阻塞 CI。
//
// 覆盖两块：
//   1) quest.QuestRules 的纯判定（三种 Mode、前置任务、进度全满、多后继挑一、根任务集合）；
//   2) QuestStore 的 Redis 语义（事件状态 / 接取 / 进度推进 / 完成清理 / 玩家重置）。

var external = Environment.GetEnvironmentVariable("QUEST_TEST_REDIS");
var redisExe = string.IsNullOrWhiteSpace(external) ? FindBundledRedis() : null;
if (string.IsNullOrWhiteSpace(external) && redisExe == null)
{
    Console.WriteLine("SKIP: 未找到 Redis（设置 QUEST_TEST_REDIS 或放入 dependences/windows/redis）");
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
    tempDir = Path.Combine(AppContext.BaseDirectory, "quest-smoke-" + Guid.NewGuid().ToString("N"));
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

    var prefix = "qs-smoke-" + Guid.NewGuid().ToString("N") + ":";
    var redis = new RedisHandle($"127.0.0.1:{port}", "");
    var settings = new QuestSettings { KeyPrefix = prefix, MaxActiveQuests = 3 };
    var store = new QuestStore(redis, settings);

    Console.WriteLine("== 1. 条件判定（三种 Mode） ==");
    var countCond = new QuestCondition("kill", "goblin", QuestConditionMode.Count, 10);
    var maxCond = new QuestCondition("level", "", QuestConditionMode.Max, 10);
    var flagCond = new QuestCondition("talk", "npc_7", QuestConditionMode.Flag);

    Check(!QuestRules.ConditionSatisfied(countCond, 9), "Count 未到目标不满足");
    Check(QuestRules.ConditionSatisfied(countCond, 10), "Count 达到目标满足");
    Check(!QuestRules.ConditionSatisfied(maxCond, 9) && QuestRules.ConditionSatisfied(maxCond, 10),
        "Max 按绝对值比较（等级 10）");
    Check(!QuestRules.ConditionSatisfied(flagCond, 0) && QuestRules.ConditionSatisfied(flagCond, 1),
        "Flag 只要发生过就满足");

    Console.WriteLine("== 2. 触发条件（对事件状态求值） ==");
    var def = new QuestDefinition(
        "q_main_1", "初入江湖",
        TriggerConditions: [new QuestCondition("level", "", QuestConditionMode.Max, 5)],
        ProgressConditions: [countCond]);
    var events = new Dictionary<string, long> { ["level|"] = 5 };
    Check(QuestRules.TriggersSatisfied(def, events), "等级事件达到 5 → 触发条件满足");
    events["level|"] = 4;
    Check(!QuestRules.TriggersSatisfied(def, events), "等级只有 4 → 触发条件不满足");

    Console.WriteLine("== 3. 能否接取（前置 / 已接 / 已完成） ==");
    var noRequire = new Dictionary<string, long> { ["level|"] = 10 };
    Check(QuestRules.CanBeAccepted(def, noRequire, [], []), "条件满足且没有任何记录 → 可接");
    Check(!QuestRules.CanBeAccepted(def, noRequire, [], ["q_main_1"]), "已在进行中 → 不可再接");
    Check(!QuestRules.CanBeAccepted(def, noRequire, ["q_main_1"], []), "已完成 → 不可再接");

    var chained = def with { QuestId = "q_main_2", RequireQuests = ["q_main_1"] };
    Check(!QuestRules.CanBeAccepted(chained, noRequire, [], []), "前置任务没完成 → 不可接");
    Check(QuestRules.CanBeAccepted(chained, noRequire, ["q_main_1"], []), "前置任务完成 → 可接");

    Console.WriteLine("== 4. 进度全满 ==");
    var progressDef = new QuestDefinition("q_p", "打怪",
        ProgressConditions:
        [
            new QuestCondition("kill", "goblin", QuestConditionMode.Count, 5),
            new QuestCondition("kill", "wolf", QuestConditionMode.Count, 2),
        ]);
    Check(!QuestRules.ProgressFull(progressDef, new Dictionary<string, long> { ["kill|goblin"] = 5 }),
        "只满足其中一条 → 不算满");
    Check(QuestRules.ProgressFull(progressDef,
            new Dictionary<string, long> { ["kill|goblin"] = 5, ["kill|wolf"] = 2 }),
        "两条都满足 → 满");
    Check(QuestRules.ProgressFull(new QuestDefinition("q_none", "无进度"), new Dictionary<string, long>()),
        "没有进度条件的任务直接算满");

    Console.WriteLine("== 5. 多后继 + 条件挑一 / 根任务集合 ==");
    var catalog = new QuestCatalog();
    catalog.RegisterRange(
    [
        new QuestDefinition("root", "起点", NextQuestIds: ["branch_a", "branch_b"]),
        new QuestDefinition("branch_a", "分支A", RequireQuests: ["root"],
            TriggerConditions: [new QuestCondition("vip", "", QuestConditionMode.Flag)]),
        new QuestDefinition("branch_b", "分支B", RequireQuests: ["root"]),
    ]);

    var roots = catalog.Roots.Select(r => r.QuestId).OrderBy(x => x).ToList();
    Check(roots.SequenceEqual(["root"]), "根任务只有 root（两条分支都在链上，不会被全局扫描）");

    var rootDef = catalog.All.First(d => d.QuestId == "root");
    var noVip = new Dictionary<string, long>();
    var withVip = new Dictionary<string, long> { ["vip|"] = 1 };

    Check(QuestRules.PickNext(rootDef, catalog, noVip, ["root"], []) == "branch_b",
        "分支A 的触发条件不满足 → 挑到分支B");
    Check(QuestRules.PickNext(rootDef, catalog, withVip, ["root"], []) == "branch_a",
        "两个都满足时按配置顺序优先挑分支A");
    Check(QuestRules.PickNext(rootDef, catalog, withVip, [], []) == null,
        "前置任务 root 没完成 → 没有可接的后继");

    Console.WriteLine("== 6. 事件状态（Redis，三种 Mode） ==");
    Check(await store.ApplyEventAsync("u1", "kill|goblin", QuestConditionMode.Count, 3) == 3, "Count 累加到 3");
    Check(await store.ApplyEventAsync("u1", "kill|goblin", QuestConditionMode.Count, 4) == 7, "Count 再累加到 7");
    Check(await store.ApplyEventAsync("u1", "level|", QuestConditionMode.Max, 8) == 8, "Max 记为 8");
    Check(await store.ApplyEventAsync("u1", "level|", QuestConditionMode.Max, 5) == 8, "Max 遇到更小的值不降");
    Check(await store.ApplyEventAsync("u1", "level|", QuestConditionMode.Max, 12) == 12, "Max 升到 12");
    Check(await store.ApplyEventAsync("u1", "talk|npc_7", QuestConditionMode.Flag, 1) == 1, "Flag 置 1");

    var state = await store.GetEventStateAsync("u1");
    Check(state["kill|goblin"] == 7 && state["level|"] == 12 && state["talk|npc_7"] == 1,
        "事件状态整体读取正确");

    Console.WriteLine("== 7. 接取 / 进度 / 完成 ==");
    var killQuest = new QuestDefinition("q_kill", "讨伐哥布林",
        ProgressConditions: [new QuestCondition("kill", "goblin", QuestConditionMode.Count, 5)],
        NextQuestIds: ["q_next"], RewardKey: "rw_kill");

    Check((await store.StartQuestAsync("u2", killQuest, 1000, 3)).Ok, "接取任务");
    var active = await store.GetActiveStatesAsync("u2");
    Check(active.ContainsKey("q_kill") && active["q_kill"].Progress.ContainsKey("kill|goblin"),
        "进行中状态里带初始进度");

    var dup = await store.StartQuestAsync("u2", killQuest, 1000, 3);
    Check(!dup.Ok && dup.Code == QuestCodes.AlreadyActive, "重复接取被拒 [already_active]");

    Check(await store.BumpProgressAsync("u2", "q_kill",
        killQuest.Progress[0], 2) == 2, "进度推进到 2");
    Check(await store.BumpProgressAsync("u2", "q_kill",
        killQuest.Progress[0], 3) == 5, "进度推进到 5");

    var progress = await store.GetQuestProgressAsync("u2", "q_kill");
    Check(progress["kill|goblin"] == 5, "按任务读出进度");
    Check((await store.GetQuestProgressAsync("u2", "q_other")).Count == 0, "别的任务读不到这段进度");

    Check((await store.CompleteQuestAsync("u2", killQuest, 2000)).Ok, "完成任务");
    Check(!(await store.GetActiveStatesAsync("u2")).ContainsKey("q_kill"), "完成后移出进行中");
    Check((await store.GetDoneAsync("u2"))["q_kill"] == 2000, "记录完成时间");
    Check((await store.GetQuestProgressAsync("u2", "q_kill")).Count == 0, "完成后清理该任务进度");

    var again = await store.CompleteQuestAsync("u2", killQuest, 3000);
    Check(!again.Ok && again.Code == QuestCodes.AlreadyDone, "重复完成被拒 [already_done]");
    var started = await store.StartQuestAsync("u2", killQuest, 4000, 3);
    Check(!started.Ok && started.Code == QuestCodes.AlreadyDone, "已完成的任务不能再接 [already_done]");

    Console.WriteLine("== 8. 进行中上限与玩家重置 ==");
    for (var i = 0; i < 3; i++)
    {
        var d = new QuestDefinition("q_limit_" + i, "上限测试" + i);
        Check((await store.StartQuestAsync("u3", d, 1000, 3)).Ok, $"第 {i + 1} 个任务接取成功");
    }

    var overflow = await store.StartQuestAsync("u3", new QuestDefinition("q_limit_x", "溢出"), 1000, 3);
    Check(!overflow.Ok && overflow.Code == QuestCodes.TooManyActive, "超过进行中上限被拒 [too_many_active]");

    await store.ResetPlayerAsync("u3");
    Check((await store.GetActiveStatesAsync("u3")).Count == 0, "重置后没有进行中的任务");
    Check((await store.GetEventStateAsync("u3")).Count == 0, "重置后没有事件状态");

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
