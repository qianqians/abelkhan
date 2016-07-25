using System;
using System.Collections;

namespace logic
{
	public class logic_msg_handle
	{
		public logic_msg_handle(common.modulemanager _modulemanager_)
		{
			_modulemanager = _modulemanager_;
		}

		public void on_reg_logic(String uuid, String callbackid)
		{
			var _proxy = logic.logics.on_reg_logic(uuid, juggle.Imodule.current_ch);
			_proxy.ack_reg_logic(callbackid);
		}

		public void on_ack_reg_logic(String uuid, String callbackid)
		{
			logic.logics.on_ack_reg_logic(uuid, juggle.Imodule.current_ch);
			logic.logics.do_callback(callbackid);
		}

		public void logic_call_logic_mothed(String module_name, String func_name, ArrayList argvs)
		{
			_modulemanager.process_module_mothed(module_name, func_name, argvs);
		}

		private common.modulemanager _modulemanager;
	}
}

