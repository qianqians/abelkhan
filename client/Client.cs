using System.Collections.Concurrent;
using System.Net.WebSockets;
using consts;
using engine;
using Google.Protobuf;

namespace client;

/// <summary>
/// 客户端会话：只走 WebSocket 连接 gate，自带拆包、心跳、版本握手，
/// 断线自动重连，并把服务器下发的消息分发到对应的实体。
///
/// 收发链路（与 gate/ClientMsgHandle 对应）：
///   进服务   ClientRequestService / 重连 ClientRequestReconnect
///   notify   GateForwardClientNotifyHub  -> hub 侧 entity.OnDoMsg
///   request  GateForwardClientRequestHub -> hub 侧 entity.OnDoMsg，等 response
///   response GateForwardClientResponseHub（内层 msg_id 必须回填服务器给的 msg_id）
///   可靠消息 HubNotifyClientMq(need_ack) -> 处理完回 AckReliabilityMsg{entity_id, seq}
///
/// 断线重连后的实体重同步：
///   重连成功时把所有已有实体标记为"未认领"，随后服务器用
///   CreatePlayerEntity / CreateRemoteEntity / RefreshEntity 重新下发一遍（顺带更新数据），
///   没被重新认领的实体在 ResyncTimeoutMs 之后本地删掉（等价于服务器发了 DeleteRemoteEntity）。
/// </summary>
public class Client
{
    private readonly ClientConfig _cfg;
    private readonly Service _service;
    private readonly WRpc _rpc = new();
    private readonly ConcurrentDictionary<string, Entity> _entities = new();
    private readonly Dictionary<string, Action<string, byte[]>> _requestCallbacks = new();

    private WebSocketNetwork? _network;
    private Task? _tHeartBeats;
    private Task? _tReconnect;
    private volatile bool _isRun;
    private bool _kicked;
    private bool _hadConnected;
    private volatile bool _connected;
    private int _disconnectHandled;
    private string _connId = string.Empty;
    private long _epoch;

    public Client(ClientConfig cfg, Service service)
    {
        _cfg = cfg;
        _service = service;
        UserId = cfg.UserId;

        // 只订阅一次：断线重连是同一条 rpc 实例，重复订阅会把同一条消息分发多次
        _rpc.OnNotify += OnNotify;
        _rpc.OnRequest += OnRequest;
        _rpc.OnResponse += OnResponse;
    }

    /// <summary>重连时带给服务器的 user_id（登录成功后由游戏设置）</summary>
    public string UserId { get; set; }

    /// <summary>gate 下发的连接 id（收到 NotifyConnID 之后才有值）</summary>
    public string ConnId => _connId;

    /// <summary>当前是否连着（拿到 conn_id 且网络在）</summary>
    public bool IsConnected => _connected && _network != null;

    /// <summary>是否被服务器踢下线（被踢后不会自动重连，手动 Connect 会清掉）</summary>
    public bool Kicked => _kicked;

    /// <summary>本地所有实体（key = entity_id），包含玩家自己的实体</summary>
    public ICollection<Entity> Entities => _entities.Values;

    /// <summary>本地玩家实体（CreatePlayerEntity 创建的那个）；服务器还没下发时为 null</summary>
    public Player? Player { get; private set; }

    /// <summary>每次连上（含重连）都会触发</summary>
    public event Action<string>? OnConnected;

    /// <summary>重连成功时触发（在 OnConnected 之后，随后服务器会重新下发实体）</summary>
    public event Action<string>? OnReconnected;

    /// <summary>连接断开（网络断开或主动 Close）</summary>
    public event Action? OnDisconnected;

    public Entity? GetEntity(string entityId)
    {
        return _entities.TryGetValue(entityId, out var entity) ? entity : null;
    }

    // ---------------- 连接 / 断线 / 重连 ----------------

