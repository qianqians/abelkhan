using System.Collections.Concurrent;
using chat;
using consts;
using core;
using engine;
using Google.Protobuf;
using guild;

namespace hub;

/// <summary>
/// 帮会系统的编排层（hub 侧）。
///
/// 数据以 Redis 为权威（<see cref="GuildStore"/> 的 Lua 原子操作保证并发下不会超员 / 双帮主），
/// 本类负责：
///   - 本 hub 的「玩家 → 帮会」缓存（帮会频道作用域、通知用）；
///   - 帮会变动通知（按 presence 找到在线成员，走 <c>entity_{userId}_client_mq</c>，可跨 hub）；
///   - 帮会任务的进度上报与完成回调（进度/奖励都归游戏，框架只管状态机）；
///   - 帮会频道：把帮会 id 喂给聊天系统的作用域解析器。
/// </summary>
public sealed class GuildService
{
    private readonly GuildStore _store;
    private readonly RedisHandle _redis;
    private readonly WRpc _rpc = new();

    /// <summary>本 hub 在线的玩家：userId → Player。</summary>
    private readonly ConcurrentDictionary<string, Player> _players = new();

    /// <summary>本 hub 玩家的帮会缓存：userId → guildId（不在帮会则无此项）。</summary>
    private readonly ConcurrentDictionary<string, string> _guildOf = new();

    public GuildService(GuildStore store, RedisHandle redis)
    {
        _store = store;
        _redis = redis;
    }

    public GuildStore Store => _store;

    /// <summary>帮会任务目录（游戏注册的默认任务）。由 MainClass 从 Service 注入。</summary>
    public IGuildTaskCatalog Catalog { get; set; } = GuildTaskCatalog.Empty;

    /// <summary>帮会成员列表里的展示信息解析（昵称 / 头像）。</summary>
    public Func<IReadOnlyCollection<string>, Task<IReadOnlyDictionary<string, GuildDisplay>>>? DisplayResolver
    {
        get;
        set;
    }

    /// <summary>任务完成回调（由 MainClass 从 Service 钩子注入）。</summary>
    public Func<GuildTaskCompletion, Task>? OnTaskCompleted { get; set; }

    /// <summary>批量查在线玩家：userId → 实体 id。由 MainClass 注入（复用好友系统的 presence）。</summary>
    public Func<IReadOnlyCollection<string>, Task<IReadOnlyDictionary<string, string>>>? ResolveOnlineEntityIds
    {
        get;
        set;
    }

    // ================================================================
    // 上下线
    // ================================================================

    public async Task OnOnlineAsync(Player player)
    {
        _players[player.UserId] = player;
        var guildId = await _store.GetPlayerGuildIdAsync(player.UserId);
        await ApplyMembershipAsync(player, guildId);
    }

