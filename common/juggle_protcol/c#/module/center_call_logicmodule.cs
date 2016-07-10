/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class center_call_logic : juggle.Imodule 
    {
        public center_call_logic()
        {
			module_name = "center_call_logic";
        }

        public delegate void ack_get_server_addresshandle(Boolean argv0, String argv1, String argv2, Int64 argv3, String argv4, Int64 argv5);
        public event ack_get_server_addresshandle onack_get_server_address;
        public void ack_get_server_address(ArrayList _event)
        {
            if(onack_get_server_address != null)
            {
                var argv0 = ((Boolean)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((Int64)_event[3]);
                var argv4 = ((String)_event[4]);
                var argv5 = ((Int64)_event[5]);
                onack_get_server_address( argv0,  argv1,  argv2,  argv3,  argv4,  argv5);
            }
        }

	}
}
