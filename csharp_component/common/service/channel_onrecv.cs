/*
 * channel_onrecv
 * 2020/6/2
 * qianqians
 */
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using MsgPack.Serialization;

namespace abelkhan
{
    public class channel_onrecv
    {
        private MemoryStream recv_buf = new MemoryStream();

        public readonly Queue<ArrayList> que = new Queue<ArrayList>();

        private MessagePackSerializer<ArrayList> serializer = MessagePackSerializer.Get<ArrayList>();

        public channel_onrecv()
        {
        }

        public event Action<byte[], int, int> on_recv_data;
        public void on_recv(byte[] recv_data)
        {
            recv_buf.Write(recv_data, 0, recv_data.Length);
            var buffer_len = recv_buf.Length;

            int offset = 0;
            var under_buf = recv_buf.GetBuffer();
            while (true)
            {
                var unread = buffer_len - offset;
                if (unread < 4)
                {
                    break;
                }

                int len = under_buf[offset];
                len |= under_buf[offset + 1] << 8;
                len |= under_buf[offset + 2] << 16;
                len |= under_buf[offset + 3] << 24;

                if (unread < len + 4)
                {
                    break;
                }

                offset += 4;
                on_recv_data?.Invoke(under_buf, offset, offset + len);

                using (var _tmp = new MemoryStream())
                {
                    _tmp.Write(under_buf, offset, len);
                    _tmp.Position = 0;
                    var _event = serializer.Unpack(_tmp);
                    lock (que)
                    {
                        que.Enqueue(_event);
                    }
                }

                offset += len;
            }

            if (offset > 4)
            {
                Buffer.BlockCopy(under_buf, offset, under_buf, 0, (int)buffer_len - offset);
                var pos = buffer_len - offset;
                recv_buf.Seek(pos, SeekOrigin.Begin);
                recv_buf.SetLength(pos);
            }
        }
    }
}