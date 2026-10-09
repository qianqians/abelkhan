using consts;
using core;
using engine;
using Google.Protobuf;
using Nito.Collections;

namespace gate;

public class ClientMsgHandle
{
    private readonly GateConfig _cfg;
    private readonly RedisHandle _redis;
    private readonly WRpc _rpc;
    private readonly Client _client;
    private readonly Deque<string> _clientReliabilityQueue;
    
    public ClientMsgHandle(GateConfig cfg, RedisHandle redis, WRpc rpc, Client client, Deque<string> clientReliabilityQueue)
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
                var userId = _client.UserId;
                if (string.IsNullOrEmpty(userId))
                {
                    break;
                }

                // 序号取 AckReliabilityMsg.seq；为 0（老客户端）则退回按 entity_id 比对
                var ackSeq = msg.Seq;
                if (ackSeq == 0 && string.IsNullOrEmpty(msg.EntityId))
                {
                    break;
                }

                // 出队前要先确认 ack 的就是 Redis 队头那一条：超时重投会让客户端对同一份消息
                // ack 两次，无条件出队会把还没投递的下一条弹掉（丢消息）
                _ = AckReliability(userId, ackSeq, msg.EntityId);
                break;
            }
            default:
            {
                Log.Error($"ClientMsgHandle ntf.Event.ProtoName:{ntf.Event.ProtoName}");
                break;
            }
        }
    }

    // 同一个 userId 的 ack 串行处理：否则两条重复 ack 可能同时读到同一个队头、
    // 各自通过校验再各出队一次，把还没投递的下一条弹掉。
    // 用 64 个桶（按 userId 散列）而不是每个 userId 一个锁，避免锁对象无限增长。
    private static readonly SemaphoreSlim[] AckLocks = CreateAckLocks();

    private static SemaphoreSlim[] CreateAckLocks()
    {
        var locks = new SemaphoreSlim[64];
        for (var i = 0; i < locks.Length; i++)
        {
            locks[i] = new SemaphoreSlim(1, 1);
        }
        return locks;
    }

    // ack 与 Redis 队头比对通过才出队；不通过（重投产生的重复 ack、迟到的 ack）直接忽略，
    // 不出队也不回队：正在等 ack 的那条消息由超时重投负责，回队反而会让它被立刻重发
    private async Task AckReliability(string userId, ulong ackSeq, string ackEntityId)
    {
        var ackLock = AckLocks[(uint)userId.GetHashCode() % (uint)AckLocks.Length];
        await ackLock.WaitAsync();
        try
        {
            var key = string.Format(Consts.EntityReliabilityClientMq, userId);
            var head = await _redis.Front(key);
            if (head != null && head.Length > 0 && !IsAckedHead(head, ackSeq, ackEntityId))
            {
                Log.Error($"gate: ack not match head, ignore userId:{userId} ack_seq:{ackSeq} ack_entity_id:{ackEntityId}");
                return;
            }

            await _redis.DeleteListElem(key);

            lock (_clientReliabilityQueue)
            {
                if (!_clientReliabilityQueue.Contains(userId))
                {
                    _clientReliabilityQueue.AddToBack(userId);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"gate: ack reliability err userId:{userId} {e}");
        }
        finally
        {
            ackLock.Release();
        }
    }

    // 队头那条可靠消息是不是这条 ack 所确认的；解析不了就按"不匹配"处理（宁可不出队）
    private bool IsAckedHead(byte[] head, ulong ackSeq, string ackEntityId)
    {
        try
        {
            var parser = new MessageParser<Msg>(() => new Msg());
            var msg = parser.ParseFrom(head);
            if (msg.PayloadCase != Msg.PayloadOneofCase.Notify ||
                msg.Notify.Event.ProtoName != Consts.GateForwardHubNotifyClientMq)
            {
                return false;
            }

            var ev = _rpc.OnMsg<GateForwardHubNotifyClientMq>(msg.Notify.Event.Content.ToByteArray());
            if (ackSeq != 0)
            {
                // seq 是 hub 投递时赋的唯一值，重投的副本沿用同一个值：
                // 重复 ack 到来时队头已经是下一条（seq 不同），会被忽略
                return ev.Seq == ackSeq;
            }

            // 老客户端没回填 seq，退回按 entity_id 比对
            return string.IsNullOrEmpty(ev.EntityId) || ev.EntityId == ackEntityId;
        }
        catch (Exception e)
        {
            Log.Error($"gate: parse reliability head err:{e}");
            return false;
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
                Log.Error($"ClientMsgHandle req.Event.ProtoName:{req.Event.ProtoName}");
                break;
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
                Log.Error($"ClientMsgHandle rsp.Event.ProtoName:{rsp.Event.ProtoName}");
                break;
            }
        }
    }
}