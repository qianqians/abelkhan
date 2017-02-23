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

		public void ack_get_logic(string logic_uuid)
		{
			_caller.ack_get_logic(logic_uuid);
		}

		public void call_client(string module, string func, ArrayList argv)
		{
			_caller.call_client(module, func, argv);
		}


		private caller.gate_call_client _caller;
	}
}

