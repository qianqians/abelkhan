using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MsgPack.Serialization;

namespace Abelkhan
{
/*this enum code is codegen by Abelkhan codegen for c#*/

/*this struct code is codegen by Abelkhan codegen for c#*/
    public class hub_info
    {
        public string hub_name;
        public string hub_type;
        public static MsgPack.MessagePackObjectDictionary hub_info_to_protcol(hub_info _struct){
            var _protocol = new MsgPack.MessagePackObjectDictionary();
            _protocol.Add("hub_name", _struct.hub_name);
            _protocol.Add("hub_type", _struct.hub_type);
            return _protocol;
        }
        public static hub_info protcol_to_hub_info(MsgPack.MessagePackObjectDictionary _protocol){
            var _struct4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2 = new hub_info();
            foreach (var i in _protocol){
                if (((MsgPack.MessagePackObject)i.Key).AsString() == "hub_name"){
                    _struct4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2.hub_name = ((MsgPack.MessagePackObject)i.Value).AsString();
                }
                else if (((MsgPack.MessagePackObject)i.Key).AsString() == "hub_type"){
                    _struct4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2.hub_type = ((MsgPack.MessagePackObject)i.Value).AsString();
                }
            }
            return _struct4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2;
        }
    }

/*this caller code is codegen by Abelkhan codegen for c#*/
    public class client_call_gate_heartbeats_cb
    {
        private UInt64 cb_uuid;
        private client_call_gate_rsp_cb module_rsp_cb;

        public client_call_gate_heartbeats_cb(UInt64 _cb_uuid, client_call_gate_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<UInt64> on_heartbeats_cb;
        public event Action on_heartbeats_err;
        public event Action on_heartbeats_timeout;

        public client_call_gate_heartbeats_cb callBack(Action<UInt64> cb, Action err)
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

    public class client_call_gate_get_hub_info_cb
    {
        private UInt64 cb_uuid;
        private client_call_gate_rsp_cb module_rsp_cb;

        public client_call_gate_get_hub_info_cb(UInt64 _cb_uuid, client_call_gate_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<hub_info> on_get_hub_info_cb;
        public event Action on_get_hub_info_err;
        public event Action on_get_hub_info_timeout;

        public client_call_gate_get_hub_info_cb callBack(Action<hub_info> cb, Action err)
        {
            on_get_hub_info_cb += cb;
            on_get_hub_info_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.get_hub_info_timeout(cb_uuid);
            });
            on_get_hub_info_timeout += timeout_cb;
        }

        public void call_cb(hub_info hub_info)
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

/*this cb code is codegen by Abelkhan for c#*/
    public class client_call_gate_rsp_cb : Abelkhan.Imodule {
        public Dictionary<UInt64, client_call_gate_heartbeats_cb> map_heartbeats;
        public Dictionary<UInt64, client_call_gate_get_hub_info_cb> map_get_hub_info;
        public client_call_gate_rsp_cb(Abelkhan.modulemng modules) : base("client_call_gate_rsp_cb")
        {
            map_heartbeats = new Dictionary<UInt64, client_call_gate_heartbeats_cb>();
            modules.reg_method("client_call_gate_rsp_cb_heartbeats_rsp", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, heartbeats_rsp));
            modules.reg_method("client_call_gate_rsp_cb_heartbeats_err", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, heartbeats_err));
            map_get_hub_info = new Dictionary<UInt64, client_call_gate_get_hub_info_cb>();
            modules.reg_method("client_call_gate_rsp_cb_get_hub_info_rsp", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, get_hub_info_rsp));
            modules.reg_method("client_call_gate_rsp_cb_get_hub_info_err", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, get_hub_info_err));
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

        private client_call_gate_heartbeats_cb try_get_and_del_heartbeats_cb(UInt64 uuid){
            lock(map_heartbeats)
            {
                if (map_heartbeats.TryGetValue(uuid, out client_call_gate_heartbeats_cb rsp))
                {
                    map_heartbeats.Remove(uuid);
                }
                return rsp;
            }
        }

