using MsgPack.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Hub
{
	public class GateProxy
	{
		public readonly Abelkhan.Ichannel _ch;
		public readonly string _name;

		private readonly Abelkhan.hub_call_gate_caller _hub_call_gate_caller;

		public GateProxy(Abelkhan.Ichannel ch, string name)
		{
			_ch = ch;
			_name = name;

			_hub_call_gate_caller = new Abelkhan.hub_call_gate_caller(ch, Abelkhan.ModuleMgrHandle._modulemng);
		}

		public void reg_hub()
        {
            Log.Log.trace("begin connect gate server");

			_hub_call_gate_caller.reg_hub(Hub.name, Hub.type, Hub.router_type).callBack(() =>
			{
				Log.Log.trace("connect gate server sucessed");
			}, () =>
			{
				Log.Log.trace("connect gate server faild");
			}).timeout(5000, () =>
			{
				Log.Log.trace("connect gate server timeout");
			});
		}

		public void tick_hub_health()
		{
			_hub_call_gate_caller.tick_hub_health(Hub.tick);
        }

		public void migrate_client_done(string client_uuid, string _src_hub, string _target_hub)
		{
			_hub_call_gate_caller.migrate_client_done(client_uuid, _src_hub, _target_hub);
        }

		public Abelkhan.hub_call_gate_reverse_reg_client_hub_cb reverse_reg_client_hub(string client_uuid)
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

