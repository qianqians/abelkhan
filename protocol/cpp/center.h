#ifndef _h_center_fd1a4f35_9b23_3f22_8094_3acc5aecb066_
#define _h_center_fd1a4f35_9b23_3f22_8094_3acc5aecb066_

#include <abelkhan.h>
#include <signals.h>


namespace abelkhan
{
/*this enum code is codegen by abelkhan codegen for cpp*/

/*this struct code is codegen by abelkhan codegen for cpp*/
/*this caller code is codegen by abelkhan codegen for cpp*/
    class center_rsp_cb;
    class center_reg_server_mq_cb : public std::enable_shared_from_this<center_reg_server_mq_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<center_rsp_cb> module_rsp_cb;

    public:
        center_reg_server_mq_cb(uint64_t _cb_uuid, std::shared_ptr<center_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void()> sig_reg_server_mq_cb;
        concurrent::signals<void()> sig_reg_server_mq_err;
        concurrent::signals<void()> sig_reg_server_mq_timeout;

        std::shared_ptr<center_reg_server_mq_cb> callBack(std::function<void()> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

    class center_rsp_cb;
    class center_reconn_reg_server_mq_cb : public std::enable_shared_from_this<center_reconn_reg_server_mq_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<center_rsp_cb> module_rsp_cb;

    public:
        center_reconn_reg_server_mq_cb(uint64_t _cb_uuid, std::shared_ptr<center_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void()> sig_reconn_reg_server_mq_cb;
        concurrent::signals<void()> sig_reconn_reg_server_mq_err;
        concurrent::signals<void()> sig_reconn_reg_server_mq_timeout;

        std::shared_ptr<center_reconn_reg_server_mq_cb> callBack(std::function<void()> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

    class center_rsp_cb;
    class center_heartbeat_cb : public std::enable_shared_from_this<center_heartbeat_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<center_rsp_cb> module_rsp_cb;

    public:
        center_heartbeat_cb(uint64_t _cb_uuid, std::shared_ptr<center_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void()> sig_heartbeat_cb;
        concurrent::signals<void()> sig_heartbeat_err;
        concurrent::signals<void()> sig_heartbeat_timeout;

        std::shared_ptr<center_heartbeat_cb> callBack(std::function<void()> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

/*this cb code is codegen by abelkhan for cpp*/
    class center_rsp_cb : public Imodule, public std::enable_shared_from_this<center_rsp_cb>{
    public:
        std::mutex mutex_map_reg_server_mq;
        std::unordered_map<uint64_t, std::shared_ptr<center_reg_server_mq_cb> > map_reg_server_mq;
        std::mutex mutex_map_reconn_reg_server_mq;
        std::unordered_map<uint64_t, std::shared_ptr<center_reconn_reg_server_mq_cb> > map_reconn_reg_server_mq;
        std::mutex mutex_map_heartbeat;
        std::unordered_map<uint64_t, std::shared_ptr<center_heartbeat_cb> > map_heartbeat;
        center_rsp_cb() : Imodule("center_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_method("center_rsp_cb_reg_server_mq_rsp", std::make_tuple(shared_from_this(), std::bind(&center_rsp_cb::reg_server_mq_rsp, this, std::placeholders::_1)));
            modules->reg_method("center_rsp_cb_reg_server_mq_err", std::make_tuple(shared_from_this(), std::bind(&center_rsp_cb::reg_server_mq_err, this, std::placeholders::_1)));
            modules->reg_method("center_rsp_cb_reconn_reg_server_mq_rsp", std::make_tuple(shared_from_this(), std::bind(&center_rsp_cb::reconn_reg_server_mq_rsp, this, std::placeholders::_1)));
            modules->reg_method("center_rsp_cb_reconn_reg_server_mq_err", std::make_tuple(shared_from_this(), std::bind(&center_rsp_cb::reconn_reg_server_mq_err, this, std::placeholders::_1)));
            modules->reg_method("center_rsp_cb_heartbeat_rsp", std::make_tuple(shared_from_this(), std::bind(&center_rsp_cb::heartbeat_rsp, this, std::placeholders::_1)));
            modules->reg_method("center_rsp_cb_heartbeat_err", std::make_tuple(shared_from_this(), std::bind(&center_rsp_cb::heartbeat_err, this, std::placeholders::_1)));
        }

        void reg_server_mq_rsp(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_reg_server_mq_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_reg_server_mq_cb.emit();
            }
        }

        void reg_server_mq_err(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_reg_server_mq_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_reg_server_mq_err.emit();
            }
        }

        void reg_server_mq_timeout(uint64_t cb_uuid){
            auto rsp = try_get_and_del_reg_server_mq_cb(cb_uuid);
            if (rsp != nullptr){
                rsp->sig_reg_server_mq_timeout.emit();
            }
        }

        std::shared_ptr<center_reg_server_mq_cb> try_get_and_del_reg_server_mq_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_reg_server_mq);
            if (map_reg_server_mq.find(uuid) != map_reg_server_mq.end()) {
                auto rsp = map_reg_server_mq[uuid];
                map_reg_server_mq.erase(uuid);
                return rsp;
            }
            return nullptr;
        }

        void reconn_reg_server_mq_rsp(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_reconn_reg_server_mq_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_reconn_reg_server_mq_cb.emit();
            }
        }

        void reconn_reg_server_mq_err(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_reconn_reg_server_mq_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_reconn_reg_server_mq_err.emit();
            }
        }

        void reconn_reg_server_mq_timeout(uint64_t cb_uuid){
            auto rsp = try_get_and_del_reconn_reg_server_mq_cb(cb_uuid);
            if (rsp != nullptr){
                rsp->sig_reconn_reg_server_mq_timeout.emit();
            }
        }

        std::shared_ptr<center_reconn_reg_server_mq_cb> try_get_and_del_reconn_reg_server_mq_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_reconn_reg_server_mq);
            if (map_reconn_reg_server_mq.find(uuid) != map_reconn_reg_server_mq.end()) {
                auto rsp = map_reconn_reg_server_mq[uuid];
                map_reconn_reg_server_mq.erase(uuid);
                return rsp;
            }
            return nullptr;
        }

        void heartbeat_rsp(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_heartbeat_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_heartbeat_cb.emit();
            }
        }

        void heartbeat_err(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_heartbeat_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_heartbeat_err.emit();
            }
        }

        void heartbeat_timeout(uint64_t cb_uuid){
            auto rsp = try_get_and_del_heartbeat_cb(cb_uuid);
            if (rsp != nullptr){
                rsp->sig_heartbeat_timeout.emit();
            }
        }

        std::shared_ptr<center_heartbeat_cb> try_get_and_del_heartbeat_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_heartbeat);
            if (map_heartbeat.find(uuid) != map_heartbeat.end()) {
                auto rsp = map_heartbeat[uuid];
                map_heartbeat.erase(uuid);
                return rsp;
            }
            return nullptr;
        }

    };

    class center_caller : Icaller {
    private:
        static std::shared_ptr<center_rsp_cb> rsp_cb_center_handle;

    private:
        std::atomic<uint64_t> uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066;

    public:
        center_caller(std::shared_ptr<Ichannel> _ch, std::shared_ptr<modulemng> modules) : Icaller("center", _ch)
        {
            if (rsp_cb_center_handle == nullptr){
                rsp_cb_center_handle = std::make_shared<center_rsp_cb>();
                rsp_cb_center_handle->Init(modules);
            }
            uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066.store(random());
        }

        std::shared_ptr<center_reg_server_mq_cb> reg_server_mq(std::string type, std::string svr_name){
            auto uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf = uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066++;
            msgpack11::MsgPack::array _argv_7254d987_ac9c_3d73_831c_f43efb3268a9;
            _argv_7254d987_ac9c_3d73_831c_f43efb3268a9.push_back(uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf);
            _argv_7254d987_ac9c_3d73_831c_f43efb3268a9.push_back(type);
            _argv_7254d987_ac9c_3d73_831c_f43efb3268a9.push_back(svr_name);
            call_module_method("center_reg_server_mq", _argv_7254d987_ac9c_3d73_831c_f43efb3268a9);

            auto cb_reg_server_mq_obj = std::make_shared<center_reg_server_mq_cb>(uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf, rsp_cb_center_handle);
            std::lock_guard<std::mutex> l(rsp_cb_center_handle->mutex_map_reg_server_mq);
            rsp_cb_center_handle->map_reg_server_mq.insert(std::make_pair(uuid_76a34a7f_e1e5_5f58_931b_9a21db9858bf, cb_reg_server_mq_obj));
            return cb_reg_server_mq_obj;
        }

        std::shared_ptr<center_reconn_reg_server_mq_cb> reconn_reg_server_mq(std::string type, std::string svr_name){
            auto uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21 = uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066++;
            msgpack11::MsgPack::array _argv_4d058274_a122_382e_8084_b9067ed713c5;
            _argv_4d058274_a122_382e_8084_b9067ed713c5.push_back(uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21);
            _argv_4d058274_a122_382e_8084_b9067ed713c5.push_back(type);
            _argv_4d058274_a122_382e_8084_b9067ed713c5.push_back(svr_name);
            call_module_method("center_reconn_reg_server_mq", _argv_4d058274_a122_382e_8084_b9067ed713c5);

            auto cb_reconn_reg_server_mq_obj = std::make_shared<center_reconn_reg_server_mq_cb>(uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21, rsp_cb_center_handle);
            std::lock_guard<std::mutex> l(rsp_cb_center_handle->mutex_map_reconn_reg_server_mq);
            rsp_cb_center_handle->map_reconn_reg_server_mq.insert(std::make_pair(uuid_0012a813_9a7b_57c8_a9d1_9a08790cad21, cb_reconn_reg_server_mq_obj));
            return cb_reconn_reg_server_mq_obj;
        }

        std::shared_ptr<center_heartbeat_cb> heartbeat(uint32_t tick){
            auto uuid_9654538a_9916_57dc_8ea5_806086d7a378 = uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066++;
            msgpack11::MsgPack::array _argv_617b63d0_e6d6_3c80_8c13_63a98d39e89f;
            _argv_617b63d0_e6d6_3c80_8c13_63a98d39e89f.push_back(uuid_9654538a_9916_57dc_8ea5_806086d7a378);
            _argv_617b63d0_e6d6_3c80_8c13_63a98d39e89f.push_back(tick);
            call_module_method("center_heartbeat", _argv_617b63d0_e6d6_3c80_8c13_63a98d39e89f);

            auto cb_heartbeat_obj = std::make_shared<center_heartbeat_cb>(uuid_9654538a_9916_57dc_8ea5_806086d7a378, rsp_cb_center_handle);
            std::lock_guard<std::mutex> l(rsp_cb_center_handle->mutex_map_heartbeat);
            rsp_cb_center_handle->map_heartbeat.insert(std::make_pair(uuid_9654538a_9916_57dc_8ea5_806086d7a378, cb_heartbeat_obj));
            return cb_heartbeat_obj;
        }

        void closed(){
            msgpack11::MsgPack::array _argv_e0d5aabb_d671_3c7e_99e4_6d374f7fd4ee;
            call_module_method("center_closed", _argv_e0d5aabb_d671_3c7e_99e4_6d374f7fd4ee);
        }

    };
