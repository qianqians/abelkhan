#ifndef _h_hub_da0ce73d_cc77_369a_81ac_34881bb542e0_
#define _h_hub_da0ce73d_cc77_369a_81ac_34881bb542e0_

#include <abelkhan.h>
#include <signals.h>

namespace abelkhan
{
/*this enum code is codegen by abelkhan codegen for cpp*/

/*this struct code is codegen by abelkhan codegen for cpp*/
/*this caller code is codegen by abelkhan codegen for cpp*/
    class hub_call_hub_rsp_cb;
    class hub_call_hub_reg_hub_cb : public std::enable_shared_from_this<hub_call_hub_reg_hub_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<hub_call_hub_rsp_cb> module_rsp_cb;

    public:
        hub_call_hub_reg_hub_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_hub_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void()> sig_reg_hub_cb;
        concurrent::signals<void()> sig_reg_hub_err;
        concurrent::signals<void()> sig_reg_hub_timeout;

        std::shared_ptr<hub_call_hub_reg_hub_cb> callBack(std::function<void()> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

/*this cb code is codegen by abelkhan for cpp*/
    class hub_call_hub_rsp_cb : public Imodule, public std::enable_shared_from_this<hub_call_hub_rsp_cb>{
    public:
        std::mutex mutex_map_reg_hub;
        std::map<uint64_t, std::shared_ptr<hub_call_hub_reg_hub_cb> > map_reg_hub;
        hub_call_hub_rsp_cb() : Imodule("hub_call_hub_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("reg_hub_rsp", std::bind(&hub_call_hub_rsp_cb::reg_hub_rsp, this, std::placeholders::_1));
            reg_method("reg_hub_err", std::bind(&hub_call_hub_rsp_cb::reg_hub_err, this, std::placeholders::_1));
        }

        void reg_hub_rsp(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_reg_hub_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_reg_hub_cb.emit();
            }
        }

        void reg_hub_err(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_reg_hub_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_reg_hub_err.emit();
            }
        }

        void reg_hub_timeout(uint64_t cb_uuid){
            auto rsp = try_get_and_del_reg_hub_cb(cb_uuid);
            if (rsp != nullptr){
                rsp->sig_reg_hub_timeout.emit();
            }
        }

        std::shared_ptr<hub_call_hub_reg_hub_cb> try_get_and_del_reg_hub_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_reg_hub);
            auto rsp = map_reg_hub[uuid];
            map_reg_hub.erase(uuid);
            return rsp;
        }

    };

    class hub_call_hub_caller : Icaller {
    private:
        static std::shared_ptr<hub_call_hub_rsp_cb> rsp_cb_hub_call_hub_handle;

    private:
        std::atomic<uint64_t> uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f;

    public:
        hub_call_hub_caller(std::shared_ptr<Ichannel> _ch, std::shared_ptr<modulemng> modules) : Icaller("hub_call_hub", _ch)
        {
            if (rsp_cb_hub_call_hub_handle == nullptr){
                rsp_cb_hub_call_hub_handle = std::make_shared<hub_call_hub_rsp_cb>();
                rsp_cb_hub_call_hub_handle->Init(modules);
            }
            uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f.store(random());
        }

        std::shared_ptr<hub_call_hub_reg_hub_cb> reg_hub(std::string hub_name, std::string hub_type){
            auto uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f++;
            msgpack11::MsgPack::array _argv_d47a6c8a_5494_35bb_9bc5_60d20f624f67;
            _argv_d47a6c8a_5494_35bb_9bc5_60d20f624f67.push_back(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106);
            _argv_d47a6c8a_5494_35bb_9bc5_60d20f624f67.push_back(hub_name);
            _argv_d47a6c8a_5494_35bb_9bc5_60d20f624f67.push_back(hub_type);
            call_module_method("reg_hub", _argv_d47a6c8a_5494_35bb_9bc5_60d20f624f67);

            auto cb_reg_hub_obj = std::make_shared<hub_call_hub_reg_hub_cb>(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, rsp_cb_hub_call_hub_handle);
            std::lock_guard<std::mutex> l(rsp_cb_hub_call_hub_handle->mutex_map_reg_hub);
            rsp_cb_hub_call_hub_handle->map_reg_hub.insert(std::make_pair(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, cb_reg_hub_obj));
            return cb_reg_hub_obj;
        }

        void hub_call_hub_mothed(std::string module, std::string func, std::vector<uint8_t> argv){
            msgpack11::MsgPack::array _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e;
            _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e.push_back(module);
            _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e.push_back(func);
            _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e.push_back(argv);
            call_module_method("hub_call_hub_mothed", _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e);
        }

    };
