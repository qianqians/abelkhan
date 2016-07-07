/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;
using MsgPack;
using MsgPack.Serialization;

namespace caller
{
    public class logic_call_center : juggle.Icaller 
    {
        public logic_call_center(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "logic_call_center";
        }

        public void req_get_server_address(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("req_get_server_address", _argv);
        }

    }
}
