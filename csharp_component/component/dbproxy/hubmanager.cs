using System;
using System.Collections;
using System.Collections.Generic;

namespace dbproxy
{
	public class hubmanager
	{
		public hubmanager()
		{
			hubproxys_name = new Dictionary<string, hubproxy> ();
			hubproxys = new Dictionary<abelkhan.Ichannel, hubproxy> ();
		}

		public hubproxy reg_hub(abelkhan.Ichannel ch, String name)
		{
			hubproxy _hubproxy = new hubproxy (ch);
			hubproxys_name.Add (name, _hubproxy);
			hubproxys.Add (ch, _hubproxy);

			return _hubproxy;
		}

		public void on_hub_closed(string name)
		{
			if (hubproxys_name.TryGetValue(name, out hubproxy _proxy))
			{
				hubproxys_name.Remove(name);
				hubproxys.Remove(_proxy._ch);

                lock (dbproxy.remove_chs)
                {
					dbproxy.remove_chs.Add(_proxy._ch);
				}
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

		private Dictionary<String, hubproxy> hubproxys_name;
		private Dictionary<abelkhan.Ichannel, hubproxy> hubproxys;

	}
}

