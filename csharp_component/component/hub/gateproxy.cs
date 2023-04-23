using MsgPack.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace hub
{
	public class Gateproxy
	{
		public readonly abelkhan.Ichannel _ch;
		public readonly string _name;

		private readonly abelkhan.hub_call_gate_caller _hub_call_gate_caller;

		public Gateproxy(abelkhan.Ichannel ch, string name)
		{
			_ch = ch;
			_name = name;

			_hub_call_gate_caller = new abelkhan.hub_call_gate_caller(ch, abelkhan.modulemng_handle._modulemng);
		}

		public void reg_hub()
        {
            log.Log.trace("begin connect gate server");

			_hub_call_gate_caller.reg_hub(Hub.name, Hub.type).callBack(() =>
			{
				log.Log.trace("connect gate server sucessed");
			}, () =>
			{
				log.Log.trace("connect gate server faild");
			}).timeout(5000, () =>
			{
				log.Log.trace("connect gate server timeout");
			});
		}

		public void tick_hub_health()
		{
			_hub_call_gate_caller.tick_hub_health(Hub.tick);
        }

		public abelkhan.hub_call_gate_reverse_reg_client_hub_cb reverse_reg_client_hub(string client_uuid)
        {
			return _hub_call_gate_caller.reverse_reg_client_hub(client_uuid);
		}

		public void unreg_client_hub(string client_uuid)
        {
			_hub_call_gate_caller.unreg_client_hub(client_uuid);
		}

		public void disconnect_client(String uuid)
        {
			_hub_call_gate_caller.disconnect_client(uuid);
        }

        public void forward_hub_call_client(String uuid, byte[] rpc_bin)
        {
			_hub_call_gate_caller.forward_hub_call_client(uuid, rpc_bin);
		}

        public void forward_hub_call_group_client(List<string> uuids, byte[] rpc_bin)
		{
			_hub_call_gate_caller.forward_hub_call_group_client(uuids, rpc_bin);
		}

		public void forward_hub_call_global_client(byte[] rpc_bin)
		{
			_hub_call_gate_caller.forward_hub_call_global_client(rpc_bin);
		}
	}
}

