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
    public class rank_svr_service_update_rank_item_cb
    {
        private UInt64 cb_uuid;
        private rank_svr_service_rsp_cb module_rsp_cb;

        public rank_svr_service_update_rank_item_cb(UInt64 _cb_uuid, rank_svr_service_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<Int32> on_update_rank_item_cb;
        public event Action on_update_rank_item_err;
        public event Action on_update_rank_item_timeout;

        public rank_svr_service_update_rank_item_cb callBack(Action<Int32> cb, Action err)
        {
            on_update_rank_item_cb += cb;
            on_update_rank_item_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.update_rank_item_timeout(cb_uuid);
            });
            on_update_rank_item_timeout += timeout_cb;
        }

        public void call_cb(Int32 rank)
        {
            if (on_update_rank_item_cb != null)
            {
                on_update_rank_item_cb(rank);
            }
        }

        public void call_err()
        {
            if (on_update_rank_item_err != null)
            {
                on_update_rank_item_err();
            }
        }

        public void call_timeout()
        {
            if (on_update_rank_item_timeout != null)
            {
                on_update_rank_item_timeout();
            }
        }

    }

    public class rank_svr_service_get_rank_guid_cb
    {
        private UInt64 cb_uuid;
        private rank_svr_service_rsp_cb module_rsp_cb;

        public rank_svr_service_get_rank_guid_cb(UInt64 _cb_uuid, rank_svr_service_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<Int32> on_get_rank_guid_cb;
        public event Action on_get_rank_guid_err;
        public event Action on_get_rank_guid_timeout;

        public rank_svr_service_get_rank_guid_cb callBack(Action<Int32> cb, Action err)
        {
            on_get_rank_guid_cb += cb;
            on_get_rank_guid_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.get_rank_guid_timeout(cb_uuid);
            });
            on_get_rank_guid_timeout += timeout_cb;
        }

        public void call_cb(Int32 rank)
        {
            if (on_get_rank_guid_cb != null)
            {
                on_get_rank_guid_cb(rank);
            }
        }

        public void call_err()
        {
            if (on_get_rank_guid_err != null)
            {
                on_get_rank_guid_err();
            }
        }

        public void call_timeout()
        {
            if (on_get_rank_guid_timeout != null)
            {
                on_get_rank_guid_timeout();
            }
        }

    }

