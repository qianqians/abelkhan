using System;
using System.Collections.Generic;
using System.Collections;

namespace hub
{
    public class hubproxy
    {
        public hubproxy(String hub_name, juggle.Ichannel ch)
        {
            name = hub_name;
            _caller = new caller.hub_call_hub(ch);
        }

        public void reg_hub_sucess()
        {
            _caller.reg_hub_sucess();
        }

        public void caller_hub(string module_name, string func_name, ArrayList argvs)
        {
            _caller.hub_call_hub_mothed(module_name, func_name, argvs);
        }

        public string name;
        private caller.hub_call_hub _caller;
    }
}