/*this cb code is codegen by abelkhan for cpp*/
    class center_call_server_rsp_cb : public Imodule, public std::enable_shared_from_this<center_call_server_rsp_cb>{
    public:
        center_call_server_rsp_cb() : Imodule("center_call_server_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
        }

    };

    class center_call_server_caller : Icaller {
    private:
        static std::shared_ptr<center_call_server_rsp_cb> rsp_cb_center_call_server_handle;

    private:
        std::atomic<uint64_t> uuid_8c11e5bb_e9ff_3a0b_a436_65a9922a8da5;

    public:
        center_call_server_caller(std::shared_ptr<Ichannel> _ch, std::shared_ptr<modulemng> modules) : Icaller("center_call_server", _ch)
        {
            if (rsp_cb_center_call_server_handle == nullptr){
                rsp_cb_center_call_server_handle = std::make_shared<center_call_server_rsp_cb>();
                rsp_cb_center_call_server_handle->Init(modules);
            }
            uuid_8c11e5bb_e9ff_3a0b_a436_65a9922a8da5.store(random());
        }

        void close_server(){
            msgpack11::MsgPack::array _argv_8394af17_8a06_3068_977d_477a1276f56e;
            call_module_method("center_call_server_close_server", _argv_8394af17_8a06_3068_977d_477a1276f56e);
        }

        void console_close_server(std::string svr_type, std::string svr_name){
            msgpack11::MsgPack::array _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc;
            _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc.push_back(svr_type);
            _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc.push_back(svr_name);
            call_module_method("center_call_server_console_close_server", _argv_57b322da_74a5_3d2e_9f27_bf5bc1921fcc);
        }

        void svr_be_closed(std::string svr_type, std::string svr_name){
            msgpack11::MsgPack::array _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac;
            _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac.push_back(svr_type);
            _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac.push_back(svr_name);
            call_module_method("center_call_server_svr_be_closed", _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac);
        }

    };
