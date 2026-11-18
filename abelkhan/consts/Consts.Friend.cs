namespace consts;

/// <summary>
/// 好友协议方法名 + 跨 hub 指令（对应 proto/friend.proto / hub_hub.proto）
/// </summary>
public partial class Consts
{
    // ---- 好友（对应 friend.proto 的 ClientFriend* 消息） ----
    public const string ClientFriendList = "client_friend_list";
    public const string ClientFriendRequests = "client_friend_requests";
    public const string ClientFriendSendRequest = "client_friend_send_request";
    public const string ClientFriendAccept = "client_friend_accept";
    public const string ClientFriendReject = "client_friend_reject";
    public const string ClientFriendRemove = "client_friend_remove";

    // 好友通知（hub -> 客户端）
    public const string ClientFriendRequestNotify = "client_friend_request_notify";
    public const string ClientFriendOnlineNotify = "client_friend_online_notify";
    public const string ClientFriendOfflineNotify = "client_friend_offline_notify";
    public const string ClientFriendAddedNotify = "client_friend_added_notify";
    public const string ClientFriendRemovedNotify = "client_friend_removed_notify";
    public const string ClientFriendAcceptTimeoutNotify = "client_friend_accept_timeout_notify";

    // ---- 跨 hub 指令（hub_hub.proto，经 hub:{hubId}:mq） ----
    public const string FriendConfirmAdd = "friend_confirm_add";
    public const string FriendConfirmAddResult = "friend_confirm_add_result";
}