    public Task OnOfflineAsync(string userId)
    {
        _players.TryRemove(userId, out _);
        _guildOf.TryRemove(userId, out _);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 玩家所在帮会发生变化（入帮 / 退帮 / 被踢 / 解散）后同步本地状态：
    /// 玩家身上的镜像 + 聊天系统的帮会作用域。
    /// </summary>
    public async Task ApplyMembershipAsync(Player player, string? guildId)
    {
        var role = GuildRole.None;
        if (!string.IsNullOrEmpty(guildId))
        {
            role = await _store.GetRoleAsync(guildId, player.UserId);
            if (role == GuildRole.None)
            {
                // 索引和成员表不一致（理论上不会发生）：以成员表为准
                guildId = null;
            }
        }

        if (string.IsNullOrEmpty(guildId))
        {
            _guildOf.TryRemove(player.UserId, out _);
        }
        else
        {
            _guildOf[player.UserId] = guildId;
        }

        player.SetGuildSnapshot(guildId, role);
        // 帮会频道：作用域同步给聊天系统（null = 退出帮会频道）
        player.SetChatScope(ChatScope.Guild, guildId);
    }

    /// <summary>帮会频道的作用域解析（供聊天系统调用）。</summary>
    public string? ScopeOf(string userId) => _guildOf.GetValueOrDefault(userId);

    // ================================================================
    // 任务进度（游戏调用）
    // ================================================================

    /// <summary>
    /// 累加任务进度。到 Target 时把任务置为 finished 并回调
    /// <see cref="Service.OnGuildTaskCompletedAsync"/>。返回最新进度与是否刚好完成。
    /// </summary>
    public async Task<(long Progress, bool Completed)> AddTaskProgressAsync(string guildId, string taskId,
        string memberId, long delta)
    {
        var state = await _store.GetTaskAsync(guildId, taskId);
        if (state == null)
        {
            return (0, false);
        }

        if (state.Status != GuildTaskStatus.Published)
        {
            return (0, false);
        }

        Catalog.TryGet(taskId, out var definition);
        var target = definition?.NormalizedTarget ?? 1;
        var shared = definition is { PerMember: false };

        var (result, progress, completed) = await _store.AddTaskProgressAsync(
            guildId, taskId, memberId, delta, target, shared);
        if (!result.Ok)
        {
            return (progress, false);
        }

        if (completed)
        {
            var finished = state with { Status = GuildTaskStatus.Finished };
            await _store.MarkTaskFinishedAsync(guildId, taskId, finished);
            await OnTaskCompletedAsync(guildId, finished, definition, memberId);
        }

        await NotifyTaskUpdatedAsync(guildId, taskId);
        return (progress, completed);
    }

    private async Task OnTaskCompletedAsync(string guildId, GuildTaskState state,
        GuildTaskDefinition? definition, string memberId)
    {
        if (definition == null)
        {
            return;
        }

        var completion = new GuildTaskCompletion(guildId, state, definition, memberId,
            DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        try
        {
            if (OnTaskCompleted != null)
            {
                await OnTaskCompleted(completion);
            }
        }
        catch (Exception e)
        {
            Log.Error("guild: task completed hook err:{0}", e);
        }
    }

    // ================================================================
    // 通知
    // ================================================================

    /// <summary>通知帮会全体在线成员「帮会信息变了，客户端该刷新」。</summary>
    public async Task NotifyGuildUpdatedAsync(string guildId, string reason, string? excludeUserId = null)
    {
        var members = await _store.GetMembersAsync(guildId);
        var targets = members.Keys.Where(id => id != excludeUserId).ToList();
        await NotifyUsersAsync(targets, Consts.ClientGuildUpdatedNotify,
            new ClientGuildUpdatedNotify { Reason = reason });
    }

    /// <summary>通知某个帮会的全体在线成员某条任务变了（发布 / 取消 / 进度 / 完成）。</summary>
    public async Task NotifyTaskUpdatedAsync(string guildId, string taskId)
    {
        var info = await BuildTaskInfoAsync(guildId, taskId, null);
        if (info == null)
        {
            return;
        }

        var members = await _store.GetMembersAsync(guildId);
        await NotifyUsersAsync(members.Keys.ToList(), Consts.ClientGuildTaskUpdatedNotify,
            new ClientGuildTaskUpdatedNotify { Task = info });
    }

    public Task NotifyUserAsync<T>(string userId, string method, T msg) where T : IMessage<T>, new()
    {
        return NotifyUsersAsync(new[] { userId }, method, msg);
    }

    /// <summary>批量推给在线玩家：一次 resolve 出实体 id，再逐个投递（可跨 hub）。</summary>
    public async Task NotifyUsersAsync<T>(IReadOnlyCollection<string> userIds, string method, T msg)
        where T : IMessage<T>, new()
    {
        if (userIds.Count == 0 || ResolveOnlineEntityIds == null)
        {
            return;
        }

        var online = await ResolveOnlineEntityIds(userIds);
        if (online.Count == 0)
        {
            return;
        }

        var call = new CallRpc
        {
            ProtoName = method,
            Content = msg.ToByteString(),
        };

        foreach (var (userId, entityId) in online)
        {
            var forward = new GateForwardHubNotifyClientMq
            {
                UserId = userId,
                EntityId = entityId,
                Event = call,
            };

            await _redis.PushList(string.Format(Consts.EntityClientMq, userId),
                _rpc.Notify(Consts.GateForwardHubNotifyClientMq, forward));
        }
    }

    // ================================================================
    // 组装
    // ================================================================

    /// <summary>组装一份完整的帮会信息（成员 / 任务 / 我收到的邀请数）。</summary>
    public async Task<ClientGuildInfoResult> BuildInfoAsync(Player player, GuildInfo info)
    {
        var members = await _store.GetMembersAsync(info.GuildId);
        var memberIds = members.Keys.ToList();
        var online = ResolveOnlineEntityIds == null
            ? new Dictionary<string, string>()
            : await ResolveOnlineEntityIds(memberIds);
        var display = await ResolveDisplayAsync(memberIds);

        var reply = new ClientGuildInfoResult
        {
            InGuild = true,
            GuildId = info.GuildId,
            Name = info.Name,
            LeaderId = info.LeaderId,
            Notice = info.Notice,
            MyRole = (uint)(int)members.GetValueOrDefault(player.UserId, GuildRole.None),
            MemberCount = (uint)members.Count,
            MemberCapacity = (uint)Math.Max(0, _store.Settings.MaxMembers + info.MemberCapExtra),
            CreateTime = (ulong)Math.Max(0, info.CreateTime),
        };

        foreach (var (userId, role) in members)
        {
            var item = new ClientGuildMemberInfo
            {
                UserId = userId,
                Role = (uint)(int)role,
                Online = online.ContainsKey(userId),
            };
            if (display.TryGetValue(userId, out var d))
            {
                item.DisplayName = d.Name;
                item.Avatar = d.Avatar;
            }

            reply.Members.Add(item);
        }

        var tasks = await _store.GetTasksAsync(info.GuildId);
        foreach (var taskId in tasks.Keys.OrderBy(k => k, StringComparer.Ordinal))
        {
            var taskInfo = await BuildTaskInfoAsync(info.GuildId, taskId, player.UserId);
            if (taskInfo != null)
            {
                reply.Tasks.Add(taskInfo);
            }
        }

        if (reply.MyRole >= (uint)GuildRole.Officer)
        {
            reply.PendingApplies = (uint)(await _store.GetAppliesAsync(info.GuildId)).Count;
        }

        reply.PendingInvites = (uint)(await _store.GetInvitesAsync(player.UserId)).Count;
        return reply;
    }

    /// <summary>组装一条任务信息（含我的进度 / 共享任务的帮会进度）。</summary>
    public async Task<ClientGuildTaskInfo?> BuildTaskInfoAsync(string guildId, string taskId, string? myUserId)
    {
        var state = await _store.GetTaskAsync(guildId, taskId);
        if (state == null)
        {
            return null;
        }

        Catalog.TryGet(taskId, out var definition);
        var progress = await _store.GetTaskProgressAsync(guildId, taskId);
        var perMember = definition?.PerMember ?? true;

        var info = new ClientGuildTaskInfo
        {
            TaskId = state.TaskId,
            Title = definition?.Title ?? state.TaskId,
            Description = definition?.Description ?? string.Empty,
            Target = definition?.NormalizedTarget ?? 1,
            Status = state.Status,
            PublisherId = state.PublisherId,
            PublishedAt = (ulong)Math.Max(0, state.PublishedAt),
            PerMember = perMember,
            RewardKey = definition?.RewardKey ?? string.Empty,
            GuildProgress = perMember ? 0 : progress.GetValueOrDefault("_"),
        };

        if (myUserId != null)
        {
            info.Accepted = progress.ContainsKey(myUserId);
            info.MyProgress = progress.GetValueOrDefault(myUserId);
        }

        return info;
    }

    /// <summary>给协议层用的展示信息解析（申请列表等）。</summary>
    public Task<IReadOnlyDictionary<string, GuildDisplay>> ResolveDisplayForAsync(
        IReadOnlyCollection<string> userIds) => ResolveDisplayAsync(userIds);

    private async Task<IReadOnlyDictionary<string, GuildDisplay>> ResolveDisplayAsync(
        IReadOnlyCollection<string> userIds)
    {
        var resolver = DisplayResolver;
        if (resolver == null || userIds.Count == 0)
        {
            return new Dictionary<string, GuildDisplay>();
        }

        try
        {
            return await resolver(userIds);
        }
        catch (Exception e)
        {
            Log.Error("guild: resolve display err:{0}", e);
            return new Dictionary<string, GuildDisplay>();
        }
    }
}
