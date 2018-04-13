/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class hub_call_hub : juggle.Icaller 
    {
        public hub_call_hub(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "hub_call_hub";
        }

        public void reg_hub(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("reg_hub", _argv);
        }

        public void reg_hub_sucess()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("reg_hub_sucess", _argv);
        }

        public void hub_call_hub_mothed(String argv0,String argv1,ArrayList argv2)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            call_module_method("hub_call_hub_mothed", _argv);
        }

    }
}
