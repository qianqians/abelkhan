using System.Globalization;
using StackExchange.Redis;
using Newtonsoft.Json;
// ReSharper disable MemberCanBePrivate.Global

namespace core;

public class RedisConnectionHelper
{
    private const int ConnectRetry = 3;
    private const int ConnectTimeout = 5000;
    private const int KeepAlive = 30;
    private static readonly ManualResetEvent WaitNotify = new(false);

    private const int WaitTimeout = 15_000;
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
        catch (RedisConnectionException ex)
        {
            Log.Error("Can NOT connect to Redis! connectRetry:{0}, connectTimeout:{1}ms, ex:{2}, _conf:{3}", ConnectRetry, ConnectTimeout, ex, _conf);
            throw;
        }
    }

    public void Recover(ref ConnectionMultiplexer? connectionMultiplexer, ref IDatabase? database, Exception? e, Action? afterRecover = null)
    {
        if (Interlocked.CompareExchange(ref _inRecover, 1, 0) == 0)
        {
            if (e != null)
            {
                Log.Error("Redis Exception:{0}", e);
            }

            Log.Info("Reconnect for {0}, count={1}", _conName, ++_recoverCnt);
            try
            {
                if (connectionMultiplexer != null)
                {
                    connectionMultiplexer.Close(allowCommandsToComplete: false);
                }
                connectionMultiplexer = ConnectionMultiplexer.Connect(_conf);
                database = connectionMultiplexer.GetDatabase(_db);
            }
            catch (RedisConnectionException)
            {
                Log.Error("Exit due to Recover-Failure! RecoverCount:{0}, connectRetry:{1}, connectTimeout:{2}ms, _conf:{3}", _recoverCnt, ConnectRetry, ConnectTimeout, _conf);
                Thread.Sleep(10);
                Environment.Exit(1);
            }
            if (afterRecover != null)
            {
                afterRecover();
            }
            _inRecover = 0;
            if (!WaitNotify.Set())
            {
                Log.Error("_waitNotify.Set() failed");
            }
            Thread.Sleep(10);
            if (!WaitNotify.Reset())
            {
                Log.Error("_waitNotify.ReSet() failed");
            }
        }
        else
        {
            if (!WaitNotify.WaitOne(WaitTimeout))
            {
                Log.Error($"_waitNotifyTimeout after {WaitTimeout}ms");
                Thread.Sleep(10);
                Environment.Exit(1);
            }
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

    public RedisHandle(string connUrl, string pwd)
    {
        _connHelper = new RedisConnectionHelper(connUrl, "RedisForCache", pwd);
        _connHelper.ConnectOnStartup(ref _connectionMultiplexer, ref _database);
    }

    void Recover(Exception e)
    {
        _connHelper.Recover(ref _connectionMultiplexer, ref _database, e);
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
            catch (RedisTimeoutException e)
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
            catch (RedisTimeoutException e)
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
            catch (RedisTimeoutException e)
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
            catch (RedisTimeoutException e)
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
                return _database.ListLeftPushAsync(key, data);
            }
            catch (RedisTimeoutException e)
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
                return _database.ListLeftPushAsync(key, JsonConvert.SerializeObject(data));
            }
            catch (RedisTimeoutException e)
            {
                Recover(e);
            }
        }
    }

    public void PopList(string key, long count)
    {
        if (_database == null)
        {
            return;
        }
        
        while (true)
        {
            try
            {
                _database.ListRightPopAsync(key, count);
                return;
            }
            catch (RedisTimeoutException e)
            {
                Recover(e);
            }
        }
    }

    public async ValueTask<T?> RandomList<T>(string key)
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
            catch (RedisTimeoutException e)
            {
                Recover(e);
            }
        }
    }

    public async ValueTask<T?> GetListElem<T>(string key, int index)
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
            catch (RedisTimeoutException e)
            {
                Recover(e);
            }
        }
    }

    public async ValueTask DeleteListElem(string key, int index)
    {
        if (_database == null)
        {
            return;
        }
        
        while (true)
        {
            try
            {
                var v = await _database.ListGetByIndexAsync(key, index);
                await _database.ListRemoveAsync(key, v);
            }
            catch (RedisTimeoutException e)
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
            catch (RedisTimeoutException e)
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
        
        var waitTime = 8;
        while (true)
        {
            try
            {
                var ret = await _database.LockTakeAsync(key, token, System.TimeSpan.FromMilliseconds(timeout));
                if (!ret)
                {
                    await Task.Delay(waitTime);
                    waitTime *= 2;
                    continue;
                }
                break;
            }
            catch (RedisTimeoutException e)
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
            catch (RedisTimeoutException e)
            {
                Recover(e);
            }
        }
    }
}