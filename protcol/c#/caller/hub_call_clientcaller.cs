/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class hub_call_client : juggle.Icaller 
    {
        public hub_call_client(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "hub_call_client";
        }

        public void call_client(String argv0,String argv1,ArrayList argv2)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            call_module_method("call_client", _argv);
        }

    }
}
