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
    // 可靠消息投出后等 ack 的超时：超时还没等到 ack 就重新入队再投一次
    private const long ReliabilityAckTimeoutMs = 3000;
    private readonly Dictionary<string, long> _reliabilityAckDeadline = new();
    private long _lastReliabilityTimeoutCheck;
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
                    if (data == null || data.Count == 0)
                    {
                        await Task.Delay(1);
                        break;
                    }

                    foreach (var msg in data)
                    {
                        if (!OnMqMsg(false, userId, rpc, msg))
                        {
                            Log.Error("OnMqMsg return false userId:{0}", userId);
                        }
                    }
                } while (false);

                lock (_clientWaitQueue)
                {
                    if (!_clientWaitQueue.Contains(userId))
                    {
                        _clientWaitQueue.AddToBack(userId);
                    }
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
                // 超时没等到 ack 的可靠消息，重新入队重投
                RetryReliabilityTimeout();

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
                if (data == null || data.Length == 0)
                {
                    await Task.Delay(1);
                    lock (_clientReliabilityQueue)
                    {
                        if (!_clientReliabilityQueue.Contains(userId))
                        {
                            _clientReliabilityQueue.AddToBack(userId);
                        }
                    }
                    continue;
                }

                OnMqMsg(true, userId, rpc, data);
                // 记下等 ack 的截止时间：客户端 ack 会把它从 Redis 出队；
                // 超时还没等到 ack，就由 RetryReliabilityTimeout 重新入队再投一次
                lock (_reliabilityAckDeadline)
                {
                    _reliabilityAckDeadline[userId] = TimerService.Tick + ReliabilityAckTimeoutMs;
                }
            }
        }, TaskCreationOptions.LongRunning).Unwrap();
    }

    // 投出去的可靠消息超过 ReliabilityAckTimeoutMs 还没等到 ack（客户端掉线或 ack 丢失），
    // 就把 userId 重新入队再投一次：否则 userId 只能靠 ack 回到队列，ack 一丢这个用户就永远不再投递
    private void RetryReliabilityTimeout()
    {
        var now = TimerService.Tick;
        if (now - _lastReliabilityTimeoutCheck < 100)
        {
            return;
        }
        _lastReliabilityTimeoutCheck = now;

        List<string>? timeoutUserIds = null;
        lock (_reliabilityAckDeadline)
        {
            foreach (var (userId, deadline) in _reliabilityAckDeadline)
            {
                if (now < deadline)
                {
                    continue;
                }

                timeoutUserIds ??= new List<string>();
                timeoutUserIds.Add(userId);
            }

            if (timeoutUserIds != null)
            {
                foreach (var userId in timeoutUserIds)
                {
                    _reliabilityAckDeadline.Remove(userId);
                }
            }
        }

        if (timeoutUserIds == null)
        {
            return;
        }

        lock (_clientReliabilityQueue)
        {
            foreach (var userId in timeoutUserIds)
            {
                // 去重：ack 可能刚好把它放回队列了
                if (!_clientReliabilityQueue.Contains(userId))
                {
                    _clientReliabilityQueue.AddToBack(userId);
                }
            }
        }
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
                        if (!string.IsNullOrEmpty(cli.UserId))
                        {
                            // 连接已经清理了，别再把它当成超时重投的对象
                            lock (_reliabilityAckDeadline) _reliabilityAckDeadline.Remove(cli.UserId);
                        }
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
        _internal?.Close();
        _ = _external?.Close();
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