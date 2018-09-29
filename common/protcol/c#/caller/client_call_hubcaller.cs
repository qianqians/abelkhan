/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class client_call_hub : juggle.Icaller 
    {
        public client_call_hub(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "client_call_hub";
        }

        public void client_connect(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("client_connect", _argv);
        }

        public void call_hub(String argv0,String argv1,String argv2,ArrayList argv3)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            _argv.Add(argv3);
            call_module_method("call_hub", _argv);
        }

    }
}
