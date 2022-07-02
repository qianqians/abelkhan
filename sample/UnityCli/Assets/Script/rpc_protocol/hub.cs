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
    public class gate_call_hub_rsp_cb : abelkhan.Imodule {
        public gate_call_hub_rsp_cb(abelkhan.modulemng modules) : base("gate_call_hub_rsp_cb")
        {
        }

    }

    public class gate_call_hub_caller : abelkhan.Icaller {
        public static gate_call_hub_rsp_cb rsp_cb_gate_call_hub_handle = null;
        private Int64 uuid_e1565384_c90b_3a02_ae2e_d0d91b2758d1 = (Int64)RandomUUID.random();

        public gate_call_hub_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("gate_call_hub", _ch)
        {
            if (rsp_cb_gate_call_hub_handle == null)
            {
                rsp_cb_gate_call_hub_handle = new gate_call_hub_rsp_cb(modules);
            }
        }

        public void client_disconnect(string client_uuid){
            var _argv_0b9435aa_3d03_3778_acfb_c7bfbd4f3e60 = new ArrayList();
            _argv_0b9435aa_3d03_3778_acfb_c7bfbd4f3e60.Add(client_uuid);
            call_module_method("gate_call_hub_client_disconnect", _argv_0b9435aa_3d03_3778_acfb_c7bfbd4f3e60);
        }

        public void client_exception(string client_uuid){
            var _argv_706b1331_3629_3681_9d39_d2ef3b6675ed = new ArrayList();
            _argv_706b1331_3629_3681_9d39_d2ef3b6675ed.Add(client_uuid);
            call_module_method("gate_call_hub_client_exception", _argv_706b1331_3629_3681_9d39_d2ef3b6675ed);
        }

        public void client_call_hub(string client_uuid, byte[] rpc_argv){
            var _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263 = new ArrayList();
            _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263.Add(client_uuid);
            _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263.Add(rpc_argv);
            call_module_method("gate_call_hub_client_call_hub", _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263);
        }

    }
    public class hub_call_hub_reg_hub_cb
    {
        private UInt64 cb_uuid;
        private hub_call_hub_rsp_cb module_rsp_cb;

        public hub_call_hub_reg_hub_cb(UInt64 _cb_uuid, hub_call_hub_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action on_reg_hub_cb;
        public event Action on_reg_hub_err;
        public event Action on_reg_hub_timeout;

        public hub_call_hub_reg_hub_cb callBack(Action cb, Action err)
        {
            on_reg_hub_cb += cb;
            on_reg_hub_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
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

    public class hub_call_hub_seep_client_gate_cb
    {
        private UInt64 cb_uuid;
        private hub_call_hub_rsp_cb module_rsp_cb;

        public hub_call_hub_seep_client_gate_cb(UInt64 _cb_uuid, hub_call_hub_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action on_seep_client_gate_cb;
        public event Action<framework_error> on_seep_client_gate_err;
        public event Action on_seep_client_gate_timeout;

        public hub_call_hub_seep_client_gate_cb callBack(Action cb, Action<framework_error> err)
        {
            on_seep_client_gate_cb += cb;
            on_seep_client_gate_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.seep_client_gate_timeout(cb_uuid);
            });
            on_seep_client_gate_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_seep_client_gate_cb != null)
            {
                on_seep_client_gate_cb();
            }
        }

        public void call_err(framework_error err)
        {
            if (on_seep_client_gate_err != null)
            {
                on_seep_client_gate_err(err);
            }
        }

        public void call_timeout()
        {
            if (on_seep_client_gate_timeout != null)
            {
                on_seep_client_gate_timeout();
            }
        }

    }

/*this cb code is codegen by abelkhan for c#*/
    public class hub_call_hub_rsp_cb : abelkhan.Imodule {
        public Dictionary<UInt64, hub_call_hub_reg_hub_cb> map_reg_hub;
        public Dictionary<UInt64, hub_call_hub_seep_client_gate_cb> map_seep_client_gate;
        public hub_call_hub_rsp_cb(abelkhan.modulemng modules) : base("hub_call_hub_rsp_cb")
        {
            map_reg_hub = new Dictionary<UInt64, hub_call_hub_reg_hub_cb>();
            modules.reg_method("hub_call_hub_rsp_cb_reg_hub_rsp", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, reg_hub_rsp));
            modules.reg_method("hub_call_hub_rsp_cb_reg_hub_err", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, reg_hub_err));
            map_seep_client_gate = new Dictionary<UInt64, hub_call_hub_seep_client_gate_cb>();
            modules.reg_method("hub_call_hub_rsp_cb_seep_client_gate_rsp", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, seep_client_gate_rsp));
            modules.reg_method("hub_call_hub_rsp_cb_seep_client_gate_err", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, seep_client_gate_err));
        }

        public void reg_hub_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_reg_hub_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void reg_hub_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
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
            {
                if (map_reg_hub.TryGetValue(uuid, out hub_call_hub_reg_hub_cb rsp))
                {
                    map_reg_hub.Remove(uuid);
                }
                return rsp;
            }
        }

        public void seep_client_gate_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_seep_client_gate_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void seep_client_gate_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _err = (framework_error)((MsgPack.MessagePackObject)inArray[1]).AsInt32();
            var rsp = try_get_and_del_seep_client_gate_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err(_err);
            }
        }

        public void seep_client_gate_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_seep_client_gate_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private hub_call_hub_seep_client_gate_cb try_get_and_del_seep_client_gate_cb(UInt64 uuid){
            lock(map_seep_client_gate)
            {
                if (map_seep_client_gate.TryGetValue(uuid, out hub_call_hub_seep_client_gate_cb rsp))
                {
                    map_seep_client_gate.Remove(uuid);
                }
                return rsp;
            }
        }

    }

    public class hub_call_hub_caller : abelkhan.Icaller {
        public static hub_call_hub_rsp_cb rsp_cb_hub_call_hub_handle = null;
        private Int64 uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f = (Int64)RandomUUID.random();

        public hub_call_hub_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("hub_call_hub", _ch)
        {
            if (rsp_cb_hub_call_hub_handle == null)
            {
                rsp_cb_hub_call_hub_handle = new hub_call_hub_rsp_cb(modules);
            }
        }

        public hub_call_hub_reg_hub_cb reg_hub(string hub_name, string hub_type){
            var uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = (UInt64)Interlocked.Increment(ref uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f);

            var _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = new ArrayList();
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106);
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(hub_name);
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(hub_type);
            call_module_method("hub_call_hub_reg_hub", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);

            var cb_reg_hub_obj = new hub_call_hub_reg_hub_cb(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, rsp_cb_hub_call_hub_handle);
            lock(rsp_cb_hub_call_hub_handle.map_reg_hub)
            {
                rsp_cb_hub_call_hub_handle.map_reg_hub.Add(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, cb_reg_hub_obj);
            }
            return cb_reg_hub_obj;
        }

        public hub_call_hub_seep_client_gate_cb seep_client_gate(string client_uuid, string gate_name){
            var uuid_31169fc3_4fd4_512f_b157_203819bcbd47 = (UInt64)Interlocked.Increment(ref uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f);

            var _argv_78da410b_1845_3253_9a34_d7cda82883b6 = new ArrayList();
            _argv_78da410b_1845_3253_9a34_d7cda82883b6.Add(uuid_31169fc3_4fd4_512f_b157_203819bcbd47);
            _argv_78da410b_1845_3253_9a34_d7cda82883b6.Add(client_uuid);
            _argv_78da410b_1845_3253_9a34_d7cda82883b6.Add(gate_name);
            call_module_method("hub_call_hub_seep_client_gate", _argv_78da410b_1845_3253_9a34_d7cda82883b6);

            var cb_seep_client_gate_obj = new hub_call_hub_seep_client_gate_cb(uuid_31169fc3_4fd4_512f_b157_203819bcbd47, rsp_cb_hub_call_hub_handle);
            lock(rsp_cb_hub_call_hub_handle.map_seep_client_gate)
            {
                rsp_cb_hub_call_hub_handle.map_seep_client_gate.Add(uuid_31169fc3_4fd4_512f_b157_203819bcbd47, cb_seep_client_gate_obj);
            }
            return cb_seep_client_gate_obj;
        }

        public void hub_call_hub_mothed(byte[] rpc_argv){
            var _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e = new ArrayList();
            _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e.Add(rpc_argv);
            call_module_method("hub_call_hub_hub_call_hub_mothed", _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e);
        }

    }
