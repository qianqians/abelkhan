namespace abelkhan;

using System;
using System.Globalization;
using StackExchange.Redis;
using System.Threading;

public class RedisConnectionHelper
{
    public static int connectRetry = 3;
    public static int connectTimeout = 5000;
    public static int keepAlive = 30;
    public static bool resolveDns = true;

    string _conUrl;
    string _conName;
    string _conf;
    int _db;
    int _recoverCnt = 0;
    private Int32 _inRecover = 0;
    private int _waitTimeout = 15000; //15s
    private static ManualResetEvent _waitNotify = new ManualResetEvent(false);


    public RedisConnectionHelper(string conUrl, string conName, int db = 0)
    {
        _conUrl = conUrl;
        _conName = conName;
        _db = db;
        _conf = BuildConfig(conUrl, conName);
    }

    public void ConnectOnStartup(ref ConnectionMultiplexer connection_Multiplexer, ref IDatabase database)
    {
        try
        {
            if (connection_Multiplexer != null)
            {
                connection_Multiplexer.Close(allowCommandsToComplete: false);
            }
            connection_Multiplexer = ConnectionMultiplexer.Connect(_conf);
            database = connection_Multiplexer.GetDatabase(_db);
        }
        catch (StackExchange.Redis.RedisConnectionException conex)
        {
            log.Log.err("Can NOT connect to Redis! connectRetry={0}, connectTimeout={1}ms", connectRetry, connectTimeout);
            throw conex;
        }
    }


    public void Recover(ref ConnectionMultiplexer connection_Multiplexer, ref IDatabase database, System.Exception e, Action afterRecover = null)
    {
        if (Interlocked.CompareExchange(ref _inRecover, 1, 0) == 0)
        {
            log.Log.err("Redis Exception: {0}", e);
            log.Log.info("Reconnect for {0}, count={1}", _conName, ++_recoverCnt);
            try
            {
                if (connection_Multiplexer != null)
                {
                    connection_Multiplexer.Close(allowCommandsToComplete: false);
                }
                connection_Multiplexer = ConnectionMultiplexer.Connect(_conf);
                database = connection_Multiplexer.GetDatabase(_db);
            }
            catch (StackExchange.Redis.RedisConnectionException)
            {
                log.Log.err("Exit due to Recover-Failure! RecoverCount={0}, connectRetry={1}, connectTimeout={2}ms", _recoverCnt, connectRetry, connectTimeout);
                Thread.Sleep(10);
                Environment.Exit(1);
            }
            if (afterRecover != null)
            {
                afterRecover();
            }
            _inRecover = 0;
            if (!_waitNotify.Set())
            {
                log.Log.err("_waitNotify.Set() failed");
            }
            Thread.Sleep(10);
            if (!_waitNotify.Reset())
            {
                log.Log.err("_waitNotify.ReSet() failed");
            }
        }
        else
        {
            if (!_waitNotify.WaitOne(_waitTimeout))
            {
                var msg = $"_waitNotifyTimeout after {_waitTimeout}ms";
                log.Log.err(msg);
                Thread.Sleep(10);
                Environment.Exit(1);
            }
        }
    }


    string BuildConfig(string conUrl, string conName)
    {
        Span<char> buf = stackalloc char[512];
        return string.Create(CultureInfo.InvariantCulture, buf, $"{conUrl},connectRetry={connectRetry},connectTimeout={connectTimeout},keepAlive={keepAlive},resolveDns={true},name={conName}");
    }
}