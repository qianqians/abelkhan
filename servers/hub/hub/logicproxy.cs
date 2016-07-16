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

		public void call_logic(String module_name, String func_name, ArrayList argvs)
		{
			var json_argvs = System.Text.Json.Jsonparser.pack(argvs);
			_hub_call_logic.hub_call_logic_mothed(module_name, func_name, json_argvs);
		}

		private caller.hub_call_logic _hub_call_logic;
	}
}

