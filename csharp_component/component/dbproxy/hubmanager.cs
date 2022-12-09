using System;
using System.Collections;
using System.Collections.Generic;

namespace dbproxy
{
	public class hubmanager
	{
		private Dictionary<string, hubproxy> hubproxys_name;
		private Dictionary<abelkhan.Ichannel, hubproxy> hubproxys;

		private List<string> closed_hub_list;

		public hubmanager()
		{
			hubproxys_name = new Dictionary<string, hubproxy> ();
			hubproxys = new Dictionary<abelkhan.Ichannel, hubproxy> ();
			closed_hub_list = new List<string>();

        }

		public hubproxy reg_hub(abelkhan.Ichannel ch, String name)
		{
			hubproxy _hubproxy = new hubproxy (ch);
			if (hubproxys_name.TryGetValue(name, out hubproxy _old_proxy))
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

		public hubproxy get_hub(abelkhan.Ichannel ch)
		{
			if (hubproxys.ContainsKey (ch)) 
			{
				return hubproxys [ch];
			}

			return null;
		}

		public bool all_hub_closed()
        {
			return hubproxys_name.Count == closed_hub_list.Count;
		}

	}
}

