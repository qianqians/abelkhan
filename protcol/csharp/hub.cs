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
    public class hub_call_hub_reg_hub_cb
    {
        private UInt64 cb_uuid;
        private hub_call_hub_rsp_cb module_rsp_cb;

        public hub_call_hub_reg_hub_cb(UInt64 _cb_uuid, hub_call_hub_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<> on_reg_hub_cb;
        public event Action<> on_reg_hub_err;
        public event Action on_reg_hub_timeout;

        public hub_call_hub_reg_hub_cb callBack(Action<> cb, Action<> err)
        {
            on_reg_hub_cb += cb;
            on_reg_hub_err += err;
            return this;
        }

        void timeout(Uint64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.reg_hub_timeout(cb_uuid);
            });
            on_reg_hub_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_reg_hub_cb != null)
            {
                on_reg_hub_cb();
            }
        }

        public void call_err()
        {
            if (on_reg_hub_err != null)
            {
                on_reg_hub_err();
            }
        }

        public void call_timeout()
        {
            if (on_reg_hub_timeout != null)
            {
                on_reg_hub_timeout();
            }
        }

    }

/*this cb code is codegen by abelkhan for c#*/
    public class hub_call_hub_rsp_cb : abelkhan.Imodule {
        public Dictionary<UInt64, hub_call_hub_reg_hub_cb> map_reg_hub;
        public hub_call_hub_rsp_cb(abelkhan.modulemng modules) : base("hub_call_hub_rsp_cb")
        {
            modules.reg_module(this);
            map_reg_hub = new Dictionary<UInt64, hub_call_hub_reg_hub_cb>();
            reg_method("reg_hub_rsp", reg_hub_rsp);
            reg_method("reg_hub_err", reg_hub_err);
        }

        public void reg_hub_rsp(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_reg_hub_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void reg_hub_err(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_reg_hub_cb(uuid);
            if (rsp != null)
            {
            rsp.call_err();
            }
        }

        public void reg_hub_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_reg_hub_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private hub_call_hub_reg_hub_cb try_get_and_del_reg_hub_cb(UInt64 uuid){
            lock(map_reg_hub)
            {                var rsp = map_reg_hub[uuid];
                map_reg_hub.Remove(uuid);
                return rsp;
            }
        }

    }

    public class hub_call_hub_caller : abelkhan.Icaller {
        public static hub_call_hub_rsp_cb rsp_cb_hub_call_hub_handle = null;
        private UInt64 uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f = RandomUUID.random();

        public hub_call_hub_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("hub_call_hub", _ch)
        {
            if (rsp_cb_hub_call_hub_handle == null)
            {
                rsp_cb_hub_call_hub_handle = new rsp_cb_hub_call_hub(modules);
            }
        }

        public hub_call_hub_reg_hub_cb reg_hub(string hub_name, string hub_type){
            Interlocked.Increment(ref uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f);
            var uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = uuid;

            var _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = new ArrayList();
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106);
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(hub_name);
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(hub_type);
            call_module_method("reg_hub", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);

            var cb_reg_hub_obj = new hub_call_hub_reg_hub_cb();
            rsp_cb_hub_call_hub_handle.map_reg_hub.Add(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, cb_reg_hub_obj);
            return cb_reg_hub_obj;
        }

        public void hub_call_hub_mothed(string module, string func, byte[] argv){
            var _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e = new ArrayList();
            _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e.Add(module);
            _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e.Add(func);
            _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e.Add(argv);
            call_module_method("hub_call_hub_mothed", _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e);
        }

    }
/*this cb code is codegen by abelkhan for c#*/
    public class gate_call_hub_rsp_cb : abelkhan.Imodule {
        public gate_call_hub_rsp_cb(abelkhan.modulemng modules) : base("gate_call_hub_rsp_cb")
        {
            modules.reg_module(this);
        }

    }

    public class gate_call_hub_caller : abelkhan.Icaller {
        public static gate_call_hub_rsp_cb rsp_cb_gate_call_hub_handle = null;
        private UInt64 uuid_e1565384_c90b_3a02_ae2e_d0d91b2758d1 = RandomUUID.random();

        public gate_call_hub_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("gate_call_hub", _ch)
        {
            if (rsp_cb_gate_call_hub_handle == null)
            {
                rsp_cb_gate_call_hub_handle = new rsp_cb_gate_call_hub(modules);
            }
        }

        public void client_call_hub(string client_uuid, byte[] rpc_argv){
            var _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263 = new ArrayList();
            _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263.Add(client_uuid);
            _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263.Add(rpc_argv);
            call_module_method("client_call_hub", _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263);
        }

    }
/*this cb code is codegen by abelkhan for c#*/
    public class client_call_hub_rsp_cb : abelkhan.Imodule {
        public client_call_hub_rsp_cb(abelkhan.modulemng modules) : base("client_call_hub_rsp_cb")
        {
            modules.reg_module(this);
        }

    }

    public class client_call_hub_caller : abelkhan.Icaller {
        public static client_call_hub_rsp_cb rsp_cb_client_call_hub_handle = null;
        private UInt64 uuid_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263 = RandomUUID.random();

        public client_call_hub_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("client_call_hub", _ch)
        {
            if (rsp_cb_client_call_hub_handle == null)
            {
                rsp_cb_client_call_hub_handle = new rsp_cb_client_call_hub(modules);
            }
        }

        public void connect_hub(string client_uuid){
            var _argv_dc2ee339_bef5_3af9_a492_592ba4f08559 = new ArrayList();
            _argv_dc2ee339_bef5_3af9_a492_592ba4f08559.Add(client_uuid);
            call_module_method("connect_hub", _argv_dc2ee339_bef5_3af9_a492_592ba4f08559);
        }

        public void call_hub(string module, string func, byte[] argv){
            var _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3 = new ArrayList();
            _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3.Add(module);
            _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3.Add(func);
            _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3.Add(argv);
            call_module_method("call_hub", _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3);
        }

    }
