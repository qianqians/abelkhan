using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Google.Protobuf;
using Consul;
using core;
using engine;
using consts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Nito.Collections;

// ReSharper disable FieldCanBeMadeReadOnly.Global
namespace gate;

public struct GateConfig()
{
    public string GateId { get; set; } = string.Empty;
    public string RedisUrl { get; set; } = string.Empty;
    public string RedisPwd { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public ushort PortInternal { get; set; } = 0;
    public ushort PortExternal { get; set; } = 0;
    public ushort PortHealth { get; set; } = 0;
    public string Pfx { get; set; } = string.Empty;
    public string PfxPassword { get; set; } = string.Empty;
    public string ConsulUrl { get; set; } = string.Empty;
    public string EnterService { get; set; } = string.Empty;
    public uint MinVersion { get; set; } = 0;
    public uint MaxVersion { get; set; } = 0;
}

class MainClass
{
    private RedisHandle? _redis;
    private TcpAcceptService? _internal;
    private WebSocketAcceptService? _external;
    private readonly Dictionary<string, Client> _clients = new();
    // ReSharper disable once CollectionNeverQueried.Local
    private List<HubMsgHandle>? _hubs;

    private bool _isRun = true;
    private Task? _tWait;
    private readonly Deque<string> _clientWaitQueue = new();
    private Task? _tWaitReliability;
    private readonly Deque<string> _clientReliabilityQueue = new();
    private readonly TimerService _timer = new();
    private ConsulClient? _consul;

    private void StartRedisMsg()
    {
        _tWait = Task.Factory.StartNew(async () =>
        {
            var rpc = new WRpc();
            lock (_clients)
            {
                var h = new HubGeneralMsgHandle(_clients, _clientWaitQueue, _clientReliabilityQueue, rpc);
                _ = new HubMsgHandle(rpc, h);
            }

            while (_isRun)
            {
                if (_clientWaitQueue == null)
                {
                    await Task.Delay(1);
                    continue;
                }

                string userId = string.Empty;
                lock (_clientWaitQueue)
                {
                    if (_clientWaitQueue.Count > 0)
                    {
                        userId = _clientWaitQueue.RemoveFromFront();
                    }
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
                    
                } while (false);

                lock (_clientWaitQueue)
                {
                    _clientWaitQueue.AddToBack(userId);
                }
            }
        }, TaskCreationOptions.LongRunning).Unwrap();
    }

    private void StartRedisReliabilityMsg()
    {
        _tWaitReliability = Task.Factory.StartNew(async () =>
        {
            var rpc = new WRpc();
            lock(_clients) 
            {
                var h = new HubGeneralMsgHandle(_clients, _clientWaitQueue, _clientReliabilityQueue, rpc);
                _ = new HubMsgHandle(rpc, h);
            }
            
            while (_isRun)
            {
                if (_clientReliabilityQueue == null)
                {
                    await Task.Delay(1);
                    continue;
                }
                
                string userId = string.Empty;
                lock (_clientReliabilityQueue)
                {
                    if (_clientReliabilityQueue.Count > 0)
                    {
                        userId = _clientReliabilityQueue.RemoveFromFront();
                    }
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
                    lock (_clientReliabilityQueue)
                    {
                        _clientReliabilityQueue.AddToBack(userId);
                    }
                    continue;
                }

                OnMqMsg(true, userId, rpc, data);
            }
        }, TaskCreationOptions.LongRunning).Unwrap();
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
        if (userId == ev.UserId)
        {
            var forward = new HubNotifyClientMq()
            {
                EntityId = ev.EntityId,
                Event = ev.Event,
                NeedAck = needAck,
            };

            Client[] cliCopy;
            lock (_clients)
            {
                cliCopy = _clients.Select(kv=>kv.Value).ToArray();
            }
            foreach (var cli in cliCopy)
            {
                if (cli.UserId == userId)
                {
                    _ = cli.SendToClient(rpc.Notify(Consts.HubNotifyClientMq, forward));
                    break;
                }
            }
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
            Name = "gate",
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
            lock (_clients)
            {
                var removeList = _clients
                    .Where((kv, _) => 10_000 < (tick - kv.Value.LastEventTime))
                    .Select(kv => kv.Key)
                    .ToList();

                foreach (var uuid in removeList)
                {
                    if (_clients.Remove(uuid, out var cli))
                    {
                        _ = cli.Close();
                        lock (_clientWaitQueue) while(_clientWaitQueue.Remove(cli.UserId!));
                        lock (_clientReliabilityQueue) while(_clientReliabilityQueue.Remove(cli.UserId!));
                    }
                }
            }
        } while (false);
        
        _timer.AddTickTime(3000, TickClients);
    }

    private async Task Run(GateConfig cfg)
    {
        try
        {
            _hubs = new();
            
            _redis = new RedisHandle(cfg.RedisUrl, cfg.RedisPwd);
            StartRedisMsg();
            StartRedisReliabilityMsg();

            _internal = new(cfg.PortInternal);
            _internal.OnListenAccept += network =>
            {
                var rpc = new WRpc();
                lock (_clients)
                {
                    var h = new HubGeneralMsgHandle(_clients, _clientWaitQueue, _clientReliabilityQueue, rpc);
                    _hubs.Add(new HubMsgHandle(network, rpc, h));
                }
                network.OnReceive(rpc.OnNetworkData);
            };
            _internal.Start();

            _external = new(cfg.PortExternal, cfg.Pfx, cfg.PfxPassword);
            _external.OnListenAccept += async network =>
            {
                try
                {
                    var rpc = new WRpc();
                    var netGuid = Guid.NewGuid().ToString();
                    var cli = new Client(netGuid, network, _redis);
                    lock (_clientReliabilityQueue)
                    {
                        _ = new ClientMsgHandle(cfg, _redis, rpc, cli, _clientReliabilityQueue);
                    }
                    network.OnReceive(rpc.OnNetworkData);

                    await network.Send(rpc.Notify(Consts.NotifyConnId, new NotifyConnID()
                    {
                        ConnId = netGuid,
                    }));
                    await _redis.PushList(cfg.EnterService, rpc.Notify(Consts.GateForwardClientRequestService,
                        new GateForwardClientRequestService()
                        {
                            ServiceName = cfg.EnterService,
                            GateName = cfg.GateId,
                            ConnId = netGuid,
                        })
                    );
                    
                    lock (_clients)
                    {
                        _clients.Add(netGuid, cli);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"gate: {cfg.GateId} {ex}");
                }
            };
            _external.Start();

            var app = WebApplication.Create();
            app.MapGet("/health", () => Results.Ok("healthy"));
            _ = app.RunAsync($"http://{cfg.Ip}:{cfg.PortHealth}");
            
            await ReportServiceConsul(cfg);
            
            _timer.AddTickTime(3000, TickClients);
            while (_isRun)
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

            await _tWait!;
            await _tWaitReliability!;
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
        var ex = e.ExceptionObject as Exception;
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
        instance.Run(cfg).Wait();
    }
}