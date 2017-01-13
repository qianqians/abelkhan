using System;
using System.Collections;

namespace logic
{
	public class logicproxy
	{
		public logicproxy(juggle.Ichannel ch)
		{
			_caller = new caller.logic_call_logic(ch);
		}

		public void reg_logic(String callbackid){
			_caller.reg_logic(logic.uuid, callbackid);
		}

		public void ack_reg_logic(String callbackid)
		{
			_caller.ack_reg_logic(logic.uuid, callbackid);
		}

		public void call_logic(String module_name, String func_name, ArrayList argvs)
		{
			_caller.logic_call_logic_mothed(module_name, func_name, argvs);
		}

		private caller.logic_call_logic _caller;
	}
}

