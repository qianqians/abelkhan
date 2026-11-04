using consts;
using engine;

namespace client;

/// <summary>
/// 帮会协议（创建 / 成员 / 职位 / 转让 / 解散 / 任务）与服务器推送的通知事件。
/// </summary>
public abstract partial class Player
{
    // ================================================================
    // 请求
    // ================================================================

    /// <summary>我的帮会：信息 / 成员 / 任务 + 我的进度 / 待审批数 / 待处理邀请。</summary>
    public Task<Result<ClientGuildInfoResult, string>> RequestGuildInfo()
    {
        return Request<ClientGuildInfo, ClientGuildInfoResult>(
            Consts.ClientGuildInfo,
            new ClientGuildInfo());
    }

    /// <summary>创建帮会（名字唯一、一人一会）。</summary>
    public Task<Result<ClientGuildCreateResult, string>> CreateGuild(string name)
    {
        return Request<ClientGuildCreate, ClientGuildCreateResult>(
            Consts.ClientGuildCreate,
            new ClientGuildCreate { Name = name });
    }

    /// <summary>改帮会名（仅帮主）。</summary>
    public Task<Result<ClientGuildRenameResult, string>> RenameGuild(string name)
    {
        return Request<ClientGuildRename, ClientGuildRenameResult>(
            Consts.ClientGuildRename,
            new ClientGuildRename { Name = name });
    }

    /// <summary>改帮会公告（仅帮主）。</summary>
    public Task<Result<ClientGuildNoticeResult, string>> SetGuildNotice(string notice)
    {
        return Request<ClientGuildNotice, ClientGuildNoticeResult>(
            Consts.ClientGuildNotice,
            new ClientGuildNotice { Notice = notice });
    }

    /// <summary>邀请玩家入帮（帮主 / 副帮主）。</summary>
    public Task<Result<ClientGuildInviteResult, string>> InviteToGuild(string userId)
    {
        return Request<ClientGuildInvite, ClientGuildInviteResult>(
            Consts.ClientGuildInvite,
            new ClientGuildInvite { UserId = userId });
    }

    /// <summary>申请加入某个帮会。</summary>
    public Task<Result<ClientGuildApplyResult, string>> ApplyToGuild(string guildId)
    {
        return Request<ClientGuildApply, ClientGuildApplyResult>(
            Consts.ClientGuildApply,
            new ClientGuildApply { GuildId = guildId });
    }

    /// <summary>待审批的申请列表（帮主 / 副帮主）。</summary>
    public Task<Result<ClientGuildAppliesResult, string>> RequestGuildApplies()
    {
        return Request<ClientGuildApplies, ClientGuildAppliesResult>(
            Consts.ClientGuildApplies,
            new ClientGuildApplies());
    }

    /// <summary>同意 / 拒绝一条入帮申请。</summary>
    public Task<Result<ClientGuildApproveResult, string>> ApproveGuildApply(string userId, bool accept)
    {
        return Request<ClientGuildApprove, ClientGuildApproveResult>(
            Consts.ClientGuildApprove,
            new ClientGuildApprove { UserId = userId, Accept = accept });
    }

    /// <summary>处理自己收到的邀请；accept = true 时返回新帮会信息。</summary>
    public Task<Result<ClientGuildInviteReplyResult, string>> ReplyGuildInvite(string guildId, bool accept)
    {
        return Request<ClientGuildInviteReply, ClientGuildInviteReplyResult>(
            Consts.ClientGuildInviteReply,
            new ClientGuildInviteReply { GuildId = guildId, Accept = accept });
    }

    /// <summary>退出帮会（帮主需要先转让或解散）。</summary>
    public Task<Result<ClientGuildLeaveResult, string>> LeaveGuild()
    {
        return Request<ClientGuildLeave, ClientGuildLeaveResult>(
            Consts.ClientGuildLeave,
            new ClientGuildLeave());
    }

