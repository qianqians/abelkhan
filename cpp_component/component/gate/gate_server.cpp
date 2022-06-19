/*
 * qianqians
 * 2016-7-5
 * gate.cpp
 */
#include <acceptservice.h>
#include <connectservice.h>
#include <enetacceptservice.h>
#include <redismqservice.h>
#include <websocketacceptservice.h>
#include <modulemng_handle.h>

#include "center_msg_handle.h"
#include "hub_svr_msg_handle.h"
#include "client_msg_handle.h"
#include "gate_server.h"

namespace _log {
std::shared_ptr<spdlog::logger> file_logger = nullptr;
}

namespace gate {

gate_service::gate_service(std::string config_file_path, std::string config_name) {
	_root_config = std::make_shared<config::config>(config_file_path);
	_center_config = _root_config->get_value_dict("center");
	_config = _root_config->get_value_dict(config_name);

	gate_name_info.name = config_name;
	reconn_count = 0;
	tick = 0;
}

void gate_service::init() {
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

	ares_library_init(ARES_LIB_INIT_ALL);

	auto this_ptr = shared_from_this();

	_timerservice = std::make_shared<service::timerservice>();
	_hubsvrmanager = std::make_shared<hubsvrmanager>();
	_clientmanager = std::make_shared<clientmanager>(_hubsvrmanager);
	_closehandle = std::make_shared<closehandle>();

	_center_msg_handle = std::make_shared<center_msg_handle>(this_ptr, _hubsvrmanager, _timerservice);
	_hub_svr_msg_handle = std::make_shared<hub_svr_msg_handle>(_clientmanager, _hubsvrmanager);
	_client_msg_handle = std::make_shared<client_msg_handle>(_clientmanager, _hubsvrmanager, _timerservice);

	bool is_enet = false;
	if (_config->has_key("inside_host") && _config->has_key("inside_port")) {
		if (enet_initialize() != 0)
		{
			spdlog::error("An error occurred while initializing ENet!");
		}
		inside_host = _config->get_value_string("inside_host");
		inside_port = (short)_config->get_value_int("inside_port");
		_hub_service = std::make_shared<service::enetacceptservice>(inside_host, inside_port);
		is_enet = true; 
	}
	else if (_root_config->has_key("redismq_listen") && _root_config->get_value_bool("redismq_listen")) {
		auto redismq_url = _root_config->get_value_string("redis_for_mq");
		auto redismq_is_cluster = _root_config->get_value_bool("redismq_is_cluster");
		if (_root_config->has_key("redis_for_mq_pwd")) {
			auto password = _root_config->get_value_string("redis_for_mq_pwd");
			_hub_redismq_service = std::make_shared<service::redismqservice>(redismq_is_cluster, gate_name_info.name, redismq_url, password);
		}
		else {
			_hub_redismq_service = std::make_shared<service::redismqservice>(redismq_is_cluster, gate_name_info.name, redismq_url);
		}
		_hub_redismq_service->start();
		is_enet = false;
	}
	else {
		spdlog::error("undefined hub msg listen model!");
	}

	io_service = std::make_shared<asio::io_service>();
	_connectnetworkservice = std::make_shared<service::connectservice>(io_service);
	center_ip = _center_config->get_value_string("host");
	center_port = (short)_center_config->get_value_int("port");
	_connectnetworkservice->connect(center_ip, center_port, [this, is_enet, this_ptr](auto _center_ch) {
		_centerproxy = std::make_shared<centerproxy>(_center_ch, _timerservice);

		if (is_enet) {
			_centerproxy->reg_server(inside_host, inside_port, gate_name_info);
		}
		else {
			_centerproxy->reg_server(gate_name_info);
		}

		heartbeat_center(shared_from_this(), [is_enet, this_ptr]() {
			if (this_ptr->reconn_count > 5) {
				spdlog::critical("connect center faild count:{0}!", this_ptr->reconn_count);
				this_ptr->sig_center_crash.emit();
			}

			++this_ptr->reconn_count;

			this_ptr->_connectnetworkservice->connect(this_ptr->center_ip, this_ptr->center_port, [is_enet, this_ptr](auto _center_ch) {
				this_ptr->_centerproxy = std::make_shared<centerproxy>(_center_ch, this_ptr->_timerservice);
				if (is_enet) {
					this_ptr->_centerproxy->reconn_reg_server(this_ptr->inside_host, this_ptr->inside_port, this_ptr->gate_name_info);
				}
				else {
					this_ptr->_centerproxy->reconn_reg_server(this_ptr->gate_name_info);
				}

				this_ptr->reconn_count = 0;
			});
		}, _timerservice->Tick);
	});

	if (_config->has_key("tcp_listen")) {
		auto is_tcp_listen = _config->get_value_bool("tcp_listen");
		if (is_tcp_listen) {
			auto tcp_outside_host = _config->get_value_string("tcp_outside_host");
			auto tcp_outside_port = (short)_config->get_value_int("tcp_outside_port");
			_client_service = std::make_shared<service::acceptservice>(tcp_outside_host, tcp_outside_port, io_service);
			_client_service->sigchannelconnect.connect([this](std::shared_ptr<abelkhan::Ichannel> ch) {
				std::static_pointer_cast<service::channel>(ch)->set_xor_key_crypt();

				auto _client = _clientmanager->reg_client(ch);
				_client->ntf_cuuid();
			});
			_client_service->sigchanneldisconnect.connect([this](std::shared_ptr<abelkhan::Ichannel> ch) {
				service::gc_put([this, ch]() {
					_clientmanager->unreg_client(ch);
				});
			});
		}
	}

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
			_websocket_service->sigchannelconnect.connect([this](std::shared_ptr<abelkhan::Ichannel> ch) {
				auto _client = _clientmanager->reg_client(ch);
				_client->ntf_cuuid();
			});
			_websocket_service->sigchanneldisconnect.connect([this](std::shared_ptr<abelkhan::Ichannel> ch) {
				service::gc_put([this, ch]() {
					_clientmanager->unreg_client(ch);
				});
			});
		}
	}

	_timerservice->addticktimer(10 * 1000, std::bind(heartbeat_client, _clientmanager, _timerservice, std::placeholders::_1));
}

