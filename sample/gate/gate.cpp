#include <gate_server.h>

int main(int argc, char* argv[]) {
    if (argc <= 1) {
        spdlog::error("non input start argv");
        return 1;
    }

    std::string config_file_path = argv[1];
    std::string config_name = argv[2];

    auto _gate = std::make_shared<gate::gate_service>(config_file_path, config_name);
    _gate->init();
    _gate->sig_close_server.connect([_gate]() {
        _gate->close_svr();
    });

    try {
        while (1) {
            auto tick_time = _gate->poll();

            if (_gate->_closehandle->is_closed) {
                break;
            }
        }
    }
    catch (std::exception e) {
        spdlog::error("error:{0}", e.what());
    }

	return 0;
}
