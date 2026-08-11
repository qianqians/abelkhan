namespace consts;

// ReSharper disable once ClassNeverInstantiated.Global
public class Consts
{
    public static readonly string EnterGame = "enter_game";
    public static readonly string EntityClientMq =  "entity_{0}_client_mq";
    public static readonly string EntityReliabilityClientMq =  "entity_reliability_{0}_client_mq";
    public static readonly string EntityServerMq =  "entity_{0}_server_mq";

    public const string ClientRequestReconnect = "client_request_reconnect";
    public const string ClientRequestService = "client_request_service";
    public const string GateForwardClientRequestHub =  "gate_forward_client_request_hub"; 
    public const string GateForwardClientResponseHub = "gate_forward_client_response_hub";
    public const string GateForwardClientNotifyHub = "Gate_forward_client_notify_hub";
    public const string VersionHandshake  = "version_handshake";
    public const string CallBackReliabilityMsg = "call_back_reliability_msg";
    
    public const string GateForwardClientRequestReconnect = "gate_forward_client_request_reconnect";
    public const string GateForwardClientRequestService = "gate_forward_client_request_service";
    public const string ClientDisconnect = "client_disconnect";
    public const string ClientRequestHub = "client_request_hub";
    public const string ClientResponseHub = "client_response_hub";
    public const string ClientNotifyHub =  "client_notify_hub";

    public const string HubCreateRemoteEntity = "hub_create_remote_entity";
    public const string HubDeleteRemoteEntity = "hub_delete_remote_entity";
    public const string HubRefreshEntity = "hub_refresh_entity";
    public const string GateForwardHubRequestClient =  "gate_forward_hub_request_client";
    public const string GateForwardHubResponseClient =  "gate_forward_hub_response_client";
    public const string GateForwardHubNotifyClient =  "gate_forward_hub_notify_client";
    public const string GateForwardHubCallGlobal = "gate_forward_hub_call_global";
    public const string HubKickOffClient = "hub_kickoff_client";
    
    public const string CreatePlayerEntity = "create_player_entity"; 
    public const string CreateRemoteEntity = "create_remote_entity"; 
    public const string DeleteRemoteEntity = "delete_remote_entity";
    public const string RefreshEntity = "refresh_entity";
    public const string NotifyConnId = "notify_conn_id";
    public const string HubRequestClient = "hub_request_client";
    public const string HubResponseClient = "hub_response_client";
    public const string HubNotifyClient = "hub_notify_client";
    public const string KickOff = "kickoff";
}