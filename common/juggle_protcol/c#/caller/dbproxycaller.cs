/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;
using MsgPack;
using MsgPack.Serialization;

namespace caller
{
    public class dbproxy : juggle.Icaller 
    {
        public dbproxy(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "dbproxy";
        }

        public void reg_logic(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("reg_logic", _argv);
        }

        public void save_object(String argv0,String argv1,Int64 argv2)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            call_module_method("save_object", _argv);
        }

        public void find_object(String argv0,Int64 argv1)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            call_module_method("find_object", _argv);
        }

        public void logic_closed()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("logic_closed", _argv);
        }

    }
}
