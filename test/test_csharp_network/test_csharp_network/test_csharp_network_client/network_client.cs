using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace test_csharp_network_client
{
	public class network_client
	{
		public network_client()
		{
			s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			s.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
			s.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234));

			Console.WriteLine("connected server completed");
		}

		public static void Main()
		{
			var _network_client = new network_client();

			Thread.CurrentThread.Join();
		}

		private Socket s;
	}
}

