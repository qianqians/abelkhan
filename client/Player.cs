namespace client;

/// <summary>
/// 客户端玩家实体：服务器通过 CreatePlayerEntity 下发的、属于这个客户端的实体
/// （对应 hub 侧的 hub.Player）。
///
/// 场景里的其它对象（别的玩家、怪物、掉落等）用 <see cref="Entity"/>。
/// 游戏侧派生这个类实现自己的玩家逻辑，框架保证 <see cref="Client.Player"/> 指向它。
/// </summary>
public abstract class Player(string entityId, string entityType) : Entity(entityId, entityType)
{
    /// <summary>当前连接 id（gate 下发的），等价于 Session.ConnId</summary>
    public string ConnId => Session?.ConnId ?? string.Empty;

    /// <summary>
    /// 重连用的 user_id。协议里的 CreatePlayerEntity 不带 user_id，
    /// 游戏可以在 OnCreate 里按自己的 argv 格式解析出来设置；
    /// 断线重连时客户端会拿它去发 ClientRequestReconnect。
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}
