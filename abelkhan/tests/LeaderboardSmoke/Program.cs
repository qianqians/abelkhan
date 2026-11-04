using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using core;
using leaderboard;

// 排行榜冒烟测试：需要 Redis。
//   - 设置 LEADERBOARD_TEST_REDIS=127.0.0.1:6379 使用已有 Redis（只用独立 key 前缀，不 flush）；
//   - 否则自动启动仓库自带的 dependences/windows/redis/redis-server.exe；
//   - 两者都没有时打印 SKIP 并返回 0，不阻塞 CI。

var external = Environment.GetEnvironmentVariable("LEADERBOARD_TEST_REDIS");
var redisExe = string.IsNullOrWhiteSpace(external) ? FindBundledRedis() : null;
if (string.IsNullOrWhiteSpace(external) && redisExe == null)
{
    Console.WriteLine("SKIP: 未找到 Redis（设置 LEADERBOARD_TEST_REDIS 或放入 dependences/windows/redis）");
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
    tempDir = Path.Combine(AppContext.BaseDirectory, "leaderboard-smoke-" + Guid.NewGuid().ToString("N"));
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
    var prefix = "lb-smoke-" + Guid.NewGuid().ToString("N") + ":";
    var redis = new RedisHandle($"127.0.0.1:{port}", "");
    var store = new LeaderboardStore(redis, new LeaderboardSettings { KeyPrefix = prefix, MaxEntries = 0 });

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

    Check(store.Keys.Board("level") == prefix + "leaderboard:level" &&
          !store.Keys.Board("level").Contains("{{"), "Redis key 使用固定前缀，不带 hash tag");

    Console.WriteLine("== 1. 打榜 + 降序排名 ==");
    Check((await store.SubmitAsync("level", "alice", 100)).Changed, "alice 上榜 100");
    Check((await store.SubmitAsync("level", "bob", 200)).Changed, "bob 上榜 200");
    Check((await store.SubmitAsync("level", "carol", 150)).Changed, "carol 上榜 150");

    var (top, total) = await store.GetRangeAsync("level", 1, 3);
    Check(total == 3 && top.Count == 3, "榜单共 3 人");
    Check(top[0].MemberId == "bob" && top[0].Score == 200 && top[0].Rank == 1, "bob 第 1（200）");
    Check(top[1].MemberId == "carol" && top[1].Score == 150 && top[1].Rank == 2, "carol 第 2（150）");
    Check(top[2].MemberId == "alice" && top[2].Score == 100 && top[2].Rank == 3, "alice 第 3（100）");

    Console.WriteLine("== 2. 仅保留最高分（ZADD GT） ==");
    var lower = await store.SubmitAsync("level", "alice", 50);
    Check(!lower.Changed && lower.Rank == 3 && lower.Score == 100, "低于最高分不更新、名次不变");
    var better = await store.SubmitAsync("level", "alice", 250);
    Check(better.Changed && better.Rank == 1 && better.Score == 250, "更高分更新并升到第 1");

    Console.WriteLine("== 3. 查询单个成员名次 ==");
    var bob = await store.GetEntryAsync("level", "bob");
    Check(bob != null && bob.Rank == 2 && bob.Score == 200, "bob 名次 2 分数 200");
    Check(await store.GetEntryAsync("level", "nobody") == null, "不在榜返回 null");

    Console.WriteLine("== 4. 区间拉取 [start, end) ==");
    var (page, pageTotal) = await store.GetRangeAsync("level", 1, 2);
    Check(pageTotal == 3 && page.Count == 2 && page[0].MemberId == "alice" && page[1].MemberId == "bob",
        "区间 [1,2] 只返回前两名");

    Console.WriteLine("== 5. 裁剪只保留 top N ==");
    var trimStore = new LeaderboardStore(redis, new LeaderboardSettings { KeyPrefix = prefix, MaxEntries = 2 });
    await trimStore.SubmitAsync("trim", "a", 1);
    await trimStore.SubmitAsync("trim", "b", 2);
    await trimStore.SubmitAsync("trim", "c", 3);
    var (trimmed, trimmedTotal) = await trimStore.GetRangeAsync("trim", 1, 10);
    Check(trimmedTotal == 2 && trimmed.Count == 2, "裁剪后只剩 2 人");
    Check(trimmed[0].MemberId == "c" && trimmed[1].MemberId == "b", "保留分数最高的 2 人");

    Console.WriteLine("== 6. 重置榜单 ==");
    await store.ResetAsync("level");
    var (afterReset, afterResetTotal) = await store.GetRangeAsync("level", 1, 100);
    Check(afterResetTotal == 0 && afterReset.Count == 0, "重置后榜单为空");

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
