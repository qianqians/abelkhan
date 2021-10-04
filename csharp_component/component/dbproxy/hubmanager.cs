using System;
using System.Collections;
using System.Collections.Generic;

namespace dbproxy
{
	public class hubmanager
	{
		public hubmanager()
		{
			hubproxys_uuid = new Dictionary<string, hubproxy> ();
			hubproxys = new Dictionary<juggle.Ichannel, hubproxy> ();
		}

		public hubproxy reg_hub(juggle.Ichannel ch, String uuid)
		{
			hubproxy _hubproxy = new hubproxy (ch);
			hubproxys_uuid.Add (uuid, _hubproxy);
			hubproxys.Add (ch, _hubproxy);

			return _hubproxy;
		}

		public hubproxy get_hub(juggle.Ichannel ch)
		{
			if (hubproxys.ContainsKey (ch)) 
			{
				return hubproxys [ch];
			}

			return null;
		}

		private Dictionary<String, hubproxy> hubproxys_uuid;
		private Dictionary<juggle.Ichannel, hubproxy> hubproxys;

	}
}

