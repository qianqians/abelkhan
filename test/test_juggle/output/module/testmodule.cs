/*this module file is codegen by juggle*/
using System;
using System.Collections;
using System.Collections.Generic;
using MsgPack;
using MsgPack.Serialization;

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
        public void test_func(IList<MsgPack.MessagePackObject> _event)
        {
            if(ontest_func != null)
            {
                var argv0 = ((MsgPack.MessagePackObject)_event[0]).AsString();
                var argv1 = ((MsgPack.MessagePackObject)_event[1]).AsInt64();
                ontest_func( argv0,  argv1);
            }
        }

	}
}
