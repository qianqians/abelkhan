using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MsgPack.Serialization;

namespace Abelkhan
{
/*this enum code is codegen by abelkhan codegen for c#*/

/*this struct code is codegen by abelkhan codegen for c#*/
/*this caller code is codegen by abelkhan codegen for c#*/
    public class avatar_migrate_avatar_cb
    {
        private UInt64 cb_uuid;
        private avatar_rsp_cb module_rsp_cb;

        public avatar_migrate_avatar_cb(UInt64 _cb_uuid, avatar_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<Int64, byte[]> on_migrate_avatar_cb;
        public event Action on_migrate_avatar_err;
        public event Action on_migrate_avatar_timeout;

        public avatar_migrate_avatar_cb callBack(Action<Int64, byte[]> cb, Action err)
        {
            on_migrate_avatar_cb += cb;
            on_migrate_avatar_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.migrate_avatar_timeout(cb_uuid);
            });
            on_migrate_avatar_timeout += timeout_cb;
        }

        public void call_cb(Int64 guid, byte[] doc)
        {
            if (on_migrate_avatar_cb != null)
            {
                on_migrate_avatar_cb(guid, doc);
            }
        }

        public void call_err()
        {
            if (on_migrate_avatar_err != null)
            {
                on_migrate_avatar_err();
            }
        }

        public void call_timeout()
        {
            if (on_migrate_avatar_timeout != null)
            {
                on_migrate_avatar_timeout();
            }
        }

    }

    public class avatar_get_remote_avatar_cb
    {
        private UInt64 cb_uuid;
        private avatar_rsp_cb module_rsp_cb;

        public avatar_get_remote_avatar_cb(UInt64 _cb_uuid, avatar_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<Int64, byte[]> on_get_remote_avatar_cb;
        public event Action on_get_remote_avatar_err;
        public event Action on_get_remote_avatar_timeout;

        public avatar_get_remote_avatar_cb callBack(Action<Int64, byte[]> cb, Action err)
        {
            on_get_remote_avatar_cb += cb;
            on_get_remote_avatar_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.get_remote_avatar_timeout(cb_uuid);
            });
            on_get_remote_avatar_timeout += timeout_cb;
        }

        public void call_cb(Int64 guid, byte[] doc)
        {
            if (on_get_remote_avatar_cb != null)
            {
                on_get_remote_avatar_cb(guid, doc);
            }
        }

        public void call_err()
        {
            if (on_get_remote_avatar_err != null)
            {
                on_get_remote_avatar_err();
            }
        }

        public void call_timeout()
        {
            if (on_get_remote_avatar_timeout != null)
            {
                on_get_remote_avatar_timeout();
            }
        }

    }

