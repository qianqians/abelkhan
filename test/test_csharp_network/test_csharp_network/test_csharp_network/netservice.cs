using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace test_csharp_network
{
	public class netservice
	{
		public netservice()
		{
			listen = new TcpListener(IPAddress.Parse("0.0.0.0"), 1234);

			listen.Start();

			listen.BeginAcceptSocket(new AsyncCallback(onAccept), listen);
		}

		private static void onAccept(IAsyncResult ar)
		{
			TcpListener listener = ar.AsyncState as TcpListener;

			Socket clientSocket = listener.EndAcceptSocket(ar);

			// Process the connection here. (Add the client to a 
			// server table, read data, etc.)
			Console.WriteLine("Client connected completed");

			listener.BeginAcceptSocket(new AsyncCallback(netservice.onAccept), listener);
		}

		static void Main()
		{
			var _service = new netservice();

			Thread.CurrentThread.Join();
		}
		                 
		private TcpListener listen;
	}
}

