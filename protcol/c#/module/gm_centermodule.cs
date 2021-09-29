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

        public delegate void close_zonehandle(String argv0, Int64 argv1);
        public event close_zonehandle onclose_zone;
        public void close_zone(ArrayList _event)
        {
            if(onclose_zone != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((Int64)_event[1]);
                onclose_zone( argv0,  argv1);
            }
        }

        public delegate void reloadhandle(String argv0, String argv1);
        public event reloadhandle onreload;
        public void reload(ArrayList _event)
        {
            if(onreload != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                onreload( argv0,  argv1);
            }
        }

	}
}
