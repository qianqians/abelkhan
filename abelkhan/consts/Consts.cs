namespace consts;

// ReSharper disable once ClassNeverInstantiated.Global
public class Consts
{
    public static readonly string EntityClientMq =  "entity_{0}_client_mq";
    public static readonly string EntityReliabilityClientMq =  "entity_reliability_{0}_client_mq";
    public static readonly string EntityServerMq =  "entity_{0}_server_mq";

    public const string ClientRequestReconnect = "client_request_reconnect";
    public const string ClientRequestService = "client_request_service";
    // 客户端请求领取邮件（对应 ClientRequestMail / ClientRequestMailResult）
    public const string ClientRequestMail = "client_request_mail";
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
}