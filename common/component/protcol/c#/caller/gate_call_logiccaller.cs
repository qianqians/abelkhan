/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class gate_call_logic : juggle.Icaller 
    {
        public gate_call_logic(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "gate_call_logic";
        }

        public void reg_logic_sucess()
        {
            ArrayList _argv = new ArrayList();
            call_module_method("reg_logic_sucess", _argv);
        }

        public void client_get_logic(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("client_get_logic", _argv);
        }

        public void client_connect(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("client_connect", _argv);
        }

        public void client_disconnect(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("client_disconnect", _argv);
        }

        public void client_exception(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("client_exception", _argv);
        }

        public void client_call_logic(String argv0,String argv1,String argv2,ArrayList argv3)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            _argv.Add(argv3);
            call_module_method("client_call_logic", _argv);
        }

    }
}