        public void get_hub_info_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _hub_info = hub_info.protcol_to_hub_info(((MsgPack.MessagePackObject)inArray[1]).AsDictionary());
            var rsp = try_get_and_del_get_hub_info_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_hub_info);
            }
        }

        public void get_hub_info_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
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
            {
                if (map_get_hub_info.TryGetValue(uuid, out client_call_gate_get_hub_info_cb rsp))
                {
                    map_get_hub_info.Remove(uuid);
                }
                return rsp;
            }
        }

    }

    public class client_call_gate_caller : Abelkhan.Icaller {
        public static client_call_gate_rsp_cb rsp_cb_client_call_gate_handle = null;
        private Int32 uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2 = (Int32)RandomUUID.random();

        public client_call_gate_caller(Abelkhan.Ichannel _ch, Abelkhan.modulemng modules) : base("client_call_gate", _ch)
        {
            if (rsp_cb_client_call_gate_handle == null)
            {
                rsp_cb_client_call_gate_handle = new client_call_gate_rsp_cb(modules);
            }
        }

        public client_call_gate_heartbeats_cb heartbeats(){
            var uuid_a514ca5f_2c67_5668_aac0_354397bdce36 = (UInt32)Interlocked.Increment(ref uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2);

            var _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = new ArrayList();
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.Add(uuid_a514ca5f_2c67_5668_aac0_354397bdce36);
            call_module_method("client_call_gate_heartbeats", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);

            var cb_heartbeats_obj = new client_call_gate_heartbeats_cb(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, rsp_cb_client_call_gate_handle);
            lock(rsp_cb_client_call_gate_handle.map_heartbeats)
            {
                rsp_cb_client_call_gate_handle.map_heartbeats.Add(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, cb_heartbeats_obj);
            }
            return cb_heartbeats_obj;
        }

        public client_call_gate_get_hub_info_cb get_hub_info(string hub_type){
            var uuid_e9d2753f_7d38_512d_80ff_7aae13508048 = (UInt32)Interlocked.Increment(ref uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2);

            var _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24 = new ArrayList();
            _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.Add(uuid_e9d2753f_7d38_512d_80ff_7aae13508048);
            _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.Add(hub_type);
            call_module_method("client_call_gate_get_hub_info", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);

            var cb_get_hub_info_obj = new client_call_gate_get_hub_info_cb(uuid_e9d2753f_7d38_512d_80ff_7aae13508048, rsp_cb_client_call_gate_handle);
            lock(rsp_cb_client_call_gate_handle.map_get_hub_info)
            {
                rsp_cb_client_call_gate_handle.map_get_hub_info.Add(uuid_e9d2753f_7d38_512d_80ff_7aae13508048, cb_get_hub_info_obj);
            }
            return cb_get_hub_info_obj;
        }

        public void forward_client_call_hub(string hub_name, byte[] rpc_argv){
            var _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5 = new ArrayList();
            _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5.Add(hub_name);
            _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5.Add(rpc_argv);
            call_module_method("client_call_gate_forward_client_call_hub", _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5);
        }

    }
    public class hub_call_gate_reg_hub_cb
    {
        private UInt64 cb_uuid;
        private hub_call_gate_rsp_cb module_rsp_cb;

        public hub_call_gate_reg_hub_cb(UInt64 _cb_uuid, hub_call_gate_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action on_reg_hub_cb;
        public event Action on_reg_hub_err;
        public event Action on_reg_hub_timeout;

        public hub_call_gate_reg_hub_cb callBack(Action cb, Action err)
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

    public class hub_call_gate_reverse_reg_client_hub_cb
    {
        private UInt64 cb_uuid;
        private hub_call_gate_rsp_cb module_rsp_cb;

        public hub_call_gate_reverse_reg_client_hub_cb(UInt64 _cb_uuid, hub_call_gate_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action on_reverse_reg_client_hub_cb;
        public event Action<framework_error> on_reverse_reg_client_hub_err;
        public event Action on_reverse_reg_client_hub_timeout;

        public hub_call_gate_reverse_reg_client_hub_cb callBack(Action cb, Action<framework_error> err)
        {
            on_reverse_reg_client_hub_cb += cb;
            on_reverse_reg_client_hub_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.reverse_reg_client_hub_timeout(cb_uuid);
            });
            on_reverse_reg_client_hub_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_reverse_reg_client_hub_cb != null)
            {
                on_reverse_reg_client_hub_cb();
            }
        }

        public void call_err(framework_error err)
        {
            if (on_reverse_reg_client_hub_err != null)
            {
                on_reverse_reg_client_hub_err(err);
            }
        }

        public void call_timeout()
        {
            if (on_reverse_reg_client_hub_timeout != null)
            {
                on_reverse_reg_client_hub_timeout();
            }
        }

    }

