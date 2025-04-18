/*
 * channel
 * 2020/6/2
 * qianqians
 */
using Microsoft.AspNetCore.Connections;
using System.Buffers;
using System.Net.Sockets;
using System.Threading;

namespace Abelkhan
{
    public class Channel : Abelkhan.Ichannel
    {
        private readonly Socket s;
        private readonly object lockobj;

        public ChannelOnRecv _channel_onrecv;

        public Channel(Socket socket)
        {
            s = socket;
            lockobj = new object();
            _channel_onrecv = new ChannelOnRecv(this);
        }

        public void disconnect()
        {
            s.Close();
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
            var send_len = 0;
            lock (lockobj)
            {
                while (send_len < data.Length)
                {
                    send_len += s.Send(data, send_len, data.Length - send_len, SocketFlags.None);
                }
            }
        }
    }
}