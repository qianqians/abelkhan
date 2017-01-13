/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class hub_call_gate : juggle.Imodule 
    {
        public hub_call_gate()
        {
			module_name = "hub_call_gate";
        }

        public delegate void reg_hubhandle(String argv0);
        public event reg_hubhandle onreg_hub;
        public void reg_hub(ArrayList _event)
        {
            if(onreg_hub != null)
            {
                var argv0 = ((String)_event[0]);
                onreg_hub( argv0);
            }
        }

        public delegate void forward_hub_call_group_clienthandle(ArrayList argv0, String argv1, String argv2, ArrayList argv3);
        public event forward_hub_call_group_clienthandle onforward_hub_call_group_client;
        public void forward_hub_call_group_client(ArrayList _event)
        {
            if(onforward_hub_call_group_client != null)
            {
                var argv0 = ((ArrayList)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((ArrayList)_event[3]);
                onforward_hub_call_group_client( argv0,  argv1,  argv2,  argv3);
            }
        }

        public delegate void forward_hub_call_global_clienthandle(String argv0, String argv1, ArrayList argv2);
        public event forward_hub_call_global_clienthandle onforward_hub_call_global_client;
        public void forward_hub_call_global_client(ArrayList _event)
        {
            if(onforward_hub_call_global_client != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((ArrayList)_event[2]);
                onforward_hub_call_global_client( argv0,  argv1,  argv2);
            }
        }

	}
}
