using System;
using System.Collections;
using System.Collections.Generic;

namespace gate
{
	public class hubmanager
	{
		public hubmanager()
		{
			hubs = new Dictionary<juggle.Ichannel, hubproxy> ();
			hubs_uuid = new Dictionary<string, hubproxy> ();
		}

		public hubproxy reg_hub(juggle.Ichannel ch, string uuid)
		{
			hubproxy _hubproxy = new hubproxy (ch);
			hubs.Add (ch, _hubproxy);
			hubs_uuid.Add (uuid, _hubproxy);

			return _hubproxy;
		}

		private Dictionary<juggle.Ichannel, hubproxy> hubs;
		private Dictionary<string, hubproxy> hubs_uuid;
	}
}

