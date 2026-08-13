using System.Collections.Concurrent;
using consts;
using core;
using engine;
using Google.Protobuf;
namespace gate;

public class ClientMsgHandle
{
    private readonly GateConfig _cfg;
    private readonly RedisHandle _redis;
    private readonly WRpc _rpc;
    private readonly Client _client;
    private readonly ConcurrentQueue<string> _clientReliabilityQueue;
    
    public ClientMsgHandle(GateConfig cfg, RedisHandle redis, WRpc rpc, Client client, ConcurrentQueue<string> clientReliabilityQueue)
    {
        _cfg = cfg;
        _redis = redis;
        _rpc = rpc;
        _client = client;
        _clientReliabilityQueue = clientReliabilityQueue;
        
        rpc.OnNotify += OnNotify;
        rpc.OnRequest += OnRequest;
        rpc.OnResponse += OnResponse;
        rpc.OnHeartBeats += OnHeartBeats;
    }

    private void OnHeartBeats(HeartBeats _)
    {
        _client.LastEventTime = TimerService.Tick;
    }

    private void OnNotify(Notify ntf)
    {
        _client.LastEventTime = TimerService.Tick;
        switch (ntf.Event.ProtoName)
        {
            case Consts.ClientRequestReconnect:
            {
                var msg = _rpc.OnMsg<ClientRequestReconnect>(ntf.Event.Content.ToByteArray());
                var forward = new GateForwardClientRequestReconnect()
                {
                    GateName = _cfg.GateId,
                    ConnId = _client.ConnId,
                    UserId = msg.UserId,
                    Argv = msg.Argv,
                };
                _ = _client.SendToServer(_cfg.EnterService, _rpc.Notify(Consts.GateForwardClientRequestReconnect, forward));
                break;
            }
            case Consts.ClientRequestService:
            {
                var msg = _rpc.OnMsg<ClientRequestService>(ntf.Event.Content.ToByteArray());
                var forward = new GateForwardClientRequestService()
                {
                    GateName = _cfg.GateId,
                    ConnId = _client.ConnId,
                    ServiceName = msg.ServiceName,
                    Argv = msg.Argv,
                };
                _ = _client.SendToServer(msg.ServiceName, _rpc.Notify(Consts.GateForwardClientRequestService, forward));
                break;
            }
            case Consts.GateForwardClientNotifyHub:
            {
                var msg = _rpc.OnMsg<GateForwardClientNotifyHub>(ntf.Event.Content.ToByteArray());
                var forward = new ClientNotifyHub()
                {
                    ConnId = _client.ConnId,
                    EntityId = msg.EntityId,
                    Event = msg.Event,
                };
                _ = _client.SendToServer(msg.EntityId, _rpc.Notify(Consts.ClientNotifyHub, forward));
                break;
            }
            case Consts.VersionHandshake:
            {
                var msg = _rpc.OnMsg<VersionHandshake>(ntf.Event.Content.ToByteArray());
                if (msg.MinVersion < _cfg.MinVersion || msg.MaxVersion > _cfg.MaxVersion)
                {
                    var ntfKick = new KickOff()
                    {
                        PromptInfo = "unsupported game version!",
                    };
                    _ = _client.SendToClient(_rpc.Notify(Consts.KickOff, ntfKick));
                }
                break;
            }
            case Consts.AckReliabilityMsg:
            {
                var msg = _rpc.OnMsg<AckReliabilityMsg>(ntf.Event.Content.ToByteArray());
                if (!string.IsNullOrEmpty(msg.EntityId))
                {
                    _ = _redis.DeleteListElem(string.Format(Consts.EntityReliabilityClientMq, msg.EntityId));
                    _clientReliabilityQueue.Enqueue(msg.EntityId);
                }
                break;
            }
            default:
                throw  new ArgumentException($"ClientMsgHandle ntf.Event.ProtoName:{ntf.Event.ProtoName}");
        }
    }

    private void OnRequest(Request req)
    {
        _client.LastEventTime = TimerService.Tick;
        switch (req.Event.ProtoName)
        {
            case Consts.GateForwardClientRequestHub:
            {
                var msg = _rpc.OnMsg<GateForwardClientRequestHub>(req.Event.Content.ToByteArray());
                var forward = new ClientRequestHub()
                {
                    ConnId = _client.ConnId,
                    EntityId = msg.EntityId,
                    Event = msg.Event,
                };
                _ = _client.SendToServer(msg.EntityId, _rpc.Request(Consts.ClientRequestHub, req.MsgId, forward));
                break;
            }
            default:
            {
                throw new ArgumentException($"ClientMsgHandle req.Event.ProtoName:{req.Event.ProtoName}");
            }
        }
    }

    private void OnResponse(Response rsp)
    {
        _client.LastEventTime = TimerService.Tick;
        switch (rsp.Event.ProtoName)
        {
            case Consts.GateForwardClientResponseHub:
            {
                var msg = _rpc.OnMsg<GateForwardClientResponseHub>(rsp.Event.Content.ToByteArray());
                var forward = new ClientResponseHub()
                {
                    EntityId =  msg.EntityId,
                    ErrMsg = msg.ErrMsg,
                    Content = msg.Content,
                };
                _ = _client.SendToServer(msg.EntityId, _rpc.Response(Consts.ClientResponseHub, msg.MsgId, forward));
                break;
            }
            default:
            {
                throw new ArgumentException($"ClientMsgHandle rsp.Event.ProtoName:{rsp.Event.ProtoName}");
            }
        }
    }
}