/*this cb code is codegen by Abelkhan for c#*/
    public class hub_call_gate_rsp_cb : Abelkhan.Imodule {
        public Dictionary<UInt64, hub_call_gate_reg_hub_cb> map_reg_hub;
        public Dictionary<UInt64, hub_call_gate_reverse_reg_client_hub_cb> map_reverse_reg_client_hub;
        public hub_call_gate_rsp_cb(Abelkhan.modulemng modules) : base("hub_call_gate_rsp_cb")
        {
            map_reg_hub = new Dictionary<UInt64, hub_call_gate_reg_hub_cb>();
            modules.reg_method("hub_call_gate_rsp_cb_reg_hub_rsp", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, reg_hub_rsp));
            modules.reg_method("hub_call_gate_rsp_cb_reg_hub_err", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, reg_hub_err));
            map_reverse_reg_client_hub = new Dictionary<UInt64, hub_call_gate_reverse_reg_client_hub_cb>();
            modules.reg_method("hub_call_gate_rsp_cb_reverse_reg_client_hub_rsp", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, reverse_reg_client_hub_rsp));
            modules.reg_method("hub_call_gate_rsp_cb_reverse_reg_client_hub_err", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, reverse_reg_client_hub_err));
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

        private hub_call_gate_reg_hub_cb try_get_and_del_reg_hub_cb(UInt64 uuid){
            lock(map_reg_hub)
            {
                if (map_reg_hub.TryGetValue(uuid, out hub_call_gate_reg_hub_cb rsp))
                {
                    map_reg_hub.Remove(uuid);
                }
                return rsp;
            }
        }

        public void reverse_reg_client_hub_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_reverse_reg_client_hub_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void reverse_reg_client_hub_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _err = (framework_error)((MsgPack.MessagePackObject)inArray[1]).AsInt32();
            var rsp = try_get_and_del_reverse_reg_client_hub_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err(_err);
            }
        }

        public void reverse_reg_client_hub_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_reverse_reg_client_hub_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private hub_call_gate_reverse_reg_client_hub_cb try_get_and_del_reverse_reg_client_hub_cb(UInt64 uuid){
            lock(map_reverse_reg_client_hub)
            {
                if (map_reverse_reg_client_hub.TryGetValue(uuid, out hub_call_gate_reverse_reg_client_hub_cb rsp))
                {
                    map_reverse_reg_client_hub.Remove(uuid);
                }
                return rsp;
            }
        }

    }

    public class hub_call_gate_caller : Abelkhan.Icaller {
        public static hub_call_gate_rsp_cb rsp_cb_hub_call_gate_handle = null;
        private Int32 uuid_9796175c_1119_3833_bf31_5ee139b40edc = (Int32)RandomUUID.random();

        public hub_call_gate_caller(Abelkhan.Ichannel _ch, Abelkhan.modulemng modules) : base("hub_call_gate", _ch)
        {
            if (rsp_cb_hub_call_gate_handle == null)
            {
                rsp_cb_hub_call_gate_handle = new hub_call_gate_rsp_cb(modules);
            }
        }

        public hub_call_gate_reg_hub_cb reg_hub(string hub_name, string hub_type){
            var uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = (UInt32)Interlocked.Increment(ref uuid_9796175c_1119_3833_bf31_5ee139b40edc);

            var _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = new ArrayList();
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106);
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(hub_name);
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(hub_type);
            call_module_method("hub_call_gate_reg_hub", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);

            var cb_reg_hub_obj = new hub_call_gate_reg_hub_cb(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, rsp_cb_hub_call_gate_handle);
            lock(rsp_cb_hub_call_gate_handle.map_reg_hub)
            {
                rsp_cb_hub_call_gate_handle.map_reg_hub.Add(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, cb_reg_hub_obj);
            }
            return cb_reg_hub_obj;
        }

        public void tick_hub_health(UInt32 tick_time){
            var _argv_e81472b5_19a4_36bc_9cd9_b8fe87a10079 = new ArrayList();
            _argv_e81472b5_19a4_36bc_9cd9_b8fe87a10079.Add(tick_time);
            call_module_method("hub_call_gate_tick_hub_health", _argv_e81472b5_19a4_36bc_9cd9_b8fe87a10079);
        }

        public hub_call_gate_reverse_reg_client_hub_cb reverse_reg_client_hub(string client_uuid){
            var uuid_5352b179_7aef_5875_a08f_06381972529f = (UInt32)Interlocked.Increment(ref uuid_9796175c_1119_3833_bf31_5ee139b40edc);

            var _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c = new ArrayList();
            _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c.Add(uuid_5352b179_7aef_5875_a08f_06381972529f);
            _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c.Add(client_uuid);
            call_module_method("hub_call_gate_reverse_reg_client_hub", _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c);

            var cb_reverse_reg_client_hub_obj = new hub_call_gate_reverse_reg_client_hub_cb(uuid_5352b179_7aef_5875_a08f_06381972529f, rsp_cb_hub_call_gate_handle);
            lock(rsp_cb_hub_call_gate_handle.map_reverse_reg_client_hub)
            {
                rsp_cb_hub_call_gate_handle.map_reverse_reg_client_hub.Add(uuid_5352b179_7aef_5875_a08f_06381972529f, cb_reverse_reg_client_hub_obj);
            }
            return cb_reverse_reg_client_hub_obj;
        }

        public void unreg_client_hub(string client_uuid){
            var _argv_3567e5c7_8e81_35c5_a6b6_c22d8e655aae = new ArrayList();
            _argv_3567e5c7_8e81_35c5_a6b6_c22d8e655aae.Add(client_uuid);
            call_module_method("hub_call_gate_unreg_client_hub", _argv_3567e5c7_8e81_35c5_a6b6_c22d8e655aae);
        }

        public void disconnect_client(string client_uuid){
            var _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85 = new ArrayList();
            _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85.Add(client_uuid);
            call_module_method("hub_call_gate_disconnect_client", _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85);
        }

        public void forward_hub_call_client(string client_uuid, byte[] rpc_argv){
            var _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1 = new ArrayList();
            _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1.Add(client_uuid);
            _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1.Add(rpc_argv);
            call_module_method("hub_call_gate_forward_hub_call_client", _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1);
        }

        public void forward_hub_call_group_client(List<string> client_uuids, byte[] rpc_argv){
            var _argv_374b012d_0718_3f84_91f0_784b12f8189c = new ArrayList();
            var _array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1 = new ArrayList();
            foreach(var v_a8e916a0_4f15_5329_b872_d0ff30ba7fe6 in client_uuids){
                _array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1.Add(v_a8e916a0_4f15_5329_b872_d0ff30ba7fe6);
            }
            _argv_374b012d_0718_3f84_91f0_784b12f8189c.Add(_array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1);
            _argv_374b012d_0718_3f84_91f0_784b12f8189c.Add(rpc_argv);
            call_module_method("hub_call_gate_forward_hub_call_group_client", _argv_374b012d_0718_3f84_91f0_784b12f8189c);
        }

        public void forward_hub_call_global_client(byte[] rpc_argv){
            var _argv_f69241c3_642a_3b51_bb37_cf638176493a = new ArrayList();
            _argv_f69241c3_642a_3b51_bb37_cf638176493a.Add(rpc_argv);
            call_module_method("hub_call_gate_forward_hub_call_global_client", _argv_f69241c3_642a_3b51_bb37_cf638176493a);
        }

    }
