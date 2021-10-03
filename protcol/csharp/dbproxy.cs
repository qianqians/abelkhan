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
    public class dbproxy_call_hub_rsp_cb : abelkhan.Imodule {
        public dbproxy_call_hub_rsp_cb(abelkhan.modulemng modules) : base("dbproxy_call_hub_rsp_cb")
        {
            modules.reg_module(this);
        }

    }

    public class dbproxy_call_hub_caller : abelkhan.Icaller {
        public static dbproxy_call_hub_rsp_cb rsp_cb_dbproxy_call_hub_handle = null;
        private UInt64 uuid_7a1d0ce9_a121_3019_b67a_319998ea37c8 = RandomUUID.random();

        public dbproxy_call_hub_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("dbproxy_call_hub", _ch)
        {
            if (rsp_cb_dbproxy_call_hub_handle == null)
            {
                rsp_cb_dbproxy_call_hub_handle = new rsp_cb_dbproxy_call_hub(modules);
            }
        }

        public void ack_get_object_info(string callbackid, byte[] object_info){
            var _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7 = new ArrayList();
            _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7.Add(callbackid);
            _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7.Add(object_info);
            call_module_method("ack_get_object_info", _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7);
        }

        public void ack_get_object_info_end(string callbackid){
            var _argv_e4756ccf_94e2_3b4f_958a_701f7076e607 = new ArrayList();
            _argv_e4756ccf_94e2_3b4f_958a_701f7076e607.Add(callbackid);
            call_module_method("ack_get_object_info_end", _argv_e4756ccf_94e2_3b4f_958a_701f7076e607);
        }

    }
    public class hub_call_dbproxy_reg_hub_cb
    {
        private UInt64 cb_uuid;
        private hub_call_dbproxy_rsp_cb module_rsp_cb;

        public hub_call_dbproxy_reg_hub_cb(UInt64 _cb_uuid, hub_call_dbproxy_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<> on_reg_hub_cb;
        public event Action<> on_reg_hub_err;
        public event Action on_reg_hub_timeout;

        public hub_call_dbproxy_reg_hub_cb callBack(Action<> cb, Action<> err)
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

    public class hub_call_dbproxy_create_persisted_object_cb
    {
        private UInt64 cb_uuid;
        private hub_call_dbproxy_rsp_cb module_rsp_cb;

        public hub_call_dbproxy_create_persisted_object_cb(UInt64 _cb_uuid, hub_call_dbproxy_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<> on_create_persisted_object_cb;
        public event Action<> on_create_persisted_object_err;
        public event Action on_create_persisted_object_timeout;

        public hub_call_dbproxy_create_persisted_object_cb callBack(Action<> cb, Action<> err)
        {
            on_create_persisted_object_cb += cb;
            on_create_persisted_object_err += err;
            return this;
        }

        void timeout(Uint64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.create_persisted_object_timeout(cb_uuid);
            });
            on_create_persisted_object_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_create_persisted_object_cb != null)
            {
                on_create_persisted_object_cb();
            }
        }

        public void call_err()
        {
            if (on_create_persisted_object_err != null)
            {
                on_create_persisted_object_err();
            }
        }

        public void call_timeout()
        {
            if (on_create_persisted_object_timeout != null)
            {
                on_create_persisted_object_timeout();
            }
        }

    }

    public class hub_call_dbproxy_updata_persisted_object_cb
    {
        private UInt64 cb_uuid;
        private hub_call_dbproxy_rsp_cb module_rsp_cb;

        public hub_call_dbproxy_updata_persisted_object_cb(UInt64 _cb_uuid, hub_call_dbproxy_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<> on_updata_persisted_object_cb;
        public event Action<> on_updata_persisted_object_err;
        public event Action on_updata_persisted_object_timeout;

        public hub_call_dbproxy_updata_persisted_object_cb callBack(Action<> cb, Action<> err)
        {
            on_updata_persisted_object_cb += cb;
            on_updata_persisted_object_err += err;
            return this;
        }

        void timeout(Uint64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.updata_persisted_object_timeout(cb_uuid);
            });
            on_updata_persisted_object_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_updata_persisted_object_cb != null)
            {
                on_updata_persisted_object_cb();
            }
        }

        public void call_err()
        {
            if (on_updata_persisted_object_err != null)
            {
                on_updata_persisted_object_err();
            }
        }

        public void call_timeout()
        {
            if (on_updata_persisted_object_timeout != null)
            {
                on_updata_persisted_object_timeout();
            }
        }

    }

    public class hub_call_dbproxy_remove_object_cb
    {
        private UInt64 cb_uuid;
        private hub_call_dbproxy_rsp_cb module_rsp_cb;

        public hub_call_dbproxy_remove_object_cb(UInt64 _cb_uuid, hub_call_dbproxy_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<> on_remove_object_cb;
        public event Action<> on_remove_object_err;
        public event Action on_remove_object_timeout;

        public hub_call_dbproxy_remove_object_cb callBack(Action<> cb, Action<> err)
        {
            on_remove_object_cb += cb;
            on_remove_object_err += err;
            return this;
        }

        void timeout(Uint64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.remove_object_timeout(cb_uuid);
            });
            on_remove_object_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_remove_object_cb != null)
            {
                on_remove_object_cb();
            }
        }

        public void call_err()
        {
            if (on_remove_object_err != null)
            {
                on_remove_object_err();
            }
        }

        public void call_timeout()
        {
            if (on_remove_object_timeout != null)
            {
                on_remove_object_timeout();
            }
        }

    }

    public class hub_call_dbproxy_get_object_count_cb
    {
        private UInt64 cb_uuid;
        private hub_call_dbproxy_rsp_cb module_rsp_cb;

        public hub_call_dbproxy_get_object_count_cb(UInt64 _cb_uuid, hub_call_dbproxy_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<UInt32> on_get_object_count_cb;
        public event Action<> on_get_object_count_err;
        public event Action on_get_object_count_timeout;

        public hub_call_dbproxy_get_object_count_cb callBack(Action<UInt32> cb, Action<> err)
        {
            on_get_object_count_cb += cb;
            on_get_object_count_err += err;
            return this;
        }

        void timeout(Uint64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.get_object_count_timeout(cb_uuid);
            });
            on_get_object_count_timeout += timeout_cb;
        }

        public void call_cb(UInt32 count)
        {
            if (on_get_object_count_cb != null)
            {
                on_get_object_count_cb(count);
            }
        }

        public void call_err()
        {
            if (on_get_object_count_err != null)
            {
                on_get_object_count_err();
            }
        }

        public void call_timeout()
        {
            if (on_get_object_count_timeout != null)
            {
                on_get_object_count_timeout();
            }
        }

    }

