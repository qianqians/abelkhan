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
    public class gate_call_client_heartbeats_cb
    {
        private UInt64 cb_uuid;
        private gate_call_client_rsp_cb module_rsp_cb;

        public gate_call_client_heartbeats_cb(UInt64 _cb_uuid, gate_call_client_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<> on_heartbeats_cb;
        public event Action<> on_heartbeats_err;
        public event Action on_heartbeats_timeout;

        public gate_call_client_heartbeats_cb callBack(Action<> cb, Action<> err)
        {
            on_heartbeats_cb += cb;
            on_heartbeats_err += err;
            return this;
        }

        void timeout(Uint64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.heartbeats_timeout(cb_uuid);
            });
            on_heartbeats_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_heartbeats_cb != null)
            {
                on_heartbeats_cb();
            }
        }

        public void call_err()
        {
            if (on_heartbeats_err != null)
            {
                on_heartbeats_err();
            }
        }

        public void call_timeout()
        {
            if (on_heartbeats_timeout != null)
            {
                on_heartbeats_timeout();
            }
        }

    }

/*this cb code is codegen by abelkhan for c#*/
    public class gate_call_client_rsp_cb : abelkhan.Imodule {
        public Dictionary<UInt64, gate_call_client_heartbeats_cb> map_heartbeats;
        public gate_call_client_rsp_cb(abelkhan.modulemng modules) : base("gate_call_client_rsp_cb")
        {
            modules.reg_module(this);
            map_heartbeats = new Dictionary<UInt64, gate_call_client_heartbeats_cb>();
            reg_method("heartbeats_rsp", heartbeats_rsp);
            reg_method("heartbeats_err", heartbeats_err);
        }

        public void heartbeats_rsp(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_heartbeats_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void heartbeats_err(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_heartbeats_cb(uuid);
            if (rsp != null)
            {
            rsp.call_err();
            }
        }

        public void heartbeats_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_heartbeats_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private gate_call_client_heartbeats_cb try_get_and_del_heartbeats_cb(UInt64 uuid){
            lock(map_heartbeats)
            {                var rsp = map_heartbeats[uuid];
                map_heartbeats.Remove(uuid);
                return rsp;
            }
        }

    }

    public class gate_call_client_caller : abelkhan.Icaller {
        public static gate_call_client_rsp_cb rsp_cb_gate_call_client_handle = null;
        private UInt64 uuid = RandomUUID.random();

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

        public gate_call_client_heartbeats_cb heartbeats(UInt64 timetmp){
            Interlocked.Increment(ref uuid);
            var uuid_a514ca5f_2c67_5668_aac0_354397bdce36 = uuid;

            var _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = new ArrayList();
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.Add(uuid_a514ca5f_2c67_5668_aac0_354397bdce36);
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.Add(timetmp);
            call_module_method("heartbeats", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);

            var cb_heartbeats_obj = new gate_call_client_heartbeats_cb();
            rsp_cb_gate_call_client_handle.map_heartbeats.Add(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, cb_heartbeats_obj);
            return cb_heartbeats_obj;
        }

        public void call_client(byte[] rpc_argv){
            var _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab = new ArrayList();
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.Add(rpc_argv);
            call_module_method("call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
        }

    }
/*this module code is codegen by abelkhan codegen for c#*/
    public class gate_call_client_heartbeats_rsp : abelkhan.Response {
        private UInt64 uuid;
        public gate_call_client_heartbeats_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("gate_call_client_rsp_cb", _ch)
        {
            uuid = _uuid;
        }

        public void rsp(){
            var _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = new ArrayList();
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.Add(uuid);
            call_module_method("heartbeats_rsp", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        }

        public void err(){
            var _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = new ArrayList();
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.Add(this.uuid);
            call_module_method("heartbeats_err", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        }

    }

    public class gate_call_client_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public gate_call_client_module(abelkhan.modulemng _modules) : base("gate_call_client")
        {
            modules = _modules;
            modules.reg_module(this);

            reg_method("ntf_cuuid", ntf_cuuid);
            reg_method("heartbeats", heartbeats);
            reg_method("call_client", call_client);
        }

        public event Action<string> on_ntf_cuuid;
        public void ntf_cuuid(ArrayList inArray){
            var _cuuid = (string)inArray[0];
            if (on_ntf_cuuid != null){
                on_ntf_cuuid(_cuuid);
            }
        }

        public event Action<UInt64> on_heartbeats;
        public void heartbeats(ArrayList inArray){
            var _cb_uuid = (UInt64)inArray[0];
            var _timetmp = (UInt64)inArray[1];
            rsp = new gate_call_client_heartbeats_rsp(current_ch, _cb_uuid);
            if (on_heartbeats != null){
                on_heartbeats(_timetmp);
            }
            rsp = null;
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
