/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;
using MsgPack;
using MsgPack.Serialization;

namespace caller
{
    public class routing : juggle.Icaller 
    {
        public routing(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "routing";
        }

        public void reg_logic(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("reg_logic", _argv);
        }

        public void req_get_object_server(String argv0,Int64 argv1)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            call_module_method("req_get_object_server", _argv);
        }

    }
}
