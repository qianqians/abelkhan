using System;

namespace gate
{
	public class centerproxy
	{
		public centerproxy(juggle.Ichannel ch)
		{
			is_reg_center_sucess = false;
			_hub_call_center = new caller.center(ch);
		}

		public void reg_gate(String ip, short port, String uuid)
		{
			Console.WriteLine("begin connect center server");
			_hub_call_center.reg_server("gate", ip, port, uuid);
		}

		public bool is_reg_center_sucess;

		private caller.center _hub_call_center;
	}
}

