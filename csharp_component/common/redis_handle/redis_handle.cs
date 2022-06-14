using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace abelkhan
{
    public class redis_handle
    {
        private ConnectionMultiplexer connectionMultiplexer;
        private RedisConnectionHelper _connHelper;
        private IDatabase database;

        public redis_handle(string connUrl)
        {
            _connHelper = new RedisConnectionHelper(connUrl, "RedisForCache");
            _connHelper.ConnectOnStartup(ref connectionMultiplexer, ref database);
        }

        void Recover(System.Exception e)
        {
            _connHelper.Recover(ref connectionMultiplexer, ref database, e);
        }

        public Task<bool> SetStrData(string key, string data, long timeout)
        {
            while (true)
            {
                try
                {
                    return database.StringSetAsync(key, data, System.TimeSpan.FromMilliseconds(timeout));
                }
                catch (StackExchange.Redis.RedisTimeoutException e)
                {
                    Recover(e);
                }
            }
        }

        public Task<bool> SetData<T>(string key, T data, long timeout)
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
                catch (StackExchange.Redis.RedisTimeoutException e)
                {
                    Recover(e);
                }
            }
        }

        public async Task<T> GetData<T>(string key)
        {
            string json = await GetStrData(key);
            if (json == null)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
