/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class center_call_hub : juggle.Icaller 
    {
        public center_call_hub(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "center_call_hub";
        }

        public void distribute_server_address(String argv0,String argv1,Int64 argv2,String argv3)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            _argv.Add(argv3);
            call_module_method("distribute_server_address", _argv);
        }

        public void reload(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("reload", _argv);
        }

    }
}
