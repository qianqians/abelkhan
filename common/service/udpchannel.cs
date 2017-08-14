using System;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace service
{
	public class udpchannel : juggle.Ichannel
	{
		public delegate void DisconnectHandle(juggle.Ichannel ch);
		public event DisconnectHandle onDisconnect;

		public udpchannel(Socket _s, IPEndPoint _remote_ep)
		{
			s = _s;
            remote_ep = new IPEndPoint(_remote_ep.Address, _remote_ep.Port);

            que = new Queue();
            
            tmpbuf = null;
            tmpbufoffset = 0;
        }

        public void disconnect()
        {
            onDisconnect(this);
        }

		public void recv(byte[] recvbuf, int read)
        {
            try
            {
                if (read > 0)
                {
                    log.log.trace(new System.Diagnostics.StackFrame(), timerservice.Tick, "recv data len:{0}", read);

                    if (read > 0)
                    {
                        MemoryStream st = new MemoryStream();
                        if (tmpbufoffset > 0)
                        {
                            st.Write(tmpbuf, 0, tmpbufoffset);
                        }
                        st.Write(recvbuf, 0, read);
                        st.Position = 0;
                        byte[] data = st.ToArray();
                        int data_len = tmpbufoffset + read;

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

                            Int32 len = ((Int32)data[offset + 0]) | ((Int32)data[offset + 1]) << 8 | ((Int32)data[offset + 2]) << 16 | ((Int32)data[offset + 3]) << 24;
                            if (over_len < len + 4)
                            {
                                break;
                            }

                            offset += 4;
                            MemoryStream _tmp = new MemoryStream();
                            _tmp.Write(data, offset, len);
                            offset += len;
                            _tmp.Position = 0;
                            var json = System.Text.Encoding.UTF8.GetString(_tmp.ToArray());
                            log.log.trace(new System.Diagnostics.StackFrame(true), timerservice.Tick, "msg:{0}", json);
                            try
                            {
                                ArrayList unpackedObject = (ArrayList)Json.Jsonparser.unpack(json);

                                Monitor.Enter(que);
                                que.Enqueue(unpackedObject);
                                Monitor.Exit(que);
                            }
                            catch (Exception e)
                            {
                                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "msg:{0}, System.Exception:{1}", json, e);
                            }
                        }

                        int overplus_len = data_len - offset;
                        st = new MemoryStream();
                        st.Write(data, offset, overplus_len);
                        st.Position = 0;
                        tmpbuf = st.ToArray();
                        tmpbufoffset = overplus_len;
                    }
                }
                else
                {
                    onDisconnect(this);
                }
            }
            catch (System.Net.Sockets.SocketException )
            {
                onDisconnect(this);
            }
			catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "System.Exception:{0}", e);

                onDisconnect(this);
            }
		}

		public ArrayList pop()
		{
			ArrayList _array = null;

            Monitor.Enter(que);
			if (que.Count > 0)
			{
				_array = (ArrayList)que.Dequeue();
			}
            Monitor.Exit(que);

            return _array;
		}

        public void push(ArrayList ev)
        {
            try
            {
                var _tmp = Json.Jsonparser.pack(ev);

                log.log.trace(new System.Diagnostics.StackFrame(), timerservice.Tick, "send:{0}", _tmp);

                var _tmpdata = System.Text.Encoding.UTF8.GetBytes(_tmp);
                var _tmplenght = _tmpdata.Length + 4;

                byte[] buf = new byte[4 + _tmplenght];
                buf[0] = (byte)(_tmplenght & 0xff);
                buf[1] = (byte)((_tmplenght >> 8) & 0xff);
                buf[2] = (byte)((_tmplenght >> 16) & 0xff);
                buf[3] = (byte)((_tmplenght >> 24) & 0xff);
                _tmpdata.CopyTo(buf, 4);

                senddata(buf);
            }
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "System.Exception:{0}", e);
            }
        }

        private void senddata(byte[] data)
		{
			try
			{
				int offset = s.SendTo(data, remote_ep);
				while (offset < data.Length)
				{
					MemoryStream st = new MemoryStream();
					st.Write(data, offset, data.Length - offset);
					data = st.ToArray();
					offset = s.SendTo(data, remote_ep);
                }
			}
			catch (System.Net.Sockets.SocketException)
			{
				onDisconnect(this);
			}
			catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "System.Exception:{0}", e);
                
				onDisconnect(this);
			}
		}

        public Socket s;

        public IPEndPoint remote_ep;

        private byte[] tmpbuf;
		private Int32 tmpbufoffset;

		private Queue que;
	}
}