/*this cb code is codegen by abelkhan for cpp*/
    class center_call_hub_rsp_cb : public Imodule, public std::enable_shared_from_this<center_call_hub_rsp_cb>{
    public:
        center_call_hub_rsp_cb() : Imodule("center_call_hub_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
        }

    };

    class center_call_hub_caller : Icaller {
    private:
        static std::shared_ptr<center_call_hub_rsp_cb> rsp_cb_center_call_hub_handle;

    private:
        std::atomic<uint64_t> uuid_adbd1e34_0c90_3426_aefa_4d734c07a706;

    public:
        center_call_hub_caller(std::shared_ptr<Ichannel> _ch, std::shared_ptr<modulemng> modules) : Icaller("center_call_hub", _ch)
        {
            if (rsp_cb_center_call_hub_handle == nullptr){
                rsp_cb_center_call_hub_handle = std::make_shared<center_call_hub_rsp_cb>();
                rsp_cb_center_call_hub_handle->Init(modules);
            }
            uuid_adbd1e34_0c90_3426_aefa_4d734c07a706.store(random());
        }

        void distribute_server_mq(std::string svr_type, std::string svr_name){
            msgpack11::MsgPack::array _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393;
            _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393.push_back(svr_type);
            _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393.push_back(svr_name);
            call_module_method("center_call_hub_distribute_server_mq", _argv_b4cefb58_72e6_34e7_8f22_562a06a9b393);
        }

        void reload(std::string argv){
            msgpack11::MsgPack::array _argv_ba37af53_beea_3d61_82e1_8d15e335971d;
            _argv_ba37af53_beea_3d61_82e1_8d15e335971d.push_back(argv);
            call_module_method("center_call_hub_reload", _argv_ba37af53_beea_3d61_82e1_8d15e335971d);
        }

    };
