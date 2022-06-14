using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace abelkhan
{
    public class redischannel : abelkhan.Ichannel
    {
        private string _channelName;
        private redis_mq _redis_mq_handle;

        public redischannel(string channelName, redis_mq mq_handle)
        {
            _channelName = channelName;
            _redis_mq_handle = mq_handle;
        }

        public void disconnect()
        {
        }

        public bool is_xor_key_crypt()
        {
            return false;
        }

        public void normal_send_crypt(byte[] data)
        {
        }

        public void send(byte[] data)
        {

        }

    }

    public class redis_mq
    {
        private ConnectionMultiplexer connectionMultiplexer;
        private RedisConnectionHelper _connHelper;
        private IDatabase database;

        private string listen_channel_name;
        private bool run_flag = true;
        private ConcurrentDictionary<string, redischannel> channels;

        public redis_mq(string connUrl, string _listen_channel_name)
        {
            listen_channel_name = _listen_channel_name;
            channels = new ConcurrentDictionary<string, redischannel>();

            _connHelper = new RedisConnectionHelper(connUrl, "RedisForMQ");
            _connHelper.ConnectOnStartup(ref connectionMultiplexer, ref database);
        }

        void Recover(System.Exception e)
        {
            _connHelper.Recover(ref connectionMultiplexer, ref database, e);
        }

        public void sendmsg(string ch_name, byte[] data)
        {
            var _ch_name_size = ch_name.;
            auto _totle_len = 4 + _ch_name_size + (uint32_t)len;
            auto _totle_buf = (char*)malloc(_totle_len);
            _totle_buf[0] = _ch_name_size & 0xff;
            _totle_buf[1] = _ch_name_size >> 8 & 0xff;
            _totle_buf[2] = _ch_name_size >> 16 & 0xff;
            _totle_buf[3] = _ch_name_size >> 24 & 0xff;
            memcpy(&_totle_buf[4], listen_channle_name.c_str(), _ch_name_size);
            memcpy(&_totle_buf[4 + _ch_name_size], data, len);

            redismqbuff buf;
            buf.ch_name = channle_name;
            buf.buf = _totle_buf;
            buf.len = _totle_len;
            send_data.push(buf);
        }

    }
}
