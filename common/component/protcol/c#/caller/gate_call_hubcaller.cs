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

        public void client_call_hub(String argv0,String argv1,String argv2,ArrayList argv3)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            _argv.Add(argv3);
            call_module_method("client_call_hub", _argv);
        }

    }
}
