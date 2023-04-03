/*
 * channel
 * 2020/6/2
 * qianqians
 */
using Microsoft.AspNetCore.Connections;
using System.Buffers;
using System.Threading;

namespace abelkhan
{
    public class channel : abelkhan.Ichannel
    {
        private ConnectionContext connection;
        private int need_flush = 0;
        private object lockobj;

        public channel_onrecv _channel_onrecv;

        public channel(ConnectionContext _connection)
        {
            connection = _connection;
            lockobj = new object();
            _channel_onrecv = new channel_onrecv(this);
        }

        public void disconnect()
        {
            connection.Abort();
        }

        public bool is_xor_key_crypt()
        {
            return false;
        }

        public void normal_send_crypt(byte[] data)
        {
        }

        public async void send(byte[] data)
        {
            lock (lockobj)
            {
                connection.Transport.Output.WriteAsync(data);
                Interlocked.Exchange(ref need_flush, 1);
            }
            var _need_flush = Interlocked.Exchange(ref need_flush, 0);
            if (_need_flush == 1)
            {
                await connection.Transport.Output.FlushAsync();
            }
        }
    }
}