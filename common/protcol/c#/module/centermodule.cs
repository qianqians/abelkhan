/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class center : juggle.Imodule 
    {
        public center()
        {
			module_name = "center";
        }

        public delegate void reg_serverhandle(String argv0, String argv1, Int64 argv2, String argv3, Int64 argv4);
        public event reg_serverhandle onreg_server;
        public void reg_server(ArrayList _event)
        {
            if(onreg_server != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((Int64)_event[2]);
                var argv3 = ((String)_event[3]);
                var argv4 = ((Int64)_event[4]);
                onreg_server( argv0,  argv1,  argv2,  argv3,  argv4);
            }
        }

	}
}
