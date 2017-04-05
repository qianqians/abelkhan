/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class hub_call_hub : juggle.Imodule 
    {
        public hub_call_hub()
        {
			module_name = "hub_call_hub";
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

        public delegate void reg_hub_sucesshandle();
        public event reg_hub_sucesshandle onreg_hub_sucess;
        public void reg_hub_sucess(ArrayList _event)
        {
            if(onreg_hub_sucess != null)
            {
                onreg_hub_sucess();
            }
        }

        public delegate void hub_call_hub_mothedhandle(String argv0, String argv1, ArrayList argv2);
        public event hub_call_hub_mothedhandle onhub_call_hub_mothed;
        public void hub_call_hub_mothed(ArrayList _event)
        {
            if(onhub_call_hub_mothed != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((ArrayList)_event[2]);
                onhub_call_hub_mothed( argv0,  argv1,  argv2);
            }
        }

	}
}