/*this cb code is codegen by abelkhan for c#*/
    public class avatar_rsp_cb : Common.IModule {
        public Dictionary<UInt64, avatar_migrate_avatar_cb> map_migrate_avatar;
        public Dictionary<UInt64, avatar_get_remote_avatar_cb> map_get_remote_avatar;
        public avatar_rsp_cb()
        {
            map_migrate_avatar = new Dictionary<UInt64, avatar_migrate_avatar_cb>();
            Hub.Hub._modules.add_mothed("avatar_rsp_cb_migrate_avatar_rsp", migrate_avatar_rsp);
            Hub.Hub._modules.add_mothed("avatar_rsp_cb_migrate_avatar_err", migrate_avatar_err);
            map_get_remote_avatar = new Dictionary<UInt64, avatar_get_remote_avatar_cb>();
            Hub.Hub._modules.add_mothed("avatar_rsp_cb_get_remote_avatar_rsp", get_remote_avatar_rsp);
            Hub.Hub._modules.add_mothed("avatar_rsp_cb_get_remote_avatar_err", get_remote_avatar_err);
        }

        public void migrate_avatar_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _guid = ((MsgPack.MessagePackObject)inArray[1]).AsInt64();
            var _doc = ((MsgPack.MessagePackObject)inArray[2]).AsBinary();
            var rsp = try_get_and_del_migrate_avatar_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_guid, _doc);
            }
        }

        public void migrate_avatar_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_migrate_avatar_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void migrate_avatar_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_migrate_avatar_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private avatar_migrate_avatar_cb try_get_and_del_migrate_avatar_cb(UInt64 uuid){
            lock(map_migrate_avatar)
            {
                if (map_migrate_avatar.TryGetValue(uuid, out avatar_migrate_avatar_cb rsp))
                {
                    map_migrate_avatar.Remove(uuid);
                }
                return rsp;
            }
        }

        public void get_remote_avatar_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _guid = ((MsgPack.MessagePackObject)inArray[1]).AsInt64();
            var _doc = ((MsgPack.MessagePackObject)inArray[2]).AsBinary();
            var rsp = try_get_and_del_get_remote_avatar_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_guid, _doc);
            }
        }

        public void get_remote_avatar_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_get_remote_avatar_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void get_remote_avatar_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_get_remote_avatar_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private avatar_get_remote_avatar_cb try_get_and_del_get_remote_avatar_cb(UInt64 uuid){
            lock(map_get_remote_avatar)
            {
                if (map_get_remote_avatar.TryGetValue(uuid, out avatar_get_remote_avatar_cb rsp))
                {
                    map_get_remote_avatar.Remove(uuid);
                }
                return rsp;
            }
        }

    }

    public class avatar_caller {
        public static avatar_rsp_cb rsp_cb_avatar_handle = null;
        private ThreadLocal<avatar_hubproxy> _hubproxy;
        public avatar_caller()
        {
            if (rsp_cb_avatar_handle == null)
            {
                rsp_cb_avatar_handle = new avatar_rsp_cb();
            }
            _hubproxy = new ThreadLocal<avatar_hubproxy>();
        }

        public avatar_hubproxy get_hub(string hub_name) {
            if (_hubproxy.Value == null)
{
                _hubproxy.Value = new avatar_hubproxy(rsp_cb_avatar_handle);
            }
            _hubproxy.Value.hub_name_3f335b73_f48e_3b8a_a9b6_e3c5a534be4f = hub_name;
            return _hubproxy.Value;
        }

    }

    public class avatar_hubproxy {
        public string hub_name_3f335b73_f48e_3b8a_a9b6_e3c5a534be4f;
        private Int32 uuid_3f335b73_f48e_3b8a_a9b6_e3c5a534be4f = (Int32)RandomUUID.random();

        private avatar_rsp_cb rsp_cb_avatar_handle;

        public avatar_hubproxy(avatar_rsp_cb rsp_cb_avatar_handle_)
        {
            rsp_cb_avatar_handle = rsp_cb_avatar_handle_;
        }

        public avatar_migrate_avatar_cb migrate_avatar(string client_uuid){
            var uuid_613b7463_be74_5bdd_b935_db0b9291695a = (UInt64)Interlocked.Increment(ref uuid_3f335b73_f48e_3b8a_a9b6_e3c5a534be4f);

            var _argv_23d57a8b_7366_36ed_92d0_45e578320579 = new ArrayList();
            _argv_23d57a8b_7366_36ed_92d0_45e578320579.Add(uuid_613b7463_be74_5bdd_b935_db0b9291695a);
            _argv_23d57a8b_7366_36ed_92d0_45e578320579.Add(client_uuid);
            Hub.Hub._hubs.call_hub(hub_name_3f335b73_f48e_3b8a_a9b6_e3c5a534be4f, "avatar_migrate_avatar", _argv_23d57a8b_7366_36ed_92d0_45e578320579);

            var cb_migrate_avatar_obj = new avatar_migrate_avatar_cb(uuid_613b7463_be74_5bdd_b935_db0b9291695a, rsp_cb_avatar_handle);
            lock(rsp_cb_avatar_handle.map_migrate_avatar)
            {
                rsp_cb_avatar_handle.map_migrate_avatar.Add(uuid_613b7463_be74_5bdd_b935_db0b9291695a, cb_migrate_avatar_obj);
            }
            return cb_migrate_avatar_obj;
        }

        public avatar_get_remote_avatar_cb get_remote_avatar(string client_uuid){
            var uuid_ebef94cb_599c_554a_982d_9e11bd6ce8c3 = (UInt64)Interlocked.Increment(ref uuid_3f335b73_f48e_3b8a_a9b6_e3c5a534be4f);

            var _argv_f7981503_551c_35e6_8df5_ca1daba221e7 = new ArrayList();
            _argv_f7981503_551c_35e6_8df5_ca1daba221e7.Add(uuid_ebef94cb_599c_554a_982d_9e11bd6ce8c3);
            _argv_f7981503_551c_35e6_8df5_ca1daba221e7.Add(client_uuid);
            Hub.Hub._hubs.call_hub(hub_name_3f335b73_f48e_3b8a_a9b6_e3c5a534be4f, "avatar_get_remote_avatar", _argv_f7981503_551c_35e6_8df5_ca1daba221e7);

            var cb_get_remote_avatar_obj = new avatar_get_remote_avatar_cb(uuid_ebef94cb_599c_554a_982d_9e11bd6ce8c3, rsp_cb_avatar_handle);
            lock(rsp_cb_avatar_handle.map_get_remote_avatar)
            {
                rsp_cb_avatar_handle.map_get_remote_avatar.Add(uuid_ebef94cb_599c_554a_982d_9e11bd6ce8c3, cb_get_remote_avatar_obj);
            }
            return cb_get_remote_avatar_obj;
        }

    }
