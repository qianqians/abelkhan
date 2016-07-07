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

        public void save_object(Hashtable argv0,Hashtable argv1,Int64 argv2)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            call_module_method("save_object", _argv);
        }

        public void find_object(Hashtable argv0,Hashtable argv1,Int64 argv2)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            call_module_method("find_object", _argv);
        }

    }
}
