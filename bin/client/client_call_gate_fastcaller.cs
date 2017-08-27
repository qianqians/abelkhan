/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class client_call_gate_fast : juggle.Icaller 
    {
        public client_call_gate_fast(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "client_call_gate_fast";
        }

        public void refresh_udp_end_point()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("refresh_udp_end_point", _argv);
        }

        public void confirm_create_udp_link(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("confirm_create_udp_link", _argv);
        }

    }
}
