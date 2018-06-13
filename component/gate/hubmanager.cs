using System;
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
			hubproxy _hubproxy = new hubproxy (ch, uuid, hub_name);
			hubs[ch] = _hubproxy;
			hubs_uuid[uuid] = _hubproxy;
            hubs_hub_name[hub_name] = _hubproxy;

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

        public hubproxy get_hub(juggle.Ichannel ch)
        {
            if (hubs.ContainsKey(ch))
            {
                return hubs[ch];
            }

            return null;
        }

        public void for_each(Action<hubproxy> cb)
        {
            foreach (var _proxy in hubs_hub_name.Values)
            {
                cb(_proxy);
            }
        }

        private Dictionary<juggle.Ichannel, hubproxy> hubs;
		private Dictionary<string, hubproxy> hubs_uuid;
        private Dictionary<string, hubproxy> hubs_hub_name;
    }
}

