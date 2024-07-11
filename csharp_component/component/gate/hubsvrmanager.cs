using System;
using System.Collections.Generic;

namespace Gate
{

	public class HubProxy {
		public string _hub_name;
		public string _hub_type;
		public string _router_type;

        public uint _tick_time;
		public Abelkhan.Ichannel _ch;

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

		public bool check_router_dynamic()
		{
			return _router_type == "dynamic";
		}
	}

	public class HubSvrManager
	{
		private readonly Random rd = new();
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
				hub_channel_name[ch] = hub_name;
				hub_name_proxy[hub_name] = _hubproxy;
			}
			else
			{
				hub_name_proxy.Add(hub_name, _hubproxy);
				hub_channel_name.Add(ch, hub_name);
			}

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

		public HubProxy get_hub(string hub_name)
		{
			hub_name_proxy.TryGetValue(hub_name, out var proxy);
			return proxy;
		}

		public HubProxy get_hub(Abelkhan.Ichannel hub_channel)
		{
			hub_channel_name.TryGetValue(hub_channel, out var proxy_name);
			return get_hub(proxy_name);
		}

		public bool get_hub_list(string hub_type, out Abelkhan.hub_info _info)
		{
			var hub_list = new List<Abelkhan.hub_info>();
			foreach (var it in hub_name_proxy)
			{
				if (it.Value._hub_type != hub_type)
				{
					continue;
				}
				if (it.Value._tick_time > 100)
				{
					continue;
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

		public void for_each_hub(Action<string, HubProxy> fn)
		{
			foreach (var it in hub_name_proxy)
			{
				fn(it.Key, it.Value);
			}
		}
	}

}
