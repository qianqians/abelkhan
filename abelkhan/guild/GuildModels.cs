namespace guild;

/// <summary>帮会职位。数值越大权限越高（比较用），不要随意改动。</summary>
public enum GuildRole
{
    None = 0,
    Member = 1,
    Officer = 2,
    Leader = 3,
}

/// <summary>帮会静态信息（`guild:{g}:info:{guildId}` 这个 hash）。</summary>
public sealed record GuildInfo(
    string GuildId,
    string Name,
    string LeaderId,
    string Notice,
    int Level,
    long MemberCapExtra,
    long CreateTime);

/// <summary>帮会成员：userId + 职位。</summary>
public sealed record GuildMember(string UserId, GuildRole Role);

/// <summary>帮会任务的发布状态。</summary>
public static class GuildTaskStatus
{
    public const string Published = "published";
    public const string Finished = "finished";
    public const string Cancelled = "cancelled";
}

/// <summary>某个帮会里一条已发布的任务。</summary>
public sealed record GuildTaskState(
    string TaskId,
    string PublisherId,
    long PublishedAt,
    string Status = GuildTaskStatus.Published);

/// <summary>任务完成回调的载荷（可能在成员离线时触发，所以只给 id 不保证有人物实例）。</summary>
public sealed record GuildTaskCompletion(
    string GuildId,
    GuildTaskState State,
    GuildTaskDefinition Definition,
    string MemberId,
    long FinishedAt);

/// <summary>帮会条目的展示信息（昵称 / 头像），由游戏钩子填充。</summary>
public sealed record GuildDisplay(string Name = "", string Avatar = "");

/// <summary>帮会操作结果码。客户端按 code 做提示。</summary>
public static class GuildCodes
{
    public const string Ok = "ok";
    public const string AlreadyInGuild = "already_in_guild";
    public const string NotInGuild = "not_in_guild";
    public const string NameTaken = "name_taken";
    public const string NameInvalid = "name_invalid";
    public const string NotFound = "not_found";
    public const string MemberFull = "member_full";
    public const string OfficerFull = "officer_full";
    public const string NoPermission = "no_permission";
    public const string TargetInGuild = "target_in_guild";
    public const string TargetNotInGuild = "target_not_in_guild";
    public const string LeaderMustTransfer = "leader_must_transfer";
    public const string NoRequest = "no_request";
    public const string TaskUnknown = "task_unknown";
    public const string TaskPublished = "task_published";
    public const string TaskNotPublished = "task_not_published";
    public const string TaskNotAccepted = "task_not_accepted";
    public const string TaskAlreadyAccepted = "task_already_accepted";
    public const string ApplyFull = "apply_full";
    public const string InviteFull = "invite_full";
    public const string Invalid = "invalid";
    public const string Blocked = "blocked";
}

/// <summary>帮会操作结果。</summary>
public sealed record GuildResult(bool Ok, string Code = GuildCodes.Ok, string Message = "")
{
    public static GuildResult Success() => new(true);

    public static GuildResult Fail(string code, string message = "") => new(false, code, message);
}
