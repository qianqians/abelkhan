using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MsgPack.Serialization;

namespace Abelkhan
{
/*this enum code is codegen by abelkhan codegen for c#*/

/*this struct code is codegen by abelkhan codegen for c#*/
    public class item_def
    {
        public Int64 uuid;
        public Int64 desc_id;
        public Int32 limit_amount;
        public Int32 amount;
        public static MsgPack.MessagePackObjectDictionary item_def_to_protcol(item_def _struct){
            var _protocol = new MsgPack.MessagePackObjectDictionary();
            _protocol.Add("uuid", _struct.uuid);
            _protocol.Add("desc_id", _struct.desc_id);
            _protocol.Add("limit_amount", _struct.limit_amount);
            _protocol.Add("amount", _struct.amount);
            return _protocol;
        }
        public static item_def protcol_to_item_def(MsgPack.MessagePackObjectDictionary _protocol){
            var _struct18820fa0_02d9_3ef6_8c40_b67521bb4df7 = new item_def();
            foreach (var i in _protocol){
                if (((MsgPack.MessagePackObject)i.Key).AsString() == "uuid"){
                    _struct18820fa0_02d9_3ef6_8c40_b67521bb4df7.uuid = ((MsgPack.MessagePackObject)i.Value).AsInt64();
                }
                else if (((MsgPack.MessagePackObject)i.Key).AsString() == "desc_id"){
                    _struct18820fa0_02d9_3ef6_8c40_b67521bb4df7.desc_id = ((MsgPack.MessagePackObject)i.Value).AsInt64();
                }
                else if (((MsgPack.MessagePackObject)i.Key).AsString() == "limit_amount"){
                    _struct18820fa0_02d9_3ef6_8c40_b67521bb4df7.limit_amount = ((MsgPack.MessagePackObject)i.Value).AsInt32();
                }
                else if (((MsgPack.MessagePackObject)i.Key).AsString() == "amount"){
                    _struct18820fa0_02d9_3ef6_8c40_b67521bb4df7.amount = ((MsgPack.MessagePackObject)i.Value).AsInt32();
                }
            }
            return _struct18820fa0_02d9_3ef6_8c40_b67521bb4df7;
        }
    }

    public class bag_def
    {
        public Int32 capacity;
        public List<item_def> items;
        public static MsgPack.MessagePackObjectDictionary bag_def_to_protcol(bag_def _struct){
            var _protocol = new MsgPack.MessagePackObjectDictionary();
            _protocol.Add("capacity", _struct.capacity);
            if (_struct.items != null) {
                var _array_items = new List<MsgPack.MessagePackObject>();
                foreach(var v_ in _struct.items){
                    _array_items.Add( new MsgPack.MessagePackObject(item_def.item_def_to_protcol(v_)));
                }
                _protocol.Add("items", new MsgPack.MessagePackObject(_array_items));
            }
            return _protocol;
        }
        public static bag_def protcol_to_bag_def(MsgPack.MessagePackObjectDictionary _protocol){
            var _struct36247089_d39f_3082_9189_f3b10b7bd024 = new bag_def();
            foreach (var i in _protocol){
                if (((MsgPack.MessagePackObject)i.Key).AsString() == "capacity"){
                    _struct36247089_d39f_3082_9189_f3b10b7bd024.capacity = ((MsgPack.MessagePackObject)i.Value).AsInt32();
                }
                else if (((MsgPack.MessagePackObject)i.Key).AsString() == "items"){
                    _struct36247089_d39f_3082_9189_f3b10b7bd024.items = new();
                    var _protocol_array = ((MsgPack.MessagePackObject)i.Value).AsList();
                    foreach (var v_ in _protocol_array){
                        _struct36247089_d39f_3082_9189_f3b10b7bd024.items.Add(item_def.protcol_to_item_def(((MsgPack.MessagePackObject)v_).AsDictionary()));
                    }
                }
            }
            return _struct36247089_d39f_3082_9189_f3b10b7bd024;
        }
    }

/*this caller code is codegen by abelkhan codegen for c#*/
    public class bag_service_get_bag_cb
    {
        private UInt64 cb_uuid;
        private bag_service_rsp_cb module_rsp_cb;

