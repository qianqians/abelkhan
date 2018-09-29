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

		public hubproxy reg_hub(juggle.Ichannel ch, String type, String ip, Int64 port, String uuid, int zone_id)
		{
			hubproxy _hubproxy = new hubproxy(ch, zone_id);
			hubproxys.Add(ch, Tuple.Create(type, ip, port, uuid, _hubproxy));

			return _hubproxy;
		}

        public hubproxy get_hub(juggle.Ichannel ch)
        {
            if (hubproxys.ContainsKey(ch))
            {
                return hubproxys[ch].Item5;
            }

            return null;
        }

        public void for_each_hub(Action<hubproxy> func)
		{
			foreach(Tuple<String, String,Int64, String, hubproxy> value in hubproxys.Values)
			{
				func(value.Item5);
			}
		}

        public void hub_closed(juggle.Ichannel ch)
        {
            hubproxys[ch].Item5.is_closed = true;
        }

        public bool checkAllHubClosed()
        {
            foreach (Tuple<String, String, Int64, String, hubproxy> value in hubproxys.Values)
            {
                if (!value.Item5.is_closed)
                {
					log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "{0}:{1}:{2}:{3}", value.Item1, value.Item2, value.Item3, value.Item4);
                    return false;
                }
            }

            return true;
        }

		private Dictionary<juggle.Ichannel, Tuple<String, String,Int64, String, hubproxy> > hubproxys;
	}
}
