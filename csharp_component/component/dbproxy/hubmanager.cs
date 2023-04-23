using System;
using System.Collections;
using System.Collections.Generic;

namespace dbproxy
{
	public class Hubmanager
	{
		private Dictionary<string, Hubproxy> hubproxys_name;
		private Dictionary<abelkhan.Ichannel, Hubproxy> hubproxys;

		private List<string> closed_hub_list = new ();

		public Hubmanager()
		{
			hubproxys_name = new Dictionary<string, Hubproxy> ();
			hubproxys = new Dictionary<abelkhan.Ichannel, Hubproxy> ();
		}

		public Hubproxy reg_hub(abelkhan.Ichannel ch, String name)
		{
			Hubproxy _hubproxy = new Hubproxy (ch);
			if (hubproxys_name.TryGetValue(name, out Hubproxy _old_proxy))
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

		public bool get_hub(abelkhan.Ichannel ch, out Hubproxy _proxy)
		{
            return hubproxys.TryGetValue(ch, out _proxy);
		}

		public bool all_hub_closed()
        {
			return hubproxys_name.Count == closed_hub_list.Count;
		}

	}
}

