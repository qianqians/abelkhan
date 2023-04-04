/*
 * cryptchannel
 * qianqians
 * 2020/6/4
 */
using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace abelkhan
{
    public class cryptchannel : abelkhan.Ichannel
    {
        private readonly Socket s;
        private readonly object lockobj;

        public channel_onrecv _channel_onrecv;

        public cryptchannel(Socket socket)
        {
            s = socket;
            lockobj = new object();

            _channel_onrecv = new channel_onrecv(this);
            _channel_onrecv.on_recv_data += crypt.crypt_func;
        }

        public void disconnect()
        {
            s.Close();
        }

        public bool is_xor_key_crypt()
        {
            return true;
        }

        public void normal_send_crypt(byte[] data)
        {
            crypt.crypt_func_send(data);
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