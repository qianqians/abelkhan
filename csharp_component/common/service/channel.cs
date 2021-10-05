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

        public channel_onrecv _channel_onrecv;

        public channel(IChannelHandlerContext _context)
        {
            context = _context;
            _channel_onrecv = new channel_onrecv(this);
        }

        public void disconnect()
        {
            context.CloseAsync();
        }

        public void send(byte[] data)
        {
            var len = data.Length;
            var initialMessage = Unpooled.Buffer((int)len);
            initialMessage.WriteBytes(data);

            context.WriteAndFlushAsync(initialMessage);
        }
    }
}