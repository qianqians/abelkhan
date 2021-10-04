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

		public void reg_hub_sucess()
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "connect gate server sucess");
		}

        public void client_connect(String client_uuid)
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "client {0} connected", client_uuid);

            hub.gates.client_connect(client_uuid, juggle.Imodule.current_ch);
        }

        public void client_disconnect(String client_uuid)
        {
            hub.gates.client_disconnect(client_uuid);
        }

        public void client_exception(String client_uuid)
        {
            hub.gates.client_exception(client_uuid);
        }

        public void client_call_hub(String uuid, String module, String func, ArrayList argv)
		{
			hub.gates.current_client_uuid = uuid;
            hub.modules.process_module_mothed(module, func, argv);
			hub.gates.current_client_uuid = "";
		}

		private common.modulemanager _modulemanager;
	}
}

