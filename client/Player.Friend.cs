using consts;
using engine;

namespace client;

/// <summary>
/// 好友协议（申请 / 同意 / 拒绝 / 删除 / 列表）与服务器推送的通知事件。
/// </summary>
public abstract partial class Player
{
    // ================================================================
    // 请求
    // ================================================================

    /// <summary>好友列表（含在线状态与当前上限）。</summary>
    public Task<Result<ClientFriendListResult, string>> RequestFriendList()
    {
        return Request<ClientFriendList, ClientFriendListResult>(
            Consts.ClientFriendList,
            new ClientFriendList());
    }

    /// <summary>待处理的好友申请（= 离线消息，上线后拉一次）。</summary>
    public Task<Result<ClientFriendRequestsResult, string>> RequestFriendRequests()
    {
        return Request<ClientFriendRequests, ClientFriendRequestsResult>(
            Consts.ClientFriendRequests,
            new ClientFriendRequests());
    }

    /// <summary>发起好友申请。对方离线时会作为离线消息保留。</summary>
    public Task<Result<ClientFriendSendRequestResult, string>> SendFriendRequest(string userId)
    {
        return Request<ClientFriendSendRequest, ClientFriendSendRequestResult>(
            Consts.ClientFriendSendRequest,
            new ClientFriendSendRequest { UserId = userId });
    }

    /// <summary>同意好友申请（服务端会占位 + 跨服确认，可能要等一小会儿）。</summary>
    public Task<Result<ClientFriendAcceptResult, string>> AcceptFriendRequest(string userId)
    {
        return Request<ClientFriendAccept, ClientFriendAcceptResult>(
            Consts.ClientFriendAccept,
            new ClientFriendAccept { UserId = userId });
    }

    /// <summary>拒绝好友申请。</summary>
    public Task<Result<ClientFriendRejectResult, string>> RejectFriendRequest(string userId)
    {
        return Request<ClientFriendReject, ClientFriendRejectResult>(
            Consts.ClientFriendReject,
            new ClientFriendReject { UserId = userId });
    }

    /// <summary>删除好友（双向解除）。</summary>
    public Task<Result<ClientFriendRemoveResult, string>> RemoveFriend(string userId)
    {
        return Request<ClientFriendRemove, ClientFriendRemoveResult>(
            Consts.ClientFriendRemove,
            new ClientFriendRemove { UserId = userId });
    }

    // ================================================================
    // 通知
    // ================================================================

    /// <summary>收到一条好友申请（或上线时补推的待处理申请）。</summary>
    public event Action<ClientFriendRequestNotify>? FriendRequested;

    /// <summary>好友上线。</summary>
    public event Action<ClientFriendOnlineNotify>? FriendOnline;

    /// <summary>好友下线。</summary>
    public event Action<ClientFriendOfflineNotify>? FriendOffline;

    /// <summary>加好友成功（对方同意了你的申请 / 对方把你加为好友）。</summary>
    public event Action<ClientFriendAddedNotify>? FriendAdded;

    /// <summary>被对方删除好友。</summary>
    public event Action<ClientFriendRemovedNotify>? FriendRemoved;

    /// <summary>同意好友超时（临时占位已回收，可重试）。</summary>
    public event Action<ClientFriendAcceptTimeoutNotify>? FriendAcceptTimeout;

    private void RegisterFriendNotifications()
    {
        TryRegisterNotify<ClientFriendRequestNotify>(Consts.ClientFriendRequestNotify,
            msg => FriendRequested?.Invoke(msg));
        TryRegisterNotify<ClientFriendOnlineNotify>(Consts.ClientFriendOnlineNotify,
            msg => FriendOnline?.Invoke(msg));
        TryRegisterNotify<ClientFriendOfflineNotify>(Consts.ClientFriendOfflineNotify,
            msg => FriendOffline?.Invoke(msg));
        TryRegisterNotify<ClientFriendAddedNotify>(Consts.ClientFriendAddedNotify,
            msg => FriendAdded?.Invoke(msg));
        TryRegisterNotify<ClientFriendRemovedNotify>(Consts.ClientFriendRemovedNotify,
            msg => FriendRemoved?.Invoke(msg));
        TryRegisterNotify<ClientFriendAcceptTimeoutNotify>(Consts.ClientFriendAcceptTimeoutNotify,
            msg => FriendAcceptTimeout?.Invoke(msg));
    }
}
