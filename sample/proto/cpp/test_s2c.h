#ifndef _h_test_s2c_a1cf7490_107a_3422_8f39_e02b73ef3c43_
#define _h_test_s2c_a1cf7490_107a_3422_8f39_e02b73ef3c43_

#include <hub_service.h>
#include <signals.h>


namespace abelkhan
{
/*this enum code is codegen by abelkhan codegen for cpp*/

/*this struct code is codegen by abelkhan codegen for cpp*/
/*this caller code is codegen by abelkhan codegen for cpp*/
    class test_s2c_rsp_cb;
    class test_s2c_ping_cb : public std::enable_shared_from_this<test_s2c_ping_cb>{
    private:
        uint64_t cb_uuid;
        std::shared_ptr<test_s2c_rsp_cb> module_rsp_cb;

    public:
        test_s2c_ping_cb(uint64_t _cb_uuid, std::shared_ptr<test_s2c_rsp_cb> _module_rsp_cb);
    public:
        concurrent::signals<void()> sig_ping_cb;
        concurrent::signals<void()> sig_ping_err;
        concurrent::signals<void()> sig_ping_timeout;

        std::shared_ptr<test_s2c_ping_cb> callBack(std::function<void()> cb, std::function<void()> err);
        void timeout(uint64_t tick, std::function<void()> timeout_cb);
    };

/*this cb code is codegen by abelkhan for cpp*/
    class test_s2c_rsp_cb : public common::imodule, public std::enable_shared_from_this<test_s2c_rsp_cb>{
    public:
        std::mutex mutex_map_ping;
        std::unordered_map<uint64_t, std::shared_ptr<test_s2c_ping_cb> > map_ping;
        test_s2c_rsp_cb() 
        {
        }

        void Init(std::shared_ptr<hub::hub_service> _hub_service){
            _hub_service->modules.add_mothed("test_s2c_rsp_cb_ping_rsp", std::bind(&test_s2c_rsp_cb::ping_rsp, this, std::placeholders::_1));
            _hub_service->modules.add_mothed("test_s2c_rsp_cb_ping_err", std::bind(&test_s2c_rsp_cb::ping_err, this, std::placeholders::_1));
        }

        void ping_rsp(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_ping_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_ping_cb.emit();
            }
        }

        void ping_err(const msgpack11::MsgPack::array& inArray){
            auto uuid = inArray[0].uint64_value();
            auto rsp = try_get_and_del_ping_cb(uuid);
            if (rsp != nullptr){
                rsp->sig_ping_err.emit();
            }
        }

        void ping_timeout(uint64_t cb_uuid){
            auto rsp = try_get_and_del_ping_cb(cb_uuid);
            if (rsp != nullptr){
                rsp->sig_ping_timeout.emit();
            }
        }

        std::shared_ptr<test_s2c_ping_cb> try_get_and_del_ping_cb(uint64_t uuid){
            std::lock_guard<std::mutex> l(mutex_map_ping);
            if (map_ping.find(uuid) != map_ping.end()) {
                auto rsp = map_ping[uuid];
                map_ping.erase(uuid);
                return rsp;
            }
            return nullptr;
        }

    };

    class test_s2c_clientproxy
{
    public:
        std::string client_uuid_a1cf7490_107a_3422_8f39_e02b73ef3c43;
        std::atomic<uint64_t> uuid_a1cf7490_107a_3422_8f39_e02b73ef3c43;

        std::shared_ptr<hub::hub_service> _hub_handle;
        std::shared_ptr<test_s2c_rsp_cb> rsp_cb_test_s2c_handle;

        test_s2c_clientproxy(std::shared_ptr<hub::hub_service> hub_service_, std::shared_ptr<test_s2c_rsp_cb> rsp_cb_test_s2c_handle_)
        {
            _hub_handle = hub_service_;
            rsp_cb_test_s2c_handle = rsp_cb_test_s2c_handle_;
            uuid_a1cf7490_107a_3422_8f39_e02b73ef3c43.store(random());

        }

