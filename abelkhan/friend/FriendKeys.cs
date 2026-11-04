namespace friend;

/// <summary>
/// 好友系统用到的 Redis key。
///
/// 所有好友相关的 key 共用 <c>{friend}</c> hash tag：Lua 脚本需要一次读写
/// 「双方好友列表 + 双方收件箱 + 占位」等 6~8 个 key，Redis Cluster 下必须落在同一 slot。
/// </summary>
public sealed class FriendKeys
{
    private readonly string _prefix;

    public FriendKeys(string? prefix = null)
    {
        prefix ??= string.Empty;
        if (prefix.Contains('{') || prefix.Contains('}'))
        {
            throw new ArgumentException("FriendSettings.KeyPrefix 不能包含 '{' 或 '}'（会破坏 hash tag）", nameof(prefix));
        }

        if (prefix.Length > 0 && !prefix.EndsWith(':'))
        {
            prefix += ":";
        }

        _prefix = prefix;
    }

    public string Prefix => _prefix;

    /// <summary>Lua 里按 userId 拼 key 用的公共前缀（以 ':' 结尾）。</summary>
    private string Base => _prefix + "friend:{friend}:";

    /// <summary>好友列表（权威）。member = 好友 userId。</summary>
    public string Friends(string userId) => Base + userId;

    /// <summary>收到的待处理申请 = 离线消息载体。</summary>
    public string Inbox(string userId) => Base + "inbox:" + userId;

    /// <summary>已发出的待处理申请。</summary>
    public string Sent(string userId) => Base + "sent:" + userId;

    /// <summary>个人额外好友位（string，默认 0）。</summary>
    public string Extra(string userId) => Base + "extra:" + userId;

    /// <summary>临时占位计数（同意后 +1，确认 / 回滚后 -1）。</summary>
    public string Reserved(string userId) => Base + "reserved:" + userId;

    /// <summary>占位明细：member = 对方 userId，score = 占位截止时间（毫秒）。</summary>
    public string Pending(string userId) => Base + "pending:" + userId;

    /// <summary>Lua 里需要按 userId 拼「好友列表」key 时用的前缀。</summary>
    public string FriendsPrefix => Base;

    /// <summary>在线标记：值为 gateId，TTL 由 gate 刷新。</summary>
    public string Presence(string userId) => _prefix + "presence:" + userId;

    /// <summary>玩家所在 hub 的路由。</summary>
    public string Route(string userId) => _prefix + "player_route:" + userId;

    /// <summary>某个 hub 的跨服指令队列。</summary>
    public string HubMq(string hubId) => _prefix + "hub:" + hubId + ":mq";

    /// <summary>离线创建 / 登录互斥的用户级锁。</summary>
    public string UserLock(string userId) => _prefix + "friend:lock:user:" + userId;
}
