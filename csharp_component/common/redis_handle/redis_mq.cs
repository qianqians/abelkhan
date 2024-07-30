using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Service;
using System.Linq;

namespace Abelkhan
{
    public class Redischannel : Ichannel
    {
        private readonly string _channelName;
        private readonly RedisMQ _redis_mq_handle;

        public readonly ChannelOnRecv _channel_onrecv;

        public Redischannel(string channelName, RedisMQ mq_handle)
        {
            _channelName = channelName;
            _redis_mq_handle = mq_handle;

            _channel_onrecv = new ChannelOnRecv(this);
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
            _redis_mq_handle.sendmsg(_channelName, data);
        }

    }

    public class RedisMQ
    {
        private readonly RedisConnectionHelper _connHelper;
        private ConnectionMultiplexer connectionMultiplexer;
        private IDatabase database;

        private readonly Timerservice _timer;
        private readonly string main_channel_name;
        private readonly Task th_recv;
        private readonly long tick_time;
        private bool run_flag = true;

        private readonly ConcurrentQueue<string> listen_channel_names;
        private RedisKey[] ch_names = null;

        private readonly ConcurrentDictionary<string, Redischannel> channels;

        private readonly Dictionary<string, Queue<RedisValue>> wait_send_data;
        private readonly List<KeyValuePair<string, Queue<RedisValue>>> send_data;

        public RedisMQ(Timerservice timer, string connUrl, string pwd, string _listen_channel_name, long _tick_time = 33)
        {
            _timer = timer;
            main_channel_name = _listen_channel_name;
            tick_time = _tick_time;

            listen_channel_names = new();
            listen_channel_names.Enqueue(_listen_channel_name);
            channels = new ConcurrentDictionary<string, Redischannel>();

            _connHelper = new RedisConnectionHelper(connUrl, "RedisForMQ", pwd);
            _connHelper.ConnectOnStartup(ref connectionMultiplexer, ref database);

            wait_send_data = new();
            send_data = new();
            th_recv = Task.Factory.StartNew(th_recv_poll, TaskCreationOptions.LongRunning);
        }

        public void take_over_svr(string svr_name)
        {
            listen_channel_names.Enqueue(svr_name);
        }

        void Recover(System.Exception e)
        {
            _connHelper.Recover(ref connectionMultiplexer, ref database, e);
        }

        public void close()
        {
            run_flag = false;
            th_recv.Wait();
        }

        public Redischannel connect(string ch_name)
        {
            lock (channels)
            {
                if (channels.TryGetValue(ch_name, out Redischannel ch))
                {
                    return ch;
                }

                ch = new Redischannel(ch_name, this);
                channels.TryAdd(ch_name, ch);
                return ch;
            }
        }

        public void sendmsg(string ch_name, byte[] data)
        {
            Log.Log.trace("send msg to:{0}", ch_name);

            try
            {
                var b_listen_ch_name = System.Text.Encoding.UTF8.GetBytes(main_channel_name);
                var _listen_ch_name_size = b_listen_ch_name.Length;
                var st = MemoryStreamPool.mstMgr.GetStream();
                st.WriteByte((byte)(_listen_ch_name_size & 0xff));
                st.WriteByte((byte)(_listen_ch_name_size >> 8 & 0xff));
                st.WriteByte((byte)(_listen_ch_name_size >> 16 & 0xff));
                st.WriteByte((byte)(_listen_ch_name_size >> 24 & 0xff));
                st.Write(b_listen_ch_name, 0, _listen_ch_name_size);
                st.Write(data, 0, data.Length);
                st.Position = 0;

                if (!wait_send_data.TryGetValue(ch_name, out Queue<RedisValue> send_queue))
                {
                    lock (wait_send_data)
                    {
                        if (!wait_send_data.TryGetValue(ch_name, out send_queue))
                        {
                            send_queue = new();
                            wait_send_data.Add(ch_name, send_queue);
                        }
                    }
                }
                lock (send_queue)
                {
                    send_queue.Enqueue(st.ToArray());
                }
            }
            catch (System.Exception e)
            {
                Log.Log.err("sendmsg error:{0}", e.Message);
            }
        }

