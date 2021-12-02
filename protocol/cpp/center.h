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
    class center_reg_server_cb : public std::enable_shared_from_this<center_reg_server_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<center_rsp_cb> module_rsp_cb;

    public:
        center_reg_server_cb(uint64_t _cb_uuid, std::shared_ptr<center_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void()> sig_reg_server_cb;
        concurrent::signals<void()> sig_reg_server_err;
        concurrent::signals<void()> sig_reg_server_timeout;

        std::shared_ptr<center_reg_server_cb> callBack(std::function<void()> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

/*this cb code is codegen by abelkhan for cpp*/
    class center_rsp_cb : public Imodule, public std::enable_shared_from_this<center_rsp_cb>{
    public:
        std::mutex mutex_map_reg_server;
        std::map<uint64_t, std::shared_ptr<center_reg_server_cb> > map_reg_server;
        center_rsp_cb() : Imodule("center_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("reg_server_rsp", std::bind(&center_rsp_cb::reg_server_rsp, this, std::placeholders::_1));
            reg_method("reg_server_err", std::bind(&center_rsp_cb::reg_server_err, this, std::placeholders::_1));
        }

        void reg_server_rsp(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_reg_server_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_reg_server_cb.emit();
            }
        }

        void reg_server_err(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_reg_server_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_reg_server_err.emit();
            }
        }

        void reg_server_timeout(uint64_t cb_uuid){
            auto rsp = try_get_and_del_reg_server_cb(cb_uuid);
            if (rsp != nullptr){
                rsp->sig_reg_server_timeout.emit();
            }
        }

        std::shared_ptr<center_reg_server_cb> try_get_and_del_reg_server_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_reg_server);
            if (map_reg_server.find(uuid) != map_reg_server.end()) {
                auto rsp = map_reg_server[uuid];
                map_reg_server.erase(uuid);
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

        std::shared_ptr<center_reg_server_cb> reg_server(std::string type, std::string ip, uint16_t port, std::string svr_name){
            auto uuid_211efc4c_e5e2_5ec9_b83c_2b2434aa8255 = uuid_fd1a4f35_9b23_3f22_8094_3acc5aecb066++;
            msgpack11::MsgPack::array _argv_e599dafa_7492_34c4_8e5a_7a0f00557fda;
            _argv_e599dafa_7492_34c4_8e5a_7a0f00557fda.push_back(uuid_211efc4c_e5e2_5ec9_b83c_2b2434aa8255);
            _argv_e599dafa_7492_34c4_8e5a_7a0f00557fda.push_back(type);
            _argv_e599dafa_7492_34c4_8e5a_7a0f00557fda.push_back(ip);
            _argv_e599dafa_7492_34c4_8e5a_7a0f00557fda.push_back(port);
            _argv_e599dafa_7492_34c4_8e5a_7a0f00557fda.push_back(svr_name);
            call_module_method("reg_server", _argv_e599dafa_7492_34c4_8e5a_7a0f00557fda);

            auto cb_reg_server_obj = std::make_shared<center_reg_server_cb>(uuid_211efc4c_e5e2_5ec9_b83c_2b2434aa8255, rsp_cb_center_handle);
            std::lock_guard<std::mutex> l(rsp_cb_center_handle->mutex_map_reg_server);
            rsp_cb_center_handle->map_reg_server.insert(std::make_pair(uuid_211efc4c_e5e2_5ec9_b83c_2b2434aa8255, cb_reg_server_obj));
            return cb_reg_server_obj;
        }

        void heartbeat(){
            msgpack11::MsgPack::array _argv_af04a217_eafb_393c_9e34_0303485bef77;
            call_module_method("heartbeat", _argv_af04a217_eafb_393c_9e34_0303485bef77);
        }

        void closed(){
            msgpack11::MsgPack::array _argv_e0d5aabb_d671_3c7e_99e4_6d374f7fd4ee;
            call_module_method("closed", _argv_e0d5aabb_d671_3c7e_99e4_6d374f7fd4ee);
        }

    };
/*this cb code is codegen by abelkhan for cpp*/
    class center_call_server_rsp_cb : public Imodule, public std::enable_shared_from_this<center_call_server_rsp_cb>{
    public:
        center_call_server_rsp_cb() : Imodule("center_call_server_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

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
            call_module_method("close_server", _argv_8394af17_8a06_3068_977d_477a1276f56e);
        }

        void svr_be_closed(std::string svr_type, std::string svr_name){
            msgpack11::MsgPack::array _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac;
            _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac.push_back(svr_type);
            _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac.push_back(svr_name);
            call_module_method("svr_be_closed", _argv_660fcd53_cd77_3915_a5d5_06e86302e8ac);
        }

    };
