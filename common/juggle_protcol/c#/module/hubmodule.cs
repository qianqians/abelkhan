/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class hub : juggle.Imodule 
    {
        public hub()
        {
			module_name = "hub";
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

        public delegate void logic_call_hub_mothedhandle(String argv0, String argv1, ArrayList argv2);
        public event logic_call_hub_mothedhandle onlogic_call_hub_mothed;
        public void logic_call_hub_mothed(ArrayList _event)
        {
            if(onlogic_call_hub_mothed != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((ArrayList)_event[2]);
                onlogic_call_hub_mothed( argv0,  argv1,  argv2);
            }
        }

	}
}
