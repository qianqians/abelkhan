using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.IO;

namespace abelkhan
{
    public class redischannel : Ichannel
    {
        private readonly string _channelName;
        private readonly redis_mq _redis_mq_handle;

        public readonly channel_onrecv _channel_onrecv;

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
        private readonly RedisConnectionHelper _connHelper;
        private ConnectionMultiplexer connectionMultiplexer;
        private IDatabase database;

        private readonly string main_channel_name;
        private readonly Task th_send;
        private readonly Task th_recv;
        private bool run_flag = true;

        private readonly List<string> listen_channel_names;
        private readonly List<string> wait_listen_channel_names;

        private readonly ConcurrentDictionary<string, redischannel> channels;
        private readonly ConcurrentQueue<Tuple<string, MemoryStream> > send_data;

        public redis_mq(string connUrl, string _listen_channel_name)
        {
            main_channel_name = _listen_channel_name;
            listen_channel_names = new() { _listen_channel_name };
            wait_listen_channel_names = new ();
            channels = new ConcurrentDictionary<string, redischannel>();

            _connHelper = new RedisConnectionHelper(connUrl, "RedisForMQ");
            _connHelper.ConnectOnStartup(ref connectionMultiplexer, ref database);

            send_data = new ConcurrentQueue<Tuple<string, MemoryStream> >();

            th_send = new Task(th_send_poll, TaskCreationOptions.LongRunning);
            th_send.Start();
            th_recv = new Task(th_recv_poll, TaskCreationOptions.LongRunning);
            th_recv.Start();
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
            await th_send;
            await th_recv;
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
                channels.TryAdd(ch_name, ch);
                return ch;
            }
        }

        public void sendmsg(string ch_name, byte[] data)
        {
            log.log.trace("send msg to:{0}", ch_name);

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

            send_data.Enqueue(Tuple.Create(ch_name, st));
        }

        private async Task<bool> sendmsg_mq()
        {
            bool is_busy = false;
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
                    catch (System.Exception ex)
                    {
                        log.log.err("sendmsg_mq error:{0}", ex);
                    }
                }
                is_busy = true;
            }

            return is_busy;
        }

        private void list_channel_name()
        {
            try
            {
                if (wait_listen_channel_names.Count > 0)
                {
                    lock (wait_listen_channel_names)
                    {
                        foreach (var name in wait_listen_channel_names)
                        {
                            listen_channel_names.Add(name);
                        }
                        wait_listen_channel_names.Clear();
                    }
                }
            }
            catch (System.Exception ex)
            {
                log.log.err("list_channel_name error:{0}", ex);
            }
        }

        private async Task<bool> recvmsg_mq()
        {
            bool is_busy = false;
            try
            {
                foreach (var listen_channel_name in listen_channel_names)
                {
                    var pop_data = (byte[])(await database.ListRightPopAsync(listen_channel_name));
                    while (pop_data != null)
                    {
                        is_busy = true;

                        var _ch_name_size = (UInt32)pop_data[0] | ((UInt32)pop_data[1] << 8) | ((UInt32)pop_data[2] << 16) | ((UInt32)pop_data[3] << 24);
                        var _ch_name = System.Text.Encoding.UTF8.GetString(pop_data, 4, (int)_ch_name_size);
                        var _header_len = 4 + _ch_name_size;
                        var _msg_len = pop_data.Length - _header_len;

                        using var _st = MemoryStreamPool.mstMgr.GetStream();
                        _st.Write(pop_data, (int)_header_len, (int)_msg_len);
                        _st.Position = 0;

                        if (channels.TryGetValue(_ch_name, out redischannel ch))
                        {
                            ch._channel_onrecv.on_recv(_st.ToArray());
                        }
                        else
                        {
                            ch = new redischannel(_ch_name, this);
                            channels.TryAdd(_ch_name, ch);
                            ch._channel_onrecv.on_recv(_st.ToArray());
                        }

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
            catch (System.Exception ex)
            {
                log.log.err("recvmsg_mq error:{0}", ex);
            }

            return is_busy;
        }

        private async void th_send_poll()
        {
            var idle_wait = 2;
            while (run_flag)
            {
                var is_send_busy = await sendmsg_mq();
                if (!is_send_busy)
                {
                    await Task.Delay(idle_wait);
                    if (idle_wait > 32)
                    {
                        idle_wait = 2;
                    }
                    else
                    {
                        idle_wait *= 2;
                    }
                }
                else
                {
                    idle_wait = 2;
                }
            }
        }

        private async void th_recv_poll()
        {
            var idle_wait = 2;
            while (run_flag)
            {
                list_channel_name();
                var is_recv_busy = await recvmsg_mq();
                if (!is_recv_busy)
                {
                    await Task.Delay(idle_wait);
                    if (idle_wait > 32)
                    {
                        idle_wait = 2;
                    }
                    else
                    {
                        idle_wait *= 2;
                    }
                }
                else
                {
                    idle_wait = 2;
                }
            }
        }
    }
}
