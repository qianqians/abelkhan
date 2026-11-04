using System.Collections.Concurrent;
using System.Globalization;
using System.Net.Sockets;
using StackExchange.Redis;
using Newtonsoft.Json;
// ReSharper disable MemberCanBePrivate.Global

namespace core;

public class RedisConnectionHelper
{
    private const int ConnectRetry = 3;
    private const int ConnectTimeout = 5000;
    private const int KeepAlive = 30;
    private readonly ManualResetEvent _waitNotify = new(false);

    private const int WaitTimeout = 999;
    private readonly string _conName;
    private readonly string _pwd;
    private readonly string _conf;
    private readonly int _db;
    private int _recoverCnt = 0;
    private int _inRecover = 0;


    public RedisConnectionHelper(string conUrl, string conName, string pwd, int db = 0)
    {
        _conName = conName;
        _pwd = pwd;
        _db = db;
        _conf = BuildConfig(conUrl, conName);
    }

    public void ConnectOnStartup(ref ConnectionMultiplexer? connectionMultiplexer, ref IDatabase? database)
    {
        try
        {
            if (connectionMultiplexer != null)
            {
                connectionMultiplexer.Close(allowCommandsToComplete: false);
            }
            connectionMultiplexer = ConnectionMultiplexer.Connect(_conf);
            database = connectionMultiplexer.GetDatabase(_db);
        }
        catch (Exception ex)
        {
            Log.Error("Can NOT connect to Redis! connectRetry:{0}, connectTimeout:{1}ms, ex:{2}, _conf:{3}", ConnectRetry, ConnectTimeout, ex, _conf);
            throw;
        }
    }

