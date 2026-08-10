using core;
using engine;
using consts;
// ReSharper disable FieldCanBeMadeReadOnly.Global
namespace gate;

struct GateConfig()
{
    public string GateId = string.Empty;
    public string RedisUrl = string.Empty;
    public string RedisPwd  = string.Empty;
    public ushort PortInternal = 0;
    public ushort PortExternal = 0;
    public string Pfx = string.Empty;
    public string PfxPassword  = string.Empty;
    public string EnterService = string.Empty;
}

class Main
{
    private RedisHandle? _redis;
    private TcpAcceptService? _internal;
    private WebSocketAcceptService? _external;
    private Dictionary<string, Clients>? _clients;

    public async void Start(GateConfig cfg)
    {
        try
        {
            _redis = new RedisHandle(cfg.RedisUrl, cfg.RedisPwd);
            _clients  = new();
            
            _internal = new(cfg.PortInternal);
            _internal.Start();

            _external = new(cfg.PortExternal, cfg.Pfx, cfg.PfxPassword);
            _external.OnListenAccept += async network =>
            {
                var netGuid = Guid.NewGuid().ToString();
                await network.Send(WRpc.Notify(Consts.NotifyConnId, new NotifyConnID()
                {
                    ConnId = netGuid,
                }));
                await _redis.PushList(cfg.EnterService, WRpc.Notify(Consts.EnterGame, new GateForwardClientRequestService()
                {
                    ServiceName  = cfg.EnterService,
                    GateName = cfg.GateId,
                    ConnId = netGuid,
                }));
                _clients.Add(netGuid, new Clients(network, _redis));
            };
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