/*
 * cryptchannel
 * qianqians
 * 2020/6/4
 */
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DotNetty.Transport.Channels;
using DotNetty.Buffers;

namespace abelkhan
{
    public class cryptchannel : abelkhan.Ichannel
    {
        private IChannelHandlerContext context;
        private object lockobj;

        public channel_onrecv _channel_onrecv;

        public cryptchannel(IChannelHandlerContext _context)
        {
            context = _context;
            lockobj = new object();

            _channel_onrecv = new channel_onrecv(this);
            _channel_onrecv.on_recv_data += crypt.crypt_func;
        }

        public void disconnect()
        {
            context.CloseAsync();
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
            var len = data.Length;
            var initialMessage = Unpooled.Buffer((int)len);
            initialMessage.WriteBytes(data);

            lock (lockobj)
            {
                context.WriteAndFlushAsync(initialMessage);
            }
        }
    }
}