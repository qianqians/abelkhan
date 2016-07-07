/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class gm_center : juggle.Imodule 
    {
        public gm_center()
        {
			module_name = "gm_center";
        }

        public delegate void close_clutterhandle();
        public event close_clutterhandle onclose_clutter;
        public void close_clutter(ArrayList _event)
        {
            if(onclose_clutter != null)
            {
                onclose_clutter();
            }
        }

	}
}
