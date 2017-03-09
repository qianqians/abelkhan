using System;
using System.Collections;

namespace hub
{
	public class gateproxy
	{
		public gateproxy(juggle.Ichannel ch)
		{
			_caller = new caller.hub_call_gate(ch);
		}

		public void reg_logic()
		{
			Console.WriteLine("begin connect gate server");
			_caller.reg_hub(hub.uuid, hub.name);
		}

        public void connect_sucess(string client_uuid)
        {
            _caller.connect_sucess(client_uuid);
        }

        public void disconnect_client(String uuid)
        {
            _caller.disconnect_client(uuid);
        }

        public void forward_hub_call_client(String uuid, String module, String func, ArrayList argv)
        {
            _caller.forward_hub_call_client(uuid, module, func, argv);
        }

        public void forward_hub_call_group_client(ArrayList uuids, String module, String func, ArrayList argv)
		{
			_caller.forward_hub_call_group_client(uuids, module, func, argv);
		}

		public void forward_hub_call_global_client(String module, String func, ArrayList argv)
		{
			_caller.forward_hub_call_global_client(module, func, argv);
		}

		private caller.hub_call_gate _caller;
	}
}

