using System;
using System.Collections;

namespace hub
{
	public class logicproxy
	{
		public logicproxy(juggle.Ichannel ch)
		{
			_hub_call_logic = new caller.hub_call_logic(ch);
		}

		public void reg_logic_sucess_and_notify_hub_nominate()
		{
			_hub_call_logic.reg_logic_sucess_and_notify_hub_nominate(hub.name);
		}

		public void call_logic(String module_name, String func_name, params object[] argvs)
		{
			ArrayList _argvs = new ArrayList();
			foreach (var o in argvs)
			{
				_argvs.Add(o);
			}

			_hub_call_logic.hub_call_logic_mothed(module_name, func_name, _argvs);
		}

		private caller.hub_call_logic _hub_call_logic;
	}
}

