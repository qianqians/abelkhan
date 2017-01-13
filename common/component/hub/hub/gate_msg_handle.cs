using System;
using System.Collections;

namespace hub
{
	public class gate_msg_handle
	{
		public gate_msg_handle(common.modulemanager _modulemanager_)
		{
			_modulemanager = _modulemanager_;
		}

		public void onreg_hub_sucess()
		{
			Console.WriteLine("connect gate server sucess");
		}

        //public void client_call_logic(String uuid, String module, String func, ArrayList argv)
		//{
		//	hub.gates.current_client_uuid = uuid;
		//	hub.modules.process_module_mothed(module, func, argv);
		//	hub.gates.current_client_uuid = "";
		//}

		private common.modulemanager _modulemanager;
	}
}

