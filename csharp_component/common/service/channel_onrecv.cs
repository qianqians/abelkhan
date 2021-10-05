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
        private byte[] tmpbuf = null;
        private int tmpbufoffset = 0;

        private Ichannel _ch;

        private MessagePackSerializer<ArrayList> serializer = MessagePackSerializer.Get<ArrayList>();

        public channel_onrecv(Ichannel ch)
        {
            _ch = ch;
        }

        public event Action<byte[]> on_recv_data;
        public void on_recv(byte[] recv_data)
        {
            MemoryStream st = new MemoryStream();
            if (tmpbufoffset > 0)
            {
                st.Write(tmpbuf, 0, tmpbufoffset);
            }
            st.Write(recv_data, 0, recv_data.Length);
            st.Position = 0;
            byte[] data = st.ToArray();
            int data_len = tmpbufoffset + recv_data.Length;

            tmpbuf = null;
            tmpbufoffset = 0;

            int offset = 0;
            while (true)
            {
                int over_len = data_len - offset;
                if (over_len < 4)
                {
                    break;
                }

                int len = data[offset];
                len |= data[offset + 1] << 8;
                len |= data[offset + 2] << 16;
                len |= data[offset + 3] << 24;

                if (over_len < len + 4)
                {
                    break;
                }
                offset += 4;

                MemoryStream _tmp = new MemoryStream();
                _tmp.Write(data, offset, len);
                _tmp.Position = 0;
                byte[] _tmp_data = _tmp.ToArray();
                if (on_recv_data != null)
                {
                    on_recv_data(_tmp_data);
                }
                _tmp = new MemoryStream();
                _tmp.Write(_tmp_data, 0, _tmp_data.Length);
                _tmp.Position = 0;
                var _event = serializer.Unpack(_tmp);
                modulemng_handle._modulemng.process_event(_ch, _event);

                offset += len;
            }

            int overplus_len = data_len - offset;
            st = new MemoryStream();
            st.Write(data, offset, overplus_len);
            st.Position = 0;
            tmpbuf = st.ToArray();
            tmpbufoffset = overplus_len;
        }
    }
}