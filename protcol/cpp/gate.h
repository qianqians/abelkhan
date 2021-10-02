#ifndef _h_gate_be3bc387_f3f2_3e39_a1fc_9dcf34921d99_
#define _h_gate_be3bc387_f3f2_3e39_a1fc_9dcf34921d99_

#include <abelkhan.h>
#include <signals.h>

namespace abelkhan
{
/*this enum code is codegen by abelkhan codegen for cpp*/

/*this struct code is codegen by abelkhan codegen for cpp*/
    class hub_info {
    public:
        std::string hub_name;
        std::string hub_type;

    public:
        static msgpack11::MsgPack::object hub_info_to_protcol(hub_info _struct){
            msgpack11::MsgPack::object _protocol;
            _protocol.insert(std::make_pair("hub_name", _struct.hub_name));
            _protocol.insert(std::make_pair("hub_type", _struct.hub_type));
            return _protocol;
        }

        static hub_info protcol_to_hub_info(const msgpack11::MsgPack::object& _protocol){
            hub_info _struct4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2;
            for(auto i : _protocol){
                if (i.first == "hub_name"){
                    _struct4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2.hub_name = i.second.string_value();
                }
                else if (i.first == "hub_type"){
                    _struct4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2.hub_type = i.second.string_value();
                }
            }
            return _struct4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2;
        }
    };

/*this caller code is codegen by abelkhan codegen for cpp*/
    class hub_call_gate_rsp_cb;
    class hub_call_gate_reg_hub_cb : public std::enable_shared_from_this<hub_call_gate_reg_hub_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<hub_call_gate_rsp_cb> module_rsp_cb;

    public:
        hub_call_gate_reg_hub_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_gate_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void()> sig_reg_hub_cb;
        concurrent::signals<void()> sig_reg_hub_err;
        concurrent::signals<void()> sig_reg_hub_timeout;

        std::shared_ptr<hub_call_gate_reg_hub_cb> callBack(std::function<void()> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

/*this cb code is codegen by abelkhan for cpp*/
    class hub_call_gate_rsp_cb : public Imodule, public std::enable_shared_from_this<hub_call_gate_rsp_cb>{
    public:
        std::mutex mutex_map_reg_hub;
        std::map<uint64_t, std::shared_ptr<hub_call_gate_reg_hub_cb> > map_reg_hub;
        hub_call_gate_rsp_cb() : Imodule("hub_call_gate_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("reg_hub_rsp", std::bind(&hub_call_gate_rsp_cb::reg_hub_rsp, this, std::placeholders::_1));
            reg_method("reg_hub_err", std::bind(&hub_call_gate_rsp_cb::reg_hub_err, this, std::placeholders::_1));
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

        std::shared_ptr<hub_call_gate_reg_hub_cb> try_get_and_del_reg_hub_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_reg_hub);
            auto rsp = map_reg_hub[uuid];
            map_reg_hub.erase(uuid);
            return rsp;
        }

    };

    class hub_call_gate_caller : Icaller {
    private:
        static std::shared_ptr<hub_call_gate_rsp_cb> rsp_cb_hub_call_gate_handle;

    private:
        std::atomic<uint64_t> uuid_9796175c_1119_3833_bf31_5ee139b40edc;

    public:
        hub_call_gate_caller(std::shared_ptr<Ichannel> _ch, std::shared_ptr<modulemng> modules) : Icaller("hub_call_gate", _ch)
        {
            if (rsp_cb_hub_call_gate_handle == nullptr){
                rsp_cb_hub_call_gate_handle = std::make_shared<hub_call_gate_rsp_cb>();
                rsp_cb_hub_call_gate_handle->Init(modules);
            }
            uuid_9796175c_1119_3833_bf31_5ee139b40edc.store(random());
        }

