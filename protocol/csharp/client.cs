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
            var _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1 = new List<MsgPack.MessagePackObject>();
            _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1.Add(cuuid);
            call_module_method("gate_call_client_ntf_cuuid", _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1);
        }

        public void call_client(string hub_name, byte[] rpc_argv){
            var _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab = new List<MsgPack.MessagePackObject>();
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.Add(hub_name);
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.Add(rpc_argv);
            call_module_method("gate_call_client_call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
        }

        public void migrate_client_start(string src_hub, string target_hub){
            var _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee = new List<MsgPack.MessagePackObject>();
            _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee.Add(src_hub);
            _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee.Add(target_hub);
            call_module_method("gate_call_client_migrate_client_start", _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee);
        }

        public void migrate_client_done(string src_hub, string target_hub){
            var _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c = new List<MsgPack.MessagePackObject>();
            _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c.Add(src_hub);
            _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c.Add(target_hub);
            call_module_method("gate_call_client_migrate_client_done", _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c);
        }

        public void hub_loss(string hub_name){
            var _argv_90f24099_13d8_3e09_b6fa_6d93a3ae6099 = new List<MsgPack.MessagePackObject>();
            _argv_90f24099_13d8_3e09_b6fa_6d93a3ae6099.Add(hub_name);
            call_module_method("gate_call_client_hub_loss", _argv_90f24099_13d8_3e09_b6fa_6d93a3ae6099);
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
            modules.reg_method("gate_call_client_migrate_client_start", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, migrate_client_start));
            modules.reg_method("gate_call_client_migrate_client_done", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, migrate_client_done));
            modules.reg_method("gate_call_client_hub_loss", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, hub_loss));
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

        public event Action<string, string> on_migrate_client_start;
        public void migrate_client_start(IList<MsgPack.MessagePackObject> inArray){
            var _src_hub = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            var _target_hub = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            if (on_migrate_client_start != null){
                on_migrate_client_start(_src_hub, _target_hub);
            }
        }

        public event Action<string, string> on_migrate_client_done;
        public void migrate_client_done(IList<MsgPack.MessagePackObject> inArray){
            var _src_hub = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            var _target_hub = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            if (on_migrate_client_done != null){
                on_migrate_client_done(_src_hub, _target_hub);
            }
        }

        public event Action<string> on_hub_loss;
        public void hub_loss(IList<MsgPack.MessagePackObject> inArray){
            var _hub_name = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            if (on_hub_loss != null){
                on_hub_loss(_hub_name);
            }
        }

    }

}
