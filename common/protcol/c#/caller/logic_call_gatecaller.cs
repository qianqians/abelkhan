/*this caller file is codegen by juggle for c#*/
using System;
using System.Collections;
using System.IO;

namespace caller
{
    public class logic_call_gate : juggle.Icaller 
    {
        public logic_call_gate(juggle.Ichannel _ch) : base(_ch)
        {
            module_name = "logic_call_gate";
        }

        public void reg_logic(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("reg_logic", _argv);
        }

        public void ack_client_get_logic(String argv0,String argv1)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            call_module_method("ack_client_get_logic", _argv);
        }

        public void connect_sucess(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("connect_sucess", _argv);
        }

        public void disconnect_client(String argv0)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            call_module_method("disconnect_client", _argv);
        }

        public void forward_logic_call_client(String argv0,String argv1,String argv2,ArrayList argv3)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            _argv.Add(argv3);
            call_module_method("forward_logic_call_client", _argv);
        }

        public void forward_logic_call_group_client(ArrayList argv0,String argv1,String argv2,ArrayList argv3)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            _argv.Add(argv3);
            call_module_method("forward_logic_call_group_client", _argv);
        }

        public void forward_logic_call_global_client(String argv0,String argv1,ArrayList argv2)
        {
            ArrayList _argv = new ArrayList();
            _argv.Add(argv0);
            _argv.Add(argv1);
            _argv.Add(argv2);
            call_module_method("forward_logic_call_global_client", _argv);
        }

    }
}
