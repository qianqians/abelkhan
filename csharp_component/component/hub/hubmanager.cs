using System;
using System.Collections.Generic;
using System.Collections;

namespace hub
{
    public class Hubmanager
    {
        public Hubproxy current_hubproxy = null;

        private readonly Dictionary<String, Hubproxy> hubproxys;
        private readonly Dictionary<abelkhan.Ichannel, Hubproxy> ch_hubproxys;

        public Hubmanager()
        {
            hubproxys = new Dictionary<string, Hubproxy>();
            ch_hubproxys = new Dictionary<abelkhan.Ichannel, Hubproxy>();
        }

        public event Action<Hubproxy> on_hubproxy;
        public event Action<Hubproxy> on_hubproxy_reconn;
        public void reg_hub(string hub_name, string hub_type, abelkhan.Ichannel ch)
        {
            Hubproxy _proxy = new Hubproxy(hub_name, hub_type, ch);
            if (hubproxys.TryGetValue(hub_name, out Hubproxy _old_proxy))
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

            lock (Hub.add_chs)
            {
                Hub.add_chs.Add(ch);
            }
        }

        public bool get_hub(abelkhan.Ichannel ch, out Hubproxy _proxy)
        {
            return ch_hubproxys.TryGetValue(ch, out _proxy);
        }

        public void hub_be_closed(string hub_name)
        {
        }

        public void call_hub(string hub_name, string func_name, ArrayList _argvs)
        {
            if (hubproxys.TryGetValue(hub_name, out Hubproxy _proxy))
            {
                _proxy.caller_hub(func_name, _argvs);
            }
            else
            {
                log.Log.err("unreg hub:{0}!", hub_name);
            }
        }
    }
}
