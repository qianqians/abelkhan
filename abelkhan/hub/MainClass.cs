using System.Net;
using System.Runtime.InteropServices;
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
    private readonly List<GateMsgHandle> _gates = new();
    private readonly TcpConnectService _serviceGate = new();
    private readonly TcpConnectService _serviceDb = new();
    private readonly TimerService _timer = new();
    
    private bool _isRun = true;
    private ConsulClient? _consul;
    private ConsulServiceWatcher? _serviceWatcher;
    
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
            
            var app = WebApplication.Create();
            app.MapGet("/health", () => Results.Ok("healthy"));
            _ = app.RunAsync($"http://{cfg.Ip}:{cfg.PortHealth}");
            await ReportServiceConsul(cfg);

            _serviceGate.OnConnect += (network) =>
            {
                var rpc = new WRpc();
                _gates.Add(new GateMsgHandle(rpc, new GateNetwork(network), _entities));
            };
            _serviceDb.OnConnect += (network) =>
            {
            }; 
                
            _serviceWatcher = new(_consul!);
            _serviceWatcher.OnNewService += (string serviceName, string ip, ushort port) =>
            {
                if (serviceName.Equals("gate", StringComparison.OrdinalIgnoreCase))
                {
                    _serviceGate.Connect(IPAddress.Parse(ip), port);
                }
                else if (serviceName.Equals("db_proxy", StringComparison.OrdinalIgnoreCase))
                {
                    _serviceDb.Connect(IPAddress.Parse(ip), port);
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