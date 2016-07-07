/*this module file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace module
{
    public class test : juggle.Imodule 
    {
        public test()
        {
			module_name = "test";
        }

        public delegate void test_funchandle(String argv0, Int64 argv1);
        public event test_funchandle ontest_func;
        public void test_func(ArrayList _event)
        {
            if(ontest_func != null)
            {
                var argv0 = ((String)_event[0]);
                var argv1 = ((Int64)_event[1]);
                ontest_func( argv0,  argv1);
            }
        }

	}
}
