using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Abelkhan
{
    public class RedisHandle
    {
        private ConnectionMultiplexer connectionMultiplexer;
        private RedisConnectionHelper _connHelper;
        private IDatabase database;

        public RedisHandle(string connUrl, string pwd)
        {
            _connHelper = new RedisConnectionHelper(connUrl, "RedisForCache", pwd);
            _connHelper.ConnectOnStartup(ref connectionMultiplexer, ref database);
        }

        void Recover(System.Exception e)
        {
            _connHelper.Recover(ref connectionMultiplexer, ref database, e);
        }

        public Task<bool> Expire(string key, int timeout)
        {
            while (true)
            {
                try
                {
                    return database.KeyExpireAsync(key, System.TimeSpan.FromMilliseconds(timeout));
                }
                catch (RedisTimeoutException e)
                {
                    Recover(e);
                }
            }
        }

        public Task<bool> SetStrData(string key, string data, int timeout)
        {
            while (true)
            {
                try
                {
                    if (timeout != 0)
                    {
                        return database.StringSetAsync(key, data, System.TimeSpan.FromMilliseconds(timeout));
                    }
                    else
                    {
                        return database.StringSetAsync(key, data);
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
            while (true)
            {
                try
                {
                    return database.StringGetAsync(key);
                }
                catch (RedisTimeoutException e)
                {
                    Recover(e);
                }
            }
        }

        public async Task<T> GetData<T>(string key)
        {
            string json = await GetStrData(key);
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }

        public bool DelData(string key)
        {
            while (true)
            {
                try
                {
                    return database.KeyDelete(key);
                }
                catch (RedisTimeoutException e)
                {
                    Recover(e);
                }
            }
        }

        public Task<long> PushList<T>(string key, T data)
        {
            while (true)
            {
                try
                {
                    return database.ListLeftPushAsync(key, JsonConvert.SerializeObject(data));
                }
                catch (RedisTimeoutException e)
                {
                    Recover(e);
                }
            }
        }

        public void PopList(string key, long count)
        {
            while (true)
            {
                try
                {
                    database.ListRightPopAsync(key, count);
                    return;
                }
                catch (RedisTimeoutException e)
                {
                    Recover(e);
                }
            }
        }

        public async Task<T> RandomList<T>(string key)
        {
            while (true)
            {
                try
                {
                    var count = await database.ListLengthAsync(key);
                    var index = RandomHelper.RandomInt((int)count);
                    string json = await database.ListGetByIndexAsync(key, index);
                    if (string.IsNullOrEmpty(json))
                    {
                        return default;
                    }
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (RedisTimeoutException e)
                {
                    Recover(e);
                }
            }
        }

        public async Task<T> GetListElem<T>(string key, int index)
        {
            while (true)
            {
                try
                {
                    var count = await database.ListLengthAsync(key);
                    string json = await database.ListGetByIndexAsync(key, index);
                    if (string.IsNullOrEmpty(json))
                    {
                        return default;
                    }
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (RedisTimeoutException e)
                {
                    Recover(e);
                }
            }
        }

        public async Task DeleteListElem(string key, int index)
        {
            while (true)
            {
                try
                {
                    var v = await database.ListGetByIndexAsync(key, index);
                    await database.ListRemoveAsync(key, v);
                }
                catch (RedisTimeoutException e)
                {
                    Recover(e);
                }
            }
        }

        public async Task<List<T> > GetList<T>(string key)
        {
            while (true)
            {
                try
                {
                    var data = await database.ListRangeAsync(key);
                    if (data.Length <= 0)
                    {
                        return default;
                    }
                    var dataResult = new List<T>();
                    foreach (var item in data)
                    {
                        dataResult.Add(JsonConvert.DeserializeObject<T>(item));
                    }
                    return dataResult;
                }
                catch (RedisTimeoutException e)
                {
                    Recover(e);
                }
            }
        }

        public async Task Lock(string key, string token, uint timeout)
        {
            var wait_time = 8;
            while (true)
            {
                try
                {
                    var ret = await database.LockTakeAsync(key, token, System.TimeSpan.FromMilliseconds(timeout));
                    if (!ret)
                    {
                        var task = new TaskCompletionSource();
                        var tasks = new Task[1] { task.Task };
                        Task.WaitAny(tasks, wait_time);
                        wait_time *= 2;
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

        public async Task UnLock(string key, string token)
        {
            while (true)
            {
                try
                {
                    await database.LockReleaseAsync(key, token);
                    break;
                }
                catch (RedisTimeoutException e)
                {
                    Recover(e);
                }
            }
        }
    }
}
