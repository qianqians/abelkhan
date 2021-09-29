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

        public void connect_server(String argv0,Int64 argv1)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            call_module_method("connect_server", _argv);
        }

        public void cancle_server()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("cancle_server", _argv);
        }

        public void enable_heartbeats()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("enable_heartbeats", _argv);
        }

        public void disable_heartbeats()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("disable_heartbeats", _argv);
        }

        public void forward_client_call_hub(String argv0,String argv1,String argv2,ArrayList argv3)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            _argv.Add(argv3);
            call_module_method("forward_client_call_hub", _argv);
        }

        public void heartbeats(Int64 argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("heartbeats", _argv);
        }

    }
}
