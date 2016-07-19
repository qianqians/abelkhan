using System;
using System.Collections.Generic;

namespace logic
{
	public class hubmanager
	{
		public hubmanager(service.connectnetworkservice _conn)
		{
			_hub_connect = _conn;
			ch_hubs = new Dictionary<juggle.Ichannel, hubproxy>();
			uuid_hubs = new Dictionary<string, hubproxy>();
		}

		public void connect_hub(String ip, short port)
		{
			var ch = _hub_connect.connect(ip, port);
			hubproxy _hubproxy = new hubproxy(ch);
			_hubproxy.reg_logic();
		}

		public void reg_hub(String hub_name, juggle.Ichannel ch)
		{
			if (ch_hubs.ContainsKey(ch))
			{
				hubproxy _hubproxy = ch_hubs[ch];
				uuid_hubs.Add(hub_name, _hubproxy);
			}
		}

		private Dictionary<juggle.Ichannel, hubproxy> ch_hubs;
		private Dictionary<String, hubproxy> uuid_hubs;

		private service.connectnetworkservice _hub_connect;
	}
}

