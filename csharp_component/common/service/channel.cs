/*
 * channel
 * 2020/6/2
 * qianqians
 */
using Microsoft.AspNetCore.Connections;
using System.Buffers;
using System.Net.Sockets;
using System.Threading;

namespace abelkhan
{
    public class Channel : abelkhan.Ichannel
    {
        private readonly Socket s;
        private readonly object lockobj;

        public channel_onrecv _channel_onrecv;

        public Channel(Socket socket)
        {
            s = socket;
            lockobj = new object();
            _channel_onrecv = new channel_onrecv(this);
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
            lock (lockobj)
            {
                s.Send(data);
            }
        }
    }
}