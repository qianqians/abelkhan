using System.Collections.Concurrent;
using core;
using engine;
using friend;
using Google.Protobuf;
using consts;

namespace hub;

/// <summary>
/// 好友系统的编排层（hub 侧）。
///
/// 关系数据全在 Redis（<see cref="FriendStore"/> 的 Lua 原子操作），本类负责：
///   - 上下线事件推送（上线 / 下线给在线好友发通知）；
///   - 加好友的三步流程：占位 → 跨服确认 → 回执（对应需求里的「临时标记 + 跨服确认」）；
///   - 跨 hub 指令队列的收发；
///   - 占位超时回收、路由续期。
///
/// 通知统一走 <c>entity_{userId}_client_mq</c>：gate 消费后按 userId 找到连接并投递，
/// 因此不受「好友在别的 hub 上」影响（直接走 GateNetwork 只能发给本 hub 的玩家）。
/// </summary>
public sealed class FriendService
{
    private readonly FriendStore _store;
    private readonly RedisHandle _redis;
    private readonly string _hubId;
    private readonly WRpc _rpc = new();

    /// <summary>本 hub 在线的玩家：userId → Player。用于刷存档镜像 / 回执。</summary>
    private readonly ConcurrentDictionary<string, Player> _players = new();

    /// <summary>等待跨服确认回执的请求：requestId → TCS。</summary>
    private readonly ConcurrentDictionary<string, TaskCompletionSource<FriendResult>> _pending = new();

    /// <summary>游戏提供的「按 userId 加载离线玩家实例」钩子（返回 null 表示不需要 / 无法加载）。</summary>
    private Func<string, Task<Player?>>? _offlineLoader;

    public FriendService(FriendStore store, RedisHandle redis, string hubId)
    {
        _store = store;
        _redis = redis;
        _hubId = hubId;
    }

    public FriendStore Store => _store;

    public string HubId => _hubId;

    /// <summary>
    /// 游戏提供的展示信息解析（昵称 / 头像）。由 <see cref="MainClass.RegisterService"/> 注入，
    /// 未提供时条目展示字段为空。
    /// </summary>
    public Func<IReadOnlyCollection<string>, Task<IReadOnlyDictionary<string, FriendDisplay>>>? DisplayResolver
    {
        get;
        set;
    }

    // ================================================================
    // 上线 / 下线
    // ================================================================

    /// <summary>
    /// 玩家上线：刷新路由、把待处理申请推给玩家、并给在线好友推送上线事件。
    /// </summary>
    public async Task OnOnlineAsync(Player player)
    {
        var userId = player.UserId;
        _players[userId] = player;
        await _store.SetRouteAsync(userId, _hubId);

        // 本地镜像：好友列表 + 临时占位
        var limit = await _store.GetLimitAsync(userId);
        var friends = await _store.ListFriendsAsync(userId);
        player.SetFriendSnapshot(friends, limit.Reserved);

        // 待处理的好友申请（离线消息）
        var pendingFrom = await _store.ListInboxAsync(userId);
        var pendingDisplay = await ResolveDisplayAsync(pendingFrom);
        foreach (var from in pendingFrom)
        {
            await NotifyAsync(userId, Consts.ClientFriendRequestNotify, new ClientFriendRequestNotify
            {
                From = ToInfo(from, true, pendingDisplay),
            });
        }

        // 上线事件：只推给「在线」的好友
        var presence = await _store.GetPresenceAsync(friends);
        foreach (var (friendId, where) in presence)
        {
            await NotifyDirectAsync(friendId, where.EntityId, Consts.ClientFriendOnlineNotify,
                new ClientFriendOnlineNotify { UserId = userId });
        }

        await SyncMirrorAsync(player);
    }

