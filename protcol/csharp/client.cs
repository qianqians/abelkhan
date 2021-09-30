using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MsgPack.Serialization;

namespace abelkhan
{
/*this enum code is codegen by abelkhan codegen for c#*/

/*this struct code is codegen by abelkhan codegen for c#*/
/*this caller code is codegen by abelkhan codegen for c#*/
/*this cb code is codegen by abelkhan for c#*/
    public class gate_call_client_rsp_cb : abelkhan.Imodule {
        public gate_call_client_rsp_cb(abelkhan.modulemng modules) : base("gate_call_client_rsp_cb")
        {
            modules.reg_module(this);
        }

    }

    public class gate_call_client_caller : abelkhan.Icaller {
        public static gate_call_client_rsp_cb rsp_cb_gate_call_client_handle = null;
        private UInt64 uuid_b84dd831_2e79_3280_a337_a69dd489e75f = RandomUUID.random();

        public gate_call_client_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("gate_call_client", _ch)
        {
            if (rsp_cb_gate_call_client_handle == null)
            {
                rsp_cb_gate_call_client_handle = new rsp_cb_gate_call_client(modules);
            }
        }

        public void ntf_cuuid(string cuuid){
            var _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1 = new ArrayList();
            _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1.Add(cuuid);
            call_module_method("ntf_cuuid", _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1);
        }

        public void call_client(byte[] rpc_argv){
            var _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab = new ArrayList();
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.Add(rpc_argv);
            call_module_method("call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
        }

    }
/*this module code is codegen by abelkhan codegen for c#*/
    public class gate_call_client_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public gate_call_client_module(abelkhan.modulemng _modules) : base("gate_call_client")
        {
            modules = _modules;
            modules.reg_module(this);

            reg_method("ntf_cuuid", ntf_cuuid);
            reg_method("call_client", call_client);
        }

        public event Action<string> on_ntf_cuuid;
        public void ntf_cuuid(ArrayList inArray){
            var _cuuid = (string)inArray[0];
            if (on_ntf_cuuid != null){
                on_ntf_cuuid(_cuuid);
            }
        }

        public event Action<byte[]> on_call_client;
        public void call_client(ArrayList inArray){
            var _rpc_argv = (byte[])inArray[0];
            if (on_call_client != null){
                on_call_client(_rpc_argv);
            }
        }

    }

}
