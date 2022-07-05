/*
 * qianqians
 * 2020-1-10
 * hub.cpp
 */
#include <enetacceptservice.h>
#include <redismqservice.h>
#include <connectservice.h>
#include <acceptservice.h>
#include <websocketacceptservice.h>

#include <client.h>

#include "hub_service.h"
#include "hubsvrmanager.h"
#include "gatemanager.h"
#include "gc_poll.h"

#include "center_msg_handle.h"
#include "dbproxy_msg_handle.h"
#include "hub_svr_msg_handle.h"
#include "gate_msg_handle.h"
#include "direct_client_msg_handle.h"

namespace _log {

std::shared_ptr<spdlog::logger> file_logger = nullptr;
}

namespace hub{

hub_service::hub_service(std::string config_file_path, std::string config_name, std::string hub_type_) {
	_config = std::make_shared<config::config>(config_file_path);
	_center_config = _config->get_value_dict("center");
	_root_config = _config;
	_config = _config->get_value_dict(config_name);

	reconn_count = 0;
	name_info.name = config_name;
	hub_type = hub_type_;
	tick = 0;
	is_busy = false;
}

void hub_service::init() {
	enet_initialize();
	ares_library_init(ARES_LIB_INIT_ALL);

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

	_io_service = std::make_shared<asio::io_service>();

	if (_config->has_key("host") && _config->has_key("port")) {
		if (enet_initialize() != 0)
		{
			spdlog::error("An error occurred while initializing ENet!");
		}
		auto host = _config->get_value_string("host");
		auto port = _config->get_value_int("port");
		_hub_service = std::make_shared<service::enetacceptservice>(host, (short)port);
		_gatemng = std::make_shared<gatemanager>(_hub_service, shared_from_this());
		is_enet = true;
	}
	else if (_root_config->has_key("redismq_listen") && _root_config->get_value_bool("redismq_listen")) {
		auto redismq_url = _root_config->get_value_string("redis_for_mq");
		auto redismq_is_cluster = _root_config->get_value_bool("redismq_is_cluster");
		if (_root_config->has_key("redis_for_mq_pwd")) {
			auto password = _root_config->get_value_string("redis_for_mq_pwd");
			_hub_redismq_service = std::make_shared<service::redismqservice>(redismq_is_cluster, name_info.name, redismq_url, password);
		}
		else {
			_hub_redismq_service = std::make_shared<service::redismqservice>(redismq_is_cluster, name_info.name, redismq_url);
		}
		_hub_redismq_service->start(); 
		_gatemng = std::make_shared<gatemanager>(_hub_redismq_service, shared_from_this());
		is_enet = false;
	}
	else {
		spdlog::error("undefined hub msg listen model!");
	}

	_timerservice = std::make_shared<service::timerservice>();
	_close_handle = std::make_shared<closehandle>();
	_hubmng = std::make_shared<hubsvrmanager>(shared_from_this());

	_center_msg_handle = std::make_shared<center_msg_handle>(shared_from_this());
	_dbproxy_msg_handle = std::make_shared<dbproxy_msg_handle>();
	_direct_client_msg_handle = std::make_shared<direct_client_msg_handle>(_gatemng, shared_from_this());
	_gate_msg_handle = std::make_shared<gate_msg_handle>(shared_from_this(), _gatemng);
	_hub_svr_msg_handle = std::make_shared<hub_svr_msg_handle>(shared_from_this(), _hubmng, _gatemng);
	
	_center_service = std::make_shared<service::connectservice>(_io_service);
	_dbproxy_service = std::make_shared<service::connectservice>(_io_service);

	if (_config->has_key("tcp_listen")) {
		auto is_tcp_listen = _config->get_value_bool("tcp_listen");
		if (is_tcp_listen) {
			tcp_address_info = std::make_shared<addressinfo>();
			tcp_address_info->host = _config->get_value_string("tcp_outside_host");
			tcp_address_info->port = (unsigned short)_config->get_value_int("tcp_outside_port");
			_client_tcp_service = std::make_shared<service::acceptservice>(tcp_address_info->host, tcp_address_info->port, _io_service);
			_client_tcp_service->sigchannelconnect.connect([this](std::shared_ptr<abelkhan::Ichannel> ch) {
				std::static_pointer_cast<service::channel>(ch)->set_xor_key_crypt();
			});
			_client_tcp_service->sigchanneldisconnect.connect([this](std::shared_ptr<abelkhan::Ichannel> ch) {
				service::gc_put([this, ch]() {
					_gatemng->client_direct_disconnect(ch);
				});
			});
		}
	}

	if (_config->has_key("websocket_listen")) {
		auto is_websocket_listen = _config->get_value_bool("websocket_listen");
		if (is_websocket_listen) {
			websocket_address_info = std::make_shared<addressinfo>();
			websocket_address_info->host = _config->get_value_string("websocket_outside_host");
			websocket_address_info->port = (short)_config->get_value_int("websocket_outside_port");
			auto is_ssl = _config->get_value_bool("is_ssl");
			std::string certificate, private_key, tmp_dh;
			if (is_ssl) {
				certificate = _config->get_value_string("certificate");
				private_key = _config->get_value_string("private_key");
				tmp_dh = _config->get_value_string("tmp_dh");
			}
			_client_websocket_service = std::make_shared<service::webacceptservice>(websocket_address_info->host, websocket_address_info->port, is_ssl, certificate, private_key, tmp_dh);
			_client_websocket_service->sigchannelconnect.connect([this](std::shared_ptr<abelkhan::Ichannel> ch) {
			});
			_client_websocket_service->sigchanneldisconnect.connect([this](std::shared_ptr<abelkhan::Ichannel> ch) {
				service::gc_put([this, ch]() {
					_gatemng->client_direct_disconnect(ch);
				});
			});
		}
	}
}

void hub_service::heartbeat(std::shared_ptr<hub_service> this_ptr, int64_t _tick) {
	do {
		if ((_tick - this_ptr->_centerproxy->timetmp) > 6 * 1000) {
			this_ptr->reconnect_center();
			break;
		}
		this_ptr->_centerproxy->heartbeat(this_ptr->tick);

	} while (false);

	this_ptr->_timerservice->addticktimer(3 * 1000, std::bind(&hub_service::heartbeat, this_ptr, std::placeholders::_1));
}

void hub_service::connect_center() {
	spdlog::trace("begin on connect center");

	if (_root_config->has_key("redismq_listen") && _root_config->get_value_bool("redismq_listen")) {
		auto center_ch = _hub_redismq_service->connect(_center_config->get_value_string("name"));
		_centerproxy = std::make_shared<centerproxy>(center_ch, _timerservice);
		if (is_enet) {
			_centerproxy->reg_server(_config->get_value_string("host"), (short)_config->get_value_int("port"), hub_type, name_info);
		}
		else {
			_centerproxy->reg_server(hub_type, name_info);
		}
		heartbeat(shared_from_this(), _timerservice->Tick);
	}
	else {
		auto ip = _center_config->get_value_string("host");
		auto port = (short)(_center_config->get_value_int("port"));
		_center_service->connect(ip, port, [this](auto center_ch) {
			spdlog::trace("connect center success");

			_centerproxy = std::make_shared<centerproxy>(center_ch, _timerservice);
			if (is_enet) {
				_centerproxy->reg_server(_config->get_value_string("host"), (short)_config->get_value_int("port"), hub_type, name_info);
			}
			else {
				_centerproxy->reg_server(hub_type, name_info);
			}

			heartbeat(shared_from_this(), _timerservice->Tick);

			spdlog::trace("end on connect center");
		});
	}
}

void hub_service::reconnect_center() {
	spdlog::trace("begin on connect center");

	if (reconn_count > 5) {
		sig_center_crash.emit();
	}

	++reconn_count;

	if (_root_config->has_key("redismq_listen") && _root_config->get_value_bool("redismq_listen")) {
		auto center_ch = _hub_redismq_service->connect(_center_config->get_value_string("name"));
		_centerproxy = std::make_shared<centerproxy>(center_ch, _timerservice);
		if (is_enet) {
			_centerproxy->reconn_reg_server(_config->get_value_string("host"), (short)_config->get_value_int("port"), hub_type, name_info);
		}
		else {
			_centerproxy->reconn_reg_server(hub_type, name_info);
		}
		reconn_count = 0;
	}
	else {
		auto ip = _center_config->get_value_string("host");
		auto port = (short)(_center_config->get_value_int("port"));
		_center_service->connect(ip, port, [this](auto center_ch) {
			reconn_count = 0;

			_centerproxy = std::make_shared<centerproxy>(center_ch, _timerservice);
			if (is_enet) {
				_centerproxy->reconn_reg_server(_config->get_value_string("host"), (short)_config->get_value_int("port"), hub_type, name_info);
			}
			else {
				_centerproxy->reconn_reg_server(hub_type, name_info);
			}

			spdlog::trace("end on connect center");
		});
	}
}

void hub_service::connect_gate(std::string gate_name, std::string host, uint16_t port) {
	_gatemng->connect_gate(gate_name, host, port);
}

void hub_service::connect_gate(std::string gate_name) {
	_gatemng->connect_gate(gate_name);
}

void hub_service::reg_hub(std::string hub_host, uint16_t hub_port) {
	_hub_service->connect(hub_host, hub_port, [this, hub_host, hub_port](std::shared_ptr<abelkhan::Ichannel> ch){
		spdlog::trace("hub host:{0} port:{1} reg_hub", hub_host, hub_port);
 		auto caller = std::make_shared<abelkhan::hub_call_hub_caller>(ch, service::_modulemng);
		caller->reg_hub(name_info.name, hub_type)->callBack([hub_host, hub_port]() {
			spdlog::trace("hub host:{0} port:{1} reg_hub sucessed!", hub_host, hub_port);
		}, [hub_host, hub_port]() {
			spdlog::trace("hub host:{0} port:{1} reg_hub faild!", hub_host, hub_port);
		})->timeout(5 * 1000, [hub_host, hub_port]() {
			spdlog::trace("hub host:{0} port:{1} reg_hub timeout!", hub_host, hub_port);
		});
	});
}

void hub_service::reg_hub(std::string hub_name) {
	auto ch = _hub_redismq_service->connect(hub_name);
	spdlog::trace("hub hub_name:{0} reg_hub", hub_name);
	auto caller = std::make_shared<abelkhan::hub_call_hub_caller>(ch, service::_modulemng);
	caller->reg_hub(name_info.name, hub_type)->callBack([hub_name]() {
		spdlog::trace("hub hub_name:{0} reg_hub sucessed!", hub_name);
	}, [hub_name]() {
		spdlog::trace("hub hub_name:{0} reg_hub faild!", hub_name);
	})->timeout(5 * 1000, [hub_name]() {
		spdlog::trace("hub hub_name:{0} reg_hub timeout!", hub_name);
	});
}

void hub_service::try_connect_db(std::string dbproxy_name, std::string dbproxy_host, uint16_t dbproxy_port) {
	if (_config->has_key("dbproxy") && _config->get_value_string("dbproxy") == dbproxy_name) {
		auto dbproxy_cfg = _root_config->get_value_dict(dbproxy_name);
		auto _db_ip = service::DNS(dbproxy_host);
		_dbproxy_service->connect(_db_ip, dbproxy_port, [this](std::shared_ptr<abelkhan::Ichannel> ch) {
			_dbproxyproxy = std::make_shared<dbproxyproxy>(ch);
			_dbproxyproxy->sig_dbproxy_init.connect([this]() {
				sig_dbproxy_init.emit();
			});
			_dbproxyproxy->reg_server(name_info.name);
		});
	}
	else if (_config->has_key("extend_dbproxy") && _config->get_value_string("extend_dbproxy") == dbproxy_name) {
		auto dbproxy_cfg = _root_config->get_value_dict(dbproxy_name);
		auto _db_ip = service::DNS(dbproxy_host);
		_dbproxy_service->connect(_db_ip, dbproxy_port, [this](std::shared_ptr<abelkhan::Ichannel> ch) {
			_extend_dbproxyproxy = std::make_shared<dbproxyproxy>(ch);
			_extend_dbproxyproxy->sig_dbproxy_init.connect([this]() {
				sig_dbproxy_init.emit();
				});
			_extend_dbproxyproxy->reg_server(name_info.name);
		});
	}
}

void hub_service::try_connect_db(std::string dbproxy_name) {
	if (_config->has_key("dbproxy") && _config->get_value_string("dbproxy") == dbproxy_name) {
		auto dbproxy_cfg = _root_config->get_value_dict(dbproxy_name);
		auto ch = _hub_redismq_service->connect(dbproxy_name);
		_dbproxyproxy = std::make_shared<dbproxyproxy>(ch);
		_dbproxyproxy->sig_dbproxy_init.connect([this]() {
			sig_dbproxy_init.emit();
		});
		_dbproxyproxy->reg_server(name_info.name);
	}
	else if (_config->has_key("extend_dbproxy") && _config->get_value_string("extend_dbproxy") == dbproxy_name) {
		auto dbproxy_cfg = _root_config->get_value_dict(dbproxy_name);
		auto ch = _hub_redismq_service->connect(dbproxy_name);
		_extend_dbproxyproxy = std::make_shared<dbproxyproxy>(ch);
		_extend_dbproxyproxy->sig_dbproxy_init.connect([this]() {
			sig_dbproxy_init.emit();
		});
		_extend_dbproxyproxy->reg_server(name_info.name);
	}
}

void hub_service::close_svr() {
	_close_handle->is_closed = true;
	if (_hub_redismq_service) {
		_hub_redismq_service->close();
	}
	if (_hub_service) {
		enet_deinitialize();
	}
	spdlog::shutdown();
}

void hub_service::run() {
	if (!_run_mu.try_lock()) {
		throw abelkhan::Exception("run mast at single thread!");
	}

	while (!_close_handle->is_closed) {
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

uint32_t hub_service::poll() {
	auto time_now = msec_time();

	try {
		_io_service->poll();

		if (_hub_service != nullptr) {
			_hub_service->poll();
		}

		if (_hub_redismq_service != nullptr) {
			_hub_redismq_service->poll();
		}

		if (_client_websocket_service != nullptr) {
			_client_websocket_service->poll();
		}

		abelkhan::TinyTimer::poll();

		_timerservice->poll();

		service::gc_poll();

		_log::file_logger->flush();
	}
	catch (std::exception err) {
		spdlog::error("hub_service::poll error:{0}", err.what());
	}

	tick = static_cast<uint32_t>(msec_time() - time_now);

	return tick;
}

}
