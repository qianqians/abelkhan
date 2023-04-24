using System;
using System.Collections.Generic;
using System.Collections;

namespace Hub
{
    public class HubManager
    {
        public HubProxy current_hubproxy = null;

        private readonly Dictionary<String, HubProxy> hubproxys;
        private readonly Dictionary<Abelkhan.Ichannel, HubProxy> ch_hubproxys;

        public HubManager()
        {
            hubproxys = new Dictionary<string, HubProxy>();
            ch_hubproxys = new Dictionary<Abelkhan.Ichannel, HubProxy>();
        }

        public event Action<HubProxy> on_hubproxy;
        public event Action<HubProxy> on_hubproxy_reconn;
        public void reg_hub(string hub_name, string hub_type, Abelkhan.Ichannel ch)
        {
            HubProxy _proxy = new HubProxy(hub_name, hub_type, ch);
            if (hubproxys.TryGetValue(hub_name, out HubProxy _old_proxy))
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

        public bool get_hub(Abelkhan.Ichannel ch, out HubProxy _proxy)
        {
            return ch_hubproxys.TryGetValue(ch, out _proxy);
        }

        public void hub_be_closed(string hub_name)
        {
        }

        public void call_hub(string hub_name, string func_name, ArrayList _argvs)
        {
            if (hubproxys.TryGetValue(hub_name, out HubProxy _proxy))
            {
                _proxy.caller_hub(func_name, _argvs);
            }
            else
            {
                Log.Log.err("unreg hub:{0}!", hub_name);
            }
        }
    }
}
