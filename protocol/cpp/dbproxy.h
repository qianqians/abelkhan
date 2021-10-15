#ifndef _h_dbproxy_ec2ff100_3643_3dba_9c7d_f391c20b497e_
#define _h_dbproxy_ec2ff100_3643_3dba_9c7d_f391c20b497e_

#include <abelkhan.h>
#include <signals.h>

namespace abelkhan
{
/*this enum code is codegen by abelkhan codegen for cpp*/

/*this struct code is codegen by abelkhan codegen for cpp*/
/*this caller code is codegen by abelkhan codegen for cpp*/
/*this cb code is codegen by abelkhan for cpp*/
    class dbproxy_call_hub_rsp_cb : public Imodule, public std::enable_shared_from_this<dbproxy_call_hub_rsp_cb>{
    public:
        dbproxy_call_hub_rsp_cb() : Imodule("dbproxy_call_hub_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

        }

    };

    class dbproxy_call_hub_caller : Icaller {
    private:
        static std::shared_ptr<dbproxy_call_hub_rsp_cb> rsp_cb_dbproxy_call_hub_handle;

    private:
        std::atomic<uint64_t> uuid_7a1d0ce9_a121_3019_b67a_319998ea37c8;

    public:
        dbproxy_call_hub_caller(std::shared_ptr<Ichannel> _ch, std::shared_ptr<modulemng> modules) : Icaller("dbproxy_call_hub", _ch)
        {
            if (rsp_cb_dbproxy_call_hub_handle == nullptr){
                rsp_cb_dbproxy_call_hub_handle = std::make_shared<dbproxy_call_hub_rsp_cb>();
                rsp_cb_dbproxy_call_hub_handle->Init(modules);
            }
            uuid_7a1d0ce9_a121_3019_b67a_319998ea37c8.store(random());
        }

        void ack_get_object_info(std::string callbackid, std::vector<uint8_t> object_info){
            msgpack11::MsgPack::array _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7;
            _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7.push_back(callbackid);
            _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7.push_back(object_info);
            call_module_method("ack_get_object_info", _argv_4b9aab45_a48a_36d2_a0cb_00e4d4c3a7c7);
        }

        void ack_get_object_info_end(std::string callbackid){
            msgpack11::MsgPack::array _argv_e4756ccf_94e2_3b4f_958a_701f7076e607;
            _argv_e4756ccf_94e2_3b4f_958a_701f7076e607.push_back(callbackid);
            call_module_method("ack_get_object_info_end", _argv_e4756ccf_94e2_3b4f_958a_701f7076e607);
        }

    };
    class hub_call_dbproxy_rsp_cb;
    class hub_call_dbproxy_reg_hub_cb : public std::enable_shared_from_this<hub_call_dbproxy_reg_hub_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<hub_call_dbproxy_rsp_cb> module_rsp_cb;

    public:
        hub_call_dbproxy_reg_hub_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_dbproxy_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void()> sig_reg_hub_cb;
        concurrent::signals<void()> sig_reg_hub_err;
        concurrent::signals<void()> sig_reg_hub_timeout;