    /// <summary>连接服务器（只支持 WebSocket）；返回 false 表示连接失败</summary>
    public async Task<bool> Connect()
    {
        if (_network != null)
        {
            return true;
        }

        try
        {
            var socket = new ClientWebSocket();
            if (_cfg.IgnoreCertificate)
            {
                socket.Options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
            }

            using var cts = new CancellationTokenSource((int)_cfg.ConnectTimeoutMs);
            await socket.ConnectAsync(new Uri(_cfg.Wss), cts.Token);

            var receiveBuffer = new ReceiveBuffer
            {
                OnPacket = _rpc.OnNetworkData,
            };
            var network = new WebSocketNetwork(socket, receiveBuffer);
            network.OnClose += OnNetworkClosed;

            _network = network;
            _isRun = true;
            _kicked = false;
            Interlocked.Exchange(ref _disconnectHandled, 0);
            network.Start();

            if (_tHeartBeats == null || _tHeartBeats.IsCompleted)
            {
                _tHeartBeats = Task.Factory.StartNew(HeartBeatsLoop, TaskCreationOptions.LongRunning).Unwrap();
            }

            Log.Info($"client connect ok:{_cfg.Wss}");
            return true;
        }
        catch (Exception e)
        {
            Log.Error($"client connect failed:{_cfg.Wss} ex:{e.Message}");
            return false;
        }
    }

    /// <summary>主动断开（不会自动重连）</summary>
    public Task Close()
    {
        _isRun = false;
        var network = _network;
        if (network != null)
        {
            // 本地关闭也会走到 OnNetworkClosed（幂等）
            _ = network.Close();
        }
        else
        {
            OnNetworkClosed();
        }

        return Task.CompletedTask;
    }

    /// <summary>统一的断线处理：读循环结束、发送失败都会走到这里</summary>
    private void OnNetworkClosed()
    {
        // 只处理一次
        if (Interlocked.Exchange(ref _disconnectHandled, 1) == 1)
        {
            return;
        }

        var network = _network;
        _network = null;
        if (network != null)
        {
            _ = network.Close();
        }

        _connected = false;
        _connId = string.Empty;

        // 断线时把所有等待回包的 request 失败掉，避免调用方一直挂着
        FailPendingRequests("connection lost");
        OnDisconnected?.Invoke();

        if (_isRun && !_kicked && _hadConnected && _cfg.AutoReconnect)
        {
            Log.Error("client disconnected, reconnecting...");
            StartReconnect();
        }
        else
        {
            Log.Error("client disconnected");
        }
    }

    private void StartReconnect()
    {
        if (_tReconnect is { IsCompleted: false })
        {
            return;
        }

        _tReconnect = Task.Factory.StartNew(ReconnectLoop, TaskCreationOptions.LongRunning).Unwrap();
    }

    private async Task ReconnectLoop()
    {
        var interval = _cfg.ReconnectIntervalMs > 0 ? _cfg.ReconnectIntervalMs : 3000;
        while (_isRun && !_kicked && _network == null)
        {
            Log.Info($"client reconnect after {interval}ms ...");
            await Task.Delay((int)interval);

            if (!_isRun || _kicked || _network != null)
            {
                return;
            }

            if (await Connect())
            {
                // 连上了，后面由 NotifyConnID 触发 EnterServer / OnReconnected
                return;
            }
        }
    }

    private async Task HeartBeatsLoop()
    {
        var interval = _cfg.HeartBeatsIntervalMs > 0 ? _cfg.HeartBeatsIntervalMs : 3000;
        while (_isRun)
        {
            try
            {
                await Task.Delay((int)interval);
                if (!_isRun)
                {
                    break;
                }

                if (_network == null)
                {
                    // 断线中，等重连
                    continue;
                }

                // gate 侧 10s 收不到任何事件就会断开连接
                await Send(new Msg { HeartBeats = new HeartBeats() }.ToByteArray());
            }
            catch (Exception e)
            {
                Log.Error($"client heartbeats ex:{e.Message}");
            }
        }
    }

