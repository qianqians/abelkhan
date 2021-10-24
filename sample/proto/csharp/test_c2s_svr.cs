using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MsgPack.Serialization;

namespace abelkhan
{
/*this enum code is codegen by abelkhan codegen for c#*/

/*this struct code is codegen by abelkhan codegen for c#*/
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

    public class test_c2s_get_websocket_svr_host_rsp : common.Response {
        private string _client_uuid_ea3a8af7_4bd0_3344_a846_4962c0e7c00f;
        private UInt64 uuid_e3b24ad3_3493_397d_93f9_1e0d27ae8bc1;
        public test_c2s_get_websocket_svr_host_rsp(string client_uuid, UInt64 _uuid)
        {
            _client_uuid_ea3a8af7_4bd0_3344_a846_4962c0e7c00f = client_uuid;
            uuid_e3b24ad3_3493_397d_93f9_1e0d27ae8bc1 = _uuid;
        }

        public void rsp(string ip_e5e31df7_6dc7_37aa_ba52_7658a4380d5c, UInt16 port_ffe0aba4_b17d_3e13_b873_0b47f48a19f1){
            var _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f = new ArrayList();
            _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f.Add(uuid_e3b24ad3_3493_397d_93f9_1e0d27ae8bc1);
            _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f.Add(ip_e5e31df7_6dc7_37aa_ba52_7658a4380d5c);
            _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f.Add(port_ffe0aba4_b17d_3e13_b873_0b47f48a19f1);
            hub.hub._gates.call_client(_client_uuid_ea3a8af7_4bd0_3344_a846_4962c0e7c00f, "test_c2s_rsp_cb", "get_websocket_svr_host_rsp", _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f);
        }

        public void err(){
            var _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f = new ArrayList();
            _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f.Add(uuid_e3b24ad3_3493_397d_93f9_1e0d27ae8bc1);
            hub.hub._gates.call_client(_client_uuid_ea3a8af7_4bd0_3344_a846_4962c0e7c00f, "test_c2s_rsp_cb", "get_websocket_svr_host_err", _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f);
        }

    }

    public class test_c2s_module : common.imodule {
        public test_c2s_module()
        {
            hub.hub._modules.add_module("test_c2s", this);

            reg_cb("login", login);
            reg_cb("get_svr_host", get_svr_host);
            reg_cb("get_websocket_svr_host", get_websocket_svr_host);
        }

        public event Action on_login;
        public void login(IList<MsgPack.MessagePackObject> inArray){
            if (on_login != null){
                on_login();
            }
        }

        public event Action on_get_svr_host;
        public void get_svr_host(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            rsp = new test_c2s_get_svr_host_rsp(hub.hub._gates.current_client_uuid, _cb_uuid);
            if (on_get_svr_host != null){
                on_get_svr_host();
            }
            rsp = null;
        }

        public event Action on_get_websocket_svr_host;
        public void get_websocket_svr_host(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            rsp = new test_c2s_get_websocket_svr_host_rsp(hub.hub._gates.current_client_uuid, _cb_uuid);
            if (on_get_websocket_svr_host != null){
                on_get_websocket_svr_host();
            }
            rsp = null;
        }

    }

}
