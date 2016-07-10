/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class routing : juggle.Imodule 
    {
        public routing()
        {
			module_name = "routing";
        }

        public delegate void reg_logichandle(String argv0);
        public event reg_logichandle onreg_logic;
        public void reg_logic(ArrayList _event)
        {
            if(onreg_logic != null)
            {
                var argv0 = ((String)_event[0]);
                onreg_logic( argv0);
            }
        }

        public delegate void req_get_object_serverhandle(String argv0, Int64 argv1);
        public event req_get_object_serverhandle onreq_get_object_server;
        public void req_get_object_server(ArrayList _event)
        {
            if(onreq_get_object_server != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((Int64)_event[1]);
                onreq_get_object_server( argv0,  argv1);
            }
        }

	}
}
