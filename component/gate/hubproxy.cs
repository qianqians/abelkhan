using System;
using System.Collections;
using System.Collections.Generic;

namespace gate
{
	public class hubproxy
	{
		public hubproxy(juggle.Ichannel ch, string _uuid, string _hub_name)
		{
			_caller = new caller.gate_call_hub(ch);
            uuid = _uuid;
            name = _hub_name;
		}

		public void reg_hub_sucess()
		{
			_caller.reg_hub_sucess();
		}

        public void client_connect(string uuid)
        {
            _caller.client_connect(uuid);
        }

        public void client_disconnect(string uuid)
        {
            _caller.client_disconnect(uuid);
        }

        public void client_exception(string uuid)
        {
            _caller.client_exception(uuid);
        }

        public void client_call_hub(string client_uuid, string module, string func, ArrayList argv)
        {
            _caller.client_call_hub(client_uuid, module, func, argv);
        }

        public string uuid;
        public string name;

        private caller.gate_call_hub _caller;
	}
}

