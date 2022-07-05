using System;
using System.Collections.Generic;
using System.Collections;

namespace hub
{
    public class hubmanager
    {
        public hubproxy current_hubproxy = null;

        private Dictionary<String, hubproxy> wait_destory_hubproxys;
        private Dictionary<String, hubproxy> hubproxys;
        private Dictionary<abelkhan.Ichannel, hubproxy> ch_hubproxys;

        public hubmanager()
        {
            wait_destory_hubproxys = new Dictionary<string, hubproxy>();
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
                if (wait_destory_hubproxys.Remove(hub_name, out hubproxy t_destory_hubproxy))
                {
                    lock (hub.remove_chs)
                    {
                        hub.remove_chs.Add(t_destory_hubproxy._ch);
                    }

                    ch_hubproxys.Remove(t_destory_hubproxy._ch);
                }
                wait_destory_hubproxys.Add(hub_name, _old_proxy);
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
            if (ch_hubproxys.TryGetValue(ch, out hubproxy _proxy))
            {
                return _proxy;
            }
            return null;
        }

        public event Action<string, string> on_hub_closed;
        public void hub_be_closed(string hub_name)
        {
            if (wait_destory_hubproxys.TryGetValue(hub_name, out hubproxy _old_proxy))
            {
                lock (hub.remove_chs)
                {
                    hub.remove_chs.Add(_old_proxy._ch);
                }

                ch_hubproxys.Remove(_old_proxy._ch);
                wait_destory_hubproxys.Remove(hub_name);
            }
            else
            {
                if (hubproxys.Remove(hub_name, out hubproxy _proxy))
                {
                    lock (hub.remove_chs)
                    {
                        hub.remove_chs.Add(_proxy._ch);
                    }

                    ch_hubproxys.Remove(_proxy._ch);
                    on_hub_closed?.Invoke(_proxy.name, _proxy.type);
                }
            }
        }

        public void call_hub(string hub_name, string func_name, ArrayList _argvs)
        {
            if (hubproxys.TryGetValue(hub_name, out hubproxy _proxy))
            {
                ArrayList _argvs_list = new ArrayList();
                foreach (var o in _argvs)
                {
                    _argvs_list.Add(o);
                }

                _proxy.caller_hub(func_name, _argvs_list);
            }
            else
            {
                log.log.err("unreg hub:{0}!", hub_name);
            }
        }
    }
}
