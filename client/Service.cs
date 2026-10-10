namespace client;

/// <summary>
/// 实体工厂：游戏侧实现，把服务器下发的 entity_type 映射成具体的客户端实体。
/// 对应 hub 侧的 <c>hub.Service</c>。
///
/// 玩家自己的实体（CreatePlayerEntity）用 <see cref="Player"/>，
/// 场景里其它实体（CreateRemoteEntity）用 <see cref="Entity"/>。
/// </summary>
public abstract class Service
{
    /// <summary>
    /// 服务器为这个客户端创建了自己的玩家实体（CreatePlayerEntity）。
    /// argv 是服务器给的实体数据（hub 侧 ClientInfo() 的序列化结果，内容由游戏决定）。
    /// 只在本地还没有这个 entity_id 时调用；重连后服务器重新下发时走的是 OnRefresh。
    /// </summary>
    public abstract Player CreatePlayerEntity(string entityId, string entityType, byte[] argv);

    /// <summary>
    /// 服务器通知创建了别的实体（CreateRemoteEntity，例如其它玩家的实体）。
    /// 不关心远端实体时可以不实现（默认忽略）。
    /// </summary>
    public virtual Entity? CreateRemoteEntity(string entityId, string entityType, byte[] argv)
    {
        return null;
    }
}
