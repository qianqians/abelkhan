using System;
using System.Collections;

namespace hub
{
	public class logic_msg_handle
	{
		public logic_msg_handle(common.modulemanager _modulemanager_, logicmanager _logicmanager_)
		{
			_modulemanager = _modulemanager_;
			_logicmanager = _logicmanager_;
		}

		public void reg_logic(String uuid)
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "logic server {0} connected", uuid);

			_logicmanager.reg_logic(uuid, juggle.Imodule.current_ch);
		}

		public void logic_call_hub_mothed(String module_name, String func_name, ArrayList argvs)
		{
            hub.modules.process_module_mothed(module_name, func_name, argvs);
		}

		private common.modulemanager _modulemanager;
		private logicmanager _logicmanager;
	}
}

