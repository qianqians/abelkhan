using consts;
using engine;
namespace hub;

public class GateMsgHandle
{
    private readonly WRpc _rpc;
    
    public GateMsgHandle(WRpc rpc)
    {
        _rpc = rpc;
        _rpc.OnNotify += OnNotify;
        _rpc.OnRequest += OnRequest;
        _rpc.OnResponse += OnResponse;
    }

    private void OnNotify(Notify ntf)
    {
        switch (ntf.Event.ProtoName)
        {
            case Consts.ClientDisconnect:
            {
                OnClientDisconnect(_rpc.OnMsg<ClientDisconnect>(ntf.Event.Content.ToByteArray()));
                break;
            }
            case Consts.ClientNotifyHub:
            {
                OnClientNotifyHub(_rpc.OnMsg<ClientNotifyHub>(ntf.Event.Content.ToByteArray()));
                break;
            }
            default:
                throw  new ArgumentException($"GateMsgHandle ntf.Event.ProtoName:{ntf.Event.ProtoName}");
        }
    }

    private void OnRequest(Request req)
    {
        switch (req.Event.ProtoName)
        {
            case Consts.ClientRequestHub:
            {
                OnClientRequestHub(_rpc.OnMsg<ClientRequestHub>(req.Event.Content.ToByteArray()));
                break;
            }
            default:
                throw new ArgumentException($"GateMsgHandle req.Event.ProtoName:{req.Event.ProtoName}");
        }
    }

    private void OnResponse(Response rsp)
    {
        switch (rsp.Event.ProtoName)
        {
            case Consts.ClientResponseHub:
            {
                OnClientResponseHub(_rpc.OnMsg<ClientResponseHub>(rsp.Event.Content.ToByteArray()));
                break;
            }
            default:
                throw new ArgumentException($"GateMsgHandle rsp.Event.ProtoName:{rsp.Event.ProtoName}");
        }
    }

    private void OnClientDisconnect(ClientDisconnect msg)
    {
        
    }

    private void OnClientRequestHub(ClientRequestHub msg)
    {
        
    }

    private void OnClientResponseHub(ClientResponseHub msg)
    {
        
    }

    private void OnClientNotifyHub(ClientNotifyHub msg)
    {
        
    }
}