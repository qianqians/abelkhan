#ifndef _h_test_c2s_c233fb06_7c62_3839_a7d5_edade25b16c5_
#define _h_test_c2s_c233fb06_7c62_3839_a7d5_edade25b16c5_

#include <hub_service.h>
#include <signals.h>


namespace abelkhan
{
/*this enum code is codegen by abelkhan codegen for cpp*/

/*this struct code is codegen by abelkhan codegen for cpp*/
/*this module code is codegen by abelkhan codegen for cpp*/
    class test_c2s_get_svr_host_rsp : public common::Response {
    private:
        std::shared_ptr<hub::hub_service> _hub_handle;
        std::string _client_cuuid_abbb842f_52d0_34e7_9d8d_642d072db165;
        uint64_t uuid_f8b1c4e0_ccd3_3a81_80ee_02001b676fd3;

    public:
        test_c2s_get_svr_host_rsp(std::shared_ptr<hub::hub_service> _hub, std::string client_cuuid, uint64_t _uuid)
        {
            _hub_handle = _hub;
            _client_cuuid_abbb842f_52d0_34e7_9d8d_642d072db165 = client_cuuid;
            uuid_f8b1c4e0_ccd3_3a81_80ee_02001b676fd3 = _uuid;
        }

        void rsp(std::string ip, uint16_t port){
            msgpack11::MsgPack::array _argv_abbb842f_52d0_34e7_9d8d_642d072db165;
            _argv_abbb842f_52d0_34e7_9d8d_642d072db165.push_back(uuid_f8b1c4e0_ccd3_3a81_80ee_02001b676fd3);
            _argv_abbb842f_52d0_34e7_9d8d_642d072db165.push_back(ip);
            _argv_abbb842f_52d0_34e7_9d8d_642d072db165.push_back(port);
            _hub_handle->_gatemng->call_client(_client_cuuid_abbb842f_52d0_34e7_9d8d_642d072db165, "test_c2s_rsp_cb", "get_svr_host_rsp", _argv_abbb842f_52d0_34e7_9d8d_642d072db165);
        }

        void err(){
            msgpack11::MsgPack::array _argv_abbb842f_52d0_34e7_9d8d_642d072db165;
            _argv_abbb842f_52d0_34e7_9d8d_642d072db165.push_back(uuid_f8b1c4e0_ccd3_3a81_80ee_02001b676fd3);
            _hub_handle->_gatemng->call_client(_client_cuuid_abbb842f_52d0_34e7_9d8d_642d072db165, "test_c2s_rsp_cb", "get_svr_host_err", _argv_abbb842f_52d0_34e7_9d8d_642d072db165);
        }

    };

    class test_c2s_module : public common::imodule, public std::enable_shared_from_this<test_c2s_module>{
    private:
        std::shared_ptr<hub::hub_service> hub_handle;

    public:
        test_c2s_module()
        {
        }

        void Init(std::shared_ptr<hub::hub_service> _hub_service){
            hub_handle = _hub_service;
            _hub_service->modules.add_module("test_c2s", std::static_pointer_cast<common::imodule>(shared_from_this()));

            reg_cb("login", std::bind(&test_c2s_module::login, this, std::placeholders::_1));
            reg_cb("get_svr_host", std::bind(&test_c2s_module::get_svr_host, this, std::placeholders::_1));
        }

        concurrent::signals<void()> sig_login;
        void login(const msgpack11::MsgPack::array& inArray){
            sig_login.emit();
        }

        concurrent::signals<void()> sig_get_svr_host;
        void get_svr_host(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            rsp = std::make_shared<test_c2s_get_svr_host_rsp>(hub_handle, hub_handle->_gatemng->current_client_cuuid, _cb_uuid);
            sig_get_svr_host.emit();
            rsp = nullptr;
        }

    };

}

#endif //_h_test_c2s_c233fb06_7c62_3839_a7d5_edade25b16c5_