/*this cb code is codegen by abelkhan for cpp*/
    class gate_call_hub_rsp_cb : public Imodule, public std::enable_shared_from_this<gate_call_hub_rsp_cb>{
    public:
        gate_call_hub_rsp_cb() : Imodule("gate_call_hub_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

        }

    };

    class gate_call_hub_caller : Icaller {
    private:
        static std::shared_ptr<gate_call_hub_rsp_cb> rsp_cb_gate_call_hub_handle;

    private:
        std::atomic<uint64_t> uuid_e1565384_c90b_3a02_ae2e_d0d91b2758d1;

    public:
        gate_call_hub_caller(std::shared_ptr<Ichannel> _ch, std::shared_ptr<modulemng> modules) : Icaller("gate_call_hub", _ch)
        {
            if (rsp_cb_gate_call_hub_handle == nullptr){
                rsp_cb_gate_call_hub_handle = std::make_shared<gate_call_hub_rsp_cb>();
                rsp_cb_gate_call_hub_handle->Init(modules);
            }
            uuid_e1565384_c90b_3a02_ae2e_d0d91b2758d1.store(random());
        }

        void client_disconnect(std::string client_uuid){
            msgpack11::MsgPack::array _argv_0b9435aa_3d03_3778_acfb_c7bfbd4f3e60;
            _argv_0b9435aa_3d03_3778_acfb_c7bfbd4f3e60.push_back(client_uuid);
            call_module_method("client_disconnect", _argv_0b9435aa_3d03_3778_acfb_c7bfbd4f3e60);
        }

        void client_call_hub(std::string client_uuid, std::vector<uint8_t> rpc_argv){
            msgpack11::MsgPack::array _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263;
            _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263.push_back(client_uuid);
            _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263.push_back(rpc_argv);
            call_module_method("client_call_hub", _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263);
        }

    };
/*this cb code is codegen by abelkhan for cpp*/
    class client_call_hub_rsp_cb : public Imodule, public std::enable_shared_from_this<client_call_hub_rsp_cb>{
    public:
        client_call_hub_rsp_cb() : Imodule("client_call_hub_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

        }

    };

    class client_call_hub_caller : Icaller {
    private:
        static std::shared_ptr<client_call_hub_rsp_cb> rsp_cb_client_call_hub_handle;

    private:
        std::atomic<uint64_t> uuid_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263;

    public:
        client_call_hub_caller(std::shared_ptr<Ichannel> _ch, std::shared_ptr<modulemng> modules) : Icaller("client_call_hub", _ch)
        {
            if (rsp_cb_client_call_hub_handle == nullptr){
                rsp_cb_client_call_hub_handle = std::make_shared<client_call_hub_rsp_cb>();
                rsp_cb_client_call_hub_handle->Init(modules);
            }
            uuid_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263.store(random());
        }

        void connect_hub(std::string client_uuid){
            msgpack11::MsgPack::array _argv_dc2ee339_bef5_3af9_a492_592ba4f08559;
            _argv_dc2ee339_bef5_3af9_a492_592ba4f08559.push_back(client_uuid);
            call_module_method("connect_hub", _argv_dc2ee339_bef5_3af9_a492_592ba4f08559);
        }

        void call_hub(std::string module, std::string func, std::vector<uint8_t> argv){
            msgpack11::MsgPack::array _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3;
            _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3.push_back(module);
            _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3.push_back(func);
            _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3.push_back(argv);
            call_module_method("call_hub", _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3);
        }

    };