    /// <summary>收到 conn_id 之后：版本握手 +（可选的）进服务/重连</summary>
    private async Task EnterServer()
    {
        try
        {
            // 版本握手：gate 只在这条消息到达时校验 [MinVersion, MaxVersion]，不合适会直接 KickOff
            await Send(_rpc.Notify(Consts.VersionHandshake, new VersionHandshake
            {
                MinVersion = _cfg.MinVersion,
                MaxVersion = _cfg.MaxVersion,
            }));

            // 注意：gate 在 accept 时就已经把 EnterService 的服务请求投给 hub 了，
            // 所以"第一次进服"不需要客户端再发东西，等 CreatePlayerEntity 就行。
            var argv = ByteString.CopyFrom(_cfg.Argv ?? []);
            if (string.IsNullOrEmpty(UserId) && Player != null)
            {
                // 重连用的 user_id：游戏可以在 Player.OnCreate 里按自己的 argv 格式解析后设置
                UserId = Player.UserId;
            }

            if (!string.IsNullOrEmpty(UserId))
            {
                await RequestReconnect(UserId, argv);
            }
            else if (!string.IsNullOrEmpty(_cfg.ServiceName))
            {
                await RequestService(_cfg.ServiceName, argv);
            }
        }
        catch (Exception e)
        {
            Log.Error($"client enter server ex:{e}");
        }
    }

    /// <summary>主动进入某个服务（gate 会把它投给 hub 侧同名 service）</summary>
    public async Task RequestService(string serviceName, ByteString argv)
    {
        await Send(_rpc.Notify(Consts.ClientRequestService, new ClientRequestService
        {
            ServiceName = serviceName,
            Argv = argv,
        }));
        Log.Info($"client request service:{serviceName}");
    }

    /// <summary>
    /// 带 user_id 重连（gate 会转给 EnterService）。
    /// 需要服务端在 OnReconnect 里把会话接回去并重新下发实体
    /// （CreatePlayerEntity / CreateRemoteEntity / RefreshEntity / DeleteRemoteEntity）。
    /// </summary>
    public async Task RequestReconnect(string userId, ByteString argv)
    {
        await Send(_rpc.Notify(Consts.ClientRequestReconnect, new ClientRequestReconnect
        {
            UserId = userId,
            Argv = argv,
        }));
        Log.Info($"client request reconnect userId:{userId}");
    }

    private async Task Send(byte[] data)
    {
        var network = _network;
        if (network == null)
        {
            Log.Error("client not connected!");
            return;
        }

        try
        {
            await network.Send(data);
        }
        catch (Exception e)
        {
            // 发送失败基本等于断线：走统一断线流程（失败未完成 request + 触发重连）
            Log.Error($"client send ex:{e.Message}");
            OnNetworkClosed();
        }
    }

    // ---------------- 服务器下发的消息 ----------------

