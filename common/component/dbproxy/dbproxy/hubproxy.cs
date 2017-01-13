using System;
using System.Collections;

namespace dbproxy
{
	public class hubproxy
	{
		public hubproxy(juggle.Ichannel ch)
		{
			_caller = new caller.dbproxy_call_hub(ch);
		}

		public void reg_hub_sucess()
		{
			_caller.reg_hub_sucess();
		}

		public void ack_create_persisted_object(String callbackid)
		{
			_caller.ack_create_persisted_object(callbackid);
		}

		public void ack_updata_persisted_object(String callbackid)
		{
			_caller.ack_updata_persisted_object(callbackid);
		}

		public void ack_get_object_info(string callbackid, ArrayList object_info)
		{
			_caller.ack_get_object_info(callbackid, object_info);
		}

		public void ack_get_object_info_end(string callbackid)
		{
			_caller.ack_get_object_info_end(callbackid);
		}

		private caller.dbproxy_call_hub _caller;
	}
}

