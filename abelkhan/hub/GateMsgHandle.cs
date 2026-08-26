using System.Collections.Concurrent;
using consts;
using engine;
using core;
namespace hub;

public class GateMsgMqHandle
{
    private readonly WRpc _rpc;

    public GateMsgMqHandle(WRpc rpc)
    {
        _rpc = rpc;
        _rpc.OnNotify += OnNotify;
    }

    public event Action<string, string, string>? OnReconnect;
    public event Action<string, string, string, byte[]>? OnRequestService;

    private void OnNotify(Notify ntf)
    {
        switch (ntf.Event.ProtoName)
        {
            case Consts.GateForwardClientRequestReconnect:
            {
                var msg = _rpc.OnMsg<GateForwardClientRequestReconnect>(ntf.Event.Content.ToByteArray());
                OnReconnect?.Invoke(msg.UserId, msg.GateName, msg.ConnId);
                break;
            }
            case Consts.GateForwardClientRequestService:
            {
                var msg = _rpc.OnMsg<GateForwardClientRequestService>(ntf.Event.Content.ToByteArray());
                OnRequestService?.Invoke(msg.ServiceName, msg.GateName, msg.ConnId, msg.Argv.ToByteArray());
                break;
            }
            default:
                throw  new ArgumentException($"GateMsgHandle ntf.Event.ProtoName:{ntf.Event.ProtoName}");
        }
    }
}

public class GateMsgHandle
{
    private readonly WRpc _rpc;
    private readonly GateNetwork _gate;
    private readonly ConcurrentDictionary<string, BaseEntity> _entities;
    
    public GateMsgHandle(WRpc rpc, GateNetwork gate, ConcurrentDictionary<string, BaseEntity> entities)
    {
        _rpc = rpc;
        _gate = gate;
        _entities = entities;
        
        _rpc.OnNotify += OnNotify;
        _rpc.OnRequest += OnRequest;
        _rpc.OnResponse += OnResponse;
    }

    // ReSharper disable once AsyncVoidMethod
    private async void OnNotify(Notify ntf)
    {
        try
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
                    await OnClientNotifyHub(_rpc.OnMsg<ClientNotifyHub>(ntf.Event.Content.ToByteArray()));
                    break;
                }
                default:
                {
                    Log.Error($"GateMsgHandle ntf.Event.ProtoName:{ntf.Event.ProtoName}");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"GateMsgHandle ntf.Event.ProtoName:{ntf.Event.ProtoName} ex:{ex}");
        }
    }

    // ReSharper disable once AsyncVoidMethod
    private async void OnRequest(Request req)
    {
        try
        {
            switch (req.Event.ProtoName)
            {
                case Consts.ClientRequestHub:
                {
                    await OnClientRequestHub(req.MsgId, _rpc.OnMsg<ClientRequestHub>(req.Event.Content.ToByteArray()));
                    break;
                }
                default:
                {
                    Log.Error($"GateMsgHandle req.Event.ProtoName:{req.Event.ProtoName}");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"GateMsgHandle req.Event.ProtoName:{req.Event.ProtoName} ex:{ex}");
        }
    }

    private void OnResponse(Response rsp)
    {
        try
        {
            switch (rsp.Event.ProtoName)
            {
                case Consts.ClientResponseHub:
                {
                    OnClientResponseHub(rsp.MsgId, _rpc.OnMsg<ClientResponseHub>(rsp.Event.Content.ToByteArray()));
                    break;
                }
                default:
                {
                    Log.Error($"GateMsgHandle rsp.Event.ProtoName:{rsp.Event.ProtoName}");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"GateMsgHandle rsp.Event.ProtoName:{rsp.Event.ProtoName} ex:{ex}");
        }
    }

    private void OnClientDisconnect(ClientDisconnect msg)
    {
        Log.Info("Client:{0} Disconnect", msg.ConnId);
    }

    private async Task OnClientRequestHub(string msgId, ClientRequestHub msg)
    {
        if (_entities.TryGetValue(msg.EntityId, out var entity))
        {
            try
            {
                await entity.OnDoMsg(msg.ConnId, msgId, _gate, msg.Event.ProtoName, msg.Event.Content);
            }
            catch (Exception ex)
            {
                Log.Error($"OnClientRequestHub Entity:{msg.EntityId} failed! {ex}");
            }
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

    private async Task OnClientNotifyHub(ClientNotifyHub msg)
    {
        if (_entities.TryGetValue(msg.EntityId, out var entity))
        {
            try
            {
                await entity.OnDoMsg(msg.ConnId, string.Empty, _gate, msg.Event.ProtoName, msg.Event.Content);
            }
            catch (Exception ex)
            {
                Log.Error($"OnClientNotifyHub Entity:{msg.EntityId} failed! {ex}");
            }
        }
        else
        {
            Log.Error($"OnClientNotifyHub Entity:{msg.EntityId} not found!");
        }
    }
}