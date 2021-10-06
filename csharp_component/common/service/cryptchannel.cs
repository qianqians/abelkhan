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

        public channel_onrecv _channel_onrecv;

        public cryptchannel(IChannelHandlerContext _context)
        {
            context = _context;

            _channel_onrecv = new channel_onrecv();
            _channel_onrecv.on_recv_data += (byte[] data) => {
                var len = data.Length;
                byte xor_key0 = (byte)(len & 0xff);
                byte xor_key1 = (byte)((len >> 8) & 0xff);
                byte xor_key2 = (byte)((len >> 16) & 0xff);
                byte xor_key3 = (byte)((len >> 24) & 0xff);

                for (var i = 0; i < data.Length; ++i)
                {
                    if ((i % 4) == 0)
                    {
                        data[i] ^= xor_key0;
                    }
                    else if ((i % 4) == 1)
                    {
                        data[i] ^= xor_key1;
                    }
                    else if ((i % 4) == 2)
                    {
                        data[i] ^= xor_key2;
                    }
                    else if ((i % 4) == 3)
                    {
                        data[i] ^= xor_key3;
                    }
                }
            };
        }

        public ArrayList pop()
        {
            if (_channel_onrecv.que.Count > 0)
            {
                return _channel_onrecv.que.Dequeue();
            }
            return null;
        }

        public void disconnect()
        {
            context.CloseAsync();
        }

        public void send(byte[] data)
        {
            byte xor_key0 = data[0];
            byte xor_key1 = data[1];
            byte xor_key2 = data[2];
            byte xor_key3 = data[3];
            for (var i = 4; i < data.Length; ++i)
            {
                if ((i % 4) == 0)
                {
                    data[i] ^= xor_key0;
                }
                else if ((i % 4) == 1)
                {
                    data[i] ^= xor_key1;
                }
                else if ((i % 4) == 2)
                {
                    data[i] ^= xor_key2;
                }
                else if ((i % 4) == 3)
                {
                    data[i] ^= xor_key3;
                }
            }

            var len = data.Length;
            var initialMessage = Unpooled.Buffer((int)len);
            initialMessage.WriteBytes(data);

            context.WriteAndFlushAsync(initialMessage);
        }
    }
}