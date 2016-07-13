/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class dbproxy_call_logic : juggle.Imodule 
    {
        public dbproxy_call_logic()
        {
			module_name = "dbproxy_call_logic";
        }

        public delegate void reg_logic_sucesshandle();
        public event reg_logic_sucesshandle onreg_logic_sucess;
        public void reg_logic_sucess(ArrayList _event)
        {
            if(onreg_logic_sucess != null)
            {
                onreg_logic_sucess();
            }
        }

        public delegate void save_object_sucesshandle(Int64 argv0);
        public event save_object_sucesshandle onsave_object_sucess;
        public void save_object_sucess(ArrayList _event)
        {
            if(onsave_object_sucess != null)
            {
                var argv0 = ((Int64)_event[0]);
                onsave_object_sucess( argv0);
            }
        }

        public delegate void ack_find_objecthandle(Int64 argv0, String argv1);
        public event ack_find_objecthandle onack_find_object;
        public void ack_find_object(ArrayList _event)
        {
            if(onack_find_object != null)
            {
                var argv0 = ((Int64)_event[0]);
                var argv1 = ((String)_event[1]);
                onack_find_object( argv0,  argv1);
            }
        }

	}
}