    public void Recover(ref ConnectionMultiplexer? connectionMultiplexer, ref IDatabase? database, Exception? e, Action? afterRecover = null)
    {
        Log.Info("Reconnect for {0}, count={1}", _conName, ++_recoverCnt);
        try
        {
            if (Interlocked.CompareExchange(ref _inRecover, 1, 0) == 0)
            {
                if (e != null)
                {
                    Log.Error("Redis Exception:{0}", e);
                }

                if (connectionMultiplexer != null)
                {
                    connectionMultiplexer.Close(allowCommandsToComplete: false);
                }

                connectionMultiplexer = ConnectionMultiplexer.Connect(_conf);
                database = connectionMultiplexer.GetDatabase(_db);

                if (afterRecover != null)
                {
                    afterRecover();
                }

                Interlocked.Exchange(ref _inRecover, 0);
                if (!_waitNotify.Set())
                {
                    Log.Error("_waitNotify.Set() failed");
                }

                Thread.Sleep(10);
                if (!_waitNotify.Reset())
                {
                    Log.Error("_waitNotify.ReSet() failed");
                }
            }
            else
            {
                if (!_waitNotify.WaitOne(WaitTimeout))
                {
                    Log.Error($"_waitNotifyTimeout after {WaitTimeout}ms");
                    Thread.Sleep(10);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error("Exit due to Recover-Failure! RecoverCount:{0}, connectRetry:{1}, connectTimeout:{2}ms, _conf:{3}, ex:{4}",
                _recoverCnt, ConnectRetry, ConnectTimeout, _conf, ex);
            Interlocked.Exchange(ref _inRecover, 0);
        }
    }


    string BuildConfig(string conUrl, string conName)
    {
        Span<char> buf = stackalloc char[512];
        if (string.IsNullOrEmpty(_pwd))
        {
            return string.Create(CultureInfo.InvariantCulture, buf, $"{conUrl},connectRetry={ConnectRetry},connectTimeout={ConnectTimeout},keepAlive={KeepAlive},resolveDns={true},name={conName}");
        }
        return string.Create(CultureInfo.InvariantCulture, buf, $"{conUrl},password={_pwd},connectRetry={ConnectRetry},connectTimeout={ConnectTimeout},keepAlive={KeepAlive},resolveDns={true},name={conName}");
    }
}

public class RedisHandle
{
    private ConnectionMultiplexer? _connectionMultiplexer;
    private readonly RedisConnectionHelper _connHelper;
    private IDatabase? _database;

    /// <summary>
    /// 已订阅的 pub/sub 通道。重连后要挨个重新订阅，否则 Redis 一抖就永久收不到广播。
    /// </summary>
    private readonly ConcurrentDictionary<string, Action<byte[]>> _subscriptions = new();

    public RedisHandle(string connUrl, string pwd)
    {
        _connHelper = new RedisConnectionHelper(connUrl, "RedisForCache", pwd);
        _connHelper.ConnectOnStartup(ref _connectionMultiplexer, ref _database);
    }

    void Recover(Exception e)
    {
        // afterRecover 在重连成功（且是本线程执行的）之后回调，用它把订阅补回去
        _connHelper.Recover(ref _connectionMultiplexer, ref _database, e, Resubscribe);
    }

    /// <summary>
    /// 用统一的重连逻辑执行任意 Redis 操作（EVAL / HASH / ZSET 等）。
    ///
    /// 只有连接类异常（断线 / 超时 / 网络）才会触发重连并重试；Redis 返回的脚本错误
    /// （例如 Lua 语法错误）属于程序缺陷，直接抛出，避免死循环掩盖问题。
    /// </summary>
    public async ValueTask<T> Execute<T>(string operation, Func<IDatabase, Task<T>> action)
    {
        while (true)
        {
            var db = _database;
            if (db == null)
            {
                throw new InvalidOperationException("redis not connected");
            }

            try
            {
                return await action(db);
            }
            catch (Exception e) when (e is RedisConnectionException or RedisTimeoutException
                                          or IOException or SocketException)
            {
                Log.Error("redis execute {0} failed, will reconnect: {1}", operation, e.Message);
                Recover(e);
            }
        }
    }

    /// <summary>同 <see cref="Execute{T}"/>，用于没有返回值的命令。</summary>
    public async ValueTask Execute(string operation, Func<IDatabase, Task> action)
    {
        await Execute<object?>(operation, async db =>
        {
            await action(db);
            return null;
        });
    }

    /// <summary>读取 list 的原始元素（不做反序列化，返回每条元素的原始字符串）。</summary>
    public async ValueTask<List<string>> GetListRaw(string key)
    {
        if (_database == null)
        {
            return new List<string>();
        }

        while (true)
        {
            try
            {
                var result = new List<string>();
                var data = await _database.ListRangeAsync(key);
                foreach (var item in data)
                {
                    if (item.HasValue)
                    {
                        result.Add(item.ToString());
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    /// <summary>裁剪 list，只保留 [start, stop] 区间（负索引从尾部算，如 -200 -1 = 最近 200 条）。</summary>
    public async Task LTrim(string key, long start, long stop)
    {
        if (_database == null)
        {
            return;
        }

        while (true)
        {
            try
            {
                await _database.ListTrimAsync(key, start, stop);
                return;
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    /// <summary>向 set 添加一个成员。</summary>
    public Task<bool> SAdd(string key, string member)
    {
        if (_database == null)
        {
            return Task.FromResult(false);
        }

        while (true)
        {
            try
            {
                return _database.SetAddAsync(key, member);
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    /// <summary>从 set 删除一个成员。返回 true 表示成员存在并被删除。</summary>
    public Task<bool> SRem(string key, string member)
    {
        if (_database == null)
        {
            return Task.FromResult(false);
        }

        while (true)
        {
            try
            {
                return _database.SetRemoveAsync(key, member);
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    /// <summary>读取 set 的全部成员。</summary>
    public async ValueTask<HashSet<string>> SMembers(string key)
    {
        if (_database == null)
        {
            return new HashSet<string>();
        }

        while (true)
        {
            try
            {
                var result = new HashSet<string>();
                var values = await _database.SetMembersAsync(key);
                foreach (var value in values)
                {
                    result.Add(value.ToString());
                }
                return result;
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    public Task<bool> Expire(string key, int timeout)
    {
        if (_database == null)
        {
            return Task.FromResult(false);
        }
        
        while (true)
        {
            try
            {
                return _database.KeyExpireAsync(key, System.TimeSpan.FromMilliseconds(timeout));
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    public Task<bool> SetStrData(string key, string data, int timeout)
    {
        if (_database == null)
        {
            return Task.FromResult(false);
        }
        
        while (true)
        {
            try
            {
                if (timeout != 0)
                {
                    return _database.StringSetAsync(key, data, System.TimeSpan.FromMilliseconds(timeout));
                }
                else
                {
                    return _database.StringSetAsync(key, data);
                }
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    public Task<bool> SetData<T>(string key, T data, int timeout = 0)
    {
        return SetStrData(key, JsonConvert.SerializeObject(data), timeout);
    }

    public Task<RedisValue> GetStrData(string key)
    {
        if (_database == null)
        {
            return Task.FromResult(RedisValue.Null);
        }
        
        while (true)
        {
            try
            {
                return _database.StringGetAsync(key);
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    public async ValueTask<T?> GetData<T>(string key)
    {
        var json = await GetStrData(key);
        if (json.IsNull || string.IsNullOrEmpty(json))
        {
            return default(T);
        }
        return JsonConvert.DeserializeObject<T>(json!);
    }

    public bool DelData(string key)
    {
        if (_database == null)
        {
            return false;
        }
        
        while (true)
        {
            try
            {
                return _database.KeyDelete(key);
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    public Task<long> PushList(string key, byte[] data)
    {
        if (_database == null)
        {
            return Task.FromResult((long)0);
        }
        
        while (true)
        {
            try
            {
                return _database.ListRightPushAsync(key, data);
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }
    
    public Task<long> PushList<T>(string key, T data)
    {
        if (_database == null)
        {
            return Task.FromResult((long)0);
        }
        
        while (true)
        {
            try
            {
                return _database.ListRightPushAsync(key, JsonConvert.SerializeObject(data));
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    public async Task<List<byte[]>?>? PopList(string key, long count)
    {
        if (_database == null)
        {
            return null;
        }
        
        while (true)
        {
            try
            {
                var result = await _database.ListLeftPopAsync(key, count);
                return result.Select(d => (byte[])d!).ToList();
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    public async Task<T?> RandomList<T>(string key)
    {
        if (_database == null)
        {
            return default;
        }
        
        while (true)
        {
            try
            {
                var count = await _database.ListLengthAsync(key);
                var index = RandomHelper.RandomInt((int)count);
                var json = await _database.ListGetByIndexAsync(key, index);
                if (json.IsNull || string.IsNullOrEmpty(json))
                {
                    return default;
                }
                return JsonConvert.DeserializeObject<T>(json!);
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    public async Task<byte[]?> GetListElem(string key, int index)
    {
        if (_database == null)
        {
            return null;
        }
        
        while (true)
        {
            try
            {
                var count = await _database.ListLengthAsync(key);
                var json = await _database.ListGetByIndexAsync(key, index);
                if (json.IsNull)
                {
                    return null;
                }
                return json!;
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }
    
    public async Task<T?> GetListElem<T>(string key, int index)
    {
        if (_database == null)
        {
            return default;
        }
        
        while (true)
        {
            try
            {
                var count = await _database.ListLengthAsync(key);
                var json = await _database.ListGetByIndexAsync(key, index);
                if (json.IsNull || string.IsNullOrEmpty(json))
                {
                    return default;
                }
                return JsonConvert.DeserializeObject<T>(json!);
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    public Task<T?> Front<T>(string key)
    {
        return GetListElem<T>(key, 0);
    }
    
    public Task<byte[]?> Front(string key)
    {
        return GetListElem(key, 0);
    }

    public async ValueTask DeleteListElem(string key)
    {
        if (_database == null)
        {
            return;
        }
        
        while (true)
        {
            try
            {
                var v = await _database.ListLeftPopAsync(key, (long)1);
                break;
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    public async ValueTask<List<T?>?> GetList<T>(string key)
    {
        if (_database == null)
        {
            return null;
        }
        
        while (true)
        {
            try
            {
                var data = await _database.ListRangeAsync(key);
                if (data.Length <= 0)
                {
                    return null;
                }
                var dataResult = new List<T?>();
                foreach (var item in data)
                {
                    if (item.IsNullOrEmpty || item.IsNull || string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    dataResult.Add(JsonConvert.DeserializeObject<T>(item!));
                }
                return dataResult;
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    /// <summary>
    /// 枚举匹配 pattern 的 key（基于 SCAN）。仅供管理端这类低频场景使用，
    /// 不要在热路径上调用。
    /// </summary>
    public async ValueTask<List<string>> GetKeys(string pattern)
    {
        var result = new List<string>();
        if (_connectionMultiplexer == null)
        {
            return result;
        }

        while (true)
        {
            try
            {
                foreach (var endpoint in _connectionMultiplexer.GetEndPoints())
                {
                    var server = _connectionMultiplexer.GetServer(endpoint);
                    await foreach (var key in server.KeysAsync(pattern: pattern))
                    {
                        result.Add(key.ToString());
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    public async ValueTask Lock(string key, string token, uint timeout)
    {
        if (_database == null)
        {
            return;
        }
        
        var waitTime = 1;
        while (true)
        {
            try
            {
                var ret = await _database.LockTakeAsync(key, token, System.TimeSpan.FromMilliseconds(timeout));
                if (!ret)
                {
                    await Task.Delay(waitTime);
                    if (waitTime < 32)
                    {
                        waitTime *= 2;
                    }
                    continue;
                }
                break;
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    public async ValueTask UnLock(string key, string token)
    {
        if (_database == null)
        {
            return;
        }
        
        while (true)
        {
            try
            {
                await _database.LockReleaseAsync(key, token);
                break;
            }
            catch (Exception e)
            {
                Recover(e);
            }
        }
    }

    // ================================================================
    // Pub/Sub（跨 hub 广播用）
    // ================================================================

    /// <summary>向通道发布一条消息，返回收到它的订阅者数量。</summary>
    public async ValueTask<long> Publish(string channel, byte[] payload)
    {
        while (true)
        {
            var mux = _connectionMultiplexer;
            if (mux == null)
            {
                throw new InvalidOperationException("redis not connected");
            }

            try
            {
                return await mux.GetSubscriber().PublishAsync(RedisChannel.Literal(channel), payload);
            }
            catch (Exception e) when (e is RedisConnectionException or RedisTimeoutException
                                          or IOException or SocketException)
            {
                Log.Error("redis publish {0} failed, will reconnect: {1}", channel, e.Message);
                Recover(e);
            }
        }
    }

    /// <summary>
    /// 订阅一个通道。同一个通道重复订阅会替换处理器。
    /// 处理器在 Redis 的接收线程上执行，耗时操作请自己抛到线程池。
    /// </summary>
    public void Subscribe(string channel, Action<byte[]> handler)
    {
        _subscriptions[channel] = handler;
        SubscribeNow(channel, handler);
    }

    private void SubscribeNow(string channel, Action<byte[]> handler)
    {
        try
        {
            _connectionMultiplexer?.GetSubscriber().Subscribe(RedisChannel.Literal(channel),
                (_, value) =>
                {
                    try
                    {
                        handler(value.HasValue ? (byte[])value! : Array.Empty<byte>());
                    }
                    catch (Exception e)
                    {
                        Log.Error("redis subscribe handler {0} err:{1}", channel, e);
                    }
                });
        }
        catch (Exception e)
        {
            Log.Error("redis subscribe {0} failed: {1}", channel, e.Message);
        }
    }

    /// <summary>重连后重新订阅所有通道。</summary>
    private void Resubscribe()
    {
        foreach (var (channel, handler) in _subscriptions)
        {
            SubscribeNow(channel, handler);
        }
    }
}