        std::shared_ptr<hub_call_dbproxy_reg_hub_cb> callBack(std::function<void()> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

    class hub_call_dbproxy_rsp_cb;
    class hub_call_dbproxy_create_persisted_object_cb : public std::enable_shared_from_this<hub_call_dbproxy_create_persisted_object_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<hub_call_dbproxy_rsp_cb> module_rsp_cb;

    public:
        hub_call_dbproxy_create_persisted_object_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_dbproxy_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void()> sig_create_persisted_object_cb;
        concurrent::signals<void()> sig_create_persisted_object_err;
        concurrent::signals<void()> sig_create_persisted_object_timeout;

        std::shared_ptr<hub_call_dbproxy_create_persisted_object_cb> callBack(std::function<void()> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

    class hub_call_dbproxy_rsp_cb;
    class hub_call_dbproxy_updata_persisted_object_cb : public std::enable_shared_from_this<hub_call_dbproxy_updata_persisted_object_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<hub_call_dbproxy_rsp_cb> module_rsp_cb;

    public:
        hub_call_dbproxy_updata_persisted_object_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_dbproxy_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void()> sig_updata_persisted_object_cb;
        concurrent::signals<void()> sig_updata_persisted_object_err;
        concurrent::signals<void()> sig_updata_persisted_object_timeout;

        std::shared_ptr<hub_call_dbproxy_updata_persisted_object_cb> callBack(std::function<void()> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

    class hub_call_dbproxy_rsp_cb;
    class hub_call_dbproxy_find_and_modify_cb : public std::enable_shared_from_this<hub_call_dbproxy_find_and_modify_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<hub_call_dbproxy_rsp_cb> module_rsp_cb;

    public:
        hub_call_dbproxy_find_and_modify_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_dbproxy_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void(std::vector<uint8_t>)> sig_find_and_modify_cb;
        concurrent::signals<void()> sig_find_and_modify_err;
        concurrent::signals<void()> sig_find_and_modify_timeout;

        std::shared_ptr<hub_call_dbproxy_find_and_modify_cb> callBack(std::function<void(std::vector<uint8_t> object_info)> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

    class hub_call_dbproxy_rsp_cb;
    class hub_call_dbproxy_remove_object_cb : public std::enable_shared_from_this<hub_call_dbproxy_remove_object_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<hub_call_dbproxy_rsp_cb> module_rsp_cb;

    public:
        hub_call_dbproxy_remove_object_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_dbproxy_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void()> sig_remove_object_cb;
        concurrent::signals<void()> sig_remove_object_err;
        concurrent::signals<void()> sig_remove_object_timeout;

        std::shared_ptr<hub_call_dbproxy_remove_object_cb> callBack(std::function<void()> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

    class hub_call_dbproxy_rsp_cb;
    class hub_call_dbproxy_get_object_count_cb : public std::enable_shared_from_this<hub_call_dbproxy_get_object_count_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<hub_call_dbproxy_rsp_cb> module_rsp_cb;

    public:
        hub_call_dbproxy_get_object_count_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_dbproxy_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void(uint32_t)> sig_get_object_count_cb;
        concurrent::signals<void()> sig_get_object_count_err;
        concurrent::signals<void()> sig_get_object_count_timeout;

        std::shared_ptr<hub_call_dbproxy_get_object_count_cb> callBack(std::function<void(uint32_t count)> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

/*this cb code is codegen by abelkhan for cpp*/
    class hub_call_dbproxy_rsp_cb : public Imodule, public std::enable_shared_from_this<hub_call_dbproxy_rsp_cb>{
    public:
        std::mutex mutex_map_reg_hub;
        std::map<uint64_t, std::shared_ptr<hub_call_dbproxy_reg_hub_cb> > map_reg_hub;
        std::mutex mutex_map_create_persisted_object;
        std::map<uint64_t, std::shared_ptr<hub_call_dbproxy_create_persisted_object_cb> > map_create_persisted_object;
        std::mutex mutex_map_updata_persisted_object;
        std::map<uint64_t, std::shared_ptr<hub_call_dbproxy_updata_persisted_object_cb> > map_updata_persisted_object;
        std::mutex mutex_map_find_and_modify;
        std::map<uint64_t, std::shared_ptr<hub_call_dbproxy_find_and_modify_cb> > map_find_and_modify;
        std::mutex mutex_map_remove_object;
        std::map<uint64_t, std::shared_ptr<hub_call_dbproxy_remove_object_cb> > map_remove_object;
        std::mutex mutex_map_get_object_count;
        std::map<uint64_t, std::shared_ptr<hub_call_dbproxy_get_object_count_cb> > map_get_object_count;
        hub_call_dbproxy_rsp_cb() : Imodule("hub_call_dbproxy_rsp_cb")
        {
        }

        void Init(std::shared_ptr<modulemng> modules){
            modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("reg_hub_rsp", std::bind(&hub_call_dbproxy_rsp_cb::reg_hub_rsp, this, std::placeholders::_1));
            reg_method("reg_hub_err", std::bind(&hub_call_dbproxy_rsp_cb::reg_hub_err, this, std::placeholders::_1));
            reg_method("create_persisted_object_rsp", std::bind(&hub_call_dbproxy_rsp_cb::create_persisted_object_rsp, this, std::placeholders::_1));
            reg_method("create_persisted_object_err", std::bind(&hub_call_dbproxy_rsp_cb::create_persisted_object_err, this, std::placeholders::_1));
            reg_method("updata_persisted_object_rsp", std::bind(&hub_call_dbproxy_rsp_cb::updata_persisted_object_rsp, this, std::placeholders::_1));
            reg_method("updata_persisted_object_err", std::bind(&hub_call_dbproxy_rsp_cb::updata_persisted_object_err, this, std::placeholders::_1));
            reg_method("find_and_modify_rsp", std::bind(&hub_call_dbproxy_rsp_cb::find_and_modify_rsp, this, std::placeholders::_1));
            reg_method("find_and_modify_err", std::bind(&hub_call_dbproxy_rsp_cb::find_and_modify_err, this, std::placeholders::_1));
            reg_method("remove_object_rsp", std::bind(&hub_call_dbproxy_rsp_cb::remove_object_rsp, this, std::placeholders::_1));
            reg_method("remove_object_err", std::bind(&hub_call_dbproxy_rsp_cb::remove_object_err, this, std::placeholders::_1));
            reg_method("get_object_count_rsp", std::bind(&hub_call_dbproxy_rsp_cb::get_object_count_rsp, this, std::placeholders::_1));
            reg_method("get_object_count_err", std::bind(&hub_call_dbproxy_rsp_cb::get_object_count_err, this, std::placeholders::_1));
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

        std::shared_ptr<hub_call_dbproxy_reg_hub_cb> try_get_and_del_reg_hub_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_reg_hub);
            auto rsp = map_reg_hub[uuid];
            map_reg_hub.erase(uuid);
            return rsp;
        }

        void create_persisted_object_rsp(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_create_persisted_object_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_create_persisted_object_cb.emit();
            }
        }

        void create_persisted_object_err(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_create_persisted_object_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_create_persisted_object_err.emit();
            }
        }

        void create_persisted_object_timeout(uint64_t cb_uuid){
            auto rsp = try_get_and_del_create_persisted_object_cb(cb_uuid);
            if (rsp != nullptr){
                rsp->sig_create_persisted_object_timeout.emit();
            }
        }

        std::shared_ptr<hub_call_dbproxy_create_persisted_object_cb> try_get_and_del_create_persisted_object_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_create_persisted_object);
            auto rsp = map_create_persisted_object[uuid];
            map_create_persisted_object.erase(uuid);
            return rsp;
        }

        void updata_persisted_object_rsp(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_updata_persisted_object_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_updata_persisted_object_cb.emit();
            }
        }

        void updata_persisted_object_err(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_updata_persisted_object_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_updata_persisted_object_err.emit();
            }
        }