    private void OnNotify(Notify ntf)
    {
        try
        {
            switch (ntf.Event.ProtoName)
            {
                case Consts.NotifyConnId:
                {
                    var msg = _rpc.OnMsg<NotifyConnID>(ntf.Event.Content.ToByteArray());
                    _connId = msg.ConnId;
                    _connected = true;
                    var isReconnect = _hadConnected;
                    _hadConnected = true;
                    Log.Info($"client got connId:{_connId} reconnect:{isReconnect}");
                    OnConnected?.Invoke(_connId);
                    if (isReconnect)
                    {
                        StartResync();
                        OnReconnected?.Invoke(_connId);
                    }

                    _ = EnterServer();
                    break;
                }
                case Consts.CreatePlayerEntity:
                {
                    var msg = _rpc.OnMsg<CreatePlayerEntity>(ntf.Event.Content.ToByteArray());
                    UpsertEntity(msg.EntityId, msg.EntityType, msg.Argv.ToByteArray(), true);
                    break;
                }
                case Consts.CreateRemoteEntity:
                {
                    var msg = _rpc.OnMsg<CreateRemoteEntity>(ntf.Event.Content.ToByteArray());
                    UpsertEntity(msg.EntityId, msg.EntityType, msg.Argv.ToByteArray(), false);
                    break;
                }
                case Consts.DeleteRemoteEntity:
                {
                    var msg = _rpc.OnMsg<DeleteRemoteEntity>(ntf.Event.Content.ToByteArray());
                    DeleteEntity(msg.EntityId);
                    break;
                }
                case Consts.RefreshEntity:
                {
                    var msg = _rpc.OnMsg<RefreshEntity>(ntf.Event.Content.ToByteArray());
                    RefreshEntity(msg.EntityId, msg.EntityType, msg.Argv.ToByteArray());
                    break;
                }
                case Consts.HubNotifyClient:
                {
                    var msg = _rpc.OnMsg<HubNotifyClient>(ntf.Event.Content.ToByteArray());
                    DispatchNotify(msg.EntityId, msg.Event.ProtoName, msg.Event.Content);
                    break;
                }
                case Consts.HubNotifyClientMq:
                {
                    var msg = _rpc.OnMsg<HubNotifyClientMq>(ntf.Event.Content.ToByteArray());
                    DispatchNotifyMq(msg);
                    break;
                }
                case Consts.KickOff:
                {
                    var msg = _rpc.OnMsg<KickOff>(ntf.Event.Content.ToByteArray());
                    Log.Error($"client kickoff:{msg.PromptInfo}");
                    // 被踢不再自动重连，避免和版本检查/顶号死循环
                    _kicked = true;
                    _ = Close();
                    break;
                }
                default:
                {
                    Log.Error($"client OnNotify ProtoName:{ntf.Event.ProtoName}");
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"client OnNotify ProtoName:{ntf.Event.ProtoName} ex:{e}");
        }
    }

    private void OnRequest(Request req)
    {
        try
        {
            switch (req.Event.ProtoName)
            {
                case Consts.HubRequestClient:
                {
                    var msg = _rpc.OnMsg<HubRequestClient>(req.Event.Content.ToByteArray());
                    var entity = GetEntity(msg.EntityId);
                    if (entity == null)
                    {
                        // 回 error，别让 hub 侧的 request 一直挂着
                        Log.Error($"client OnRequest entity:{msg.EntityId} not found!");
                        _ = ErrorClient(msg.EntityId, req.MsgId, $"entity:{msg.EntityId} not found!");
                        break;
                    }

                    entity.Post(() => entity.OnRequestMsg(req.MsgId, msg.Event.ProtoName, msg.Event.Content));
                    break;
                }
                default:
                {
                    Log.Error($"client OnRequest ProtoName:{req.Event.ProtoName}");
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"client OnRequest ProtoName:{req.Event.ProtoName} ex:{e}");
        }
    }

    private void OnResponse(Response rsp)
    {
        try
        {
            switch (rsp.Event.ProtoName)
            {
                case Consts.HubResponseClient:
                {
                    // 注意：HubResponseClient 里没有 entity_id，只能按 msg_id 找回等待中的 request
                    var msg = _rpc.OnMsg<HubResponseClient>(rsp.Event.Content.ToByteArray());
                    Action<string, byte[]>? callback;
                    lock (_requestCallbacks)
                    {
                        _requestCallbacks.Remove(rsp.MsgId, out callback);
                    }

                    if (callback == null)
                    {
                        Log.Error($"client OnResponse msgId:{rsp.MsgId} not found!");
                        break;
                    }

                    callback(msg.ErrMsg, msg.Content.ToByteArray());
                    break;
                }
                default:
                {
                    Log.Error($"client OnResponse ProtoName:{rsp.Event.ProtoName}");
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"client OnResponse ProtoName:{rsp.Event.ProtoName} ex:{e}");
        }
    }

    // ---------------- entity 增删改（重连重同步也走这里） ----------------

    /// <summary>
    /// CreatePlayerEntity / CreateRemoteEntity：本地没有就按服务器给的类型创建，
    /// 已经有了说明是重连后服务器重新下发 —— 当作 Refresh 处理，不重建对象。
    /// </summary>
    private void UpsertEntity(string entityId, string entityType, byte[] argv, bool isPlayer)
    {
        if (_entities.TryGetValue(entityId, out var exist))
        {
            exist.ConfirmedEpoch = _epoch;
            if (exist is Player existPlayer)
            {
                // 重连后玩家实体被重新认领
                Player = existPlayer;
            }

            exist.Post(() => exist.OnRefresh(argv));
            Log.Info($"client refresh exist entity id:{entityId} type:{entityType} isPlayer:{isPlayer}");
            return;
        }

        Entity? entity = isPlayer
            ? _service.CreatePlayerEntity(entityId, entityType, argv)
            : _service.CreateRemoteEntity(entityId, entityType, argv);
        if (entity == null)
        {
            Log.Error($"client create entity failed id:{entityId} type:{entityType} isPlayer:{isPlayer}");
            return;
        }

        entity.ConfirmedEpoch = _epoch;
        if (!_entities.TryAdd(entityId, entity))
        {
            Log.Error($"client entity:{entityId} already exist!");
            return;
        }

        entity.Session = this;
        if (entity is Player player)
        {
            Player = player;
        }

        // 走实体自己的队列，保证 OnCreate 排在该实体的其它消息之前
        entity.Post(() => entity.OnCreate(argv));
        Log.Info($"client create entity id:{entityId} type:{entityType} isPlayer:{isPlayer}");
    }

    private void DeleteEntity(string entityId)
    {
        if (!_entities.TryRemove(entityId, out var entity))
        {
            Log.Error($"client delete entity:{entityId} not found!");
            return;
        }

        if (ReferenceEquals(entity, Player))
        {
            Player = null;
        }

        entity.Post(() => entity.OnDelete());
        Log.Info($"client delete entity id:{entityId} type:{entity.EntityType}");
    }

    private void RefreshEntity(string entityId, string entityType, byte[] argv)
    {
        if (_entities.TryGetValue(entityId, out var entity))
        {
            entity.ConfirmedEpoch = _epoch;
            entity.Post(() => entity.OnRefresh(argv));
            return;
        }

        // 本地没有（比如重连后服务器只发了增量刷新）：按远端实体补建一个
        UpsertEntity(entityId, entityType, argv, false);
    }

    /// <summary>重连成功：本轮重连的实体都要被服务器重新认领一遍</summary>
    private void StartResync()
    {
        _epoch++;
        foreach (var entity in _entities.Values)
        {
            entity.ConfirmedEpoch = -1;
        }

        Log.Info($"client resync start epoch:{_epoch} entities:{_entities.Count}");
        if (_cfg.ResyncTimeoutMs > 0)
        {
            _ = ResyncCheck(_epoch);
        }
    }

    /// <summary>重连后超时还没被认领的实体，本地删掉</summary>
    private async Task ResyncCheck(long epoch)
    {
        await Task.Delay((int)_cfg.ResyncTimeoutMs);
        if (epoch != _epoch)
        {
            // 期间又重连了一次，交给新一轮
            return;
        }

        foreach (var (entityId, entity) in _entities)
        {
            if (entity.ConfirmedEpoch == epoch)
            {
                continue;
            }

            if (_entities.TryRemove(entityId, out var stale))
            {
                if (ReferenceEquals(stale, Player))
                {
                    Player = null;
                }

                stale.Post(() => stale.OnDelete());
                Log.Info($"client resync drop stale entity id:{entityId} type:{stale.EntityType}");
            }
        }
    }

    // ---------------- 分发到实体 ----------------

    private void DispatchNotify(string entityId, string method, ByteString content)
    {
        var entity = GetEntity(entityId);
        if (entity == null)
        {
            Log.Error($"client notify entity:{entityId} not found!");
            return;
        }

        entity.Post(() => entity.OnNotifyMsg(method, content));
    }

    private void DispatchNotifyMq(HubNotifyClientMq msg)
    {
        var entity = GetEntity(msg.EntityId);
        if (entity == null)
        {
            Log.Error($"client notify entity:{msg.EntityId} not found!");
            // 没有对应实体也要回 ack：否则 gate 每 10s 重投一次，把这个用户的可靠队列顶死
            if (msg.NeedAck)
            {
                _ = AckReliability(msg.EntityId, msg.Seq);
            }
            return;
        }

        entity.Post(async () =>
        {
            await entity.OnNotifyMsg(msg.Event.ProtoName, msg.Event.Content);
            if (msg.NeedAck)
            {
                // 处理完再 ack（至少一次语义）
                await AckReliability(msg.EntityId, msg.Seq);
            }
        });
    }

    private async Task AckReliability(string entityId, ulong seq)
    {
        await Send(_rpc.Notify(Consts.AckReliabilityMsg, new AckReliabilityMsg
        {
            EntityId = entityId,
            Seq = seq,
        }));
    }

    private void FailPendingRequests(string errMsg)
    {
        Action<string, byte[]>[] callbacks;
        lock (_requestCallbacks)
        {
            callbacks = _requestCallbacks.Values.ToArray();
            _requestCallbacks.Clear();
        }

        foreach (var callback in callbacks)
        {
            callback(errMsg, []);
        }
    }

    // ---------------- 供 Entity 调用 ----------------

    /// <summary>服务器 request 的 response：内层 msg_id 必须是服务器给的 msg_id（gate 用它转发回 hub）</summary>
    internal async Task ResponseClient(string entityId, string msgId, byte[] content)
    {
        var msg = new GateForwardClientResponseHub
        {
            MsgId = msgId,
            EntityId = entityId,
            Content = ByteString.CopyFrom(content),
        };
        await Send(_rpc.Response(Consts.GateForwardClientResponseHub, msgId, msg));
    }

    /// <summary>服务器 request 的 error</summary>
    internal async Task ErrorClient(string entityId, string msgId, string errMsg)
    {
        var msg = new GateForwardClientResponseHub
        {
            MsgId = msgId,
            EntityId = entityId,
            ErrMsg = errMsg,
        };
        await Send(_rpc.Response(Consts.GateForwardClientResponseHub, msgId, msg));
    }

    /// <summary>向服务器侧 entity 发 notify</summary>
    internal async Task Notify<T>(string entityId, string method, T argv)
        where T : IMessage<T>
    {
        var callRpc = new CallRpc
        {
            ProtoName = method,
            Content = argv.ToByteString(),
        };
        var forward = new GateForwardClientNotifyHub
        {
            EntityId = entityId,
            Event = callRpc,
        };
        await Send(_rpc.Notify(Consts.GateForwardClientNotifyHub, forward));
    }

    /// <summary>向服务器侧 entity 发 request，等它的 response</summary>
    internal async Task<Result<T1, string>> Request<T0, T1>(string entityId, string method, T0 argv)
        where T0 : IMessage<T0>
        where T1 : IMessage<T1>, new()
    {
        var msgId = Guid.NewGuid().ToString();
        var t = new TaskCompletionSource<Result<T1, string>>();
        lock (_requestCallbacks)
        {
            _requestCallbacks.Add(msgId, (errMsg, content) =>
            {
                if (!string.IsNullOrEmpty(errMsg))
                {
                    t.TrySetResult(Result<T1, string>.Err(errMsg));
                    return;
                }

                try
                {
                    var parser = new MessageParser<T1>(() => new T1());
                    t.TrySetResult(Result<T1, string>.Ok(parser.ParseFrom(content)));
                }
                catch (Exception e)
                {
                    t.TrySetResult(Result<T1, string>.Err($"parse response failed:{e.Message}"));
                }
            });
        }

        try
        {
            var callRpc = new CallRpc
            {
                ProtoName = method,
                Content = argv.ToByteString(),
            };
            var forward = new GateForwardClientRequestHub
            {
                MsgId = msgId,
                EntityId = entityId,
                Event = callRpc,
            };
            await Send(_rpc.Request(Consts.GateForwardClientRequestHub, msgId, forward));

            if (_cfg.RequestTimeoutMs > 0)
            {
                var done = await Task.WhenAny(t.Task, Task.Delay((int)_cfg.RequestTimeoutMs));
                if (done != t.Task)
                {
                    lock (_requestCallbacks)
                    {
                        _requestCallbacks.Remove(msgId);
                    }

                    return Result<T1, string>.Err($"request {method} timeout");
                }
            }

            return await t.Task;
        }
        catch (Exception e)
        {
            lock (_requestCallbacks)
            {
                _requestCallbacks.Remove(msgId);
            }

            Log.Error($"client request method:{method} ex:{e}");
            return Result<T1, string>.Err(e.Message);
        }
    }
}
