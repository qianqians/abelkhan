using System;
using System.Net;
using System.Net.Sockets;

namespace service
{
	public class networkservice : service
	{
		public networkservice(String ip, short port, process _process)
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
				onChannelConn(ch);
				ch.onDisconnect += this.onChannelDisconn;
			}

			listener.BeginAcceptSocket(new AsyncCallback(this.onAccept), listener);
		}

		public delegate void ChannelConnectHandle(channel ch);
		public event ChannelConnectHandle onChannelConnect;

		public void onChannelConn(channel ch)
		{
			onChannelConnect(ch);
		}

		public delegate void ChannelDisconnectHandle(channel ch);
		public event ChannelDisconnectHandle onChannelDisconnect;

		public void onChannelDisconn(channel ch)
		{
			onChannelDisconnect(ch);
		}

		public void poll(Int64 tick)
		{
		}

		private TcpListener listen;
		private process process_;
	}
}

