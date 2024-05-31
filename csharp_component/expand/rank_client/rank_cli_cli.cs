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
    public class rank_cli_service_get_rank_guid_cb
    {
        private UInt64 cb_uuid;
        private rank_cli_service_rsp_cb module_rsp_cb;

        public rank_cli_service_get_rank_guid_cb(UInt64 _cb_uuid, rank_cli_service_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<rank_item> on_get_rank_guid_cb;
        public event Action on_get_rank_guid_err;
        public event Action on_get_rank_guid_timeout;

        public rank_cli_service_get_rank_guid_cb callBack(Action<rank_item> cb, Action err)
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

        public void call_cb(rank_item rank)
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

    public class rank_cli_service_get_rank_range_cb
    {
        private UInt64 cb_uuid;
        private rank_cli_service_rsp_cb module_rsp_cb;

        public rank_cli_service_get_rank_range_cb(UInt64 _cb_uuid, rank_cli_service_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<List<rank_item>> on_get_rank_range_cb;
        public event Action on_get_rank_range_err;
        public event Action on_get_rank_range_timeout;

        public rank_cli_service_get_rank_range_cb callBack(Action<List<rank_item>> cb, Action err)
        {
            on_get_rank_range_cb += cb;
            on_get_rank_range_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.get_rank_range_timeout(cb_uuid);
            });
            on_get_rank_range_timeout += timeout_cb;
        }

        public void call_cb(List<rank_item> rank_list)
        {
            if (on_get_rank_range_cb != null)
            {
                on_get_rank_range_cb(rank_list);
            }
        }

        public void call_err()
        {
            if (on_get_rank_range_err != null)
            {
                on_get_rank_range_err();
            }
        }

        public void call_timeout()
        {
            if (on_get_rank_range_timeout != null)
            {
                on_get_rank_range_timeout();
            }
        }

    }

/*this cb code is codegen by abelkhan for c#*/
    public class rank_cli_service_rsp_cb : Common.IModule {
        public Dictionary<UInt64, rank_cli_service_get_rank_guid_cb> map_get_rank_guid;
        public Dictionary<UInt64, rank_cli_service_get_rank_range_cb> map_get_rank_range;
        public rank_cli_service_rsp_cb(Common.ModuleManager modules)
        {
            map_get_rank_guid = new Dictionary<UInt64, rank_cli_service_get_rank_guid_cb>();
            modules.add_mothed("rank_cli_service_rsp_cb_get_rank_guid_rsp", get_rank_guid_rsp);
            modules.add_mothed("rank_cli_service_rsp_cb_get_rank_guid_err", get_rank_guid_err);
            map_get_rank_range = new Dictionary<UInt64, rank_cli_service_get_rank_range_cb>();
            modules.add_mothed("rank_cli_service_rsp_cb_get_rank_range_rsp", get_rank_range_rsp);
            modules.add_mothed("rank_cli_service_rsp_cb_get_rank_range_err", get_rank_range_err);
        }

        public void get_rank_guid_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _rank = rank_item.protcol_to_rank_item(((MsgPack.MessagePackObject)inArray[1]).AsDictionary());
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

        private rank_cli_service_get_rank_guid_cb try_get_and_del_get_rank_guid_cb(UInt64 uuid){
            lock(map_get_rank_guid)
            {
                if (map_get_rank_guid.TryGetValue(uuid, out rank_cli_service_get_rank_guid_cb rsp))
                {
                    map_get_rank_guid.Remove(uuid);
                }
                return rsp;
            }
        }

        public void get_rank_range_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _rank_list = new List<rank_item>();
            var _protocol_arrayrank_list = ((MsgPack.MessagePackObject)inArray[1]).AsList();
            foreach (var v_6fa951c6_19a5_56e0_806f_8685b83ba249 in _protocol_arrayrank_list){
                _rank_list.Add(rank_item.protcol_to_rank_item(((MsgPack.MessagePackObject)v_6fa951c6_19a5_56e0_806f_8685b83ba249).AsDictionary()));
            }
            var rsp = try_get_and_del_get_rank_range_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_rank_list);
            }
        }

        public void get_rank_range_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_get_rank_range_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void get_rank_range_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_get_rank_range_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private rank_cli_service_get_rank_range_cb try_get_and_del_get_rank_range_cb(UInt64 uuid){
            lock(map_get_rank_range)
            {
                if (map_get_rank_range.TryGetValue(uuid, out rank_cli_service_get_rank_range_cb rsp))
                {
                    map_get_rank_range.Remove(uuid);
                }
                return rsp;
            }
        }

    }

    public class rank_cli_service_caller {
        public static rank_cli_service_rsp_cb rsp_cb_rank_cli_service_handle = null;
        private ThreadLocal<rank_cli_service_hubproxy> _hubproxy;
        public Client.Client _client_handle;
        public rank_cli_service_caller(Client.Client client_handle_) 
        {
            _client_handle = client_handle_;
            if (rsp_cb_rank_cli_service_handle == null)
            {
                rsp_cb_rank_cli_service_handle = new rank_cli_service_rsp_cb(_client_handle.modulemanager);
            }

            _hubproxy = new ThreadLocal<rank_cli_service_hubproxy>();
        }

        public rank_cli_service_hubproxy get_hub(string hub_name)
        {
            if (_hubproxy.Value == null)
{
                _hubproxy.Value = new rank_cli_service_hubproxy(_client_handle, rsp_cb_rank_cli_service_handle);
            }
            _hubproxy.Value.hub_name_cdcf8098_c212_361e_95df_8b4d8ec6340e = hub_name;
            return _hubproxy.Value;
        }

    }

    public class rank_cli_service_hubproxy {
        public string hub_name_cdcf8098_c212_361e_95df_8b4d8ec6340e;
        private Int32 uuid_cdcf8098_c212_361e_95df_8b4d8ec6340e = (Int32)RandomUUID.random();

        public Client.Client _client_handle;
        public rank_cli_service_rsp_cb rsp_cb_rank_cli_service_handle;

        public rank_cli_service_hubproxy(Client.Client client_handle_, rank_cli_service_rsp_cb rsp_cb_rank_cli_service_handle_)
        {
            _client_handle = client_handle_;
            rsp_cb_rank_cli_service_handle = rsp_cb_rank_cli_service_handle_;
        }

        public rank_cli_service_get_rank_guid_cb get_rank_guid(string rank_name, Int64 guid){
            var uuid_aae4e0b8_0b60_55bf_a8e6_2afd98164325 = (UInt64)Interlocked.Increment(ref uuid_cdcf8098_c212_361e_95df_8b4d8ec6340e);

            var _argv_90f752ce_ee17_38de_b679_4a35e21e4129 = new ArrayList();
            _argv_90f752ce_ee17_38de_b679_4a35e21e4129.Add(uuid_aae4e0b8_0b60_55bf_a8e6_2afd98164325);
            _argv_90f752ce_ee17_38de_b679_4a35e21e4129.Add(rank_name);
            _argv_90f752ce_ee17_38de_b679_4a35e21e4129.Add(guid);
            _client_handle.call_hub(hub_name_cdcf8098_c212_361e_95df_8b4d8ec6340e, "rank_cli_service_get_rank_guid", _argv_90f752ce_ee17_38de_b679_4a35e21e4129);

            var cb_get_rank_guid_obj = new rank_cli_service_get_rank_guid_cb(uuid_aae4e0b8_0b60_55bf_a8e6_2afd98164325, rsp_cb_rank_cli_service_handle);
            lock(rsp_cb_rank_cli_service_handle.map_get_rank_guid)
            {                rsp_cb_rank_cli_service_handle.map_get_rank_guid.Add(uuid_aae4e0b8_0b60_55bf_a8e6_2afd98164325, cb_get_rank_guid_obj);
            }            return cb_get_rank_guid_obj;
        }

        public rank_cli_service_get_rank_range_cb get_rank_range(string rank_name, Int32 start, Int32 end){
            var uuid_502cc321_083d_54f3_be2e_94e03d09d51d = (UInt64)Interlocked.Increment(ref uuid_cdcf8098_c212_361e_95df_8b4d8ec6340e);

            var _argv_17367d36_e3ba_3b3f_87b8_4f982846a886 = new ArrayList();
            _argv_17367d36_e3ba_3b3f_87b8_4f982846a886.Add(uuid_502cc321_083d_54f3_be2e_94e03d09d51d);
            _argv_17367d36_e3ba_3b3f_87b8_4f982846a886.Add(rank_name);
            _argv_17367d36_e3ba_3b3f_87b8_4f982846a886.Add(start);
            _argv_17367d36_e3ba_3b3f_87b8_4f982846a886.Add(end);
            _client_handle.call_hub(hub_name_cdcf8098_c212_361e_95df_8b4d8ec6340e, "rank_cli_service_get_rank_range", _argv_17367d36_e3ba_3b3f_87b8_4f982846a886);

            var cb_get_rank_range_obj = new rank_cli_service_get_rank_range_cb(uuid_502cc321_083d_54f3_be2e_94e03d09d51d, rsp_cb_rank_cli_service_handle);
            lock(rsp_cb_rank_cli_service_handle.map_get_rank_range)
            {                rsp_cb_rank_cli_service_handle.map_get_rank_range.Add(uuid_502cc321_083d_54f3_be2e_94e03d09d51d, cb_get_rank_range_obj);
            }            return cb_get_rank_range_obj;
        }

    }

}
