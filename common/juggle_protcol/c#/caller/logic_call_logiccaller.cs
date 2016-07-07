/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;
using MsgPack;
using MsgPack.Serialization;

namespace caller
{
    public class logic_call_logic : juggle.Icaller 
    {
        public logic_call_logic(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "logic_call_logic";
        }

        public void reg_logic(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("reg_logic", _argv);
        }

        public void client_call_logic(String argv0,String argv1,String argv2)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            call_module_method("client_call_logic", _argv);
        }

    }
}
