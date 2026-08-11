using core;
using engine;
using consts;
// ReSharper disable FieldCanBeMadeReadOnly.Global
namespace gate;

public struct GateConfig()
{
    public string GateId = string.Empty;
    public string RedisUrl = string.Empty;
    public string RedisPwd  = string.Empty;
    public ushort PortInternal = 0;
    public ushort PortExternal = 0;
    public string Pfx = string.Empty;
    public string PfxPassword  = string.Empty;
    public string EnterService = string.Empty;
    public uint MinVersion = 0;
    public uint MaxVersion = 0;
}

class Main
{
    private RedisHandle? _redis;
    private TcpAcceptService? _internal;
    private WebSocketAcceptService? _external;
    private Dictionary<string, Client>? _clients;

    private void OnRedisMsg()
    {
        
    }

    private void OnRedisReliabilityMsg()
    {
        
    }

    public async void Start(GateConfig cfg)
    {
        try
        {
            _clients = new();
            _redis = new RedisHandle(cfg.RedisUrl, cfg.RedisPwd);
            OnRedisMsg();
            OnRedisReliabilityMsg();

            _internal = new(cfg.PortInternal);
            _internal.OnListenAccept += async network =>
            {
                var rpc = new WRpc();

                _ = new HubMsgHandle(network, _clients, rpc, new HubGeneralMsgHandle(_clients, rpc));
                network.OnReceive(rpc.OnNetworkData);
            };
            _internal.Start();

            _external = new(cfg.PortExternal, cfg.Pfx, cfg.PfxPassword);
            _external.OnListenAccept += async network =>
            {
                var rpc = new WRpc();
                
                var netGuid = Guid.NewGuid().ToString();
                await network.Send(rpc.Notify(Consts.NotifyConnId, new NotifyConnID()
                {
                    ConnId = netGuid,
                }));
                await _redis.PushList(cfg.EnterService, rpc.Notify(Consts.EnterGame, new GateForwardClientRequestService()
                {
                    ServiceName  = cfg.EnterService,
                    GateName = cfg.GateId,
                    ConnId = netGuid,
                }));

                var cli = new Client(netGuid, network, _redis);
                _ = new ClientMsgHandle(cfg, rpc, cli);
                network.OnReceive(rpc.OnNetworkData);
                
                _clients.Add(netGuid, cli);
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