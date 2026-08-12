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
    private Dictionary<string, Client>? _entityClients; 

    private bool _isRun = true;
    private Task? _tWait;
    private ConcurrentQueue<string>? _clientWaitQueue;
    private Task? _tWaitReliability;
    private ConcurrentQueue<string>? _clientReliabilityQueue;

    private void StartRedisMsg()
    {
        _tWait = Task.Factory.StartNew(async () =>
        {
            while (_isRun)
            {
                var rpc = new WRpc();
                _ = new HubMsgHandle(rpc, new HubGeneralMsgHandle(_clients!, _entityClients!, _clientWaitQueue!, _clientReliabilityQueue!, rpc));

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
                    _clientWaitQueue.Enqueue(playerId);
                    await Task.Delay(1);
                    continue;
                }
                
                foreach (var m in msg)
                {
                    if (!OnMqMsg(rpc, m))
                    {
                        await Task.Delay(1);
                    }
                }

                _clientWaitQueue.Enqueue(playerId);
            }
        }, TaskCreationOptions.LongRunning);
    }

    private void StartRedisReliabilityMsg()
    {
        _tWaitReliability = Task.Factory.StartNew(async () =>
        {
            var rpc = new WRpc();
            _ = new HubMsgHandle(rpc, new HubGeneralMsgHandle(_clients!, _entityClients!, _clientWaitQueue!, _clientReliabilityQueue!, rpc));

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
                if (!OnMqMsg(rpc, data))
                {
                    await Task.Delay(1);
                }
            }
        }, TaskCreationOptions.LongRunning);
    }

    private bool OnMqMsg(WRpc rpc, byte[] data)
    {
        var parser = new MessageParser<Msg>(() => new Msg());
        var msg = parser.ParseFrom(data);
        if (msg == null)
        {
            return false;
        }
        if (msg.PayloadCase != Msg.PayloadOneofCase.Notify)
        {
            return false;
        }
        if (msg.Notify.Event.ProtoName != Consts.GateForwardHubNotifyClientMq)
        {
            return false;
        }
                
        var ev = rpc.OnMsg<GateForwardHubNotifyClientMq>(msg.Notify.Event.Content.ToByteArray());
        var forward = new HubNotifyClient()
        {
            EntityId = ev.EntityId,
            Event = ev.Event,
        };
        if (_entityClients!.TryGetValue(ev.EntityId, out var cli))
        {
            _ = cli.SendToClient(rpc.Notify(Consts.HubNotifyClient, forward));
        }
        
        return true;
    }

    public async void Start(GateConfig cfg)
    {
        try
        {
            _clientWaitQueue = new();
            _clientReliabilityQueue = new();
            
            _clients = new();
            _entityClients = new Dictionary<string, Client>();
            _redis = new RedisHandle(cfg.RedisUrl, cfg.RedisPwd);
            StartRedisMsg();
            StartRedisReliabilityMsg();

            _internal = new(cfg.PortInternal);
            _internal.OnListenAccept += async network =>
            {
                var rpc = new WRpc();
                _ = new HubMsgHandle(network, rpc, new HubGeneralMsgHandle(_clients, _entityClients!, _clientWaitQueue!, _clientReliabilityQueue!, rpc));
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
                _ = new ClientMsgHandle(cfg, _redis, rpc, cli, _clientReliabilityQueue);
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