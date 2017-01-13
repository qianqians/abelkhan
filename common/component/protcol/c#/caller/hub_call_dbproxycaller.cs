/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class hub_call_dbproxy : juggle.Icaller 
    {
        public hub_call_dbproxy(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "hub_call_dbproxy";
        }

        public void reg_hub(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("reg_hub", _argv);
        }

        public void create_persisted_object(Hashtable argv0,String argv1)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            call_module_method("create_persisted_object", _argv);
        }

        public void updata_persisted_object(Hashtable argv0,Hashtable argv1,String argv2)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            call_module_method("updata_persisted_object", _argv);
        }

        public void get_object_info(Hashtable argv0,String argv1)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            call_module_method("get_object_info", _argv);
        }

    }
}
