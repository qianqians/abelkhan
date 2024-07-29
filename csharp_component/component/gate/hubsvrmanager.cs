using System;
using System.Collections.Generic;

namespace Gate
{

	public class HubProxy
    {
        public uint _tick_time;

        public readonly string _hub_name;
		public readonly string _hub_type;
		public readonly string _router_type;

		public readonly Abelkhan.Ichannel _ch;

		private readonly Abelkhan.gate_call_hub_caller _gate_call_hub_caller;

		public HubProxy(string hub_name, string hub_type, string router_type, Abelkhan.Ichannel ch) {
			_tick_time = 0;

			_hub_name = hub_name;
			_hub_type = hub_type;
            _router_type = router_type;
            _ch = ch;

			_gate_call_hub_caller = new Abelkhan.gate_call_hub_caller(_ch, Abelkhan.ModuleMgrHandle._modulemng);
		}

		public void client_disconnect(string client_cuuid) {
			_gate_call_hub_caller.client_disconnect(client_cuuid);
		}

		public void client_exception(string client_cuuid) {
			_gate_call_hub_caller.client_exception(client_cuuid);
		}

		public void client_call_hub(string client_cuuid, byte[] data) {
			_gate_call_hub_caller.client_call_hub(client_cuuid, data);
		}

		public void migrate_client(string client_uuid, string target_hub)
		{
			_gate_call_hub_caller.migrate_client(client_uuid, target_hub);
        }

		public bool check_router_dynamic()
		{
			return _router_type == "dynamic";
		}
	}

	public class HubSvrManager
	{
		public readonly Random rd = new();
		private readonly Dictionary<string, HubProxy> wait_destory_proxy = new();
		private readonly Dictionary<string, HubProxy> hub_name_proxy = new();
		private readonly Dictionary<Abelkhan.Ichannel, string> hub_channel_name = new();

		public HubSvrManager()
		{
		}

		public HubProxy reg_hub(string hub_name, string hub_type, string router_type, Abelkhan.Ichannel ch)
		{
			var _hubproxy = new HubProxy(hub_name, hub_type, router_type, ch);

			if (hub_name_proxy.TryGetValue(hub_name, out var proxy))
			{
				wait_destory_proxy.Add(hub_name, proxy);
            }
            hub_channel_name[ch] = hub_name;
            hub_name_proxy[hub_name] = _hubproxy;

            return _hubproxy;
		}

		public void unreg_hub(string hub_name)
		{
			if (wait_destory_proxy.TryGetValue(hub_name, out var _old_proxy))
			{
				hub_channel_name.Remove(_old_proxy._ch);
				wait_destory_proxy.Remove(hub_name);
			}
			else
			{
				if (hub_name_proxy.TryGetValue(hub_name, out var _proxy))
				{
					hub_channel_name.Remove(_proxy._ch);
					hub_name_proxy.Remove(hub_name);
				}
			}
		}

		public bool get_hub(string hub_name, out HubProxy proxy)
		{
			return hub_name_proxy.TryGetValue(hub_name, out proxy);
		}

		public bool get_hub(Abelkhan.Ichannel hub_channel, out HubProxy proxy)
		{
			if (!hub_channel_name.TryGetValue(hub_channel, out var proxy_name))
			{
                proxy = null;
                return false;
			}
			return get_hub(proxy_name, out proxy);
		}

		public string hash_hubproxy(string client_uuid, string hub_type)
		{
            var hub_list = new List<string>();
            foreach (var it in hub_name_proxy)
            {
                if (it.Value._hub_type != hub_type)
                {
                    continue;
                }

                hub_list.Add(it.Value._hub_name);
            }

			var hub_name = string.Empty;
            if (hub_list.Count > 0)
            {
                hub_list.Sort();

				var index = client_uuid.GetHashCode() % hub_list.Count;
                hub_name = hub_list[index];
            }

            return hub_name;
        }

		public bool get_hub_list(string client_uuid, string hub_type, out Abelkhan.hub_info _info)
		{
			var hub_list = new List<Abelkhan.hub_info>();
			foreach (var it in hub_name_proxy)
			{
				if (it.Value._hub_type != hub_type)
				{
					continue;
				}
				if (it.Value._tick_time > 50)
				{
					continue;
				}

				if (it.Value.check_router_dynamic())
				{
                    _info = new Abelkhan.hub_info();
                    _info.hub_name = hash_hubproxy(client_uuid, hub_type);
                    _info.hub_type = it.Value._hub_type;

                    return true;
				}

				var info = new Abelkhan.hub_info();
                info.hub_name = it.Value._hub_name;
                info.hub_type = it.Value._hub_type;

				hub_list.Add(info);
			}

			_info = null;
            if (hub_list.Count > 0)
			{
				var index = rd.Next(hub_list.Count);
				_info = hub_list[index];

				return true;
			}

			return false;
		}
	}

}
