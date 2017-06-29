/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class gate_call_client_fast : juggle.Icaller 
    {
        public gate_call_client_fast(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "gate_call_client_fast";
        }

        public void confirm_refresh_udp_end_point()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("confirm_refresh_udp_end_point", _argv);
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
