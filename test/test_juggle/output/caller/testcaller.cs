/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;
using MsgPack;
using MsgPack.Serialization;

namespace caller
{
    public class test : juggle.Icaller 
    {
        public test(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "test";
        }

        public void test_func(String argv0,Int64 argv1)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            call_module_method("test_func", _argv);
        }

    }
}
