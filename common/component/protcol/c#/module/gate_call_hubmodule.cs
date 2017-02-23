/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class gate_call_hub : juggle.Imodule 
    {
        public gate_call_hub()
        {
			module_name = "gate_call_hub";
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

        public delegate void client_call_hubhandle(String argv0, String argv1, String argv2, ArrayList argv3);
        public event client_call_hubhandle onclient_call_hub;
        public void client_call_hub(ArrayList _event)
        {
            if(onclient_call_hub != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((ArrayList)_event[3]);
                onclient_call_hub( argv0,  argv1,  argv2,  argv3);
            }
        }

	}
}
