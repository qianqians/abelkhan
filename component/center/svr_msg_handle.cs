using System;

namespace center
{
	class svr_msg_handle
	{
		public svr_msg_handle(svrmanager _svrmanager_, hubmanager _hubmanager_, logicmanager _logicmanager_)
		{
			_svrmanager = _svrmanager_;
			_logicmanager = _logicmanager_;
			_hubmanager = _hubmanager_;
			_dbproxy = null;
		}

		public void reg_server(String type, String ip, Int64 port, String uuid)
		{
			_logicmanager.for_each_logic(
				(logicproxy _logicproxy) => {
					_logicproxy.distribute_server_address(type, ip, port, uuid);
				}
			);

			_hubmanager.for_each_hub (
				(hubproxy _hubproxy) => {
					_hubproxy.distribute_server_address(type, ip, port, uuid);
				}
			);

			if (type == "logic") {
				logicproxy _logicproxy = _logicmanager.reg_logic(juggle.Imodule.current_ch, type, ip, port, uuid);

				_svrmanager.for_each_svr_info (
					(String _type, String _ip, Int64 _port, String _uuid) => {
						_logicproxy.distribute_server_address(_type, _ip, _port, _uuid);
					}
				);
			}

			if (type == "hub") {
				hubproxy _hubproxy = _hubmanager.reg_hub (juggle.Imodule.current_ch, type, ip, port, uuid);

				_svrmanager.for_each_svr_info (
					(String _type, String _ip, Int64 _port, String _uuid) => {
						_hubproxy.distribute_server_address(_type, _ip, _port, _uuid);
					}
				);
			}

			if (type == "dbproxy") {
				_dbproxy = new dbproxy (juggle.Imodule.current_ch, type, ip, port, uuid);
			}
			
			svrproxy _svrproxy = _svrmanager.reg_svr(juggle.Imodule.current_ch, type, ip, port, uuid);
			_svrproxy.reg_server_sucess();

			System.Console.WriteLine (type + " server " + uuid + " connected");
		}

		private svrmanager _svrmanager;
		private logicmanager _logicmanager;
		private hubmanager _hubmanager;
		private dbproxy _dbproxy;
	}
}
