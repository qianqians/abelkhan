/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class gate_call_hub : juggle.Imodule 
    {
        public gate_call_hub()
        {
			module_name = "gate_call_hub";
        }

        public delegate void reg_hub_sucesshandle();
        public event reg_hub_sucesshandle onreg_hub_sucess;
        public void reg_hub_sucess(ArrayList _event)
        {
            if(onreg_hub_sucess != null)
            {
                onreg_hub_sucess();
            }
        }

        public delegate void client_reg_hubhandle(String argv0);
        public event client_reg_hubhandle onclient_reg_hub;
        public void client_reg_hub(ArrayList _event)
        {
            if(onclient_reg_hub != null)
            {
                var argv0 = ((String)_event[0]);
                onclient_reg_hub( argv0);
            }
        }

        public delegate void client_unreg_hubhandle(String argv0);
        public event client_unreg_hubhandle onclient_unreg_hub;
        public void client_unreg_hub(ArrayList _event)
        {
            if(onclient_unreg_hub != null)
            {
                var argv0 = ((String)_event[0]);
                onclient_unreg_hub( argv0);
            }
        }

        public delegate void client_disconnecthandle(String argv0);
        public event client_disconnecthandle onclient_disconnect;
        public void client_disconnect(ArrayList _event)
        {
            if(onclient_disconnect != null)
            {
                var argv0 = ((String)_event[0]);
                onclient_disconnect( argv0);
            }
        }

        public delegate void client_exceptionhandle(String argv0);
        public event client_exceptionhandle onclient_exception;
        public void client_exception(ArrayList _event)
        {
            if(onclient_exception != null)
            {
                var argv0 = ((String)_event[0]);
                onclient_exception( argv0);
            }
        }

        public delegate void client_call_hubhandle(String argv0, String argv1, String argv2, ArrayList argv3);
        public event client_call_hubhandle onclient_call_hub;
        public void client_call_hub(ArrayList _event)
        {
            if(onclient_call_hub != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((ArrayList)_event[3]);
                onclient_call_hub( argv0,  argv1,  argv2,  argv3);
            }
        }

	}
}