        private List<Task<long> > waits = new List<Task<long>>();
        public async Task sendmsg_mq()
        {
            if (wait_send_data.Count > 0)
            {
                lock (wait_send_data)
                {
                    foreach (var (ch_name, send_queue) in wait_send_data)
                    {
                        if (send_queue.Count > 0)
                        {
                            send_data.Add(KeyValuePair.Create(ch_name, send_queue));
                        }
                    }
                }
            }

            foreach (var (ch_name, send_queue) in send_data)
            {
                RedisValue[] push_data_array = null;
                lock (send_queue)
                {
                    push_data_array = send_queue.ToArray();
                    send_queue.Clear();
                }

                retry:
                try
                {
                    waits.Add(database.ListLeftPushAsync(ch_name, push_data_array));
                }
                catch (RedisTimeoutException ex)
                {
                    Log.Log.err("ListLeftPushAsync error:{0}", ex);
                    Recover(ex);
                    goto retry;
                }
                catch (RedisConnectionException ex)
                {
                    Log.Log.err("ListLeftPushAsync error:{0}", ex);
                    Recover(ex);
                    goto retry;
                }
                catch (System.Exception ex)
                {
                    Log.Log.err("sendmsg_mq error:{0}", ex);
                    goto retry;
                }
            }
            await Task.WhenAll(waits);
            waits.Clear();
            send_data.Clear();
        }

        private async Task recvmsg_mq_ch()
        {
            var count = listen_channel_names.Count;
            if (ch_names == null || count > ch_names.Length)
            {
                ch_names = new RedisKey[count];
                for (var index = 0; index < count; index++)
                {
                    ch_names[index] = listen_channel_names.ElementAt(index);
                }
            }

            var batch_pop_data = await database.ListRightPopAsync(ch_names, 10);
            while (!batch_pop_data.IsNull)
            {
                foreach (byte[] pop_data in batch_pop_data.Values) {
                    var _ch_name_size = pop_data[0] | ((uint)pop_data[1] << 8) | ((uint)pop_data[2] << 16) | ((uint)pop_data[3] << 24);
                    var _ch_name = System.Text.Encoding.UTF8.GetString(pop_data, 4, (int)_ch_name_size);
                    var _header_len = 4 + _ch_name_size;
                    var _msg_len = pop_data.Length - _header_len;

                    using var _st = MemoryStreamPool.mstMgr.GetStream();
                    _st.Write(pop_data, (int)_header_len, (int)_msg_len);
                    _st.Position = 0;

                    if (!channels.TryGetValue(_ch_name, out Redischannel ch))
                    {
                        ch = new Redischannel(_ch_name, this);
                        channels.TryAdd(_ch_name, ch);
                    }
                    ch._channel_onrecv.on_recv(_st.ToArray());
                }

                batch_pop_data = await database.ListRightPopAsync(ch_names, 10);
            }
        }

        private async ValueTask<long> recvmsg_mq()
        {
            var tick_begin = _timer.refresh();
            await recvmsg_mq_ch();
            
            return _timer.refresh() - tick_begin;
        }

        private async void th_recv_poll()
        {
            rerun:
            try
            {
                while (run_flag)
                {
                    var tick = await recvmsg_mq();
                    if (tick < tick_time)
                    {
                        await Task.Delay((int)(tick_time - tick));
                    }
                }
            }
            catch (RedisTimeoutException ex)
            {
                Log.Log.err("ListLeftPushAsync error:{0}", ex);
                Recover(ex);
            }
            catch (RedisConnectionException ex)
            {
                Log.Log.err("ListLeftPushAsync error:{0}", ex);
                Recover(ex);
            }
            catch (System.Exception ex)
            {
                Log.Log.err("recvmsg_mq error:{0}", ex);
            }

            if (run_flag)
            {
                goto rerun;
            }
        }
    }
}
