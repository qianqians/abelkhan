using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MsgPack.Serialization;

namespace abelkhan
{
/*this enum code is codegen by abelkhan codegen for c#*/

/*this struct code is codegen by abelkhan codegen for c#*/
    public class hub_info
    {
        public string hub_name;
        public string hub_type;
        public static Hashtable hub_info_to_protcol(hub_info _struct){
            var _protocol = new Hashtable();
            _protocol.Add("hub_name", _struct.hub_name);
            _protocol.Add("hub_type", _struct.hub_type);
            return _protocol;
        }
        public static hub_info protcol_to_hub_info(Hashtable _protocol){
            var _struct4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2 = new hub_info();
            foreach(var i in _protocol){
                if (i.Key == "hub_name"){
                    _struct4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2.hub_name = (string)i.Value;
                }
                else if (i.Key == "hub_type"){
                    _struct4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2.hub_type = (string)i.Value;
                }
            }
            return _struct;
        }
    }

/*this caller code is codegen by abelkhan codegen for c#*/
    public class hub_call_gate_reg_hub_cb
    {
        private UInt64 cb_uuid;
        private hub_call_gate_rsp_cb module_rsp_cb;

        public hub_call_gate_reg_hub_cb(UInt64 _cb_uuid, hub_call_gate_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<> on_reg_hub_cb;
        public event Action<> on_reg_hub_err;
        public event Action on_reg_hub_timeout;

        public hub_call_gate_reg_hub_cb callBack(Action<> cb, Action<> err)
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
    public class hub_call_gate_rsp_cb : abelkhan.Imodule {
        public Dictionary<UInt64, hub_call_gate_reg_hub_cb> map_reg_hub;
        public hub_call_gate_rsp_cb(abelkhan.modulemng modules) : base("hub_call_gate_rsp_cb")
        {
            modules.reg_module(this);
            map_reg_hub = new Dictionary<UInt64, hub_call_gate_reg_hub_cb>();
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

        private hub_call_gate_reg_hub_cb try_get_and_del_reg_hub_cb(UInt64 uuid){
            lock(map_reg_hub)
            {                var rsp = map_reg_hub[uuid];
                map_reg_hub.Remove(uuid);
                return rsp;
            }
        }

    }

    public class hub_call_gate_caller : abelkhan.Icaller {
        public static hub_call_gate_rsp_cb rsp_cb_hub_call_gate_handle = null;
        private UInt64 uuid_9796175c_1119_3833_bf31_5ee139b40edc = RandomUUID.random();

        public hub_call_gate_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("hub_call_gate", _ch)
        {
            if (rsp_cb_hub_call_gate_handle == null)
            {
                rsp_cb_hub_call_gate_handle = new rsp_cb_hub_call_gate(modules);
            }
        }

        public hub_call_gate_reg_hub_cb reg_hub(string hub_name, string hub_type){
            Interlocked.Increment(ref uuid_9796175c_1119_3833_bf31_5ee139b40edc);
            var uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = uuid;

            var _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = new ArrayList();
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106);
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(hub_name);
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(hub_type);
            call_module_method("reg_hub", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);

            var cb_reg_hub_obj = new hub_call_gate_reg_hub_cb();
            rsp_cb_hub_call_gate_handle.map_reg_hub.Add(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, cb_reg_hub_obj);
            return cb_reg_hub_obj;
        }

        public void disconnect_client(string client_uuid){
            var _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85 = new ArrayList();
            _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85.Add(client_uuid);
            call_module_method("disconnect_client", _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85);
        }

        public void forward_hub_call_client(string client_uuid, byte[] rpc_argv){
            var _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1 = new ArrayList();
            _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1.Add(client_uuid);
            _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1.Add(rpc_argv);
            call_module_method("forward_hub_call_client", _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1);
        }

        public void forward_hub_call_group_client(List<string> client_uuids, byte[] rpc_argv){
            var _argv_374b012d_0718_3f84_91f0_784b12f8189c = new ArrayList();
            var _array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1 = new ArrayList();
            foreach(var v_a8e916a0_4f15_5329_b872_d0ff30ba7fe6 in client_uuids){
                _array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1.Add(v_a8e916a0_4f15_5329_b872_d0ff30ba7fe6);
            }
            _argv_374b012d_0718_3f84_91f0_784b12f8189c.Add(_array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1);
            _argv_374b012d_0718_3f84_91f0_784b12f8189c.Add(rpc_argv);
            call_module_method("forward_hub_call_group_client", _argv_374b012d_0718_3f84_91f0_784b12f8189c);
        }

        public void forward_hub_call_global_client(byte[] rpc_argv){
            var _argv_f69241c3_642a_3b51_bb37_cf638176493a = new ArrayList();
            _argv_f69241c3_642a_3b51_bb37_cf638176493a.Add(rpc_argv);
            call_module_method("forward_hub_call_global_client", _argv_f69241c3_642a_3b51_bb37_cf638176493a);
        }

    }
    public class client_call_gate_get_hub_info_cb
    {
        private UInt64 cb_uuid;
        private client_call_gate_rsp_cb module_rsp_cb;

        public client_call_gate_get_hub_info_cb(UInt64 _cb_uuid, client_call_gate_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<List<hub_info>> on_get_hub_info_cb;
        public event Action<> on_get_hub_info_err;
        public event Action on_get_hub_info_timeout;

        public client_call_gate_get_hub_info_cb callBack(Action<List<hub_info>> cb, Action<> err)
        {
            on_get_hub_info_cb += cb;
            on_get_hub_info_err += err;
            return this;
        }

        void timeout(Uint64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.get_hub_info_timeout(cb_uuid);
            });
            on_get_hub_info_timeout += timeout_cb;
        }

        public void call_cb(List<hub_info> hub_info)
        {
            if (on_get_hub_info_cb != null)
            {
                on_get_hub_info_cb(hub_info);
            }
        }

        public void call_err()
        {
            if (on_get_hub_info_err != null)
            {
                on_get_hub_info_err();
            }
        }

        public void call_timeout()
        {
            if (on_get_hub_info_timeout != null)
            {
                on_get_hub_info_timeout();
            }
        }

    }

/*this cb code is codegen by abelkhan for c#*/
    public class client_call_gate_rsp_cb : abelkhan.Imodule {
        public Dictionary<UInt64, client_call_gate_get_hub_info_cb> map_get_hub_info;
        public client_call_gate_rsp_cb(abelkhan.modulemng modules) : base("client_call_gate_rsp_cb")
        {
            modules.reg_module(this);
            map_get_hub_info = new Dictionary<UInt64, client_call_gate_get_hub_info_cb>();
            reg_method("get_hub_info_rsp", get_hub_info_rsp);
            reg_method("get_hub_info_err", get_hub_info_err);
        }

