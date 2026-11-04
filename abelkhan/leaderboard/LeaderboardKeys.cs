namespace leaderboard;

/// <summary>
/// leaderboard 用到的 Redis key。每个榜是一个独立的 ZSET，单 key 操作，
/// 不需要跨 key 的 hash tag（打榜的 Lua 也只读写同一个榜的 key）。
/// </summary>
public sealed class LeaderboardKeys
{
    private readonly string _prefix;

    public LeaderboardKeys(string? prefix = null)
    {
        prefix ??= string.Empty;
        if (prefix.Contains('{') || prefix.Contains('}'))
        {
            throw new ArgumentException("LeaderboardSettings.KeyPrefix 不能包含 '{' 或 '}'", nameof(prefix));
        }

        if (prefix.Length > 0 && !prefix.EndsWith(':'))
        {
            prefix += ":";
        }

        _prefix = prefix;
    }

    public string Prefix => _prefix;

    /// <summary>某个榜的 ZSET key（member = userId，score = 分数）。</summary>
    public string Board(string boardId) => _prefix + "leaderboard:" + boardId;
}
