/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class client_call_gate_fast : juggle.Imodule 
    {
        public client_call_gate_fast()
        {
			module_name = "client_call_gate_fast";
        }

        public delegate void refresh_udp_end_pointhandle();
        public event refresh_udp_end_pointhandle onrefresh_udp_end_point;
        public void refresh_udp_end_point(ArrayList _event)
        {
            if(onrefresh_udp_end_point != null)
            {
                onrefresh_udp_end_point();
            }
        }

        public delegate void confirm_create_udp_linkhandle(String argv0);
        public event confirm_create_udp_linkhandle onconfirm_create_udp_link;
        public void confirm_create_udp_link(ArrayList _event)
        {
            if(onconfirm_create_udp_link != null)
            {
                var argv0 = ((String)_event[0]);
                onconfirm_create_udp_link( argv0);
            }
        }

	}
}