/*this cb code is codegen by abelkhan for c#*/
    public class hub_call_client_rsp_cb : abelkhan.Imodule {
        public hub_call_client_rsp_cb(abelkhan.modulemng modules) : base("hub_call_client_rsp_cb")
        {
        }

    }

    public class hub_call_client_caller : abelkhan.Icaller {
        public static hub_call_client_rsp_cb rsp_cb_hub_call_client_handle = null;
        private Int64 uuid_44e0e3b5_d5d3_3ab4_87a3_bdf8d8aefeeb = (Int64)RandomUUID.random();

        public hub_call_client_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("hub_call_client", _ch)
        {
            if (rsp_cb_hub_call_client_handle == null)
            {
                rsp_cb_hub_call_client_handle = new hub_call_client_rsp_cb(modules);
            }
        }

        public void call_client(byte[] rpc_argv){
            var _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab = new ArrayList();
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.Add(rpc_argv);
            call_module_method("hub_call_client_call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
        }

    }
    public class client_call_hub_heartbeats_cb
    {
        private UInt64 cb_uuid;
        private client_call_hub_rsp_cb module_rsp_cb;

        public client_call_hub_heartbeats_cb(UInt64 _cb_uuid, client_call_hub_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<UInt64> on_heartbeats_cb;
        public event Action on_heartbeats_err;
        public event Action on_heartbeats_timeout;

        public client_call_hub_heartbeats_cb callBack(Action<UInt64> cb, Action err)
        {
            on_heartbeats_cb += cb;
            on_heartbeats_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.heartbeats_timeout(cb_uuid);
            });
            on_heartbeats_timeout += timeout_cb;
        }

        public void call_cb(UInt64 timetmp)
        {
            if (on_heartbeats_cb != null)
            {
                on_heartbeats_cb(timetmp);
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
    public class client_call_hub_rsp_cb : abelkhan.Imodule {
        public Dictionary<UInt64, client_call_hub_heartbeats_cb> map_heartbeats;
        public client_call_hub_rsp_cb(abelkhan.modulemng modules) : base("client_call_hub_rsp_cb")
        {
            map_heartbeats = new Dictionary<UInt64, client_call_hub_heartbeats_cb>();
            modules.reg_method("client_call_hub_rsp_cb_heartbeats_rsp", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, heartbeats_rsp));
            modules.reg_method("client_call_hub_rsp_cb_heartbeats_err", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, heartbeats_err));
        }

        public void heartbeats_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _timetmp = ((MsgPack.MessagePackObject)inArray[1]).AsUInt64();
            var rsp = try_get_and_del_heartbeats_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_timetmp);
            }
        }

        public void heartbeats_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
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

        private client_call_hub_heartbeats_cb try_get_and_del_heartbeats_cb(UInt64 uuid){
            lock(map_heartbeats)
            {
                if (map_heartbeats.TryGetValue(uuid, out client_call_hub_heartbeats_cb rsp))
                {
                    map_heartbeats.Remove(uuid);
                }
                return rsp;
            }
        }

    }

    public class client_call_hub_caller : abelkhan.Icaller {
        public static client_call_hub_rsp_cb rsp_cb_client_call_hub_handle = null;
        private Int64 uuid_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263 = (Int64)RandomUUID.random();

        public client_call_hub_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("client_call_hub", _ch)
        {
            if (rsp_cb_client_call_hub_handle == null)
            {
                rsp_cb_client_call_hub_handle = new client_call_hub_rsp_cb(modules);
            }
        }

        public void connect_hub(string client_uuid){
            var _argv_dc2ee339_bef5_3af9_a492_592ba4f08559 = new ArrayList();
            _argv_dc2ee339_bef5_3af9_a492_592ba4f08559.Add(client_uuid);
            call_module_method("client_call_hub_connect_hub", _argv_dc2ee339_bef5_3af9_a492_592ba4f08559);
        }

        public client_call_hub_heartbeats_cb heartbeats(){
            var uuid_a514ca5f_2c67_5668_aac0_354397bdce36 = (UInt64)Interlocked.Increment(ref uuid_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263);

            var _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = new ArrayList();
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.Add(uuid_a514ca5f_2c67_5668_aac0_354397bdce36);
            call_module_method("client_call_hub_heartbeats", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);

            var cb_heartbeats_obj = new client_call_hub_heartbeats_cb(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, rsp_cb_client_call_hub_handle);
            lock(rsp_cb_client_call_hub_handle.map_heartbeats)
            {
                rsp_cb_client_call_hub_handle.map_heartbeats.Add(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, cb_heartbeats_obj);
            }
            return cb_heartbeats_obj;
        }

        public void call_hub(byte[] rpc_argv){
            var _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3 = new ArrayList();
            _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3.Add(rpc_argv);
            call_module_method("client_call_hub_call_hub", _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3);
        }

    }
