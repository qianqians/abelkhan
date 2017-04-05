using System;
using System.Collections;

namespace logic
{
	public class gate_msg_handle
	{
		public gate_msg_handle(common.modulemanager _modulemanager_)
		{
			_modulemanager = _modulemanager_;
		}

		public void onreg_logic_sucess()
		{
			Console.WriteLine("connect gate server sucess");
		}

		public void client_get_logic(String uuid)
		{
			if (logic.is_busy)
			{
				var _proxy = logic.gates.get_gateproxy(juggle.Imodule.current_ch);
				_proxy.ack_client_get_logic(uuid, "svr_is_busy");
			}
			else 
			{
				var _proxy = logic.gates.get_gateproxy(juggle.Imodule.current_ch);
				_proxy.ack_client_get_logic(uuid, "svr_is_sucess");
			}
		}

        public void client_connect(String uuid)
        {
            Console.WriteLine("client " + uuid + " connected");

            logic.gates.client_connect(uuid, juggle.Imodule.current_ch);
        }

        public void client_disconnect(String uuid)
		{
			logic.gates.client_disconnect(uuid);
		}

        public void client_exception(String uuid)
        {
            logic.gates.client_exception(uuid);
        }

        public void client_call_logic(String uuid, String module, String func, ArrayList argv)
		{
			logic.gates.current_client_uuid = uuid;
			logic.modules.process_module_mothed(module, func, argv);
			logic.gates.current_client_uuid = "";
		}

		private common.modulemanager _modulemanager;
	}
}

