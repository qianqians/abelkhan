using System;

namespace hub
{
	public class centerproxy
	{
		public centerproxy(juggle.Ichannel ch)
		{
			is_reg_center_sucess = false;
			_hub_call_center = new caller.center(ch);
		}

		public void reg_hub(String ip, short port, String uuid)
		{
			Console.WriteLine("begin connect center server");
			_hub_call_center.reg_server("hub", ip, port, uuid);
		}

		public bool is_reg_center_sucess;

		private caller.center _hub_call_center;
	}
}