        public bag_service_get_bag_cb(UInt64 _cb_uuid, bag_service_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<bag_def> on_get_bag_cb;
        public event Action on_get_bag_err;
        public event Action on_get_bag_timeout;

        public bag_service_get_bag_cb callBack(Action<bag_def> cb, Action err)
        {
            on_get_bag_cb += cb;
            on_get_bag_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.get_bag_timeout(cb_uuid);
            });
            on_get_bag_timeout += timeout_cb;
        }

        public void call_cb(bag_def _bag)
        {
            if (on_get_bag_cb != null)
            {
                on_get_bag_cb(_bag);
            }
        }

        public void call_err()
        {
            if (on_get_bag_err != null)
            {
                on_get_bag_err();
            }
        }

        public void call_timeout()
        {
            if (on_get_bag_timeout != null)
            {
                on_get_bag_timeout();
            }
        }

    }

    public class bag_service_use_item_cb
    {
        private UInt64 cb_uuid;
        private bag_service_rsp_cb module_rsp_cb;

        public bag_service_use_item_cb(UInt64 _cb_uuid, bag_service_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action on_use_item_cb;
        public event Action on_use_item_err;
        public event Action on_use_item_timeout;

        public bag_service_use_item_cb callBack(Action cb, Action err)
        {
            on_use_item_cb += cb;
            on_use_item_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.use_item_timeout(cb_uuid);
            });
            on_use_item_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_use_item_cb != null)
            {
                on_use_item_cb();
            }
        }

        public void call_err()
        {
            if (on_use_item_err != null)
            {
                on_use_item_err();
            }
        }

        public void call_timeout()
        {
            if (on_use_item_timeout != null)
            {
                on_use_item_timeout();
            }
        }

    }