/*this module code is codegen by abelkhan codegen for c#*/
    public class gate_call_hub_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public gate_call_hub_module(abelkhan.modulemng _modules) : base("gate_call_hub")
        {
            modules = _modules;
            modules.reg_method("gate_call_hub_client_disconnect", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, client_disconnect));
            modules.reg_method("gate_call_hub_client_exception", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, client_exception));
            modules.reg_method("gate_call_hub_client_call_hub", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, client_call_hub));
        }

        public event Action<string> on_client_disconnect;
        public void client_disconnect(IList<MsgPack.MessagePackObject> inArray){
            var _client_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            if (on_client_disconnect != null){
                on_client_disconnect(_client_uuid);
            }
        }

        public event Action<string> on_client_exception;
        public void client_exception(IList<MsgPack.MessagePackObject> inArray){
            var _client_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            if (on_client_exception != null){
                on_client_exception(_client_uuid);
            }
        }

        public event Action<string, byte[]> on_client_call_hub;
        public void client_call_hub(IList<MsgPack.MessagePackObject> inArray){
            var _client_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            var _rpc_argv = ((MsgPack.MessagePackObject)inArray[1]).AsBinary();
            if (on_client_call_hub != null){
                on_client_call_hub(_client_uuid, _rpc_argv);
            }
        }

    }
    public class hub_call_hub_reg_hub_rsp : abelkhan.Response {
        private UInt64 uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67;
        public hub_call_hub_reg_hub_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_hub_rsp_cb", _ch)
        {
            uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67 = _uuid;
        }

        public void rsp(){
            var _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = new ArrayList();
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67);
            call_module_method("hub_call_hub_rsp_cb_reg_hub_rsp", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        }

        public void err(){
            var _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = new ArrayList();
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67);
            call_module_method("hub_call_hub_rsp_cb_reg_hub_err", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        }

    }

    public class hub_call_hub_seep_client_gate_rsp : abelkhan.Response {
        private UInt64 uuid_3068725f_71fe_3459_a18d_b3f1dc698c98;
        public hub_call_hub_seep_client_gate_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_hub_rsp_cb", _ch)
        {
            uuid_3068725f_71fe_3459_a18d_b3f1dc698c98 = _uuid;
        }

        public void rsp(){
            var _argv_78da410b_1845_3253_9a34_d7cda82883b6 = new ArrayList();
            _argv_78da410b_1845_3253_9a34_d7cda82883b6.Add(uuid_3068725f_71fe_3459_a18d_b3f1dc698c98);
            call_module_method("hub_call_hub_rsp_cb_seep_client_gate_rsp", _argv_78da410b_1845_3253_9a34_d7cda82883b6);
        }

        public void err(framework_error err_ad2710a2_3dd2_3a8f_a4c8_a7ebbe1df696){
            var _argv_78da410b_1845_3253_9a34_d7cda82883b6 = new ArrayList();
            _argv_78da410b_1845_3253_9a34_d7cda82883b6.Add(uuid_3068725f_71fe_3459_a18d_b3f1dc698c98);
            _argv_78da410b_1845_3253_9a34_d7cda82883b6.Add(err_ad2710a2_3dd2_3a8f_a4c8_a7ebbe1df696);
            call_module_method("hub_call_hub_rsp_cb_seep_client_gate_err", _argv_78da410b_1845_3253_9a34_d7cda82883b6);
        }

    }

    public class hub_call_hub_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public hub_call_hub_module(abelkhan.modulemng _modules) : base("hub_call_hub")
        {
            modules = _modules;
            modules.reg_method("hub_call_hub_reg_hub", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, reg_hub));
            modules.reg_method("hub_call_hub_seep_client_gate", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, seep_client_gate));
            modules.reg_method("hub_call_hub_hub_call_hub_mothed", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, hub_call_hub_mothed));
        }

        public event Action<string, string> on_reg_hub;
        public void reg_hub(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _hub_name = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _hub_type = ((MsgPack.MessagePackObject)inArray[2]).AsString();
            rsp.Value = new hub_call_hub_reg_hub_rsp(current_ch.Value, _cb_uuid);
            if (on_reg_hub != null){
                on_reg_hub(_hub_name, _hub_type);
            }
            rsp.Value = null;
        }

        public event Action<string, string> on_seep_client_gate;
        public void seep_client_gate(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _client_uuid = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _gate_name = ((MsgPack.MessagePackObject)inArray[2]).AsString();
            rsp.Value = new hub_call_hub_seep_client_gate_rsp(current_ch.Value, _cb_uuid);
            if (on_seep_client_gate != null){
                on_seep_client_gate(_client_uuid, _gate_name);
            }
            rsp.Value = null;
        }

        public event Action<byte[]> on_hub_call_hub_mothed;
        public void hub_call_hub_mothed(IList<MsgPack.MessagePackObject> inArray){
            var _rpc_argv = ((MsgPack.MessagePackObject)inArray[0]).AsBinary();
            if (on_hub_call_hub_mothed != null){
                on_hub_call_hub_mothed(_rpc_argv);
            }
        }

    }
    public class hub_call_client_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public hub_call_client_module(abelkhan.modulemng _modules) : base("hub_call_client")
        {
            modules = _modules;
            modules.reg_method("hub_call_client_call_client", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, call_client));
        }

        public event Action<byte[]> on_call_client;
        public void call_client(IList<MsgPack.MessagePackObject> inArray){
            var _rpc_argv = ((MsgPack.MessagePackObject)inArray[0]).AsBinary();
            if (on_call_client != null){
                on_call_client(_rpc_argv);
            }
        }

    }
    public class client_call_hub_heartbeats_rsp : abelkhan.Response {
        private UInt64 uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56;
        public client_call_hub_heartbeats_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("client_call_hub_rsp_cb", _ch)
        {
            uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56 = _uuid;
        }

        public void rsp(UInt64 timetmp_3c36cb1d_ce2b_3926_8169_233374fa19ac){
            var _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = new ArrayList();
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.Add(uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56);
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.Add(timetmp_3c36cb1d_ce2b_3926_8169_233374fa19ac);
            call_module_method("client_call_hub_rsp_cb_heartbeats_rsp", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        }

        public void err(){
            var _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = new ArrayList();
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.Add(uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56);
            call_module_method("client_call_hub_rsp_cb_heartbeats_err", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        }

    }

    public class client_call_hub_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public client_call_hub_module(abelkhan.modulemng _modules) : base("client_call_hub")
        {
            modules = _modules;
            modules.reg_method("client_call_hub_connect_hub", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, connect_hub));
            modules.reg_method("client_call_hub_heartbeats", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, heartbeats));
            modules.reg_method("client_call_hub_call_hub", Tuple.Create<abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((abelkhan.Imodule)this, call_hub));
        }

        public event Action<string> on_connect_hub;
        public void connect_hub(IList<MsgPack.MessagePackObject> inArray){
            var _client_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            if (on_connect_hub != null){
                on_connect_hub(_client_uuid);
            }
        }

        public event Action on_heartbeats;
        public void heartbeats(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            rsp.Value = new client_call_hub_heartbeats_rsp(current_ch.Value, _cb_uuid);
            if (on_heartbeats != null){
                on_heartbeats();
            }
            rsp.Value = null;
        }

        public event Action<byte[]> on_call_hub;
        public void call_hub(IList<MsgPack.MessagePackObject> inArray){
            var _rpc_argv = ((MsgPack.MessagePackObject)inArray[0]).AsBinary();
            if (on_call_hub != null){
                on_call_hub(_rpc_argv);
            }
        }

    }

}
