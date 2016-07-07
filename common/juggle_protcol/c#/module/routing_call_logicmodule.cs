/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class routing_call_logic : juggle.Imodule 
    {
        public routing_call_logic()
        {
			module_name = "routing_call_logic";
        }

        public delegate void ack_get_object_serverhandle(String argv0, String argv1, Int64 argv2);
        public event ack_get_object_serverhandle onack_get_object_server;
        public void ack_get_object_server(ArrayList _event)
        {
            if(onack_get_object_server != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((Int64)_event[2]);
                onack_get_object_server( argv0,  argv1,  argv2);
            }
        }

	}
}
