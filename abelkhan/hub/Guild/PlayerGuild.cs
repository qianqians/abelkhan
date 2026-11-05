using chat;
using core;
using consts;
using engine;
using guild;

namespace hub;

/// <summary>
/// 玩家实体上的帮会协议（信息 / 创建 / 邀请 / 申请 / 职位 / 转让 / 解散 / 任务）与本地镜像。
/// 编排逻辑与通知在 <see cref="GuildService"/>，这里只做协议解析和结果拼装。
/// </summary>
public abstract partial class Player
{
    /// <summary>本地镜像：玩家当前所在帮会 id（不在帮会为 null）。以 Redis 为准，下次拉信息时自动校正。</summary>
    public string? GuildId { get; private set; }

    /// <summary>本地镜像：玩家在帮会里的职位。</summary>
    public GuildRole GuildRole { get; private set; } = GuildRole.None;

    internal void SetGuildSnapshot(string? guildId, GuildRole role)
    {
        GuildId = guildId;
        GuildRole = role;
    }

    private GuildService? _guildService;

    internal void RegisterGuildRequests(Service service)
    {
        var guildService = service.Guild;
        if (guildService == null)
        {
            return;
        }

        _guildService = guildService;
        var store = guildService.Store;
        var settings = store.Settings;

        // ---- 查询 ----
        RegisterRequest<ClientGuildInfo, ClientGuildInfoResult>(Consts.ClientGuildInfo, async _ =>
        {
            // 每次查询都从 Redis 重新校正一次本地镜像：入帮 / 被踢可能发生在别的 hub 上，
            // 靠这一下自愈（顺带修正帮会频道的作用域）。
            await guildService.ApplyMembershipAsync(this, await store.GetPlayerGuildIdAsync(UserId));

            var info = GuildId == null ? null : await store.GetInfoAsync(GuildId);
            if (info == null)
            {
                var empty = new ClientGuildInfoResult { InGuild = false };
                empty.PendingInvites = (uint)(await store.GetInvitesAsync(UserId)).Count;
                return Result<ClientGuildInfoResult, string>.Ok(empty);
            }

            return Result<ClientGuildInfoResult, string>.Ok(await guildService.BuildInfoAsync(this, info));
        });

        // ---- 创建 ----
        RegisterRequest<ClientGuildCreate, ClientGuildCreateResult>(Consts.ClientGuildCreate, async req =>
        {
            var reject = service.CheckGuildCreate(this, req.Name ?? string.Empty);
            if (!string.IsNullOrEmpty(reject))
            {
                return Result<ClientGuildCreateResult, string>.Err(reject);
            }

            var invalid = settings.NormalizeName(req.Name, out var name);
            if (invalid != null)
            {
                return Result<ClientGuildCreateResult, string>.Err(GuildMessageOf(invalid));
            }

            var guildId = GuildStore.NewGuildId();
            var created = await store.CreateAsync(guildId, name, UserId);
            if (!created.Ok)
            {
                return Result<ClientGuildCreateResult, string>.Err(GuildMessageOf(created.Code));
            }

            Metrics.Count("abelkhan_guild_created_total", "Guilds created");

            // 新帮会：自动发布配置里的默认任务（目录里不存在或等级不够的跳过）
            await PublishDefaultTasksAsync(guildService, guildId);

            await guildService.ApplyMembershipAsync(this, guildId);
            var info = await store.GetInfoAsync(guildId);
            return Result<ClientGuildCreateResult, string>.Ok(new ClientGuildCreateResult
            {
                Guild = info == null
                    ? new ClientGuildInfoResult { InGuild = false }
                    : await guildService.BuildInfoAsync(this, info),
            });
        });

        // ---- 帮会信息维护 ----
        RegisterRequest<ClientGuildRename, ClientGuildRenameResult>(Consts.ClientGuildRename, async req =>
        {
            if (GuildId == null)
            {
                return Result<ClientGuildRenameResult, string>.Err(GuildMessageOf(GuildCodes.NotInGuild));
            }

            var invalid = settings.NormalizeName(req.Name, out var name);
            if (invalid != null)
            {
                return Result<ClientGuildRenameResult, string>.Err(GuildMessageOf(invalid));
            }

            var result = await store.RenameAsync(GuildId, UserId, name);
            if (!result.Ok)
            {
                return Result<ClientGuildRenameResult, string>.Err(GuildMessageOf(result.Code));
            }

            await guildService.NotifyGuildUpdatedAsync(GuildId, "rename");
            return Result<ClientGuildRenameResult, string>.Ok(new ClientGuildRenameResult());
        });

        RegisterRequest<ClientGuildNotice, ClientGuildNoticeResult>(Consts.ClientGuildNotice, async req =>
        {
            if (GuildId == null)
            {
                return Result<ClientGuildNoticeResult, string>.Err(GuildMessageOf(GuildCodes.NotInGuild));
            }

            var notice = (req.Notice ?? string.Empty).Trim();
            var max = Math.Max(1, settings.NoticeMaxLength);
            if (notice.Length > max)
            {
                notice = notice[..max];
            }

            var result = await store.SetNoticeAsync(GuildId, UserId, notice);
            if (!result.Ok)
            {
                return Result<ClientGuildNoticeResult, string>.Err(GuildMessageOf(result.Code));
            }

            await guildService.NotifyGuildUpdatedAsync(GuildId, "notice");
            return Result<ClientGuildNoticeResult, string>.Ok(new ClientGuildNoticeResult());
        });

        // ---- 邀请 / 申请 / 审批 ----
        RegisterRequest<ClientGuildInvite, ClientGuildInviteResult>(Consts.ClientGuildInvite, async req =>
        {
            if (GuildId == null)
            {
                return Result<ClientGuildInviteResult, string>.Err(GuildMessageOf(GuildCodes.NotInGuild));
            }

            if (string.Equals(req.UserId, UserId, StringComparison.Ordinal))
            {
                return Result<ClientGuildInviteResult, string>.Err(GuildMessageOf(GuildCodes.Invalid));
            }

            var result = await store.InviteAsync(GuildId, UserId, req.UserId);
            if (!result.Ok)
            {
                return Result<ClientGuildInviteResult, string>.Err(GuildMessageOf(result.Code));
            }

            var info = await store.GetInfoAsync(GuildId);
            await guildService.NotifyUserAsync(req.UserId, Consts.ClientGuildInvitedNotify,
                new ClientGuildInvitedNotify
                {
                    GuildId = GuildId,
                    GuildName = info?.Name ?? string.Empty,
                    InviterId = UserId,
                });

            return Result<ClientGuildInviteResult, string>.Ok(new ClientGuildInviteResult());
        });

        RegisterRequest<ClientGuildApply, ClientGuildApplyResult>(Consts.ClientGuildApply, async req =>
        {
            var result = await store.ApplyAsync(req.GuildId, UserId);
            if (!result.Ok)
            {
                return Result<ClientGuildApplyResult, string>.Err(GuildMessageOf(result.Code));
            }

            await guildService.NotifyGuildUpdatedAsync(req.GuildId, "apply");
            return Result<ClientGuildApplyResult, string>.Ok(new ClientGuildApplyResult());
        });

        RegisterRequest<ClientGuildApplies, ClientGuildAppliesResult>(Consts.ClientGuildApplies, async _ =>
        {
            if (GuildId == null)
            {
                return Result<ClientGuildAppliesResult, string>.Err(GuildMessageOf(GuildCodes.NotInGuild));
            }

            if (await store.GetRoleAsync(GuildId, UserId) < GuildRole.Officer)
            {
                return Result<ClientGuildAppliesResult, string>.Err(GuildMessageOf(GuildCodes.NoPermission));
            }

            var applies = await store.GetAppliesAsync(GuildId);
            var ids = applies.Keys.ToList();
            var online = guildService.ResolveOnlineEntityIds == null
                ? new Dictionary<string, string>()
                : await guildService.ResolveOnlineEntityIds(ids);
            var display = await guildService.ResolveDisplayForAsync(ids);

            var result = new ClientGuildAppliesResult();
            foreach (var (userId, _) in applies.OrderBy(kv => kv.Value))
            {
                var item = new ClientGuildMemberInfo
                {
                    UserId = userId,
                    Role = (uint)GuildRole.None,
                    Online = online.ContainsKey(userId),
                };
                if (display.TryGetValue(userId, out var d))
                {
                    item.DisplayName = d.Name;
                    item.Avatar = d.Avatar;
                }

                result.Applies.Add(item);
            }

            return Result<ClientGuildAppliesResult, string>.Ok(result);
        });

        RegisterRequest<ClientGuildApprove, ClientGuildApproveResult>(Consts.ClientGuildApprove, async req =>
        {
            if (GuildId == null)
            {
                return Result<ClientGuildApproveResult, string>.Err(GuildMessageOf(GuildCodes.NotInGuild));
            }

            var result = await store.ApproveAsync(GuildId, UserId, req.UserId, req.Accept);
            if (!result.Ok)
            {
                return Result<ClientGuildApproveResult, string>.Err(GuildMessageOf(result.Code));
            }

            if (req.Accept)
            {
                // 申请人可能在别的 hub 上：发通知让他客户端拉一次信息（那边会自愈镜像与聊天作用域）
                await guildService.NotifyUserAsync(req.UserId, Consts.ClientGuildUpdatedNotify,
                    new ClientGuildUpdatedNotify { Reason = "member" });
            }

            await guildService.NotifyGuildUpdatedAsync(GuildId, "member");
            return Result<ClientGuildApproveResult, string>.Ok(new ClientGuildApproveResult());
        });

        RegisterRequest<ClientGuildInviteReply, ClientGuildInviteReplyResult>(Consts.ClientGuildInviteReply,
            async req =>
            {
                if (!req.Accept)
                {
                    var rejected = await store.RejectInviteAsync(UserId, req.GuildId);
                    return rejected.Ok
                        ? Result<ClientGuildInviteReplyResult, string>.Ok(new ClientGuildInviteReplyResult())
                        : Result<ClientGuildInviteReplyResult, string>.Err(GuildMessageOf(rejected.Code));
                }

                var result = await store.AcceptInviteAsync(req.GuildId, UserId);
                if (!result.Ok)
                {
                    return Result<ClientGuildInviteReplyResult, string>.Err(GuildMessageOf(result.Code));
                }

                await guildService.ApplyMembershipAsync(this, req.GuildId);
                await guildService.NotifyGuildUpdatedAsync(req.GuildId, "member");

                var info = await store.GetInfoAsync(req.GuildId);
                return Result<ClientGuildInviteReplyResult, string>.Ok(new ClientGuildInviteReplyResult
                {
                    Guild = info == null
                        ? new ClientGuildInfoResult { InGuild = false }
                        : await guildService.BuildInfoAsync(this, info),
                });
            });

        // ---- 退出 / 踢人 / 职位 / 转让 / 解散 ----
        RegisterRequest<ClientGuildLeave, ClientGuildLeaveResult>(Consts.ClientGuildLeave, async _ =>
        {
            if (GuildId == null)
            {
                return Result<ClientGuildLeaveResult, string>.Err(GuildMessageOf(GuildCodes.NotInGuild));
            }

            var guildId = GuildId;
            var result = await store.LeaveAsync(guildId, UserId);
            if (!result.Ok)
            {
                return Result<ClientGuildLeaveResult, string>.Err(GuildMessageOf(result.Code));
            }

            await guildService.ApplyMembershipAsync(this, null);
            await guildService.NotifyGuildUpdatedAsync(guildId, "member", UserId);
            await guildService.NotifyUserAsync(UserId, Consts.ClientGuildRemovedNotify,
                new ClientGuildRemovedNotify { GuildId = guildId, Reason = "left" });

            return Result<ClientGuildLeaveResult, string>.Ok(new ClientGuildLeaveResult());
        });

        RegisterRequest<ClientGuildKick, ClientGuildKickResult>(Consts.ClientGuildKick, async req =>
        {
            if (GuildId == null)
            {
                return Result<ClientGuildKickResult, string>.Err(GuildMessageOf(GuildCodes.NotInGuild));
            }

            var guildId = GuildId;
            var result = await store.KickAsync(guildId, UserId, req.UserId);
            if (!result.Ok)
            {
                return Result<ClientGuildKickResult, string>.Err(GuildMessageOf(result.Code));
            }

            await guildService.NotifyUserAsync(req.UserId, Consts.ClientGuildRemovedNotify,
                new ClientGuildRemovedNotify { GuildId = guildId, Reason = "kicked" });
            await guildService.NotifyGuildUpdatedAsync(guildId, "member", req.UserId);

            return Result<ClientGuildKickResult, string>.Ok(new ClientGuildKickResult());
        });

        RegisterRequest<ClientGuildSetRole, ClientGuildSetRoleResult>(Consts.ClientGuildSetRole, async req =>
        {
            if (GuildId == null)
            {
                return Result<ClientGuildSetRoleResult, string>.Err(GuildMessageOf(GuildCodes.NotInGuild));
            }

            var result = await store.SetRoleAsync(GuildId, UserId, req.UserId, (GuildRole)req.Role);
            if (!result.Ok)
            {
                return Result<ClientGuildSetRoleResult, string>.Err(GuildMessageOf(result.Code));
            }

            await guildService.NotifyGuildUpdatedAsync(GuildId, "role");
            return Result<ClientGuildSetRoleResult, string>.Ok(new ClientGuildSetRoleResult());
        });

        RegisterRequest<ClientGuildTransfer, ClientGuildTransferResult>(Consts.ClientGuildTransfer, async req =>
        {
            if (GuildId == null)
            {
                return Result<ClientGuildTransferResult, string>.Err(GuildMessageOf(GuildCodes.NotInGuild));
            }

            var guildId = GuildId;
            var result = await store.TransferAsync(guildId, UserId, req.UserId);
            if (!result.Ok)
            {
                return Result<ClientGuildTransferResult, string>.Err(GuildMessageOf(result.Code));
            }

            // 自己的职位从帮主降成副帮主了，本地镜像要跟着变
            await guildService.ApplyMembershipAsync(this, guildId);
            await guildService.NotifyGuildUpdatedAsync(guildId, "role");
            return Result<ClientGuildTransferResult, string>.Ok(new ClientGuildTransferResult());
        });

        RegisterRequest<ClientGuildDisband, ClientGuildDisbandResult>(Consts.ClientGuildDisband, async _ =>
        {
            if (GuildId == null)
            {
                return Result<ClientGuildDisbandResult, string>.Err(GuildMessageOf(GuildCodes.NotInGuild));
            }

            var guildId = GuildId;
            var (result, members) = await store.DisbandAsync(guildId, UserId);
            if (!result.Ok)
            {
                return Result<ClientGuildDisbandResult, string>.Err(GuildMessageOf(result.Code));
            }

            await guildService.NotifyUsersAsync(members, Consts.ClientGuildRemovedNotify,
                new ClientGuildRemovedNotify { GuildId = guildId, Reason = "disbanded" });
            await guildService.ApplyMembershipAsync(this, null);

            return Result<ClientGuildDisbandResult, string>.Ok(new ClientGuildDisbandResult());
        });

        // ---- 帮会任务 ----
        RegisterRequest<ClientGuildTaskPublish, ClientGuildTaskPublishResult>(Consts.ClientGuildTaskPublish,
            async req =>
            {
                if (GuildId == null)
                {
                    return Result<ClientGuildTaskPublishResult, string>.Err(
                        GuildMessageOf(GuildCodes.NotInGuild));
                }

                if (!guildService.Catalog.TryGet(req.TaskId, out var definition) || definition == null)
                {
                    return Result<ClientGuildTaskPublishResult, string>.Err(
                        GuildMessageOf(GuildCodes.TaskUnknown));
                }

                var info = await store.GetInfoAsync(GuildId);
                if (info != null && info.Level < definition.MinGuildLevel)
                {
                    return Result<ClientGuildTaskPublishResult, string>.Err(
                        GuildMessageOf(GuildCodes.NoPermission));
                }

                var state = new GuildTaskState(req.TaskId, UserId,
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                var result = await store.PublishTaskAsync(GuildId, UserId, state, definition.OfficerCanPublish);
                if (!result.Ok)
                {
                    return Result<ClientGuildTaskPublishResult, string>.Err(GuildMessageOf(result.Code));
                }

                await guildService.NotifyTaskUpdatedAsync(GuildId, req.TaskId);
                return Result<ClientGuildTaskPublishResult, string>.Ok(new ClientGuildTaskPublishResult());
            });

        RegisterRequest<ClientGuildTaskCancel, ClientGuildTaskCancelResult>(Consts.ClientGuildTaskCancel,
            async req =>
            {
                if (GuildId == null)
                {
                    return Result<ClientGuildTaskCancelResult, string>.Err(
                        GuildMessageOf(GuildCodes.NotInGuild));
                }

                var guildId = GuildId;
                var result = await store.CancelTaskAsync(guildId, UserId, req.TaskId);
                if (!result.Ok)
                {
                    return Result<ClientGuildTaskCancelResult, string>.Err(GuildMessageOf(result.Code));
                }

                // 任务被删掉了，客户端按 task_id 清掉自己那份即可
                await guildService.NotifyUsersAsync((await store.GetMembersAsync(guildId)).Keys.ToList(),
                    Consts.ClientGuildTaskUpdatedNotify,
                    new ClientGuildTaskUpdatedNotify
                    {
                        Task = new ClientGuildTaskInfo { TaskId = req.TaskId, Status = GuildTaskStatus.Cancelled },
                    });

                return Result<ClientGuildTaskCancelResult, string>.Ok(new ClientGuildTaskCancelResult());
            });

        RegisterRequest<ClientGuildTaskAccept, ClientGuildTaskAcceptResult>(Consts.ClientGuildTaskAccept,
            async req =>
            {
                if (GuildId == null)
                {
                    return Result<ClientGuildTaskAcceptResult, string>.Err(
                        GuildMessageOf(GuildCodes.NotInGuild));
                }

                var result = await store.AcceptTaskAsync(GuildId, UserId, req.TaskId);
                if (!result.Ok)
                {
                    return Result<ClientGuildTaskAcceptResult, string>.Err(GuildMessageOf(result.Code));
                }

                await guildService.NotifyUserAsync(UserId, Consts.ClientGuildTaskUpdatedNotify,
                    new ClientGuildTaskUpdatedNotify
                    {
                        Task = await guildService.BuildTaskInfoAsync(GuildId, req.TaskId, UserId) ??
                               new ClientGuildTaskInfo { TaskId = req.TaskId },
                    });

                return Result<ClientGuildTaskAcceptResult, string>.Ok(new ClientGuildTaskAcceptResult());
            });
    }

    /// <summary>新帮会创建后自动发布配置里的默认任务。</summary>
    private async Task PublishDefaultTasksAsync(GuildService guildService, string guildId)
    {
        foreach (var taskId in guildService.Store.Settings.DefaultTaskIds)
        {
            if (!guildService.Catalog.TryGet(taskId, out var definition) || definition == null)
            {
                Log.Error("guild: 默认任务 {0} 不在任务目录里，已跳过", taskId);
                continue;
            }

            var state = new GuildTaskState(taskId, UserId,
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            var result = await guildService.Store.PublishTaskAsync(guildId, UserId, state,
                definition.OfficerCanPublish);
            if (!result.Ok)
            {
                Log.Error("guild: 自动发布默认任务 {0} 失败:{1}", taskId, result.Code);
            }
        }
    }

    private static string GuildMessageOf(string code) => code switch
    {
        GuildCodes.AlreadyInGuild => "你已经在帮会里了",
        GuildCodes.NotInGuild => "你还没有帮会",
        GuildCodes.NameTaken => "这个帮会名已经被占用了",
        GuildCodes.NameInvalid => "帮会名不合法（长度或字符）",
        GuildCodes.NotFound => "帮会不存在",
        GuildCodes.MemberFull => "帮会人数已满",
        GuildCodes.OfficerFull => "副帮主名额已满",
        GuildCodes.NoPermission => "你没有这个权限",
        GuildCodes.TargetInGuild => "对方已经有帮会了",
        GuildCodes.TargetNotInGuild => "对方不在这个帮会里",
        GuildCodes.LeaderMustTransfer => "帮主需要先转让或解散帮会",
        GuildCodes.NoRequest => "找不到这条申请 / 邀请",
        GuildCodes.TaskUnknown => "未知的帮会任务",
        GuildCodes.TaskPublished => "这个任务已经发布过了",
        GuildCodes.TaskNotPublished => "这个任务没有发布",
        GuildCodes.TaskNotAccepted => "你还没有领取这个任务",
        GuildCodes.TaskAlreadyAccepted => "你已经领取过这个任务了",
        GuildCodes.ApplyFull => "该帮会的申请列表已满",
        GuildCodes.InviteFull => "对方的邀请列表已满",
        GuildCodes.Invalid => "参数不合法",
        GuildCodes.Blocked => "操作被拒绝",
        _ => "帮会操作失败：" + code,
    };
}
