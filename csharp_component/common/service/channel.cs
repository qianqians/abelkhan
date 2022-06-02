/*
 * channel
 * 2020/6/2
 * qianqians
 */
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DotNetty.Transport.Channels;
using DotNetty.Buffers;

namespace abelkhan
{
    public class channel : abelkhan.Ichannel
    {
        private IChannelHandlerContext context;
        private object lockobj;

        public channel_onrecv _channel_onrecv;

        public channel(IChannelHandlerContext _context)
        {
            context = _context;
            lockobj = new object();
            _channel_onrecv = new channel_onrecv(this);
        }

        public void disconnect()
        {
            context.CloseAsync();
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