/*this cb code is codegen by abelkhan for cpp*/
    class center_call_hub_rsp_cb : public Imodule, public std::enable_shared_from_this<center_call_hub_rsp_cb>{
    public:
        center_call_hub_rsp_cb() : Imodule("center_call_hub_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

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

        void distribute_server_address(std::string svr_type, std::string svr_name, std::string ip, uint16_t port){
            msgpack11::MsgPack::array _argv_b71bf35c_d65b_3682_98d1_b934f5276558;
            _argv_b71bf35c_d65b_3682_98d1_b934f5276558.push_back(svr_type);
            _argv_b71bf35c_d65b_3682_98d1_b934f5276558.push_back(svr_name);
            _argv_b71bf35c_d65b_3682_98d1_b934f5276558.push_back(ip);
            _argv_b71bf35c_d65b_3682_98d1_b934f5276558.push_back(port);
            call_module_method("distribute_server_address", _argv_b71bf35c_d65b_3682_98d1_b934f5276558);
        }

        void reload(std::string argv){
            msgpack11::MsgPack::array _argv_ba37af53_beea_3d61_82e1_8d15e335971d;
            _argv_ba37af53_beea_3d61_82e1_8d15e335971d.push_back(argv);
            call_module_method("reload", _argv_ba37af53_beea_3d61_82e1_8d15e335971d);
        }

    };
/*this cb code is codegen by abelkhan for cpp*/
    class gm_center_rsp_cb : public Imodule, public std::enable_shared_from_this<gm_center_rsp_cb>{
    public:
        gm_center_rsp_cb() : Imodule("gm_center_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

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
            call_module_method("confirm_gm", _argv_d097689c_b711_3837_8783_64916ae34cea);
        }

        void close_clutter(std::string gmname){
            msgpack11::MsgPack::array _argv_576c03c3_06da_36a2_b868_752da601cb54;
            _argv_576c03c3_06da_36a2_b868_752da601cb54.push_back(gmname);
            call_module_method("close_clutter", _argv_576c03c3_06da_36a2_b868_752da601cb54);
        }

        void reload(std::string gmname, std::string argv){
            msgpack11::MsgPack::array _argv_ba37af53_beea_3d61_82e1_8d15e335971d;
            _argv_ba37af53_beea_3d61_82e1_8d15e335971d.push_back(gmname);
            _argv_ba37af53_beea_3d61_82e1_8d15e335971d.push_back(argv);
            call_module_method("reload", _argv_ba37af53_beea_3d61_82e1_8d15e335971d);
        }

    };
/*this module code is codegen by abelkhan codegen for cpp*/
    class center_reg_server_rsp : public Response {
    private:
        uint64_t uuid_e599dafa_7492_34c4_8e5a_7a0f00557fda;

    public:
        center_reg_server_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("center_rsp_cb", _ch)
        {
            uuid_e599dafa_7492_34c4_8e5a_7a0f00557fda = _uuid;
        }

        void rsp(){
            msgpack11::MsgPack::array _argv_86ab8166_c1a7_3809_8c9b_df444f746076;
            _argv_86ab8166_c1a7_3809_8c9b_df444f746076.push_back(uuid_e599dafa_7492_34c4_8e5a_7a0f00557fda);
            call_module_method("reg_server_rsp", _argv_86ab8166_c1a7_3809_8c9b_df444f746076);
        }

        void err(){
            msgpack11::MsgPack::array _argv_86ab8166_c1a7_3809_8c9b_df444f746076;
            _argv_86ab8166_c1a7_3809_8c9b_df444f746076.push_back(uuid_e599dafa_7492_34c4_8e5a_7a0f00557fda);
            call_module_method("reg_server_err", _argv_86ab8166_c1a7_3809_8c9b_df444f746076);
        }

    };

    class center_module : public Imodule, public std::enable_shared_from_this<center_module>{
    public:
        center_module() : Imodule("center")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("reg_server", std::bind(&center_module::reg_server, this, std::placeholders::_1));
            reg_method("heartbeat", std::bind(&center_module::heartbeat, this, std::placeholders::_1));
            reg_method("closed", std::bind(&center_module::closed, this, std::placeholders::_1));
        }

        concurrent::signals<void(std::string, std::string, uint16_t, std::string)> sig_reg_server;
        void reg_server(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            auto _type = inArray[1].string_value();
            auto _ip = inArray[2].string_value();
            auto _port = inArray[3].uint16_value();
            auto _svr_name = inArray[4].string_value();
            rsp = std::make_shared<center_reg_server_rsp>(current_ch, _cb_uuid);
            sig_reg_server.emit(_type, _ip, _port, _svr_name);
            rsp = nullptr;
        }

        concurrent::signals<void()> sig_heartbeat;
        void heartbeat(const msgpack11::MsgPack::array& inArray){
            sig_heartbeat.emit();
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
            _modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("close_server", std::bind(&center_call_server_module::close_server, this, std::placeholders::_1));
            reg_method("svr_be_closed", std::bind(&center_call_server_module::svr_be_closed, this, std::placeholders::_1));
        }

        concurrent::signals<void()> sig_close_server;
        void close_server(const msgpack11::MsgPack::array& inArray){
            sig_close_server.emit();
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
            _modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("distribute_server_address", std::bind(&center_call_hub_module::distribute_server_address, this, std::placeholders::_1));
            reg_method("reload", std::bind(&center_call_hub_module::reload, this, std::placeholders::_1));
        }

        concurrent::signals<void(std::string, std::string, std::string, uint16_t)> sig_distribute_server_address;
        void distribute_server_address(const msgpack11::MsgPack::array& inArray){
            auto _svr_type = inArray[0].string_value();
            auto _svr_name = inArray[1].string_value();
            auto _ip = inArray[2].string_value();
            auto _port = inArray[3].uint16_value();
            sig_distribute_server_address.emit(_svr_type, _svr_name, _ip, _port);
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
            _modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("confirm_gm", std::bind(&gm_center_module::confirm_gm, this, std::placeholders::_1));
            reg_method("close_clutter", std::bind(&gm_center_module::close_clutter, this, std::placeholders::_1));
            reg_method("reload", std::bind(&gm_center_module::reload, this, std::placeholders::_1));
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
