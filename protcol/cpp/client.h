#ifndef _h_client_47f7b5bf_5eb0_3794_bd8c_e38da400d730_
#define _h_client_47f7b5bf_5eb0_3794_bd8c_e38da400d730_

#include <abelkhan.h>
#include <signals.h>

namespace abelkhan
{
/*this enum code is codegen by abelkhan codegen for cpp*/

/*this struct code is codegen by abelkhan codegen for cpp*/
/*this caller code is codegen by abelkhan codegen for cpp*/
/*this cb code is codegen by abelkhan for cpp*/
    class gate_call_client_rsp_cb : public Imodule, public std::enable_shared_from_this<gate_call_client_rsp_cb>{
    public:
        gate_call_client_rsp_cb() : Imodule("gate_call_client_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

        }

    };

    class gate_call_client_caller : Icaller {
    private:
        static std::shared_ptr<gate_call_client_rsp_cb> rsp_cb_gate_call_client_handle;

    private:
        std::atomic<uint64_t> uuid_b84dd831_2e79_3280_a337_a69dd489e75f;

    public:
        gate_call_client_caller(std::shared_ptr<Ichannel> _ch, std::shared_ptr<modulemng> modules) : Icaller("gate_call_client", _ch)
        {
            if (rsp_cb_gate_call_client_handle == nullptr){
                rsp_cb_gate_call_client_handle = std::make_shared<gate_call_client_rsp_cb>();
                rsp_cb_gate_call_client_handle->Init(modules);
            }
            uuid_b84dd831_2e79_3280_a337_a69dd489e75f.store(random());
        }

        void ntf_cuuid(std::string cuuid){
            msgpack11::MsgPack::array _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1;
            _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1.push_back(cuuid);
            call_module_method("ntf_cuuid", _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1);
        }

        void call_client(std::vector<uint8_t> rpc_argv){
            msgpack11::MsgPack::array _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab;
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push_back(rpc_argv);
            call_module_method("call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
        }

    };
/*this module code is codegen by abelkhan codegen for cpp*/
    class gate_call_client_module : Imodule, public std::enable_shared_from_this<gate_call_client_module>{
    public:
        gate_call_client_module() : Imodule("gate_call_client")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("ntf_cuuid", std::bind(&gate_call_client_module::ntf_cuuid, this, std::placeholders::_1));
            reg_method("call_client", std::bind(&gate_call_client_module::call_client, this, std::placeholders::_1));
        }

        concurrent::signals<void(std::string cuuid)> sig_ntf_cuuid;
        void ntf_cuuid(const msgpack11::MsgPack::array& inArray){
            auto _cuuid = inArray[0].string_value();
            sig_ntf_cuuid.emit(_cuuid);
        }

        concurrent::signals<void(std::vector<uint8_t> rpc_argv)> sig_call_client;
        void call_client(const msgpack11::MsgPack::array& inArray){
            auto _rpc_argv = inArray[0].binary_items();
            sig_call_client.emit(_rpc_argv);
        }

    };

}

#endif //_h_client_47f7b5bf_5eb0_3794_bd8c_e38da400d730_
