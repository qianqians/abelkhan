using System;
using System.Collections;

namespace gate
{
	public class client_msg_handle
	{
		public client_msg_handle(clientmanager _clientmanager_, hubmanager _hubmanager_, service.timerservice _timerservice_)
		{
			_clientmanager = _clientmanager_;
            _hubmanager = _hubmanager_;
            _timerservice = _timerservice_;
		}

		public void connect_server(string uuid, long tick)
		{
			if (!_clientmanager.has_client(uuid))
            {
                log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "client {0} connected", uuid);

				var _proxy = _clientmanager.reg_client(uuid, juggle.Imodule.current_ch, service.timerservice.Tick, tick);
                _proxy.connect_gate_sucessa();
            }
		}

		public void cancle_server()
		{
			_clientmanager.unreg_client(juggle.Imodule.current_ch);
        }

        public void connect_hub(string client_uuid, string hub_name)
        {
            hubproxy _hubproxy = _hubmanager.get_hub(hub_name);
            if (_hubproxy != null)
            {
                _clientmanager.reg_client_hub(client_uuid, _hubproxy);
                _hubproxy.client_connect(client_uuid);
            }
        }

        public void disconnect_hub(string client_uuid, string hub_name)
        {
            hubproxy _hubproxy = _hubmanager.get_hub(hub_name);
            if (_hubproxy != null)
            {
                _hubproxy.client_disconnect(client_uuid);
                _clientmanager.unreg_client_hub(juggle.Imodule.current_ch);
            }
        }

        public void forward_client_call_hub(string hub_name, string module, string func, ArrayList argv)
        {
            hubproxy _hubproxy = _hubmanager.get_hub(hub_name);

            if (_hubproxy != null)
            {
                var _proxy = _clientmanager.get_clientproxy(juggle.Imodule.current_ch);
                if (_proxy != null)
                {
                    string uuid = _clientmanager.get_client_uuid(_proxy);
                    _hubproxy.client_call_hub(uuid, module, func, argv);
                }
            }
        }

        public void heartbeats(long clockt)
		{
			_clientmanager.refresh_and_check_client(juggle.Imodule.current_ch, service.timerservice.Tick, clockt);
		}

		private clientmanager _clientmanager;
        private hubmanager _hubmanager;
		private service.timerservice _timerservice;
	}
}

