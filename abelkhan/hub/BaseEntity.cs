using consts;
using core;
using engine;
using Google.Protobuf;
namespace hub;

public abstract class BaseEntity(string entityId, string entityType, RedisHandle redis)
{
    private readonly WRpc _rpc = new();
    private readonly Dictionary<string, Func<string, string, GateNetwork, ByteString, Task>> _onMsg = new();
    private readonly Dictionary<string, ClientNetwork> _clients = new();
    private readonly Dictionary<string, Action<string, byte[]>> _requestCallbacks = new();

    private class ClientNetwork(GateNetwork gateNetwork)
    {
        public async Task Send(byte[] message)
        {
            await gateNetwork.Send(message);
        }
    }

    private async Task SendToGate(string connId, byte[] message)
    {
        if (_clients.TryGetValue(connId, out var client))
        {
            await client.Send(message);
        }
    }

    public abstract IMessage FullInfo();
    
    public abstract IMessage ClientInfo();
    
    protected virtual async Task CreateRemotePlayer(Client client)
    {
        var msg = new HubCreatePlayerEntity()
        {
            ConnId = client.ConnId,
            UserId = client.UserId,
            EntityId = entityId,
            EntityType = entityType,
            Argv = ClientInfo().ToByteString(),
        };
        await SendToGate(client.ConnId, _rpc.Notify(Consts.HubCreatePlayerEntity, msg));
    }
    
    protected virtual async Task CreateRemoteEntity(Client client)
    {
        var msg = new HubCreateRemoteEntity()
        {
            ConnId = client.ConnId,
            EntityId = entityId,
            EntityType = entityType,
            Argv = ClientInfo().ToByteString(),
        };
        await SendToGate(client.ConnId, _rpc.Notify(Consts.HubCreateRemoteEntity, msg));
    }

    public virtual async Task DeleteRemoteEntity(Client client)
    {
        var msg = new HubDeleteRemoteEntity()
        {
            EntityId = entityId,
        };
        await SendToGate(client.ConnId, _rpc.Notify(Consts.HubDeleteRemoteEntity, msg));
    }

    public virtual async Task<Result<T1, string>> Request<T0, T1>(string connId, string method, T0 argv) 
        where T0 : IMessage<T0>
        where T1 : IMessage<T1>, new()
    {
        var t = new TaskCompletionSource<Result<T1, string>>();
        var callRpc = new CallRpc()
        {
            ProtoName = method,
            Content = argv.ToByteString()
        };
        var msg = new GateForwardHubRequestClient()
        {
            ConnId = connId,
            EntityId = entityId,
            Event = callRpc,
        };
        var msgId = Guid.NewGuid().ToString();
        await SendToGate(connId, _rpc.Request(Consts.GateForwardHubRequestClient, msgId, msg));
            
        _requestCallbacks.Add(msgId, (string errMsg, byte[] content) =>
        {
            if (!string.IsNullOrEmpty(errMsg))
            {
                t.SetResult(Result<T1, string>.Err(errMsg));
            }
            else
            {
                var parser = new MessageParser<T1>(() => new T1());
                t.SetResult(Result<T1, string>.Ok(parser.ParseFrom(content)));
            }
        });
        return await t.Task;
    }

    public void OnResponse(string msgId, string errMsg, byte[] data)
    {
        if (_requestCallbacks.Remove(msgId, out var callback))
        {
            callback(errMsg, data);
        }
        else
        {
            Log.Error($"OnResponse Msg:{msgId} not found!");
        }
    }

    public async Task Notify<T>(string connId, string method, T argv)
        where T : IMessage<T>
    {
        var callRpc = new CallRpc()
        {
            ProtoName = method,
            Content = argv.ToByteString()
        };
        var msg = new GateForwardHubNotifyClient()
        {
            ConnId = connId,
            EntityId = entityId,
            Event = callRpc,
        };
        await SendToGate(connId, _rpc.Notify(Consts.GateForwardHubNotifyClient, msg));
    }

    private async Task Response(string connId, string msgId, byte[] data)
    {
        var msg = new GateForwardHubResponseClient
        {
            ConnId = connId,
            EntityId = entityId,
            Content = ByteString.CopyFrom(data)
        };
        await SendToGate(connId, _rpc.Response(Consts.GateForwardHubResponseClient, msgId, msg));
    }

    private async Task Error(string connId, string msgId, string err)
    {
        var msg = new GateForwardHubResponseClient
        {
            ConnId = connId,
            EntityId = entityId,
            ErrMsg = err
        };
        await SendToGate(connId, _rpc.Response(Consts.GateForwardHubResponseClient, msgId, msg));
    }

