using System.Collections.Concurrent;
using Google.Protobuf;
using core;
using engine;
using consts;
// ReSharper disable FieldCanBeMadeReadOnly.Global
namespace gate;

public struct GateConfig()
{
    public string GateId = string.Empty;
    public string RedisUrl = string.Empty;
    public string RedisPwd  = string.Empty;
    public ushort PortInternal = 0;
    public ushort PortExternal = 0;
    public string Pfx = string.Empty;
    public string PfxPassword  = string.Empty;
    public string EnterService = string.Empty;
    public uint MinVersion = 0;
    public uint MaxVersion = 0;
}

class Main
{
    private RedisHandle? _redis;
    private TcpAcceptService? _internal;
    private WebSocketAcceptService? _external;
    private Dictionary<string, Client>? _clients;

    private bool _isRun = true;
    private Task? _tWait;
    private ConcurrentQueue<string>? _clientWaitQueue;
    private Task? _tWaitReliability;
    private ConcurrentQueue<string>? _clientReliabilityQueue;

    private void StartRedisMsg()
    {
        var rpc = new WRpc();
        _ = new HubMsgHandle(rpc, new HubGeneralMsgHandle(_clients!, _clientWaitQueue!, _clientReliabilityQueue!, rpc));
        
        _tWait = Task.Factory.StartNew(async () =>
        {
            while (_isRun)
            {
                if (_clientWaitQueue!.TryDequeue(out var playerId))
                {
                    await Task.Delay(1);
                    continue;
                }
                if (string.IsNullOrEmpty(playerId))
                {
                    await Task.Delay(1);
                    continue;
                }

                var msg = await _redis?.PopList(string.Format(Consts.EntityClientMq, playerId), 8)!;
                if (msg == null)
                {
                    if (!string.IsNullOrEmpty(playerId))
                    {
                        _clientWaitQueue.Enqueue(playerId);
                    }
                    await Task.Delay(1);
                    continue;
                }
                
                foreach (var m in msg)
                {
                    rpc.OnNetworkData(m);
                }

                _clientWaitQueue.Enqueue(playerId);
            }
        }, TaskCreationOptions.LongRunning);
    }

    private void StartRedisReliabilityMsg()
    {
        var rpc = new WRpc();
        _ = new HubMsgHandle(rpc, new HubGeneralMsgHandle(_clients!, _clientWaitQueue!, _clientReliabilityQueue!, rpc));
        
        _tWaitReliability = Task.Factory.StartNew(async () =>
        {
            while (_isRun)
            {
                if (_clientReliabilityQueue!.TryDequeue(out var playerId))
                {
                    await Task.Delay(1);
                    continue;
                }
                if (string.IsNullOrEmpty(playerId))
                {
                    await Task.Delay(1);
                    continue;
                }
                
                var data = await _redis?.Front(string.Format(Consts.EntityReliabilityClientMq, playerId))!;
                if (data == null)
                {
                    await Task.Delay(1);
                    continue;
                }
                var parser = new MessageParser<Msg>(() => new Msg());
                var msg = parser.ParseFrom(data);
                if (msg == null)
                {
                    await Task.Delay(1);
                    continue;
                }
                if (msg.PayloadCase != Msg.PayloadOneofCase.Notify)
                {
                    await Task.Delay(1);
                    continue;
                }
                if (msg.Notify.Event.ProtoName != Consts.GateForwardHubNotifyClient)
                {
                    await Task.Delay(1);
                    continue;
                }
                
                var ev = rpc.OnMsg<GateForwardHubNotifyClient>(msg.Notify.Event.ToByteArray());
                if (ev.ConnId.Count <= 0 || ev.ConnId.Count > 1)
                {
                    await Task.Delay(1);
                    continue;
                }
                var forward = new HubNotifyClient()
                {
                    EntityId = ev.EntityId,
                    Event = ev.Event,
                };
                if (_clients!.TryGetValue(ev.ConnId[0], out var cli))
                {
                    _ = cli.SendToClient(rpc.Notify(Consts.HubNotifyClient, forward));
                }
            }
        }, TaskCreationOptions.LongRunning);
    }

    public async void Start(GateConfig cfg)
    {
        try
        {
            _clientWaitQueue = new();
            _clientReliabilityQueue = new();
            
            _clients = new();
            _redis = new RedisHandle(cfg.RedisUrl, cfg.RedisPwd);
            StartRedisMsg();
            StartRedisReliabilityMsg();

            _internal = new(cfg.PortInternal);
            _internal.OnListenAccept += async network =>
            {
                var rpc = new WRpc();
                _ = new HubMsgHandle(network, rpc, new HubGeneralMsgHandle(_clients, _clientWaitQueue!, _clientReliabilityQueue!, rpc));
                network.OnReceive(rpc.OnNetworkData);
            };
            _internal.Start();

            _external = new(cfg.PortExternal, cfg.Pfx, cfg.PfxPassword);
            _external.OnListenAccept += async network =>
            {
                var rpc = new WRpc();
                
                var netGuid = Guid.NewGuid().ToString();
                await network.Send(rpc.Notify(Consts.NotifyConnId, new NotifyConnID()
                {
                    ConnId = netGuid,
                }));
                await _redis.PushList(cfg.EnterService, rpc.Notify(Consts.EnterGame, new GateForwardClientRequestService()
                {
                    ServiceName  = cfg.EnterService,
                    GateName = cfg.GateId,
                    ConnId = netGuid,
                }));

                var cli = new Client(netGuid, network, _redis);
                _ = new ClientMsgHandle(cfg, rpc, cli, _clientReliabilityQueue);
                network.OnReceive(rpc.OnNetworkData);
                
                _clients.Add(netGuid, cli);
            };
            _external.Start();
            
            await _internal.Join();
            await _external.Join();

            _isRun = false;
            Task.WaitAll(_tWait!, _tWaitReliability!);
        }
        catch (Exception ex)
        {
            Log.Error("gate Main run error:{0}", ex);
        }
    }
    
}