    /// <summary>玩家下线：给在线好友推送下线事件。</summary>
    public async Task OnOfflineAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }

        _players.TryRemove(userId, out _);

        var friends = await _store.ListFriendsAsync(userId);
        var presence = await _store.GetPresenceAsync(friends);
        foreach (var (friendId, where) in presence)
        {
            await NotifyDirectAsync(friendId, where.EntityId, Consts.ClientFriendOfflineNotify,
                new ClientFriendOfflineNotify { UserId = userId });
        }
    }

    // ================================================================
    // 加好友：占位 → 跨服确认 → 回执
    // ================================================================

    /// <summary>
    /// 同意好友申请：
    ///   1. 在同意方身上占位（<see cref="FriendStore.AcceptReserveAsync"/>）；
    ///   2. 找到申请方所在 hub：本 hub 直接确认、远端 hub 发指令、离线则加锁建离线实例；
    ///   3. 等回执，拿到结果后清占位（失败时）并刷新本地好友列表。
    /// </summary>
    public async Task<FriendResult> AcceptAsync(Player player, string targetId)
    {
        var userId = player.UserId;

        var (reserve, _) = await _store.AcceptReserveAsync(userId, targetId);
        if (!reserve.Ok)
        {
            return reserve;
        }

        var requestId = Guid.NewGuid().ToString("N");
        var tcs = new TaskCompletionSource<FriendResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pending[requestId] = tcs;
        Metrics.SetGauge("abelkhan_friend_pending_confirms", "In-flight cross-hub friend confirms",
            _pending.Count);

        try
        {
            await SyncMirrorAsync(player);

            var route = await _store.GetRouteAsync(targetId);
            if (route == _hubId)
            {
                // 申请方就在本 hub：不用绕队列，直接确认
                var local = await _store.ConfirmAddAsync(targetId, userId);
                tcs.TrySetResult(local);
                if (local.Ok && _players.TryGetValue(targetId, out var target))
                {
                    await RefreshSnapshotAsync(target);
                }
            }
            else if (!string.IsNullOrEmpty(route))
            {
                await PushHubCommandAsync(route, Consts.FriendConfirmAdd, new FriendConfirmAdd
                {
                    RequesterId = userId,
                    TargetId = targetId,
                    ReplyHub = _hubId,
                    RequestId = requestId,
                });
            }
            else
            {
                // 申请方离线：加用户锁，必要时按需创建离线实例，本地完成确认
                await ConfirmForOfflinePlayerAsync(userId, targetId, tcs);
            }

            // 等待回执：正常路径只是一次 Redis / 队列往返，超时设短，
            // 避免同意请求长时间占住该玩家实体的任务队列（占位本身由定时扫描兜底）
            var timeout = Math.Max(500, Math.Min(_store.Settings.AcceptTimeoutMs,
                _store.Settings.ReserveTimeoutMs));
            var finished = await Task.WhenAny(tcs.Task, Task.Delay(timeout));
            if (finished != tcs.Task)
            {
                // 超时：回滚占位，避免好友位被永久占住（定时扫描也会兜底）
                await _store.CancelReserveAsync(userId, targetId);
                await RefreshSnapshotAsync(player);
                return FriendResult.Fail(FriendCodes.Timeout, "加好友确认超时");
            }

            var result = await tcs.Task;
            if (!result.Ok)
            {
                // 确认失败（对方已满 / 申请已失效）：占位已在 Lua 里回滚，这里只刷新列表
                await RefreshSnapshotAsync(player);
                return result;
            }

            await RefreshSnapshotAsync(player);
            await SyncMirrorAsync(player);

            // 通知申请方：好友关系已建立
            await NotifyAsync(targetId, Consts.ClientFriendAddedNotify,
                new ClientFriendAddedNotify { UserId = userId });
            return result;
        }
        finally
        {
            _pending.TryRemove(requestId, out _);
            Metrics.SetGauge("abelkhan_friend_pending_confirms", "In-flight cross-hub friend confirms",
                _pending.Count);
        }
    }

    /// <summary>
    /// 对离线玩家执行确认：加用户级锁 → 按需创建离线实例（游戏钩子）→ 写好友 → 落盘。
    /// 关系写入本身是 Redis Lua 原子的，离线实例只用于同步对方的存档镜像。
    /// </summary>
    private async Task ConfirmForOfflinePlayerAsync(string requesterId, string targetId,
        TaskCompletionSource<FriendResult> tcs)
    {
        var token = Guid.NewGuid().ToString("N");
        var lockKey = _store.Keys.UserLock(targetId);
        try
        {
            await _redis.Lock(lockKey, token, 5000);
            var result = await _store.ConfirmAddAsync(targetId, requesterId);
            tcs.TrySetResult(result);

            // 只有成功才需要刷新对方的存档镜像
            if (result.Ok && _offlineLoader != null)
            {
                var offline = await _offlineLoader(targetId);
                if (offline != null)
                {
                    var friends = await _store.ListFriendsAsync(targetId);
                    var limit = await _store.GetLimitAsync(targetId);
                    offline.SetFriendSnapshot(friends, limit.Reserved);
                    await SyncMirrorAsync(offline);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error("friend: confirm offline target:{0} err:{1}", targetId, e);
            tcs.TrySetResult(FriendResult.Fail("internal", e.Message));
        }
        finally
        {
            try
            {
                await _redis.UnLock(lockKey, token);
            }
            catch (Exception e)
            {
                Log.Error("friend: unlock user:{0} err:{1}", targetId, e);
            }
        }
    }

    /// <summary>游戏提供的「按 userId 加载离线玩家实例」钩子（返回 null 表示不需要 / 无法加载）。</summary>
    public void SetOfflineLoader(Func<string, Task<Player?>> loader)
    {
        _offlineLoader = loader;
    }

    // ================================================================
    // 跨 hub 指令
    // ================================================================

    /// <summary>处理来自其它 hub 的指令（由 <see cref="MainClass"/> 的队列消费循环调用）。</summary>
    public async Task HandleHubCommandAsync(Notify ntf)
    {
        switch (ntf.Event.ProtoName)
        {
            case Consts.FriendConfirmAdd:
            {
                var msg = _rpc.OnMsg<FriendConfirmAdd>(ntf.Event.Content.ToByteArray());
                var result = await _store.ConfirmAddAsync(msg.TargetId, msg.RequesterId);
                if (result.Ok)
                {
                    // 申请方就在本 hub：刷新它的本地好友列表 + 存档镜像
                    if (_players.TryGetValue(msg.TargetId, out var target))
                    {
                        await RefreshSnapshotAsync(target);
                        await SyncMirrorAsync(target);
                    }

                    await NotifyAsync(msg.RequesterId, Consts.ClientFriendAddedNotify,
                        new ClientFriendAddedNotify { UserId = msg.TargetId });
                }

                await PushHubCommandAsync(msg.ReplyHub, Consts.FriendConfirmAddResult, new FriendConfirmAddResult
                {
                    RequesterId = msg.RequesterId,
                    TargetId = msg.TargetId,
                    RequestId = msg.RequestId,
                    Code = result.Code,
                });
                break;
            }
            case Consts.FriendConfirmAddResult:
            {
                var msg = _rpc.OnMsg<FriendConfirmAddResult>(ntf.Event.Content.ToByteArray());
                if (_pending.TryRemove(msg.RequestId, out var tcs))
                {
                    tcs.TrySetResult(msg.Code == FriendCodes.Ok
                        ? FriendResult.Success(msg.Code)
                        : FriendResult.Fail(msg.Code));
                }

                break;
            }
            default:
            {
                Log.Error("friend: unknown hub command:{0}", ntf.Event.ProtoName);
                break;
            }
        }
    }

    private Task PushHubCommandAsync<T>(string hubId, string method, T msg) where T : IMessage<T>, new()
    {
        return _store.PushHubCommandAsync(hubId, _rpc.Notify(method, msg));
    }

    // ================================================================
    // 定时任务
    // ================================================================

    /// <summary>回收超期占位，并给同意方推一条超时通知。</summary>
    public async Task SweepReservesAsync(int limit)
    {
        var expired = await _store.SweepExpiredReservesAsync(limit);
        if (expired.Count > 0)
        {
            // 非 0 说明有跨服确认超时了 —— 最该告警的信号之一
            Metrics.Count("abelkhan_friend_reserve_swept_total",
                "Temporary friend slots reclaimed after timeout", expired.Count);
        }

        foreach (var (userId, otherId) in expired)
        {
            if (_players.TryGetValue(userId, out var player))
            {
                await RefreshSnapshotAsync(player);
            }

            await NotifyAsync(userId, Consts.ClientFriendAcceptTimeoutNotify,
                new ClientFriendAcceptTimeoutNotify { UserId = otherId });
        }
    }

    /// <summary>给本 hub 在线玩家续期路由（TTL 到期即视为不在本 hub）。</summary>
    public async Task RefreshRoutesAsync()
    {
        foreach (var userId in _players.Keys)
        {
            await _store.SetRouteAsync(userId, _hubId);
        }
    }

    // ================================================================
    // 通知 / 展示信息 / 镜像
    // ================================================================

    /// <summary>
    /// 给某个 userId 推通知。走 <c>entity_{userId}_client_mq</c>：gate 按 userId 找到连接，
    /// 所以目标玩家在别的 hub 上也能收到。
    /// </summary>
    public async Task NotifyAsync<T>(string userId, string method, T msg) where T : IMessage<T>, new()
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return;
        }

        // 本 hub 在线的玩家直接投递：presence 由 gate 每 3s 刷新，刚登录的玩家可能还没写进去，
        // 靠它判断会漏掉「上线时推待处理申请」这类立刻要发的通知。
        if (_players.TryGetValue(userId, out var local))
        {
            await NotifyDirectAsync(userId, local.EntityId, method, msg);
            return;
        }

        var presence = await _store.GetPresenceAsync(new[] { userId });
        if (!presence.TryGetValue(userId, out var where))
        {
            // 不在线：不推（上下线 / 申请通知都是在线才能看到的 toast）
            return;
        }

        await NotifyDirectAsync(userId, where.EntityId, method, msg);
    }

    /// <summary>
    /// 已经拿到 presence 时直接投递，省一次 MGET。
    /// （上线 / 下线事件是批量推送的，逐个再查 presence 会变成 N+1 次往返。）
    /// </summary>
    private async Task NotifyDirectAsync<T>(string userId, string entityId, string method, T msg)
        where T : IMessage<T>, new()
    {
        var forward = new GateForwardHubNotifyClientMq
        {
            UserId = userId,
            EntityId = entityId,
            Event = new CallRpc
            {
                ProtoName = method,
                Content = msg.ToByteString(),
            },
        };

        var payload = _rpc.Notify(Consts.GateForwardHubNotifyClientMq, forward);
        await _redis.PushList(string.Format(Consts.EntityClientMq, userId), payload);
    }

    /// <summary>批量解析展示信息（未提供钩子时返回空字典）。</summary>
    public async Task<IReadOnlyDictionary<string, FriendDisplay>> ResolveDisplayAsync(
        IReadOnlyCollection<string> userIds)
    {
        var resolver = DisplayResolver;
        if (resolver == null || userIds.Count == 0)
        {
            return new Dictionary<string, FriendDisplay>();
        }

        try
        {
            return await resolver(userIds);
        }
        catch (Exception e)
        {
            Log.Error("friend: resolve display err:{0}", e);
            return new Dictionary<string, FriendDisplay>();
        }
    }

    /// <summary>把在线状态 + 展示信息拼成协议条目。</summary>
    public ClientFriendInfo ToInfo(string userId, bool online,
        IReadOnlyDictionary<string, FriendDisplay> display)
    {
        var info = new ClientFriendInfo { UserId = userId, Online = online };
        if (display.TryGetValue(userId, out var d))
        {
            info.DisplayName = d.Name;
            info.Avatar = d.Avatar;
        }

        return info;
    }

    /// <summary>刷新玩家内存里的好友镜像（好友列表 + 临时占位）。</summary>
    public async Task RefreshSnapshotAsync(Player player)
    {
        var friends = await _store.ListFriendsAsync(player.UserId);
        var limit = await _store.GetLimitAsync(player.UserId);
        player.SetFriendSnapshot(friends, limit.Reserved);
    }

    private async Task SyncMirrorAsync(Player player)
    {
        try
        {
            await player.SyncFriendMirrorAsync();
        }
        catch (Exception e)
        {
            Log.Error("friend: sync mirror user:{0} err:{1}", player.UserId, e);
        }
    }
}
