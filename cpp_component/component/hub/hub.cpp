/*
 * qianqians
 * 2020-1-10
 * hub.cpp
 */

#include <angmalloc.h>

#include <boost/uuid/uuid.hpp>
#include <boost/uuid/uuid_generators.hpp>
#include <boost/uuid/uuid_io.hpp>
#include <boost/lexical_cast.hpp>
#include <boost/thread.hpp>

#include <enetacceptservice.h>
#include <enetconnectservice.h>
#include <connectservice.h>
#include <websocketacceptservice.h>

#include "hub_call_hubmodule.h"
#include "center_call_hubmodule.h"
#include "center_call_servermodule.h"
#include "gate_call_hubmodule.h"
#include "client_call_hubmodule.h"
#include "dbproxy_call_hubmodule.h"

#include "hub.h"
#include "centerproxy.h"
#include "center_msg_handle.h"
#include "dbproxyproxy.h"
#include "dbproxy_msg_handle.h"
#include "hubsvrmanager.h"
#include "hub_svr_msg_handle.h"
#include "gatemanager.h"
#include "gate_msg_handle.h"
#include "direct_client_msg_handle.h"
#include "gc_poll.h"

namespace _log {

std::shared_ptr<spdlog::logger> file_logger = nullptr;
}

namespace hub{

hub_service::hub_service(std::string config_file_path, std::string config_name) {
	uuid = boost::lexical_cast<std::string>(boost::uuids::random_generator()());

	_config = Fossilizid::pool::factory::create<config::config>(config_file_path);
	_center_config = _config->get_value_dict("center");
	_root_config = _config;
	_config = _config->get_value_dict(config_name);

	name = _config->get_value_string("hub_name");
	is_busy = false;
}

void hub_service::init() {
	enet_initialize();

	auto file_path = _config->get_value_string("log_dir") + _config->get_value_string("log_file");
	auto log_level = _config->get_value_string("log_level");
	if (log_level == "trace") {
		_log::InitLog(file_path, spdlog::level::level_enum::trace);
	}
	else if (log_level == "error") {
		_log::InitLog(file_path, spdlog::level::level_enum::err);
	}

	_timerservice = Fossilizid::pool::factory::create<service::timerservice>();
	close_handle = Fossilizid::pool::factory::create<closehandle>();
	hubs = Fossilizid::pool::factory::create<hubsvrmanager>(shared_from_this());

	auto ip = _config->get_value_string("ip");
	auto port = _config->get_value_int("port");
	auto hub_call_hub = Fossilizid::pool::factory::create<module::hub_call_hub>();
	hub_call_hub->sig_reg_hub.connect(std::bind(hub_msg::reg_hub, hubs, std::placeholders::_1));
	hub_call_hub->sig_reg_hub_sucess.connect(std::bind(hub_msg::reg_hub_sucess));
	hub_call_hub->sig_hub_call_hub_mothed.connect(std::bind(hub_msg::hub_call_hub_mothed, shared_from_this(), std::placeholders::_1, std::placeholders::_2, std::placeholders::_3));
	auto hub_process = Fossilizid::pool::factory::create<juggle::process>();
	hub_process->reg_module(hub_call_hub);
	_hub_service = Fossilizid::pool::factory::create<service::enetacceptservice>(ip, (short)port, hub_process);

	_center_process = Fossilizid::pool::factory::create<juggle::process>();
	_center_service = Fossilizid::pool::factory::create<service::connectservice>(_center_process);

	auto gate_process = Fossilizid::pool::factory::create<juggle::process>();
	_gate_service = Fossilizid::pool::factory::create<service::enetconnectservice>(gate_process);
	gates = Fossilizid::pool::factory::create<gatemanager>(_gate_service, shared_from_this());
	auto gate_call_hub = Fossilizid::pool::factory::create<module::gate_call_hub>();
	gate_call_hub->sig_reg_hub_sucess.connect(std::bind(gate_msg::reg_hub_sucess));
	gate_call_hub->sig_client_connect.connect(std::bind(gate_msg::client_connect, gates, std::placeholders::_1));
	gate_call_hub->sig_client_disconnect.connect(std::bind(gate_msg::client_disconnect, gates, std::placeholders::_1));
	gate_call_hub->sig_client_exception.connect(std::bind(gate_msg::client_exception, gates, std::placeholders::_1));
	gate_call_hub->sig_client_call_hub.connect(std::bind(gate_msg::client_call_hub, shared_from_this(), std::placeholders::_1, std::placeholders::_2, std::placeholders::_3, std::placeholders::_4));
	gate_process->reg_module(gate_call_hub);

	_juggleservice = Fossilizid::pool::factory::create<service::juggleservice>();

	if (_config->has_key("host") && _config->has_key("out_port")) {
		auto client_call_hub = Fossilizid::pool::factory::create<module::client_call_hub>();
		client_call_hub->sig_client_connect.connect(std::bind(client_msg::client_connect, gates, std::placeholders::_1));
		client_call_hub->sig_call_hub.connect(std::bind(client_msg::call_hub, shared_from_this(), std::placeholders::_1, std::placeholders::_2, std::placeholders::_3, std::placeholders::_4));
		auto direct_client_process = Fossilizid::pool::factory::create<juggle::process>();
		direct_client_process->reg_module(client_call_hub);
		_client_service = Fossilizid::pool::factory::create<service::webacceptservice>(
			_config->get_value_string("host"), (short)_config->get_value_int("out_port"), 
			_config->get_value_bool("is_ssl"),
			_config->get_value_string("certificate"), _config->get_value_string("private_key"), _config->get_value_string("tmp_dh"),
			direct_client_process);
		_client_service->sigchanneldisconnect.connect([this](std::shared_ptr<juggle::Ichannel> ch) {
			gates->client_direct_disconnect(ch);
		});
		_client_service->sigchannelconnectexception.connect([this](std::shared_ptr<juggle::Ichannel> ch) {
			gates->client_direct_exception(ch);
		});

		_juggleservice->add_process(direct_client_process);
	}

	_juggleservice->add_process(hub_process);
	_juggleservice->add_process(_center_process);
	_juggleservice->add_process(gate_process);
}

void hub_service::connect_center() {
	spdlog::trace("begin on connect center");

	auto ip = _center_config->get_value_string("ip");
	auto port = (short)(_center_config->get_value_int("port"));
	auto center_ch = _center_service->connect(ip, port);

	_centerproxy = Fossilizid::pool::factory::create<centerproxy>(center_ch);

	auto center_call_hub = Fossilizid::pool::factory::create<module::center_call_hub>();
	auto center_call_server = Fossilizid::pool::factory::create<module::center_call_server>();
	center_call_server->sig_reg_server_sucess.connect(std::bind(center_msg::reg_server_sucess, _centerproxy));
	center_call_server->sig_close_server.connect(std::bind(center_msg::close_server, close_handle));
	center_call_hub->sig_distribute_server_address.connect(std::bind(center_msg::distribute_server_address, shared_from_this(), std::placeholders::_1, std::placeholders::_2, std::placeholders::_3, std::placeholders::_4));
	center_call_hub->sig_reload.connect(std::bind(center_msg::reload, shared_from_this(), std::placeholders::_1));
	_center_process->reg_module(center_call_hub);
	_center_process->reg_module(center_call_server);

	_centerproxy->reg_server(_config->get_value_string("ip"), (short)_config->get_value_int("port"), uuid);

	spdlog::trace("end on connect center");
}

void hub_service::connect_gate(std::string uuid, std::string ip, uint16_t port) {
	gates->connect_gate(uuid, ip, port);
}

void hub_service::reg_hub(std::string hub_ip, uint16_t hub_port) {
	_hub_service->connect(hub_ip, hub_port, [this, hub_ip, hub_port](std::shared_ptr<juggle::Ichannel> ch){
		spdlog::trace("hub ip:{0} port:{1} reg_hub", hub_ip, hub_port);
 		auto caller = Fossilizid::pool::factory::create< caller::hub_call_hub>(ch);
		caller->reg_hub(name);
	});
}

void hub_service::try_connect_db(std::string dbproxy_ip, uint16_t dbproxy_port) {
	if (!_config->has_key("dbproxy")) {
		return;
	}

	auto dbproxy_cfg = _root_config->get_value_dict(_config->get_value_string("dbproxy"));
	auto _db_ip = dbproxy_cfg->get_value_string("ip");
	auto _db_port = (short)dbproxy_cfg->get_value_int("port");
	if (dbproxy_ip != _db_ip || dbproxy_port != _db_port) {
		return;
	}

	auto dbproxy_process = Fossilizid::pool::factory::create<juggle::process>();
	_dbproxy_service = Fossilizid::pool::factory::create<service::connectservice>(dbproxy_process);
	auto ch = _dbproxy_service->connect(_db_ip, _db_port);
	_dbproxyproxy = Fossilizid::pool::factory::create<dbproxyproxy>(ch);

	auto dbproxy_call_hub = Fossilizid::pool::factory::create<module::dbproxy_call_hub>();
	dbproxy_call_hub->sig_reg_hub_sucess.connect(std::bind(db_msg::reg_hub_sucess));
	dbproxy_call_hub->sig_ack_create_persisted_object.connect(std::bind(db_msg::ack_create_persisted_object, _dbproxyproxy, std::placeholders::_1, std::placeholders::_2));
	dbproxy_call_hub->sig_ack_updata_persisted_object.connect(std::bind(db_msg::ack_updata_persisted_objec, _dbproxyproxy, std::placeholders::_1));
	dbproxy_call_hub->sig_ack_get_object_count.connect(std::bind(db_msg::ack_get_object_count, _dbproxyproxy, std::placeholders::_1, std::placeholders::_2));
	dbproxy_call_hub->sig_ack_get_object_info.connect(std::bind(db_msg::ack_get_object_info, _dbproxyproxy, std::placeholders::_1, std::placeholders::_2));
	dbproxy_call_hub->sig_ack_get_object_info_end.connect(std::bind(db_msg::ack_get_object_info_end, _dbproxyproxy, std::placeholders::_1));
	dbproxy_call_hub->sig_ack_remove_object.connect(std::bind(db_msg::ack_remove_object, _dbproxyproxy, std::placeholders::_1));

	dbproxy_process->reg_module(dbproxy_call_hub);
	_juggleservice->add_process(dbproxy_process);
}

void hub_service::poll() {
	auto time_now = msec_time();
	while (1) {
		try {
			_hub_service->poll();
			_center_service->poll();
			_gate_service->poll();

			if (_dbproxy_service) {
				_dbproxy_service->poll();
			}

			_timerservice->poll();
			_juggleservice->poll();

			service::gc_poll();

			_log::file_logger->flush();
		}
		catch (std::exception err) {
			spdlog::error("hub_service::poll error:{0}", err.what());
		}

		if (close_handle->is_closed) {
			_centerproxy->closed();
			_timerservice->addticktimer(2000, [](uint64_t timetmp) {
				spdlog::shutdown();
				exit(0);
			});
		}
		else {
			auto _tmp_now = msec_time();
			auto _tmp_time = _tmp_now - time_now;
			time_now = _tmp_now;
			if (_tmp_time < 15) {
				Sleep(0);
				is_busy = false;
			}
			else {
				is_busy = true;
			}
		}
	}
}

}
