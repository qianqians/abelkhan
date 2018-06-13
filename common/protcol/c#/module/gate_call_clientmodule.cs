/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class gate_call_client : juggle.Imodule 
    {
        public gate_call_client()
        {
			module_name = "gate_call_client";
        }

        public delegate void connect_server_sucesshandle();
        public event connect_server_sucesshandle onconnect_server_sucess;
        public void connect_server_sucess(ArrayList _event)
        {
            if(onconnect_server_sucess != null)
            {
                onconnect_server_sucess();
            }
        }

        public delegate void ack_heartbeatshandle();
        public event ack_heartbeatshandle onack_heartbeats;
        public void ack_heartbeats(ArrayList _event)
        {
            if(onack_heartbeats != null)
            {
                onack_heartbeats();
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
