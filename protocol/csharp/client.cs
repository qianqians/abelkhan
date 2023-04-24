using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MsgPack.Serialization;

namespace Abelkhan
{
/*this enum code is codegen by Abelkhan codegen for c#*/

/*this struct code is codegen by Abelkhan codegen for c#*/
/*this caller code is codegen by Abelkhan codegen for c#*/
/*this cb code is codegen by Abelkhan for c#*/
    public class gate_call_client_rsp_cb : Abelkhan.Imodule {
        public gate_call_client_rsp_cb(Abelkhan.modulemng modules) : base("gate_call_client_rsp_cb")
        {
        }

    }

    public class gate_call_client_caller : Abelkhan.Icaller {
        public static gate_call_client_rsp_cb rsp_cb_gate_call_client_handle = null;
        private Int32 uuid_b84dd831_2e79_3280_a337_a69dd489e75f = (Int32)RandomUUID.random();

        public gate_call_client_caller(Abelkhan.Ichannel _ch, Abelkhan.modulemng modules) : base("gate_call_client", _ch)
        {
            if (rsp_cb_gate_call_client_handle == null)
            {
                rsp_cb_gate_call_client_handle = new gate_call_client_rsp_cb(modules);
            }
        }

        public void ntf_cuuid(string cuuid){
            var _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1 = new ArrayList();
            _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1.Add(cuuid);
            call_module_method("gate_call_client_ntf_cuuid", _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1);
        }

        public void call_client(string hub_name, byte[] rpc_argv){
            var _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab = new ArrayList();
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.Add(hub_name);
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.Add(rpc_argv);
            call_module_method("gate_call_client_call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
        }

    }
/*this module code is codegen by Abelkhan codegen for c#*/
    public class gate_call_client_module : Abelkhan.Imodule {
        private Abelkhan.modulemng modules;
        public gate_call_client_module(Abelkhan.modulemng _modules) : base("gate_call_client")
        {
            modules = _modules;
            modules.reg_method("gate_call_client_ntf_cuuid", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, ntf_cuuid));
            modules.reg_method("gate_call_client_call_client", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, call_client));
        }

        public event Action<string> on_ntf_cuuid;
        public void ntf_cuuid(IList<MsgPack.MessagePackObject> inArray){
            var _cuuid = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            if (on_ntf_cuuid != null){
                on_ntf_cuuid(_cuuid);
            }
        }

        public event Action<string, byte[]> on_call_client;
        public void call_client(IList<MsgPack.MessagePackObject> inArray){
            var _hub_name = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            var _rpc_argv = ((MsgPack.MessagePackObject)inArray[1]).AsBinary();
            if (on_call_client != null){
                on_call_client(_hub_name, _rpc_argv);
            }
        }

    }

}