        std::shared_ptr<test_s2c_ping_cb> ping(){
            auto uuid_80c27ee8_c9bc_583c_bad4_a73880e2ce8f = uuid_a1cf7490_107a_3422_8f39_e02b73ef3c43++;
            msgpack11::MsgPack::array _argv_94d71f95_a670_3916_89a9_44df18fb711b;
            _argv_94d71f95_a670_3916_89a9_44df18fb711b.push_back(uuid_80c27ee8_c9bc_583c_bad4_a73880e2ce8f);
            _hub_handle->_gatemng->call_client(client_uuid_a1cf7490_107a_3422_8f39_e02b73ef3c43, "test_s2c_ping", _argv_94d71f95_a670_3916_89a9_44df18fb711b);
            auto cb_ping_obj = std::make_shared<test_s2c_ping_cb>(uuid_80c27ee8_c9bc_583c_bad4_a73880e2ce8f, rsp_cb_test_s2c_handle);
            std::lock_guard<std::mutex> l(rsp_cb_test_s2c_handle->mutex_map_ping);
            rsp_cb_test_s2c_handle->map_ping.insert(std::make_pair(uuid_80c27ee8_c9bc_583c_bad4_a73880e2ce8f, cb_ping_obj));
            return cb_ping_obj;
        }

    };

    class test_s2c_multicast
{
    public:
        std::vector<std::string> client_uuids_a1cf7490_107a_3422_8f39_e02b73ef3c43;
        std::shared_ptr<hub::hub_service> _hub_handle;
        std::shared_ptr<test_s2c_rsp_cb> rsp_cb_test_s2c_handle;

        test_s2c_multicast(std::shared_ptr<hub::hub_service> hub_service_, std::shared_ptr<test_s2c_rsp_cb> rsp_cb_test_s2c_handle_)
        {
            _hub_handle = hub_service_;
            rsp_cb_test_s2c_handle = rsp_cb_test_s2c_handle_;
        }

    };

    class test_s2c_broadcast
{
    public:
        std::shared_ptr<hub::hub_service> _hub_handle;
        std::shared_ptr<test_s2c_rsp_cb> rsp_cb_test_s2c_handle;

        test_s2c_broadcast(std::shared_ptr<hub::hub_service> hub_service_, std::shared_ptr<test_s2c_rsp_cb> rsp_cb_test_s2c_handle_)
        {
            _hub_handle = hub_service_;
            rsp_cb_test_s2c_handle = rsp_cb_test_s2c_handle_;
        }

    };

    class test_s2c_clientproxy;
    class test_s2c_multicast;
    class test_s2c_broadcast;
    class test_s2c_caller {
    private:
        static std::shared_ptr<test_s2c_rsp_cb> rsp_cb_test_s2c_handle;

    private:
        std::shared_ptr<test_s2c_clientproxy> _clientproxy;
        std::shared_ptr<test_s2c_multicast> _multicast;
        std::shared_ptr<test_s2c_broadcast> _broadcast;

    public:
        test_s2c_caller(std::shared_ptr<hub::hub_service> hub_service_) 
        {
            if (rsp_cb_test_s2c_handle == nullptr) {
                rsp_cb_test_s2c_handle = std::make_shared<test_s2c_rsp_cb>();
                rsp_cb_test_s2c_handle->Init(hub_service_);
            }
            _clientproxy = std::make_shared<test_s2c_clientproxy>(hub_service_, rsp_cb_test_s2c_handle);
            _multicast = std::make_shared<test_s2c_multicast>(hub_service_, rsp_cb_test_s2c_handle);
            _broadcast = std::make_shared<test_s2c_broadcast>(hub_service_, rsp_cb_test_s2c_handle);
        }

        std::shared_ptr<test_s2c_clientproxy> get_client(std::string client_uuid) {
            _clientproxy->client_uuid_a1cf7490_107a_3422_8f39_e02b73ef3c43 = client_uuid;
            return _clientproxy;
        }

        std::shared_ptr<test_s2c_multicast> get_multicast(std::vector<std::string> client_uuids) {
            _multicast->client_uuids_a1cf7490_107a_3422_8f39_e02b73ef3c43 = client_uuids;
            return _multicast;
        }

        std::shared_ptr<test_s2c_broadcast> get_broadcast() {
            return _broadcast;
        }

    };

}

#endif //_h_test_s2c_a1cf7490_107a_3422_8f39_e02b73ef3c43_
