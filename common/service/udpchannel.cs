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

            recvbuflenght = 16 * 1024;
            tmpbuf = null;
			tmpbuflenght = 0;
			tmpbufoffset = 0;
		}

        public void disconnect()
        {
            s.Close();
        }

		public void recv(byte[] recvbuf, int read)
        {
            try
            {
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
                                    log.log.trace(new System.Diagnostics.StackFrame(true), timerservice.Tick, "{0}", json);
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
                                    log.log.trace(new System.Diagnostics.StackFrame(true), timerservice.Tick, "{0}", json);
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

		public void senddata(byte[] data)
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

		private Socket s;

        private IPEndPoint remote_ep;

        private byte[] tmpbuf;
        private Int32 recvbuflenght;
        private Int32 tmpbuflenght;
		private Int32 tmpbufoffset;

		private Queue que;
	}
}