/*this cb code is codegen by abelkhan for c#*/
    public class hub_call_dbproxy_rsp_cb : abelkhan.Imodule {
        public Dictionary<UInt64, hub_call_dbproxy_reg_hub_cb> map_reg_hub;
        public Dictionary<UInt64, hub_call_dbproxy_create_persisted_object_cb> map_create_persisted_object;
        public Dictionary<UInt64, hub_call_dbproxy_updata_persisted_object_cb> map_updata_persisted_object;
        public Dictionary<UInt64, hub_call_dbproxy_remove_object_cb> map_remove_object;
        public Dictionary<UInt64, hub_call_dbproxy_get_object_count_cb> map_get_object_count;
        public hub_call_dbproxy_rsp_cb(abelkhan.modulemng modules) : base("hub_call_dbproxy_rsp_cb")
        {
            modules.reg_module(this);
            map_reg_hub = new Dictionary<UInt64, hub_call_dbproxy_reg_hub_cb>();
            reg_method("reg_hub_rsp", reg_hub_rsp);
            reg_method("reg_hub_err", reg_hub_err);
            map_create_persisted_object = new Dictionary<UInt64, hub_call_dbproxy_create_persisted_object_cb>();
            reg_method("create_persisted_object_rsp", create_persisted_object_rsp);
            reg_method("create_persisted_object_err", create_persisted_object_err);
            map_updata_persisted_object = new Dictionary<UInt64, hub_call_dbproxy_updata_persisted_object_cb>();
            reg_method("updata_persisted_object_rsp", updata_persisted_object_rsp);
            reg_method("updata_persisted_object_err", updata_persisted_object_err);
            map_remove_object = new Dictionary<UInt64, hub_call_dbproxy_remove_object_cb>();
            reg_method("remove_object_rsp", remove_object_rsp);
            reg_method("remove_object_err", remove_object_err);
            map_get_object_count = new Dictionary<UInt64, hub_call_dbproxy_get_object_count_cb>();
            reg_method("get_object_count_rsp", get_object_count_rsp);
            reg_method("get_object_count_err", get_object_count_err);
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

        private hub_call_dbproxy_reg_hub_cb try_get_and_del_reg_hub_cb(UInt64 uuid){
            lock(map_reg_hub)
            {
                var rsp = map_reg_hub[uuid];
                map_reg_hub.Remove(uuid);
                return rsp;
            }
        }

        public void create_persisted_object_rsp(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_create_persisted_object_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void create_persisted_object_err(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_create_persisted_object_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void create_persisted_object_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_create_persisted_object_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private hub_call_dbproxy_create_persisted_object_cb try_get_and_del_create_persisted_object_cb(UInt64 uuid){
            lock(map_create_persisted_object)
            {
                var rsp = map_create_persisted_object[uuid];
                map_create_persisted_object.Remove(uuid);
                return rsp;
            }
        }

        public void updata_persisted_object_rsp(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_updata_persisted_object_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void updata_persisted_object_err(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_updata_persisted_object_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void updata_persisted_object_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_updata_persisted_object_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private hub_call_dbproxy_updata_persisted_object_cb try_get_and_del_updata_persisted_object_cb(UInt64 uuid){
            lock(map_updata_persisted_object)
            {
                var rsp = map_updata_persisted_object[uuid];
                map_updata_persisted_object.Remove(uuid);
                return rsp;
            }
        }

        public void remove_object_rsp(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_remove_object_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void remove_object_err(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_remove_object_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void remove_object_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_remove_object_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private hub_call_dbproxy_remove_object_cb try_get_and_del_remove_object_cb(UInt64 uuid){
            lock(map_remove_object)
            {
                var rsp = map_remove_object[uuid];
                map_remove_object.Remove(uuid);
                return rsp;
            }
        }

        public void get_object_count_rsp(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var _count = (UInt32)inArray[1];
            var rsp = try_get_and_del_get_object_count_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_count);
            }
        }

        public void get_object_count_err(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_get_object_count_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void get_object_count_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_get_object_count_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private hub_call_dbproxy_get_object_count_cb try_get_and_del_get_object_count_cb(UInt64 uuid){
            lock(map_get_object_count)
            {
                var rsp = map_get_object_count[uuid];
                map_get_object_count.Remove(uuid);
                return rsp;
            }
        }

    }

    public class hub_call_dbproxy_caller : abelkhan.Icaller {
        public static hub_call_dbproxy_rsp_cb rsp_cb_hub_call_dbproxy_handle = null;
        private UInt64 uuid_e713438c_e791_3714_ad31_4ccbddee2554 = RandomUUID.random();

        public hub_call_dbproxy_caller(abelkhan.Ichannel _ch, abelkhan.modulemng modules) : base("hub_call_dbproxy", _ch)
        {
            if (rsp_cb_hub_call_dbproxy_handle == null)
            {
                rsp_cb_hub_call_dbproxy_handle = new rsp_cb_hub_call_dbproxy(modules);
            }
        }

        public hub_call_dbproxy_reg_hub_cb reg_hub(string hub_name){
            Interlocked.Increment(ref uuid_e713438c_e791_3714_ad31_4ccbddee2554);
            var uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = uuid;

            var _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = new ArrayList();
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106);
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.Add(hub_name);
            call_module_method("reg_hub", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);

            var cb_reg_hub_obj = new hub_call_dbproxy_reg_hub_cb();
            rsp_cb_hub_call_dbproxy_handle.map_reg_hub.Add(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, cb_reg_hub_obj);
            return cb_reg_hub_obj;
        }

        public hub_call_dbproxy_create_persisted_object_cb create_persisted_object(string db, string collection, byte[] object_info){
            Interlocked.Increment(ref uuid_e713438c_e791_3714_ad31_4ccbddee2554);
            var uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb = uuid;

            var _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607 = new ArrayList();
            _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.Add(uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb);
            _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.Add(db);
            _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.Add(collection);
            _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.Add(object_info);
            call_module_method("create_persisted_object", _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607);

            var cb_create_persisted_object_obj = new hub_call_dbproxy_create_persisted_object_cb();
            rsp_cb_hub_call_dbproxy_handle.map_create_persisted_object.Add(uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb, cb_create_persisted_object_obj);
            return cb_create_persisted_object_obj;
        }

        public hub_call_dbproxy_updata_persisted_object_cb updata_persisted_object(string db, string collection, byte[] query_json, byte[] object_info){
            Interlocked.Increment(ref uuid_e713438c_e791_3714_ad31_4ccbddee2554);
            var uuid_7864a402_2d75_5c02_b24b_50287a06732f = uuid;

            var _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425 = new ArrayList();
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(uuid_7864a402_2d75_5c02_b24b_50287a06732f);
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(db);
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(collection);
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(query_json);
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(object_info);
            call_module_method("updata_persisted_object", _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425);

            var cb_updata_persisted_object_obj = new hub_call_dbproxy_updata_persisted_object_cb();
            rsp_cb_hub_call_dbproxy_handle.map_updata_persisted_object.Add(uuid_7864a402_2d75_5c02_b24b_50287a06732f, cb_updata_persisted_object_obj);
            return cb_updata_persisted_object_obj;
        }

        public hub_call_dbproxy_remove_object_cb remove_object(string db, string collection, byte[] query_json){
            Interlocked.Increment(ref uuid_e713438c_e791_3714_ad31_4ccbddee2554);
            var uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f = uuid;

            var _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da = new ArrayList();
            _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.Add(uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f);
            _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.Add(db);
            _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.Add(collection);
            _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.Add(query_json);
            call_module_method("remove_object", _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da);

            var cb_remove_object_obj = new hub_call_dbproxy_remove_object_cb();
            rsp_cb_hub_call_dbproxy_handle.map_remove_object.Add(uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f, cb_remove_object_obj);
            return cb_remove_object_obj;
        }

        public void get_object_info(string db, string collection, byte[] query_json, string callbackid){
            var _argv_1f17e6de_d423_391b_a599_7268e665a53f = new ArrayList();
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.Add(db);
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.Add(collection);
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.Add(query_json);
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.Add(callbackid);
            call_module_method("get_object_info", _argv_1f17e6de_d423_391b_a599_7268e665a53f);
        }

        public hub_call_dbproxy_get_object_count_cb get_object_count(string db, string collection, byte[] query_json){
            Interlocked.Increment(ref uuid_e713438c_e791_3714_ad31_4ccbddee2554);
            var uuid_975425f5_8baf_5905_beeb_4454e78907f6 = uuid;

            var _argv_2632cded_162c_3a9b_86ee_462b614cbeea = new ArrayList();
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.Add(uuid_975425f5_8baf_5905_beeb_4454e78907f6);
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.Add(db);
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.Add(collection);
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.Add(query_json);
            call_module_method("get_object_count", _argv_2632cded_162c_3a9b_86ee_462b614cbeea);

            var cb_get_object_count_obj = new hub_call_dbproxy_get_object_count_cb();
            rsp_cb_hub_call_dbproxy_handle.map_get_object_count.Add(uuid_975425f5_8baf_5905_beeb_4454e78907f6, cb_get_object_count_obj);
            return cb_get_object_count_obj;
        }

    }
/*this module code is codegen by abelkhan codegen for c#*/
    public class dbproxy_call_hub_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public dbproxy_call_hub_module(abelkhan.modulemng _modules) : base("dbproxy_call_hub")
        {
            modules = _modules;
            modules.reg_module(this);

            reg_method("ack_get_object_info", ack_get_object_info);
            reg_method("ack_get_object_info_end", ack_get_object_info_end);
        }

        public event Action<string, byte[]> on_ack_get_object_info;
        public void ack_get_object_info(ArrayList inArray){
            var _callbackid = (string)inArray[0];
            var _object_info = (byte[])inArray[1];
            if (on_ack_get_object_info != null){
                on_ack_get_object_info(_callbackid, _object_info);
            }
        }

        public event Action<string> on_ack_get_object_info_end;
        public void ack_get_object_info_end(ArrayList inArray){
            var _callbackid = (string)inArray[0];
            if (on_ack_get_object_info_end != null){
                on_ack_get_object_info_end(_callbackid);
            }
        }

    }
    public class hub_call_dbproxy_reg_hub_rsp : abelkhan.Response {
        private UInt64 uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67;
        public hub_call_dbproxy_reg_hub_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_dbproxy_rsp_cb", _ch)
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

    public class hub_call_dbproxy_create_persisted_object_rsp : abelkhan.Response {
        private UInt64 uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b;
        public hub_call_dbproxy_create_persisted_object_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b = _uuid;
        }

        public void rsp(){
            var _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607 = new ArrayList();
            _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.Add(uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b);
            call_module_method("create_persisted_object_rsp", _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607);
        }

        public void err(){
            var _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607 = new ArrayList();
            _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.Add(uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b);
            call_module_method("create_persisted_object_err", _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607);
        }

    }

    public class hub_call_dbproxy_updata_persisted_object_rsp : abelkhan.Response {
        private UInt64 uuid_16267d40_cddc_312f_87c0_185a55b79ad2;
        public hub_call_dbproxy_updata_persisted_object_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_16267d40_cddc_312f_87c0_185a55b79ad2 = _uuid;
        }

        public void rsp(){
            var _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425 = new ArrayList();
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(uuid_16267d40_cddc_312f_87c0_185a55b79ad2);
            call_module_method("updata_persisted_object_rsp", _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425);
        }

        public void err(){
            var _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425 = new ArrayList();
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.Add(uuid_16267d40_cddc_312f_87c0_185a55b79ad2);
            call_module_method("updata_persisted_object_err", _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425);
        }

    }

    public class hub_call_dbproxy_remove_object_rsp : abelkhan.Response {
        private UInt64 uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1;
        public hub_call_dbproxy_remove_object_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1 = _uuid;
        }

        public void rsp(){
            var _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da = new ArrayList();
            _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.Add(uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1);
            call_module_method("remove_object_rsp", _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da);
        }

        public void err(){
            var _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da = new ArrayList();
            _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.Add(uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1);
            call_module_method("remove_object_err", _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da);
        }

    }

    public class hub_call_dbproxy_get_object_count_rsp : abelkhan.Response {
        private UInt64 uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2;
        public hub_call_dbproxy_get_object_count_rsp(abelkhan.Ichannel _ch, UInt64 _uuid) : base("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2 = _uuid;
        }

        public void rsp(UInt32 count){
            var _argv_2632cded_162c_3a9b_86ee_462b614cbeea = new ArrayList();
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.Add(uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2);
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.Add(count);
            call_module_method("get_object_count_rsp", _argv_2632cded_162c_3a9b_86ee_462b614cbeea);
        }

        public void err(){
            var _argv_2632cded_162c_3a9b_86ee_462b614cbeea = new ArrayList();
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.Add(uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2);
            call_module_method("get_object_count_err", _argv_2632cded_162c_3a9b_86ee_462b614cbeea);
        }

    }

    public class hub_call_dbproxy_module : abelkhan.Imodule {
        private abelkhan.modulemng modules;
        public hub_call_dbproxy_module(abelkhan.modulemng _modules) : base("hub_call_dbproxy")
        {
            modules = _modules;
            modules.reg_module(this);

            reg_method("reg_hub", reg_hub);
            reg_method("create_persisted_object", create_persisted_object);
            reg_method("updata_persisted_object", updata_persisted_object);
            reg_method("remove_object", remove_object);
            reg_method("get_object_info", get_object_info);
            reg_method("get_object_count", get_object_count);
        }

        public event Action<string> on_reg_hub;
        public void reg_hub(ArrayList inArray){
            var _cb_uuid = (UInt64)inArray[0];
            var _hub_name = (string)inArray[1];
            rsp = new hub_call_dbproxy_reg_hub_rsp(current_ch, _cb_uuid);
            if (on_reg_hub != null){
                on_reg_hub(_hub_name);
            }
            rsp = null;
        }

        public event Action<string, string, byte[]> on_create_persisted_object;
        public void create_persisted_object(ArrayList inArray){
            var _cb_uuid = (UInt64)inArray[0];
            var _db = (string)inArray[1];
            var _collection = (string)inArray[2];
            var _object_info = (byte[])inArray[3];
            rsp = new hub_call_dbproxy_create_persisted_object_rsp(current_ch, _cb_uuid);
            if (on_create_persisted_object != null){
                on_create_persisted_object(_db, _collection, _object_info);
            }
            rsp = null;
        }

        public event Action<string, string, byte[], byte[]> on_updata_persisted_object;
        public void updata_persisted_object(ArrayList inArray){
            var _cb_uuid = (UInt64)inArray[0];
            var _db = (string)inArray[1];
            var _collection = (string)inArray[2];
            var _query_json = (byte[])inArray[3];
            var _object_info = (byte[])inArray[4];
            rsp = new hub_call_dbproxy_updata_persisted_object_rsp(current_ch, _cb_uuid);
            if (on_updata_persisted_object != null){
                on_updata_persisted_object(_db, _collection, _query_json, _object_info);
            }
            rsp = null;
        }

        public event Action<string, string, byte[]> on_remove_object;
        public void remove_object(ArrayList inArray){
            var _cb_uuid = (UInt64)inArray[0];
            var _db = (string)inArray[1];
            var _collection = (string)inArray[2];
            var _query_json = (byte[])inArray[3];
            rsp = new hub_call_dbproxy_remove_object_rsp(current_ch, _cb_uuid);
            if (on_remove_object != null){
                on_remove_object(_db, _collection, _query_json);
            }
            rsp = null;
        }

        public event Action<string, string, byte[], string> on_get_object_info;
        public void get_object_info(ArrayList inArray){
            var _db = (string)inArray[0];
            var _collection = (string)inArray[1];
            var _query_json = (byte[])inArray[2];
            var _callbackid = (string)inArray[3];
            if (on_get_object_info != null){
                on_get_object_info(_db, _collection, _query_json, _callbackid);
            }
        }

        public event Action<string, string, byte[]> on_get_object_count;
        public void get_object_count(ArrayList inArray){
            var _cb_uuid = (UInt64)inArray[0];
            var _db = (string)inArray[1];
            var _collection = (string)inArray[2];
            var _query_json = (byte[])inArray[3];
            rsp = new hub_call_dbproxy_get_object_count_rsp(current_ch, _cb_uuid);
            if (on_get_object_count != null){
                on_get_object_count(_db, _collection, _query_json);
            }
            rsp = null;
        }

    }

}
