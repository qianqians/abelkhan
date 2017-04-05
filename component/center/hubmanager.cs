using System;
using System.Collections;
using System.Collections.Generic;

namespace center
{
	class hubmanager
	{
		public hubmanager()
		{
			hubproxys = new Dictionary<juggle.Ichannel, Tuple<string, string, long, string, hubproxy> >();
		}

		public hubproxy reg_hub(juggle.Ichannel ch, String type, String ip, Int64 port, String uuid)
		{
			hubproxy _hubproxy = new hubproxy(ch);
			hubproxys.Add(ch, Tuple.Create(type, ip, port, uuid, _hubproxy));

			return _hubproxy;
		}

		public void for_each_hub(Action<hubproxy> func)
		{
			foreach(Tuple<String, String,Int64, String, hubproxy> value in hubproxys.Values)
			{
				func(value.Item5);
			}
		}

		private Dictionary<juggle.Ichannel, Tuple<String, String,Int64, String, hubproxy> > hubproxys;
	}
}
