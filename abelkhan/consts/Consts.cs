namespace consts;

// ReSharper disable once ClassNeverInstantiated.Global
public class Consts
{
    public static readonly string EntityClientMq =  "entity_{0}_client_mq";
    public static readonly string EntityReliabilityClientMq =  "entity_reliability_{0}_client_mq";
    public static readonly string EntityServerMq =  "entity_{0}_server_mq";

    public const string ClientRequestReconnect = "client_request_reconnect";
    public const string ClientRequestService = "client_request_service";
    // 客户端请求领取邮件（对应 mail.proto 的 ClientRequestMail / ClientRequestMailResult）
    public const string ClientRequestMail = "client_request_mail";

    // ---- 背包（对应 bag.proto 的 ClientBag* 消息） ----
    public const string ClientBagInfo = "client_bag_info";
    public const string ClientBagMove = "client_bag_move";
    public const string ClientBagSort = "client_bag_sort";
    public const string ClientBagDiscard = "client_bag_discard";
    public const string ClientBagUse = "client_bag_use";

    // 背包变更通知（hub -> 客户端）
    public const string ClientBagChangedNotify = "client_bag_changed_notify";

    // ---- 商城 / 寄拍 / 限时拍卖（对应 shop.proto 的 ClientShop* 消息） ----
    public const string ClientShopList = "client_shop_list";
    public const string ClientShopBuy = "client_shop_buy";
    public const string ClientShopBid = "client_shop_bid";
    public const string ClientShopConsign = "client_shop_consign";
    public const string ClientShopCancel = "client_shop_cancel";
    public const string ClientShopClaim = "client_shop_claim";
    public const string GateForwardClientRequestHub =  "gate_forward_client_request_hub"; 
    public const string GateForwardClientResponseHub = "gate_forward_client_response_hub";
    public const string GateForwardClientNotifyHub = "Gate_forward_client_notify_hub";
    public const string VersionHandshake  = "version_handshake";
    public const string AckReliabilityMsg = "ack_reliability_msg";
    
    public const string GateForwardClientRequestReconnect = "gate_forward_client_request_reconnect";
    public const string GateForwardClientRequestService = "gate_forward_client_request_service";
    public const string ClientDisconnect = "client_disconnect";
    public const string ClientRequestHub = "client_request_hub";
    public const string ClientResponseHub = "client_response_hub";
    public const string ClientNotifyHub =  "client_notify_hub";

    public const string HubCreatePlayerEntity = "hub_create_player_entity";
    public const string HubCreateRemoteEntity = "hub_create_remote_entity";
    public const string HubDeleteRemoteEntity = "hub_delete_remote_entity";
    public const string HubRefreshEntity = "hub_refresh_entity";
    public const string GateForwardHubRequestClient =  "gate_forward_hub_request_client";
    public const string GateForwardHubResponseClient =  "gate_forward_hub_response_client";
    public const string GateForwardHubNotifyClient =  "gate_forward_hub_notify_client";
    public const string GateForwardHubCallGlobal = "gate_forward_hub_call_global";
    public const string GateForwardHubNotifyClientMq = "gate_forward_hub_notify_client_mq";
    public const string HubKickOffClient = "hub_kickoff_client";
    
    public const string CreatePlayerEntity = "create_player_entity"; 
    public const string CreateRemoteEntity = "create_remote_entity"; 
    public const string DeleteRemoteEntity = "delete_remote_entity";
    public const string RefreshEntity = "refresh_entity";
    public const string NotifyConnId = "notify_conn_id";
    public const string HubRequestClient = "hub_request_client";
    public const string HubResponseClient = "hub_response_client";
    public const string HubNotifyClient = "hub_notify_client";
    public const string HubNotifyClientMq = "hub_notify_client_mq";
    public const string KickOff = "kickoff";

    // ---- admin / dashboard ----
    // gate 周期上报的在线状态，admin 汇总所有 gate 的这两个 key 得到全局在线数
    public const string AdminOnlineGateKey = "admin_online_gate_{0}";
    public const string AdminOnlineConnectionKey = "admin_online_connection_{0}";

    // 定向玩家 GM 命令队列（{0} = playerId，admin 写入，游戏服消费）
    public const string AdminGmQueueKey = "admin_gm_queue_{0}";

    // 单玩家 / 批量邮件队列（{0} = playerId，游戏服按玩家消费）
    public const string AdminMailQueueKey = "admin_mail_queue_{0}";

    // 全局邮件：所有玩家共用一份，存 Redis list；玩家请求邮件时读取并判断是否已领取
    public const string AdminMailGlobalListKey = "admin_mail_global";

    // 玩家已领取的全局邮件 id 集合（{0} = playerId）
    public const string AdminMailClaimedKey = "admin_mail_claimed_{0}";

    // ---- shop / 寄拍 ----
    // 所有 shop key 都带 {shop} hash tag：Lua 脚本可以安全地跨多个 key 操作
    // （Redis Cluster 下这些 key 落在同一个 slot；单机 Redis 下 hash tag 只是普通字符）。
    // 注意：下面常量里的 {shop} 写成 {{shop}} 是为了 string.Format 转义，
    // 实际 key 里是 {shop}。通过 shop.ShopKeys 统一生成，不要手写 key。
    // 单条商品/寄拍的 hash key（{0} = listingId）
    public const string ShopListingKey = "shop:{{shop}}:listing:{0}";
    // 全部商品/寄拍 id 集合
    public const string ShopListingsKey = "shop:{{shop}}:listings";
    // 待结算（按 end_time 排序）的 zset，hub 扫描它结算过期拍卖
    public const string ShopActiveKey = "shop:{{shop}}:active";
    // 卖家名下的寄拍 id 集合（{0} = playerId）
    public const string ShopSellerListingsKey = "shop:{{shop}}:seller:{0}";
    // 单个 listing 的每人购买计数（{0} = listingId）
    public const string ShopPurchasedKey = "shop:{{shop}}:purchased:{0}";
    // 玩家待领取的结算物品/货币（{0} = playerId）
    public const string ShopDeliveryKey = "shop:{{shop}}:delivery:{0}";
    // 成交/结算记录（Redis list，admin 查询）
    public const string ShopOrdersKey = "shop:{{shop}}:orders";

    // ---- 排行榜（对应 leaderboard.proto 的 ClientLeaderboard* 消息） ----
    public const string ClientLeaderboardSubmit = "client_leaderboard_submit";
    public const string ClientLeaderboardRange = "client_leaderboard_range";
    public const string ClientLeaderboardRank = "client_leaderboard_rank";

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

    // ---- 聊天（对应 chat.proto 的 ClientChat* 消息） ----
    public const string ClientChatChannels = "client_chat_channels";
    public const string ClientChatSend = "client_chat_send";
    public const string ClientChatHistory = "client_chat_history";
    public const string ClientChatPrivate = "client_chat_private";

    // 聊天通知（hub -> 客户端）
    public const string ClientChatMessageNotify = "client_chat_message_notify";
    public const string ClientChatPrivateNotify = "client_chat_private_notify";

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

    // ---- 任务（对应 quest.proto 的 ClientQuest* 消息） ----
    public const string ClientQuestList = "client_quest_list";
    public const string ClientQuestAccept = "client_quest_accept";
    public const string ClientQuestComplete = "client_quest_complete";

    // 任务通知（hub -> 客户端）
    public const string ClientQuestUpdatedNotify = "client_quest_updated_notify";
}
