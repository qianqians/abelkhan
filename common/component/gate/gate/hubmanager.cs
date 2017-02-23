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
            hubs_hub_name = new Dictionary<string, hubproxy>();
        }

		public hubproxy reg_hub(juggle.Ichannel ch, string uuid, string hub_name)
		{
			hubproxy _hubproxy = new hubproxy (ch);
			hubs.Add (ch, _hubproxy);
			hubs_uuid.Add (uuid, _hubproxy);
            hubs_hub_name.Add(hub_name, _hubproxy);

            return _hubproxy;
		}

        public hubproxy get_hub(string hub_name)
        {
            if (hubs_hub_name.ContainsKey(hub_name))
            {
                return hubs_hub_name[hub_name];
            }

            return null;
        }

        private Dictionary<juggle.Ichannel, hubproxy> hubs;
		private Dictionary<string, hubproxy> hubs_uuid;
        private Dictionary<string, hubproxy> hubs_hub_name;
    }
}

