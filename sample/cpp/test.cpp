#include <hub_service.h>

#include <test_c2s.h>
#include <test_s2c.h>

void heartbeat(std::shared_ptr<hub::hub_service> _hub, std::shared_ptr<abelkhan::test_s2c_caller> _caller, std::shared_ptr<std::vector<std::string> > client_list, uint64_t tick) {
    for (auto cuuid : *client_list) {
        _caller->get_client(cuuid)->ping()->callBack([]() {
            spdlog::trace("ping client sucessed!");
        }, []() {
            spdlog::error("ping client faild!");
        })->timeout(5000, []() {
            spdlog::error("ping client timeout!");
        });
    }
    _hub->_timerservice->addticktimer(3000, std::bind(heartbeat, _hub, _caller, client_list, std::placeholders::_1));
}

int main(int argc, char* argv[]) {
    if (argc <= 1) {
        spdlog::error("non input start argv");
        return 1;
    }

    std::string config_file_path = argv[1];
    std::string config_name = argv[2];

    auto _hub = std::make_shared<hub::hub_service>(config_file_path, config_name, "test");
    _hub->init();
    _hub->sig_close_server.connect([_hub]() {
        _hub->close_svr();
    });

    _hub->sig_dbproxy_init.connect([_hub]() {
        BSON::Value query = BSON::Object{};
        _hub->get_random_dbproxy()->getCollection("test", "test")->getObjectInfo(query, [](auto _array) {
            for (auto doc : _array) {
                spdlog::trace("getObjectInfo doc:{0}!", doc.toJSON());
            }
        }, []() {
            spdlog::trace("getObjectInfo end!");
        });
    });

    auto client_list = std::make_shared<std::vector<std::string> >();
    auto _test_c2s_module = std::make_shared<abelkhan::test_c2s_module>();
    _test_c2s_module->Init(_hub);
    _test_c2s_module->sig_login.connect([client_list, _hub]() {
        spdlog::trace("client{0} login!", _hub->_gatemng->current_client_cuuid);
        client_list->push_back(_hub->_gatemng->current_client_cuuid);

        BSON::Value doc = BSON::Object{ { "info", BSON::Array{"svr", "test_cpp", "cuuid", _hub->_gatemng->current_client_cuuid}}};
        _hub->get_random_dbproxy()->getCollection("test", "test")->createPersistedObject(doc, [](auto ret) {
            spdlog::trace("createPersistedObject ret:{0}!", ret);
        });
    });
    _test_c2s_module->sig_get_svr_host.connect([_hub, _test_c2s_module]() {
        spdlog::trace("sig_get_svr_host!");
        auto rsp = std::static_pointer_cast<abelkhan::test_c2s_get_svr_host_rsp>(_test_c2s_module->rsp);
        rsp->rsp(_hub->tcp_address_info->host, _hub->tcp_address_info->port);
    });
    _test_c2s_module->sig_get_websocket_svr_host.connect([_hub, _test_c2s_module]() {
        spdlog::trace("get_websocket_svr_host!");
        auto rsp = std::static_pointer_cast<abelkhan::test_c2s_get_websocket_svr_host_rsp>(_test_c2s_module->rsp);
        rsp->rsp(_hub->websocket_address_info->host, _hub->websocket_address_info->port);
    });

    auto _test_s2c_caller = std::make_shared<abelkhan::test_s2c_caller>(_hub);
    _hub->_timerservice->addticktimer(3000, std::bind(heartbeat, _hub, _test_s2c_caller, client_list, std::placeholders::_1));

    _hub->connect_center();

    _hub->run();

	return 0;
}
