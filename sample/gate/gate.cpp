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

    _gate->run();

	return 0;
}