/*this cb code is codegen by abelkhan for c#*/
    public class bag_service_rsp_cb : Common.IModule {
        public Dictionary<UInt64, bag_service_get_bag_cb> map_get_bag;
        public Dictionary<UInt64, bag_service_use_item_cb> map_use_item;
        public bag_service_rsp_cb()
        {
            map_get_bag = new Dictionary<UInt64, bag_service_get_bag_cb>();
            Hub.Hub._modules.add_mothed("bag_service_rsp_cb_get_bag_rsp", get_bag_rsp);
            Hub.Hub._modules.add_mothed("bag_service_rsp_cb_get_bag_err", get_bag_err);
            map_use_item = new Dictionary<UInt64, bag_service_use_item_cb>();
            Hub.Hub._modules.add_mothed("bag_service_rsp_cb_use_item_rsp", use_item_rsp);
            Hub.Hub._modules.add_mothed("bag_service_rsp_cb_use_item_err", use_item_err);
        }

        public void get_bag_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var __bag = bag_def.protcol_to_bag_def(((MsgPack.MessagePackObject)inArray[1]).AsDictionary());
            var rsp = try_get_and_del_get_bag_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(__bag);
            }
        }

        public void get_bag_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_get_bag_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void get_bag_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_get_bag_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private bag_service_get_bag_cb try_get_and_del_get_bag_cb(UInt64 uuid){
            lock(map_get_bag)
            {
                if (map_get_bag.TryGetValue(uuid, out bag_service_get_bag_cb rsp))
                {
                    map_get_bag.Remove(uuid);
                }
                return rsp;
            }
        }

        public void use_item_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_use_item_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void use_item_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_use_item_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void use_item_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_use_item_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private bag_service_use_item_cb try_get_and_del_use_item_cb(UInt64 uuid){
            lock(map_use_item)
            {
                if (map_use_item.TryGetValue(uuid, out bag_service_use_item_cb rsp))
                {
                    map_use_item.Remove(uuid);
                }
                return rsp;
            }
        }

    }

    public class bag_service_caller {
        public static bag_service_rsp_cb rsp_cb_bag_service_handle = null;
        private ThreadLocal<bag_service_hubproxy> _hubproxy;
        public bag_service_caller()
        {
            if (rsp_cb_bag_service_handle == null)
            {
                rsp_cb_bag_service_handle = new bag_service_rsp_cb();
            }
            _hubproxy = new ThreadLocal<bag_service_hubproxy>();
        }

        public bag_service_hubproxy get_hub(string hub_name) {
            if (_hubproxy.Value == null)
{
                _hubproxy.Value = new bag_service_hubproxy(rsp_cb_bag_service_handle);
            }
            _hubproxy.Value.hub_name_894370de_0386_39f9_9417_050dbc7d5f48 = hub_name;
            return _hubproxy.Value;
        }

    }

    public class bag_service_hubproxy {
        public string hub_name_894370de_0386_39f9_9417_050dbc7d5f48;
        private Int32 uuid_894370de_0386_39f9_9417_050dbc7d5f48 = (Int32)RandomUUID.random();

        private bag_service_rsp_cb rsp_cb_bag_service_handle;

        public bag_service_hubproxy(bag_service_rsp_cb rsp_cb_bag_service_handle_)
        {
            rsp_cb_bag_service_handle = rsp_cb_bag_service_handle_;
        }

        public bag_service_get_bag_cb get_bag(){
            var uuid_c9d12bc7_0c87_5ba3_ae63_54db2f405700 = (UInt64)Interlocked.Increment(ref uuid_894370de_0386_39f9_9417_050dbc7d5f48);

            var _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b = new ArrayList();
            _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b.Add(uuid_c9d12bc7_0c87_5ba3_ae63_54db2f405700);
            Hub.Hub._hubs.call_hub(hub_name_894370de_0386_39f9_9417_050dbc7d5f48, "bag_service_get_bag", _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b);

            var cb_get_bag_obj = new bag_service_get_bag_cb(uuid_c9d12bc7_0c87_5ba3_ae63_54db2f405700, rsp_cb_bag_service_handle);
            lock(rsp_cb_bag_service_handle.map_get_bag)
            {
                rsp_cb_bag_service_handle.map_get_bag.Add(uuid_c9d12bc7_0c87_5ba3_ae63_54db2f405700, cb_get_bag_obj);
            }
            return cb_get_bag_obj;
        }

        public bag_service_use_item_cb use_item(Int64 uuid){
            var uuid_569d45db_8555_50b6_81fd_aeaa55c518ce = (UInt64)Interlocked.Increment(ref uuid_894370de_0386_39f9_9417_050dbc7d5f48);

            var _argv_8e931b2d_2ecb_30da_928a_750ecb587c14 = new ArrayList();
            _argv_8e931b2d_2ecb_30da_928a_750ecb587c14.Add(uuid_569d45db_8555_50b6_81fd_aeaa55c518ce);
            _argv_8e931b2d_2ecb_30da_928a_750ecb587c14.Add(uuid);
            Hub.Hub._hubs.call_hub(hub_name_894370de_0386_39f9_9417_050dbc7d5f48, "bag_service_use_item", _argv_8e931b2d_2ecb_30da_928a_750ecb587c14);

            var cb_use_item_obj = new bag_service_use_item_cb(uuid_569d45db_8555_50b6_81fd_aeaa55c518ce, rsp_cb_bag_service_handle);
            lock(rsp_cb_bag_service_handle.map_use_item)
            {
                rsp_cb_bag_service_handle.map_use_item.Add(uuid_569d45db_8555_50b6_81fd_aeaa55c518ce, cb_use_item_obj);
            }
            return cb_use_item_obj;
        }

    }
