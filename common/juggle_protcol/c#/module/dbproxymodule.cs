/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class dbproxy : juggle.Imodule 
    {
        public dbproxy()
        {
			module_name = "dbproxy";
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

        public delegate void save_objecthandle(String argv0, String argv1, Int64 argv2);
        public event save_objecthandle onsave_object;
        public void save_object(ArrayList _event)
        {
            if(onsave_object != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((String)_event[1]);
                var argv2 = ((Int64)_event[2]);
                onsave_object( argv0,  argv1,  argv2);
            }
        }

        public delegate void find_objecthandle(String argv0, Int64 argv1);
        public event find_objecthandle onfind_object;
        public void find_object(ArrayList _event)
        {
            if(onfind_object != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((Int64)_event[1]);
                onfind_object( argv0,  argv1);
            }
        }

	}
}
