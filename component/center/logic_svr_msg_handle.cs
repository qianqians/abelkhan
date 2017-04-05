using System;

namespace center
{
	class logic_svr_msg_handle
	{
		public logic_svr_msg_handle(svrmanager _svrmanager_, logicmanager _logicmanager_)
		{
			_svrmanager = _svrmanager_;
			_logicmanager = _logicmanager_;
		}

		public void req_get_server_address(String uuid, String callbaclid)
		{
			Tuple<String, String,Int64, String, svrproxy> info = _svrmanager.get_svr_info(uuid);
			logicproxy _logicproxy = _logicmanager.get_logicproxy(juggle.Imodule.current_ch);

			if (_logicproxy != null) {
				if (info != null) {
					_logicproxy.ack_get_server_address (true, info.Item1, info.Item2, info.Item3, info.Item4, callbaclid);
				} 
				else 
				{
					_logicproxy.ack_get_server_address (false, "", "", 0, "", callbaclid);
				}
			} 
			else
			{
				System.Console.WriteLine("not a logic channel call this function");
			}
		}

		private svrmanager _svrmanager;
		private logicmanager _logicmanager;
	}
}