    /// <summary>踢出成员（帮主 / 副帮主，不能踢同级或更高）。</summary>
    public Task<Result<ClientGuildKickResult, string>> KickFromGuild(string userId)
    {
        return Request<ClientGuildKick, ClientGuildKickResult>(
            Consts.ClientGuildKick,
            new ClientGuildKick { UserId = userId });
    }

    /// <summary>升 / 降副帮主（帮主操作）。role 只能填 1（成员）或 2（副帮主）。</summary>
    public Task<Result<ClientGuildSetRoleResult, string>> SetGuildRole(string userId, uint role)
    {
        return Request<ClientGuildSetRole, ClientGuildSetRoleResult>(
            Consts.ClientGuildSetRole,
            new ClientGuildSetRole { UserId = userId, Role = role });
    }

    /// <summary>转让帮主（老帮主降为副帮主）。</summary>
    public Task<Result<ClientGuildTransferResult, string>> TransferGuild(string userId)
    {
        return Request<ClientGuildTransfer, ClientGuildTransferResult>(
            Consts.ClientGuildTransfer,
            new ClientGuildTransfer { UserId = userId });
    }

    /// <summary>解散帮会（仅帮主）。</summary>
    public Task<Result<ClientGuildDisbandResult, string>> DisbandGuild()
    {
        return Request<ClientGuildDisband, ClientGuildDisbandResult>(
            Consts.ClientGuildDisband,
            new ClientGuildDisband());
    }

    /// <summary>发布帮会任务（帮主，或任务定义允许的副帮主）。</summary>
    public Task<Result<ClientGuildTaskPublishResult, string>> PublishGuildTask(string taskId)
    {
        return Request<ClientGuildTaskPublish, ClientGuildTaskPublishResult>(
            Consts.ClientGuildTaskPublish,
            new ClientGuildTaskPublish { TaskId = taskId });
    }

    /// <summary>取消帮会任务（仅帮主）。</summary>
    public Task<Result<ClientGuildTaskCancelResult, string>> CancelGuildTask(string taskId)
    {
        return Request<ClientGuildTaskCancel, ClientGuildTaskCancelResult>(
            Consts.ClientGuildTaskCancel,
            new ClientGuildTaskCancel { TaskId = taskId });
    }

    /// <summary>领取帮会任务。</summary>
    public Task<Result<ClientGuildTaskAcceptResult, string>> AcceptGuildTask(string taskId)
    {
        return Request<ClientGuildTaskAccept, ClientGuildTaskAcceptResult>(
            Consts.ClientGuildTaskAccept,
            new ClientGuildTaskAccept { TaskId = taskId });
    }

    // ================================================================
    // 通知
    // ================================================================

    /// <summary>帮会信息变了（成员 / 职位 / 公告 / 改名），收到后重新拉一次 client_guild_info。</summary>
    public event Action<ClientGuildUpdatedNotify>? GuildUpdated;

    /// <summary>收到帮会邀请。</summary>
    public event Action<ClientGuildInvitedNotify>? GuildInvited;

    /// <summary>被踢 / 帮会解散 / 主动退出（reason = kicked / disbanded / left）。</summary>
    public event Action<ClientGuildRemovedNotify>? GuildRemoved;

    /// <summary>帮会任务变化（发布 / 取消 / 进度 / 完成）。</summary>
    public event Action<ClientGuildTaskUpdatedNotify>? GuildTaskUpdated;

    private void RegisterGuildNotifications()
    {
        TryRegisterNotify<ClientGuildUpdatedNotify>(Consts.ClientGuildUpdatedNotify,
            msg => GuildUpdated?.Invoke(msg));
        TryRegisterNotify<ClientGuildInvitedNotify>(Consts.ClientGuildInvitedNotify,
            msg => GuildInvited?.Invoke(msg));
        TryRegisterNotify<ClientGuildRemovedNotify>(Consts.ClientGuildRemovedNotify,
            msg => GuildRemoved?.Invoke(msg));
        TryRegisterNotify<ClientGuildTaskUpdatedNotify>(Consts.ClientGuildTaskUpdatedNotify,
            msg => GuildTaskUpdated?.Invoke(msg));
    }
}
