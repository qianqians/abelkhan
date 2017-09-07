using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace service
{
	public class udpacceptnetworkservice : service
	{
		public udpacceptnetworkservice(String ip, short port, juggle.process _process)
		{
            udpchannels = new Dictionary<string, udpchannel>();

            process_ = _process;

            listen = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            listen.Bind(ep);

            recvbuflenght = 16 * 1024;
            recvbuf = new byte[recvbuflenght];

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            tempRemoteEP = (EndPoint)sender;

            listen.BeginReceiveFrom(recvbuf, 0, recvbuflenght, 0, ref tempRemoteEP, new AsyncCallback(this.onRecv), this);
		}

		private void onRecv(IAsyncResult ar)
		{
            udpacceptnetworkservice service = ar.AsyncState as udpacceptnetworkservice;

            try
            {
                int read = listen.EndReceiveFrom(ar, ref tempRemoteEP);
                if (read > 0)
                {
                    IPEndPoint sender = (IPEndPoint)tempRemoteEP;

                    if (udpchannels.ContainsKey(sender.ToString()))
                    {
                        udpchannel ch = udpchannels[sender.ToString()];

                        ch.recv(recvbuf, read);
                    }
                    else
                    {
                        udpchannel ch = new udpchannel(listen, sender);
                        ch.onDisconnect += this.onChannelDisconn;

                        udpchannels.Add(sender.ToString(), ch);
                        onChannelConn(ch);

                        ch.recv(recvbuf, read);
                    }
                }

                listen.BeginReceiveFrom(recvbuf, 0, recvbuflenght, 0, ref tempRemoteEP, new AsyncCallback(this.onRecv), this);
            }
            catch (System.Net.Sockets.SocketException)
            {
            }
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "System.Exception:{0}", e);
            }
        }

		public delegate void ChannelConnectHandle(juggle.Ichannel ch);
		public event ChannelConnectHandle onChannelConnect;

		public void onChannelConn(juggle.Ichannel ch)
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

            udpchannel udp_ch = ch as udpchannel;
            udpchannels.Remove(udp_ch.remote_ep.ToString());
        }

		public void poll(Int64 tick)
		{
		}

        private byte[] recvbuf;
        private Int32 recvbuflenght;
        EndPoint tempRemoteEP;
        private Socket listen;
		private juggle.process process_;

        private Dictionary<string, udpchannel> udpchannels;

    }
}

