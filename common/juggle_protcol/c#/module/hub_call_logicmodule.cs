/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class hub_call_logic : juggle.Imodule 
    {
        public hub_call_logic()
        {
			module_name = "hub_call_logic";
        }

        public delegate void reg_logic_sucess_and_notify_hub_nominatehandle(String argv0);
        public event reg_logic_sucess_and_notify_hub_nominatehandle onreg_logic_sucess_and_notify_hub_nominate;
        public void reg_logic_sucess_and_notify_hub_nominate(ArrayList _event)
        {
            if(onreg_logic_sucess_and_notify_hub_nominate != null)
            {
                var argv0 = ((String)_event[0]);
                onreg_logic_sucess_and_notify_hub_nominate( argv0);
            }
        }

        public delegate void hub_call_logic_mothedhandle(String argv0, String argv1, String argv2);
        public event hub_call_logic_mothedhandle onhub_call_logic_mothed;
        public void hub_call_logic_mothed(ArrayList _event)
        {
            if(onhub_call_logic_mothed != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                onhub_call_logic_mothed( argv0,  argv1,  argv2);
            }
        }

	}
}
