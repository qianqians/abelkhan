/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class gate_call_logic : juggle.Imodule 
    {
        public gate_call_logic()
        {
			module_name = "gate_call_logic";
        }

        public delegate void client_connecthandle(String argv0);
        public event client_connecthandle onclient_connect;
        public void client_connect(ArrayList _event)
        {
            if(onclient_connect != null)
            {
                var argv0 = ((String)_event[0]);
                onclient_connect( argv0);
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

        public delegate void client_call_logichandle(String argv0, String argv1, String argv2, String argv3);
        public event client_call_logichandle onclient_call_logic;
        public void client_call_logic(ArrayList _event)
        {
            if(onclient_call_logic != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((String)_event[2]);
                var argv3 = ((String)_event[3]);
                onclient_call_logic( argv0,  argv1,  argv2,  argv3);
            }
        }

	}
}