        public void get_hub_info_rsp(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var _hub_info = new List<hub_info>();
            foreach(var v_53b78086_1765_5879_87b4_63333838766a in inArray[1]){
                _hub_info.Add(hub_info.protcol_to_hub_info(v_53b78086_1765_5879_87b4_63333838766a));
            }
            var rsp = try_get_and_del_get_hub_info_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_hub_info);
            }
        }

        public void get_hub_info_err(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_get_hub_info_cb(uuid);
            if (rsp != null)
            {
            rsp.call_err();
            }
        }

        public void get_hub_info_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_get_hub_info_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private client_call_gate_get_hub_info_cb try_get_and_del_get_hub_info_cb(UInt64 uuid){
            lock(map_get_hub_info)
            {                var rsp = map_get_hub_info[uuid];
                map_get_hub_info.Remove(uuid);
                return rsp;
            }
        }

    }

    public class client_call_gate_caller : abelkhan.Icaller {
        public static client_call_gate_rsp_cb rsp_cb_client_call_gate_handle = null;
        private UInt64 uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2 = RandomUUID.random();

        public client_call_gate_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("client_call_gate", _ch)
        {
            if (rsp_cb_client_call_gate_handle == null)
            {
                rsp_cb_client_call_gate_handle = new rsp_cb_client_call_gate(modules);
            }
        }

        public client_call_gate_get_hub_info_cb get_hub_info(){
            Interlocked.Increment(ref uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2);
            var uuid_e9d2753f_7d38_512d_80ff_7aae13508048 = uuid;

            var _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24 = new ArrayList();
            _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.Add(uuid_e9d2753f_7d38_512d_80ff_7aae13508048);
            call_module_method("get_hub_info", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);

            var cb_get_hub_info_obj = new client_call_gate_get_hub_info_cb();
            rsp_cb_client_call_gate_handle.map_get_hub_info.Add(uuid_e9d2753f_7d38_512d_80ff_7aae13508048, cb_get_hub_info_obj);
            return cb_get_hub_info_obj;
        }

        public void forward_client_call_hub(string hub_name, byte[] rpc_argv){
            var _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5 = new ArrayList();
            _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5.Add(hub_name);
            _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5.Add(rpc_argv);
            call_module_method("forward_client_call_hub", _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5);
        }

    }
