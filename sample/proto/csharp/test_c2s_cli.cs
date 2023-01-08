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
    public class test_c2s_get_svr_host_cb
    {
        private UInt64 cb_uuid;
        private test_c2s_rsp_cb module_rsp_cb;

        public test_c2s_get_svr_host_cb(UInt64 _cb_uuid, test_c2s_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<string, UInt16> on_get_svr_host_cb;
        public event Action on_get_svr_host_err;
        public event Action on_get_svr_host_timeout;

        public test_c2s_get_svr_host_cb callBack(Action<string, UInt16> cb, Action err)
        {
            on_get_svr_host_cb += cb;
            on_get_svr_host_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.get_svr_host_timeout(cb_uuid);
            });
            on_get_svr_host_timeout += timeout_cb;
        }

        public void call_cb(string ip, UInt16 port)
        {
            if (on_get_svr_host_cb != null)
            {
                on_get_svr_host_cb(ip, port);
            }
        }

        public void call_err()
        {
            if (on_get_svr_host_err != null)
            {
                on_get_svr_host_err();
            }
        }

        public void call_timeout()
        {
            if (on_get_svr_host_timeout != null)
            {
                on_get_svr_host_timeout();
            }
        }

    }

    public class test_c2s_get_websocket_svr_host_cb
    {
        private UInt64 cb_uuid;
        private test_c2s_rsp_cb module_rsp_cb;

        public test_c2s_get_websocket_svr_host_cb(UInt64 _cb_uuid, test_c2s_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action<string, UInt16> on_get_websocket_svr_host_cb;
        public event Action on_get_websocket_svr_host_err;
        public event Action on_get_websocket_svr_host_timeout;

        public test_c2s_get_websocket_svr_host_cb callBack(Action<string, UInt16> cb, Action err)
        {
            on_get_websocket_svr_host_cb += cb;
            on_get_websocket_svr_host_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.get_websocket_svr_host_timeout(cb_uuid);
            });
            on_get_websocket_svr_host_timeout += timeout_cb;
        }

        public void call_cb(string ip, UInt16 port)
        {
            if (on_get_websocket_svr_host_cb != null)
            {
                on_get_websocket_svr_host_cb(ip, port);
            }
        }

        public void call_err()
        {
            if (on_get_websocket_svr_host_err != null)
            {
                on_get_websocket_svr_host_err();
            }
        }

        public void call_timeout()
        {
            if (on_get_websocket_svr_host_timeout != null)
            {
                on_get_websocket_svr_host_timeout();
            }
        }

    }

