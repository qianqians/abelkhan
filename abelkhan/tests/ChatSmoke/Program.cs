using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using chat;
using core;

// 聊天系统冒烟测试：需要 Redis。
//   - 设置 CHAT_TEST_REDIS=127.0.0.1:6379 使用已有 Redis（只用独立 key 前缀，不 flush）；
//   - 否则自动启动仓库自带的 dependences/windows/redis/redis-server.exe；
//   - 两者都没有时打印 SKIP 并返回 0，不阻塞 CI。
//
// 覆盖 store 层的语义（hub 的编排都建立在这些操作上）：
//   频道定义查找、内容清洗、发言冷却、历史写入与裁剪、历史时间正序、跨 hub pub/sub 往返。

var external = Environment.GetEnvironmentVariable("CHAT_TEST_REDIS");
var redisExe = string.IsNullOrWhiteSpace(external) ? FindBundledRedis() : null;
if (string.IsNullOrWhiteSpace(external) && redisExe == null)
{
    Console.WriteLine("SKIP: 未找到 Redis（设置 CHAT_TEST_REDIS 或放入 dependences/windows/redis）");
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
    tempDir = Path.Combine(AppContext.BaseDirectory, "chat-smoke-" + Guid.NewGuid().ToString("N"));
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
    var prefix = "chat-smoke-" + Guid.NewGuid().ToString("N") + ":";
    var redis = new RedisHandle($"127.0.0.1:{port}", "");
    var settings = new ChatSettings
    {
        KeyPrefix = prefix,
        ZoneId = "zone-1",
        ServerId = "server-1",
        MaxLength = 20,
        PrivateCooldownMs = 300,
        MaxHistoryFetch = 50,
    };
    var store = new ChatStore(redis, settings);

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

    Console.WriteLine("== 1. 频道定义 / key ==");
    var world = settings.FindChannel("world");
    Check(world != null && world.Scope == ChatScope.Zone && world.HistoryCount == 100, "找到 world 频道（全区）");
    Check(settings.FindChannel("GUILD")?.Id == "guild", "频道 id 大小写不敏感");
    Check(settings.FindChannel("nope") == null, "未知频道返回 null");
    Check(store.Keys.History("guild", "g1") == prefix + "chat:history:guild:g1", "历史 key 带频道 + 作用域");
    Check(store.Keys.Cooldown("u1", "guild") == prefix + "chat:cd:u1:guild", "冷却 key 带用户 + 频道");
    Check(store.Keys.Broadcast == prefix + "chat:broadcast", "广播通道名");

    Console.WriteLine("== 2. 内容清洗 ==");
    Check(settings.Sanitize("  hello  ") == "hello", "去掉首尾空白");
    Check(settings.Sanitize("a\u0001b\u0002c") == "abc", "丢掉控制字符");
    Check(settings.Sanitize("   ") == "", "全空白 → 空");
    Check(settings.Sanitize("12345678901234567890123").Length == 20, "超长截到 MaxLength");

    Console.WriteLine("== 3. 发言冷却 ==");
    Check(await store.TryAcquireCooldownAsync("u1", "guild", 300), "第一次发言拿到冷却锁");
    Check(!await store.TryAcquireCooldownAsync("u1", "guild", 300), "冷却期内第二次被拒");
    Check(await store.TryAcquireCooldownAsync("u2", "guild", 300), "不同用户互不影响");
    Check(await store.TryAcquireCooldownAsync("u1", "world", 300), "不同频道互不影响");
    Check(await store.TryAcquireCooldownAsync("u1", "guild", 0), "冷却为 0 时不限制");
    Check(await store.TryAcquirePrivateCooldownAsync("u1"), "私聊冷却首次通过");
    Check(!await store.TryAcquirePrivateCooldownAsync("u1"), "私聊冷却期内被拒");

    Console.WriteLine("== 4. 历史写入 / 裁剪 / 顺序 ==");
    static ChatEntry Entry(string id, string content, long time) =>
        new(id, "guild", "u1", "甲", "", content, time);

    for (var i = 1; i <= 5; i++)
    {
        await store.AppendHistoryAsync("guild", "g1", Entry("m" + i, "消息" + i, 1000 + i * 10), 3);
    }

    var history = await store.ReadHistoryAsync("guild", "g1", 10);
    Check(history.Count == 3, "超出保留条数后被裁剪到 3 条");
    Check(history[0].MessageId == "m3" && history[1].MessageId == "m4" && history[2].MessageId == "m5",
        "按时间正序返回（最旧在前）");
    Check(history[2].Content == "消息5" && history[2].SenderName == "甲", "内容与发送者昵称保留");

    var other = await store.ReadHistoryAsync("guild", "g2", 10);
    Check(other.Count == 0, "不同作用域的历史互相隔离");

    await store.AppendHistoryAsync("scene", "s1", Entry("x1", "不存", 1), 0);
    Check((await store.ReadHistoryAsync("scene", "s1", 10)).Count == 0, "HistoryCount=0 时不写历史");

    var limited = await store.ReadHistoryAsync("guild", "g1", 200);
    Check(limited.Count == 3, "读取条数被夹到上限内");

    Console.WriteLine("== 5. 跨 hub pub/sub 往返 ==");
    var received = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
    store.Subscribe(payload => received.TrySetResult(payload));

    // 订阅是异步生效的：给它一点时间，避免 publish 早于 subscribe 完成
    await Task.Delay(200);

    var message = System.Text.Encoding.UTF8.GetBytes("{\"channel_id\":\"guild\",\"scope_id\":\"g1\"}");
    var subscribers = await store.PublishAsync(message);
    Check(subscribers > 0, "至少有一个订阅者收到广播");

    var completed = await Task.WhenAny(received.Task, Task.Delay(3000));
    Check(completed == received.Task, "订阅回调收到了广播");
    Check((await received.Task).SequenceEqual(message), "广播内容原样送达");

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
