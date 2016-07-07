/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class client_call_gate : juggle.Imodule 
    {
        public client_call_gate()
        {
			module_name = "client_call_gate";
        }

        public delegate void connect_serverhandle(String argv0);
        public event connect_serverhandle onconnect_server;
        public void connect_server(ArrayList _event)
        {
            if(onconnect_server != null)
            {
                var argv0 = ((String)_event[0]);
                onconnect_server( argv0);
            }
        }

        public delegate void cancle_serverhandle();
        public event cancle_serverhandle oncancle_server;
        public void cancle_server(ArrayList _event)
        {
            if(oncancle_server != null)
            {
                oncancle_server();
            }
        }

        public delegate void forward_client_call_logichandle(String argv0, String argv1, String argv2);
        public event forward_client_call_logichandle onforward_client_call_logic;
        public void forward_client_call_logic(ArrayList _event)
        {
            if(onforward_client_call_logic != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                onforward_client_call_logic( argv0,  argv1,  argv2);
            }
        }

        public delegate void heartbeatshandle();
        public event heartbeatshandle onheartbeats;
        public void heartbeats(ArrayList _event)
        {
            if(onheartbeats != null)
            {
                onheartbeats();
            }
        }

	}
}
