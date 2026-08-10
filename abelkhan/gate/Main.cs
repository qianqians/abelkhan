using core;
namespace gate;

struct GateConfig()
{
    public readonly ushort PortInternal = 0;
    public readonly ushort PortExternal = 0;
    public readonly string Pfx = string.Empty;
    public readonly string PfxPassword  = string.Empty;
}

class Main
{
    private core.TcpAcceptService? _internal;
    private core.WebSocketAcceptService? _external;

    public async void Start(GateConfig cfg)
    {
        try
        {
            _internal = new(cfg.PortInternal);
            _internal.OnListenAccept += network =>
            {
                
            };
            _internal.Start();

            _external = new(cfg.PortExternal, cfg.Pfx, cfg.PfxPassword);
            _external.Start();
            
            await _internal.Join();
            await _external.Join();
        }
        catch (Exception ex)
        {
            Log.Error("gate Main run error:{0}", ex);
        }
    }
    
}