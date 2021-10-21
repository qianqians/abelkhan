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

    auto client_list = std::make_shared<std::vector<std::string> >();
    auto _test_c2s_module = std::make_shared<abelkhan::test_c2s_module>();
    _test_c2s_module->Init(_hub);
    _test_c2s_module->sig_login.connect([client_list, _hub]() {
        spdlog::trace("client{0} login!", _hub->_gatemng->current_client_cuuid);
        client_list->push_back(_hub->_gatemng->current_client_cuuid);
    });
    _test_c2s_module->sig_get_svr_host.connect([_test_c2s_module]() {
        spdlog::trace("sig_get_svr_host!");
        auto rsp = std::static_pointer_cast<abelkhan::test_c2s_get_svr_host_rsp>(_test_c2s_module->rsp);
        rsp->rsp("127.0.0.1", (uint16_t)4001);
    });

    auto _test_s2c_caller = std::make_shared<abelkhan::test_s2c_caller>(_hub);
    _hub->_timerservice->addticktimer(3000, std::bind(heartbeat, _hub, _test_s2c_caller, client_list, std::placeholders::_1));

    _hub->connect_center();

    try {
        while (1) {
            auto tick_time = _hub->poll();

            if (_hub->_close_handle->is_closed) {
                break;
            }
        }
    }
    catch (std::exception e) {
        spdlog::error("error:{0}", e.what());
    }

	return 0;
}
