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

        public delegate void dispatch_gate_serverhandle(String argv0, Int64 argv1, String argv2);
        public event dispatch_gate_serverhandle ondispatch_gate_server;
        public void dispatch_gate_server(ArrayList _event)
        {
            if(ondispatch_gate_server != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((Int64)_event[1]);
                var argv2 = ((String)_event[2]);
                ondispatch_gate_server( argv0,  argv1,  argv2);
            }
        }

        public delegate void ack_get_server_addresshandle(String argv0, String argv1, Int64 argv2, String argv3);
        public event ack_get_server_addresshandle onack_get_server_address;
        public void ack_get_server_address(ArrayList _event)
        {
            if(onack_get_server_address != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((Int64)_event[2]);
                var argv3 = ((String)_event[3]);
                onack_get_server_address( argv0,  argv1,  argv2,  argv3);
            }
        }

	}
}
