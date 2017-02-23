using System;
using System.Collections;

namespace gate
{
	public class logicproxy
	{
		public logicproxy(juggle.Ichannel ch, string _uuid)
		{
			_caller = new caller.gate_call_logic(ch);
            uuid = _uuid;
        }

		public void reg_logic_sucess()
		{
			_caller.reg_logic_sucess();
		}

		public void get_logic(string uuid)
		{
			_caller.client_get_logic(uuid);
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

        public string uuid;

		private caller.gate_call_logic _caller;
	}
}