        void updata_persisted_object_timeout(uint64_t cb_uuid){
            auto rsp = try_get_and_del_updata_persisted_object_cb(cb_uuid);
            if (rsp != nullptr){
                rsp->sig_updata_persisted_object_timeout.emit();
            }
        }

        std::shared_ptr<hub_call_dbproxy_updata_persisted_object_cb> try_get_and_del_updata_persisted_object_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_updata_persisted_object);
            auto rsp = map_updata_persisted_object[uuid];
            map_updata_persisted_object.erase(uuid);
            return rsp;
        }

        void find_and_modify_rsp(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto _object_info = inArray[1].binary_items();
            auto rsp = try_get_and_del_find_and_modify_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_find_and_modify_cb.emit(_object_info);
            }
        }

        void find_and_modify_err(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_find_and_modify_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_find_and_modify_err.emit();
            }
        }

        void find_and_modify_timeout(uint64_t cb_uuid){
            auto rsp = try_get_and_del_find_and_modify_cb(cb_uuid);
            if (rsp != nullptr){
                rsp->sig_find_and_modify_timeout.emit();
            }
        }

        std::shared_ptr<hub_call_dbproxy_find_and_modify_cb> try_get_and_del_find_and_modify_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_find_and_modify);
            auto rsp = map_find_and_modify[uuid];
            map_find_and_modify.erase(uuid);
            return rsp;
        }

        void remove_object_rsp(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_remove_object_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_remove_object_cb.emit();
            }
        }

        void remove_object_err(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_remove_object_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_remove_object_err.emit();
            }
        }

        void remove_object_timeout(uint64_t cb_uuid){
            auto rsp = try_get_and_del_remove_object_cb(cb_uuid);
            if (rsp != nullptr){
                rsp->sig_remove_object_timeout.emit();
            }
        }

        std::shared_ptr<hub_call_dbproxy_remove_object_cb> try_get_and_del_remove_object_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_remove_object);
            auto rsp = map_remove_object[uuid];
            map_remove_object.erase(uuid);
            return rsp;
        }

        void get_object_count_rsp(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto _count = inArray[1].uint32_value();
            auto rsp = try_get_and_del_get_object_count_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_get_object_count_cb.emit(_count);
            }
        }

        void get_object_count_err(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_get_object_count_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_get_object_count_err.emit();
            }
        }

        void get_object_count_timeout(uint64_t cb_uuid){
            auto rsp = try_get_and_del_get_object_count_cb(cb_uuid);
            if (rsp != nullptr){
                rsp->sig_get_object_count_timeout.emit();
            }
        }

        std::shared_ptr<hub_call_dbproxy_get_object_count_cb> try_get_and_del_get_object_count_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_get_object_count);
            auto rsp = map_get_object_count[uuid];
            map_get_object_count.erase(uuid);
            return rsp;
        }

    };

    class hub_call_dbproxy_caller : Icaller {
    private:
        static std::shared_ptr<hub_call_dbproxy_rsp_cb> rsp_cb_hub_call_dbproxy_handle;

    private:
        std::atomic<uint64_t> uuid_e713438c_e791_3714_ad31_4ccbddee2554;

    public:
        hub_call_dbproxy_caller(std::shared_ptr<Ichannel> _ch, std::shared_ptr<modulemng> modules) : Icaller("hub_call_dbproxy", _ch)
        {
            if (rsp_cb_hub_call_dbproxy_handle == nullptr){
                rsp_cb_hub_call_dbproxy_handle = std::make_shared<hub_call_dbproxy_rsp_cb>();
                rsp_cb_hub_call_dbproxy_handle->Init(modules);
            }
            uuid_e713438c_e791_3714_ad31_4ccbddee2554.store(random());
        }

        std::shared_ptr<hub_call_dbproxy_reg_hub_cb> reg_hub(std::string hub_name){
            auto uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = uuid_e713438c_e791_3714_ad31_4ccbddee2554++;
            msgpack11::MsgPack::array _argv_d47a6c8a_5494_35bb_9bc5_60d20f624f67;
            _argv_d47a6c8a_5494_35bb_9bc5_60d20f624f67.push_back(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106);
            _argv_d47a6c8a_5494_35bb_9bc5_60d20f624f67.push_back(hub_name);
            call_module_method("reg_hub", _argv_d47a6c8a_5494_35bb_9bc5_60d20f624f67);

            auto cb_reg_hub_obj = std::make_shared<hub_call_dbproxy_reg_hub_cb>(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, rsp_cb_hub_call_dbproxy_handle);
            std::lock_guard<std::mutex> l(rsp_cb_hub_call_dbproxy_handle->mutex_map_reg_hub);
            rsp_cb_hub_call_dbproxy_handle->map_reg_hub.insert(std::make_pair(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, cb_reg_hub_obj));
            return cb_reg_hub_obj;
        }

        std::shared_ptr<hub_call_dbproxy_create_persisted_object_cb> create_persisted_object(std::string db, std::string collection, std::vector<uint8_t> object_info){
            auto uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb = uuid_e713438c_e791_3714_ad31_4ccbddee2554++;
            msgpack11::MsgPack::array _argv_c5ae7137_dfe0_316b_9f1d_5dffa222d32b;
            _argv_c5ae7137_dfe0_316b_9f1d_5dffa222d32b.push_back(uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb);
            _argv_c5ae7137_dfe0_316b_9f1d_5dffa222d32b.push_back(db);
            _argv_c5ae7137_dfe0_316b_9f1d_5dffa222d32b.push_back(collection);
            _argv_c5ae7137_dfe0_316b_9f1d_5dffa222d32b.push_back(object_info);
            call_module_method("create_persisted_object", _argv_c5ae7137_dfe0_316b_9f1d_5dffa222d32b);

            auto cb_create_persisted_object_obj = std::make_shared<hub_call_dbproxy_create_persisted_object_cb>(uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb, rsp_cb_hub_call_dbproxy_handle);
            std::lock_guard<std::mutex> l(rsp_cb_hub_call_dbproxy_handle->mutex_map_create_persisted_object);
            rsp_cb_hub_call_dbproxy_handle->map_create_persisted_object.insert(std::make_pair(uuid_91387a79_b9d1_5601_bac5_4fc46430f5fb, cb_create_persisted_object_obj));
            return cb_create_persisted_object_obj;
        }

        std::shared_ptr<hub_call_dbproxy_updata_persisted_object_cb> updata_persisted_object(std::string db, std::string collection, std::vector<uint8_t> query_info, std::vector<uint8_t> updata_info, bool _upsert){
            auto uuid_7864a402_2d75_5c02_b24b_50287a06732f = uuid_e713438c_e791_3714_ad31_4ccbddee2554++;
            msgpack11::MsgPack::array _argv_16267d40_cddc_312f_87c0_185a55b79ad2;
            _argv_16267d40_cddc_312f_87c0_185a55b79ad2.push_back(uuid_7864a402_2d75_5c02_b24b_50287a06732f);
            _argv_16267d40_cddc_312f_87c0_185a55b79ad2.push_back(db);
            _argv_16267d40_cddc_312f_87c0_185a55b79ad2.push_back(collection);
            _argv_16267d40_cddc_312f_87c0_185a55b79ad2.push_back(query_info);
            _argv_16267d40_cddc_312f_87c0_185a55b79ad2.push_back(updata_info);
            _argv_16267d40_cddc_312f_87c0_185a55b79ad2.push_back(_upsert);
            call_module_method("updata_persisted_object", _argv_16267d40_cddc_312f_87c0_185a55b79ad2);

            auto cb_updata_persisted_object_obj = std::make_shared<hub_call_dbproxy_updata_persisted_object_cb>(uuid_7864a402_2d75_5c02_b24b_50287a06732f, rsp_cb_hub_call_dbproxy_handle);
            std::lock_guard<std::mutex> l(rsp_cb_hub_call_dbproxy_handle->mutex_map_updata_persisted_object);
            rsp_cb_hub_call_dbproxy_handle->map_updata_persisted_object.insert(std::make_pair(uuid_7864a402_2d75_5c02_b24b_50287a06732f, cb_updata_persisted_object_obj));
            return cb_updata_persisted_object_obj;
        }

        std::shared_ptr<hub_call_dbproxy_find_and_modify_cb> find_and_modify(std::string db, std::string collection, std::vector<uint8_t> query_info, std::vector<uint8_t> updata_info, bool _new, bool _upsert){
            auto uuid_e70b09ff_6d2a_5ea6_b2ff_99643df60f2a = uuid_e713438c_e791_3714_ad31_4ccbddee2554++;
            msgpack11::MsgPack::array _argv_c7725286_bd2c_331b_8ba9_90ffcefab6ae;
            _argv_c7725286_bd2c_331b_8ba9_90ffcefab6ae.push_back(uuid_e70b09ff_6d2a_5ea6_b2ff_99643df60f2a);
            _argv_c7725286_bd2c_331b_8ba9_90ffcefab6ae.push_back(db);
            _argv_c7725286_bd2c_331b_8ba9_90ffcefab6ae.push_back(collection);
            _argv_c7725286_bd2c_331b_8ba9_90ffcefab6ae.push_back(query_info);
            _argv_c7725286_bd2c_331b_8ba9_90ffcefab6ae.push_back(updata_info);
            _argv_c7725286_bd2c_331b_8ba9_90ffcefab6ae.push_back(_new);
            _argv_c7725286_bd2c_331b_8ba9_90ffcefab6ae.push_back(_upsert);
            call_module_method("find_and_modify", _argv_c7725286_bd2c_331b_8ba9_90ffcefab6ae);

            auto cb_find_and_modify_obj = std::make_shared<hub_call_dbproxy_find_and_modify_cb>(uuid_e70b09ff_6d2a_5ea6_b2ff_99643df60f2a, rsp_cb_hub_call_dbproxy_handle);
            std::lock_guard<std::mutex> l(rsp_cb_hub_call_dbproxy_handle->mutex_map_find_and_modify);
            rsp_cb_hub_call_dbproxy_handle->map_find_and_modify.insert(std::make_pair(uuid_e70b09ff_6d2a_5ea6_b2ff_99643df60f2a, cb_find_and_modify_obj));
            return cb_find_and_modify_obj;
        }

        std::shared_ptr<hub_call_dbproxy_remove_object_cb> remove_object(std::string db, std::string collection, std::vector<uint8_t> query_info){
            auto uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f = uuid_e713438c_e791_3714_ad31_4ccbddee2554++;
            msgpack11::MsgPack::array _argv_f3bda2d9_d71c_307f_b727_d893a1cc0cd1;
            _argv_f3bda2d9_d71c_307f_b727_d893a1cc0cd1.push_back(uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f);
            _argv_f3bda2d9_d71c_307f_b727_d893a1cc0cd1.push_back(db);
            _argv_f3bda2d9_d71c_307f_b727_d893a1cc0cd1.push_back(collection);
            _argv_f3bda2d9_d71c_307f_b727_d893a1cc0cd1.push_back(query_info);
            call_module_method("remove_object", _argv_f3bda2d9_d71c_307f_b727_d893a1cc0cd1);

            auto cb_remove_object_obj = std::make_shared<hub_call_dbproxy_remove_object_cb>(uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f, rsp_cb_hub_call_dbproxy_handle);
            std::lock_guard<std::mutex> l(rsp_cb_hub_call_dbproxy_handle->mutex_map_remove_object);
            rsp_cb_hub_call_dbproxy_handle->map_remove_object.insert(std::make_pair(uuid_713503ae_bbb7_5af6_8c82_f1a61f71040f, cb_remove_object_obj));
            return cb_remove_object_obj;
        }

        void get_object_info(std::string db, std::string collection, std::vector<uint8_t> query_info, std::string callbackid){
            msgpack11::MsgPack::array _argv_1f17e6de_d423_391b_a599_7268e665a53f;
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.push_back(db);
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.push_back(collection);
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.push_back(query_info);
            _argv_1f17e6de_d423_391b_a599_7268e665a53f.push_back(callbackid);
            call_module_method("get_object_info", _argv_1f17e6de_d423_391b_a599_7268e665a53f);
        }

        std::shared_ptr<hub_call_dbproxy_get_object_count_cb> get_object_count(std::string db, std::string collection, std::vector<uint8_t> query_info){
            auto uuid_975425f5_8baf_5905_beeb_4454e78907f6 = uuid_e713438c_e791_3714_ad31_4ccbddee2554++;
            msgpack11::MsgPack::array _argv_175cd463_d9ac_3cde_804f_1c917ef2c7d2;
            _argv_175cd463_d9ac_3cde_804f_1c917ef2c7d2.push_back(uuid_975425f5_8baf_5905_beeb_4454e78907f6);
            _argv_175cd463_d9ac_3cde_804f_1c917ef2c7d2.push_back(db);
            _argv_175cd463_d9ac_3cde_804f_1c917ef2c7d2.push_back(collection);
            _argv_175cd463_d9ac_3cde_804f_1c917ef2c7d2.push_back(query_info);
            call_module_method("get_object_count", _argv_175cd463_d9ac_3cde_804f_1c917ef2c7d2);

            auto cb_get_object_count_obj = std::make_shared<hub_call_dbproxy_get_object_count_cb>(uuid_975425f5_8baf_5905_beeb_4454e78907f6, rsp_cb_hub_call_dbproxy_handle);
            std::lock_guard<std::mutex> l(rsp_cb_hub_call_dbproxy_handle->mutex_map_get_object_count);
            rsp_cb_hub_call_dbproxy_handle->map_get_object_count.insert(std::make_pair(uuid_975425f5_8baf_5905_beeb_4454e78907f6, cb_get_object_count_obj));
            return cb_get_object_count_obj;
        }

    };