/*this cb code is codegen by abelkhan for c#*/
    public class rank_svr_service_rsp_cb : Common.IModule {
        public Dictionary<UInt64, rank_svr_service_update_rank_item_cb> map_update_rank_item;
        public Dictionary<UInt64, rank_svr_service_get_rank_guid_cb> map_get_rank_guid;
        public rank_svr_service_rsp_cb()
        {
            map_update_rank_item = new Dictionary<UInt64, rank_svr_service_update_rank_item_cb>();
            Hub.Hub._modules.add_mothed("rank_svr_service_rsp_cb_update_rank_item_rsp", update_rank_item_rsp);
            Hub.Hub._modules.add_mothed("rank_svr_service_rsp_cb_update_rank_item_err", update_rank_item_err);
            map_get_rank_guid = new Dictionary<UInt64, rank_svr_service_get_rank_guid_cb>();
            Hub.Hub._modules.add_mothed("rank_svr_service_rsp_cb_get_rank_guid_rsp", get_rank_guid_rsp);
            Hub.Hub._modules.add_mothed("rank_svr_service_rsp_cb_get_rank_guid_err", get_rank_guid_err);
        }

        public void update_rank_item_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _rank = ((MsgPack.MessagePackObject)inArray[1]).AsInt32();
            var rsp = try_get_and_del_update_rank_item_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_rank);
            }
        }

        public void update_rank_item_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_update_rank_item_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void update_rank_item_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_update_rank_item_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private rank_svr_service_update_rank_item_cb try_get_and_del_update_rank_item_cb(UInt64 uuid){
            lock(map_update_rank_item)
            {
                if (map_update_rank_item.TryGetValue(uuid, out rank_svr_service_update_rank_item_cb rsp))
                {
                    map_update_rank_item.Remove(uuid);
                }
                return rsp;
            }
        }

        public void get_rank_guid_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _rank = ((MsgPack.MessagePackObject)inArray[1]).AsInt32();
            var rsp = try_get_and_del_get_rank_guid_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_rank);
            }
        }

        public void get_rank_guid_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_get_rank_guid_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void get_rank_guid_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_get_rank_guid_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private rank_svr_service_get_rank_guid_cb try_get_and_del_get_rank_guid_cb(UInt64 uuid){
            lock(map_get_rank_guid)
            {
                if (map_get_rank_guid.TryGetValue(uuid, out rank_svr_service_get_rank_guid_cb rsp))
                {
                    map_get_rank_guid.Remove(uuid);
                }
                return rsp;
            }
        }

    }

    public class rank_svr_service_caller {
        public static rank_svr_service_rsp_cb rsp_cb_rank_svr_service_handle = null;
        private ThreadLocal<rank_svr_service_hubproxy> _hubproxy;
        public rank_svr_service_caller()
        {
            if (rsp_cb_rank_svr_service_handle == null)
            {
                rsp_cb_rank_svr_service_handle = new rank_svr_service_rsp_cb();
            }
            _hubproxy = new ThreadLocal<rank_svr_service_hubproxy>();
        }

        public rank_svr_service_hubproxy get_hub(string hub_name) {
            if (_hubproxy.Value == null)
{
                _hubproxy.Value = new rank_svr_service_hubproxy(rsp_cb_rank_svr_service_handle);
            }
            _hubproxy.Value.hub_name_77c3d96c_b4b4_34f4_8512_6b4adba08663 = hub_name;
            return _hubproxy.Value;
        }

    }

    public class rank_svr_service_hubproxy {
        public string hub_name_77c3d96c_b4b4_34f4_8512_6b4adba08663;
        private Int32 uuid_77c3d96c_b4b4_34f4_8512_6b4adba08663 = (Int32)RandomUUID.random();

        private rank_svr_service_rsp_cb rsp_cb_rank_svr_service_handle;

        public rank_svr_service_hubproxy(rank_svr_service_rsp_cb rsp_cb_rank_svr_service_handle_)
        {
            rsp_cb_rank_svr_service_handle = rsp_cb_rank_svr_service_handle_;
        }

        public rank_svr_service_update_rank_item_cb update_rank_item(string rank_name, rank_item item){
            var uuid_ed0fa482_37bd_5abe_916e_3cca2f1b8b4d = (UInt64)Interlocked.Increment(ref uuid_77c3d96c_b4b4_34f4_8512_6b4adba08663);

            var _argv_a8ebcc3f_b73d_3dad_922b_4fe810788aec = new ArrayList();
            _argv_a8ebcc3f_b73d_3dad_922b_4fe810788aec.Add(uuid_ed0fa482_37bd_5abe_916e_3cca2f1b8b4d);
            _argv_a8ebcc3f_b73d_3dad_922b_4fe810788aec.Add(rank_name);
            _argv_a8ebcc3f_b73d_3dad_922b_4fe810788aec.Add(rank_item.rank_item_to_protcol(item));
            Hub.Hub._hubs.call_hub(hub_name_77c3d96c_b4b4_34f4_8512_6b4adba08663, "rank_svr_service_update_rank_item", _argv_a8ebcc3f_b73d_3dad_922b_4fe810788aec);

            var cb_update_rank_item_obj = new rank_svr_service_update_rank_item_cb(uuid_ed0fa482_37bd_5abe_916e_3cca2f1b8b4d, rsp_cb_rank_svr_service_handle);
            lock(rsp_cb_rank_svr_service_handle.map_update_rank_item)
            {
                rsp_cb_rank_svr_service_handle.map_update_rank_item.Add(uuid_ed0fa482_37bd_5abe_916e_3cca2f1b8b4d, cb_update_rank_item_obj);
            }
            return cb_update_rank_item_obj;
        }

        public rank_svr_service_get_rank_guid_cb get_rank_guid(string rank_name, Int64 guid){
            var uuid_aae4e0b8_0b60_55bf_a8e6_2afd98164325 = (UInt64)Interlocked.Increment(ref uuid_77c3d96c_b4b4_34f4_8512_6b4adba08663);

            var _argv_90f752ce_ee17_38de_b679_4a35e21e4129 = new ArrayList();
            _argv_90f752ce_ee17_38de_b679_4a35e21e4129.Add(uuid_aae4e0b8_0b60_55bf_a8e6_2afd98164325);
            _argv_90f752ce_ee17_38de_b679_4a35e21e4129.Add(rank_name);
            _argv_90f752ce_ee17_38de_b679_4a35e21e4129.Add(guid);
            Hub.Hub._hubs.call_hub(hub_name_77c3d96c_b4b4_34f4_8512_6b4adba08663, "rank_svr_service_get_rank_guid", _argv_90f752ce_ee17_38de_b679_4a35e21e4129);

            var cb_get_rank_guid_obj = new rank_svr_service_get_rank_guid_cb(uuid_aae4e0b8_0b60_55bf_a8e6_2afd98164325, rsp_cb_rank_svr_service_handle);
            lock(rsp_cb_rank_svr_service_handle.map_get_rank_guid)
            {
                rsp_cb_rank_svr_service_handle.map_get_rank_guid.Add(uuid_aae4e0b8_0b60_55bf_a8e6_2afd98164325, cb_get_rank_guid_obj);
            }
            return cb_get_rank_guid_obj;
        }

    }