/*this module code is codegen by abelkhan codegen for c#*/
    public class avatar_migrate_avatar_rsp : Common.Response {
        private string _hub_name_23d57a8b_7366_36ed_92d0_45e578320579;
        private UInt64 uuid_cac0d70e_861b_3aa8_b5b4_eeaa5b63f2e2;
        public avatar_migrate_avatar_rsp(string hub_name, UInt64 _uuid) 
        {
            _hub_name_23d57a8b_7366_36ed_92d0_45e578320579 = hub_name;
            uuid_cac0d70e_861b_3aa8_b5b4_eeaa5b63f2e2 = _uuid;
        }

        public void rsp(Int64 guid_25cce41a_8adf_3dfe_9b75_9eb957e9743c, byte[] doc_6217a4bb_e21e_3fa1_a1c6_ad1267d01e4d){
            var _argv_23d57a8b_7366_36ed_92d0_45e578320579 = new ArrayList();
            _argv_23d57a8b_7366_36ed_92d0_45e578320579.Add(uuid_cac0d70e_861b_3aa8_b5b4_eeaa5b63f2e2);
            _argv_23d57a8b_7366_36ed_92d0_45e578320579.Add(guid_25cce41a_8adf_3dfe_9b75_9eb957e9743c);
            _argv_23d57a8b_7366_36ed_92d0_45e578320579.Add(doc_6217a4bb_e21e_3fa1_a1c6_ad1267d01e4d);
            Hub.Hub._hubs.call_hub(_hub_name_23d57a8b_7366_36ed_92d0_45e578320579, "avatar_rsp_cb_migrate_avatar_rsp", _argv_23d57a8b_7366_36ed_92d0_45e578320579);
        }

        public void err(){
            var _argv_23d57a8b_7366_36ed_92d0_45e578320579 = new ArrayList();
            _argv_23d57a8b_7366_36ed_92d0_45e578320579.Add(uuid_cac0d70e_861b_3aa8_b5b4_eeaa5b63f2e2);
            Hub.Hub._hubs.call_hub(_hub_name_23d57a8b_7366_36ed_92d0_45e578320579, "avatar_rsp_cb_migrate_avatar_err", _argv_23d57a8b_7366_36ed_92d0_45e578320579);
        }

    }

    public class avatar_get_remote_avatar_rsp : Common.Response {
        private string _hub_name_f7981503_551c_35e6_8df5_ca1daba221e7;
        private UInt64 uuid_3c56f389_4436_3650_a479_d019fab8865d;
        public avatar_get_remote_avatar_rsp(string hub_name, UInt64 _uuid) 
        {
            _hub_name_f7981503_551c_35e6_8df5_ca1daba221e7 = hub_name;
            uuid_3c56f389_4436_3650_a479_d019fab8865d = _uuid;
        }

        public void rsp(Int64 guid_25cce41a_8adf_3dfe_9b75_9eb957e9743c, byte[] doc_6217a4bb_e21e_3fa1_a1c6_ad1267d01e4d){
            var _argv_f7981503_551c_35e6_8df5_ca1daba221e7 = new ArrayList();
            _argv_f7981503_551c_35e6_8df5_ca1daba221e7.Add(uuid_3c56f389_4436_3650_a479_d019fab8865d);
            _argv_f7981503_551c_35e6_8df5_ca1daba221e7.Add(guid_25cce41a_8adf_3dfe_9b75_9eb957e9743c);
            _argv_f7981503_551c_35e6_8df5_ca1daba221e7.Add(doc_6217a4bb_e21e_3fa1_a1c6_ad1267d01e4d);
            Hub.Hub._hubs.call_hub(_hub_name_f7981503_551c_35e6_8df5_ca1daba221e7, "avatar_rsp_cb_get_remote_avatar_rsp", _argv_f7981503_551c_35e6_8df5_ca1daba221e7);
        }

        public void err(){
            var _argv_f7981503_551c_35e6_8df5_ca1daba221e7 = new ArrayList();
            _argv_f7981503_551c_35e6_8df5_ca1daba221e7.Add(uuid_3c56f389_4436_3650_a479_d019fab8865d);
            Hub.Hub._hubs.call_hub(_hub_name_f7981503_551c_35e6_8df5_ca1daba221e7, "avatar_rsp_cb_get_remote_avatar_err", _argv_f7981503_551c_35e6_8df5_ca1daba221e7);
        }

    }

    public class avatar_module : Common.IModule {
        public avatar_module() 
        {
            Hub.Hub._modules.add_mothed("avatar_migrate_avatar", migrate_avatar);
            Hub.Hub._modules.add_mothed("avatar_get_remote_avatar", get_remote_avatar);
        }

        public event Action<string> on_migrate_avatar;
        public void migrate_avatar(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _client_uuid = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            rsp = new avatar_migrate_avatar_rsp(Hub.Hub._hubs.current_hubproxy.name, _cb_uuid);
            if (on_migrate_avatar != null){
                on_migrate_avatar(_client_uuid);
            }
            rsp = null;
        }

        public event Action<string> on_get_remote_avatar;
        public void get_remote_avatar(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _client_uuid = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            rsp = new avatar_get_remote_avatar_rsp(Hub.Hub._hubs.current_hubproxy.name, _cb_uuid);
            if (on_get_remote_avatar != null){
                on_get_remote_avatar(_client_uuid);
            }
            rsp = null;
        }

    }

}
