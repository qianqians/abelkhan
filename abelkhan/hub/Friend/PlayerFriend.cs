using consts;
using engine;
using friend;

namespace hub;

/// <summary>
/// 玩家实体上的好友协议处理（好友列表 / 申请 / 同意 / 拒绝 / 删除）。
/// Redis 操作与跨服编排在 <see cref="FriendService"/>，这里只做协议解析和结果拼装。
/// </summary>
public abstract partial class Player
{
    private IReadOnlyList<string> _friendIds = Array.Empty<string>();
    private long _pendingFriendReserve;

    /// <summary>
    /// 好友 id 列表（Redis 权威数据的本地镜像，由 hub 在关系变化 / 上线时刷新）。
    /// 游戏的存档镜像钩子里读它。
    /// </summary>
    public IReadOnlyList<string> Friends => _friendIds;

    /// <summary>
    /// 临时占位：已同意但还没被对方服务器确认的「预计添加的好友数」。
    /// 存档镜像里把它一起写进去，游戏侧看「好友数 + 待确认数」是否超上限。
    /// </summary>
    public long PendingFriendReserve => _pendingFriendReserve;

    /// <summary>存档镜像钩子：好友数据变化时由游戏实现把最新关系写进存档。</summary>
    private Func<Task>? _friendMirrorSync;

    internal void SetFriendSnapshot(IReadOnlyList<string> friends, long pendingReserve)
    {
        _friendIds = friends;
        _pendingFriendReserve = pendingReserve;
    }

    internal Task SyncFriendMirrorAsync() => _friendMirrorSync?.Invoke() ?? Task.CompletedTask;

    internal void RegisterFriendRequests(Service service)
    {
        var friendService = service.Friend;
        if (friendService == null)
        {
            return;
        }

        var store = friendService.Store;
        // 存档镜像：游戏可选实现；未实现时纯 Redis 模式仍然完整可用
        _friendMirrorSync = () => service.SyncFriendMirrorAsync(this);

        // 好友列表 + 在线状态 + 当前上限
        RegisterRequest<ClientFriendList, ClientFriendListResult>(Consts.ClientFriendList, async _ =>
        {
            var userIds = Friends;
            var presence = await store.GetPresenceAsync(userIds);
            var display = await friendService.ResolveDisplayAsync(userIds);
            var limit = await store.GetLimitAsync(UserId);

            var result = new ClientFriendListResult
            {
                MaxFriends = (uint)Math.Max(0, limit.Max),
                ExtraFriends = (uint)Math.Max(0, limit.Extra),
                Used = (uint)Math.Max(0, limit.Used),
            };

            foreach (var id in userIds)
            {
                result.Friends.Add(friendService.ToInfo(id, presence.ContainsKey(id), display));
            }

            return Result<ClientFriendListResult, string>.Ok(result);
        });

        // 待处理的好友申请（= 离线消息）
        RegisterRequest<ClientFriendRequests, ClientFriendRequestsResult>(Consts.ClientFriendRequests, async _ =>
        {
            var fromIds = await store.ListInboxAsync(UserId);
            var presence = await store.GetPresenceAsync(fromIds);
            var display = await friendService.ResolveDisplayAsync(fromIds);
            var result = new ClientFriendRequestsResult();
            foreach (var id in fromIds)
            {
                result.Requests.Add(friendService.ToInfo(id, presence.ContainsKey(id), display));
            }

            return Result<ClientFriendRequestsResult, string>.Ok(result);
        });

        // 发起申请：对方离线时作为离线消息留着，上线后处理
        RegisterRequest<ClientFriendSendRequest, ClientFriendSendRequestResult>(Consts.ClientFriendSendRequest,
            async req =>
            {
                var result = await store.SubmitRequestAsync(UserId, req.UserId);
                if (!result.Ok)
                {
                    return Result<ClientFriendSendRequestResult, string>.Err(MessageOf(result.Code));
                }

                // 对方在线的话立刻提示；离线就等他上线时由 FriendService.OnOnlineAsync 推
                var presence = await store.GetPresenceAsync(new[] { req.UserId });
                if (presence.ContainsKey(req.UserId))
                {
                    var display = await friendService.ResolveDisplayAsync(new[] { UserId });
                    await friendService.NotifyAsync(req.UserId, Consts.ClientFriendRequestNotify,
                        new ClientFriendRequestNotify
                        {
                            From = friendService.ToInfo(UserId, true, display),
                        });
                }

                return Result<ClientFriendSendRequestResult, string>.Ok(
                    new ClientFriendSendRequestResult { Pending = true });
            });

        // 同意：占位 → 跨服确认 → 回执
        RegisterRequest<ClientFriendAccept, ClientFriendAcceptResult>(Consts.ClientFriendAccept, async req =>
        {
            var result = await friendService.AcceptAsync(this, req.UserId);
            if (!result.Ok)
            {
                return Result<ClientFriendAcceptResult, string>.Err(MessageOf(result.Code));
            }

            var display = await friendService.ResolveDisplayAsync(new[] { req.UserId });
            return Result<ClientFriendAcceptResult, string>.Ok(new ClientFriendAcceptResult
            {
                Friend = friendService.ToInfo(req.UserId, true, display),
            });
        });

        // 拒绝
        RegisterRequest<ClientFriendReject, ClientFriendRejectResult>(Consts.ClientFriendReject, async req =>
        {
            var removed = await store.RejectRequestAsync(UserId, req.UserId);
            return removed
                ? Result<ClientFriendRejectResult, string>.Ok(new ClientFriendRejectResult())
                : Result<ClientFriendRejectResult, string>.Err(MessageOf(FriendCodes.NoRequest));
        });

        // 删除好友（双向解除）
        RegisterRequest<ClientFriendRemove, ClientFriendRemoveResult>(Consts.ClientFriendRemove, async req =>
        {
            var removed = await store.RemoveFriendAsync(UserId, req.UserId);
            if (!removed)
            {
                return Result<ClientFriendRemoveResult, string>.Err(MessageOf(FriendCodes.NotFriend));
            }

            await friendService.RefreshSnapshotAsync(this);
            await SyncFriendMirrorAsync();

            // 对方在线就同步通知
            await friendService.NotifyAsync(req.UserId, Consts.ClientFriendRemovedNotify,
                new ClientFriendRemovedNotify { UserId = UserId });

            return Result<ClientFriendRemoveResult, string>.Ok(new ClientFriendRemoveResult());
        });
    }

    private static string MessageOf(string code) => code switch
    {
        FriendCodes.Self => "不能加自己为好友",
        FriendCodes.AlreadyFriend => "你们已经是好友了",
        FriendCodes.Duplicate => "已经申请过了，等待对方处理",
        FriendCodes.NoRequest => "没有找到这条好友申请",
        FriendCodes.InboxFull => "对方的申请列表已满，稍后再试",
        FriendCodes.TargetFull => "对方的好友已满",
        FriendCodes.RequesterFull => "你的好友已满，先删掉一些再加",
        FriendCodes.SelfFull => "你的好友已满，先删掉一些再加",
        FriendCodes.NotFriend => "对方不是你的好友",
        FriendCodes.Timeout => "加好友确认超时，请重试",
        _ => "好友操作失败：" + code,
    };
}
