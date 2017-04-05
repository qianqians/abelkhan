/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class center_call_server : juggle.Imodule 
    {
        public center_call_server()
        {
			module_name = "center_call_server";
        }

        public delegate void reg_server_sucesshandle();
        public event reg_server_sucesshandle onreg_server_sucess;
        public void reg_server_sucess(ArrayList _event)
        {
            if(onreg_server_sucess != null)
            {
                onreg_server_sucess();
            }
        }

        public delegate void close_serverhandle();
        public event close_serverhandle onclose_server;
        public void close_server(ArrayList _event)
        {
            if(onclose_server != null)
            {
                onclose_server();
            }
        }

	}
}
