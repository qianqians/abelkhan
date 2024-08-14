using System;
using System.Globalization;
using StackExchange.Redis;
using System.Threading;

namespace Abelkhan
{

    public class RedisConnectionHelper
    {
        private static readonly int connectRetry = 3;
        private static readonly int connectTimeout = 5000;
        private static readonly int keepAlive = 30;
        private static readonly bool resolveDns = true;
        private static readonly ManualResetEvent _waitNotify = new ManualResetEvent(false);

        private readonly int _waitTimeout = 15000; //15s
        private readonly string _conUrl;
        private readonly string _conName;
        private readonly string _pwd;
        private readonly string _conf;
        private readonly int _db;
        private int _recoverCnt = 0;
        private int _inRecover = 0;


        public RedisConnectionHelper(string conUrl, string conName, string pwd, int db = 0)
        {
            _conUrl = conUrl;
            _conName = conName;
            _pwd = pwd;
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
            catch (StackExchange.Redis.RedisConnectionException ex)
            {
                Log.Log.err("Can NOT connect to Redis! connectRetry={0}, connectTimeout={1}ms, conex:{2}, _conf:{3}", connectRetry, connectTimeout, ex, _conf);
                throw;
            }
        }

        public void Recover(ref ConnectionMultiplexer connection_Multiplexer, ref IDatabase database, System.Exception e, Action afterRecover = null)
        {
            if (Interlocked.CompareExchange(ref _inRecover, 1, 0) == 0)
            {
                Log.Log.err("Redis Exception: {0}", e);
                Log.Log.info("Reconnect for {0}, count={1}", _conName, ++_recoverCnt);
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
                    Log.Log.err("Exit due to Recover-Failure! RecoverCount={0}, connectRetry={1}, connectTimeout={2}ms, _conf={3}", _recoverCnt, connectRetry, connectTimeout, _conf);
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
                    Log.Log.err("_waitNotify.Set() failed");
                }
                Thread.Sleep(10);
                if (!_waitNotify.Reset())
                {
                    Log.Log.err("_waitNotify.ReSet() failed");
                }
            }
            else
            {
                if (!_waitNotify.WaitOne(_waitTimeout))
                {
                    var msg = $"_waitNotifyTimeout after {_waitTimeout}ms";
                    Log.Log.err(msg);
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
                return string.Create(CultureInfo.InvariantCulture, buf, $"{conUrl},connectRetry={connectRetry},connectTimeout={connectTimeout},keepAlive={keepAlive},resolveDns={true},name={conName}");
            }
            return string.Create(CultureInfo.InvariantCulture, buf, $"{conUrl},password={_pwd},connectRetry={connectRetry},connectTimeout={connectTimeout},keepAlive={keepAlive},resolveDns={true},name={conName}");
        }
    }

}