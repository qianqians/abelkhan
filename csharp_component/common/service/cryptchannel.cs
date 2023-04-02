/*
 * cryptchannel
 * qianqians
 * 2020/6/4
 */
using System.Buffers;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace abelkhan
{
    public class cryptchannel : abelkhan.Ichannel
    {
        private ConnectionContext connection;
        private int need_flush = 0;
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

        public async void send(byte[] data)
        {
            lock (lockobj)
            {
                Interlocked.Exchange(ref need_flush, 1);
                connection.Transport.Output.WriteAsync(data);
            }
            var _need_flush = Interlocked.Exchange(ref need_flush, 0);
            if (_need_flush == 1)
            {
                await connection.Transport.Output.FlushAsync();
            }
        }
    }
}