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
        }

        public hubproxy reg_hub(String hub_name, juggle.Ichannel ch)
        {
            hubproxy _proxy = new hubproxy(hub_name, ch);
            hubproxys.Add(hub_name, _proxy);

            return _proxy;
        }

        public void call_hub(string hub_name, string module_name, string func_name, ArrayList argvs)
        {
            if (hubproxys.ContainsKey(hub_name))
            {
                hubproxys[hub_name].caller_hub(module_name, func_name, argvs);
            }
        }

        private Dictionary<String, hubproxy> hubproxys;
    }
}
