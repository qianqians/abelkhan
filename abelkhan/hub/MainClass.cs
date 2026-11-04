using System.Collections.Concurrent;
using System.Net;
using System.Runtime.InteropServices;
using consts;
using Consul;
using core;
using engine;
using leaderboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using shop;

namespace hub;

public struct HubConfig()
{
    public string HubId { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public ushort PortHealth { get; set; } = 0;
    public string ConsulUrl { get; set; } = string.Empty;
    public string RedisUrl { get; set; } = string.Empty;
    public string RedisPwd { get; set; } = string.Empty;

    /// <summary>
    /// 商城配置。admin 的 Shop 配置必须与这里一致（KeyPrefix / 货币），
    /// 否则后台“上架”的商品游戏服看不到。
    /// </summary>
    public ShopSettings Shop { get; set; } = new();

    /// <summary>多久扫一次到期商品（毫秒），默认 5 秒。</summary>
    public int ShopSettleIntervalMs { get; set; } = 5000;

    /// <summary>每轮最多结算多少条，避免一次占用 Redis 太久。</summary>
    public int ShopSettleBatchSize { get; set; } = 200;

    /// <summary>排行榜配置。多游戏共用一个 Redis 时用 KeyPrefix 隔离。</summary>
    public LeaderboardSettings Leaderboard { get; set; } = new();
}

public class MainClass
{
    private RedisHandle? _redis;
    private readonly ConcurrentDictionary<string, Service>  _services = new();
    private readonly ConcurrentDictionary<string, BaseEntity> _entities = new();
    // ReSharper disable once CollectionNeverQueried.Local
    private readonly ConcurrentDictionary<string, GateMsgHandle> _gateMsgHandles = new();
    // ReSharper disable once CollectionNeverQueried.Local
    private readonly ConcurrentDictionary<string, Client> _clients = new();
    // ReSharper disable once CollectionNeverQueried.Local
    private readonly ConcurrentDictionary<string, GateNetwork> _gates = new();
    private readonly TcpConnectService _serviceGate = new();
    private readonly TimerService _timer = new();
    private GlobalMailBox? _serviceMailBox;
    private ShopStore? _shopStore;
    private LeaderboardStore? _leaderboardStore;
    private int _shopSettling;
    
    private Task? _tWait;
    private readonly ConcurrentQueue<string> _gateWaitQueue = new();
    
    private bool _isRun = true;
    private ConsulClient? _consul;
    private ConsulServiceWatcher? _serviceWatcher;
    
    public event Action<Client>? OnClientConnected;
    private void TickClients(long tick)
    {
        do
        {
            var removeList = _clients
                .Where((kv, _) => 8_000 < (tick - kv.Value.LastEventTime))
                .Select(kv => kv.Key)
                .ToArray();

            foreach (var uuid in removeList)
            {
                if (_clients.Remove(uuid, out var cli))
                {
                    OnClientConnected?.Invoke(cli);
                }
            }
        } while (false);
        
        _timer.AddTickTime(3000, TickClients);
    }
    
    public void RegisterService(string serviceName, Service service)
    {
        _services[serviceName] = service;
        if (_serviceMailBox != null)
        {
            service.GlobalMailBox = _serviceMailBox;
        }

        if (_shopStore != null)
        {
            service.Shop = _shopStore;
        }

        if (_leaderboardStore != null)
        {
            service.Leaderboard = _leaderboardStore;
        }
    }

    private void OnReconnect(string userId, string gateName, string connId)
    {
        var cli = new Client(userId, gateName, connId);
        _ = _clients.AddOrUpdate(userId, cli, (_, _) => cli);
    }

    // ReSharper disable once AsyncVoidMethod
    private async void OnRequestService(string serviceName, string gateName, string connId, byte[] data)
    {
        try
        {
            if (_services.TryGetValue(serviceName, out var service))
            {
                var e = await service.EchoQueryServiceEntity(gateName, connId, data);
                _entities.TryAdd(e.EntityId, e);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"OnRequestService:{ex}");
        }
    }
    
    private void StartRedisMsg()
    {
        _tWait = Task.Factory.StartNew(async () =>
        {
            var rpc = new WRpc();
            var h = new GateMsgMqHandle(rpc);
            h.OnReconnect += OnReconnect;
            h.OnRequestService += OnRequestService;

            while (_isRun)
            {
                if (_gateWaitQueue == null || !_gateWaitQueue.TryDequeue(out var entityId))
                {
                    await Task.Delay(1);
                    continue;
                }
                if (string.IsNullOrEmpty(entityId))
                {
                    await Task.Delay(1);
                    continue;
                }

                do
                {
                    var data = await _redis?.PopList(string.Format(Consts.EntityServerMq, entityId), 8)!;
                    if (data == null || data.Count == 0)
                    {
                        await Task.Delay(1);
                        break;
                    }

                    foreach (var msg in data)
                    {
                        rpc.OnNetworkData(msg);
                    }
                    
                } while (false);
                    
                _gateWaitQueue.Enqueue(entityId);
            }
        }, TaskCreationOptions.LongRunning).Unwrap();
    }
    
