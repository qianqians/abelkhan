namespace friend;

/// <summary>一条好友（或申请）在协议层需要的展示数据。昵称 / 头像由游戏钩子填充。</summary>
public sealed record FriendDisplay(string Name = "", string Avatar = "");

/// <summary>好友条目：userId + 是否在线 + 展示信息。</summary>
public sealed record FriendEntry(string UserId, bool Online, FriendDisplay Display);

/// <summary>好友上限信息：已用 = 好友数 + 临时占位。</summary>
public sealed record FriendLimit(long Friends, long Reserved, long Extra, long Max)
{
    /// <summary>生效上限 = 全局默认 + 个人额外。</summary>
    public long Capacity => Max + Extra;

    /// <summary>有效占用 = 好友数 + 临时占位。</summary>
    public long Used => Friends + Reserved;

    public bool IsFull => Used >= Capacity;
}

/// <summary>好友操作的结果码。客户端按 code 做提示，message 只用于日志 / 兜底。</summary>
public static class FriendCodes
{
    public const string Ok = "ok";
    public const string Self = "self";
    public const string AlreadyFriend = "already_friend";
    public const string Duplicate = "duplicate";
    public const string NoRequest = "no_request";
    public const string InboxFull = "inbox_full";
    public const string TargetFull = "target_full";
    public const string RequesterFull = "requester_full";
    public const string SelfFull = "self_full";
    public const string NotFriend = "not_friend";
    public const string Invalid = "invalid";
}

/// <summary>好友操作结果。</summary>
public sealed record FriendResult(bool Ok, string Code = FriendCodes.Ok, string Message = "")
{
    public static FriendResult Success(string code = FriendCodes.Ok) => new(true, code);

    public static FriendResult Fail(string code, string message = "") => new(false, code, message);
}
