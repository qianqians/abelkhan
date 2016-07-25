/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class logic_call_gate : juggle.Imodule 
    {
        public logic_call_gate()
        {
			module_name = "logic_call_gate";
        }

        public delegate void reg_logichandle(String argv0);
        public event reg_logichandle onreg_logic;
        public void reg_logic(ArrayList _event)
        {
            if(onreg_logic != null)
            {
                var argv0 = ((String)_event[0]);
                onreg_logic( argv0);
            }
        }

        public delegate void ack_client_connect_serverhandle(String argv0, String argv1);
        public event ack_client_connect_serverhandle onack_client_connect_server;
        public void ack_client_connect_server(ArrayList _event)
        {
            if(onack_client_connect_server != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                onack_client_connect_server( argv0,  argv1);
            }
        }

        public delegate void forward_logic_call_clienthandle(String argv0, String argv1, String argv2, ArrayList argv3);
        public event forward_logic_call_clienthandle onforward_logic_call_client;
        public void forward_logic_call_client(ArrayList _event)
        {
            if(onforward_logic_call_client != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((ArrayList)_event[3]);
                onforward_logic_call_client( argv0,  argv1,  argv2,  argv3);
            }
        }

        public delegate void forward_logic_call_group_clienthandle(ArrayList argv0, String argv1, String argv2, ArrayList argv3);
        public event forward_logic_call_group_clienthandle onforward_logic_call_group_client;
        public void forward_logic_call_group_client(ArrayList _event)
        {
            if(onforward_logic_call_group_client != null)
            {
                var argv0 = ((ArrayList)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((ArrayList)_event[3]);
                onforward_logic_call_group_client( argv0,  argv1,  argv2,  argv3);
            }
        }

        public delegate void forward_logic_call_global_clienthandle(String argv0, String argv1, ArrayList argv2);
        public event forward_logic_call_global_clienthandle onforward_logic_call_global_client;
        public void forward_logic_call_global_client(ArrayList _event)
        {
            if(onforward_logic_call_global_client != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((ArrayList)_event[2]);
                onforward_logic_call_global_client( argv0,  argv1,  argv2);
            }
        }

	}
}
