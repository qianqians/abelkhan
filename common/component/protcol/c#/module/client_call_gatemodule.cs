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

        public delegate void connect_serverhandle(String argv0, Int64 argv1);
        public event connect_serverhandle onconnect_server;
        public void connect_server(ArrayList _event)
        {
            if(onconnect_server != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((Int64)_event[1]);
                onconnect_server( argv0,  argv1);
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

        public delegate void get_logichandle();
        public event get_logichandle onget_logic;
        public void get_logic(ArrayList _event)
        {
            if(onget_logic != null)
            {
                onget_logic();
            }
        }

        public delegate void connect_logichandle(String argv0, String argv1);
        public event connect_logichandle onconnect_logic;
        public void connect_logic(ArrayList _event)
        {
            if(onconnect_logic != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                onconnect_logic( argv0,  argv1);
            }
        }

        public delegate void disconnect_logichandle(String argv0, String argv1);
        public event disconnect_logichandle ondisconnect_logic;
        public void disconnect_logic(ArrayList _event)
        {
            if(ondisconnect_logic != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                ondisconnect_logic( argv0,  argv1);
            }
        }

        public delegate void connect_hubhandle(String argv0, String argv1);
        public event connect_hubhandle onconnect_hub;
        public void connect_hub(ArrayList _event)
        {
            if(onconnect_hub != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                onconnect_hub( argv0,  argv1);
            }
        }

        public delegate void disconnect_hubhandle(String argv0, String argv1);
        public event disconnect_hubhandle ondisconnect_hub;
        public void disconnect_hub(ArrayList _event)
        {
            if(ondisconnect_hub != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                ondisconnect_hub( argv0,  argv1);
            }
        }

        public delegate void forward_client_call_logichandle(String argv0, String argv1, String argv2, ArrayList argv3);
        public event forward_client_call_logichandle onforward_client_call_logic;
        public void forward_client_call_logic(ArrayList _event)
        {
            if(onforward_client_call_logic != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((ArrayList)_event[3]);
                onforward_client_call_logic( argv0,  argv1,  argv2,  argv3);
            }
        }

        public delegate void forward_client_call_hubhandle(String argv0, String argv1, String argv2, ArrayList argv3);
        public event forward_client_call_hubhandle onforward_client_call_hub;
        public void forward_client_call_hub(ArrayList _event)
        {
            if(onforward_client_call_hub != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((ArrayList)_event[3]);
                onforward_client_call_hub( argv0,  argv1,  argv2,  argv3);
            }
        }

        public delegate void heartbeatshandle(Int64 argv0);
        public event heartbeatshandle onheartbeats;
        public void heartbeats(ArrayList _event)
        {
            if(onheartbeats != null)
            {
                var argv0 = ((Int64)_event[0]);
                onheartbeats( argv0);
            }
        }

	}
}
