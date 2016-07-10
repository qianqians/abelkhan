/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class logic_call_center : juggle.Imodule 
    {
        public logic_call_center()
        {
			module_name = "logic_call_center";
        }

        public delegate void req_get_server_addresshandle(String argv0, Int64 argv1);
        public event req_get_server_addresshandle onreq_get_server_address;
        public void req_get_server_address(ArrayList _event)
        {
            if(onreq_get_server_address != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((Int64)_event[1]);
                onreq_get_server_address( argv0,  argv1);
            }
        }

	}
}
