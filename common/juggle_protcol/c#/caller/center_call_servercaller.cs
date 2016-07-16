/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class center_call_server : juggle.Icaller 
    {
        public center_call_server(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "center_call_server";
        }

        public void reg_server_sucess()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("reg_server_sucess", _argv);
        }

        public void close_server()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("close_server", _argv);
        }

    }
}
