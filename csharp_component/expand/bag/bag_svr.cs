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
        if (_struct == null) {
            return null;
        }

            var _protocol = new MsgPack.MessagePackObjectDictionary();
            _protocol.Add("uuid", _struct.uuid);
            _protocol.Add("desc_id", _struct.desc_id);
            _protocol.Add("limit_amount", _struct.limit_amount);
            _protocol.Add("amount", _struct.amount);
            return _protocol;
        }
        public static item_def protcol_to_item_def(MsgPack.MessagePackObjectDictionary _protocol){
        if (_protocol == null) {
            return null;
        }

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
        if (_struct == null) {
            return null;
        }

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
        if (_protocol == null) {
            return null;
        }

            var _struct36247089_d39f_3082_9189_f3b10b7bd024 = new bag_def();
            foreach (var i in _protocol){
                if (((MsgPack.MessagePackObject)i.Key).AsString() == "capacity"){
                    _struct36247089_d39f_3082_9189_f3b10b7bd024.capacity = ((MsgPack.MessagePackObject)i.Value).AsInt32();
                }
                else if (((MsgPack.MessagePackObject)i.Key).AsString() == "items"){
                    _struct36247089_d39f_3082_9189_f3b10b7bd024.items = new List<item_def>();
                    var _protocol_array = ((MsgPack.MessagePackObject)i.Value).AsList();
                    foreach (var v_ in _protocol_array){
                        _struct36247089_d39f_3082_9189_f3b10b7bd024.items.Add(item_def.protcol_to_item_def(((MsgPack.MessagePackObject)v_).AsDictionary()));
                    }
                }
            }
            return _struct36247089_d39f_3082_9189_f3b10b7bd024;
        }
    }

/*this module code is codegen by abelkhan codegen for c#*/
    public class bag_service_get_bag_rsp : Common.Response {
        private string _client_uuid_d3863e11_ea63_38a7_94fc_1673af9ced0b;
        private UInt64 uuid_82fde665_e33a_3f78_a862_086dfa5e8891;
        public bag_service_get_bag_rsp(string client_uuid, UInt64 _uuid)
        {
            _client_uuid_d3863e11_ea63_38a7_94fc_1673af9ced0b = client_uuid;
            uuid_82fde665_e33a_3f78_a862_086dfa5e8891 = _uuid;
        }

        public void rsp(bag_def _bag_0989ed80_deae_3d1b_b867_21e2d70ab4b3){
            var _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b = new ArrayList();
            _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b.Add(uuid_82fde665_e33a_3f78_a862_086dfa5e8891);
            _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b.Add(bag_def.bag_def_to_protcol(_bag_0989ed80_deae_3d1b_b867_21e2d70ab4b3));
            Hub.Hub._gates.call_client(_client_uuid_d3863e11_ea63_38a7_94fc_1673af9ced0b, "bag_service_rsp_cb_get_bag_rsp", _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b);
        }

        public void err(){
            var _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b = new ArrayList();
            _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b.Add(uuid_82fde665_e33a_3f78_a862_086dfa5e8891);
            Hub.Hub._gates.call_client(_client_uuid_d3863e11_ea63_38a7_94fc_1673af9ced0b, "bag_service_rsp_cb_get_bag_err", _argv_d3863e11_ea63_38a7_94fc_1673af9ced0b);
        }

    }

    public class bag_service_use_item_rsp : Common.Response {
        private string _client_uuid_8e931b2d_2ecb_30da_928a_750ecb587c14;
        private UInt64 uuid_6a895a27_7c90_3d7f_b9f5_d47cbcbd1584;
        public bag_service_use_item_rsp(string client_uuid, UInt64 _uuid)
        {
            _client_uuid_8e931b2d_2ecb_30da_928a_750ecb587c14 = client_uuid;
            uuid_6a895a27_7c90_3d7f_b9f5_d47cbcbd1584 = _uuid;
        }

        public void rsp(){
            var _argv_8e931b2d_2ecb_30da_928a_750ecb587c14 = new ArrayList();
            _argv_8e931b2d_2ecb_30da_928a_750ecb587c14.Add(uuid_6a895a27_7c90_3d7f_b9f5_d47cbcbd1584);
            Hub.Hub._gates.call_client(_client_uuid_8e931b2d_2ecb_30da_928a_750ecb587c14, "bag_service_rsp_cb_use_item_rsp", _argv_8e931b2d_2ecb_30da_928a_750ecb587c14);
        }

        public void err(){
            var _argv_8e931b2d_2ecb_30da_928a_750ecb587c14 = new ArrayList();
            _argv_8e931b2d_2ecb_30da_928a_750ecb587c14.Add(uuid_6a895a27_7c90_3d7f_b9f5_d47cbcbd1584);
            Hub.Hub._gates.call_client(_client_uuid_8e931b2d_2ecb_30da_928a_750ecb587c14, "bag_service_rsp_cb_use_item_err", _argv_8e931b2d_2ecb_30da_928a_750ecb587c14);
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
            rsp = new bag_service_get_bag_rsp(Hub.Hub._gates.current_client_uuid, _cb_uuid);
            if (on_get_bag != null){
                on_get_bag();
            }
            rsp = null;
        }

        public event Action<Int64> on_use_item;
        public void use_item(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _uuid = ((MsgPack.MessagePackObject)inArray[1]).AsInt64();
            rsp = new bag_service_use_item_rsp(Hub.Hub._gates.current_client_uuid, _cb_uuid);
            if (on_use_item != null){
                on_use_item(_uuid);
            }
            rsp = null;
        }

    }

}
