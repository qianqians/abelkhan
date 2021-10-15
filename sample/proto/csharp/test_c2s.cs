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

/*this cb code is codegen by abelkhan for c#*/
    public class test_c2s_rsp_cb : common.imodule {
        public Dictionary<UInt64, test_c2s_get_svr_host_cb> map_get_svr_host;
        public test_c2s_rsp_cb(common.modulemanager modules)
        {
            modules.add_module("test_c2s_rsp_cb", this);
            map_get_svr_host = new Dictionary<UInt64, test_c2s_get_svr_host_cb>();
            reg_cb("get_svr_host_rsp", get_svr_host_rsp);
            reg_cb("get_svr_host_err", get_svr_host_err);
        }

        public void get_svr_host_rsp(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var _ip = (string)inArray[1];
            var _port = (UInt16)inArray[2];
            var rsp = try_get_and_del_get_svr_host_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb(_ip, _port);
            }
        }

        public void get_svr_host_err(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
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
                var rsp = map_get_svr_host[uuid];
                map_get_svr_host.Remove(uuid);
                return rsp;
            }
        }

    }

    public class test_c2s_caller {
        public static test_c2s_rsp_cb rsp_cb_test_c2s_handle = null;
        private test_c2s_hubproxy _hubproxy;
        private Int64 uuid_c233fb06_7c62_3839_a7d5_edade25b16c5 = (Int64)RandomUUID.random();

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

    public classtest_c2s_hubproxy
{
        public string hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5;
        public client.client _client_handle;
        public test_c2s_rsp_cb rsp_cb_test_c2s_handle;

        public test_c2s_hubproxy(client.client client_handle_, test_c2s_rsp_cb rsp_cb_test_c2s_handle_)
        {
            _client_handle = client_handle_;
            rsp_cb_test_c2s_handle = rsp_cb_test_c2s_handle_;
        }

        public void login(){
            var _argv_d3bb20a7_d0fc_3440_bb9e_b3cc0630e2d1 = new ArrayList();
            _client_handle.call_hub(hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5, "test_c2s", "login", _argv_d3bb20a7_d0fc_3440_bb9e_b3cc0630e2d1);
        }

        public test_c2s_get_svr_host_cb get_svr_host(){
            Interlocked.Increment(ref uuid_c233fb06_7c62_3839_a7d5_edade25b16c5);
            var uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65 = (UInt64)uuid_c233fb06_7c62_3839_a7d5_edade25b16c5;

            var _argv_abbb842f_52d0_34e7_9d8d_642d072db165 = new ArrayList();
            _argv_abbb842f_52d0_34e7_9d8d_642d072db165.Add(uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65);
            _client_handle.call_hub(hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5, "test_c2s", "get_svr_host", _argv_abbb842f_52d0_34e7_9d8d_642d072db165);

            var cb_get_svr_host_obj = new test_c2s_get_svr_host_cb(uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65, rsp_cb_test_c2s_handle);
            rsp_cb_test_c2s_handle.map_get_svr_host.Add(uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65, cb_get_svr_host_obj);
            return cb_get_svr_host_obj;
        }

    }
/*this module code is codegen by abelkhan codegen for c#*/
    public class test_c2s_get_svr_host_rsp : common.Response {
        private string _client_uuid_abbb842f_52d0_34e7_9d8d_642d072db165;
        private UInt64 uuid_f8b1c4e0_ccd3_3a81_80ee_02001b676fd3;
        public test_c2s_get_svr_host_rsp(string client_uuid, UInt64 _uuid)
        {
            _client_uuid_abbb842f_52d0_34e7_9d8d_642d072db165 = client_uuid;
            uuid_f8b1c4e0_ccd3_3a81_80ee_02001b676fd3 = _uuid;
        }

        public void rsp(string ip_e5e31df7_6dc7_37aa_ba52_7658a4380d5c, UInt16 port_ffe0aba4_b17d_3e13_b873_0b47f48a19f1){
            var _argv_abbb842f_52d0_34e7_9d8d_642d072db165 = new ArrayList();
            _argv_abbb842f_52d0_34e7_9d8d_642d072db165.Add(uuid_f8b1c4e0_ccd3_3a81_80ee_02001b676fd3);
            _argv_abbb842f_52d0_34e7_9d8d_642d072db165.Add(ip_e5e31df7_6dc7_37aa_ba52_7658a4380d5c);
            _argv_abbb842f_52d0_34e7_9d8d_642d072db165.Add(port_ffe0aba4_b17d_3e13_b873_0b47f48a19f1);
            hub.hub._gates.call_client(_client_uuid_abbb842f_52d0_34e7_9d8d_642d072db165, "test_c2s_rsp_cb", "get_svr_host_rsp", _argv_abbb842f_52d0_34e7_9d8d_642d072db165);
        }

        public void err(){
            var _argv_abbb842f_52d0_34e7_9d8d_642d072db165 = new ArrayList();
            _argv_abbb842f_52d0_34e7_9d8d_642d072db165.Add(uuid_f8b1c4e0_ccd3_3a81_80ee_02001b676fd3);
            hub.hub._gates.call_client(_client_uuid_abbb842f_52d0_34e7_9d8d_642d072db165, "test_c2s_rsp_cb", "get_svr_host_err", _argv_abbb842f_52d0_34e7_9d8d_642d072db165);
        }

    }

    public class test_c2s_module : common.imodule {
        public test_c2s_module()
        {
            hub.hub._modules.add_module("test_c2s", this);

            reg_cb("login", login);
            reg_cb("get_svr_host", get_svr_host);
        }

        public event Action on_login;
        public void login(ArrayList inArray){
            if (on_login != null){
                on_login();
            }
        }

        public event Action on_get_svr_host;
        public void get_svr_host(ArrayList inArray){
            var _cb_uuid = (UInt64)inArray[0];
            rsp = new test_c2s_get_svr_host_rsp(_hub._gates.current_client_uuid, _cb_uuid);
            if (on_get_svr_host != null){
                on_get_svr_host();
            }
            rsp = null;
        }

    }

}
