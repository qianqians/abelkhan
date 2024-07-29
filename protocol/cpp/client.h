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
        }

    };

    class gate_call_client_caller : public Icaller {
    private:
        static std::shared_ptr<gate_call_client_rsp_cb> rsp_cb_gate_call_client_handle;

    private:
        std::atomic<uint32_t> uuid_b84dd831_2e79_3280_a337_a69dd489e75f;

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
            call_module_method("gate_call_client_ntf_cuuid", _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1);
        }

        void kick_off_reason(std::string reason){
            msgpack11::MsgPack::array _argv_ff383c66_a796_3167_804e_2bbebc0bcb27;
            _argv_ff383c66_a796_3167_804e_2bbebc0bcb27.push_back(reason);
            call_module_method("gate_call_client_kick_off_reason", _argv_ff383c66_a796_3167_804e_2bbebc0bcb27);
        }

        void call_client(std::string hub_name, std::vector<uint8_t> rpc_argv){
            msgpack11::MsgPack::array _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab;
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push_back(hub_name);
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push_back(rpc_argv);
            call_module_method("gate_call_client_call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
        }

        void migrate_client_start(std::string src_hub, std::string target_hub){
            msgpack11::MsgPack::array _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee;
            _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee.push_back(src_hub);
            _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee.push_back(target_hub);
            call_module_method("gate_call_client_migrate_client_start", _argv_c9d99b35_c1ee_347e_8597_4736a13ac8ee);
        }

        void migrate_client_done(std::string src_hub, std::string target_hub){
            msgpack11::MsgPack::array _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c;
            _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c.push_back(src_hub);
            _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c.push_back(target_hub);
            call_module_method("gate_call_client_migrate_client_done", _argv_7e93ee66_7ffc_3958_b9d8_f5ed2e9be23c);
        }

        void hub_loss(std::string hub_name){
            msgpack11::MsgPack::array _argv_90f24099_13d8_3e09_b6fa_6d93a3ae6099;
            _argv_90f24099_13d8_3e09_b6fa_6d93a3ae6099.push_back(hub_name);
            call_module_method("gate_call_client_hub_loss", _argv_90f24099_13d8_3e09_b6fa_6d93a3ae6099);
        }

    };
/*this module code is codegen by abelkhan codegen for cpp*/
    class gate_call_client_module : public Imodule, public std::enable_shared_from_this<gate_call_client_module>{
    public:
        gate_call_client_module() : Imodule("gate_call_client")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_method("gate_call_client_ntf_cuuid", std::make_tuple(shared_from_this(), std::bind(&gate_call_client_module::ntf_cuuid, this, std::placeholders::_1)));
            _modules->reg_method("gate_call_client_kick_off_reason", std::make_tuple(shared_from_this(), std::bind(&gate_call_client_module::kick_off_reason, this, std::placeholders::_1)));
            _modules->reg_method("gate_call_client_call_client", std::make_tuple(shared_from_this(), std::bind(&gate_call_client_module::call_client, this, std::placeholders::_1)));
            _modules->reg_method("gate_call_client_migrate_client_start", std::make_tuple(shared_from_this(), std::bind(&gate_call_client_module::migrate_client_start, this, std::placeholders::_1)));
            _modules->reg_method("gate_call_client_migrate_client_done", std::make_tuple(shared_from_this(), std::bind(&gate_call_client_module::migrate_client_done, this, std::placeholders::_1)));
            _modules->reg_method("gate_call_client_hub_loss", std::make_tuple(shared_from_this(), std::bind(&gate_call_client_module::hub_loss, this, std::placeholders::_1)));
        }

        concurrent::signals<void(std::string)> sig_ntf_cuuid;
        void ntf_cuuid(const msgpack11::MsgPack::array& inArray){
            auto _cuuid = inArray[0].string_value();
            sig_ntf_cuuid.emit(_cuuid);
        }

        concurrent::signals<void(std::string)> sig_kick_off_reason;
        void kick_off_reason(const msgpack11::MsgPack::array& inArray){
            auto _reason = inArray[0].string_value();
            sig_kick_off_reason.emit(_reason);
        }

        concurrent::signals<void(std::string, std::vector<uint8_t>)> sig_call_client;
        void call_client(const msgpack11::MsgPack::array& inArray){
            auto _hub_name = inArray[0].string_value();
            auto _rpc_argv = inArray[1].binary_items();
            sig_call_client.emit(_hub_name, _rpc_argv);
        }

        concurrent::signals<void(std::string, std::string)> sig_migrate_client_start;
        void migrate_client_start(const msgpack11::MsgPack::array& inArray){
            auto _src_hub = inArray[0].string_value();
            auto _target_hub = inArray[1].string_value();
            sig_migrate_client_start.emit(_src_hub, _target_hub);
        }

        concurrent::signals<void(std::string, std::string)> sig_migrate_client_done;
        void migrate_client_done(const msgpack11::MsgPack::array& inArray){
            auto _src_hub = inArray[0].string_value();
            auto _target_hub = inArray[1].string_value();
            sig_migrate_client_done.emit(_src_hub, _target_hub);
        }

        concurrent::signals<void(std::string)> sig_hub_loss;
        void hub_loss(const msgpack11::MsgPack::array& inArray){
            auto _hub_name = inArray[0].string_value();
            sig_hub_loss.emit(_hub_name);
        }

    };

}

#endif //_h_client_47f7b5bf_5eb0_3794_bd8c_e38da400d730_
