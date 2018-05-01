using System;

namespace hub
{
	public class centerproxy
	{
		public centerproxy(juggle.Ichannel ch)
		{
			is_reg_center_sucess = false;
            _center = new caller.center(ch);
            _hub_call_center = new caller.hub_call_center(ch);

        }

		public void reg_hub(String ip, short port, String uuid)
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "begin connect center server");

            _center.reg_server("hub", ip, port, uuid);
		}

        public void closed()
        {
            _hub_call_center.closed();
        }

		public bool is_reg_center_sucess;

		private caller.center _center;
        private caller.hub_call_center _hub_call_center;

    }
}

