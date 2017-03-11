using System;
using System.Collections.Generic;
using System.Collections;

namespace hub
{
    public class hubmanager
    {
        public hubmanager()
        {
            hubproxys = new Dictionary<string, hubproxy>();
            hubproxys_ch = new Dictionary<juggle.Ichannel, hubproxy>();
        }

        public hubproxy reg_hub(String hub_name, juggle.Ichannel ch)
        {
            hubproxy _proxy = new hubproxy(hub_name, ch);
            hubproxys.Add(hub_name, _proxy);
            hubproxys_ch.Add(ch, _proxy);

            return _proxy;
        }

        public hubproxy get_hub(juggle.Ichannel ch)
        {
            if (hubproxys_ch.ContainsKey(ch))
            {
                return hubproxys_ch[ch];
            }

            return null;
        }

        public void call_hub(string hub_name, string module_name, string func_name, params object[] _argvs)
        {
            if (hubproxys.ContainsKey(hub_name))
            {
                ArrayList _argvs_list = new ArrayList();
                foreach (var o in _argvs)
                {
                    _argvs_list.Add(o);
                }

                hubproxys[hub_name].caller_hub(module_name, func_name, _argvs_list);
            }
        }

        private Dictionary<String, hubproxy> hubproxys;
        private Dictionary<juggle.Ichannel, hubproxy> hubproxys_ch;
    }
}