/*this module code is codegen by abelkhan codegen for c#*/
    public class rank_svr_service_update_rank_item_rsp : Common.Response {
        private string _hub_name_a8ebcc3f_b73d_3dad_922b_4fe810788aec;
        private UInt64 uuid_cc4e15be_006c_30dd_9030_022e68eed639;
        public rank_svr_service_update_rank_item_rsp(string hub_name, UInt64 _uuid) 
        {
            _hub_name_a8ebcc3f_b73d_3dad_922b_4fe810788aec = hub_name;
            uuid_cc4e15be_006c_30dd_9030_022e68eed639 = _uuid;
        }

        public void rsp(Int32 rank_4c7a18ad_077b_37ed_8d4e_92440bd00c2b){
            var _argv_a8ebcc3f_b73d_3dad_922b_4fe810788aec = new ArrayList();
            _argv_a8ebcc3f_b73d_3dad_922b_4fe810788aec.Add(uuid_cc4e15be_006c_30dd_9030_022e68eed639);
            _argv_a8ebcc3f_b73d_3dad_922b_4fe810788aec.Add(rank_4c7a18ad_077b_37ed_8d4e_92440bd00c2b);
            Hub.Hub._hubs.call_hub(_hub_name_a8ebcc3f_b73d_3dad_922b_4fe810788aec, "rank_svr_service_rsp_cb_update_rank_item_rsp", _argv_a8ebcc3f_b73d_3dad_922b_4fe810788aec);
        }

        public void err(){
            var _argv_a8ebcc3f_b73d_3dad_922b_4fe810788aec = new ArrayList();
            _argv_a8ebcc3f_b73d_3dad_922b_4fe810788aec.Add(uuid_cc4e15be_006c_30dd_9030_022e68eed639);
            Hub.Hub._hubs.call_hub(_hub_name_a8ebcc3f_b73d_3dad_922b_4fe810788aec, "rank_svr_service_rsp_cb_update_rank_item_err", _argv_a8ebcc3f_b73d_3dad_922b_4fe810788aec);
        }

    }

    public class rank_svr_service_get_rank_guid_rsp : Common.Response {
        private string _hub_name_90f752ce_ee17_38de_b679_4a35e21e4129;
        private UInt64 uuid_3eca1bfd_46e9_32ae_ac6c_bba1510a64e4;
        public rank_svr_service_get_rank_guid_rsp(string hub_name, UInt64 _uuid) 
        {
            _hub_name_90f752ce_ee17_38de_b679_4a35e21e4129 = hub_name;
            uuid_3eca1bfd_46e9_32ae_ac6c_bba1510a64e4 = _uuid;
        }

        public void rsp(Int32 rank_4c7a18ad_077b_37ed_8d4e_92440bd00c2b){
            var _argv_90f752ce_ee17_38de_b679_4a35e21e4129 = new ArrayList();
            _argv_90f752ce_ee17_38de_b679_4a35e21e4129.Add(uuid_3eca1bfd_46e9_32ae_ac6c_bba1510a64e4);
            _argv_90f752ce_ee17_38de_b679_4a35e21e4129.Add(rank_4c7a18ad_077b_37ed_8d4e_92440bd00c2b);
            Hub.Hub._hubs.call_hub(_hub_name_90f752ce_ee17_38de_b679_4a35e21e4129, "rank_svr_service_rsp_cb_get_rank_guid_rsp", _argv_90f752ce_ee17_38de_b679_4a35e21e4129);
        }

        public void err(){
            var _argv_90f752ce_ee17_38de_b679_4a35e21e4129 = new ArrayList();
            _argv_90f752ce_ee17_38de_b679_4a35e21e4129.Add(uuid_3eca1bfd_46e9_32ae_ac6c_bba1510a64e4);
            Hub.Hub._hubs.call_hub(_hub_name_90f752ce_ee17_38de_b679_4a35e21e4129, "rank_svr_service_rsp_cb_get_rank_guid_err", _argv_90f752ce_ee17_38de_b679_4a35e21e4129);
        }

    }

    public class rank_svr_service_module : Common.IModule {
        public rank_svr_service_module() 
        {
            Hub.Hub._modules.add_mothed("rank_svr_service_update_rank_item", update_rank_item);
            Hub.Hub._modules.add_mothed("rank_svr_service_get_rank_guid", get_rank_guid);
        }

        public event Action<string, rank_item> on_update_rank_item;
        public void update_rank_item(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _rank_name = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _item = rank_item.protcol_to_rank_item(((MsgPack.MessagePackObject)inArray[2]).AsDictionary());
            rsp = new rank_svr_service_update_rank_item_rsp(Hub.Hub._hubs.current_hubproxy.name, _cb_uuid);
            if (on_update_rank_item != null){
                on_update_rank_item(_rank_name, _item);
            }
            rsp = null;
        }

        public event Action<string, Int64> on_get_rank_guid;
        public void get_rank_guid(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _rank_name = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _guid = ((MsgPack.MessagePackObject)inArray[2]).AsInt64();
            rsp = new rank_svr_service_get_rank_guid_rsp(Hub.Hub._hubs.current_hubproxy.name, _cb_uuid);
            if (on_get_rank_guid != null){
                on_get_rank_guid(_rank_name, _guid);
            }
            rsp = null;
        }

    }

}
