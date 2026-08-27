using System.Collections.Concurrent;
using consts;
using core;
using engine;
using Nito.Collections;

namespace gate;

public class HubGeneralMsgHandle(Dictionary<string, Client> clients, 
    Deque<string> clientWaitQueue, 
    Deque<string> clientReliabilityQueue, WRpc rpc)
{
    public void OnHubCreatePlayerEntity(INetwork? network, HubCreatePlayerEntity msg)
    {
        lock (clients)
        {
            if (clients.TryGetValue(msg.ConnId, out var cli))
            {
                var forward = new CreatePlayerEntity()
                {
                    EntityId = msg.EntityId,
                    EntityType = msg.EntityType,
                    Argv = msg.Argv,
                };
                _ = cli.SendToClient(rpc.Notify(Consts.CreatePlayerEntity, forward));
                if (network != null)
                {
                    cli.RegisterNetwork(msg.EntityId, network);
                }

                lock (clientWaitQueue)
                {
                    if (!clientWaitQueue.Contains(msg.UserId))
                    {
                        clientWaitQueue.AddToBack(msg.UserId);
                    }
                }

                lock (clientReliabilityQueue)
                {
                    if (!clientReliabilityQueue.Contains(msg.UserId))
                    {
                        clientReliabilityQueue.AddToBack(msg.UserId);
                    }
                }

                cli.UserId = msg.UserId;
            }
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
        lock (clients)
        {
            if (clients.TryGetValue(msg.ConnId, out var cli))
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
        lock (clients)
        {
            if (clients.TryGetValue(msg.ConnId, out var cli))
            {
                _ = cli.SendToClient(rpc.Notify(Consts.DeleteRemoteEntity, forward));
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
        lock (clients)
        {
            if (clients.TryGetValue(msg.ConnId, out var cli))
            {
                _ = cli.SendToClient(rpc.Notify(Consts.RefreshEntity, forward));
            }
        }
    }

    public void OnGateForwardHubRequestClient(string msgId, GateForwardHubRequestClient msg)
    {
        var forward = new HubRequestClient()
        {
            EntityId = msg.EntityId,
            Event = msg.Event,
        };
        lock (clients)
        {
            if (clients.TryGetValue(msg.ConnId, out var cli))
            {
                _ = cli.SendToClient(rpc.Request(Consts.HubRequestClient, msgId, forward));
            }
        }
    }

    public void OnGateForwardHubResponseClient(string msgId, GateForwardHubResponseClient msg)
    {
        var forward = new HubResponseClient()
        {
            ErrMsg = msg.ErrMsg,
            Content = msg.Content,
        };
        lock (clients)
        {
            if (clients.TryGetValue(msg.ConnId, out var cli))
            {
                _ = cli.SendToClient(rpc.Response(Consts.HubResponseClient, msgId, forward));
            }
        }
    }

    public void OnGateForwardHubNotifyClient(GateForwardHubNotifyClient msg)
    {
        var forward = new HubNotifyClient()
        {
            EntityId = msg.EntityId,
            Event = msg.Event,
        };
        lock (clients)
        {
            if (clients.TryGetValue(msg.ConnId, out var cli))
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
        List<Client> cliList;
        lock (clients)
        {
            cliList = clients.Select(cli => cli.Value).ToList();
        }
        foreach (var cli in cliList)
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
        lock (clients)
        {
            if (clients.TryGetValue(msg.ConnId, out var cli))
            {
                _ = cli.SendToClient(rpc.Notify(Consts.KickOff, forward));
            }
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
                Log.Error($"HubMsgHandle ntf.Event.ProtoName:{ntf.Event.ProtoName}");
                break;
            }
        }
    }

    private void OnRequest(Request req)
    {
        switch (req.Event.ProtoName)
        {
            case Consts.GateForwardHubRequestClient:
            {
                _msgHandle.OnGateForwardHubRequestClient(req.MsgId, _rpc.OnMsg<GateForwardHubRequestClient>(req.Event.Content.ToByteArray()));
                break;
            }
            default:
            {
                Log.Error($"HubMsgHandle req.Event.ProtoName:{req.Event.ProtoName}");
                break;
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
                Log.Error($"HubMsgHandle rsp.Event.ProtoName:{rsp.Event.ProtoName}");
                break;
            }
        }
    }
}