        std::shared_ptr<hub_call_gate_reg_hub_cb> reg_hub(std::string hub_name, std::string hub_type){
            auto uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = uuid_9796175c_1119_3833_bf31_5ee139b40edc++;
            msgpack11::MsgPack::array _argv_d47a6c8a_5494_35bb_9bc5_60d20f624f67;
            _argv_d47a6c8a_5494_35bb_9bc5_60d20f624f67.push_back(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106);
            _argv_d47a6c8a_5494_35bb_9bc5_60d20f624f67.push_back(hub_name);
            _argv_d47a6c8a_5494_35bb_9bc5_60d20f624f67.push_back(hub_type);
            call_module_method("reg_hub", _argv_d47a6c8a_5494_35bb_9bc5_60d20f624f67);

            auto cb_reg_hub_obj = std::make_shared<hub_call_gate_reg_hub_cb>(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, rsp_cb_hub_call_gate_handle);
            std::lock_guard<std::mutex> l(rsp_cb_hub_call_gate_handle->mutex_map_reg_hub);
            rsp_cb_hub_call_gate_handle->map_reg_hub.insert(std::make_pair(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, cb_reg_hub_obj));
            return cb_reg_hub_obj;
        }

        void disconnect_client(std::string client_uuid){
            msgpack11::MsgPack::array _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85;
            _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85.push_back(client_uuid);
            call_module_method("disconnect_client", _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85);
        }

        void forward_hub_call_client(std::string client_uuid, std::vector<uint8_t> rpc_argv){
            msgpack11::MsgPack::array _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1;
            _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1.push_back(client_uuid);
            _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1.push_back(rpc_argv);
            call_module_method("forward_hub_call_client", _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1);
        }

        void forward_hub_call_group_client(std::vector<std::string> client_uuids, std::vector<uint8_t> rpc_argv){
            msgpack11::MsgPack::array _argv_374b012d_0718_3f84_91f0_784b12f8189c;
            msgpack11::MsgPack::array _array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1;
            for(auto v_a8e916a0_4f15_5329_b872_d0ff30ba7fe6 : client_uuids){
                _array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1.push_back(v_a8e916a0_4f15_5329_b872_d0ff30ba7fe6);
            }
            _argv_374b012d_0718_3f84_91f0_784b12f8189c.push_back(_array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1);
            _argv_374b012d_0718_3f84_91f0_784b12f8189c.push_back(rpc_argv);
            call_module_method("forward_hub_call_group_client", _argv_374b012d_0718_3f84_91f0_784b12f8189c);
        }

        void forward_hub_call_global_client(std::vector<uint8_t> rpc_argv){
            msgpack11::MsgPack::array _argv_f69241c3_642a_3b51_bb37_cf638176493a;
            _argv_f69241c3_642a_3b51_bb37_cf638176493a.push_back(rpc_argv);
            call_module_method("forward_hub_call_global_client", _argv_f69241c3_642a_3b51_bb37_cf638176493a);
        }

    };
    class client_call_gate_rsp_cb;
    class client_call_gate_heartbeats_cb : public std::enable_shared_from_this<client_call_gate_heartbeats_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<client_call_gate_rsp_cb> module_rsp_cb;

