using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace service
{
    public class udpconnectnetworkservice : service
	{
		public udpconnectnetworkservice(juggle.process _process)
		{
            _tmp_socket_data = new Dictionary<Socket, tmp_socket_data>();

            process_ = _process;
		}


        public juggle.Ichannel connect(String ip, short port)
		{
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
			{
                s.Bind(new IPEndPoint(IPAddress.Any, 0));

                udpchannel ch = new udpchannel(s, new IPEndPoint(IPAddress.Parse(ip), port));
				ch.onDisconnect += this.onChannelDisconn;
                ch.Disconnect += this.disconn_channel;

                process_.reg_channel(ch);

                _tmp_socket_data.Add(s, new tmp_socket_data(ch));
                s.BeginReceiveFrom(_tmp_socket_data[s].recvbuf, 0, _tmp_socket_data[s].recvbuflenght, 0, ref _tmp_socket_data[s].remote_ep, new AsyncCallback(this.onRecv), s);

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

        private void onRecv(IAsyncResult ar)
        {
            Socket s = ar.AsyncState as Socket;

            try
            {
                int read = s.EndReceiveFrom(ar, ref _tmp_socket_data[s].remote_ep);
                if (read > 0)
                {
                    IPEndPoint sender = (IPEndPoint)_tmp_socket_data[s].remote_ep;

                    var ip_sender = sender.Address.GetAddressBytes();
                    var ip_remote = _tmp_socket_data[s].ch.remote_ep.Address.GetAddressBytes();

                    if (ip_sender[0] == ip_remote[0] &&
                        ip_sender[1] == ip_remote[1] &&
                        ip_sender[2] == ip_remote[2] &&
                        ip_sender[3] == ip_remote[3] &&
                        sender.Port == _tmp_socket_data[s].ch.remote_ep.Port)
                    {
                        _tmp_socket_data[s].ch.recv(_tmp_socket_data[s].recvbuf, read);
                    }
                }

                s.BeginReceiveFrom(_tmp_socket_data[s].recvbuf, 0, _tmp_socket_data[s].recvbuflenght, 0, ref _tmp_socket_data[s].remote_ep, new AsyncCallback(this.onRecv), s);
            }
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), timerservice.Tick, "System.Exception:{0}", e);
            }
        }

        public void disconn_channel(juggle.Ichannel ch)
        {
            udpchannel udp_ch = ch as udpchannel;
            _tmp_socket_data.Remove(udp_ch.s);
            udp_ch.s.Close();
        }

        public delegate void ChannelDisconnectHandle(juggle.Ichannel ch);
		public event ChannelDisconnectHandle onChannelDisconnect;

		public void onChannelDisconn(juggle.Ichannel ch)
		{
            disconn_channel(ch);

            if (onChannelDisconnect != null)
			{
				onChannelDisconnect(ch);
			}
		}

		public void poll(Int64 tick)
		{
        }

        public class tmp_socket_data
        {
            public tmp_socket_data(udpchannel _ch)
            {
                remote_ep = new IPEndPoint(IPAddress.Any, 0);
                recvbuflenght = 16 * 1024;
                recvbuf = new byte[recvbuflenght];

                ch = _ch;
            }

            public EndPoint remote_ep;
            public byte[] recvbuf;
            public Int32 recvbuflenght;
            public udpchannel ch;
        }
        private Dictionary<Socket, tmp_socket_data> _tmp_socket_data;

        private juggle.process process_;
	}
}

