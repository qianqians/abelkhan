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

        public delegate void save_ovject_sucesshandle(Int64 argv0);
        public event save_ovject_sucesshandle onsave_ovject_sucess;
        public void save_ovject_sucess(ArrayList _event)
        {
            if(onsave_ovject_sucess != null)
            {
                var argv0 = ((Int64)_event[0]);
                onsave_ovject_sucess( argv0);
            }
        }

        public delegate void ack_find_objecthandle(Int64 argv0, Hashtable argv1);
        public event ack_find_objecthandle onack_find_object;
        public void ack_find_object(ArrayList _event)
        {
            if(onack_find_object != null)
            {
                var argv0 = ((Int64)_event[0]);
                var argv1 = ((Hashtable)_event[1]);
                onack_find_object( argv0,  argv1);
            }
        }

	}
}
