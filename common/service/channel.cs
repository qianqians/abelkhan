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
			tmpbuflenght = 0;
			tmpbufoffset = 0;

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
					if (tmpbufoffset == 0)
					{
						int offset = 0;
						do
						{
                            if (read > 4)
                            {
                                Int32 len = ((Int32)recvbuf[offset + 0]) | ((Int32)recvbuf[offset + 1]) << 8 | ((Int32)recvbuf[offset + 2]) << 16 | ((Int32)recvbuf[offset + 3]) << 24;

                                if (len <= (read - 4))
                                {
                                    read -= len + 4;
                                    offset += 4;

                                    MemoryStream _tmp = new MemoryStream();

                                    _tmp.Write(recvbuf, offset, len);
                                    offset += len;

                                    _tmp.Position = 0;

                                    var json = System.Text.Encoding.UTF8.GetString(_tmp.ToArray());
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
                                else
                                {
                                    if (tmpbuflenght == 0)
                                    {
                                        tmpbuflenght = recvbuflenght * 2;
                                        tmpbuf = new byte[tmpbuflenght];
                                    }

                                    while ((tmpbuflenght - tmpbufoffset) < read)
                                    {
                                        byte[] newtmpbuf = new byte[2 * tmpbuflenght];
                                        tmpbuf.CopyTo(newtmpbuf, 0);
                                        tmpbuf = newtmpbuf;
                                    }

                                    MemoryStream _tmp = new MemoryStream();
                                    _tmp.Write(recvbuf, offset, read);

                                    _tmp.ToArray().CopyTo(tmpbuf, tmpbufoffset);
                                    tmpbufoffset = read;

                                    break;
                                }
                            }
                            else
                            {
                                if (read > 0)
                                {
                                    if (tmpbuflenght == 0)
                                    {
                                        tmpbuflenght = recvbuflenght * 2;
                                        tmpbuf = new byte[tmpbuflenght];
                                    }

                                    while ((tmpbuflenght - tmpbufoffset) < read)
                                    {
                                        byte[] newtmpbuf = new byte[2 * tmpbuflenght];
                                        tmpbuf.CopyTo(newtmpbuf, 0);
                                        tmpbuf = newtmpbuf;
                                    }

                                    MemoryStream _tmp = new MemoryStream();
                                    _tmp.Write(recvbuf, offset, read);

                                    _tmp.ToArray().CopyTo(tmpbuf, tmpbufoffset);
                                    tmpbufoffset = read;
                                }
                                break;
                            }

						} while (true);
					}
					else if (tmpbufoffset > 0)
					{
						while ((tmpbuflenght - tmpbufoffset) < read)
						{
							byte[] newtmpbuf = new byte[2 * tmpbuflenght];
							tmpbuf.CopyTo(newtmpbuf, 0);
							tmpbuf = newtmpbuf;
						}

						MemoryStream _tmp_ = new MemoryStream();
						_tmp_.Write(recvbuf, 0, read);

						_tmp_.ToArray().CopyTo(tmpbuf, tmpbufoffset);
						tmpbufoffset += (Int32)_tmp_.Length;

						int offset = 0;
						do
						{
                            if (tmpbufoffset > 4)
                            {
                                Int32 len = ((Int32)tmpbuf[offset + 0]) | ((Int32)tmpbuf[offset + 1]) << 8 | ((Int32)tmpbuf[offset + 2]) << 16 | ((Int32)tmpbuf[offset + 3]) << 24;

                                if (len <= (tmpbufoffset - 4))
                                {
                                    tmpbufoffset -= len + 4;
                                    offset += 4;

                                    MemoryStream _tmp = new MemoryStream();

                                    _tmp.Write(tmpbuf, offset, len);
                                    offset += len;

                                    _tmp.Position = 0;

                                    var json = System.Text.Encoding.UTF8.GetString(_tmp.ToArray());
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
                                else
                                {
                                    MemoryStream _tmp = new MemoryStream();
                                    _tmp.Write(tmpbuf, offset, tmpbufoffset);

                                    _tmp.ToArray().CopyTo(tmpbuf, 0);

                                    break;
                                }
                            }
                            else
                            {
                                if (tmpbufoffset > 0)
                                {
                                    MemoryStream _tmp = new MemoryStream();
                                    _tmp.Write(tmpbuf, offset, tmpbufoffset);

                                    _tmp.ToArray().CopyTo(tmpbuf, 0);
                                }
                                break;
                            }

						} while (true);
					}

					ch.s.BeginReceive(recvbuf, 0, recvbuflenght, 0, new AsyncCallback(this.onRead), this);
				}
				else
                {
                    ch.s.Close();
					onDisconnect(ch);
				}
			}
            catch (System.ObjectDisposedException )
            {
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "socket is release");
            }
            catch (System.Net.Sockets.SocketException )
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

            Monitor.Enter(que);
			if (que.Count > 0)
			{
				_array = (ArrayList)que.Dequeue();
			}
            Monitor.Exit(que);

            return _array;
		}

		public void senddata(byte[] data)
		{
			try
			{
				int offset = s.Send(data);
				while (offset < data.Length)
				{
					MemoryStream st = new MemoryStream();
					st.Write(data, offset, data.Length - offset);
					data = st.ToArray();
					offset = s.Send(data);
				}
			}
			catch (System.Net.Sockets.SocketException)
			{
				s.Close();
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
		private Int32 tmpbuflenght;
		private Int32 tmpbufoffset;

		private Queue que;
	}
}

