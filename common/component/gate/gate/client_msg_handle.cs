using System;
using System.Collections;

namespace gate
{
	public class client_msg_handle
	{
		public client_msg_handle(clientmanager _clientmanager_, logicmanager _logicmanager_, service.timerservice _timerservice_)
		{
			_clientmanager = _clientmanager_;
			_logicmanager = _logicmanager_;
			_timerservice = _timerservice_;
		}

		public void connect_server(string uuid, long tick)
		{
			logicproxy _logicproxy = _logicmanager.get_logic();

			if (!_clientmanager.has_client(uuid)) {
				System.Console.WriteLine("client " + uuid + " connected");

				_clientmanager.reg_client(uuid, juggle.Imodule.current_ch, _timerservice.Tick, tick);

				_logicproxy.client_connect (uuid);
			}
		}

		public void cancle_server()
		{
			logicproxy _logicproxy = _clientmanager.get_clientproxy_logicproxy (juggle.Imodule.current_ch);
			if (_logicproxy != null)
			{
				string uuid = _clientmanager.get_client_uuid(
					_clientmanager.get_clientproxy(juggle.Imodule.current_ch));

				_logicproxy.client_disconnect (uuid);
			}
		}

		public void forward_client_call_logic(string module, string func, ArrayList argv)
		{
			logicproxy _logicproxy = _clientmanager.get_clientproxy_logicproxy (juggle.Imodule.current_ch);
			if (_logicproxy != null)
			{
				string uuid = _clientmanager.get_client_uuid(
					_clientmanager.get_clientproxy(juggle.Imodule.current_ch));
				
				_logicproxy.client_call_logic (uuid, module, func, argv);
			}
		}

		public void heartbeats(long clockt)
		{
			_clientmanager.refresh_and_check_client(juggle.Imodule.current_ch, _timerservice.Tick, clockt);
		}

		private clientmanager _clientmanager;
		private logicmanager _logicmanager;
		private service.timerservice _timerservice;
	}
}

