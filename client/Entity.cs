using engine;
using Google.Protobuf;

namespace client;

/// <summary>
/// 客户端实体：对应服务器(hub)侧的一个 entity。
///
/// 1) 处理服务器下发的 request / notify：RegisterRequest / RegisterNotify；
/// 2) 向服务器侧 entity 发 request / notify：Request / Notify；
/// 3) 服务器 request 的回包由框架自动完成：handler 返回 Ok → response，返回 Err → error(err_msg)；
///    handler 抛异常也会回 error，避免 hub 侧一直挂着。
///
/// 服务器下发的消息都排在本实体自己的串行队列里按到达顺序执行（Post），
/// 不同实体之间互不阻塞。
/// </summary>
public abstract class Entity(string entityId, string entityType)
{
    public string EntityId { get; } = entityId;
    public string EntityType { get; } = entityType;

    /// <summary>框架注入的会话，游戏侧不用管</summary>
    internal Client? Session { get; set; }

    /// <summary>
    /// 重连重同步用：最后一次被服务器消息（CreatePlayerEntity/CreateRemoteEntity/RefreshEntity）
    /// 认领时的重连轮次。-1 表示本轮重连还没被认领，超时后会被本地删掉。
    /// </summary>
    internal long ConfirmedEpoch { get; set; } = -1;

    // 服务器 request / notify 的处理函数，key = method(即 CallRpc.proto_name)
    private readonly Dictionary<string, Func<string, ByteString, Task>> _onRequest = new();
    private readonly Dictionary<string, Func<ByteString, Task>> _onNotify = new();

    // 每个实体一条串行队列
    private readonly object _queueLock = new();
    private Task _queue = Task.CompletedTask;

    internal void Post(Action action)
    {
        Post(() =>
        {
            action();
            return Task.CompletedTask;
        });
    }

    internal void Post(Func<Task> work)
    {
        lock (_queueLock)
        {
            _queue = _queue.ContinueWith(async _ =>
            {
                try
                {
                    await work();
                }
                catch (Exception e)
                {
                    Log.Error($"Entity:{EntityId} type:{EntityType} post task ex:{e}");
                }
            }, TaskScheduler.Default).Unwrap();
        }
    }

    // ---------------- 生命周期（框架回调，按需重写） ----------------

    /// <summary>实体被创建（收到 CreatePlayerEntity / CreateRemoteEntity），argv 是服务器给的实体数据</summary>
    public virtual void OnCreate(byte[] argv)
    {
    }

    /// <summary>实体被删除（收到 DeleteRemoteEntity）</summary>
    public virtual void OnDelete()
    {
    }

    /// <summary>实体数据被刷新（收到 RefreshEntity）</summary>
    public virtual void OnRefresh(byte[] argv)
    {
    }

    // ---------------- 注册服务器下发消息的处理 ----------------

    /// <summary>注册服务器 request 的处理：返回 Ok 自动回 response，返回 Err 自动回 error</summary>
    public void RegisterRequest<T0, T1>(string method, Func<T0, Result<T1, string>> callback)
        where T0 : IMessage<T0>, new()
        where T1 : IMessage<T1>, new()
    {
        RegisterRequest<T0, T1>(method, argv => Task.FromResult(callback(argv)));
    }

    /// <summary>注册服务器 request 的处理（异步版本）</summary>
    public void RegisterRequest<T0, T1>(string method, Func<T0, Task<Result<T1, string>>> callback)
        where T0 : IMessage<T0>, new()
        where T1 : IMessage<T1>, new()
    {
        var parser = new MessageParser<T0>(() => new T0());
        _onRequest.Add(method, async (msgId, content) =>
        {
            var session = Session;
            if (session == null)
            {
                Log.Error($"Entity:{EntityId} not bind session, method:{method}");
                return;
            }

            try
            {
                var argv = parser.ParseFrom(content);
                var ret = await callback(argv);
                if (ret.IsOk)
                {
                    await session.ResponseClient(EntityId, msgId, ret.Value.ToByteArray());
                }
                else
                {
                    await session.ErrorClient(EntityId, msgId, ret.Error);
                }
            }
            catch (Exception e)
            {
                // 处理失败也必须回 error，否则 hub 侧的 request 永远等不到回包
                Log.Error($"Entity:{EntityId} type:{EntityType} do request method:{method} ex:{e}");
                await session.ErrorClient(EntityId, msgId, e.Message);
            }
        });
    }

    /// <summary>注册服务器 notify 的处理</summary>
    public void RegisterNotify<T>(string method, Action<T> callback)
        where T : IMessage<T>, new()
    {
        RegisterNotify<T>(method, argv =>
        {
            callback(argv);
            return Task.CompletedTask;
        });
    }

    /// <summary>注册服务器 notify 的处理（异步版本）</summary>
    public void RegisterNotify<T>(string method, Func<T, Task> callback)
        where T : IMessage<T>, new()
    {
        var parser = new MessageParser<T>(() => new T());
        _onNotify.Add(method, async content =>
        {
            try
            {
                var argv = parser.ParseFrom(content);
                await callback(argv);
            }
            catch (Exception e)
            {
                Log.Error($"Entity:{EntityId} type:{EntityType} do notify method:{method} ex:{e}");
            }
        });
    }

    // ---------------- 向服务器侧 entity 发消息 ----------------

    /// <summary>向服务器侧这个 entity 发 request，等它的 response（ErrMsg 非空则是 Err）</summary>
    public Task<Result<T1, string>> Request<T0, T1>(string method, T0 argv)
        where T0 : IMessage<T0>
        where T1 : IMessage<T1>, new()
    {
        var session = Session;
        if (session == null)
        {
            Log.Error($"Entity:{EntityId} not bind session, method:{method}");
            return Task.FromResult(Result<T1, string>.Err("entity not bind session"));
        }

        return session.Request<T0, T1>(EntityId, method, argv);
    }

    /// <summary>向服务器侧这个 entity 发 notify（不回包）</summary>
    public Task Notify<T>(string method, T argv)
        where T : IMessage<T>
    {
        var session = Session;
        if (session == null)
        {
            Log.Error($"Entity:{EntityId} not bind session, method:{method}");
            return Task.CompletedTask;
        }

        return session.Notify(EntityId, method, argv);
    }

    // ---------------- 框架内部：分发服务器下发的消息 ----------------

    /// <summary>服务器 request：交给注册的 handler 处理，handler 负责回包</summary>
    internal async Task OnRequestMsg(string msgId, string method, ByteString content)
    {
        if (_onRequest.TryGetValue(method, out var handler))
        {
            await handler(msgId, content);
            return;
        }

        Log.Error($"Entity:{EntityId} type:{EntityType} request method:{method} not exist");
        if (Session != null)
        {
            await Session.ErrorClient(EntityId, msgId, $"method:{method} not register");
        }
    }

    /// <summary>服务器 notify：交给注册的 handler 处理</summary>
    internal async Task OnNotifyMsg(string method, ByteString content)
    {
        if (_onNotify.TryGetValue(method, out var handler))
        {
            await handler(content);
            return;
        }

        Log.Error($"Entity:{EntityId} type:{EntityType} notify method:{method} not exist");
    }
}
