/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class dbproxy_call_logic : juggle.Icaller 
    {
        public dbproxy_call_logic(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "dbproxy_call_logic";
        }

        public void reg_logic_sucess()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("reg_logic_sucess", _argv);
        }

        public void ack_create_persisted_object(Int64 argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("ack_create_persisted_object", _argv);
        }

        public void ack_updata_persisted_object(Int64 argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("ack_updata_persisted_object", _argv);
        }

        public void ack_get_object_info(Int64 argv0,String argv1)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            call_module_method("ack_get_object_info", _argv);
        }

    }
}
