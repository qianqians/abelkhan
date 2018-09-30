using System;

namespace dbproxy
{
	public class centerproxy
	{
		public centerproxy(juggle.Ichannel ch)
		{
			is_reg_sucess = false;
			_logic_call_center = new caller.center(ch);
		}

		public void reg_dbproxy(String ip, short port, String uuid, int zone_id)
		{
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "begin connect center server");

			_logic_call_center.reg_server("dbproxy", ip, port, uuid, zone_id);
		}

		public bool is_reg_sucess;

		private caller.center _logic_call_center;
	}
}

