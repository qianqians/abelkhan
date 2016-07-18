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

        public delegate void ack_create_persisted_objecthandle(String argv0);
        public event ack_create_persisted_objecthandle onack_create_persisted_object;
        public void ack_create_persisted_object(ArrayList _event)
        {
            if(onack_create_persisted_object != null)
            {
                var argv0 = ((String)_event[0]);
                onack_create_persisted_object( argv0);
            }
        }

        public delegate void ack_updata_persisted_objecthandle(String argv0);
        public event ack_updata_persisted_objecthandle onack_updata_persisted_object;
        public void ack_updata_persisted_object(ArrayList _event)
        {
            if(onack_updata_persisted_object != null)
            {
                var argv0 = ((String)_event[0]);
                onack_updata_persisted_object( argv0);
            }
        }

        public delegate void ack_get_object_infohandle(String argv0, String argv1);
        public event ack_get_object_infohandle onack_get_object_info;
        public void ack_get_object_info(ArrayList _event)
        {
            if(onack_get_object_info != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                onack_get_object_info( argv0,  argv1);
            }
        }

	}
}
