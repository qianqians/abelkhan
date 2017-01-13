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

	}
}
