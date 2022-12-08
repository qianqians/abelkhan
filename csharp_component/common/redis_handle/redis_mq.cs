using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace abelkhan
{
    public class redischannel : abelkhan.Ichannel
    {
        private string _channelName;
        private redis_mq _redis_mq_handle;

        public channel_onrecv _channel_onrecv;

        public redischannel(string channelName, redis_mq mq_handle)
        {
            _channelName = channelName;
            _redis_mq_handle = mq_handle;

            _channel_onrecv = new channel_onrecv(this);
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

    public class redis_mq
    {
        private ConnectionMultiplexer connectionMultiplexer;
        private RedisConnectionHelper _connHelper;
        private IDatabase database;

        private string main_channel_name;
        private List<string> listen_channel_names;
        private List<string> wait_listen_channel_names;
        private bool run_flag = true;
        private Task th;

        private Dictionary<string, redischannel> channels;

        private ConcurrentQueue<Tuple<string, MemoryStream> > send_data;

        public redis_mq(string connUrl, string _listen_channel_name)
        {
            main_channel_name = _listen_channel_name;
            listen_channel_names = new() { _listen_channel_name };
            wait_listen_channel_names = new ();
            channels = new Dictionary<string, redischannel>();

            _connHelper = new RedisConnectionHelper(connUrl, "RedisForMQ");
            _connHelper.ConnectOnStartup(ref connectionMultiplexer, ref database);

            send_data = new ConcurrentQueue<Tuple<string, MemoryStream> >();

            th = new Task(th_poll);
            th.Start();
        }

        public void take_over_svr(string svr_name)
        {
            lock (wait_listen_channel_names)
            {
                wait_listen_channel_names.Add(svr_name);
            }
        }

        void Recover(System.Exception e)
        {
            _connHelper.Recover(ref connectionMultiplexer, ref database, e);
        }

        public async void close()
        {
            run_flag = false;
            await th;
        }

        public redischannel connect(string ch_name)
        {
            lock (channels)
            {
                if (channels.TryGetValue(ch_name, out redischannel ch))
                {
                    return ch;
                }

                ch = new redischannel(ch_name, this);
                channels.Add(ch_name, ch);
                return ch;
            }
        }

        public void sendmsg(string ch_name, byte[] data)
        {
            log.log.trace("send msg to:{0}", ch_name);

            var b_listen_ch_name = System.Text.Encoding.UTF8.GetBytes(main_channel_name);
            var _listen_ch_name_size = b_listen_ch_name.Length;
            var st = new MemoryStream();
            st.WriteByte((byte)(_listen_ch_name_size & 0xff));
            st.WriteByte((byte)(_listen_ch_name_size >> 8 & 0xff));
            st.WriteByte((byte)(_listen_ch_name_size >> 16 & 0xff));
            st.WriteByte((byte)(_listen_ch_name_size >> 24 & 0xff));
            st.Write(b_listen_ch_name, 0, _listen_ch_name_size);
            st.Write(data, 0, data.Length);
            st.Position = 0;

            send_data.Enqueue(Tuple.Create(ch_name, st));
        }

        private async void th_poll()
        {
            while (run_flag)
            {
                bool is_idle = true;
                if (send_data.TryDequeue(out Tuple<string, MemoryStream> data))
                {
                    while (true)
                    {
                        try
                        {
                            await database.ListLeftPushAsync(data.Item1, data.Item2.ToArray());
                            break;
                        }
                        catch (RedisTimeoutException ex)
                        {
                            log.log.err("ListLeftPushAsync error:{0}", ex);
                            Recover(ex);
                        }
                        catch (RedisConnectionException ex)
                        {
                            log.log.err("ListLeftPushAsync error:{0}", ex);
                            Recover(ex);
                        }
                    }
                    is_idle = false;
                }

                try
                {
                    lock (wait_listen_channel_names)
                    {
                        foreach(var name in wait_listen_channel_names)
                        {
                            listen_channel_names.Add(name);
                        }
                        wait_listen_channel_names.Clear();
                    }

                    byte[] pop_data = null;
                    foreach (var listen_channel_name in listen_channel_names)
                    {
                        pop_data = await database.ListRightPopAsync(listen_channel_name);
                        while (pop_data != null)
                        {
                            var _ch_name_size = (UInt32)pop_data[0] | ((UInt32)pop_data[1] << 8) | ((UInt32)pop_data[2] << 16) | ((UInt32)pop_data[3] << 24);
                            var _ch_name = System.Text.Encoding.UTF8.GetString(pop_data, 4, (int)_ch_name_size);
                            var _header_len = 4 + _ch_name_size;
                            var _msg_len = pop_data.Length - _header_len;
                            var _st = new MemoryStream();
                            _st.Write(pop_data, (int)_header_len, (int)_msg_len);
                            _st.Position = 0;

                            lock (channels)
                            {
                                if (channels.TryGetValue(_ch_name, out redischannel ch))
                                {
                                    ch._channel_onrecv.on_recv(_st.ToArray());
                                }
                                else
                                {
                                    ch = new redischannel(_ch_name, this);
                                    channels.Add(_ch_name, ch);
                                    ch._channel_onrecv.on_recv(_st.ToArray());
                                }
                            }
                            is_idle = false;

                            pop_data = await database.ListRightPopAsync(listen_channel_name);
                        }
                    }
                }
                catch (RedisTimeoutException ex)
                {
                    log.log.err("ListLeftPushAsync error:{0}", ex);
                    Recover(ex);
                }
                catch (RedisConnectionException ex)
                {
                    log.log.err("ListLeftPushAsync error:{0}", ex);
                    Recover(ex);
                }

                if (is_idle)
                {
                    await Task.Delay(5);
                }
            }
        }
    }
}
