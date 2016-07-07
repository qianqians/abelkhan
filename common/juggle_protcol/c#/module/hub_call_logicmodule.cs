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

        public delegate void hub_call_logichandle(String argv0, String argv1, String argv2);
        public event hub_call_logichandle onhub_call_logic;
        public void hub_call_logic(ArrayList _event)
        {
            if(onhub_call_logic != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                onhub_call_logic( argv0,  argv1,  argv2);
            }
        }

	}
}
