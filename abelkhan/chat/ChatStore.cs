using core;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace chat;

/// <summary>
/// 聊天系统的 Redis 存储层：频道历史、发言冷却、跨 hub 广播。
///
/// 频道消息不落库、只保留最近 N 条（<see cref="ChatChannel.HistoryCount"/>），
/// 冷却用 <c>SET NX PX</c> 实现，天然跨进程/跨 hub 生效。
/// </summary>
public sealed class ChatStore
{
    private readonly RedisHandle _redis;
    private readonly ChatSettings _settings;

    public ChatStore(RedisHandle redis, ChatSettings settings)
    {
        _redis = redis;
        _settings = settings;
        Keys = new ChatKeys(settings.KeyPrefix);
    }

    public ChatSettings Settings => _settings;

    public ChatKeys Keys { get; }

    // ================================================================
    // 历史
    // ================================================================

    /// <summary>追加一条历史（最新在前），并把长度裁到 <paramref name="keep"/> 条。</summary>
    public async Task AppendHistoryAsync(string channelId, string scopeId, ChatEntry entry, int keep)
    {
        if (keep <= 0)
        {
            return;
        }

        var json = JsonConvert.SerializeObject(entry);
        await _redis.Execute("chat.history_append", db => db.ScriptEvaluateAsync(
            AppendHistoryScript,
            new RedisKey[] { Keys.History(channelId, scopeId) },
            new RedisValue[] { json, keep }));
    }

    /// <summary>
    /// 读取最近 <paramref name="count"/> 条，按时间正序返回（最旧在前，方便客户端直接追加显示）。
    /// </summary>
    public async Task<List<ChatEntry>> ReadHistoryAsync(string channelId, string scopeId, int count)
    {
        var limit = Math.Clamp(count, 1, Math.Max(1, _settings.MaxHistoryFetch));
        var values = await _redis.Execute("chat.history_read", db =>
            db.ListRangeAsync(Keys.History(channelId, scopeId), 0, limit - 1));

        var entries = new List<ChatEntry>(values.Length);
        // list 是最新在前，倒序塞进结果就是时间正序
        for (var i = values.Length - 1; i >= 0; i--)
        {
            var raw = values[i];
            if (!raw.HasValue || raw.IsNullOrEmpty)
            {
                continue;
            }

            try
            {
                var entry = JsonConvert.DeserializeObject<ChatEntry>(raw.ToString());
                if (entry != null)
                {
                    entries.Add(entry);
                }
            }
            catch (Exception e)
            {
                Log.Error("chat: decode history entry failed: {0}", e.Message);
            }
        }

        return entries;
    }

    // ================================================================
    // 冷却
    // ================================================================

    /// <summary>频道发言冷却：拿到锁（可以发言）返回 true。</summary>
    public Task<bool> TryAcquireCooldownAsync(string userId, string channelId, int cooldownMs)
    {
        return TryAcquireAsync(Keys.Cooldown(userId, channelId), cooldownMs, "chat.cooldown");
    }

    /// <summary>私聊冷却。</summary>
    public Task<bool> TryAcquirePrivateCooldownAsync(string userId)
    {
        return TryAcquireAsync(Keys.PrivateCooldown(userId), _settings.PrivateCooldownMs, "chat.cooldown_pm");
    }

    private async Task<bool> TryAcquireAsync(string key, int milliseconds, string operation)
    {
        if (milliseconds <= 0)
        {
            return true;
        }

        return await _redis.Execute(operation, db => db.StringSetAsync(
            key, "1", TimeSpan.FromMilliseconds(milliseconds), When.NotExists));
    }

    // ================================================================
    // 跨 hub 广播
    // ================================================================

    /// <summary>发布一条频道广播，返回收到它的 hub 数量。</summary>
    public ValueTask<long> PublishAsync(byte[] payload)
    {
        return _redis.Publish(Keys.Broadcast, payload);
    }

    /// <summary>订阅频道广播（每个 hub 启动时订阅一次；重连后由 RedisHandle 自动补回）。</summary>
    public void Subscribe(Action<byte[]> handler)
    {
        _redis.Subscribe(Keys.Broadcast, handler);
    }

    private const string AppendHistoryScript = """
        redis.call('LPUSH', KEYS[1], ARGV[1])
        local keep = tonumber(ARGV[2])
        if keep > 0 then
          redis.call('LTRIM', KEYS[1], 0, keep - 1)
        end
        return 1
        """;
}
