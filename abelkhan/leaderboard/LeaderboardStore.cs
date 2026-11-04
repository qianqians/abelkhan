using core;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace leaderboard;

/// <summary>
/// 排行榜 Redis 存储层。每个榜是一个独立的 ZSET（member = userId，score = 分数）。
///
/// 打榜用 Lua 在 Redis 内原子执行「ZADD GT + 裁剪 + ZREVRANK」，多 hub 进程并发时也只会
/// 保留每人的最高分（GT：新分低于历史最高分时不更新）；拉榜 / 查名次 / 重置都是单 key
/// 命令，天然线程安全。
/// </summary>
public sealed class LeaderboardStore
{
    private readonly RedisHandle _redis;
    private readonly LeaderboardSettings _settings;

    public LeaderboardStore(RedisHandle redis, LeaderboardSettings settings)
    {
        _redis = redis;
        _settings = settings;
        Keys = new LeaderboardKeys(settings.KeyPrefix);
    }

    public LeaderboardSettings Settings => _settings;

    public LeaderboardKeys Keys { get; }

    // ================================================================
    // 打榜
    // ================================================================

    /// <summary>
    /// 打榜：仅保留最高分（ZADD GT）。返回 (是否更新了榜, 当前名次 1-based；0 = 不在榜)。
    /// </summary>
    public async Task<(bool Changed, long Rank)> SubmitAsync(string boardId, string memberId, double score)
    {
        if (string.IsNullOrWhiteSpace(boardId))
        {
            throw new ArgumentException("boardId 不能为空", nameof(boardId));
        }

        if (string.IsNullOrWhiteSpace(memberId))
        {
            throw new ArgumentException("memberId 不能为空", nameof(memberId));
        }

        if (double.IsNaN(score) || double.IsInfinity(score))
        {
            throw new ArgumentException("score 非法", nameof(score));
        }

        var result = await _redis.Execute("leaderboard.submit", db => db.ScriptEvaluateAsync(
            SubmitScript,
            new RedisKey[] { Keys.Board(boardId) },
            new RedisValue[] { memberId, score, Math.Max(0, _settings.MaxEntries) }));

        var obj = JObject.Parse(result.ToString());
        var changed = obj.Value<long?>("changed") ?? 0;
        var rank = obj.Value<long?>("rank") ?? 0;
        return (changed != 0, rank);
    }

    // ================================================================
    // 拉榜 / 查名次
    // ================================================================

    /// <summary>拉取 [start, stop] 名次区间（start / stop 为 1-based 名次，闭区间）。</summary>
    public async Task<(List<LeaderboardEntry> Entries, long Total)> GetRangeAsync(
        string boardId, long start, long stop)
    {
        var key = Keys.Board(boardId);
        var total = await _redis.Execute("leaderboard.zcard",
            db => db.SortedSetLengthAsync(key));
        var values = await _redis.Execute("leaderboard.zrevrange",
            db => db.SortedSetRangeByRankWithScoresAsync(key, start - 1, stop - 1, Order.Descending));

        var entries = new List<LeaderboardEntry>(values.Length);
        var rank = start;
        foreach (var value in values)
        {
            entries.Add(new LeaderboardEntry(value.Element.ToString(), value.Score, rank++));
        }

        return (entries, total);
    }

    /// <summary>查询成员名次（1-based，0 = 不在榜）。</summary>
    public async Task<long> GetRankAsync(string boardId, string memberId)
    {
        var rank = await _redis.Execute("leaderboard.zrevrank",
            db => db.SortedSetRankAsync(Keys.Board(boardId), memberId, Order.Descending));
        return rank == null ? 0 : rank.Value + 1;
    }

    /// <summary>榜单总人数。</summary>
    public async Task<long> CountAsync(string boardId)
    {
        return await _redis.Execute("leaderboard.zcard_count",
            db => db.SortedSetLengthAsync(Keys.Board(boardId)));
    }

    // ================================================================
    // 重置
    // ================================================================

    /// <summary>重置（清空）整个榜。</summary>
    public async Task ResetAsync(string boardId)
    {
        await _redis.Execute("leaderboard.reset",
            db => db.KeyDeleteAsync(Keys.Board(boardId)));
    }

    // ================================================================
    // Lua：单 key，返回 cjson 编码的单个字符串
    // ================================================================

    private const string SubmitScript = """
        local key = KEYS[1]
        local member = ARGV[1]
        local score = tonumber(ARGV[2])
        local maxEntries = tonumber(ARGV[3])

        -- GT：仅当新分大于现有分（或成员不存在）时更新，实现「只保留最高分」
        local changed = redis.call('ZADD', key, 'GT', score, member)
        if maxEntries > 0 then
          -- 升序排名里 [0, -(N+1)] 是最低的 (总数-N) 名，删掉它们即只保留最高 N 名
          redis.call('ZREMRANGEBYRANK', key, 0, -maxEntries - 1)
        end
        local rank = redis.call('ZREVRANK', key, member)
        return cjson.encode({ changed = changed, rank = rank or 0 })
        """;
}
