using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Google.Protobuf;
using Consul;
using core;
using engine;
using consts;
using friend;
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

    /// <summary>
    /// 好友 / 在线状态参数。KeyPrefix 与 PresenceTtlMs 必须与 hub 的 HubConfig.Friend 一致，
    /// 否则 gate 写的 presence 与 hub 读的不是同一个 key。
    /// </summary>
    public FriendSettings Friend { get; set; } = new();
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
    // （调大是为了少制造重复副本：重投产生的重复 ack 目前只能靠 entity_id 兜一部分）
    private const long ReliabilityAckTimeoutMs = 10_000;
    private readonly Dictionary<string, long> _reliabilityAckDeadline = new();
    private long _lastReliabilityTimeoutCheck;
    private readonly TimerService _timer = new();
    private ConsulClient? _consul;
    private string _gateId = string.Empty;

    /// <summary>好友 / presence 存储：gate 只用来刷新和释放在线标记。</summary>
    private FriendStore? _friend;
    private int _presenceRefreshing;

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
                
                try
                {
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
                catch (Exception e)
                {
                    // 兜底：队头脏数据（ParseFrom 抛异常）之类的问题不能打死整个可靠投递线程，
                    // 否则超时重投也没人执行了。当作没投出去，交给超时扫描重新入队。
                    Log.Error($"gate: reliability msg userId:{userId} err:{e}");
                    lock (_reliabilityAckDeadline)
                    {
                        _reliabilityAckDeadline[userId] = TimerService.Tick + ReliabilityAckTimeoutMs;
                    }
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
                // 可靠消息的序号（hub 赋的）原样转给客户端，客户端 ack 时回填到 AckReliabilityMsg.seq
                Seq = ev.Seq,
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
        List<Client> dropped = new();
        do
        {
            int onlinePlayers = 0;
            int onlineConnections = 0;
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
                            dropped.Add(cli);
                        }
                    }
                }

                onlineConnections = _clients.Count;
                onlinePlayers = _clients.Values.Count(c => !string.IsNullOrEmpty(c.UserId));
            }

            // 丢弃的连接：释放 presence + 通知实体所属 hub（好友下线事件）
            foreach (var cli in dropped)
            {
                ReleasePresence(cli);
                NotifyDisconnect(cli);
            }

            // 在线玩家的 presence 续期（PRESENCE TTL 比这个间隔长，gate 挂掉后会自动过期）
            RefreshPresence();

            // 周期上报到 Redis，供 admin dashboard 汇总全局在线数
            _ = _redis?.SetStrData(string.Format(Consts.AdminOnlineGateKey, _gateId), onlinePlayers.ToString(), 10_000);
            _ = _redis?.SetStrData(string.Format(Consts.AdminOnlineConnectionKey, _gateId), onlineConnections.ToString(), 10_000);
        } while (false);
        
        _timer.AddTickTime(3000, TickClients);
    }

    /// <summary>为所有在线玩家刷新 presence（值为本 gateId，TTL 到期即视为离线）。</summary>
    private void RefreshPresence()
    {
        var store = _friend;
        if (store == null || Interlocked.CompareExchange(ref _presenceRefreshing, 1, 0) != 0)
        {
            // 上一轮还没跑完就跳过，避免 Redis 抖动时任务堆积
            return;
        }

        // presence 的值要带 entityId：好友 / 跨 hub 推消息时靠它路由到客户端上的实体
        (string UserId, string EntityId)[] online;
        lock (_clients)
        {
            online = _clients.Values
                .Where(c => !string.IsNullOrEmpty(c.UserId) && !string.IsNullOrEmpty(c.EntityId))
                .Select(c => (c.UserId!, c.EntityId!))
                .Distinct()
                .ToArray();
        }

        _ = Task.Run(async () =>
        {
            try
            {
                foreach (var (userId, entityId) in online)
                {
                    await store.SetPresenceAsync(userId, _gateId, entityId);
                }
            }
            catch (Exception e)
            {
                Log.Error("gate: refresh presence err:{0}", e);
            }
            finally
            {
                Interlocked.Exchange(ref _presenceRefreshing, 0);
            }
        });
    }

    /// <summary>释放某个连接的 presence（只删属于本连接的那份标记，避免误删同一玩家的新连接）。</summary>
    private void ReleasePresence(Client cli)
    {
        var store = _friend;
        if (store == null || string.IsNullOrEmpty(cli.UserId) || string.IsNullOrEmpty(cli.EntityId))
        {
            return;
        }

        var userId = cli.UserId!;
        var entityId = cli.EntityId!;
        _ = Task.Run(async () =>
        {
            try
            {
                await store.ReleasePresenceAsync(userId, _gateId, entityId);
            }
            catch (Exception e)
            {
                Log.Error("gate: release presence err:{0}", e);
            }
        });
    }

    /// <summary>把下线通知发给实体所属 hub：有直连网络就直接投递，否则落到 entity_{id}_server_mq。</summary>
    private void NotifyDisconnect(Client cli)
    {
        if (string.IsNullOrEmpty(cli.EntityId) || string.IsNullOrEmpty(cli.UserId))
        {
            Log.Info("gate: client {0} disconnected without entity", cli.ConnId);
            return;
        }

        var rpc = new WRpc();
        var msg = rpc.Notify(Consts.ClientDisconnect, new ClientDisconnect
        {
            ConnId = cli.ConnId,
            UserId = cli.UserId!,
            EntityId = cli.EntityId!,
        });
        _ = cli.SendToServer(cli.EntityId, msg);
    }

    private async Task Run(GateConfig cfg)
    {
        try
        {
            _hubs = new();
            _gateId = cfg.GateId;
            
            _redis = new RedisHandle(cfg.RedisUrl, cfg.RedisPwd);
            // gate 侧只要 presence（在线标记）：和 hub 共用同一个 Redis / KeyPrefix
            _friend = new FriendStore(_redis, cfg.Friend);
            Log.Info("gate: friend presence prefix:{0} ttl:{1}ms",
                cfg.Friend.KeyPrefix, cfg.Friend.PresenceTtlMs);
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