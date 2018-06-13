/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class gate_call_client : juggle.Icaller 
    {
        public gate_call_client(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "gate_call_client";
        }

        public void connect_server_sucess()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("connect_server_sucess", _argv);
        }

        public void ack_heartbeats()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("ack_heartbeats", _argv);
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
