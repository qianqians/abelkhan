#ifndef _h_client_47f7b5bf_5eb0_3794_bd8c_e38da400d730_
#define _h_client_47f7b5bf_5eb0_3794_bd8c_e38da400d730_

#include <abelkhan.h>
#include <signals.h>

namespace abelkhan
{
/*this enum code is codegen by abelkhan codegen for cpp*/

/*this struct code is codegen by abelkhan codegen for cpp*/
/*this caller code is codegen by abelkhan codegen for cpp*/
    class gate_call_client_heartbeats_cb : public std::enable_shared_from_this<gate_call_client_heartbeats_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<gate_call_client_rsp_cb> module_rsp_cb;

    public:
        gate_call_client_heartbeats_cb(uint64_t _cb_uuid, std::shared_ptr<gate_call_client_rsp_cb> _module_rsp_cb){
            cb_uuid = _cb_uuid;
            module_rsp_cb = _module_rsp_cb;
        }

    public:
        concurrent::signals<void()> sig_heartbeats_cb;
        concurrent::signals<void()> sig_heartbeats_err;
        concurrent::signals<void()> sig_heartbeats_timeout;

        std::shared_ptr<gate_call_client_heartbeats_cb> callBack(std::function<void()> cb, std::function<void()> err)
        {
            sig_heartbeats_cb.connect(cb);
            sig_heartbeats_err.connect(err);
            return shared_from_this();
        }

        void timeout(uint64_t tick, std::function<void()> timeout_cb)
        {
            TinyTimer::add_timer(tick, [this](){
                module_rsp_cb->heartbeats_timeout(cb_uuid);
            });
            sig_heartbeats_timeout.connect(timeout_cb);
        }

    };

/*this cb code is codegen by abelkhan for cpp*/
    class gate_call_client_rsp_cb : public Imodule, public std::enable_shared_from_this<gate_call_client_rsp_cb>{
    public:
        std::mutex mutex_map_heartbeats;
        std::map<uint64_t, std::shared_ptr<gate_call_client_heartbeats_cb> > map_heartbeats;
        gate_call_client_rsp_cb() : Imodule("gate_call_client_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("heartbeats_rsp", std::bind(&gate_call_client_rsp_cb::heartbeats_rsp, this, std::placeholders::_1));
            reg_method("heartbeats_err", std::bind(&gate_call_client_rsp_cb::heartbeats_err, this, std::placeholders::_1));
        }

        void heartbeats_rsp(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_heartbeats_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_heartbeats_cb.emit();
            }
        }

        void heartbeats_err(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_heartbeats_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_heartbeats_err.emit();
            }
        }

        void heartbeats_timeout(uint64_t cb_uuid){
            auto rsp = try_get_and_del_heartbeats_cb(cb_uuid);
            if (rsp != nullptr){
                rsp->sig_heartbeats_timeout.emit();
            }
        }

        std::shared_ptr<gate_call_client_heartbeats_cb> try_get_and_del_heartbeats_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_heartbeats);
            auto rsp = map_heartbeats[uuid];
            map_heartbeats.erase(uuid);
            return rsp;
        }

    };

    class gate_call_client_caller : Icaller {
    private:
        static std::shared_ptr<gate_call_client_rsp_cb> rsp_cb_gate_call_client_handle;

    private:
        std::atomic<uint64_t> uuid;

    public:
        gate_call_client_caller(std::shared_ptr<Ichannel> _ch, std::shared_ptr<modulemng> modules) : Icaller("gate_call_client", _ch)
        {
            if (rsp_cb_gate_call_client_handle == nullptr){
                rsp_cb_gate_call_client_handle = std::make_shared<gate_call_client_rsp_cb>();
                rsp_cb_gate_call_client_handle->Init(modules);
            }
            uuid.store(random());
        }

        void ntf_cuuid(std::string cuuid){
            msgpack11::MsgPack::array _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1;
            _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1.push_back(cuuid);
            call_module_method("ntf_cuuid", _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1);
        }

        std::shared_ptr<gate_call_client_heartbeats_cb> heartbeats(uint64_t timetmp){
            auto uuid_a514ca5f_2c67_5668_aac0_354397bdce36 = uuid++;
            msgpack11::MsgPack::array _argv_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56;
            _argv_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56.push_back(uuid_a514ca5f_2c67_5668_aac0_354397bdce36);
            _argv_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56.push_back(timetmp);
            call_module_method("heartbeats", _argv_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56);

            auto cb_heartbeats_obj = std::make_shared<gate_call_client_heartbeats_cb>(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, rsp_cb_gate_call_client_handle);
            std::lock_guard<std::mutex> l(rsp_cb_gate_call_client_handle->mutex_map_heartbeats);
            rsp_cb_gate_call_client_handle->map_heartbeats.insert(std::make_pair(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, cb_heartbeats_obj));
            return cb_heartbeats_obj;
        }

        void call_client(std::vector<uint8_t> rpc_argv){
            msgpack11::MsgPack::array _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab;
            _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push_back(rpc_argv);
            call_module_method("call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
        }

    };
/*this module code is codegen by abelkhan codegen for cpp*/
    class gate_call_client_heartbeats_rsp : Response {
    private:
        uint64_t uuid;

    public:
        gate_call_client_heartbeats_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("gate_call_client_rsp_cb", _ch)
        {
            uuid = _uuid;
        }

        void rsp(){
            msgpack11::MsgPack::array _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4;
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.push_back(uuid);
            call_module_method("heartbeats_rsp", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        }

        void err(){
            msgpack11::MsgPack::array _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4;
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.push_back(uuid);
            call_module_method("heartbeats_err", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        }

    };

    class gate_call_client_module : Imodule, public std::enable_shared_from_this<gate_call_client_module>{
    public:
        gate_call_client_module() : Imodule("gate_call_client")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("ntf_cuuid", std::bind(&gate_call_client_module::ntf_cuuid, this, std::placeholders::_1));
            reg_method("heartbeats", std::bind(&gate_call_client_module::heartbeats, this, std::placeholders::_1));
            reg_method("call_client", std::bind(&gate_call_client_module::call_client, this, std::placeholders::_1));
        }

        concurrent::signals<void(std::string cuuid)> sig_ntf_cuuid;
        void ntf_cuuid(const msgpack11::MsgPack::array& inArray){
            auto _cuuid = inArray[0].string_value();
            sig_ntf_cuuid.emit(_cuuid);
        }

        concurrent::signals<void(uint64_t timetmp)> sig_heartbeats;
        void heartbeats(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            auto _timetmp = inArray[1].uint64_value();
            rsp = std::make_shared<gate_call_client_heartbeats_rsp>(current_ch, _cb_uuid);
            sig_heartbeats.emit(_timetmp);
            rsp = nullptr;
        }

        concurrent::signals<void(std::vector<uint8_t> rpc_argv)> sig_call_client;
        void call_client(const msgpack11::MsgPack::array& inArray){
            auto _rpc_argv = inArray[0].binary_items();
            sig_call_client.emit(_rpc_argv);
        }

    };

}

#endif //_h_client_47f7b5bf_5eb0_3794_bd8c_e38da400d730_
