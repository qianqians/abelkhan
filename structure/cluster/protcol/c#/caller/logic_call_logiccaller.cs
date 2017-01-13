/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class logic_call_logic : juggle.Icaller 
    {
        public logic_call_logic(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "logic_call_logic";
        }

        public void reg_logic(String argv0,String argv1)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            call_module_method("reg_logic", _argv);
        }

        public void ack_reg_logic(String argv0,String argv1)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            call_module_method("ack_reg_logic", _argv);
        }

        public void logic_call_logic_mothed(String argv0,String argv1,ArrayList argv2)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            call_module_method("logic_call_logic_mothed", _argv);
        }

    }
}
