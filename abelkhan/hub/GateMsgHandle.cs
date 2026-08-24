using consts;
using engine;
using core;
namespace hub;

public class GateMsgHandle
{
    private readonly WRpc _rpc;
    private readonly GateNetwork _gate;
    private readonly Dictionary<string, BaseEntity> _entities;
    
    public GateMsgHandle(WRpc rpc, GateNetwork gate, Dictionary<string, BaseEntity> entities)
    {
        _rpc = rpc;
        _gate = gate;
        _entities = entities;
        
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
                OnClientRequestHub(req.MsgId, _rpc.OnMsg<ClientRequestHub>(req.Event.Content.ToByteArray()));
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
                OnClientResponseHub(rsp.MsgId, _rpc.OnMsg<ClientResponseHub>(rsp.Event.Content.ToByteArray()));
                break;
            }
            default:
                throw new ArgumentException($"GateMsgHandle rsp.Event.ProtoName:{rsp.Event.ProtoName}");
        }
    }

    private void OnClientDisconnect(ClientDisconnect msg)
    {
        Log.Info("Client:{0} Disconnect", msg.ConnId);
    }

    private void OnClientRequestHub(string msgId, ClientRequestHub msg)
    {
        if (_entities.TryGetValue(msg.EntityId, out var entity))
        {
            entity.OnDoMsg(msg.ConnId, msgId, _gate, msg.Event.ProtoName, msg.Event.Content);
        } 
        else
        {
            Log.Error($"OnClientRequestHub Entity:{msg.EntityId} not found!");
        }
    }

    private void OnClientResponseHub(string msgId, ClientResponseHub msg)
    {
        if (_entities.TryGetValue(msg.EntityId, out var entity))
        {
            entity.OnResponse(msgId, msg.ErrMsg, msg.Content.ToByteArray());
        }
        else
        {
            Log.Error($"OnClientResponseHub Entity:{msg.EntityId} not found!");
        }
    }

    private void OnClientNotifyHub(ClientNotifyHub msg)
    {
        if (_entities.TryGetValue(msg.EntityId, out var entity))
        {
            entity.OnDoMsg(msg.ConnId, string.Empty, _gate, msg.Event.ProtoName, msg.Event.Content);
        }
        else
        {
            Log.Error($"OnClientNotifyHub Entity:{msg.EntityId} not found!");
        }
    }
}