/*this cb code is codegen by abelkhan for c#*/
    public class test_c2s_rsp_cb : common.imodule {
        public Dictionary<UInt64, test_c2s_get_svr_host_cb> map_get_svr_host;
        public Dictionary<UInt64, test_c2s_get_websocket_svr_host_cb> map_get_websocket_svr_host;
        public test_c2s_rsp_cb(common.modulemanager modules)
        {
            map_get_svr_host = new Dictionary<UInt64, test_c2s_get_svr_host_cb>();
            modules.add_mothed("test_c2s_rsp_cb_get_svr_host_rsp", get_svr_host_rsp);
            modules.add_mothed("test_c2s_rsp_cb_get_svr_host_err", get_svr_host_err);
            map_get_websocket_svr_host = new Dictionary<UInt64, test_c2s_get_websocket_svr_host_cb>();
            modules.add_mothed("test_c2s_rsp_cb_get_websocket_svr_host_rsp", get_websocket_svr_host_rsp);
            modules.add_mothed("test_c2s_rsp_cb_get_websocket_svr_host_err", get_websocket_svr_host_err);
        }

        public void get_svr_host_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _ip = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _port = ((MsgPack.MessagePackObject)inArray[2]).AsUInt16();
            var rsp = try_get_and_del_get_svr_host_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_ip, _port);
            }
        }

        public void get_svr_host_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_get_svr_host_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void get_svr_host_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_get_svr_host_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private test_c2s_get_svr_host_cb try_get_and_del_get_svr_host_cb(UInt64 uuid){
            lock(map_get_svr_host)
            {
                if (map_get_svr_host.TryGetValue(uuid, out test_c2s_get_svr_host_cb rsp))
                {
                    map_get_svr_host.Remove(uuid);
                }
                return rsp;
            }
        }

        public void get_websocket_svr_host_rsp(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var _ip = ((MsgPack.MessagePackObject)inArray[1]).AsString();
            var _port = ((MsgPack.MessagePackObject)inArray[2]).AsUInt16();
            var rsp = try_get_and_del_get_websocket_svr_host_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_ip, _port);
            }
        }

        public void get_websocket_svr_host_err(IList<MsgPack.MessagePackObject> inArray){
            var uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            var rsp = try_get_and_del_get_websocket_svr_host_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void get_websocket_svr_host_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_get_websocket_svr_host_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private test_c2s_get_websocket_svr_host_cb try_get_and_del_get_websocket_svr_host_cb(UInt64 uuid){
            lock(map_get_websocket_svr_host)
            {
                if (map_get_websocket_svr_host.TryGetValue(uuid, out test_c2s_get_websocket_svr_host_cb rsp))
                {
                    map_get_websocket_svr_host.Remove(uuid);
                }
                return rsp;
            }
        }

    }

    public class test_c2s_caller {
        public static test_c2s_rsp_cb rsp_cb_test_c2s_handle = null;
        private test_c2s_hubproxy _hubproxy;
        public test_c2s_caller(client.client _client_handle) 
        {
            if (rsp_cb_test_c2s_handle == null)
            {
                rsp_cb_test_c2s_handle = new test_c2s_rsp_cb(_client_handle.modulemanager);
            }

            _hubproxy = new test_c2s_hubproxy(_client_handle, rsp_cb_test_c2s_handle);
        }

        public test_c2s_hubproxy get_hub(string hub_name)
        {
            _hubproxy.hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5 = hub_name;
            return _hubproxy;
        }

    }

    public class test_c2s_hubproxy {
        public string hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5;
        private Int32 uuid_c233fb06_7c62_3839_a7d5_edade25b16c5 = (Int32)RandomUUID.random();

        public client.client _client_handle;
        public test_c2s_rsp_cb rsp_cb_test_c2s_handle;

        public test_c2s_hubproxy(client.client client_handle_, test_c2s_rsp_cb rsp_cb_test_c2s_handle_)
        {
            _client_handle = client_handle_;
            rsp_cb_test_c2s_handle = rsp_cb_test_c2s_handle_;
        }

        public void login(){
            var _argv_d3bb20a7_d0fc_3440_bb9e_b3cc0630e2d1 = new ArrayList();
            _client_handle.call_hub(hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5, "test_c2s_login", _argv_d3bb20a7_d0fc_3440_bb9e_b3cc0630e2d1);
        }

        public test_c2s_get_svr_host_cb get_svr_host(){
            var uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65 = (UInt64)Interlocked.Increment(ref uuid_c233fb06_7c62_3839_a7d5_edade25b16c5);

            var _argv_abbb842f_52d0_34e7_9d8d_642d072db165 = new ArrayList();
            _argv_abbb842f_52d0_34e7_9d8d_642d072db165.Add(uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65);
            _client_handle.call_hub(hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5, "test_c2s_get_svr_host", _argv_abbb842f_52d0_34e7_9d8d_642d072db165);

            var cb_get_svr_host_obj = new test_c2s_get_svr_host_cb(uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65, rsp_cb_test_c2s_handle);
            lock(rsp_cb_test_c2s_handle.map_get_svr_host)
            {                rsp_cb_test_c2s_handle.map_get_svr_host.Add(uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65, cb_get_svr_host_obj);
            }            return cb_get_svr_host_obj;
        }

        public test_c2s_get_websocket_svr_host_cb get_websocket_svr_host(){
            var uuid_4c3154db_d59e_53aa_8765_bd54308cf4a5 = (UInt64)Interlocked.Increment(ref uuid_c233fb06_7c62_3839_a7d5_edade25b16c5);

            var _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f = new ArrayList();
            _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f.Add(uuid_4c3154db_d59e_53aa_8765_bd54308cf4a5);
            _client_handle.call_hub(hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5, "test_c2s_get_websocket_svr_host", _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f);

            var cb_get_websocket_svr_host_obj = new test_c2s_get_websocket_svr_host_cb(uuid_4c3154db_d59e_53aa_8765_bd54308cf4a5, rsp_cb_test_c2s_handle);
            lock(rsp_cb_test_c2s_handle.map_get_websocket_svr_host)
            {                rsp_cb_test_c2s_handle.map_get_websocket_svr_host.Add(uuid_4c3154db_d59e_53aa_8765_bd54308cf4a5, cb_get_websocket_svr_host_obj);
            }            return cb_get_websocket_svr_host_obj;
        }

    }

}
