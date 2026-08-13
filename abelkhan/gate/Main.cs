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
    // ReSharper disable once CollectionNeverQueried.Local
    private List<HubMsgHandle>? _hubs;

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

                if (_clientWaitQueue == null || !_clientWaitQueue.TryDequeue(out var accountId))
                {
                    await Task.Delay(1);
                    continue;
                }
                if (string.IsNullOrEmpty(accountId))
                {
                    await Task.Delay(1);
                    continue;
                }

                do
                {
                    var data = await _redis?.PopList(string.Format(Consts.EntityClientMq, accountId), 8)!;
                    if (data == null)
                    {
                        await Task.Delay(1);
                        break;
                    }

                    var hasCli = true;
                    foreach (var msg in data)
                    {
                        if (!OnMqMsg(false, accountId, rpc, msg))
                        {
                            hasCli = false;
                        }
                    }
                    if (!hasCli)
                    {
                        break;
                    }
                    
                    _clientWaitQueue.Enqueue(accountId);
                    
                } while (false);
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
                if (_clientReliabilityQueue == null || !_clientReliabilityQueue.TryDequeue(out var accountId))
                {
                    await Task.Delay(1);
                    continue;
                }
                if (string.IsNullOrEmpty(accountId))
                {
                    await Task.Delay(1);
                    continue;
                }
                
                var data = await _redis?.Front(string.Format(Consts.EntityReliabilityClientMq, accountId))!;
                if (data == null)
                {
                    await Task.Delay(1);
                    continue;
                }

                OnMqMsg(true, accountId, rpc, data);
            }
        }, TaskCreationOptions.LongRunning);
    }

    private bool OnMqMsg(bool needAck, string accountId, WRpc rpc, byte[] data)
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
        var forward = new HubNotifyClientMq()
        {
            EntityId = ev.EntityId,
            Event = ev.Event,
            NeedAck = needAck,
        };
        if (accountId == ev.AccountId && _entityClients!.TryGetValue(accountId, out var cli))
        {
            _ = cli.SendToClient(rpc.Notify(Consts.HubNotifyClientMq, forward));
        }
        else
        {
            return false;
        }
        
        return true;
    }

    public async void Run(GateConfig cfg)
    {
        try
        {
            _clientWaitQueue = new();
            _clientReliabilityQueue = new();
            
            _clients = new();
            _entityClients = new Dictionary<string, Client>();
            _hubs = new();
            
            _redis = new RedisHandle(cfg.RedisUrl, cfg.RedisPwd);
            StartRedisMsg();
            StartRedisReliabilityMsg();

            _internal = new(cfg.PortInternal);
            _internal.OnListenAccept += network =>
            {
                var rpc = new WRpc();
                var h = new HubGeneralMsgHandle(_clients, _entityClients, _clientWaitQueue, _clientReliabilityQueue, rpc);
                _hubs.Add(new HubMsgHandle(network, rpc, h));
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

                lock (_clients)
                {
                    _clients.Add(netGuid, cli);
                }
            };
            _external.Start();

            var timer = new TimerService();
            timer.AddTickTime(3000, (tick) =>
            {
                lock (_clients)
                {
                    var removeList = _clients
                        .Where((kv, _) => 5000 < (tick-kv.Value.LastEventTime))
                        .Select(kv => kv.Key)
                        .ToList();
                    
                    foreach (var uuid in removeList)
                    {
                        if (_clients.Remove(uuid, out var cli))
                        {
                            var removeEntity = _entityClients
                                .Where((kv, _) => kv.Value.ConnId == cli.ConnId)
                                .Select(kv => kv.Key)
                                .ToList();
                            foreach (var entityId in removeEntity)
                            {
                                _entityClients.Remove(entityId);
                            }
                        }
                    }
                }
            });
            while (!_isRun)
            {
                var begin = TimerService.Tick;
                timer.Poll();
                var detail = TimerService.Tick - begin;
                if (detail < 16)
                {
                    await Task.Delay((int)(16-detail));
                }
            }
            
            await _internal.Join();
            await _external.Join();

            Task.WaitAll(_tWait!, _tWaitReliability!);
        }
        catch (Exception ex)
        {
            Log.Error("gate Main run error:{0}", ex);
        }
    }

    public void Stop()
    {
        _isRun = false;
    }
}