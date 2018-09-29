using System;
using System.Collections.Generic;

namespace center
{
    class svrimpl
    {
        public svrimpl(juggle.Ichannel ch, String _type)
        {
            type = _type;
            is_closed = false;

            caller = new caller.center_call_server(ch);
        }

        public void close_server()
        {
            caller.close_server();
        }

        public string type;
        public bool is_closed;

        public caller.center_call_server caller;
    }

    class zone
    {
        public zone()
        {
            svrproxys = new Dictionary<juggle.Ichannel, svrimpl>();
            hubproxys = new Dictionary<juggle.Ichannel, svrimpl>();

            is_closed = false;
        }

        public void reg_svr(juggle.Ichannel ch, String type)
        {
            svrimpl _svrimpl = new svrimpl(ch, type);

            svrproxys.Add(ch, _svrimpl);

            if (type == "dbproxy")
            {
                _dbproxy = _svrimpl;
            }

            if (type == "hub")
            {
                hubproxys.Add(ch, _svrimpl);
            }
        }

        public void hub_closed(juggle.Ichannel ch)
        {
            hubproxys[ch].is_closed = true;
        }

        public void for_each_svr(Action<svrimpl> func)
        {
            foreach (svrimpl value in svrproxys.Values)
            {
                func(value);
            }
        }

        public void for_each_svr_ch(Action<juggle.Ichannel> func)
        {
            foreach (juggle.Ichannel ch in svrproxys.Keys)
            {
                func(ch);
            }
        }

        public void for_each_hub_ch(Action<juggle.Ichannel> func)
        {
            foreach (juggle.Ichannel ch in hubproxys.Keys)
            {
                func(ch);
            }
        }

        public bool checkAllHubClosed()
        {
            foreach (svrimpl value in hubproxys.Values)
            {
                if (!value.is_closed)
                {
                    return false;
                }
            }

            return true;
        }

        public void close_db()
        {
            _dbproxy.close_server();
        }

        public bool is_closed;

        private svrimpl _dbproxy;
        private Dictionary<juggle.Ichannel, svrimpl> svrproxys;
        private Dictionary<juggle.Ichannel, svrimpl> hubproxys;

    }

    class clutter
    {
        public clutter()
        {
            zones = new Dictionary<Int64, zone>();
        }

        public zone get_zone(int zone_id)
        {
            if (!zones.ContainsKey(zone_id))
            {
                zones.Add(zone_id, new zone());
            }

            return zones[zone_id];
        }

        private Dictionary<Int64, zone> zones;
    }
}
