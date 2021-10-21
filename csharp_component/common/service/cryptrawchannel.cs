﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

namespace abelkhan
{
    public class cryptrawchannel : abelkhan.Ichannel
    {
        public event Action<cryptrawchannel> onDisconnect;
        public event Action<cryptrawchannel> Disconnect;

        public channel_onrecv _channel_onrecv;

        public cryptrawchannel(Socket _s)
        {
            s = _s;
            _channel_onrecv = new channel_onrecv();
            _channel_onrecv.on_recv_data += (byte[] data) => {
                var len = data.Length;
                byte xor_key0 = (byte)(len & 0xff);
                byte xor_key1 = (byte)((len >> 8) & 0xff);
                byte xor_key2 = (byte)((len >> 16) & 0xff);
                byte xor_key3 = (byte)((len >> 24) & 0xff);

                for (var i = 0; i < data.Length; ++i)
                {
                    if ((i % 4) == 0)
                    {
                        data[i] ^= xor_key0;
                    }
                    else if ((i % 4) == 1)
                    {
                        data[i] ^= xor_key1;
                    }
                    else if ((i % 4) == 2)
                    {
                        data[i] ^= xor_key2;
                    }
                    else if ((i % 4) == 3)
                    {
                        data[i] ^= xor_key3;
                    }
                }
            };

            recvbuflength = 8 * 1024;
            recvbuf = new byte[recvbuflength];

            _send_state = send_state.idel;
            send_buff = new Queue();

            try
            {
                s.BeginReceive(recvbuf, 0, recvbuflength, 0, new AsyncCallback(this.onRead), this);
            }
            catch (System.Net.Sockets.SocketException)
            {
                onDisconnect?.Invoke(this);
            }
        }

        public ArrayList pop()
        {
            if (_channel_onrecv.que.Count > 0)
            {
                return _channel_onrecv.que.Dequeue();
            }
            return null;
        }

        public void disconnect()
        {
            s.Close();

            Disconnect?.Invoke(this);
        }

        private void onRead(IAsyncResult ar)
        {
            cryptrawchannel ch = ar.AsyncState as cryptrawchannel;

            try
            {
                int read = ch.s.EndReceive(ar);
                if (read > 0)
                {
                    MemoryStream st = new MemoryStream();
                    st.Write(recvbuf, 0, read);
                    st.Position = 0;
                    _channel_onrecv.on_recv(st.ToArray());

                    ch.s.BeginReceive(recvbuf, 0, recvbuflength, 0, new AsyncCallback(this.onRead), this);
                }
                else
                {
                    ch.s.Close();

                    onDisconnect?.Invoke(this);
                }
            }
            catch (System.ObjectDisposedException)
            {
                onDisconnect?.Invoke(this);

            }
            catch (System.Net.Sockets.SocketException)
            {
                onDisconnect?.Invoke(this);
            }
        }

        public void send(byte[] data)
        {
            byte xor_key0 = data[0];
            byte xor_key1 = data[1];
            byte xor_key2 = data[2];
            byte xor_key3 = data[3];
            for (var i = 4; i < data.Length; ++i)
            {
                if ((i % 4) == 0)
                {
                    data[i] ^= xor_key0;
                }
                else if ((i % 4) == 1)
                {
                    data[i] ^= xor_key1;
                }
                else if ((i % 4) == 2)
                {
                    data[i] ^= xor_key2;
                }
                else if ((i % 4) == 3)
                {
                    data[i] ^= xor_key3;
                }
            }

            senddata(data);
        }

        private void senddata(byte[] data)
        {
            try
            {
                lock (send_buff)
                {
                    if (_send_state == send_state.idel)
                    {
                        _send_state = send_state.busy;
                        tmp_send_buff = data;
                        send_len = 0;
                        s.BeginSend(data, 0, data.Length, SocketFlags.None,
                                    new AsyncCallback(this.send_callback), this);
                    }
                    else
                    {
                        send_buff.Enqueue(data);
                    }
                }
            }
            catch (System.ObjectDisposedException)
            {
                onDisconnect?.Invoke(this);

            }
            catch (System.Net.Sockets.SocketException)
            {
                onDisconnect?.Invoke(this);
            }
        }

        private void send_callback(IAsyncResult ar)
        {
            cryptrawchannel ch = ar.AsyncState as cryptrawchannel;

            try
            {
                int send = ch.s.EndSend(ar);
                lock (send_buff)
                {
                    send_len += send;
                    if (send_len < tmp_send_buff.Length)
                    {
                        s.BeginSend(tmp_send_buff, send_len, tmp_send_buff.Length - send_len,
                                    SocketFlags.None, new AsyncCallback(this.send_callback), this);
                    }
                    else if (send_len == tmp_send_buff.Length)
                    {
                        if (send_buff.Count <= 0)
                        {
                            _send_state = send_state.idel;
                        }
                        else
                        {
                            var data = (byte[])send_buff.Dequeue();
                            tmp_send_buff = data;
                            send_len = 0;
                            s.BeginSend(data, 0, data.Length, SocketFlags.None,
                                        new AsyncCallback(this.send_callback), this);
                        }
                    }
                }
            }
            catch (System.ObjectDisposedException)
            {
                onDisconnect?.Invoke(this);

            }
            catch (System.Net.Sockets.SocketException)
            {
                onDisconnect?.Invoke(this);
            }
        }

        public Socket s;

        private readonly byte[] recvbuf;
        private readonly int recvbuflength;

        private readonly Queue send_buff;
        enum send_state
        {
            idel = 0,
            busy = 1,
        }
        private send_state _send_state;
        private byte[] tmp_send_buff;
        private int send_len;
    }
}

