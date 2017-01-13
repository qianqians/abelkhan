using System;

namespace center
{
	class logicproxy
	{
		public logicproxy(juggle.Ichannel _ch)
		{
			_caller = new caller.center_call_logic (_ch);
		}

		public void distribute_server_address(String type, String ip, Int64 port, String uuid)
		{
			_caller.distribute_server_address(type, ip, port, uuid);
		}

		public void ack_get_server_address(bool have_svr, String type, String ip, Int64 port, String uuid, String callbaclid)
		{
			_caller.ack_get_server_address (have_svr, type, ip, port, uuid, callbaclid);
		}

		private caller.center_call_logic _caller;
	}
}
