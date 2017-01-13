/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class gate_call_hub : juggle.Icaller 
    {
        public gate_call_hub(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "gate_call_hub";
        }

        public void reg_hub_sucess()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("reg_hub_sucess", _argv);
        }

    }
}
