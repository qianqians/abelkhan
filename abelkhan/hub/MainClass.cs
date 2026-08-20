using System.Runtime.InteropServices;
using Consul;
using core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace hub;

public struct HubConfig()
{
    public readonly string HubId = string.Empty;
    public readonly string ServiceName = string.Empty;
    public readonly string Ip = string.Empty;
    public readonly ushort PortInternal = 0;
    public readonly ushort PortHealth = 0;
    public readonly string ConsulUrl  = string.Empty;
    public readonly string RedisUrl = string.Empty;
    public readonly string RedisPwd  = string.Empty;
}

public class MainClass
{
    private readonly Dictionary<string, Entity> _entities = new();
    private readonly Dictionary<string, GateNetwork> _gates = new();
    private readonly TcpConnectService _service = new();
    private TcpAcceptService? _internal;
    private readonly TimerService _timer = new();
    
    private bool _isRun = true;
    private ConsulClient? _consul;
    
    public MainClass()
    {
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
                    await Task.Delay((int)(16 - detail));
                }
            }

            _internal = new(cfg.PortInternal);

            await _internal.Join();
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