/*this cb code is codegen by abelkhan for c#*/
    public class hub_call_client_rsp_cb : abelkhan.Imodule {
        public hub_call_client_rsp_cb(abelkhan.modulemng modules) : base("hub_call_client_rsp_cb")
        {
            modules.reg_module(this);
        }

    }

    public class hub_call_client_caller : abelkhan.Icaller {
        public static hub_call_client_rsp_cb rsp_cb_hub_call_client_handle = null;
        private UInt64 uuid_44e0e3b5_d5d3_3ab4_87a3_bdf8d8aefeeb = RandomUUID.random();

        public hub_call_client_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("hub_call_client", _ch)
        {
            if (rsp_cb_hub_call_client_handle == null)
            {
                rsp_cb_hub_call_client_handle = new rsp_cb_hub_call_client(modules);
            }
        }

        public void call_client(string module, string func, byte[] argv){
            var _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab = new ArrayList();
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.Add(module);
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.Add(func);
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.Add(argv);
            call_module_method("call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
        }

    }
/*this module code is codegen by abelkhan codegen for c#*/
    public class hub_call_hub_reg_hub_rsp : abelkhan.Response {
        private UInt64 uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67;
        public hub_call_hub_reg_hub_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_hub_rsp_cb", _ch)
        {
            uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67 = _uuid;
        }

        public void rsp(){
            var _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = new ArrayList();
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67);
            call_module_method("reg_hub_rsp", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        }

        public void err(){
            var _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = new ArrayList();
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67);
            call_module_method("reg_hub_err", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        }

    }

    public class hub_call_hub_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public hub_call_hub_module(abelkhan.modulemng _modules) : base("hub_call_hub")
        {
            modules = _modules;
            modules.reg_module(this);

            reg_method("reg_hub", reg_hub);
            reg_method("hub_call_hub_mothed", hub_call_hub_mothed);
        }

        public event Action<string, string> on_reg_hub;
        public void reg_hub(ArrayList inArray){
            var _cb_uuid = (UInt64)inArray[0];
            var _hub_name = (string)inArray[1];
            var _hub_type = (string)inArray[2];
            rsp = new hub_call_hub_reg_hub_rsp(current_ch, _cb_uuid);
            if (on_reg_hub != null){
                on_reg_hub(_hub_name, _hub_type);
            }
            rsp = null;
        }

        public event Action<string, string, byte[]> on_hub_call_hub_mothed;
        public void hub_call_hub_mothed(ArrayList inArray){
            var _module = (string)inArray[0];
            var _func = (string)inArray[1];
            var _argv = (byte[])inArray[2];
            if (on_hub_call_hub_mothed != null){
                on_hub_call_hub_mothed(_module, _func, _argv);
            }
        }

    }
    public class gate_call_hub_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public gate_call_hub_module(abelkhan.modulemng _modules) : base("gate_call_hub")
        {
            modules = _modules;
            modules.reg_module(this);

            reg_method("client_call_hub", client_call_hub);
        }

        public event Action<string, byte[]> on_client_call_hub;
        public void client_call_hub(ArrayList inArray){
            var _client_uuid = (string)inArray[0];
            var _rpc_argv = (byte[])inArray[1];
            if (on_client_call_hub != null){
                on_client_call_hub(_client_uuid, _rpc_argv);
            }
        }

    }
    public class client_call_hub_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public client_call_hub_module(abelkhan.modulemng _modules) : base("client_call_hub")
        {
            modules = _modules;
            modules.reg_module(this);

            reg_method("connect_hub", connect_hub);
            reg_method("call_hub", call_hub);
        }

        public event Action<string> on_connect_hub;
        public void connect_hub(ArrayList inArray){
            var _client_uuid = (string)inArray[0];
            if (on_connect_hub != null){
                on_connect_hub(_client_uuid);
            }
        }

        public event Action<string, string, byte[]> on_call_hub;
        public void call_hub(ArrayList inArray){
            var _module = (string)inArray[0];
            var _func = (string)inArray[1];
            var _argv = (byte[])inArray[2];
            if (on_call_hub != null){
                on_call_hub(_module, _func, _argv);
            }
        }

    }
    public class hub_call_client_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public hub_call_client_module(abelkhan.modulemng _modules) : base("hub_call_client")
        {
            modules = _modules;
            modules.reg_module(this);

            reg_method("call_client", call_client);
        }

        public event Action<string, string, byte[]> on_call_client;
        public void call_client(ArrayList inArray){
            var _module = (string)inArray[0];
            var _func = (string)inArray[1];
            var _argv = (byte[])inArray[2];
            if (on_call_client != null){
                on_call_client(_module, _func, _argv);
            }
        }

    }

}
