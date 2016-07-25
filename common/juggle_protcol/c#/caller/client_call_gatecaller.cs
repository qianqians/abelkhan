/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class client_call_gate : juggle.Icaller 
    {
        public client_call_gate(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "client_call_gate";
        }

        public void connect_server(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("connect_server", _argv);
        }

        public void cancle_server()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("cancle_server", _argv);
        }

        public void forward_client_call_logic(String argv0,String argv1,ArrayList argv2)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            call_module_method("forward_client_call_logic", _argv);
        }

        public void heartbeats()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("heartbeats", _argv);
        }

    }
}
