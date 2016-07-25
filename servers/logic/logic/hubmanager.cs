using System;
using System.Collections;
using System.Collections.Generic;

namespace logic
{
	public class hubmanager
	{
		public hubmanager(service.connectnetworkservice _conn)
		{
			_hub_connect = _conn;
			ch_hubs = new Dictionary<juggle.Ichannel, hubproxy>();
			name_hubs = new Dictionary<string, hubproxy>();
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
				name_hubs.Add(hub_name, _hubproxy);
			}
		}

		public void call_hub(String hub_name, String module_name, String func_name, params object[] _argvs)
		{
			if (name_hubs.ContainsKey(hub_name))
			{
				ArrayList _argvs_list = new ArrayList();
				foreach (var o in _argvs)
				{
					_argvs_list.Add(o);
				}
				var argvs = System.Text.Json.Jsonparser.pack(_argvs_list);

				name_hubs[hub_name].logic_call_hub_mothed(module_name, func_name, argvs);
			}
		}

		private Dictionary<juggle.Ichannel, hubproxy> ch_hubs;
		private Dictionary<String, hubproxy> name_hubs;

		private service.connectnetworkservice _hub_connect;
	}
}

