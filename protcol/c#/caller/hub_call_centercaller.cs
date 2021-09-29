/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class hub_call_center : juggle.Icaller 
    {
        public hub_call_center(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "hub_call_center";
        }

        public void closed()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("closed", _argv);
        }

    }
}
