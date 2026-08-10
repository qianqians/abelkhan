namespace consts;

// ReSharper disable once ClassNeverInstantiated.Global
public class Consts
{
    public static readonly string NotifyConnId = "notify_conn_id";
    public static readonly string EnterGame = "enter_game";
    public static readonly string EntityServerMq =  "entity_{0}_server_mq";
    public static readonly string Kickoff = "server_kickoff_client";

    public const string ClientRequestReconnect = "client_request_reconnect";
    public const string ClientRequestService = "client_request_service";
    public const string GateForwardClientRequestHub =  "gate_forward_client_request_hub"; 
    public const string GateForwardClientResponseHub = "gate_forward_client_response_hub";
    public const string GateForwardClientNotifyHub = "Gate_forward_client_notify_hub";
    public const string VersionHandshake  = "version_handshake";
    
    public const string GateForwardClientRequestReconnect = "gate_forward_client_request_reconnect";
    public const string GateForwardClientRequestService = "gate_forward_client_request_service";
    public const string ClientDisconnect = "client_disconnect";
    public const string ClientRequestHub = "client_request_hub";
    public const string ClientResponseHub = "client_response_hub";
    public const string ClientNotifyHub =  "client_notify_hub";
}