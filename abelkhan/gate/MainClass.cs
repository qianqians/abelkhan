using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Google.Protobuf;
using Consul;
using core;
using engine;
using consts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

// ReSharper disable FieldCanBeMadeReadOnly.Global
namespace gate;

public struct GateConfig()
{
    public readonly string GateId = string.Empty;
    public readonly string RedisUrl = string.Empty;
    public readonly string RedisPwd  = string.Empty;
    public readonly string Ip = string.Empty;
    public readonly ushort PortInternal = 0;
    public readonly ushort PortExternal = 0;
    public readonly ushort PortHealth = 0;
    public readonly string Pfx = string.Empty;
    public readonly string PfxPassword  = string.Empty;
    public readonly string ConsulUrl  = string.Empty;
    public readonly string EnterService = string.Empty;
    public readonly uint MinVersion = 0;
    public readonly uint MaxVersion = 0;
}

class MainClass
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
    private readonly TimerService _timer = new();
    private ConsulClient? _consul;

    private void StartRedisMsg()
    {
        _tWait = Task.Factory.StartNew(async () =>
        {
            while (_isRun)
            {
                var rpc = new WRpc();
                var h = new HubGeneralMsgHandle(_clients!, _entityClients!, _clientWaitQueue!, _clientReliabilityQueue!, rpc);
                _ = new HubMsgHandle(rpc, h);

                if (_clientWaitQueue == null || !_clientWaitQueue.TryDequeue(out var userId))
                {
                    await Task.Delay(1);
                    continue;
                }
                if (string.IsNullOrEmpty(userId))
                {
                    await Task.Delay(1);
                    continue;
                }

                do
                {
                    var data = await _redis?.PopList(string.Format(Consts.EntityClientMq, userId), 8)!;
                    if (data == null)
                    {
                        await Task.Delay(1);
                        break;
                    }

                    var hasCli = true;
                    foreach (var msg in data)
                    {
                        if (!OnMqMsg(false, userId, rpc, msg))
                        {
                            hasCli = false;
                        }
                    }
                    if (!hasCli)
                    {
                        break;
                    }
                    
                    _clientWaitQueue.Enqueue(userId);
                    
                } while (false);
            }
        }, TaskCreationOptions.LongRunning);
    }

    private void StartRedisReliabilityMsg()
    {
        _tWaitReliability = Task.Factory.StartNew(async () =>
        {
            var rpc = new WRpc();
            var h = new HubGeneralMsgHandle(_clients!, _entityClients!, _clientWaitQueue!, _clientReliabilityQueue!, rpc);
            _ = new HubMsgHandle(rpc, h);

            while (_isRun)
            {
                if (_clientReliabilityQueue == null || !_clientReliabilityQueue.TryDequeue(out var userId))
                {
                    await Task.Delay(1);
                    continue;
                }
                if (string.IsNullOrEmpty(userId))
                {
                    await Task.Delay(1);
                    continue;
                }
                
                var data = await _redis?.Front(string.Format(Consts.EntityReliabilityClientMq, userId))!;
                if (data == null)
                {
                    await Task.Delay(1);
                    continue;
                }

                OnMqMsg(true, userId, rpc, data);
            }
        }, TaskCreationOptions.LongRunning);
    }

    private bool OnMqMsg(bool needAck, string userId, WRpc rpc, byte[] data)
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
        if (userId == ev.UserId && _entityClients!.TryGetValue(userId, out var cli))
        {
            _ = cli.SendToClient(rpc.Notify(Consts.HubNotifyClientMq, forward));
        }
        else
        {
            return false;
        }
        
        return true;
    }

    private async Task ReportServiceConsul(GateConfig cfg)
    {
        _consul = new (c =>
        {
            c.Address = new Uri(cfg.ConsulUrl);
        });
        var registration = new AgentServiceRegistration
        {
            ID = cfg.GateId,
            Name = "Gateway",
            Address = cfg.Ip,
            Port = cfg.PortInternal,
            Tags = ["v1", "api"],
            Check = new AgentServiceCheck
            {
                DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                Interval = TimeSpan.FromSeconds(10),
                HTTP = $"http://{cfg.Ip}:{cfg.PortHealth}/health",
                Timeout = TimeSpan.FromSeconds(5)
            }
        };
        await _consul.Agent.ServiceRegister(registration);
    }

    private void TickClients(long tick)
    {
        do
        {
            if (_clients == null)
            {
                break;
            }

            lock (_clients)
            {
                var removeList = _clients
                    .Where((kv, _) => 5000 < (tick - kv.Value.LastEventTime))
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
        } while (false);
        
        _timer.AddTickTime(3000, TickClients);
    }

    private async void Run(GateConfig cfg)
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

            _timer.AddTickTime(3000, TickClients);
            
            var app = WebApplication.Create();
            app.MapGet("/health", () => Results.Ok("healthy"));
            _ = app.RunAsync($"http://{cfg.Ip}:{cfg.PortHealth}");
            
            await ReportServiceConsul(cfg);
            
            while (!_isRun)
            {
                var begin = TimerService.Tick;
                _timer.Poll();
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

    private void Stop()
    {
        _isRun = false;
    }
    
    void HandleSignal(PosixSignalContext context)
    {
        context.Cancel = true;
        Stop();
    }
    
    static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as System.Exception;
        Log.Error($"not handle exception:{ex}");
    }
    
    public static void Main(string[] args)
    {
        FileStream fs = File.OpenRead(args[0]);
        byte[] data = new byte[fs.Length];
        int offset = 0;
        int remaining = data.Length;
        while (remaining > 0)
        {
            int read = fs.Read(data, offset, remaining);
            if (read <= 0)
            {
                throw new EndOfStreamException($"file read at:{read} failed");
            }
            remaining -= read;
            offset += read;
        }
        var cfg = Newtonsoft.Json.JsonConvert.DeserializeObject<GateConfig>(System.Text.Encoding.Default.GetString(data));
        
        AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        
        var instance = new MainClass();
        using var sigTermReg = PosixSignalRegistration.Create(PosixSignal.SIGTERM, instance.HandleSignal);
        using var sigIntReg = PosixSignalRegistration.Create(PosixSignal.SIGINT, instance.HandleSignal);
        instance.Run(cfg);
    }
}