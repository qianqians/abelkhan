using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MsgPack.Serialization;

namespace Abelkhan
{
/*this enum code is codegen by abelkhan codegen for c#*/

/*this struct code is codegen by abelkhan codegen for c#*/
/*this module code is codegen by abelkhan codegen for c#*/
    public class rank_cli_service_get_rank_guid_rsp : Common.Response {
        private string _client_uuid_90f752ce_ee17_38de_b679_4a35e21e4129;
        private UInt64 uuid_3eca1bfd_46e9_32ae_ac6c_bba1510a64e4;
        public rank_cli_service_get_rank_guid_rsp(string client_uuid, UInt64 _uuid)
        {
            _client_uuid_90f752ce_ee17_38de_b679_4a35e21e4129 = client_uuid;
            uuid_3eca1bfd_46e9_32ae_ac6c_bba1510a64e4 = _uuid;
        }

        public void rsp(Int32 rank_4c7a18ad_077b_37ed_8d4e_92440bd00c2b){
            var _argv_90f752ce_ee17_38de_b679_4a35e21e4129 = new ArrayList();
            _argv_90f752ce_ee17_38de_b679_4a35e21e4129.Add(uuid_3eca1bfd_46e9_32ae_ac6c_bba1510a64e4);
            _argv_90f752ce_ee17_38de_b679_4a35e21e4129.Add(rank_4c7a18ad_077b_37ed_8d4e_92440bd00c2b);
            Hub.Hub._gates.call_client(_client_uuid_90f752ce_ee17_38de_b679_4a35e21e4129, "rank_cli_service_rsp_cb_get_rank_guid_rsp", _argv_90f752ce_ee17_38de_b679_4a35e21e4129);
        }

        public void err(){
            var _argv_90f752ce_ee17_38de_b679_4a35e21e4129 = new ArrayList();
            _argv_90f752ce_ee17_38de_b679_4a35e21e4129.Add(uuid_3eca1bfd_46e9_32ae_ac6c_bba1510a64e4);
            Hub.Hub._gates.call_client(_client_uuid_90f752ce_ee17_38de_b679_4a35e21e4129, "rank_cli_service_rsp_cb_get_rank_guid_err", _argv_90f752ce_ee17_38de_b679_4a35e21e4129);
        }

    }

    public class rank_cli_service_get_rank_range_rsp : Common.Response {
        private string _client_uuid_17367d36_e3ba_3b3f_87b8_4f982846a886;
        private UInt64 uuid_62880434_51e4_376d_ae03_494ca99f5b65;
        public rank_cli_service_get_rank_range_rsp(string client_uuid, UInt64 _uuid)
        {
            _client_uuid_17367d36_e3ba_3b3f_87b8_4f982846a886 = client_uuid;
            uuid_62880434_51e4_376d_ae03_494ca99f5b65 = _uuid;
        }

        public void rsp(List<rank_item> rank_list_97e71bcb_581e_338e_b072_0ef986ce8722){
            var _argv_17367d36_e3ba_3b3f_87b8_4f982846a886 = new ArrayList();
            _argv_17367d36_e3ba_3b3f_87b8_4f982846a886.Add(uuid_62880434_51e4_376d_ae03_494ca99f5b65);
            var _array_97e71bcb_581e_338e_b072_0ef986ce8722 = new ArrayList();
            foreach(var v_6fa951c6_19a5_56e0_806f_8685b83ba249 in rank_list_97e71bcb_581e_338e_b072_0ef986ce8722){
                _array_97e71bcb_581e_338e_b072_0ef986ce8722.Add(rank_item.rank_item_to_protcol(v_6fa951c6_19a5_56e0_806f_8685b83ba249));
            }
            _argv_17367d36_e3ba_3b3f_87b8_4f982846a886.Add(_array_97e71bcb_581e_338e_b072_0ef986ce8722);
            Hub.Hub._gates.call_client(_client_uuid_17367d36_e3ba_3b3f_87b8_4f982846a886, "rank_cli_service_rsp_cb_get_rank_range_rsp", _argv_17367d36_e3ba_3b3f_87b8_4f982846a886);
        }

        public void err(){
            var _argv_17367d36_e3ba_3b3f_87b8_4f982846a886 = new ArrayList();
            _argv_17367d36_e3ba_3b3f_87b8_4f982846a886.Add(uuid_62880434_51e4_376d_ae03_494ca99f5b65);
            Hub.Hub._gates.call_client(_client_uuid_17367d36_e3ba_3b3f_87b8_4f982846a886, "rank_cli_service_rsp_cb_get_rank_range_err", _argv_17367d36_e3ba_3b3f_87b8_4f982846a886);
        }

    }

    public class rank_cli_service_module : Common.IModule {
        public rank_cli_service_module()
        {
            Hub.Hub._modules.add_mothed("rank_cli_service_get_rank_guid", get_rank_guid);
            Hub.Hub._modules.add_mothed("rank_cli_service_get_rank_range", get_rank_range);
        }

        public event Action<string, Int64> on_get_rank_guid;
        public void get_rank_guid(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _rank_name = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _guid = ((MsgPack.MessagePackObject)inArray[2]).AsInt64();
            rsp = new rank_cli_service_get_rank_guid_rsp(Hub.Hub._gates.current_client_uuid, _cb_uuid);
            if (on_get_rank_guid != null){
                on_get_rank_guid(_rank_name, _guid);
            }
            rsp = null;
        }

        public event Action<string, Int32, Int32> on_get_rank_range;
        public void get_rank_range(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _rank_name = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _start = ((MsgPack.MessagePackObject)inArray[2]).AsInt32();
            var _end = ((MsgPack.MessagePackObject)inArray[3]).AsInt32();
            rsp = new rank_cli_service_get_rank_range_rsp(Hub.Hub._gates.current_client_uuid, _cb_uuid);
            if (on_get_rank_range != null){
                on_get_rank_range(_rank_name, _start, _end);
            }
            rsp = null;
        }

    }

}
