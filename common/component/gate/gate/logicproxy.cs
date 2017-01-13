using System;
using System.Collections;

namespace gate
{
	public class logicproxy
	{
		public logicproxy(juggle.Ichannel ch)
		{
			_caller = new caller.gate_call_logic(ch);
		}

		public void reg_logic_sucess()
		{
			_caller.reg_logic_sucess();
		}

		public void client_connect(string uuid)
		{
			_caller.client_connect(uuid);
		}

		public void client_disconnect(string uuid)
		{
			_caller.client_disconnect(uuid);
		}

		public void client_exception(string uuid)
		{
			_caller.client_exception(uuid);
		}

		public void client_call_logic(string uuid, string module, string func, ArrayList argv)
		{
			_caller.client_call_logic(uuid, module, func, argv);
		}

		private caller.gate_call_logic _caller;
	}
}

