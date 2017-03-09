using System;
using System.Collections;

namespace gate
{
	public class hub_msg_handle
	{
		public hub_msg_handle(hubmanager _hubmanager_, clientmanager _clientmanager_)
		{
			_hubmanager = _hubmanager_;
			_clientmanager = _clientmanager_;
		}

		public void reg_hub(string uuid, string hub_name)
		{
			hubproxy _hubproxy = _hubmanager.reg_hub(juggle.Imodule.current_ch, uuid, hub_name);
			_hubproxy.reg_hub_sucess ();
		}

        public void connect_sucess(string client_uuid)
        {
            clientproxy _clientproxy = _clientmanager.get_clientproxy(client_uuid);
            if (_clientproxy != null)
            {
                hubproxy _hubproxy = _hubmanager.get_hub(juggle.Imodule.current_ch);
                if (_hubproxy != null)
                {
                    _clientproxy.connect_hub_sucess(_hubproxy.name);
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

        public void forward_hub_call_client(string uuid, string module, string func, ArrayList argv)
        {
            clientproxy _clientproxy = _clientmanager.get_clientproxy(uuid);
            if (_clientproxy != null)
            {
                _clientproxy.call_client(module, func, argv);
            }
        }

        public void forward_hub_call_group_client(ArrayList uuids, string module, string func, ArrayList argv)
		{
			foreach(String uuid in uuids)
			{
				clientproxy _clientproxy = _clientmanager.get_clientproxy(uuid);
				if (_clientproxy != null)
				{
					_clientproxy.call_client (module, func, argv);
				}
			}
		}

		public void forward_hub_call_global_client(string module, string func, ArrayList argv)
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

		private hubmanager _hubmanager;
		private clientmanager _clientmanager;
	}
}

