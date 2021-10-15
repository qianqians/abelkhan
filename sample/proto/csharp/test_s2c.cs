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
    public class test_s2c_ping_cb
    {
        private UInt64 cb_uuid;
        private test_s2c_rsp_cb module_rsp_cb;

        public test_s2c_ping_cb(UInt64 _cb_uuid, test_s2c_rsp_cb _module_rsp_cb)
        {
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

        public event Action on_ping_cb;
        public event Action on_ping_err;
        public event Action on_ping_timeout;

        public test_s2c_ping_cb callBack(Action cb, Action err)
        {
            on_ping_cb += cb;
            on_ping_err += err;
            return this;
        }

        public void timeout(UInt64 tick, Action timeout_cb)
        {
            TinyTimer.add_timer(tick, ()=>{
                module_rsp_cb.ping_timeout(cb_uuid);
            });
            on_ping_timeout += timeout_cb;
        }

        public void call_cb()
        {
            if (on_ping_cb != null)
            {
                on_ping_cb();
            }
        }

        public void call_err()
        {
            if (on_ping_err != null)
            {
                on_ping_err();
            }
        }

        public void call_timeout()
        {
            if (on_ping_timeout != null)
            {
                on_ping_timeout();
            }
        }

    }

/*this cb code is codegen by abelkhan for c#*/
    public class test_s2c_rsp_cb : common.imodule {
        public Dictionary<UInt64, test_s2c_ping_cb> map_ping;
        public test_s2c_rsp_cb() 
        {
            hub.hub._modules.add_module("test_s2c_rsp_cb", this);
            map_ping = new Dictionary<UInt64, test_s2c_ping_cb>();
            reg_cb("ping_rsp", ping_rsp);
            reg_cb("ping_err", ping_err);
        }

        public void ping_rsp(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_ping_cb(uuid);
            if (rsp != null)
            {
                rsp.call_cb();
            }
        }

        public void ping_err(ArrayList inArray){
            var uuid = (UInt64)inArray[0];
            var rsp = try_get_and_del_ping_cb(uuid);
            if (rsp != null)
            {
                rsp.call_err();
            }
        }

        public void ping_timeout(UInt64 cb_uuid){
            var rsp = try_get_and_del_ping_cb(cb_uuid);
            if (rsp != null){
                rsp.call_timeout();
            }
        }

        private test_s2c_ping_cb try_get_and_del_ping_cb(UInt64 uuid){
            lock(map_ping)
            {
                var rsp = map_ping[uuid];
                map_ping.Remove(uuid);
                return rsp;
            }
        }

    }

    public class test_s2c_clientproxy
{
    public string client_uuid_a1cf7490_107a_3422_8f39_e02b73ef3c43;
    public test_s2c_rsp_cb rsp_cb_test_s2c_handle;

    public test_s2c_clientproxy(test_s2c_rsp_cb rsp_cb_test_s2c_handle_)
    {
        rsp_cb_test_s2c_handle = rsp_cb_test_s2c_handle_;
    }

        public test_s2c_ping_cb ping(){
            Interlocked.Increment(ref uuid_a1cf7490_107a_3422_8f39_e02b73ef3c43);
            var uuid_80c27ee8_c9bc_583c_bad4_a73880e2ce8f = (UInt64)uuid_a1cf7490_107a_3422_8f39_e02b73ef3c43;

            var _argv_ca6794ee_a403_309d_b40e_f37578d53e8d = new ArrayList();
            _argv_ca6794ee_a403_309d_b40e_f37578d53e8d.Add(uuid_80c27ee8_c9bc_583c_bad4_a73880e2ce8f);
            hub.hub._gates.call_client(client_uuid_a1cf7490_107a_3422_8f39_e02b73ef3c43, "test_s2c", "ping", _argv_ca6794ee_a403_309d_b40e_f37578d53e8d);

            var cb_ping_obj = new test_s2c_ping_cb(uuid_80c27ee8_c9bc_583c_bad4_a73880e2ce8f, rsp_cb_test_s2c_handle);
            rsp_cb_test_s2c_handle.map_ping.Add(uuid_80c27ee8_c9bc_583c_bad4_a73880e2ce8f, cb_ping_obj);
            return cb_ping_obj;
        }

    };

    class test_s2c_multicast
{
    public List<string> client_uuids_a1cf7490_107a_3422_8f39_e02b73ef3c43;
    public test_s2c_rsp_cb rsp_cb_test_s2c_handle;

        test_s2c_multicast(test_s2c_rsp_cb rsp_cb_test_s2c_handle_)
        {
            rsp_cb_test_s2c_handle = rsp_cb_test_s2c_handle_;
        }

    };

    class test_s2c_broadcast
{
    public test_s2c_rsp_cb> rsp_cb_test_s2c_handle;

        test_s2c_multicast(std::shared_ptr<hub::hub_service> hub_service_, std::shared_ptr<test_s2c_rsp_cb> rsp_cb_test_s2c_handle_)
        {
            rsp_cb_test_s2c_handle = rsp_cb_test_s2c_handle_;
        }

    public class test_s2c_caller {
        public static test_s2c_rsp_cb rsp_cb_test_s2c_handle = null;
        private test_s2c_clientproxy _clientproxy;
        private test_s2c_multicast _multicast;
        private test_s2c_broadcast _broadcast;

        private Int64 uuid_a1cf7490_107a_3422_8f39_e02b73ef3c43 = (Int64)RandomUUID.random();

        public test_s2c_caller() 
        {
            if (rsp_cb_test_s2c_handle == null)
            {
                rsp_cb_test_s2c_handle = new test_s2c_rsp_cb(modules);
            }

            _clientproxy = new test_s2c_clientproxy(rsp_cb_test_s2c_handle);
            _multicast = new test_s2c_multicast(rsp_cb_test_s2c_handle);
            _broadcast = new test_s2c_broadcast(rsp_cb_test_s2c_handle);
        }

        public test_s2c_clientproxy get_client(string client_uuid) {
            _clientproxy.client_uuid_a1cf7490_107a_3422_8f39_e02b73ef3c43 = client_uuid;
            return _clientproxy;
        }

        public test_s2c_multicast get_multicast(List<string> client_uuids) {
            _multicast.client_uuids_a1cf7490_107a_3422_8f39_e02b73ef3c43 = client_uuids;
            return _multicast;
        }

        public test_s2c_broadcast get_multicast() {
            return _broadcast;
        }
    }

/*this module code is codegen by abelkhan codegen for c#*/
    public class test_s2c_ping_rsp : common.Response {
        private UInt64 uuid_94d71f95_a670_3916_89a9_44df18fb711b;
        private string hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d;
        private client.client _client_handle;
        public test_s2c_ping_rsp(client.client client_handle_, string current_hub, UInt64 _uuid) : base("test_s2c_rsp_cb", _ch)
        {
            _client_handle = client_handle_;
            hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d = current_hub;
            uuid_94d71f95_a670_3916_89a9_44df18fb711b = _uuid;
        }

        public void rsp(){
            var _argv_ca6794ee_a403_309d_b40e_f37578d53e8d = new ArrayList();
            _argv_ca6794ee_a403_309d_b40e_f37578d53e8d.Add(uuid_94d71f95_a670_3916_89a9_44df18fb711b);
            _client_handle.call_hub(hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d, "test_s2c_rsp_cb", "ping_rsp", _argv_ca6794ee_a403_309d_b40e_f37578d53e8d);
        }

        public void err(){
            var _argv_ca6794ee_a403_309d_b40e_f37578d53e8d = new ArrayList();
            _argv_ca6794ee_a403_309d_b40e_f37578d53e8d.Add(uuid_94d71f95_a670_3916_89a9_44df18fb711b);
            _client_handle.call_hub(hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d, "test_s2c_rsp_cb", "ping_err", _argv_ca6794ee_a403_309d_b40e_f37578d53e8d);
        }

    }

    public class test_s2c_module : common.imodule {
        public client.client _client_handle;
        public test_s2c_module(client.client client_handle_) 
        {
            _client_handle = client_handle_;
            _client_handle.modulemanager.add_module("test_s2c", this);

            reg_cb("ping", ping);
        }

        public event Action on_ping;
        public void ping(ArrayList inArray){
            var _cb_uuid = (UInt64)inArray[0];
            rsp = new test_s2c_ping_rsp(_client_handle, _client_handle.current_hub, _cb_uuid);
            if (on_ping != null){
                on_ping();
            }
            rsp = null;
        }

    }

}
