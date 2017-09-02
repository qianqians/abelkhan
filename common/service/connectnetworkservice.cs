using System;
using System.Net;
using System.Net.Sockets;

namespace service
{
	public class connectnetworkservice : service
	{
		public connectnetworkservice(juggle.process _process)
		{
			process_ = _process;
		}

		public juggle.Ichannel connect(String ip, short port)
		{
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
			try
			{
                s.Connect(new IPEndPoint(IPAddress.Parse(ip), port));

                channel ch = new channel(s);
				ch.onDisconnect += this.onChannelDisconn;

				process_.reg_channel(ch);

				return ch;
			}
			catch (System.Net.Sockets.SocketException e)
			{
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "System.Net.Sockets.SocketException:{0}", e);

                return null;
			}
			catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "System.Exceptio:{0}", e);

                return null;
			}
        }

		public delegate void ChannelDisconnectHandle(juggle.Ichannel ch);
		public event ChannelDisconnectHandle onChannelDisconnect;

		public void onChannelDisconn(channel ch)
		{
            if (onChannelDisconnect != null)
			{
				onChannelDisconnect(ch);
			}

            process_.unreg_channel(ch);
        }

		public void poll(Int64 tick)
		{
		}

		private juggle.process process_;
	}
}

