using System;
using System.Collections;

namespace gate
{
	public class clientproxy
	{
		public clientproxy(juggle.Ichannel ch)
		{
            client_ch = ch;
            _caller = new caller.gate_call_client(client_ch);
		}

        public void connect_server_sucess()
        {
            _caller.connect_server_sucess();
        }

        public void ack_heartbeats()
        {
            _caller.ack_heartbeats();
        }

		public void call_client(string module, string func, ArrayList argv)
		{
			_caller.call_client(module, func, argv);
		}

		private caller.gate_call_client _caller;
        public juggle.Ichannel client_ch;

    }
}

