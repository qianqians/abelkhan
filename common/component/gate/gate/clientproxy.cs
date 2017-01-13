using System;
using System.Collections;

namespace gate
{
	public class clientproxy
	{
		public clientproxy(juggle.Ichannel ch)
		{
			_caller = new caller.gate_call_client(ch);
		}

		public void ack_connect_server(string result)
		{
			_caller.ack_connect_server(result);
		}

		public void call_client(string module, string func, ArrayList argv)
		{
			_caller.call_client(module, func, argv);
		}


		private caller.gate_call_client _caller;
	}
}

