using System;
using System.Collections;

namespace logic
{
	public class gateproxy
	{
		public gateproxy(juggle.Ichannel ch)
		{
			_caller = new caller.logic_call_gate(ch);
		}

		public void reg_logic()
		{
			Console.WriteLine("begin connect gate server");
			_caller.reg_logic(logic.uuid);
		}

		public void ack_client_connect_server(String uuid, String result)
		{
			_caller.ack_client_connect_server(uuid, result);
		}

		public void forward_logic_call_client(String uuid, String module, String func, ArrayList argv)
		{
			_caller.forward_logic_call_client(uuid, module, func, argv);
		}

		public void forward_logic_call_group_client(ArrayList uuids, String module, String func, ArrayList argv)
		{
			_caller.forward_logic_call_group_client(uuids, module, func, argv);
		}

		public void forward_logic_call_global_client(String module, String func, ArrayList argv)
		{
			_caller.forward_logic_call_global_client(module, func, argv);
		}

		private caller.logic_call_gate _caller;
	}
}