    public:
        client_call_gate_heartbeats_cb(uint64_t _cb_uuid, std::shared_ptr<client_call_gate_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void(uint64_t)> sig_heartbeats_cb;
        concurrent::signals<void()> sig_heartbeats_err;
        concurrent::signals<void()> sig_heartbeats_timeout;

        std::shared_ptr<client_call_gate_heartbeats_cb> callBack(std::function<void(uint64_t timetmp)> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

    class client_call_gate_rsp_cb;
    class client_call_gate_get_hub_info_cb : public std::enable_shared_from_this<client_call_gate_get_hub_info_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<client_call_gate_rsp_cb> module_rsp_cb;

    public:
        client_call_gate_get_hub_info_cb(uint64_t _cb_uuid, std::shared_ptr<client_call_gate_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void(std::vector<hub_info>)> sig_get_hub_info_cb;
        concurrent::signals<void()> sig_get_hub_info_err;
        concurrent::signals<void()> sig_get_hub_info_timeout;

        std::shared_ptr<client_call_gate_get_hub_info_cb> callBack(std::function<void(std::vector<hub_info> hub_info)> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

/*this cb code is codegen by abelkhan for cpp*/
    class client_call_gate_rsp_cb : public Imodule, public std::enable_shared_from_this<client_call_gate_rsp_cb>{
    public:
        std::mutex mutex_map_heartbeats;
        std::map<uint64_t, std::shared_ptr<client_call_gate_heartbeats_cb> > map_heartbeats;
        std::mutex mutex_map_get_hub_info;
        std::map<uint64_t, std::shared_ptr<client_call_gate_get_hub_info_cb> > map_get_hub_info;
        client_call_gate_rsp_cb() : Imodule("client_call_gate_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("heartbeats_rsp", std::bind(&client_call_gate_rsp_cb::heartbeats_rsp, this, std::placeholders::_1));
            reg_method("heartbeats_err", std::bind(&client_call_gate_rsp_cb::heartbeats_err, this, std::placeholders::_1));
            reg_method("get_hub_info_rsp", std::bind(&client_call_gate_rsp_cb::get_hub_info_rsp, this, std::placeholders::_1));
            reg_method("get_hub_info_err", std::bind(&client_call_gate_rsp_cb::get_hub_info_err, this, std::placeholders::_1));
        }

        void heartbeats_rsp(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto _timetmp = inArray[1].uint64_value();
            auto rsp = try_get_and_del_heartbeats_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_heartbeats_cb.emit(_timetmp);
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

        std::shared_ptr<client_call_gate_heartbeats_cb> try_get_and_del_heartbeats_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_heartbeats);
            auto rsp = map_heartbeats[uuid];
            map_heartbeats.erase(uuid);
            return rsp;
        }

        void get_hub_info_rsp(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            std::vector<hub_info> _hub_info;
            auto _protocol_array = inArray[1].array_items();
            for(auto it_53b78086_1765_5879_87b4_63333838766a : _protocol_array){
                _hub_info.push_back(hub_info::protcol_to_hub_info(it_53b78086_1765_5879_87b4_63333838766a.object_items()));
            }
            auto rsp = try_get_and_del_get_hub_info_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_get_hub_info_cb.emit(_hub_info);
            }
        }

        void get_hub_info_err(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_get_hub_info_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_get_hub_info_err.emit();
            }
        }

        void get_hub_info_timeout(uint64_t cb_uuid){
            auto rsp = try_get_and_del_get_hub_info_cb(cb_uuid);
            if (rsp != nullptr){
                rsp->sig_get_hub_info_timeout.emit();
            }
        }

        std::shared_ptr<client_call_gate_get_hub_info_cb> try_get_and_del_get_hub_info_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_get_hub_info);
            auto rsp = map_get_hub_info[uuid];
            map_get_hub_info.erase(uuid);
            return rsp;
        }

    };

    class client_call_gate_caller : Icaller {
    private:
        static std::shared_ptr<client_call_gate_rsp_cb> rsp_cb_client_call_gate_handle;

    private:
        std::atomic<uint64_t> uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2;

    public:
        client_call_gate_caller(std::shared_ptr<Ichannel> _ch, std::shared_ptr<modulemng> modules) : Icaller("client_call_gate", _ch)
        {
            if (rsp_cb_client_call_gate_handle == nullptr){
                rsp_cb_client_call_gate_handle = std::make_shared<client_call_gate_rsp_cb>();
                rsp_cb_client_call_gate_handle->Init(modules);
            }
            uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2.store(random());
        }

        std::shared_ptr<client_call_gate_heartbeats_cb> heartbeats(){
            auto uuid_a514ca5f_2c67_5668_aac0_354397bdce36 = uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2++;
            msgpack11::MsgPack::array _argv_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56;
            _argv_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56.push_back(uuid_a514ca5f_2c67_5668_aac0_354397bdce36);
            call_module_method("heartbeats", _argv_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56);

            auto cb_heartbeats_obj = std::make_shared<client_call_gate_heartbeats_cb>(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, rsp_cb_client_call_gate_handle);
            std::lock_guard<std::mutex> l(rsp_cb_client_call_gate_handle->mutex_map_heartbeats);
            rsp_cb_client_call_gate_handle->map_heartbeats.insert(std::make_pair(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, cb_heartbeats_obj));
            return cb_heartbeats_obj;
        }

        std::shared_ptr<client_call_gate_get_hub_info_cb> get_hub_info(std::string hub_type){
            auto uuid_e9d2753f_7d38_512d_80ff_7aae13508048 = uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2++;
            msgpack11::MsgPack::array _argv_db7b7f0f_c3d0_380b_b51e_53fea108bc3b;
            _argv_db7b7f0f_c3d0_380b_b51e_53fea108bc3b.push_back(uuid_e9d2753f_7d38_512d_80ff_7aae13508048);
            _argv_db7b7f0f_c3d0_380b_b51e_53fea108bc3b.push_back(hub_type);
            call_module_method("get_hub_info", _argv_db7b7f0f_c3d0_380b_b51e_53fea108bc3b);

            auto cb_get_hub_info_obj = std::make_shared<client_call_gate_get_hub_info_cb>(uuid_e9d2753f_7d38_512d_80ff_7aae13508048, rsp_cb_client_call_gate_handle);
            std::lock_guard<std::mutex> l(rsp_cb_client_call_gate_handle->mutex_map_get_hub_info);
            rsp_cb_client_call_gate_handle->map_get_hub_info.insert(std::make_pair(uuid_e9d2753f_7d38_512d_80ff_7aae13508048, cb_get_hub_info_obj));
            return cb_get_hub_info_obj;
        }

        void forward_client_call_hub(std::string hub_name, std::vector<uint8_t> rpc_argv){
            msgpack11::MsgPack::array _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5;
            _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5.push_back(hub_name);
            _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5.push_back(rpc_argv);
            call_module_method("forward_client_call_hub", _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5);
        }

    };