/*this module code is codegen by abelkhan codegen for c#*/
    public class hub_call_gate_reg_hub_rsp : abelkhan.Response {
        private UInt64 uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67;
        public hub_call_gate_reg_hub_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_gate_rsp_cb", _ch)
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

    public class hub_call_gate_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public hub_call_gate_module(abelkhan.modulemng _modules) : base("hub_call_gate")
        {
            modules = _modules;
            modules.reg_module(this);

            reg_method("reg_hub", reg_hub);
            reg_method("disconnect_client", disconnect_client);
            reg_method("forward_hub_call_client", forward_hub_call_client);
            reg_method("forward_hub_call_group_client", forward_hub_call_group_client);
            reg_method("forward_hub_call_global_client", forward_hub_call_global_client);
        }

        public event Action<string, string> on_reg_hub;
        public void reg_hub(ArrayList inArray){
            var _cb_uuid = (UInt64)inArray[0];
            var _hub_name = (string)inArray[1];
            var _hub_type = (string)inArray[2];
            rsp = new hub_call_gate_reg_hub_rsp(current_ch, _cb_uuid);
            if (on_reg_hub != null){
                on_reg_hub(_hub_name, _hub_type);
            }
            rsp = null;
        }

        public event Action<string> on_disconnect_client;
        public void disconnect_client(ArrayList inArray){
            var _client_uuid = (string)inArray[0];
            if (on_disconnect_client != null){
                on_disconnect_client(_client_uuid);
            }
        }

        public event Action<string, byte[]> on_forward_hub_call_client;
        public void forward_hub_call_client(ArrayList inArray){
            var _client_uuid = (string)inArray[0];
            var _rpc_argv = (byte[])inArray[1];
            if (on_forward_hub_call_client != null){
                on_forward_hub_call_client(_client_uuid, _rpc_argv);
            }
        }

        public event Action<List<string>, byte[]> on_forward_hub_call_group_client;
        public void forward_hub_call_group_client(ArrayList inArray){
            var _client_uuids = new List<string>();
            foreach(var v_dfd11414_89c9_5adb_8977_69b93b30195b in (ArrayList)inArray[0]){
                _client_uuids.Add((string)v_dfd11414_89c9_5adb_8977_69b93b30195b);
            }
            var _rpc_argv = (byte[])inArray[1];
            if (on_forward_hub_call_group_client != null){
                on_forward_hub_call_group_client(_client_uuids, _rpc_argv);
            }
        }

        public event Action<byte[]> on_forward_hub_call_global_client;
        public void forward_hub_call_global_client(ArrayList inArray){
            var _rpc_argv = (byte[])inArray[0];
            if (on_forward_hub_call_global_client != null){
                on_forward_hub_call_global_client(_rpc_argv);
            }
        }

    }
    public class client_call_gate_get_hub_info_rsp : abelkhan.Response {
        private UInt64 uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b;
        public client_call_gate_get_hub_info_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("client_call_gate_rsp_cb", _ch)
        {
            uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b = _uuid;
        }

        public void rsp(List<hub_info> hub_info){
            var _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24 = new ArrayList();
            _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.Add(uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b);
            var _array_4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2 = new JArray();            foreach(var v_53b78086_1765_5879_87b4_63333838766a in hub_info){
                _array_4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2.Add(hub_info.hub_info_to_protcol(v_53b78086_1765_5879_87b4_63333838766a));
            }
            _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.Add(_array_4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2);
            call_module_method("get_hub_info_rsp", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
        }

        public void err(){
            var _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24 = new ArrayList();
            _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.Add(uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b);
            call_module_method("get_hub_info_err", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
        }

    }

    public class client_call_gate_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public client_call_gate_module(abelkhan.modulemng _modules) : base("client_call_gate")
        {
            modules = _modules;
            modules.reg_module(this);

            reg_method("get_hub_info", get_hub_info);
            reg_method("forward_client_call_hub", forward_client_call_hub);
        }

        public event Action<> on_get_hub_info;
        public void get_hub_info(ArrayList inArray){
            var _cb_uuid = (UInt64)inArray[0];
            rsp = new client_call_gate_get_hub_info_rsp(current_ch, _cb_uuid);
            if (on_get_hub_info != null){
                on_get_hub_info();
            }
            rsp = null;
        }

        public event Action<string, byte[]> on_forward_client_call_hub;
        public void forward_client_call_hub(ArrayList inArray){
            var _hub_name = (string)inArray[0];
            var _rpc_argv = (byte[])inArray[1];
            if (on_forward_client_call_hub != null){
                on_forward_client_call_hub(_hub_name, _rpc_argv);
            }
        }

    }

}
