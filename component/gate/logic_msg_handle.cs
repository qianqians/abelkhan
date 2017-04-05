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

		public void ack_client_get_logic(string client_uuid, string result)
		{
			clientproxy _clientproxy = _clientmanager.get_clientproxy(client_uuid);
			if (_clientproxy != null)
            {
                logicproxy _logicproxy = _logicmanager.get_logic(juggle.Imodule.current_ch);
                if (result == "svr_is_success")
				{
                    _clientproxy.ack_get_logic(_logicproxy.uuid);
				}
				else {
                    Console.WriteLine(result + " svr:" + _logicproxy.uuid);

					_logicproxy = _logicmanager.get_logic();
					_logicproxy.get_logic (client_uuid);
				}
			}
		}

        public void connect_sucess(string client_uuid)
        {
            clientproxy _clientproxy = _clientmanager.get_clientproxy(client_uuid);
            if (_clientproxy != null)
            {
                logicproxy _logicproxy = _logicmanager.get_logic(juggle.Imodule.current_ch);
                if (_logicproxy != null)
                {
                    _clientproxy.connect_logic_sucess(_logicproxy.uuid);
                }
            }
        }

        public void disconnect_client(string client_uuid)
        {
            clientproxy _clientproxy = _clientmanager.get_clientproxy(client_uuid);
            if (_clientproxy != null)
            {
                _clientmanager.unreg_client(_clientproxy.client_ch);
                _clientmanager.unreg_client_hub(_clientproxy.client_ch);
                _clientmanager.unreg_client_logic(_clientproxy.client_ch);
                _clientproxy.client_ch.disconnect();
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

