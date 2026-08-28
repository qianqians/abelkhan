using System.Collections.Concurrent;
using System.Net;
using System.Runtime.InteropServices;
using consts;
using Consul;
using core;
using engine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

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
    private readonly TcpConnectService _serviceDb = new();
    private readonly TimerService _timer = new();
    
    private Task? _tWait;
    private readonly ConcurrentQueue<string> _gateWaitQueue = new();
    
    private bool _isRun = true;
    private ConsulClient? _consul;
    private ConsulServiceWatcher? _serviceWatcher;
    
    private void TickClients(long tick)
    {
        do
        {
            var removeList = _clients
                .Where((kv, _) => 8_000 < (tick - kv.Value.LastEventTime))
                .Select(kv => kv.Key)
                .ToList();

            foreach (var uuid in removeList)
            {
                if (_clients.Remove(uuid, out var _))
                {
                }
            }
        } while (false);
        
        _timer.AddTickTime(3000, TickClients);
    }
    
    public void RegisterService(string serviceName, Service service)
    {
        _services[serviceName] = service;
    }

    private void OnReconnect(string userId, string gateName, string connId)
    {
        var cli = new Client(userId, gateName, connId);
        _clients.AddOrUpdate(userId, cli, (_, _) => cli);
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
                    if (data == null)
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

    void HandleSignal(PosixSignalContext context)
    {
        context.Cancel = true;
        Stop();
    }
    
    private void Stop()
    {
        _isRun = false;
    }
    
    private async Task Run(HubConfig cfg)
    {
        try
        {
            _redis = new RedisHandle(cfg.RedisUrl, cfg.RedisPwd);
            StartRedisMsg();
            
            var app = WebApplication.Create();
            app.MapGet("/health", () => Results.Ok("healthy"));
            _ = app.RunAsync($"http://{cfg.Ip}:{cfg.PortHealth}");
            await ReportServiceConsul(cfg);

            _serviceGate.OnConnect += (id, network) =>
            {
                var rpc = new WRpc();
                var gate = new GateNetwork(network);
                _gates.TryAdd(id, gate);
                var handle = new GateMsgHandle(rpc, gate, _entities);
                _gateMsgHandles.TryAdd(id, handle);
                network.OnReceive(rpc.OnNetworkData);
            };
            _serviceDb.OnConnect += (id, network) =>
            {
            }; 
                
            _serviceWatcher = new(_consul!);
            _serviceWatcher.OnNewService += (string serviceName, string id, string ip, ushort port) =>
            {
                if (serviceName.Equals("gate", StringComparison.OrdinalIgnoreCase))
                {
                    _serviceGate.Connect(id, IPAddress.Parse(ip), port);
                }
                else if (serviceName.Equals("db_proxy", StringComparison.OrdinalIgnoreCase))
                {
                    _serviceDb.Connect(id, IPAddress.Parse(ip), port);
                }
            };
            using var cts = new CancellationTokenSource();
            var stoppingToken = cts.Token;
            _ = _serviceWatcher.ExecuteAsync(stoppingToken);

            _timer.AddTickTime(3000, TickClients);
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