/*this module code is codegen by abelkhan codegen for c#*/
    public class bag_service_get_bag_rsp : Common.Response {
        private string _hub_name_d3863e11_ea63_38a7_94fc_1673af9ced0b;
        private UInt64 uuid_82fde665_e33a_3f78_a862_086dfa5e8891;
        public bag_service_get_bag_rsp(string hub_name, UInt64 _uuid) 
        {
            _hub_name_d3863e11_ea63_38a7_94fc_1673af9ced0b = hub_name;
            uuid_82fde665_e33a_3f78_a862_086dfa5e8891 = _uuid;
        }

        public void rsp(bag_def _bag_0989ed80_deae_3d1b_b867_21e2d70ab4b3){
            var _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b = new ArrayList();
            _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b.Add(uuid_82fde665_e33a_3f78_a862_086dfa5e8891);
            _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b.Add(bag_def.bag_def_to_protcol(_bag_0989ed80_deae_3d1b_b867_21e2d70ab4b3));
            Hub.Hub._hubs.call_hub(_hub_name_d3863e11_ea63_38a7_94fc_1673af9ced0b, "bag_service_rsp_cb_get_bag_rsp", _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b);
        }

        public void err(){
            var _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b = new ArrayList();
            _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b.Add(uuid_82fde665_e33a_3f78_a862_086dfa5e8891);
            Hub.Hub._hubs.call_hub(_hub_name_d3863e11_ea63_38a7_94fc_1673af9ced0b, "bag_service_rsp_cb_get_bag_err", _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b);
        }

    }

    public class bag_service_use_item_rsp : Common.Response {
        private string _hub_name_8e931b2d_2ecb_30da_928a_750ecb587c14;
        private UInt64 uuid_6a895a27_7c90_3d7f_b9f5_d47cbcbd1584;
        public bag_service_use_item_rsp(string hub_name, UInt64 _uuid) 
        {
            _hub_name_8e931b2d_2ecb_30da_928a_750ecb587c14 = hub_name;
            uuid_6a895a27_7c90_3d7f_b9f5_d47cbcbd1584 = _uuid;
        }

        public void rsp(){
            var _argv_8e931b2d_2ecb_30da_928a_750ecb587c14 = new ArrayList();
            _argv_8e931b2d_2ecb_30da_928a_750ecb587c14.Add(uuid_6a895a27_7c90_3d7f_b9f5_d47cbcbd1584);
            Hub.Hub._hubs.call_hub(_hub_name_8e931b2d_2ecb_30da_928a_750ecb587c14, "bag_service_rsp_cb_use_item_rsp", _argv_8e931b2d_2ecb_30da_928a_750ecb587c14);
        }

        public void err(){
            var _argv_8e931b2d_2ecb_30da_928a_750ecb587c14 = new ArrayList();
            _argv_8e931b2d_2ecb_30da_928a_750ecb587c14.Add(uuid_6a895a27_7c90_3d7f_b9f5_d47cbcbd1584);
            Hub.Hub._hubs.call_hub(_hub_name_8e931b2d_2ecb_30da_928a_750ecb587c14, "bag_service_rsp_cb_use_item_err", _argv_8e931b2d_2ecb_30da_928a_750ecb587c14);
        }

    }

    public class bag_service_module : Common.IModule {
        public bag_service_module() 
        {
            Hub.Hub._modules.add_mothed("bag_service_get_bag", get_bag);
            Hub.Hub._modules.add_mothed("bag_service_use_item", use_item);
        }

        public event Action on_get_bag;
        public void get_bag(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            rsp = new bag_service_get_bag_rsp(Hub.Hub._hubs.current_hubproxy.name, _cb_uuid);
            if (on_get_bag != null){
                on_get_bag();
            }
            rsp = null;
        }

        public event Action<Int64> on_use_item;
        public void use_item(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _uuid = ((MsgPack.MessagePackObject)inArray[1]).AsInt64();
            rsp = new bag_service_use_item_rsp(Hub.Hub._hubs.current_hubproxy.name, _cb_uuid);
            if (on_use_item != null){
                on_use_item(_uuid);
            }
            rsp = null;
        }

    }

}
