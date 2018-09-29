/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class client_call_hub : juggle.Imodule 
    {
        public client_call_hub()
        {
			module_name = "client_call_hub";
        }

        public delegate void client_connecthandle(String argv0);
        public event client_connecthandle onclient_connect;
        public void client_connect(ArrayList _event)
        {
            if(onclient_connect != null)
            {
                var argv0 = ((String)_event[0]);
                onclient_connect( argv0);
            }
        }

        public delegate void call_hubhandle(String argv0, String argv1, String argv2, ArrayList argv3);
        public event call_hubhandle oncall_hub;
        public void call_hub(ArrayList _event)
        {
            if(oncall_hub != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((ArrayList)_event[3]);
                oncall_hub( argv0,  argv1,  argv2,  argv3);
            }
        }

	}
}
