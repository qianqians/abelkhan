namespace friend;

/// <summary>
/// 好友系统运行参数。hub 必须与（将来的）admin 使用同一份 <see cref="KeyPrefix"/>，
/// 否则看到 / 重置的不是同一批数据。
/// </summary>
public sealed class FriendSettings
{
    /// <summary>Redis key 前缀。多游戏共用一个 Redis 时用它隔离，不能包含 '{' / '}'。</summary>
    public string KeyPrefix { get; set; } = string.Empty;

    /// <summary>全局默认好友上限；每玩家还可以有额外好友位（见 <see cref="FriendStore.GetExtraLimitAsync"/>）。</summary>
    public int MaxFriends { get; set; } = 50;

    /// <summary>待处理申请（收件箱）上限，防止被刷申请。</summary>
    public int MaxPendingRequests { get; set; } = 100;

    /// <summary>
    /// 同意好友后「临时占位」的有效期（毫秒）。跨服确认失败 / hub 崩溃时，
    /// 超期占位会被自动回收，避免好友位永久泄漏。
    /// </summary>
    public int ReserveTimeoutMs { get; set; } = 30_000;

    /// <summary>
    /// 同意好友时等待跨服确认回执的超时（毫秒）。正常路径只是一次 Redis / 队列往返，
    /// 这里设得比 <see cref="ReserveTimeoutMs"/> 短很多：同意请求在玩家实体的任务队列里同步等待，
    /// 等太久会拖住该玩家的其它请求。
    /// </summary>
    public int AcceptTimeoutMs { get; set; } = 5_000;

    /// <summary>在线标记（presence）的 TTL（毫秒），gate 每 3 秒刷新一次。</summary>
    public int PresenceTtlMs { get; set; } = 15_000;

    /// <summary>玩家所在 hub 的路由 TTL（毫秒），hub 定期刷新。</summary>
    public int RouteTtlMs { get; set; } = 60_000;
}
