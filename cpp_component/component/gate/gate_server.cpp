/*
 * qianqians
 * 2016-7-5
 * gate.cpp
 */
#include <boost/uuid/uuid.hpp>
#include <boost/uuid/uuid_generators.hpp>
#include <boost/uuid/uuid_io.hpp>
#include <boost/lexical_cast.hpp>

#include <abelkhan.h>

#include <acceptservice.h>
#include <connectservice.h>
#include <enetacceptservice.h>
#include <websocketacceptservice.h>
#include <timerservice.h>
#include <modulemng_handle.h>
#include <config.h>
#include <log.h>
#include <gc_poll.h>

#include "centerproxy.h"
#include "closehandle.h"
#include "clientmanager.h"
#include "hubsvrmanager.h"
#include "timer_handle.h"
#include "center_msg_handle.h"
#include "hub_svr_msg_handle.h"
#include "client_msg_handle.h"

namespace _log {
std::shared_ptr<spdlog::logger> file_logger = nullptr;
}

int main(int argc, char * argv[]) {
	if (argc <= 1) {
		std::cout << "non input start argv" << std::endl;
		return -1;
	}

	std::string config_file_path = argv[1];
	auto _config = std::make_shared<config::config>(config_file_path);
	auto _center_config = _config->get_value_dict("center");
	if (argc >= 3) {
		_config = _config->get_value_dict(argv[2]);
	}

	gate::name_info gate_name_info;
	gate_name_info.name = argv[2];
	gate_name_info.serial = 0;

	auto file_path = _config->get_value_string("log_dir") + _config->get_value_string("log_file");
	auto log_level = _config->get_value_string("log_level");
	if (log_level == "trace") {
		_log::InitLog(file_path, spdlog::level::level_enum::trace);
	}
	else if (log_level == "debug") {
		_log::InitLog(file_path, spdlog::level::level_enum::debug);
	}
	else if (log_level == "info") {
		_log::InitLog(file_path, spdlog::level::level_enum::info);
	}
	else if (log_level == "warn") {
		_log::InitLog(file_path, spdlog::level::level_enum::warn);
	}
	else if (log_level == "error") {
		_log::InitLog(file_path, spdlog::level::level_enum::err);
	}

	if (enet_initialize() != 0)
	{
		spdlog::error("An error occurred while initializing ENet!");
	}
	ares_library_init(ARES_LIB_INIT_ALL);

	auto _timerservice = std::make_shared<service::timerservice>();
	auto _hubsvrmanager = std::make_shared<gate::hubsvrmanager>();
	auto _clientmanager = std::make_shared<gate::clientmanager>(_hubsvrmanager);
	auto _closehandle = std::make_shared<gate::closehandle>();

	auto _center_msg_handle = std::make_shared<gate::center_msg_handle>(_closehandle, _hubsvrmanager, _timerservice);
	auto _hub_svr_msg_handle = std::make_shared<gate::hub_svr_msg_handle>(_clientmanager, _hubsvrmanager);
	auto _client_msg_handle = std::make_shared<gate::client_msg_handle>(_clientmanager, _hubsvrmanager, _timerservice);

	auto inside_host = _config->get_value_string("inside_host");
	auto inside_port = (short)_config->get_value_int("inside_port");
	auto _hub_service = std::make_shared<service::enetacceptservice>(inside_host, inside_port);

	auto io_service = std::make_shared<boost::asio::io_service>();
	auto _connectnetworkservice = std::make_shared<service::connectservice>(io_service);
	auto center_ip = _center_config->get_value_string("host");
	auto center_port = (short)_center_config->get_value_int("port");
	auto _center_ch = _connectnetworkservice->connect(center_ip, center_port);
	auto _centerproxy = std::make_shared<gate::centerproxy>(_center_ch);
	_centerproxy->reg_server(inside_host, inside_port, gate_name_info);

	std::shared_ptr<service::acceptservice> _client_service = nullptr;
	if (_config->has_key("tcp_listen")) {
		auto is_tcp_listen = _config->get_value_bool("tcp_listen");
		if (is_tcp_listen) {
			auto tcp_outside_host = _config->get_value_string("tcp_outside_host");
			auto tcp_outside_port = (short)_config->get_value_int("tcp_outside_port");
			_client_service = std::make_shared<service::acceptservice>(tcp_outside_host, tcp_outside_port, io_service);
			_client_service->sigchannelconnect.connect([_clientmanager](std::shared_ptr<abelkhan::Ichannel> ch) {
				std::static_pointer_cast<service::channel>(ch)->set_xor_key_crypt();

				auto _client = _clientmanager->reg_client(ch);
				_client->ntf_cuuid();
			});
			_client_service->sigchanneldisconnect.connect([_clientmanager](std::shared_ptr<abelkhan::Ichannel> ch) {
				service::gc_put([_clientmanager, ch]() {
					_clientmanager->unreg_client(ch);
				});
			});
		}
	}

	std::shared_ptr<service::webacceptservice> _websocket_service = nullptr;
	if (_config->has_key("websocket_listen")) {
		auto is_websocket_listen = _config->get_value_bool("websocket_listen");
		if (is_websocket_listen) {
			auto websocket_outside_host = _config->get_value_string("websocket_outside_host");
			auto websocket_outside_port = (short)_config->get_value_int("websocket_outside_port");
			auto is_ssl = _config->get_value_bool("is_ssl");
			std::string certificate, private_key, tmp_dh;
			if (is_ssl) {
				auto certificate = _config->get_value_string("certificate");
				auto private_key = _config->get_value_string("private_key");
				auto tmp_dh = _config->get_value_string("tmp_dh");
			}
			_websocket_service = std::make_shared<service::webacceptservice>(websocket_outside_host, websocket_outside_port, is_ssl, certificate, private_key, tmp_dh);
			_websocket_service->sigchannelconnect.connect([_clientmanager](std::shared_ptr<abelkhan::Ichannel> ch) {
				auto _client = _clientmanager->reg_client(ch);
				_client->ntf_cuuid();
			});
			_websocket_service->sigchanneldisconnect.connect([_clientmanager](std::shared_ptr<abelkhan::Ichannel> ch) {
				service::gc_put([_clientmanager, ch]() {
					_clientmanager->unreg_client(ch);
				});
			});
		}
	}

	_timerservice->addticktimer(10 * 1000, std::bind(heartbeat_client, _clientmanager, _timerservice, std::placeholders::_1));
	heartbeat_center(_centerproxy, _timerservice, _timerservice->Tick);

	while (true){
		clock_t begin = clock();
		try {

			io_service->poll();

			_hub_service->poll();
			
			if (_websocket_service != nullptr) {
				_websocket_service->poll();
			}

			abelkhan::TinyTimer::poll();

			_timerservice->poll();

			_log::file_logger->flush();

		}
		catch(std::exception e) {
			spdlog::info("poll error:{0}!", e.what());
		}

		try {
			service::gc_poll();
		}
		catch (std::exception e) {
			spdlog::info("gc_poll, error:{0}!", e.what());
		}

		if (_closehandle->is_closed) {
			enet_deinitialize();
			spdlog::info("server closed, gate server name:{0}!", gate_name_info.name);
			spdlog::shutdown();
			break;
		}

		if ((clock() - begin) < 50){
			std::this_thread::yield();
		}
	}

	return 0;
}