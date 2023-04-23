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
    public class test_s2c_ping_rsp : common.Response {
        private UInt64 uuid_94d71f95_a670_3916_89a9_44df18fb711b;
        private string hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d;
        private client.Client _client_handle;
        public test_s2c_ping_rsp(client.Client client_handle_, string current_hub, UInt64 _uuid) 
        {
            _client_handle = client_handle_;
            hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d = current_hub;
            uuid_94d71f95_a670_3916_89a9_44df18fb711b = _uuid;
        }

        public void rsp(){
            var _argv_ca6794ee_a403_309d_b40e_f37578d53e8d = new ArrayList();
            _argv_ca6794ee_a403_309d_b40e_f37578d53e8d.Add(uuid_94d71f95_a670_3916_89a9_44df18fb711b);
            _client_handle.call_hub(hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d, "test_s2c_rsp_cb_ping_rsp", _argv_ca6794ee_a403_309d_b40e_f37578d53e8d);
        }

        public void err(){
            var _argv_ca6794ee_a403_309d_b40e_f37578d53e8d = new ArrayList();
            _argv_ca6794ee_a403_309d_b40e_f37578d53e8d.Add(uuid_94d71f95_a670_3916_89a9_44df18fb711b);
            _client_handle.call_hub(hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d, "test_s2c_rsp_cb_ping_err", _argv_ca6794ee_a403_309d_b40e_f37578d53e8d);
        }

    }

    public class test_s2c_module : common.IModule {
        public client.Client _client_handle;
        public test_s2c_module(client.Client client_handle_) 
        {
            _client_handle = client_handle_;
            _client_handle.modulemanager.add_mothed("test_s2c_ping", ping);
        }

        public event Action on_ping;
        public void ping(IList<MsgPack.MessagePackObject> inArray){
            var _cb_uuid = ((MsgPack.MessagePackObject)inArray[0]).AsUInt64();
            rsp = new test_s2c_ping_rsp(_client_handle, _client_handle.current_hub, _cb_uuid);
            if (on_ping != null){
                on_ping();
            }
            rsp = null;
        }

    }

}
