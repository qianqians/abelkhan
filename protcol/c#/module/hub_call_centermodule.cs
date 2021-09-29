/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class hub_call_center : juggle.Imodule 
    {
        public hub_call_center()
        {
			module_name = "hub_call_center";
        }

        public delegate void closedhandle();
        public event closedhandle onclosed;
        public void closed(ArrayList _event)
        {
            if(onclosed != null)
            {
                onclosed();
            }
        }

	}
}
