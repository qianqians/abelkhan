using System.Collections.Concurrent;
using System.Diagnostics;
using consts;
using core;
using engine;
using Google.Protobuf;
using MongoDB.Bson;

namespace hub;

public abstract class BaseEntity(string entityId, string entityType, RedisHandle redis,
    ConcurrentDictionary<string, Client> clients,
    ConcurrentDictionary<string, GateNetwork> gates) : Actor
{
    public string EntityId => entityId;
    public string EntityType => entityType;
    
    private readonly WRpc _rpc = new();
    private readonly Dictionary<string, Func<string, string, GateNetwork, ByteString, Task>> _onMsg = new();
    private readonly Dictionary<string, Action<string, byte[]>> _requestCallbacks = new();
    
    internal async Task SendToGate(string userId, byte[] message)
    {
        if (!clients.TryGetValue(userId, out var cli))
        {
            Log.Error($"SendToGate userId:{userId} not found!");
            return;
        }
        
        if (!gates.TryGetValue(cli.GateName, out var gate))
        {
            Log.Error($"SendToGate connId:{cli.GateName} not found!");
            return;
        }
        
        await gate.Send(message);
    }

    public abstract BsonDocument FullInfo();
    // ReSharper disable once MemberCanBeProtected.Global
    public abstract BsonDocument ClientInfo();
    
    protected virtual async Task CreateRemotePlayer(Client client)
    {
        var msg = new HubCreatePlayerEntity()
        {
            ConnId = client.ConnId,
            UserId = client.UserId,
            EntityId = entityId,
            EntityType = entityType,
            Argv = ByteString.CopyFrom(ClientInfo().ToBson()),
        };
        await SendToGate(client.UserId, _rpc.Notify(Consts.HubCreatePlayerEntity, msg));
    }
    
    protected virtual async Task CreateRemoteEntity(Client client)
    {
        var msg = new HubCreateRemoteEntity()
        {
            ConnId = client.ConnId,
            EntityId = entityId,
            EntityType = entityType,
            Argv = ByteString.CopyFrom(ClientInfo().ToBson()),
        };
        await SendToGate(client.UserId, _rpc.Notify(Consts.HubCreateRemoteEntity, msg));
    }

    public virtual async Task DeleteRemoteEntity(Client client)
    {
        var msg = new HubDeleteRemoteEntity()
        {
            ConnId = client.ConnId,
            EntityId = entityId,
        };
        await SendToGate(client.UserId, _rpc.Notify(Consts.HubDeleteRemoteEntity, msg));
    }

    public virtual async Task<Result<T1, string>> Request<T0, T1>(string userId, string method, T0 argv) 
        where T0 : IMessage<T0>
        where T1 : IMessage<T1>, new()
    {
        var t = new TaskCompletionSource<Result<T1, string>>();

        if (clients.TryGetValue(userId, out var client))
        {
            var callRpc = new CallRpc()
            {
                ProtoName = method,
                Content = argv.ToByteString()
            };
            var msg = new GateForwardHubRequestClient()
            {
                ConnId = client!.ConnId,
                EntityId = entityId,
                Event = callRpc,
            };
            var msgId = Guid.NewGuid().ToString();
            await SendToGate(client.UserId, _rpc.Request(Consts.GateForwardHubRequestClient, msgId, msg));

            lock (_requestCallbacks)
            {
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
            }
        }
        else
        {
            var err = $"Request {userId} not found!";
            Log.Error(err);
            t.SetResult(Result<T1, string>.Err(err));
        }

        return await t.Task;
    }

    public void OnResponse(string msgId, string errMsg, byte[] data)
    {
        lock (_requestCallbacks)
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
    }

    public async Task Notify<T>(string userId, string method, T argv)
        where T : IMessage<T>
    {
        if (clients.TryGetValue(userId, out var client))
        {
            var callRpc = new CallRpc()
            {
                ProtoName = method,
                Content = argv.ToByteString()
            };
            var msg = new GateForwardHubNotifyClient()
            {
                ConnId = client.ConnId,
                EntityId = entityId,
                Event = callRpc,
            };
            await SendToGate(client.UserId, _rpc.Notify(Consts.GateForwardHubNotifyClient, msg));
        }
        else
        {
            Log.Error($"Notify {userId} not found!");
        }
    }

    /// <summary>
    /// 回包给客户端。
    ///
    /// 注意这里必须用 **userId** 路由（<c>SendToGate</c> 是按 userId 建键的 clients 字典查
    /// gateName），而报文里的 <c>ConnId</c> 仍然是连接 id —— gate 靠它把消息投给具体连接。
    /// 之前这里传的是 connId，而 connId 是 gate 生成的 GUID、与 userId 不同，
    /// 结果所有 request/response 协议的回包都会被丢掉（只留一行 SendToGate not found 日志）。
    /// </summary>
    private async Task Response(string userId, string connId, string msgId, byte[] data)
    {
        var msg = new GateForwardHubResponseClient
        {
            ConnId = connId,
            EntityId = entityId,
            Content = ByteString.CopyFrom(data)
        };
        await SendToGate(userId, _rpc.Response(Consts.GateForwardHubResponseClient, msgId, msg));
    }

    private async Task Error(string userId, string connId, string msgId, string err)
    {
        var msg = new GateForwardHubResponseClient
        {
            ConnId = connId,
            EntityId = entityId,
            ErrMsg = err
        };
        await SendToGate(userId, _rpc.Response(Consts.GateForwardHubResponseClient, msgId, msg));
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
    
    // 可靠消息序号的起点用 31 位随机数：序号只要在"同一 userId 的消息流"里唯一即可，
    // 随机起点避免多进程（跨服接管）各自都从 1 开始而产生重复；同时保证序号一直小于 2^53，
    // 这样 JS/TS 客户端用 double 表示也不会丢精度（用 bigint 也照样正确）
    private static long _reliabilitySeq = Random.Shared.Next();

    private static ulong NextReliabilitySeq()
    {
        return (ulong)Interlocked.Increment(ref _reliabilitySeq);
    }

    public async Task NotifyListMq<T>(Client client, bool isReliability, string method, T argv)
        where T : IMessage<T>
    {
        var callRpc = new CallRpc()
        {
            ProtoName = method,
            Content = argv.ToByteString()
        };
        var msg = new GateForwardHubNotifyClientMq()
        {
            UserId = client.UserId,
            EntityId = entityId,
            Event = callRpc,
        };
        if (isReliability)
        {
            // 可靠消息带唯一序号：gate 靠它判断客户端 ack 的是不是队头那一条。
            // 超时重投用的是 Redis 里同一份消息，序号不变，所以重复 ack 会被忽略，
            // 不会把还没投递给客户端的下一条弹掉。
            msg.Seq = NextReliabilitySeq();
        }
        await SendToListMq(client.UserId, isReliability, _rpc.Notify(Consts.GateForwardHubNotifyClientMq, msg));
    }


    private bool TryGetClient(string connId, out Client? client)
    {
        client = null;
        foreach (var (_, cli) in clients)
        {
            if (cli.ConnId == connId)
            {
                client = cli;
                return true;
            }
        }
        return false;
    }
    
    public virtual void RegisterNotify<T>(string method, Action<string, T> callback)
        where T : IMessage<T>, new()
    {
        var parser = new MessageParser<T>(() => new T());
        _onMsg.Add(method, (string connId, string msgId, GateNetwork gateNetwork, ByteString data) =>
        {
            try
            {
                if (TryGetClient(connId, out var client))
                {
                    var t = parser.ParseFrom(data);
                    callback(client!.UserId, t);
                }
                else 
                {
                    Log.Error($"OnNotify method:{method} connId:{connId} not found!");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Do Notify method:{method} ex:{ex}");
            }
            return Task.CompletedTask;
        });
    }
    
    public virtual void RegisterNotify<T>(string method, Func<string, T, Task> callback)
        where T : IMessage<T>, new()
    {
        var parser = new MessageParser<T>(() => new T());
        _onMsg.Add(method, async (string connId, string msgId, GateNetwork gateNetwork, ByteString data) =>
        {
            try
            {
                if (TryGetClient(connId, out var client))
                {
                    var t = parser.ParseFrom(data);
                    await callback(client!.UserId, t);
                }
                else 
                {
                    Log.Error($"OnNotifyAsync method:{method} connId:{connId} not found!");
                }
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
                if (TryGetClient(connId, out var client))
                {
                    var t = parser.ParseFrom(data);
                    var ret = callback(t);
                    if (ret.IsOk)
                    {
                        await Response(client!.UserId, client!.ConnId, msgId, ret.Value.ToByteArray());
                    }
                    else
                    {
                        await Error(client!.UserId, client!.ConnId, msgId, ret.Error);
                    }
                }
                else 
                {
                    Log.Error($"OnRequest method:{method} connId:{connId} not found!");
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
                if (TryGetClient(connId, out var client))
                {
                    var t = parser.ParseFrom(data);
                    var ret = await callback(t);
                    if (ret.IsOk)
                    {
                        await Response(client!.UserId, client!.ConnId, msgId, ret.Value.ToByteArray());
                    }
                    else
                    {
                        await Error(client!.UserId, client!.ConnId, msgId, ret.Error);
                    }
                }
                else 
                {
                    Log.Error($"OnRequestAsync method:{method} connId:{connId} not found!");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Do Request method:{method} ex:{ex}");
            }
        });
    }

    public async Task OnDoMsg(string connId, string msgId, GateNetwork gate, string method, ByteString message)
    {
        if (_onMsg.TryGetValue(method, out var action))
        {
            var kind = msgId.Length > 0 ? "request" : "notify";
            using var activity = Tracing.Start("hub." + method, ActivityKind.Server,
                ("rpc.method", method), ("rpc.kind", kind), ("entity.type", EntityType));
            using var timer = Metrics.Time("abelkhan_hub_message_duration_seconds",
                "Hub message handling latency", Metrics.HandlerBuckets, ("method", method));
            var started = Stopwatch.GetTimestamp();

            try
            {
                await action(connId, msgId, gate, message);
            }
            catch(Exception ex)
            {
                Metrics.Count("abelkhan_hub_handler_errors_total", "Hub handler exceptions", 1, ("method", method));
                Tracing.MarkError(activity, ex);
                Log.Error($"OnDoMsg method:{method} ex:{ex}");
            }
            finally
            {
                Metrics.Count("abelkhan_hub_messages_total", "Hub messages handled", 1,
                    ("method", method), ("kind", kind));
                LogSlow(activity, method, started);
            }
        }
        else
        {
            Metrics.Count("abelkhan_hub_message_unrouted_total", "Hub messages with no handler", 1,
                ("method", method));
            Log.Error($"OnDoMsg method:{method} not exist");
        }
    }

    /// <summary>慢调用日志：超过 <see cref="MonitoringOptions.SlowLogThresholdMs"/> 打一条 warn（带 span 里已有上下文）。</summary>
    private void LogSlow(Activity? activity, string method, long startedTimestamp)
    {
        var threshold = Metrics.Options.SlowLogThresholdMs;
        if (threshold <= 0)
        {
            return;
        }

        var elapsedMs = Stopwatch.GetElapsedTime(startedTimestamp).TotalMilliseconds;
        if (elapsedMs < threshold)
        {
            return;
        }

        activity?.SetTag("slow", true);
        Log.Warn("slow message entity:{0} type:{1} method:{2} elapsed:{3:0.0}ms",
            EntityId, EntityType, method, elapsedMs);
    }
}