/*this module code is codegen by abelkhan codegen for cpp*/
    class dbproxy_call_hub_module : public Imodule, public std::enable_shared_from_this<dbproxy_call_hub_module>{
    public:
        dbproxy_call_hub_module() : Imodule("dbproxy_call_hub")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("ack_get_object_info", std::bind(&dbproxy_call_hub_module::ack_get_object_info, this, std::placeholders::_1));
            reg_method("ack_get_object_info_end", std::bind(&dbproxy_call_hub_module::ack_get_object_info_end, this, std::placeholders::_1));
        }

        concurrent::signals<void(std::string, std::vector<uint8_t>)> sig_ack_get_object_info;
        void ack_get_object_info(const msgpack11::MsgPack::array& inArray){
            auto _callbackid = inArray[0].string_value();
            auto _object_info = inArray[1].binary_items();
            sig_ack_get_object_info.emit(_callbackid, _object_info);
        }

        concurrent::signals<void(std::string)> sig_ack_get_object_info_end;
        void ack_get_object_info_end(const msgpack11::MsgPack::array& inArray){
            auto _callbackid = inArray[0].string_value();
            sig_ack_get_object_info_end.emit(_callbackid);
        }

    };
    class hub_call_dbproxy_reg_hub_rsp : public Response {
    private:
        uint64_t uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67;

    public:
        hub_call_dbproxy_reg_hub_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("hub_call_dbproxy_rsp_cb", _ch)
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

    class hub_call_dbproxy_create_persisted_object_rsp : public Response {
    private:
        uint64_t uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b;

    public:
        hub_call_dbproxy_create_persisted_object_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b = _uuid;
        }

        void rsp(){
            msgpack11::MsgPack::array _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607;
            _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.push_back(uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b);
            call_module_method("create_persisted_object_rsp", _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607);
        }

        void err(){
            msgpack11::MsgPack::array _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607;
            _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607.push_back(uuid_c5ae7137_dfe0_316b_9f1d_5dffa222d32b);
            call_module_method("create_persisted_object_err", _argv_095b02b5_7f29_3bf1_8a63_87de3b3d6607);
        }

    };

    class hub_call_dbproxy_updata_persisted_object_rsp : public Response {
    private:
        uint64_t uuid_16267d40_cddc_312f_87c0_185a55b79ad2;

    public:
        hub_call_dbproxy_updata_persisted_object_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_16267d40_cddc_312f_87c0_185a55b79ad2 = _uuid;
        }

        void rsp(){
            msgpack11::MsgPack::array _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425;
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.push_back(uuid_16267d40_cddc_312f_87c0_185a55b79ad2);
            call_module_method("updata_persisted_object_rsp", _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425);
        }

        void err(){
            msgpack11::MsgPack::array _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425;
            _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425.push_back(uuid_16267d40_cddc_312f_87c0_185a55b79ad2);
            call_module_method("updata_persisted_object_err", _argv_0e29e55c_5309_3e23_82f9_e4944bc2c425);
        }

    };

    class hub_call_dbproxy_find_and_modify_rsp : public Response {
    private:
        uint64_t uuid_c7725286_bd2c_331b_8ba9_90ffcefab6ae;

    public:
        hub_call_dbproxy_find_and_modify_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_c7725286_bd2c_331b_8ba9_90ffcefab6ae = _uuid;
        }

        void rsp(std::vector<uint8_t> object_info){
            msgpack11::MsgPack::array _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58;
            _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.push_back(uuid_c7725286_bd2c_331b_8ba9_90ffcefab6ae);
            _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.push_back(object_info);
            call_module_method("find_and_modify_rsp", _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58);
        }

        void err(){
            msgpack11::MsgPack::array _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58;
            _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58.push_back(uuid_c7725286_bd2c_331b_8ba9_90ffcefab6ae);
            call_module_method("find_and_modify_err", _argv_fadbd43b_fa27_327c_83e3_1ede6e1a2f58);
        }

    };

    class hub_call_dbproxy_remove_object_rsp : public Response {
    private:
        uint64_t uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1;

    public:
        hub_call_dbproxy_remove_object_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1 = _uuid;
        }

        void rsp(){
            msgpack11::MsgPack::array _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da;
            _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.push_back(uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1);
            call_module_method("remove_object_rsp", _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da);
        }

        void err(){
            msgpack11::MsgPack::array _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da;
            _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da.push_back(uuid_f3bda2d9_d71c_307f_b727_d893a1cc0cd1);
            call_module_method("remove_object_err", _argv_28aff888_d5ee_3477_b1f3_249ffe9d48da);
        }

    };

    class hub_call_dbproxy_get_object_count_rsp : public Response {
    private:
        uint64_t uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2;

    public:
        hub_call_dbproxy_get_object_count_rsp(std::shared_ptr<Ichannel> _ch, uint64_t _uuid) : Response("hub_call_dbproxy_rsp_cb", _ch)
        {
            uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2 = _uuid;
        }

        void rsp(uint32_t count){
            msgpack11::MsgPack::array _argv_2632cded_162c_3a9b_86ee_462b614cbeea;
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.push_back(uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2);
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.push_back(count);
            call_module_method("get_object_count_rsp", _argv_2632cded_162c_3a9b_86ee_462b614cbeea);
        }

        void err(){
            msgpack11::MsgPack::array _argv_2632cded_162c_3a9b_86ee_462b614cbeea;
            _argv_2632cded_162c_3a9b_86ee_462b614cbeea.push_back(uuid_175cd463_d9ac_3cde_804f_1c917ef2c7d2);
            call_module_method("get_object_count_err", _argv_2632cded_162c_3a9b_86ee_462b614cbeea);
        }

    };

    class hub_call_dbproxy_module : public Imodule, public std::enable_shared_from_this<hub_call_dbproxy_module>{
    public:
        hub_call_dbproxy_module() : Imodule("hub_call_dbproxy")
        {
        }

        void Init(std::shared_ptr<modulemng> _modules){
            _modules->reg_module(std::static_pointer_cast<Imodule>(shared_from_this()));

            reg_method("reg_hub", std::bind(&hub_call_dbproxy_module::reg_hub, this, std::placeholders::_1));
            reg_method("create_persisted_object", std::bind(&hub_call_dbproxy_module::create_persisted_object, this, std::placeholders::_1));
            reg_method("updata_persisted_object", std::bind(&hub_call_dbproxy_module::updata_persisted_object, this, std::placeholders::_1));
            reg_method("find_and_modify", std::bind(&hub_call_dbproxy_module::find_and_modify, this, std::placeholders::_1));
            reg_method("remove_object", std::bind(&hub_call_dbproxy_module::remove_object, this, std::placeholders::_1));
            reg_method("get_object_info", std::bind(&hub_call_dbproxy_module::get_object_info, this, std::placeholders::_1));
            reg_method("get_object_count", std::bind(&hub_call_dbproxy_module::get_object_count, this, std::placeholders::_1));
        }

        concurrent::signals<void(std::string)> sig_reg_hub;
        void reg_hub(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            auto _hub_name = inArray[1].string_value();
            rsp = std::make_shared<hub_call_dbproxy_reg_hub_rsp>(current_ch, _cb_uuid);
            sig_reg_hub.emit(_hub_name);
            rsp = nullptr;
        }

        concurrent::signals<void(std::string, std::string, std::vector<uint8_t>)> sig_create_persisted_object;
        void create_persisted_object(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            auto _db = inArray[1].string_value();
            auto _collection = inArray[2].string_value();
            auto _object_info = inArray[3].binary_items();
            rsp = std::make_shared<hub_call_dbproxy_create_persisted_object_rsp>(current_ch, _cb_uuid);
            sig_create_persisted_object.emit(_db, _collection, _object_info);
            rsp = nullptr;
        }

        concurrent::signals<void(std::string, std::string, std::vector<uint8_t>, std::vector<uint8_t>, bool)> sig_updata_persisted_object;
        void updata_persisted_object(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            auto _db = inArray[1].string_value();
            auto _collection = inArray[2].string_value();
            auto _query_info = inArray[3].binary_items();
            auto _updata_info = inArray[4].binary_items();
            auto __upsert = inArray[5].bool_value();
            rsp = std::make_shared<hub_call_dbproxy_updata_persisted_object_rsp>(current_ch, _cb_uuid);
            sig_updata_persisted_object.emit(_db, _collection, _query_info, _updata_info, __upsert);
            rsp = nullptr;
        }

        concurrent::signals<void(std::string, std::string, std::vector<uint8_t>, std::vector<uint8_t>, bool, bool)> sig_find_and_modify;
        void find_and_modify(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            auto _db = inArray[1].string_value();
            auto _collection = inArray[2].string_value();
            auto _query_info = inArray[3].binary_items();
            auto _updata_info = inArray[4].binary_items();
            auto __new = inArray[5].bool_value();
            auto __upsert = inArray[6].bool_value();
            rsp = std::make_shared<hub_call_dbproxy_find_and_modify_rsp>(current_ch, _cb_uuid);
            sig_find_and_modify.emit(_db, _collection, _query_info, _updata_info, __new, __upsert);
            rsp = nullptr;
        }

        concurrent::signals<void(std::string, std::string, std::vector<uint8_t>)> sig_remove_object;
        void remove_object(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            auto _db = inArray[1].string_value();
            auto _collection = inArray[2].string_value();
            auto _query_info = inArray[3].binary_items();
            rsp = std::make_shared<hub_call_dbproxy_remove_object_rsp>(current_ch, _cb_uuid);
            sig_remove_object.emit(_db, _collection, _query_info);
            rsp = nullptr;
        }

        concurrent::signals<void(std::string, std::string, std::vector<uint8_t>, std::string)> sig_get_object_info;
        void get_object_info(const msgpack11::MsgPack::array& inArray){
            auto _db = inArray[0].string_value();
            auto _collection = inArray[1].string_value();
            auto _query_info = inArray[2].binary_items();
            auto _callbackid = inArray[3].string_value();
            sig_get_object_info.emit(_db, _collection, _query_info, _callbackid);
        }

        concurrent::signals<void(std::string, std::string, std::vector<uint8_t>)> sig_get_object_count;
        void get_object_count(const msgpack11::MsgPack::array& inArray){
            auto _cb_uuid = inArray[0].uint64_value();
            auto _db = inArray[1].string_value();
            auto _collection = inArray[2].string_value();
            auto _query_info = inArray[3].binary_items();
            rsp = std::make_shared<hub_call_dbproxy_get_object_count_rsp>(current_ch, _cb_uuid);
            sig_get_object_count.emit(_db, _collection, _query_info);
            rsp = nullptr;
        }

    };

}

#endif //_h_dbproxy_ec2ff100_3643_3dba_9c7d_f391c20b497e_
