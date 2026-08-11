using consts;
using core;
using engine;
namespace gate;

public class HubGeneralMsgHandle
{   
    private readonly Dictionary<string, Client> _clients;
    private readonly WRpc _rpc;

    public HubGeneralMsgHandle(Dictionary<string, Client> clients, WRpc rpc)
    {
        _clients = clients;
        _rpc  = rpc;
    }

    public void OnHubCreateRemoteEntity(INetwork network, HubCreateRemoteEntity msg)
    {
        if (string.IsNullOrEmpty(msg.OwnerConnId))
        {
            if (_clients.TryGetValue(msg.OwnerConnId, out var client))
            {
                var forward = new CreatePlayerEntity()
                {
                    EntityId = msg.EntityId,
                    EntityType = msg.EntityType,
                    Argv = msg.Argv,
                };
                _ = client.SendToClient(_rpc.Notify(Consts.CreatePlayerEntity, forward));
                client.RegisterNetwork(msg.EntityId, network);
            }
            else
            {
                Log.Error($"OnHubCreateRemoteEntity not found Entity:{msg.EntityId}");
            }
        }
        
        var forwardMsg = new CreateRemoteEntity()
        {
            EntityId = msg.EntityId,
            EntityType = msg.EntityType,
            Argv = msg.Argv,
        };
        foreach (var guid in msg.ConnId)
        {
            if (_clients.TryGetValue(guid, out var cli))
            {
                _ = cli.SendToClient(_rpc.Notify(Consts.CreateRemoteEntity, forwardMsg));
            }
        }
    }

    public void OnHubDeleteRemoteEntity(HubDeleteRemoteEntity msg)
    {
        var forward = new DeleteRemoteEntity()
        {
            EntityId = msg.EntityId,
        };
        foreach (var guid in msg.ConnId)
        {
            if (_clients.TryGetValue(guid, out var cli))
            {
                _ = cli.SendToClient(_rpc.Notify(Consts.DeleteRemoteEntity, forward));
            }
        }
    }

    public void OnHubRefreshEntity(HubRefreshEntity msg)
    {
        var forward = new RefreshEntity()
        {
            EntityId = msg.EntityId,
            EntityType = msg.EntityType,
            Argv = msg.Argv,
        };
        foreach (var guid in msg.ConnId)
        {
            if (_clients.TryGetValue(guid, out var cli))
            {
                _ = cli.SendToClient(_rpc.Notify(Consts.RefreshEntity, forward));
            }
        }
    }

    public void OnGateForwardHubResponseClient(GateForwardHubResponseClient msg) { }

    public void OnGateForwardHubNotifyClient(GateForwardHubNotifyClient msg) {}

    public void OnGateForwardHubCallGlobal(GateForwardHubCallGlobal msg) {}

    public void OnHubKickOffClient(HubKickOffClient msg) {}

}

public class HubMsgHandle
{
    private readonly Dictionary<string, Client> _clients;
    private readonly INetwork _network;
    private readonly WRpc _rpc;
    private readonly HubGeneralMsgHandle _msgHandle;
    
    public HubMsgHandle(INetwork network, Dictionary<string, Client> clients, WRpc rpc, HubGeneralMsgHandle msgHandle)
    {
        _network = network;
        _clients = clients;
        _rpc  = rpc;
        _msgHandle = msgHandle;
        
        rpc.OnNotify += OnNotify;
        rpc.OnRequest += OnRequest;
        rpc.OnResponse += OnResponse;
    }

    private void OnNotify(Notify ntf)
    {
        switch (ntf.Event.ProtoName)
        {
            case Consts.HubCreateRemoteEntity:
                _msgHandle.OnHubCreateRemoteEntity(_network, _rpc.OnMsg<HubCreateRemoteEntity>(ntf.Event.Content.ToByteArray()));
                break;
            case Consts.HubDeleteRemoteEntity:
                _msgHandle.OnHubDeleteRemoteEntity(_rpc.OnMsg<HubDeleteRemoteEntity>(ntf.Event.Content.ToByteArray()));
                break;
            case Consts.HubRefreshEntity:
                _msgHandle.OnHubRefreshEntity(_rpc.OnMsg<HubRefreshEntity>(ntf.Event.Content.ToByteArray()));
                break;
            case Consts.GateForwardHubNotifyClient:
                _msgHandle.OnGateForwardHubNotifyClient(_rpc.OnMsg<GateForwardHubNotifyClient>(ntf.Event.Content.ToByteArray()));
                break;
            case Consts.GateForwardHubCallGlobal:
                _msgHandle.OnGateForwardHubCallGlobal(_rpc.OnMsg<GateForwardHubCallGlobal>(ntf.Event.Content.ToByteArray()));
                break;
            case Consts.HubKickOffClient:
                _msgHandle.OnHubKickOffClient(_rpc.OnMsg<HubKickOffClient>(ntf.Event.Content.ToByteArray()));
                break;
            default:
            {
                throw new ArgumentException($"HubMsgHandle ntf.Event.ProtoName:{ntf.Event.ProtoName}");
            }
        }
    }

    private void OnRequest(Request req)
    {
        switch (req.Event.ProtoName)
        {
            case Consts.GateForwardHubRequestClient:
            {
                var msg = _rpc.OnMsg<GateForwardHubRequestClient>(req.Event.Content.ToByteArray());
                var forward = new HubRequestClient()
                {
                    EntityId = msg.EntityId,
                    Event = msg.Event,
                };
                if (_clients.TryGetValue(msg.ConnId, out var cli))
                {
                    _ = cli.SendToClient(_rpc.Request(Consts.HubRequestClient, Guid.NewGuid().ToString(), forward));
                }
                break;
            }
            default:
            {
                throw new ArgumentException($"HubMsgHandle req.Event.ProtoName:{req.Event.ProtoName}");
            }
        }
    }

    private void OnResponse(Response rsp)
    {
        switch (rsp.Event.ProtoName)
        {
            case Consts.GateForwardHubResponseClient:
            {
                var msg = _rpc.OnMsg<GateForwardHubResponseClient>(rsp.Event.Content.ToByteArray());
                var forward = new HubResponseClient()
                {
                    ErrMsg = msg.ErrMsg,
                    Content = msg.Content,
                };
                if (_clients.TryGetValue(msg.ConnId, out var cli))
                {
                    _ = cli.SendToClient(_rpc.Response(Consts.HubResponseClient, rsp.MsgId, forward));
                }
                break;
            }
            default:
            {
                throw new ArgumentException($"HubMsgHandle rsp.Event.ProtoName:{rsp.Event.ProtoName}");
            }
        }
    }
}