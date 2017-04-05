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

        public delegate void confirm_gmhandle(String argv0);
        public event confirm_gmhandle onconfirm_gm;
        public void confirm_gm(ArrayList _event)
        {
            if(onconfirm_gm != null)
            {
                var argv0 = ((String)_event[0]);
                onconfirm_gm( argv0);
            }
        }

        public delegate void close_clutterhandle(String argv0);
        public event close_clutterhandle onclose_clutter;
        public void close_clutter(ArrayList _event)
        {
            if(onclose_clutter != null)
            {
                var argv0 = ((String)_event[0]);
                onclose_clutter( argv0);
            }
        }

	}
}
