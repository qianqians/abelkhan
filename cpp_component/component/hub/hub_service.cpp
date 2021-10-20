/*
 * qianqians
 * 2020-1-10
 * hub.cpp
 */
#include <enetacceptservice.h>
#include <connectservice.h>
#include <acceptservice.h>
#include <websocketacceptservice.h>

#include <client.h>

#include "hub_service.h"
#include "centerproxy.h"
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

	hub_name = config_name;
	hub_type = hub_type_;
	is_busy = false;
}

void hub_service::init() {
	enet_initialize();

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

	_io_service = std::make_shared<boost::asio::io_service>();

	if (enet_initialize() != 0)
	{
		spdlog::error("An error occurred while initializing ENet!");
	}
	auto ip = _config->get_value_string("ip");
	auto port = _config->get_value_int("port");
	_hub_service = std::make_shared<service::enetacceptservice>(ip, (short)port);

	_timerservice = std::make_shared<service::timerservice>();
	_close_handle = std::make_shared<closehandle>();
	_hubmng = std::make_shared<hubsvrmanager>(shared_from_this());
	_gatemng = std::make_shared<gatemanager>(_hub_service, shared_from_this());

	_center_msg_handle = std::make_shared<center_msg_handle>(shared_from_this());
	_dbproxy_msg_handle = std::make_shared<dbproxy_msg_handle>();
	_direct_client_msg_handle = std::make_shared<direct_client_msg_handle>(_gatemng, shared_from_this());
	_gate_msg_handle = std::make_shared<gate_msg_handle>(shared_from_this(), _gatemng);
	_hub_svr_msg_handle = std::make_shared<hub_svr_msg_handle>(shared_from_this(), _hubmng);
	
	_center_service = std::make_shared<service::connectservice>(_io_service);
	_dbproxy_service = std::make_shared<service::connectservice>(_io_service);

	if (_config->has_key("tcp_listen")) {
		auto is_tcp_listen = _config->get_value_bool("tcp_listen");
		if (is_tcp_listen) {
			auto tcp_outside_ip = _config->get_value_string("tcp_outside_ip");
			auto tcp_outside_port = (short)_config->get_value_int("tcp_outside_port");
			_client_tcp_service = std::make_shared<service::acceptservice>(tcp_outside_ip, tcp_outside_port, _io_service);
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
			auto websocket_outside_ip = _config->get_value_string("websocket_outside_ip");
			auto websocket_outside_port = (short)_config->get_value_int("websocket_outside_port");
			auto is_ssl = _config->get_value_bool("is_ssl");
			std::string certificate, private_key, tmp_dh;
			if (is_ssl) {
				auto certificate = _config->get_value_string("certificate");
				auto private_key = _config->get_value_string("private_key");
				auto tmp_dh = _config->get_value_string("tmp_dh");
			}
			_client_websocket_service = std::make_shared<service::webacceptservice>(websocket_outside_ip, websocket_outside_port, is_ssl, certificate, private_key, tmp_dh);
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

void hub_service::heartbeat(int64_t tick) {
	_centerproxy->heartbeat();
	_timerservice->addticktimer(3 * 1000, std::bind(&hub_service::heartbeat, this, std::placeholders::_1));
}

void hub_service::connect_center() {
	spdlog::trace("begin on connect center");

	auto ip = _center_config->get_value_string("ip");
	auto port = (short)(_center_config->get_value_int("port"));
	auto center_ch = _center_service->connect(ip, port);

	_centerproxy = std::make_shared<centerproxy>(center_ch);
	_centerproxy->reg_server(_config->get_value_string("ip"), (short)_config->get_value_int("port"), hub_name);

	heartbeat(_timerservice->Tick);

	spdlog::trace("end on connect center");
}

void hub_service::connect_gate(std::string uuid, std::string ip, uint16_t port) {
	_gatemng->connect_gate(uuid, ip, port);
}

void hub_service::reg_hub(std::string hub_ip, uint16_t hub_port) {
	_hub_service->connect(hub_ip, hub_port, [this, hub_ip, hub_port](std::shared_ptr<abelkhan::Ichannel> ch){
		spdlog::trace("hub ip:{0} port:{1} reg_hub", hub_ip, hub_port);
 		auto caller = std::make_shared<abelkhan::hub_call_hub_caller>(ch, service::_modulemng);
		caller->reg_hub(hub_name, hub_type)->callBack([hub_ip, hub_port]() {
			spdlog::trace("hub ip:{0} port:{1} reg_hub sucessed!", hub_ip, hub_port);
		}, [hub_ip, hub_port]() {
			spdlog::trace("hub ip:{0} port:{1} reg_hub faild!", hub_ip, hub_port);
		})->timeout(5 * 1000, [hub_ip, hub_port]() {
			spdlog::trace("hub ip:{0} port:{1} reg_hub timeout!", hub_ip, hub_port);
		});
	});
}

void hub_service::try_connect_db(std::string dbproxy_name, std::string dbproxy_ip, uint16_t dbproxy_port) {
	if (_config->has_key("dbproxy") && _config->get_value_string("dbproxy") == dbproxy_name) {
		auto dbproxy_cfg = _root_config->get_value_dict(dbproxy_name);
		auto _db_ip = dbproxy_cfg->get_value_string("ip");
		auto _db_port = (short)dbproxy_cfg->get_value_int("port");
		if (dbproxy_ip != _db_ip || dbproxy_port != _db_port) {
			return;
		}

		auto ch = _dbproxy_service->connect(_db_ip, _db_port);
		_dbproxyproxy = std::make_shared<dbproxyproxy>(ch);
		_dbproxyproxy->sig_dbproxy_init.connect([this]() {
			sig_dbproxy_init.emit();
		});
		_dbproxyproxy->reg_server(hub_name);
	}
	else if (_config->has_key("extend_dbproxy") && _config->get_value_string("extend_dbproxy") == dbproxy_name) {
		auto dbproxy_cfg = _root_config->get_value_dict(dbproxy_name);
		auto _db_ip = dbproxy_cfg->get_value_string("ip");
		auto _db_port = (short)dbproxy_cfg->get_value_int("port");
		if (dbproxy_ip != _db_ip || dbproxy_port != _db_port) {
			return;
		}

		auto ch = _dbproxy_service->connect(_db_ip, _db_port);
		_extend_dbproxyproxy = std::make_shared<dbproxyproxy>(ch);
		_extend_dbproxyproxy->sig_dbproxy_init.connect([this]() {
			sig_dbproxy_init.emit();
		});
		_extend_dbproxyproxy->reg_server(hub_name);
	}
	
}

void hub_service::close_svr() {
	_close_handle->is_closed = true;
	enet_deinitialize();
	spdlog::shutdown();
}

int hub_service::poll() {
	auto time_now = msec_time();

	try {
		_io_service->poll();

		_hub_service->poll();

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

	auto _tmp_now = msec_time();
	auto _tmp_time = _tmp_now - time_now;
	time_now = _tmp_now;

	return (int)_tmp_time;
}

}
