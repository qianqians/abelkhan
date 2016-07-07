/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;
using MsgPack;
using MsgPack.Serialization;

namespace caller
{
    public class center : juggle.Icaller 
    {
        public center(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "center";
        }

        public void reg_server(String argv0,String argv1,Int64 argv2,String argv3)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            _argv.Add(argv3);
            call_module_method("reg_server", _argv);
        }

    }
}
