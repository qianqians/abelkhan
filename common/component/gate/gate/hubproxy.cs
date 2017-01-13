using System;
using System.Collections;
using System.Collections.Generic;

namespace gate
{
	public class hubproxy
	{
		public hubproxy(juggle.Ichannel ch)
		{
			_caller = new caller.gate_call_hub(ch);
		}

		public void reg_hub_sucess()
		{
			_caller.reg_hub_sucess();
		}

		private caller.gate_call_hub _caller;
	}
}

