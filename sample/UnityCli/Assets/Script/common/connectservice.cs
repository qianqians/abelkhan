using System;
using System.Net;
using System.Net.Sockets;

namespace Abelkhan
{
    public class ConnectService
    {
        public static Socket connect(IPAddress address, short port)
        {
            Socket s;

            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                s = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            }

            s?.Connect(address, port);
            return s;
        }
    }
}

