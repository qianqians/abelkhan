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

            listen.Start(32);

			listen.BeginAcceptSocket(new AsyncCallback(this.onAccept), listen);
		}

		private void onAccept(IAsyncResult ar)
		{
			TcpListener listener = ar.AsyncState as TcpListener;
            
            try
			{
                Socket clientSocket = listener.EndAcceptSocket(ar);
                channel ch = new channel(clientSocket);
				ch.onDisconnect += this.onChannelDisconn;
                ch.Disconnect += this.ChannelDisconn;

                onChannelConn(ch);
			}
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "System.Exception:{0}", e);
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

		public void onChannelDisconn(channel ch)
		{
            ch.s.Dispose();

            if (onChannelDisconnect != null)
			{
				onChannelDisconnect(ch);
			}

			process_.unreg_channel(ch);
		}

        public void ChannelDisconn(channel ch)
        {
            process_.unreg_channel(ch);
        }
        
        public void poll(Int64 tick)
		{
		}

		private TcpListener listen;
		private juggle.process process_;
	}
}

