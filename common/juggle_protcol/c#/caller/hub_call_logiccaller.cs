/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;
using MsgPack;
using MsgPack.Serialization;

namespace caller
{
    public class hub_call_logic : juggle.Icaller 
    {
        public hub_call_logic(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "hub_call_logic";
        }

        public void hub_call_logic(String argv0,String argv1,String argv2)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            call_module_method("hub_call_logic", _argv);
        }

    }
}