    private async Task ReportServiceConsul(HubConfig cfg)
    {
        _consul = new (c =>
        {
            c.Address = new Uri(cfg.ConsulUrl);
        });
        var registration = new AgentServiceRegistration
        {
            ID = cfg.HubId,
            Name = cfg.ServiceName,
            Address = cfg.Ip,
            Port = -1,
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

    /// <summary>
    /// 定时结算到期的一口价寄拍 / 限时拍卖。结算本身是 Lua 原子的，多 hub 同时扫也只会成功一次；
    /// 这里用 Task.Run 避免 Redis 抖动阻塞主循环。
    /// </summary>
    private void TickShop(long tick, int intervalMs, int batchSize)
    {
        var store = _shopStore;
        // 上一次结算还没跑完就跳过这一轮，避免 Redis 抖动时任务堆积
        if (store != null && Interlocked.CompareExchange(ref _shopSettling, 1, 0) == 0)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await store.SettleExpiredAsync(Math.Max(1, batchSize),
                        DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                }
                catch (Exception e)
                {
                    Log.Error("shop settle tick error:{0}", e);
                }
                finally
                {
                    Interlocked.Exchange(ref _shopSettling, 0);
                }
            });
        }

        _timer.AddTickTime(intervalMs, t => TickShop(t, intervalMs, batchSize));
    }

    void HandleSignal(PosixSignalContext context)
    {
        context.Cancel = true;
        Stop();
    }
    
    private void Stop()
    {
        _isRun = false;
        _serviceGate.Close();
    }
    
    private async Task Run(HubConfig cfg)
    {
        try
        {
            _redis = new RedisHandle(cfg.RedisUrl, cfg.RedisPwd);

            // 全局邮件箱：所有 Service 共用一个（共享 Redis 连接），
            // 玩家通过「客户端请求领取邮件」协议按需读取
            _serviceMailBox = new GlobalMailBox(_redis, Consts.AdminMailGlobalListKey, Consts.AdminMailClaimedKey);

            // 商城：商品目录 / 寄拍 / 拍卖都存 Redis，admin 上架时写的是同一个库
            _shopStore = new ShopStore(_redis, cfg.Shop);
            Log.Info("hub: shop key prefix:{0} currency:{1}",
                cfg.Shop.KeyPrefix, cfg.Shop.CurrencyItemId);

            // 排行榜：每个榜一个 Redis ZSET
            _leaderboardStore = new LeaderboardStore(_redis, cfg.Leaderboard);
            Log.Info("hub: leaderboard key prefix:{0} maxEntries:{1}",
                cfg.Leaderboard.KeyPrefix, cfg.Leaderboard.MaxEntries);

            foreach (var service in _services.Values)
            {
                service.GlobalMailBox = _serviceMailBox;
                service.Shop = _shopStore;
                service.Leaderboard = _leaderboardStore;
            }

            StartRedisMsg();
            
            var app = WebApplication.Create();
            app.MapGet("/health", () => Results.Ok("healthy"));
            _ = app.RunAsync($"http://{cfg.Ip}:{cfg.PortHealth}");
            await ReportServiceConsul(cfg);

            _serviceGate.OnConnect += (id, network) =>
            {
                var rpc = new WRpc();
                var gate = new GateNetwork(network);
                _gates[id] = gate;
                var handle = new GateMsgHandle(rpc, gate, _entities);
                _gateMsgHandles[id] = handle;
                network.OnReceive(rpc.OnNetworkData);
            };
                
            _serviceWatcher = new(_consul!);
            _serviceWatcher.OnNewService += (string serviceName, string id, string ip, ushort port) =>
            {
                if (serviceName.Equals("gate", StringComparison.OrdinalIgnoreCase))
                {
                    _serviceGate.Connect(id, IPAddress.Parse(ip), port);
                }
            };
            using var cts = new CancellationTokenSource();
            var stoppingToken = cts.Token;
            _ = _serviceWatcher.ExecuteAsync(stoppingToken);

            _timer.AddTickTime(3000, TickClients);
            _timer.AddTickTime(Math.Max(1000, cfg.ShopSettleIntervalMs),
                tick => TickShop(tick, Math.Max(1000, cfg.ShopSettleIntervalMs), cfg.ShopSettleBatchSize));
            while (_isRun)
            {
                var begin = TimerService.Tick;
                _timer.Poll();
                var detail = TimerService.Tick - begin;
                if (detail < 16)
                {
                    // ReSharper disable once MethodSupportsCancellation
                    await Task.Delay((int)(16 - detail));
                }
            }
            
            await cts.CancelAsync();
            await _tWait!;
        }
        catch (Exception ex)
        {
            Log.Error("hub Main run error:{0}", ex);
        }
    }

    void UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as System.Exception;
        Log.Error($"not handle exception:{ex}");
    }
    
    public void RunMain(string[] args)
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
        var cfg = Newtonsoft.Json.JsonConvert.DeserializeObject<HubConfig>(System.Text.Encoding.Default.GetString(data));

        AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        
        using var sigTermReg = PosixSignalRegistration.Create(PosixSignal.SIGTERM, HandleSignal);
        using var sigIntReg = PosixSignalRegistration.Create(PosixSignal.SIGINT, HandleSignal);
        Run(cfg).Wait();
    }
}