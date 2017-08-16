/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class gate_call_client_fast : juggle.Imodule 
    {
        public gate_call_client_fast()
        {
			module_name = "gate_call_client_fast";
        }

        public delegate void confirm_refresh_udp_end_pointhandle();
        public event confirm_refresh_udp_end_pointhandle onconfirm_refresh_udp_end_point;
        public void confirm_refresh_udp_end_point(ArrayList _event)
        {
            if(onconfirm_refresh_udp_end_point != null)
            {
                onconfirm_refresh_udp_end_point();
            }
        }

        public delegate void call_clienthandle(String argv0, String argv1, ArrayList argv2);
        public event call_clienthandle oncall_client;
        public void call_client(ArrayList _event)
        {
            if(oncall_client != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((ArrayList)_event[2]);
                oncall_client( argv0,  argv1,  argv2);
            }
        }

	}
}