/*this module code is codegen by abelkhan codegen for cpp*/
    class hub_call_gate_reg_hub_rsp : public Response {
    private:
        uint64_t uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67;

    public:
        hub_call_gate_reg_hub_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("hub_call_gate_rsp_cb", _ch)
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

    class hub_call_gate_module : public Imodule, public std::enable_shared_from_this<hub_call_gate_module>{
    public:
        hub_call_gate_module() : Imodule("hub_call_gate")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("reg_hub", std::bind(&hub_call_gate_module::reg_hub, this, std::placeholders::_1));
            reg_method("disconnect_client", std::bind(&hub_call_gate_module::disconnect_client, this, std::placeholders::_1));
            reg_method("forward_hub_call_client", std::bind(&hub_call_gate_module::forward_hub_call_client, this, std::placeholders::_1));
            reg_method("forward_hub_call_group_client", std::bind(&hub_call_gate_module::forward_hub_call_group_client, this, std::placeholders::_1));
            reg_method("forward_hub_call_global_client", std::bind(&hub_call_gate_module::forward_hub_call_global_client, this, std::placeholders::_1));
        }

        concurrent::signals<void(std::string, std::string)> sig_reg_hub;
        void reg_hub(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            auto _hub_name = inArray[1].string_value();
            auto _hub_type = inArray[2].string_value();
            rsp = std::make_shared<hub_call_gate_reg_hub_rsp>(current_ch, _cb_uuid);
            sig_reg_hub.emit(_hub_name, _hub_type);
            rsp = nullptr;
        }

        concurrent::signals<void(std::string)> sig_disconnect_client;
        void disconnect_client(const msgpack11::MsgPack::array& inArray){
            auto _client_uuid = inArray[0].string_value();
            sig_disconnect_client.emit(_client_uuid);
        }

        concurrent::signals<void(std::string, std::vector<uint8_t>)> sig_forward_hub_call_client;
        void forward_hub_call_client(const msgpack11::MsgPack::array& inArray){
            auto _client_uuid = inArray[0].string_value();
            auto _rpc_argv = inArray[1].binary_items();
            sig_forward_hub_call_client.emit(_client_uuid, _rpc_argv);
        }

        concurrent::signals<void(std::vector<std::string>, std::vector<uint8_t>)> sig_forward_hub_call_group_client;
        void forward_hub_call_group_client(const msgpack11::MsgPack::array& inArray){
            std::vector<std::string> _client_uuids;
            auto _protocol_array = inArray[0].array_items();
            for(auto it_dfd11414_89c9_5adb_8977_69b93b30195b : _protocol_array){
                _client_uuids.push_back(it_dfd11414_89c9_5adb_8977_69b93b30195b.string_value());
            }
            auto _rpc_argv = inArray[1].binary_items();
            sig_forward_hub_call_group_client.emit(_client_uuids, _rpc_argv);
        }

        concurrent::signals<void(std::vector<uint8_t>)> sig_forward_hub_call_global_client;
        void forward_hub_call_global_client(const msgpack11::MsgPack::array& inArray){
            auto _rpc_argv = inArray[0].binary_items();
            sig_forward_hub_call_global_client.emit(_rpc_argv);
        }

    };
    class client_call_gate_heartbeats_rsp : public Response {
    private:
        uint64_t uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56;

    public:
        client_call_gate_heartbeats_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("client_call_gate_rsp_cb", _ch)
        {
            uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56 = _uuid;
        }

        void rsp(uint64_t timetmp){
            msgpack11::MsgPack::array _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4;
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.push_back(uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56);
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.push_back(timetmp);
            call_module_method("heartbeats_rsp", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        }

        void err(){
            msgpack11::MsgPack::array _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4;
            _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.push_back(uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56);
            call_module_method("heartbeats_err", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        }

    };

    class client_call_gate_get_hub_info_rsp : public Response {
    private:
        uint64_t uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b;

    public:
        client_call_gate_get_hub_info_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("client_call_gate_rsp_cb", _ch)
        {
            uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b = _uuid;
        }

        void rsp(std::vector<hub_info> hub_info){
            msgpack11::MsgPack::array _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24;
            _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.push_back(uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b);
            msgpack11::MsgPack::array _array_4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2;
            for(auto v_53b78086_1765_5879_87b4_63333838766a : hub_info){
                _array_4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2.push_back(hub_info::hub_info_to_protcol(v_53b78086_1765_5879_87b4_63333838766a));
            }
            _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.push_back(_array_4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2);
            call_module_method("get_hub_info_rsp", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
        }

        void err(){
            msgpack11::MsgPack::array _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24;
            _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.push_back(uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b);
            call_module_method("get_hub_info_err", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
        }

    };

    class client_call_gate_module : public Imodule, public std::enable_shared_from_this<client_call_gate_module>{
    public:
        client_call_gate_module() : Imodule("client_call_gate")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("heartbeats", std::bind(&client_call_gate_module::heartbeats, this, std::placeholders::_1));
            reg_method("get_hub_info", std::bind(&client_call_gate_module::get_hub_info, this, std::placeholders::_1));
            reg_method("forward_client_call_hub", std::bind(&client_call_gate_module::forward_client_call_hub, this, std::placeholders::_1));
        }

        concurrent::signals<void()> sig_heartbeats;
        void heartbeats(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            rsp = std::make_shared<client_call_gate_heartbeats_rsp>(current_ch, _cb_uuid);
            sig_heartbeats.emit();
            rsp = nullptr;
        }

        concurrent::signals<void(std::string)> sig_get_hub_info;
        void get_hub_info(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            auto _hub_type = inArray[1].string_value();
            rsp = std::make_shared<client_call_gate_get_hub_info_rsp>(current_ch, _cb_uuid);
            sig_get_hub_info.emit(_hub_type);
            rsp = nullptr;
        }

        concurrent::signals<void(std::string, std::vector<uint8_t>)> sig_forward_client_call_hub;
        void forward_client_call_hub(const msgpack11::MsgPack::array& inArray){
            auto _hub_name = inArray[0].string_value();
            auto _rpc_argv = inArray[1].binary_items();
            sig_forward_client_call_hub.emit(_hub_name, _rpc_argv);
        }

    };

}

#endif //_h_gate_be3bc387_f3f2_3e39_a1fc_9dcf34921d99_