/*this cb code is codegen by abelkhan for cpp*/
    class gm_center_rsp_cb : public Imodule, public std::enable_shared_from_this<gm_center_rsp_cb>{
    public:
        gm_center_rsp_cb() : Imodule("gm_center_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
        }

    };

    class gm_center_caller : Icaller {
    private:
        static std::shared_ptr<gm_center_rsp_cb> rsp_cb_gm_center_handle;

    private:
        std::atomic<uint64_t> uuid_130fb971_5ae0_3446_b480_f9ee83dbeb28;

    public:
        gm_center_caller(std::shared_ptr<Ichannel> _ch, std::shared_ptr<modulemng> modules) : Icaller("gm_center", _ch)
        {
            if (rsp_cb_gm_center_handle == nullptr){
                rsp_cb_gm_center_handle = std::make_shared<gm_center_rsp_cb>();
                rsp_cb_gm_center_handle->Init(modules);
            }
            uuid_130fb971_5ae0_3446_b480_f9ee83dbeb28.store(random());
        }

        void confirm_gm(std::string gm_name){
            msgpack11::MsgPack::array _argv_d097689c_b711_3837_8783_64916ae34cea;
            _argv_d097689c_b711_3837_8783_64916ae34cea.push_back(gm_name);
            call_module_method("gm_center_confirm_gm", _argv_d097689c_b711_3837_8783_64916ae34cea);
        }

        void close_clutter(std::string gmname){
            msgpack11::MsgPack::array _argv_576c03c3_06da_36a2_b868_752da601cb54;
            _argv_576c03c3_06da_36a2_b868_752da601cb54.push_back(gmname);
            call_module_method("gm_center_close_clutter", _argv_576c03c3_06da_36a2_b868_752da601cb54);
        }

        void reload(std::string gmname, std::string argv){
            msgpack11::MsgPack::array _argv_ba37af53_beea_3d61_82e1_8d15e335971d;
            _argv_ba37af53_beea_3d61_82e1_8d15e335971d.push_back(gmname);
            _argv_ba37af53_beea_3d61_82e1_8d15e335971d.push_back(argv);
            call_module_method("gm_center_reload", _argv_ba37af53_beea_3d61_82e1_8d15e335971d);
        }

    };
