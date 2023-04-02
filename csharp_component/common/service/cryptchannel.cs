/*
 * cryptchannel
 * qianqians
 * 2020/6/4
 */
using Microsoft.AspNetCore.Connections;
using System.Buffers;

namespace abelkhan
{
    public class cryptchannel : abelkhan.Ichannel
    {
        private ConnectionContext connection;
        private object lockobj;

        public channel_onrecv _channel_onrecv;

        public cryptchannel(ConnectionContext _connection)
        {
            connection = _connection;
            lockobj = new object();

            _channel_onrecv = new channel_onrecv(this);
            _channel_onrecv.on_recv_data += crypt.crypt_func;
        }

        public void disconnect()
        {
            connection.Abort();
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
                connection.Transport.Output.Write(data);
            }
        }
    }
}