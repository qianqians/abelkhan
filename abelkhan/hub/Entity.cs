using consts;
using core;
using engine;
using Google.Protobuf;
namespace hub;

public class Entity(string entityId, RedisHandle redis, Dictionary<string, GateNetwork> gateNetworks, Dictionary<string, string> mappingUser)
{
    private readonly Dictionary<string, Func<string, GateNetwork, ByteString, Task>> _onMsg = new();
    private readonly Dictionary<string, ClientNetwork> _clients = new();
    private readonly Dictionary<string, Action<string, byte[]>> _requestCallbacks = new();

    private class ClientNetwork(GateNetwork gateNetwork, string connId)
    {
        public async Task Send(byte[] message)
        {
            await gateNetwork.Send(message);
        }
    }

    private async Task SendToGate(string connId, string userId, byte[] message)
    {
        if (_clients.TryGetValue(connId, out var client))
        {
            await client.Send(message);
        }
        else
        {
            await redis.PushList(string.Format(Consts.EntityClientMq, userId), message);
        }
    }

    private async Task<Result<T1, string>> Request<T0, T1>(string connId, string method, T0 argv) 
        where T0 : IMessage<T0>
        where T1 : IMessage<T1>, new()
    {
        var t = new TaskCompletionSource<Result<T1, string>>();
        
        if (mappingUser.TryGetValue(connId, out var userId))
        {
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
            var req = new Request()
            {
                MsgId = Guid.NewGuid().ToString(),
                Event = new CallRpc()
                {
                    ProtoName = consts.Consts.GateForwardHubRequestClient,
                    Content = msg.ToByteString(),
                }
            };
            await SendToGate(connId, userId, req.ToByteArray());
            _requestCallbacks.Add(req.MsgId, (string errMsg, byte[] content) =>
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
        }
        
        return await t.Task;
    }

    public void OnResponse(string msgId, string errMsg, byte[] data)
    {
        if (_requestCallbacks.TryGetValue(msgId, out var callback))
        {
            callback(errMsg, data);
        }
        else
        {
            Log.Error($"OnResponse Msg:{msgId} not found!");
        }
    }

    private async Task Notify<T>(string connId, string method, T argv)
        where T : IMessage<T>
    {
        if (mappingUser.TryGetValue(connId, out var userId))
        {
            var callRpc = new CallRpc()
            {
                ProtoName = method,
                Content = argv.ToByteString()
            };
            var msg = new GateForwardHubNotifyClient()
            {
                EntityId = entityId,
                Event = callRpc,
            };
            msg.ConnId.Add(connId);
            var ntf = new Notify()
            {
                Event = new CallRpc()
                {
                    ProtoName = consts.Consts.GateForwardHubNotifyClient,
                    Content = msg.ToByteString(),
                }
            };
            await SendToGate(connId, userId, ntf.ToByteArray());
        }
    }
    
    private async Task Response(string connId, byte[] data)
    {
        if (mappingUser.TryGetValue(connId, out var userId))
        {
            var msg = new GateForwardHubResponseClient
            {
                ConnId = connId,
                EntityId = entityId,
                Content = ByteString.CopyFrom(data)
            };
            await SendToGate(connId, userId, msg.ToByteArray());
        }
    }

    private async Task Error(string connId, string err)
    {
        if (mappingUser.TryGetValue(connId, out var userId))
        {
            var msg = new GateForwardHubResponseClient
            {
                ConnId = connId,
                EntityId = entityId,
                ErrMsg = err
            };
            await SendToGate(connId, userId, msg.ToByteArray());
        }
    }

    private void RegisterNotify<T>(string method, Action<T> callback)
        where T : IMessage<T>, new()
    {
        var parser = new MessageParser<T>(() => new T());
        _onMsg.Add(method, (string connId, GateNetwork gateNetwork, ByteString data) =>
        {
            try
            {
                if (!_clients.ContainsKey(connId))
                {
                    _clients.Add(connId, new ClientNetwork(gateNetwork, connId));
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
    
    private void RegisterNotify<T>(string method, Func<T, Task> callback)
        where T : IMessage<T>, new()
    {
        var parser = new MessageParser<T>(() => new T());
        _onMsg.Add(method, async (string connId, GateNetwork gateNetwork, ByteString data) =>
        {
            try
            {
                if (!_clients.ContainsKey(connId))
                {
                    _clients.Add(connId, new ClientNetwork(gateNetwork, connId));
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

    private void RegisterRequest<T0, T1>(string method, Func<T0, Result<T1, string>> callback)
        where T0 : IMessage<T0>, new()
        where T1 : IMessage<T1>, new()
    {
        var parser = new MessageParser<T0>(() => new T0());
        _onMsg.Add(method, async (string connId, GateNetwork gateNetwork, ByteString data) =>
        {
            try
            {
                if (!_clients.ContainsKey(connId))
                {
                    _clients.Add(connId, new ClientNetwork(gateNetwork, connId));
                }
                
                var t = parser.ParseFrom(data);
                var ret = callback(t);
                if (ret.IsOk)
                {
                    await Response(connId, ret.Value.ToByteArray());
                }
                else
                {
                    await Error(connId, ret.Error);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Do Request method:{method} ex:{ex}");
            }
        });
    }
    
    private void RegisterRequest<T0, T1>(string method, Func<T0, Task<Result<T1, string>>> callback)
        where T0 : IMessage<T0>, new()
        where T1 : IMessage<T1>, new()
    {
        var parser = new MessageParser<T0>(() => new T0());
        _onMsg.Add(method, async (string connId, GateNetwork gateNetwork, ByteString data) =>
        {
            try
            {
                if (!_clients.ContainsKey(connId))
                {
                    _clients.Add(connId, new ClientNetwork(gateNetwork, connId));
                }
                
                var t = parser.ParseFrom(data);
                var ret = await callback(t);
                if (ret.IsOk)
                {
                    await Response(connId, ret.Value.ToByteArray());
                }
                else
                {
                    await Error(connId, ret.Error);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Do Request method:{method} ex:{ex}");
            }
        });
    }
    
    public void OnDoMsg(string connId, GateNetwork gate, string method, ByteString message)
    {
        if (_onMsg.TryGetValue(method, out var action))
        {
            try
            {
                action(connId, gate, message);
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