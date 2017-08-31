using System;
using System.IO;
using System.Collections;
using System.Net.Sockets;
using System.Threading;

namespace service
{
	public class channel : juggle.Ichannel
	{
		public delegate void DisconnectHandle(juggle.Ichannel ch);
		public event DisconnectHandle onDisconnect;

		public channel(Socket _s)
		{
			s = _s;

			que = new Queue();

            recvbuflenght = 16 * 1024;
            recvbuf = new byte[recvbuflenght];
            
            tmpbuf = null;
            tmpbufoffset = 0;

            _send_state = send_state.idel;
            send_buff = new Queue();

            s.BeginReceive(recvbuf, 0, recvbuflenght, 0, new AsyncCallback(this.onRead), this);
		}

        public void disconnect()
        {
            s.Close();
            onDisconnect(this);
        }

		private void onRead(IAsyncResult ar)
        {
            channel ch = ar.AsyncState as channel;

            try
            {
                int read = ch.s.EndReceive(ar);

				if (read > 0)
                {
                    log.log.trace(new System.Diagnostics.StackFrame(), timerservice.Tick, "recv data len:{0}", read);

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
                    while(true)
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
                        _tmp.Position = 0;
                        var json = System.Text.Encoding.UTF8.GetString(_tmp.ToArray());
                        log.log.trace(new System.Diagnostics.StackFrame(true), timerservice.Tick, "len:{0} msg:{1}", len, json);
                        try
                        {
                            ArrayList unpackedObject = (ArrayList)Json.Jsonparser.unpack(json);

                            lock (que)
                            {
                                que.Enqueue(unpackedObject);
                            }
                        }
                        catch (Exception e)
                        {
                            log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "msg:{0}, System.Exception:{1}", json, e);
                        }
                        offset += len;
                    }

                    int overplus_len = data_len - offset;
                    st = new MemoryStream();
                    st.Write(data, offset, overplus_len);
                    st.Position = 0;
                    tmpbuf = st.ToArray();
                    tmpbufoffset = overplus_len;
				}

                ch.s.BeginReceive(recvbuf, 0, recvbuflenght, 0, new AsyncCallback(this.onRead), this);
            }
            catch (System.ObjectDisposedException )
            {
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "socket is release");
            }
            catch (System.Net.Sockets.SocketException)
            {
                onDisconnect(ch);
            }
			catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "System.Exception:{0}", e);
                
				onDisconnect(ch);
			}
        }

		public ArrayList pop()
		{
			ArrayList _array = null;

            lock (que)
            {
                if (que.Count > 0)
                {
                    _array = (ArrayList)que.Dequeue();
                }
            }

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
                    
                var st = new MemoryStream();
                st.WriteByte((byte)(_tmplenght & 0xff));
                st.WriteByte((byte)((_tmplenght >> 8) & 0xff));
                st.WriteByte((byte)((_tmplenght >> 16) & 0xff));
                st.WriteByte((byte)((_tmplenght >> 24) & 0xff));
                st.Write(_tmpdata, 0, _tmpdata.Length);
                st.WriteByte(0);
                st.WriteByte(0);
                st.WriteByte(0);
                st.WriteByte(0);
                st.Position = 0;

                senddata(st.ToArray());
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
                log.log.trace(new System.Diagnostics.StackFrame(), timerservice.Tick, "data.Length:{0}", data.Length);

                lock(send_buff)
                {
                    if (_send_state == send_state.idel)
                    {
                        _send_state = send_state.busy;
                        s.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(this.send_callback), this);
                    }
                    else
                    {
                        send_buff.Enqueue(data);
                    }
                }
            }
			catch (System.Net.Sockets.SocketException e)
			{
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "System.Net.Sockets.SocketException:{0}", e);
                
				onDisconnect(this);
			}
			catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "System.Exception:{0}", e);
                
                s.Close();
				onDisconnect(this);
			}
		}

        private void send_callback(IAsyncResult ar)
        {
            channel ch = ar.AsyncState as channel;

            try
            {
                int send = ch.s.EndSend(ar);

                lock (send_buff)
                {
                    if (send_buff.Count <= 0)
                    {
                        _send_state = send_state.idel;
                    }
                    else
                    {
                        var data = (byte[])send_buff.Dequeue();
                        s.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(this.send_callback), this);
                    }
                }
            }
            catch (System.Net.Sockets.SocketException e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "System.Net.Sockets.SocketException:{0}", e);

                onDisconnect(this);
            }
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "System.Exception:{0}", e);

                s.Close();
                onDisconnect(this);
            }
        }

		private Socket s;

        private byte[] recvbuf;
        private Int32 recvbuflenght;

		private byte[] tmpbuf;
		private Int32 tmpbufoffset;
        
        private Queue send_buff;
        enum send_state
        {
            idel = 0,
            busy = 1,
        }
        send_state _send_state;

		private Queue que;
	}
}

