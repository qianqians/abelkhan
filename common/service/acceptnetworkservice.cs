using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace service
{
	public class acceptnetworkservice : service
	{
		public acceptnetworkservice(String ip, short port, juggle.process _process)
		{
			process_ = _process;

			listen = new TcpListener(IPAddress.Parse(ip), port);

			listen.Start();

			listen.BeginAcceptSocket(new AsyncCallback(this.onAccept), listen);
		}

		private void onAccept(IAsyncResult ar)
		{
			TcpListener listener = ar.AsyncState as TcpListener;

			Socket clientSocket = listener.EndAcceptSocket(ar);

			{
				channel ch = new channel(clientSocket);
				ch.onDisconnect += this.onChannelDisconn;

				onChannelConn(ch);
			}

			listener.BeginAcceptSocket(new AsyncCallback(this.onAccept), listener);
		}

		public delegate void ChannelConnectHandle(juggle.Ichannel ch);
		public event ChannelConnectHandle onChannelConnect;

		public void onChannelConn(channel ch)
		{
			process_.reg_channel(ch);

			if (onChannelConnect != null)
			{
				onChannelConnect(ch);
			}
		}

		public delegate void ChannelDisconnectHandle(juggle.Ichannel ch);
		public event ChannelDisconnectHandle onChannelDisconnect;

		public void onChannelDisconn(juggle.Ichannel ch)
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

		private TcpListener listen;
		private juggle.process process_;
	}
}

