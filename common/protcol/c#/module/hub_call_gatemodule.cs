/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class hub_call_gate : juggle.Imodule 
    {
        public hub_call_gate()
        {
			module_name = "hub_call_gate";
        }

        public delegate void reg_hubhandle(String argv0, String argv1);
        public event reg_hubhandle onreg_hub;
        public void reg_hub(ArrayList _event)
        {
            if(onreg_hub != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                onreg_hub( argv0,  argv1);
            }
        }

        public delegate void connect_sucesshandle(String argv0);
        public event connect_sucesshandle onconnect_sucess;
        public void connect_sucess(ArrayList _event)
        {
            if(onconnect_sucess != null)
            {
                var argv0 = ((String)_event[0]);
                onconnect_sucess( argv0);
            }
        }

        public delegate void disconnect_clienthandle(String argv0);
        public event disconnect_clienthandle ondisconnect_client;
        public void disconnect_client(ArrayList _event)
        {
            if(ondisconnect_client != null)
            {
                var argv0 = ((String)_event[0]);
                ondisconnect_client( argv0);
            }
        }

        public delegate void forward_hub_call_clienthandle(String argv0, String argv1, String argv2, ArrayList argv3);
        public event forward_hub_call_clienthandle onforward_hub_call_client;
        public void forward_hub_call_client(ArrayList _event)
        {
            if(onforward_hub_call_client != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((ArrayList)_event[3]);
                onforward_hub_call_client( argv0,  argv1,  argv2,  argv3);
            }
        }

        public delegate void forward_hub_call_group_clienthandle(ArrayList argv0, String argv1, String argv2, ArrayList argv3);
        public event forward_hub_call_group_clienthandle onforward_hub_call_group_client;
        public void forward_hub_call_group_client(ArrayList _event)
        {
            if(onforward_hub_call_group_client != null)
            {
                var argv0 = ((ArrayList)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((ArrayList)_event[3]);
                onforward_hub_call_group_client( argv0,  argv1,  argv2,  argv3);
            }
        }

        public delegate void forward_hub_call_global_clienthandle(String argv0, String argv1, ArrayList argv2);
        public event forward_hub_call_global_clienthandle onforward_hub_call_global_client;
        public void forward_hub_call_global_client(ArrayList _event)
        {
            if(onforward_hub_call_global_client != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((ArrayList)_event[2]);
                onforward_hub_call_global_client( argv0,  argv1,  argv2);
            }
        }

        public delegate void forward_hub_call_client_fasthandle(String argv0, String argv1, String argv2, ArrayList argv3);
        public event forward_hub_call_client_fasthandle onforward_hub_call_client_fast;
        public void forward_hub_call_client_fast(ArrayList _event)
        {
            if(onforward_hub_call_client_fast != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((ArrayList)_event[3]);
                onforward_hub_call_client_fast( argv0,  argv1,  argv2,  argv3);
            }
        }

        public delegate void forward_hub_call_group_client_fasthandle(ArrayList argv0, String argv1, String argv2, ArrayList argv3);
        public event forward_hub_call_group_client_fasthandle onforward_hub_call_group_client_fast;
        public void forward_hub_call_group_client_fast(ArrayList _event)
        {
            if(onforward_hub_call_group_client_fast != null)
            {
                var argv0 = ((ArrayList)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((ArrayList)_event[3]);
                onforward_hub_call_group_client_fast( argv0,  argv1,  argv2,  argv3);
            }
        }

	}
}
