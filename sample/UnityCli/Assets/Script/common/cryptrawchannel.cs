using System;
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
            _channel_onrecv = new channel_onrecv(this);
            _channel_onrecv.on_recv_data += crypt.crypt_func;

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

        public void disconnect()
        {
            s.Close();

            Disconnect?.Invoke(this);
        }

        public bool is_xor_key_crypt()
        {
            return true;
        }

        public void normal_send_crypt(byte[] data)
        {
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
            crypt.crypt_func_send(data);
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

