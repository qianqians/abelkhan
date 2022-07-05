using System;
using System.Collections;
using System.Collections.Generic;

namespace dbproxy
{
	public class hubmanager
	{
		private Dictionary<string, hubproxy> wait_destory_hubs;
		private Dictionary<string, hubproxy> hubproxys_name;
		private Dictionary<abelkhan.Ichannel, hubproxy> hubproxys;

		public hubmanager()
		{
			wait_destory_hubs = new Dictionary<string, hubproxy>();
			hubproxys_name = new Dictionary<string, hubproxy> ();
			hubproxys = new Dictionary<abelkhan.Ichannel, hubproxy> ();
		}

		public hubproxy reg_hub(abelkhan.Ichannel ch, String name)
		{
			hubproxy _hubproxy = new hubproxy (ch);
			if (hubproxys_name.TryGetValue(name, out hubproxy _old_proxy))
            {
				if (wait_destory_hubs.Remove(name, out hubproxy _destory_hubproxy))
				{
					hubproxys.Remove(_destory_hubproxy._ch);

					lock (dbproxy.remove_chs)
					{
						dbproxy.remove_chs.Add(_destory_hubproxy._ch);
					}
				}

				wait_destory_hubs.Add(name, _old_proxy);
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
			if (wait_destory_hubs.TryGetValue(name, out hubproxy _old_proxy))
			{
				hubproxys.Remove(_old_proxy._ch);
				wait_destory_hubs.Remove(name);

				lock (dbproxy.remove_chs)
				{
					dbproxy.remove_chs.Add(_old_proxy._ch);
				}
			}
			else
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
		}

		public hubproxy get_hub(abelkhan.Ichannel ch)
		{
			if (hubproxys.ContainsKey (ch)) 
			{
				return hubproxys [ch];
			}

			return null;
		}

		public int hub_num()
        {
			return hubproxys_name.Count;
		}

	}
}

