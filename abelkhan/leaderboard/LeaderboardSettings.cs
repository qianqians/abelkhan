namespace leaderboard;

/// <summary>
/// 排行榜运行参数。hub 与（将来的）admin 必须使用同一份配置（尤其是 <see cref="KeyPrefix"/>），
/// 否则后台重置 / 查看的榜与游戏服不是同一个。
/// </summary>
public sealed class LeaderboardSettings
{
    /// <summary>Redis key 前缀。多游戏共用一个 Redis 时用它隔离，不能包含 '{' / '}'。</summary>
    public string KeyPrefix { get; set; } = string.Empty;

    /// <summary>每个榜最多保留多少名（打榜后裁剪，0 = 不裁剪）。</summary>
    public int MaxEntries { get; set; } = 1000;

    /// <summary>单次拉榜最多返回多少条（客户端请求区间会被夹到这个上限）。</summary>
    public int RangePageLimit { get; set; } = 100;
}
