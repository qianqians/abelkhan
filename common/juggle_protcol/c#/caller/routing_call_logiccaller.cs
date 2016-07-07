/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;
using MsgPack;
using MsgPack.Serialization;

namespace caller
{
    public class routing_call_logic : juggle.Icaller 
    {
        public routing_call_logic(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "routing_call_logic";
        }

        public void ack_get_object_server(String argv0,String argv1,Int64 argv2)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            call_module_method("ack_get_object_server", _argv);
        }

    }
}
