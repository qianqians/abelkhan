/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;
using MsgPack;
using MsgPack.Serialization;

namespace caller
{
    public class center_call_logic : juggle.Icaller 
    {
        public center_call_logic(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "center_call_logic";
        }

        public void dispatch_gate_server(String argv0,Int64 argv1,String argv2)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            call_module_method("dispatch_gate_server", _argv);
        }

        public void ack_get_server_address(String argv0,String argv1,Int64 argv2,String argv3)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            _argv.Add(argv3);
            call_module_method("ack_get_server_address", _argv);
        }

    }
}
