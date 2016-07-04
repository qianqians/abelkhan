using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using MsgPack;
using MsgPack.Serialization;

namespace service
{
	public class channel : juggle.Ichannel
	{
		public delegate void DisconnectHandle(channel ch);
		public event DisconnectHandle onDisconnect;

		public channel(Socket _s)
		{
			s = _s;

			que = new Queue();

            recvbuflenght = 1024 * 1024;
            recvbuf = new byte[recvbuflenght];
			tmpbuf = null;
			tmpbuflenght = 0;
			tmpbufoffset = 0;

			s.BeginReceive(recvbuf, 0, recvbuflenght, 0, new AsyncCallback(this.onRead), this);
		}

		private void onRead(IAsyncResult ar)
		{
			Console.WriteLine("onRead");
			
			channel ch = ar.AsyncState as channel;

			int read = ch.s.EndReceive(ar);
			Console.WriteLine("read {0:D}", read);

			if (read > 0)
			{
				if (tmpbufoffset == 0)
				{
					int offset = 0;
					do
					{
						Int32 len = BitConverter.ToInt32(recvbuf, offset);
						Console.WriteLine("len {0:D}", len);

						if (len <= (read - 4))
						{
							read -= len + 4;
							offset += 4;

							MemoryStream _tmp = new MemoryStream();

							_tmp.Write(recvbuf, offset, len);
							offset += len;

							_tmp.Position = 0;

							var serializer = SerializationContext.Default.GetSerializer<ArrayList>();
							ArrayList unpackedObject = serializer.Unpack(_tmp);

							lock (que)
							{
								Console.WriteLine("Enqueue {0:D}", read);
								que.Enqueue(unpackedObject);
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
						Int32 len = BitConverter.ToInt32(tmpbuf, offset);

						if (len <= (tmpbufoffset - 4))
						{
							tmpbufoffset -= len + 4;
							offset += 4;

							MemoryStream _tmp = new MemoryStream();

							_tmp.Write(tmpbuf, offset, len);
							offset += len;

							_tmp.Position = 0;

							var serializer = SerializationContext.Default.GetSerializer<ArrayList>();
							ArrayList unpackedObject = serializer.Unpack(_tmp);

							lock (que)
							{
								que.Enqueue(unpackedObject);
							}
						}
						else
						{
							MemoryStream _tmp = new MemoryStream();
							_tmp.Write(tmpbuf, offset, tmpbufoffset);

							_tmp.ToArray().CopyTo(tmpbuf, 0);

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

		public void senddata(byte[] data)
		{
			s.Send(data);
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