    private async Task SendToListMq(string userId, bool isReliability, byte[] message)
    {
        if (isReliability)
        {
            await redis.PushList(string.Format(Consts.EntityReliabilityClientMq, userId), message);
        }
        else
        {
            await redis.PushList(string.Format(Consts.EntityClientMq, userId), message);
        }
    }
    
    public async Task NotifyListMq<T>(string userId, bool isReliability, string method, T argv)
        where T : IMessage<T>
    {
        var callRpc = new CallRpc()
        {
            ProtoName = method,
            Content = argv.ToByteString()
        };
        var msg = new GateForwardHubNotifyClientMq()
        {
            UserId = userId,
            EntityId = entityId,
            Event = callRpc,
        };
        await SendToListMq(userId, isReliability, _rpc.Notify(Consts.GateForwardHubNotifyClientMq, msg));
    }
    
    public virtual void RegisterNotify<T>(string method, Action<T> callback)
        where T : IMessage<T>, new()
    {
        var parser = new MessageParser<T>(() => new T());
        _onMsg.Add(method, (string connId, string msgId, GateNetwork gateNetwork, ByteString data) =>
        {
            try
            {
                if (!_clients.ContainsKey(connId))
                {
                    _clients.Add(connId, new ClientNetwork(gateNetwork));
                }

                var t = parser.ParseFrom(data);
                callback(t);
            }
            catch (Exception ex)
            {
                Log.Error($"Do Notify method:{method} ex:{ex}");
            }
            return Task.CompletedTask;
        });
    }
    
    public virtual void RegisterNotify<T>(string method, Func<T, Task> callback)
        where T : IMessage<T>, new()
    {
        var parser = new MessageParser<T>(() => new T());
        _onMsg.Add(method, async (string connId, string msgId, GateNetwork gateNetwork, ByteString data) =>
        {
            try
            {
                if (!_clients.ContainsKey(connId))
                {
                    _clients.Add(connId, new ClientNetwork(gateNetwork));
                }
                
                var t = parser.ParseFrom(data);
                await callback(t);
            }
            catch (Exception ex)
            {
                Log.Error($"Do Notify method:{method} ex:{ex}");
            }
        });
    }

    public virtual void RegisterRequest<T0, T1>(string method, Func<T0, Result<T1, string>> callback)
        where T0 : IMessage<T0>, new()
        where T1 : IMessage<T1>, new()
    {
        var parser = new MessageParser<T0>(() => new T0());
        _onMsg.Add(method, async (string connId, string msgId, GateNetwork gateNetwork, ByteString data) =>
        {
            try
            {
                if (!_clients.ContainsKey(connId))
                {
                    _clients.Add(connId, new ClientNetwork(gateNetwork));
                }
                
                var t = parser.ParseFrom(data);
                var ret = callback(t);
                if (ret.IsOk)
                {
                    await Response(connId, msgId, ret.Value.ToByteArray());
                }
                else
                {
                    await Error(connId, msgId, ret.Error);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Do Request method:{method} ex:{ex}");
            }
        });
    }
    
    public virtual void RegisterRequest<T0, T1>(string method, Func<T0, Task<Result<T1, string>>> callback)
        where T0 : IMessage<T0>, new()
        where T1 : IMessage<T1>, new()
    {
        var parser = new MessageParser<T0>(() => new T0());
        _onMsg.Add(method, async (string connId, string msgId, GateNetwork gateNetwork, ByteString data) =>
        {
            try
            {
                if (!_clients.ContainsKey(connId))
                {
                    _clients.Add(connId, new ClientNetwork(gateNetwork));
                }
                
                var t = parser.ParseFrom(data);
                var ret = await callback(t);
                if (ret.IsOk)
                {
                    await Response(connId, msgId, ret.Value.ToByteArray());
                }
                else
                {
                    await Error(connId, msgId, ret.Error);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Do Request method:{method} ex:{ex}");
            }
        });
    }

    public void OnDoMsg(string connId, string msgId, GateNetwork gate, string method, ByteString message)
    {
        if (_onMsg.TryGetValue(method, out var action))
        {
            try
            {
                action(connId, msgId, gate, message);
            }
            catch(Exception ex)
            {
                Log.Error($"OnDoMsg method:{method} ex:{ex}");
            }
        }
        else
        {
            Log.Error($"OnDoMsg method:{method} not exist");
        }
    }
}