/*this module code is codegen by Abelkhan codegen for c#*/
    public class client_call_gate_heartbeats_rsp : Abelkhan.Response {
        private UInt64 uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56;
        public client_call_gate_heartbeats_rsp(Abelkhan.Ichannel _ch, UInt64 _uuid) : base("client_call_gate_rsp_cb", _ch)
        {
            uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56 = _uuid;
        }

        public void rsp(UInt64 timetmp_3c36cb1d_ce2b_3926_8169_233374fa19ac){
            var _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = new ArrayList();
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.Add(uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56);
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.Add(timetmp_3c36cb1d_ce2b_3926_8169_233374fa19ac);
            call_module_method("client_call_gate_rsp_cb_heartbeats_rsp", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        }

        public void err(){
            var _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = new ArrayList();
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.Add(uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56);
            call_module_method("client_call_gate_rsp_cb_heartbeats_err", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        }

    }

    public class client_call_gate_get_hub_info_rsp : Abelkhan.Response {
        private UInt64 uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b;
        public client_call_gate_get_hub_info_rsp(Abelkhan.Ichannel _ch, UInt64 _uuid) : base("client_call_gate_rsp_cb", _ch)
        {
            uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b = _uuid;
        }

        public void rsp(hub_info hub_info_4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2){
            var _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24 = new ArrayList();
            _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.Add(uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b);
            _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.Add(hub_info.hub_info_to_protcol(hub_info_4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2));
            call_module_method("client_call_gate_rsp_cb_get_hub_info_rsp", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
        }

        public void err(){
            var _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24 = new ArrayList();
            _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.Add(uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b);
            call_module_method("client_call_gate_rsp_cb_get_hub_info_err", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
        }

    }

    public class client_call_gate_module : Abelkhan.Imodule {
        private Abelkhan.modulemng modules;
        public client_call_gate_module(Abelkhan.modulemng _modules) : base("client_call_gate")
        {
            modules = _modules;
            modules.reg_method("client_call_gate_heartbeats", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, heartbeats));
            modules.reg_method("client_call_gate_get_hub_info", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, get_hub_info));
            modules.reg_method("client_call_gate_forward_client_call_hub", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, forward_client_call_hub));
        }

        public event Action on_heartbeats;
        public void heartbeats(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            rsp.Value = new client_call_gate_heartbeats_rsp(current_ch.Value, _cb_uuid);
            if (on_heartbeats != null){
                on_heartbeats();
            }
            rsp.Value = null;
        }

        public event Action<string> on_get_hub_info;
        public void get_hub_info(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _hub_type = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            rsp.Value = new client_call_gate_get_hub_info_rsp(current_ch.Value, _cb_uuid);
            if (on_get_hub_info != null){
                on_get_hub_info(_hub_type);
            }
            rsp.Value = null;
        }

        public event Action<string, byte[]> on_forward_client_call_hub;
        public void forward_client_call_hub(IList<MsgPack.MessagePackObject> inArray){
            var _hub_name = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            var _rpc_argv = ((MsgPack.MessagePackObject)inArray[1]).AsBinary();
            if (on_forward_client_call_hub != null){
                on_forward_client_call_hub(_hub_name, _rpc_argv);
            }
        }

    }
    public class hub_call_gate_reg_hub_rsp : Abelkhan.Response {
        private UInt64 uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67;
        public hub_call_gate_reg_hub_rsp(Abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_gate_rsp_cb", _ch)
        {
            uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67 = _uuid;
        }

        public void rsp(){
            var _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = new ArrayList();
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67);
            call_module_method("hub_call_gate_rsp_cb_reg_hub_rsp", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        }

        public void err(){
            var _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = new ArrayList();
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67);
            call_module_method("hub_call_gate_rsp_cb_reg_hub_err", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        }

    }

    public class hub_call_gate_reverse_reg_client_hub_rsp : Abelkhan.Response {
        private UInt64 uuid_ef84ff12_6e4a_39cd_896e_27f3ac82fa1a;
        public hub_call_gate_reverse_reg_client_hub_rsp(Abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_gate_rsp_cb", _ch)
        {
            uuid_ef84ff12_6e4a_39cd_896e_27f3ac82fa1a = _uuid;
        }

        public void rsp(){
            var _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c = new ArrayList();
            _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c.Add(uuid_ef84ff12_6e4a_39cd_896e_27f3ac82fa1a);
            call_module_method("hub_call_gate_rsp_cb_reverse_reg_client_hub_rsp", _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c);
        }

        public void err(framework_error err_ad2710a2_3dd2_3a8f_a4c8_a7ebbe1df696){
            var _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c = new ArrayList();
            _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c.Add(uuid_ef84ff12_6e4a_39cd_896e_27f3ac82fa1a);
            _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c.Add((int)err_ad2710a2_3dd2_3a8f_a4c8_a7ebbe1df696);
            call_module_method("hub_call_gate_rsp_cb_reverse_reg_client_hub_err", _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c);
        }

    }

    public class hub_call_gate_module : Abelkhan.Imodule {
        private Abelkhan.modulemng modules;
        public hub_call_gate_module(Abelkhan.modulemng _modules) : base("hub_call_gate")
        {
            modules = _modules;
            modules.reg_method("hub_call_gate_reg_hub", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, reg_hub));
            modules.reg_method("hub_call_gate_tick_hub_health", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, tick_hub_health));
            modules.reg_method("hub_call_gate_reverse_reg_client_hub", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, reverse_reg_client_hub));
            modules.reg_method("hub_call_gate_unreg_client_hub", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, unreg_client_hub));
            modules.reg_method("hub_call_gate_disconnect_client", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, disconnect_client));
            modules.reg_method("hub_call_gate_forward_hub_call_client", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, forward_hub_call_client));
            modules.reg_method("hub_call_gate_forward_hub_call_group_client", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, forward_hub_call_group_client));
            modules.reg_method("hub_call_gate_forward_hub_call_global_client", Tuple.Create<Abelkhan.Imodule, Action<IList<MsgPack.MessagePackObject> > >((Abelkhan.Imodule)this, forward_hub_call_global_client));
        }

        public event Action<string, string> on_reg_hub;
        public void reg_hub(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _hub_name = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _hub_type = ((MsgPack.MessagePackObject)inArray[2]).AsString();
            rsp.Value = new hub_call_gate_reg_hub_rsp(current_ch.Value, _cb_uuid);
            if (on_reg_hub != null){
                on_reg_hub(_hub_name, _hub_type);
            }
            rsp.Value = null;
        }

        public event Action<UInt32> on_tick_hub_health;
        public void tick_hub_health(IList<MsgPack.MessagePackObject> inArray){
            var _tick_time = ((MsgPack.MessagePackObject)inArray[0]).AsUInt32();
            if (on_tick_hub_health != null){
                on_tick_hub_health(_tick_time);
            }
        }

        public event Action<string> on_reverse_reg_client_hub;
        public void reverse_reg_client_hub(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _client_uuid = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            rsp.Value = new hub_call_gate_reverse_reg_client_hub_rsp(current_ch.Value, _cb_uuid);
            if (on_reverse_reg_client_hub != null){
                on_reverse_reg_client_hub(_client_uuid);
            }
            rsp.Value = null;
        }

        public event Action<string> on_unreg_client_hub;
        public void unreg_client_hub(IList<MsgPack.MessagePackObject> inArray){
            var _client_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            if (on_unreg_client_hub != null){
                on_unreg_client_hub(_client_uuid);
            }
        }

        public event Action<string> on_disconnect_client;
        public void disconnect_client(IList<MsgPack.MessagePackObject> inArray){
            var _client_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            if (on_disconnect_client != null){
                on_disconnect_client(_client_uuid);
            }
        }

        public event Action<string, byte[]> on_forward_hub_call_client;
        public void forward_hub_call_client(IList<MsgPack.MessagePackObject> inArray){
            var _client_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsString();
            var _rpc_argv = ((MsgPack.MessagePackObject)inArray[1]).AsBinary();
            if (on_forward_hub_call_client != null){
                on_forward_hub_call_client(_client_uuid, _rpc_argv);
            }
        }

        public event Action<List<string>, byte[]> on_forward_hub_call_group_client;
        public void forward_hub_call_group_client(IList<MsgPack.MessagePackObject> inArray){
            var _client_uuids = new List<string>();
            var _protocol_arrayclient_uuids = ((MsgPack.MessagePackObject)inArray[0]).AsList();
            foreach (var v_dfd11414_89c9_5adb_8977_69b93b30195b in _protocol_arrayclient_uuids){
                _client_uuids.Add(((MsgPack.MessagePackObject)v_dfd11414_89c9_5adb_8977_69b93b30195b).AsString());
            }
            var _rpc_argv = ((MsgPack.MessagePackObject)inArray[1]).AsBinary();
            if (on_forward_hub_call_group_client != null){
                on_forward_hub_call_group_client(_client_uuids, _rpc_argv);
            }
        }

        public event Action<byte[]> on_forward_hub_call_global_client;
        public void forward_hub_call_global_client(IList<MsgPack.MessagePackObject> inArray){
            var _rpc_argv = ((MsgPack.MessagePackObject)inArray[0]).AsBinary();
            if (on_forward_hub_call_global_client != null){
                on_forward_hub_call_global_client(_rpc_argv);
            }
        }

    }

}