/*this module code is codegen by abelkhan codegen for cpp*/
    class center_reg_server_mq_rsp : public Response {
    private:
        uint64_t uuid_7254d987_ac9c_3d73_831c_f43efb3268a9;

    public:
        center_reg_server_mq_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("center_rsp_cb", _ch)
        {
            uuid_7254d987_ac9c_3d73_831c_f43efb3268a9 = _uuid;
        }

        void rsp(){
            msgpack11::MsgPack::array _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2;
            _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2.push_back(uuid_7254d987_ac9c_3d73_831c_f43efb3268a9);
            call_module_method("center_rsp_cb_reg_server_mq_rsp", _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2);
        }

        void err(){
            msgpack11::MsgPack::array _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2;
            _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2.push_back(uuid_7254d987_ac9c_3d73_831c_f43efb3268a9);
            call_module_method("center_rsp_cb_reg_server_mq_err", _argv_08d68bf2_5282_3fde_ba14_da677a0a04b2);
        }

    };

    class center_reconn_reg_server_mq_rsp : public Response {
    private:
        uint64_t uuid_4d058274_a122_382e_8084_b9067ed713c5;

    public:
        center_reconn_reg_server_mq_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("center_rsp_cb", _ch)
        {
            uuid_4d058274_a122_382e_8084_b9067ed713c5 = _uuid;
        }

        void rsp(){
            msgpack11::MsgPack::array _argv_a018be20_2048_315d_9832_8120b194980f;
            _argv_a018be20_2048_315d_9832_8120b194980f.push_back(uuid_4d058274_a122_382e_8084_b9067ed713c5);
            call_module_method("center_rsp_cb_reconn_reg_server_mq_rsp", _argv_a018be20_2048_315d_9832_8120b194980f);
        }

        void err(){
            msgpack11::MsgPack::array _argv_a018be20_2048_315d_9832_8120b194980f;
            _argv_a018be20_2048_315d_9832_8120b194980f.push_back(uuid_4d058274_a122_382e_8084_b9067ed713c5);
            call_module_method("center_rsp_cb_reconn_reg_server_mq_err", _argv_a018be20_2048_315d_9832_8120b194980f);
        }

    };

    class center_heartbeat_rsp : public Response {
    private:
        uint64_t uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f;

    public:
        center_heartbeat_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("center_rsp_cb", _ch)
        {
            uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f = _uuid;
        }

        void rsp(){
            msgpack11::MsgPack::array _argv_af04a217_eafb_393c_9e34_0303485bef77;
            _argv_af04a217_eafb_393c_9e34_0303485bef77.push_back(uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f);
            call_module_method("center_rsp_cb_heartbeat_rsp", _argv_af04a217_eafb_393c_9e34_0303485bef77);
        }

        void err(){
            msgpack11::MsgPack::array _argv_af04a217_eafb_393c_9e34_0303485bef77;
            _argv_af04a217_eafb_393c_9e34_0303485bef77.push_back(uuid_617b63d0_e6d6_3c80_8c13_63a98d39e89f);
            call_module_method("center_rsp_cb_heartbeat_err", _argv_af04a217_eafb_393c_9e34_0303485bef77);
        }

    };

    class center_module : public Imodule, public std::enable_shared_from_this<center_module>{
    public:
        center_module() : Imodule("center")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_method("center_reg_server_mq", std::make_tuple(shared_from_this(), std::bind(&center_module::reg_server_mq, this, std::placeholders::_1)));
            _modules->reg_method("center_reconn_reg_server_mq", std::make_tuple(shared_from_this(), std::bind(&center_module::reconn_reg_server_mq, this, std::placeholders::_1)));
            _modules->reg_method("center_heartbeat", std::make_tuple(shared_from_this(), std::bind(&center_module::heartbeat, this, std::placeholders::_1)));
            _modules->reg_method("center_closed", std::make_tuple(shared_from_this(), std::bind(&center_module::closed, this, std::placeholders::_1)));
        }

        concurrent::signals<void(std::string, std::string)> sig_reg_server_mq;
        void reg_server_mq(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            auto _type = inArray[1].string_value();
            auto _svr_name = inArray[2].string_value();
            rsp = std::make_shared<center_reg_server_mq_rsp>(current_ch, _cb_uuid);
            sig_reg_server_mq.emit(_type, _svr_name);
            rsp = nullptr;
        }

        concurrent::signals<void(std::string, std::string)> sig_reconn_reg_server_mq;
        void reconn_reg_server_mq(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            auto _type = inArray[1].string_value();
            auto _svr_name = inArray[2].string_value();
            rsp = std::make_shared<center_reconn_reg_server_mq_rsp>(current_ch, _cb_uuid);
            sig_reconn_reg_server_mq.emit(_type, _svr_name);
            rsp = nullptr;
        }

        concurrent::signals<void(uint32_t)> sig_heartbeat;
        void heartbeat(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            auto _tick = inArray[1].uint32_value();
            rsp = std::make_shared<center_heartbeat_rsp>(current_ch, _cb_uuid);
            sig_heartbeat.emit(_tick);
            rsp = nullptr;
        }

        concurrent::signals<void()> sig_closed;
        void closed(const msgpack11::MsgPack::array& inArray){
            sig_closed.emit();
        }

    };
    class center_call_server_module : public Imodule, public std::enable_shared_from_this<center_call_server_module>{
    public:
        center_call_server_module() : Imodule("center_call_server")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_method("center_call_server_close_server", std::make_tuple(shared_from_this(), std::bind(&center_call_server_module::close_server, this, std::placeholders::_1)));
            _modules->reg_method("center_call_server_console_close_server", std::make_tuple(shared_from_this(), std::bind(&center_call_server_module::console_close_server, this, std::placeholders::_1)));
            _modules->reg_method("center_call_server_svr_be_closed", std::make_tuple(shared_from_this(), std::bind(&center_call_server_module::svr_be_closed, this, std::placeholders::_1)));
        }

        concurrent::signals<void()> sig_close_server;
        void close_server(const msgpack11::MsgPack::array& inArray){
            sig_close_server.emit();
        }

        concurrent::signals<void(std::string, std::string)> sig_console_close_server;
        void console_close_server(const msgpack11::MsgPack::array& inArray){
            auto _svr_type = inArray[0].string_value();
            auto _svr_name = inArray[1].string_value();
            sig_console_close_server.emit(_svr_type, _svr_name);
        }

        concurrent::signals<void(std::string, std::string)> sig_svr_be_closed;
        void svr_be_closed(const msgpack11::MsgPack::array& inArray){
            auto _svr_type = inArray[0].string_value();
            auto _svr_name = inArray[1].string_value();
            sig_svr_be_closed.emit(_svr_type, _svr_name);
        }

    };
    class center_call_hub_module : public Imodule, public std::enable_shared_from_this<center_call_hub_module>{
    public:
        center_call_hub_module() : Imodule("center_call_hub")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_method("center_call_hub_distribute_server_mq", std::make_tuple(shared_from_this(), std::bind(&center_call_hub_module::distribute_server_mq, this, std::placeholders::_1)));
            _modules->reg_method("center_call_hub_reload", std::make_tuple(shared_from_this(), std::bind(&center_call_hub_module::reload, this, std::placeholders::_1)));
        }

        concurrent::signals<void(std::string, std::string)> sig_distribute_server_mq;
        void distribute_server_mq(const msgpack11::MsgPack::array& inArray){
            auto _svr_type = inArray[0].string_value();
            auto _svr_name = inArray[1].string_value();
            sig_distribute_server_mq.emit(_svr_type, _svr_name);
        }

        concurrent::signals<void(std::string)> sig_reload;
        void reload(const msgpack11::MsgPack::array& inArray){
            auto _argv = inArray[0].string_value();
            sig_reload.emit(_argv);
        }

    };
    class gm_center_module : public Imodule, public std::enable_shared_from_this<gm_center_module>{
    public:
        gm_center_module() : Imodule("gm_center")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_method("gm_center_confirm_gm", std::make_tuple(shared_from_this(), std::bind(&gm_center_module::confirm_gm, this, std::placeholders::_1)));
            _modules->reg_method("gm_center_close_clutter", std::make_tuple(shared_from_this(), std::bind(&gm_center_module::close_clutter, this, std::placeholders::_1)));
            _modules->reg_method("gm_center_reload", std::make_tuple(shared_from_this(), std::bind(&gm_center_module::reload, this, std::placeholders::_1)));
        }

        concurrent::signals<void(std::string)> sig_confirm_gm;
        void confirm_gm(const msgpack11::MsgPack::array& inArray){
            auto _gm_name = inArray[0].string_value();
            sig_confirm_gm.emit(_gm_name);
        }

        concurrent::signals<void(std::string)> sig_close_clutter;
        void close_clutter(const msgpack11::MsgPack::array& inArray){
            auto _gmname = inArray[0].string_value();
            sig_close_clutter.emit(_gmname);
        }

        concurrent::signals<void(std::string, std::string)> sig_reload;
        void reload(const msgpack11::MsgPack::array& inArray){
            auto _gmname = inArray[0].string_value();
            auto _argv = inArray[1].string_value();
            sig_reload.emit(_gmname, _argv);
        }

    };

}

#endif //_h_center_fd1a4f35_9b23_3f22_8094_3acc5aecb066_
