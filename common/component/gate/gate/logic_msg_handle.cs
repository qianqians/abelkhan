using System;
using System.Collections;

namespace gate
{
	public class logic_msg_handle
	{
		public logic_msg_handle(logicmanager _logicmanager_, clientmanager _clientmanager_)
		{
			_logicmanager = _logicmanager_;
			_clientmanager = _clientmanager_;
		}

		public void reg_logic(string uuid)
		{
			logicproxy _logproxy = _logicmanager.reg_logic(uuid, juggle.Imodule.current_ch);
			_logproxy.reg_logic_sucess ();
		}

		public void ack_client_connect_server(string uuid, string result)
		{
			clientproxy _clientproxy = _clientmanager.get_clientproxy(uuid);
			if (_clientproxy != null)
			{
				if (result != "svr_is_busy")
				{
					_clientmanager.reg_client_logic(uuid, _logicmanager.get_logic(juggle.Imodule.current_ch));
					_clientproxy.ack_connect_server(result);
				}
				else {
					logicproxy _logicproxy = _logicmanager.get_logic();
					_logicproxy.client_connect (uuid);
				}
			}
		}

		public void forward_logic_call_client(string uuid, string module, string func, ArrayList argv)
		{
			clientproxy _clientproxy = _clientmanager.get_clientproxy(uuid);
			if (_clientproxy != null)
			{
				_clientproxy.call_client(module, func, argv);
			}
		}

		public void forward_logic_call_group_client(ArrayList uuids, string module, string func, ArrayList argv)
		{
			foreach(String uuid in uuids)
			{
				forward_logic_call_client(uuid, module, func, argv);
			}
		}

		public void forward_logic_call_global_client(string module, string func, ArrayList argv)
		{
			_clientmanager.for_each_client(
				(clientproxy _clientproxy) =>
                {
                    if (_clientproxy != null)
                    {
                        _clientproxy.call_client(module, func, argv);
                    }
                }
			);
		}

		private clientmanager _clientmanager;
		private logicmanager _logicmanager;
	}
}

