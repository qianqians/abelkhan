using System;
using System.Collections;

namespace logic
{
	public class hubproxy
	{
		public hubproxy(juggle.Ichannel _ch)
		{
			_logic_call_hub = new caller.hub(_ch);
		}

		public void reg_logic()
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "begin connect hub server");

			_logic_call_hub.reg_logic(logic.uuid);
		}

		public void logic_call_hub_mothed(String module_name, String func_name, ArrayList argvs)
		{
			_logic_call_hub.logic_call_hub_mothed(module_name, func_name, argvs);
		}

		private caller.hub _logic_call_hub;

	}
}