void gate_service::heartbeat_center(std::shared_ptr<gate_service> _gate_service, std::function<void()> reconn_func, int64_t tick) {
	do {
		if ((tick - _gate_service->_centerproxy->timetmp) > 6 * 1000) {
			reconn_func();
			break;
		}

		_gate_service->_centerproxy->heartbeat(_gate_service->tick);

	} while (false);

	_gate_service->_timerservice->addticktimer(3 * 1000, std::bind(&gate_service::heartbeat_center, _gate_service, reconn_func, std::placeholders::_1));
}

void gate_service::run() {
	if (!_run_mu.try_lock()) {
		throw abelkhan::Exception("run mast at single thread!");
	}

	while (!_closehandle->is_closed) {
		try {
			auto tick_time = poll();

			if (tick_time < 33) {
				std::this_thread::yield();
			}
		}
		catch (std::exception e) {
			spdlog::error("error:{0}", e.what());
		}
	}

	_run_mu.unlock();
}

uint32_t gate_service::poll(){
	auto begin = msec_time();
	try {

		io_service->poll();

		if (_hub_service != nullptr) {
			_hub_service->poll();
		}

		if (_hub_redismq_service != nullptr) {
			_hub_redismq_service->poll();
		}

		if (_websocket_service != nullptr) {
			_websocket_service->poll();
		}

		abelkhan::TinyTimer::poll();

		_timerservice->poll();

		_log::file_logger->flush();

	}
	catch (std::exception e) {
		spdlog::info("poll error:{0}!", e.what());
	}

	try {
		service::gc_poll();
	}
	catch (std::exception e) {
		spdlog::info("gc_poll, error:{0}!", e.what());
	}

	tick = static_cast<uint32_t>(msec_time() - begin);
	return tick;
}

void gate_service::close_svr() {
	spdlog::info("server closed, gate server name:{0}!", gate_name_info.name);
	_closehandle->is_closed = true;
	enet_deinitialize();
	spdlog::shutdown();
}

}