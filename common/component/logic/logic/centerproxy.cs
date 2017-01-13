using System;

namespace logic
{
	public class centerproxy
	{
		public centerproxy(juggle.Ichannel ch)
		{
			is_reg_sucess = false;
			_logic_call_center = new caller.center(ch);
		}

		public void reg_logic(String ip, short port, String uuid)
		{
			Console.WriteLine("begin connect center server");
			_logic_call_center.reg_server("logic", ip, port, uuid);
		}

		public bool is_reg_sucess;

		private caller.center _logic_call_center;
	}
}

