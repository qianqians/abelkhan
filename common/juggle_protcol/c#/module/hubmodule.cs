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

        public delegate void logic_call_hubhandle(String argv0, String argv1, String argv2);
        public event logic_call_hubhandle onlogic_call_hub;
        public void logic_call_hub(ArrayList _event)
        {
            if(onlogic_call_hub != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                onlogic_call_hub( argv0,  argv1,  argv2);
            }
        }

	}
}
