/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class dbproxy_call_hub : juggle.Imodule 
    {
        public dbproxy_call_hub()
        {
			module_name = "dbproxy_call_hub";
        }

        public delegate void reg_hub_sucesshandle();
        public event reg_hub_sucesshandle onreg_hub_sucess;
        public void reg_hub_sucess(ArrayList _event)
        {
            if(onreg_hub_sucess != null)
            {
                onreg_hub_sucess();
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

        public delegate void ack_get_object_counthandle(String argv0, Int64 argv1);
        public event ack_get_object_counthandle onack_get_object_count;
        public void ack_get_object_count(ArrayList _event)
        {
            if(onack_get_object_count != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((Int64)_event[1]);
                onack_get_object_count( argv0,  argv1);
            }
        }

        public delegate void ack_get_object_infohandle(String argv0, ArrayList argv1);
        public event ack_get_object_infohandle onack_get_object_info;
        public void ack_get_object_info(ArrayList _event)
        {
            if(onack_get_object_info != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((ArrayList)_event[1]);
                onack_get_object_info( argv0,  argv1);
            }
        }

        public delegate void ack_get_object_info_endhandle(String argv0);
        public event ack_get_object_info_endhandle onack_get_object_info_end;
        public void ack_get_object_info_end(ArrayList _event)
        {
            if(onack_get_object_info_end != null)
            {
                var argv0 = ((String)_event[0]);
                onack_get_object_info_end( argv0);
            }
        }

	}
}
