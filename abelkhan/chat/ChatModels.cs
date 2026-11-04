namespace chat;

/// <summary>
/// 频道的作用域：消息群发给「这个作用域内」的所有玩家。
/// 前三个按玩家自己的数据解析（可能为空 = 玩家不在这个作用域里），
/// 后两个用 <see cref="ChatSettings"/> 里的固定 id。
/// </summary>
public enum ChatScope
{
    /// <summary>当前场景（含实例，例如 <c>scene:1001:3</c>）。</summary>
    Scene = 1,

    /// <summary>帮会。</summary>
    Guild = 2,

    /// <summary>门派。</summary>
    Sect = 3,

    /// <summary>全区。</summary>
    Zone = 4,

    /// <summary>全服。</summary>
    Server = 5,
}

/// <summary>
/// 一个聊天频道的定义（在代码里声明，见 <see cref="ChatSettings.Channels"/>）。
/// </summary>
/// <param name="Id">客户端使用的频道 id，如 <c>guild</c>。</param>
/// <param name="Name">展示名，如 <c>帮会</c>。</param>
/// <param name="Scope">作用域。</param>
/// <param name="CooldownMs">同频道发言间隔（毫秒），0 = 不限。</param>
/// <param name="HistoryCount">历史保留条数，0 = 不存历史。</param>
public sealed record ChatChannel(
    string Id,
    string Name,
    ChatScope Scope,
    int CooldownMs,
    int HistoryCount);

/// <summary>聊天条目的展示信息（昵称 / 头像），由游戏钩子填充。</summary>
public sealed record ChatDisplay(string Name = "", string Avatar = "");

/// <summary>一条聊天消息（频道消息或私聊）。发送时把昵称 / 头像冗余进去，接收方不用再解析。</summary>
public sealed record ChatEntry(
    string MessageId,
    string ChannelId,
    string SenderId,
    string SenderName,
    string SenderAvatar,
    string Content,
    long SendTime);

/// <summary>聊天操作的结果码。客户端按 code 做提示。</summary>
public static class ChatCodes
{
    public const string Ok = "ok";
    public const string UnknownChannel = "unknown_channel";
    public const string EmptyContent = "empty_content";
    public const string TooLong = "too_long";
    public const string Cooldown = "cooldown";
    public const string NotInScope = "not_in_scope";
    public const string SelfTarget = "self_target";
    public const string Offline = "offline";
    public const string Blocked = "blocked";
}

/// <summary>聊天操作结果。</summary>
public sealed record ChatResult(bool Ok, string Code = ChatCodes.Ok, string Message = "")
{
    public static ChatResult Success() => new(true);

    public static ChatResult Fail(string code, string message = "") => new(false, code, message);
}
