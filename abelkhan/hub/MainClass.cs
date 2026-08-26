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
    public readonly string HubId = string.Empty;
    public readonly string ServiceName = string.Empty;
    public readonly string Ip = string.Empty;
    public readonly ushort PortHealth = 0;
    public readonly string ConsulUrl  = string.Empty;
    public readonly string RedisUrl = string.Empty;
    public readonly string RedisPwd  = string.Empty;
}

public class MainClass
{
    private RedisHandle? _redis;
    private readonly Dictionary<string, BaseEntity> _entities = new();
    // ReSharper disable once CollectionNeverQueried.Local
    private readonly ConcurrentDictionary<string, GateMsgHandle> _gateMsgHandles = new();
    private readonly ConcurrentDictionary<string, Client> _clients = new();
    private readonly ConcurrentDictionary<string, GateNetwork> _gates = new();
    private readonly TcpConnectService _serviceGate = new();
    private readonly TcpConnectService _serviceDb = new();
    private readonly TimerService _timer = new();
    
    private Task? _tWait;
    private ConcurrentQueue<string> _gateWaitQueue = new();
    
    private bool _isRun = true;
    private ConsulClient? _consul;
    private ConsulServiceWatcher? _serviceWatcher;

    private void OnReconnect(string userId, string gateName, string connId)
    {
        _clients[userId] = new Client(userId, gateName, connId);
    }
    
    public event Action<string, string, string>? OnRequestService;
    
    private void StartRedisMsg()
    {
        _tWait = Task.Factory.StartNew(async () =>
        {
            var rpc = new WRpc();
            var h = new GateMsgMqHandle(rpc);
            h.OnReconnect += OnReconnect;
            h.OnRequestService += (string serviceName, string gateName, string connId) =>
            {
                OnRequestService?.Invoke(serviceName, gateName, connId);
            };

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
        }, TaskCreationOptions.LongRunning);
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
    
    private async void Run(HubConfig cfg)
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
                _gateMsgHandles.TryAdd(id, new GateMsgHandle(rpc, gate, _entities));
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
        var cfg = Newtonsoft.Json.JsonConvert.DeserializeObject<HubConfig>(System.Text.Encoding.Default.GetString(data));

        AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        
        var instance = new MainClass();
        using var sigTermReg = PosixSignalRegistration.Create(PosixSignal.SIGTERM, instance.HandleSignal);
        using var sigIntReg = PosixSignalRegistration.Create(PosixSignal.SIGINT, instance.HandleSignal);
        instance.Run(cfg);
    }
}