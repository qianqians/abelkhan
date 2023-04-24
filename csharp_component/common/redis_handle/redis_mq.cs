﻿using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Service;

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
        private readonly Thread th_send;
        private readonly Thread th_recv;
        private bool run_flag = true;
        private long tick_time;

        private readonly List<string> listen_channel_names;
        private readonly List<string> wait_listen_channel_names;

        private readonly ConcurrentDictionary<string, Redischannel> channels;

        private readonly Dictionary<string, Queue<RedisValue>> wait_send_data;
        private readonly Dictionary<string, Queue<RedisValue>> send_data;

        public RedisMQ(Timerservice timer, string connUrl, string _listen_channel_name, long _tick_time = 33)
        {
            _timer = timer;
            main_channel_name = _listen_channel_name;
            tick_time = _tick_time;

            listen_channel_names = new() { _listen_channel_name };
            wait_listen_channel_names = new();
            channels = new ConcurrentDictionary<string, Redischannel>();

            _connHelper = new RedisConnectionHelper(connUrl, "RedisForMQ");
            _connHelper.ConnectOnStartup(ref connectionMultiplexer, ref database);

            wait_send_data = new();
            send_data = new();

            ThreadStart th_send_poll_start = new ThreadStart(th_send_poll);
            th_send = new Thread(th_send_poll_start);
            th_send.Start();
            ThreadStart th_recv_poll_start = new ThreadStart(th_recv_poll);
            th_recv = new Thread(th_recv_poll_start);
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

        public void close()
        {
            run_flag = false;
            th_send.Join();
            th_recv.Join();
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

            if (!send_data.TryGetValue(ch_name, out Queue<RedisValue> send_queue))
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

        private async Task<long> sendmsg_mq()
        {
            var tick_begin = _timer.refresh();
            if (wait_send_data.Count > 0)
            {
                lock (wait_send_data)
                {
                    foreach (var (ch_name, send_queue) in wait_send_data)
                    {
                        send_data.Add(ch_name, send_queue);
                    }
                    wait_send_data.Clear();
                }
            }

            foreach (var (ch_name, send_queue) in send_data)
            {
                RedisValue[] push_data_array = null;
                lock (send_queue)
                {
                    if (send_queue.Count > 0)
                    {
                        push_data_array = send_queue.ToArray();
                        send_queue.Clear();
                    }
                }

                while (push_data_array != null)
                {
                    try
                    {
                        await database.ListLeftPushAsync(ch_name, push_data_array);
                        break;
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
                        Log.Log.err("sendmsg_mq error:{0}", ex);
                    }
                }
            }

            return _timer.refresh() - tick_begin;
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
                Log.Log.err("list_channel_name error:{0}", ex);
            }
        }

        private async Task recvmsg_mq_ch(string ch_name)
        {
            byte[] pop_data = await database.ListRightPopAsync(ch_name);
            while (pop_data != null)
            {
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

                pop_data = await database.ListRightPopAsync(ch_name);
            }
        }

        private async Task<long> recvmsg_mq()
        {
            var tick_begin = _timer.refresh();

            list_channel_name();
            foreach (var listen_channel_name in listen_channel_names)
            {
                try
                {
                    await recvmsg_mq_ch(listen_channel_name);
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
            }

            return _timer.refresh() - tick_begin;
        }

        private async void th_send_poll()
        {
            while (run_flag)
            {
                var tick = await sendmsg_mq();
                if (tick < tick_time)
                {
                    Thread.Sleep((int)(tick_time - tick));
                }
            }
        }

        private async void th_recv_poll()
        {
            while (run_flag)
            {
                var tick = await recvmsg_mq();
                if (tick < tick_time)
                {
                    Thread.Sleep((int)(tick_time - tick));
                }
            }
        }
    }
}