/*this cb code is codegen by abelkhan for cpp*/
    class hub_call_client_rsp_cb : public Imodule, public std::enable_shared_from_this<hub_call_client_rsp_cb>{
    public:
        hub_call_client_rsp_cb() : Imodule("hub_call_client_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

        }

    };

    class hub_call_client_caller : Icaller {
    private:
        static std::shared_ptr<hub_call_client_rsp_cb> rsp_cb_hub_call_client_handle;

    private:
        std::atomic<uint64_t> uuid_44e0e3b5_d5d3_3ab4_87a3_bdf8d8aefeeb;

    public:
        hub_call_client_caller(std::shared_ptr<Ichannel> _ch, std::shared_ptr<modulemng> modules) : Icaller("hub_call_client", _ch)
        {
            if (rsp_cb_hub_call_client_handle == nullptr){
                rsp_cb_hub_call_client_handle = std::make_shared<hub_call_client_rsp_cb>();
                rsp_cb_hub_call_client_handle->Init(modules);
            }
            uuid_44e0e3b5_d5d3_3ab4_87a3_bdf8d8aefeeb.store(random());
        }

        void call_client(std::vector<uint8_t> rpc_argv){
            msgpack11::MsgPack::array _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab;
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push_back(rpc_argv);
            call_module_method("call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
        }

    };
/*this module code is codegen by abelkhan codegen for cpp*/
    class hub_call_hub_reg_hub_rsp : public Response {
    private:
        uint64_t uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67;

    public:
        hub_call_hub_reg_hub_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("hub_call_hub_rsp_cb", _ch)
        {
            uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67 = _uuid;
        }

        void rsp(){
            msgpack11::MsgPack::array _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7;
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.push_back(uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67);
            call_module_method("reg_hub_rsp", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        }

        void err(){
            msgpack11::MsgPack::array _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7;
            _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.push_back(uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67);
            call_module_method("reg_hub_err", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        }

    };

    class hub_call_hub_module : public Imodule, public std::enable_shared_from_this<hub_call_hub_module>{
    public:
        hub_call_hub_module() : Imodule("hub_call_hub")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("reg_hub", std::bind(&hub_call_hub_module::reg_hub, this, std::placeholders::_1));
            reg_method("hub_call_hub_mothed", std::bind(&hub_call_hub_module::hub_call_hub_mothed, this, std::placeholders::_1));
        }

        concurrent::signals<void(std::string, std::string)> sig_reg_hub;
        void reg_hub(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            auto _hub_name = inArray[1].string_value();
            auto _hub_type = inArray[2].string_value();
            rsp = std::make_shared<hub_call_hub_reg_hub_rsp>(current_ch, _cb_uuid);
            sig_reg_hub.emit(_hub_name, _hub_type);
            rsp = nullptr;
        }

        concurrent::signals<void(std::string, std::string, std::vector<uint8_t>)> sig_hub_call_hub_mothed;
        void hub_call_hub_mothed(const msgpack11::MsgPack::array& inArray){
            auto _module = inArray[0].string_value();
            auto _func = inArray[1].string_value();
            auto _argv = inArray[2].binary_items();
            sig_hub_call_hub_mothed.emit(_module, _func, _argv);
        }

    };
    class gate_call_hub_module : public Imodule, public std::enable_shared_from_this<gate_call_hub_module>{
    public:
        gate_call_hub_module() : Imodule("gate_call_hub")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("client_disconnect", std::bind(&gate_call_hub_module::client_disconnect, this, std::placeholders::_1));
            reg_method("client_call_hub", std::bind(&gate_call_hub_module::client_call_hub, this, std::placeholders::_1));
        }

        concurrent::signals<void(std::string)> sig_client_disconnect;
        void client_disconnect(const msgpack11::MsgPack::array& inArray){
            auto _client_uuid = inArray[0].string_value();
            sig_client_disconnect.emit(_client_uuid);
        }

        concurrent::signals<void(std::string, std::vector<uint8_t>)> sig_client_call_hub;
        void client_call_hub(const msgpack11::MsgPack::array& inArray){
            auto _client_uuid = inArray[0].string_value();
            auto _rpc_argv = inArray[1].binary_items();
            sig_client_call_hub.emit(_client_uuid, _rpc_argv);
        }

    };
    class client_call_hub_module : public Imodule, public std::enable_shared_from_this<client_call_hub_module>{
    public:
        client_call_hub_module() : Imodule("client_call_hub")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("connect_hub", std::bind(&client_call_hub_module::connect_hub, this, std::placeholders::_1));
            reg_method("call_hub", std::bind(&client_call_hub_module::call_hub, this, std::placeholders::_1));
        }

        concurrent::signals<void(std::string)> sig_connect_hub;
        void connect_hub(const msgpack11::MsgPack::array& inArray){
            auto _client_uuid = inArray[0].string_value();
            sig_connect_hub.emit(_client_uuid);
        }

        concurrent::signals<void(std::string, std::string, std::vector<uint8_t>)> sig_call_hub;
        void call_hub(const msgpack11::MsgPack::array& inArray){
            auto _module = inArray[0].string_value();
            auto _func = inArray[1].string_value();
            auto _argv = inArray[2].binary_items();
            sig_call_hub.emit(_module, _func, _argv);
        }

    };
    class hub_call_client_module : public Imodule, public std::enable_shared_from_this<hub_call_client_module>{
    public:
        hub_call_client_module() : Imodule("hub_call_client")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("call_client", std::bind(&hub_call_client_module::call_client, this, std::placeholders::_1));
        }

        concurrent::signals<void(std::vector<uint8_t>)> sig_call_client;
        void call_client(const msgpack11::MsgPack::array& inArray){
            auto _rpc_argv = inArray[0].binary_items();
            sig_call_client.emit(_rpc_argv);
        }

    };

}

#endif //_h_hub_da0ce73d_cc77_369a_81ac_34881bb542e0_
