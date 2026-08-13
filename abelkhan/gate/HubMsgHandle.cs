using System.Collections.Concurrent;
using consts;
using core;
using engine;
namespace gate;

public class HubGeneralMsgHandle(Dictionary<string, Client> clients, Dictionary<string, Client> entityClients, ConcurrentQueue<string> clientWaitQueue, ConcurrentQueue<string> clientReliabilityQueue, WRpc rpc)
{
    public void OnHubCreatePlayerEntity(INetwork? network, HubCreatePlayerEntity msg)
    {
        var forward = new CreatePlayerEntity()
        {
            EntityId = msg.EntityId,
            EntityType = msg.EntityType,
            Argv = msg.Argv,
        };
        if (clients.TryGetValue(msg.ConnId, out var cli))
        {
            _ = cli.SendToClient(rpc.Notify(Consts.CreatePlayerEntity, forward));
            if (network != null)
            {
                cli.RegisterNetwork(msg.EntityId, network);
            }
            clientWaitQueue.Enqueue(msg.UserId);
            clientReliabilityQueue.Enqueue(msg.UserId);
            entityClients.Add(msg.UserId, cli);
        }
    }
    
    public void OnHubCreateRemoteEntity(HubCreateRemoteEntity msg)
    {
        var forwardMsg = new CreateRemoteEntity()
        {
            EntityId = msg.EntityId,
            EntityType = msg.EntityType,
            Argv = msg.Argv,
        };
        foreach (var guid in msg.ConnId)
        {
            if (clients.TryGetValue(guid, out var cli))
            {
                _ = cli.SendToClient(rpc.Notify(Consts.CreateRemoteEntity, forwardMsg));
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
            if (clients.TryGetValue(guid, out var cli))
            {
                _ = cli.SendToClient(rpc.Notify(Consts.DeleteRemoteEntity, forward));
            }
        }
        entityClients.Remove(msg.EntityId);
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
            if (clients.TryGetValue(guid, out var cli))
            {
                _ = cli.SendToClient(rpc.Notify(Consts.RefreshEntity, forward));
            }
        }
    }

    public void OnGateForwardHubRequestClient(GateForwardHubRequestClient msg)
    {
        var forward = new HubRequestClient()
        {
            EntityId = msg.EntityId,
            Event = msg.Event,
        };
        if (clients.TryGetValue(msg.ConnId, out var cli))
        {
            _ = cli.SendToClient(rpc.Request(Consts.HubRequestClient, Guid.NewGuid().ToString(), forward));
        }
    }

    public void OnGateForwardHubResponseClient(string msgId, GateForwardHubResponseClient msg)
    {
        var forward = new HubResponseClient()
        {
            ErrMsg = msg.ErrMsg,
            Content = msg.Content,
        };
        if (clients.TryGetValue(msg.ConnId, out var cli))
        {
            _ = cli.SendToClient(rpc.Response(Consts.HubResponseClient, msgId, forward));
        }
    }

    public void OnGateForwardHubNotifyClient(GateForwardHubNotifyClient msg)
    {
        var forward = new HubNotifyClient()
        {
            EntityId = msg.EntityId,
            Event = msg.Event,
        };
        foreach (var guid in msg.ConnId)
        {
            if (clients.TryGetValue(guid, out var cli))
            {
                _ = cli.SendToClient(rpc.Notify(Consts.HubNotifyClient, forward));
            }
        }
    }

    public void OnGateForwardHubCallGlobal(GateForwardHubCallGlobal msg)
    {
        var forward = new HubNotifyClient()
        {
            EntityId = msg.EntityId,
            Event = msg.Event,
        };
        foreach (var (_, cli) in clients)
        {
            _ = cli.SendToClient(rpc.Notify(Consts.HubNotifyClient, forward));
        }
    }

    public void OnHubKickOffClient(HubKickOffClient msg)
    {
        var forward = new KickOff()
        {
            PromptInfo = msg.PromptInfo,
        };
        if (clients.TryGetValue(msg.ConnId, out var cli))
        {
            _ = cli.SendToClient(rpc.Notify(Consts.KickOff, forward));
        }
    }

}

public class HubMsgHandle
{
    private readonly INetwork? _network;
    private readonly WRpc _rpc;
    private readonly HubGeneralMsgHandle _msgHandle;
    
    public HubMsgHandle(INetwork network, WRpc rpc, HubGeneralMsgHandle msgHandle)
    {
        _network = network;
        _rpc  = rpc;
        _msgHandle = msgHandle;
        
        rpc.OnNotify += OnNotify;
        rpc.OnRequest += OnRequest;
        rpc.OnResponse += OnResponse;
    }
    
    public HubMsgHandle(WRpc rpc, HubGeneralMsgHandle msgHandle)
    {
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
            case Consts.HubCreatePlayerEntity:
                _msgHandle.OnHubCreatePlayerEntity(_network, _rpc.OnMsg<HubCreatePlayerEntity>(ntf.Event.Content.ToByteArray()));
                break;
            case Consts.HubCreateRemoteEntity:
                _msgHandle.OnHubCreateRemoteEntity(_rpc.OnMsg<HubCreateRemoteEntity>(ntf.Event.Content.ToByteArray()));
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
                _msgHandle.OnGateForwardHubRequestClient(_rpc.OnMsg<GateForwardHubRequestClient>(req.Event.Content.ToByteArray()));
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
                _msgHandle.OnGateForwardHubResponseClient(rsp.MsgId, _rpc.OnMsg<GateForwardHubResponseClient>(rsp.Event.Content.ToByteArray()));
                break;
            }
            default:
            {
                throw new ArgumentException($"HubMsgHandle rsp.Event.ProtoName:{rsp.Event.ProtoName}");
            }
        }
    }
}