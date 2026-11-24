using core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace hub;

/// <summary>
/// 玩家会话的跨 hub 归属记录。
///
/// 单 hub 内存里的 <c>clients</c> 字典跨 hub 看不见，而顶号 / 迁移都必须回答
/// 「这个玩家现在归哪个 hub、哪条连接、哪个实体」，所以权威记录放 Redis：
///
/// <code>session:{userId} = { hub, gate, conn, entity, seq }   TTL 90s（在线时续期）</code>
///
/// <see cref="ClaimAsync"/> 用单条 Lua 做「读旧值 + seq+1 + 写新值」，跨 hub 并发登录时
/// 恰好有一个赢家拿到旧记录去踢人；<see cref="RefreshAsync"/> / <see cref="ReleaseAsync"/>
/// 都带 seq 比对，迟到的旧 hub 不会覆盖新会话。
/// </summary>
public sealed class SessionRegistry
{
    private readonly RedisHandle _redis;
    private readonly string _hubId;
    private readonly int _ttlSeconds;

    public SessionRegistry(RedisHandle redis, string hubId, int ttlSeconds = 90)
    {
        _redis = redis;
        _hubId = hubId;
        _ttlSeconds = Math.Max(5, ttlSeconds);
    }

    public string HubId => _hubId;

    public string Key(string userId) => "session:" + userId;

    // ================================================================
    // 读 / 占 / 续 / 释放
    // ================================================================

    /// <summary>当前会话记录；没有（或已过期）返回 null。</summary>
    public async Task<PlayerSession?> GetAsync(string userId)
    {
        var raw = await _redis.GetStrData(Key(userId));
        return Decode(raw.HasValue && !raw.IsNullOrEmpty ? raw.ToString() : null);
    }

    /// <summary>
    /// 登录占位：seq 自增并写入新会话，返回占位前的旧会话（没有则 null）与新 seq。
    /// 整条 Lua 原子，跨 hub 并发登录只有一个赢家看到同一个旧记录。
    /// </summary>
    public async Task<(PlayerSession? Previous, long Seq)> ClaimAsync(
        string userId, string gateName, string connId, string entityId)
    {
        var result = await _redis.Execute("session.claim", db => db.ScriptEvaluateAsync(
            ClaimScript,
            new RedisKey[] { Key(userId) },
            new RedisValue[] { _hubId, gateName, connId, entityId, _ttlSeconds }));

        var obj = JObject.Parse(result.ToString());
        var prev = Decode(obj.Value<string>("prev"));
        return (prev, obj.Value<long>("seq"));
    }

    /// <summary>在线续期；只有 seq 匹配（还是我这个会话）才续，防止旧 hub 给新会话续命。</summary>
    public Task<bool> RefreshAsync(string userId, long seq)
    {
        return CompareAsync("session.refresh", userId, seq, RefreshScript);
    }

    /// <summary>下线 / 摘除时释放；只有 seq 匹配才删，防止旧 hub 删掉新会话的记录。</summary>
    public Task<bool> ReleaseAsync(string userId, long seq)
    {
        return CompareAsync("session.release", userId, seq, ReleaseScript);
    }

    private async Task<bool> CompareAsync(string operation, string userId, long seq, string script)
    {
        var result = await _redis.Execute(operation, db => db.ScriptEvaluateAsync(
            script,
            new RedisKey[] { Key(userId) },
            new RedisValue[] { seq, _ttlSeconds }));

        return (long)result != 0;
    }

    // ================================================================
    // 编解码
    // ================================================================

    private static PlayerSession? Decode(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var obj = JObject.Parse(json);
            return new PlayerSession(
                obj.Value<string>("hub") ?? string.Empty,
                obj.Value<string>("gate") ?? string.Empty,
                obj.Value<string>("conn") ?? string.Empty,
                obj.Value<string>("entity") ?? string.Empty,
                obj.Value<long>("seq"));
        }
        catch (Exception e)
        {
            Log.Error("session: decode failed: {0}", e.Message);
            return null;
        }
    }

    // ================================================================
    // Lua
    // ================================================================

    private const string ClaimScript = """
        local key = KEYS[1]
        local prev = redis.call('GET', key)

        local seq = 1
        if prev then
          seq = tonumber(cjson.decode(prev).seq) + 1
        end

        redis.call('SET', key, cjson.encode({
          hub = ARGV[1], gate = ARGV[2], conn = ARGV[3], entity = ARGV[4], seq = seq,
        }), 'EX', tonumber(ARGV[5]))

        return cjson.encode({ prev = prev or '', seq = seq })
        """;

    private const string RefreshScript = """
        local cur = redis.call('GET', KEYS[1])
        if cur and tonumber(cjson.decode(cur).seq) == tonumber(ARGV[1]) then
          redis.call('EXPIRE', KEYS[1], tonumber(ARGV[2]))
          return 1
        end
        return 0
        """;

    private const string ReleaseScript = """
        local cur = redis.call('GET', KEYS[1])
        if cur and tonumber(cjson.decode(cur).seq) == tonumber(ARGV[1]) then
          redis.call('DEL', KEYS[1])
          return 1
        end
        return 0
        """;
}

/// <summary>一条会话归属记录。</summary>
public sealed record PlayerSession(string HubId, string GateName, string ConnId, string EntityId, long Seq);
