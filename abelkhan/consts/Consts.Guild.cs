namespace consts;

/// <summary>
/// 帮会协议方法名 + 通知（对应 proto/guild.proto）
/// </summary>
public partial class Consts
{
    // ---- 帮会（对应 guild.proto 的 ClientGuild* 消息） ----
    public const string ClientGuildInfo = "client_guild_info";
    public const string ClientGuildCreate = "client_guild_create";
    public const string ClientGuildRename = "client_guild_rename";
    public const string ClientGuildNotice = "client_guild_notice";
    public const string ClientGuildInvite = "client_guild_invite";
    public const string ClientGuildApply = "client_guild_apply";
    public const string ClientGuildApplies = "client_guild_applies";
    public const string ClientGuildApprove = "client_guild_approve";
    public const string ClientGuildInviteReply = "client_guild_invite_reply";
    public const string ClientGuildLeave = "client_guild_leave";
    public const string ClientGuildKick = "client_guild_kick";
    public const string ClientGuildSetRole = "client_guild_set_role";
    public const string ClientGuildTransfer = "client_guild_transfer";
    public const string ClientGuildDisband = "client_guild_disband";
    public const string ClientGuildTaskPublish = "client_guild_task_publish";
    public const string ClientGuildTaskCancel = "client_guild_task_cancel";
    public const string ClientGuildTaskAccept = "client_guild_task_accept";

    // 帮会通知（hub -> 客户端）
    public const string ClientGuildUpdatedNotify = "client_guild_updated_notify";
    public const string ClientGuildInvitedNotify = "client_guild_invited_notify";
    public const string ClientGuildRemovedNotify = "client_guild_removed_notify";
    public const string ClientGuildTaskUpdatedNotify = "client_guild_task_updated_notify";
}
