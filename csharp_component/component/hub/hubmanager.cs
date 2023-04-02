using System;
using System.Collections.Generic;
using System.Collections;

namespace hub
{
    public class hubmanager
    {
        public hubproxy current_hubproxy = null;

        private readonly Dictionary<String, hubproxy> hubproxys;
        private readonly Dictionary<abelkhan.Ichannel, hubproxy> ch_hubproxys;

        public hubmanager()
        {
            hubproxys = new Dictionary<string, hubproxy>();
            ch_hubproxys = new Dictionary<abelkhan.Ichannel, hubproxy>();
        }

        public event Action<hubproxy> on_hubproxy;
        public event Action<hubproxy> on_hubproxy_reconn;
        public void reg_hub(string hub_name, string hub_type, abelkhan.Ichannel ch)
        {
            hubproxy _proxy = new hubproxy(hub_name, hub_type, ch);
            if (hubproxys.TryGetValue(hub_name, out hubproxy _old_proxy))
            {
                hubproxys[hub_name] = _proxy;
                on_hubproxy_reconn?.Invoke(_proxy);
            }
            else
            {
                hubproxys.Add(hub_name, _proxy);
                on_hubproxy?.Invoke(_proxy);
            }
            ch_hubproxys[ch] = _proxy;

            lock (hub.add_chs)
            {
                hub.add_chs.Add(ch);
            }
        }

        public hubproxy get_hub(abelkhan.Ichannel ch)
        {
            ch_hubproxys.TryGetValue(ch, out hubproxy _proxy);
            return _proxy;
        }

        public void hub_be_closed(string hub_name)
        {
        }

        public void call_hub(string hub_name, string func_name, ArrayList _argvs)
        {
            if (hubproxys.TryGetValue(hub_name, out hubproxy _proxy))
            {
                _proxy.caller_hub(func_name, _argvs);
            }
            else
            {
                log.log.err("unreg hub:{0}!", hub_name);
            }
        }
    }
}
