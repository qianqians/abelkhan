using System;
using System.Collections;
using System.Collections.Generic;

namespace DBProxy
{
	public class HubManager
	{
		private Dictionary<string, HubProxy> hubproxys_name;
		private Dictionary<Abelkhan.Ichannel, HubProxy> hubproxys;

		private List<string> closed_hub_list = new ();

		public HubManager()
		{
			hubproxys_name = new Dictionary<string, HubProxy> ();
			hubproxys = new Dictionary<Abelkhan.Ichannel, HubProxy> ();
		}

		public HubProxy reg_hub(Abelkhan.Ichannel ch, String name)
		{
			HubProxy _hubproxy = new HubProxy (ch);
			if (hubproxys_name.TryGetValue(name, out HubProxy _old_proxy))
            {
				hubproxys_name[name] = _hubproxy;
			}
            else
            {
				hubproxys_name.Add(name, _hubproxy);
			}
			hubproxys[ch] = _hubproxy;

			return _hubproxy;
		}

		public void on_hub_closed(string name)
		{
			if (!closed_hub_list.Contains(name))
			{
				closed_hub_list.Add(name);
			}
        }

		public bool get_hub(Abelkhan.Ichannel ch, out HubProxy _proxy)
		{
            return hubproxys.TryGetValue(ch, out _proxy);
		}

		public bool all_hub_closed()
        {
			return hubproxys_name.Count == closed_hub_list.Count;
		}

	}
}

