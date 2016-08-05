/*
 * qianqians
 * 2016-7-5
 * dbproxy.cpp
 */
#include <boost/thread.hpp>
#include <boost/uuid/uuid.hpp>
#include <boost/uuid/uuid_generators.hpp>
#include <boost/uuid/uuid_io.hpp>
#include <boost/lexical_cast.hpp>

#include <acceptnetworkservice.h>
#include <connectnetworkservice.h>
#include <process_.h>
#include <Ichannel.h>
#include <Imodule.h>
#include <channel.h>
#include <juggleservice.h>
#include <timerservice.h>

#include <config.h>

#include <module/logic_call_dbproxymodule.h>
#include <module/center_call_servermodule.h>
#include <module/hub_call_dbproxymodule.h>

#include "center_msg_handle.h"
#include "logic_svr_msg_handle.h"
#include "hub_svr_msg_handle.h"
#include "centerproxy.h"
#include "logicsvrmanager.h"
#include "hubsvrmanager.h"
#include "mongodb_proxy.h"
#include "closehandle.h"

void main(int argc, char * argv[]) {
	auto svr_uuid = boost::lexical_cast<std::string>(boost::uuids::random_generator()());

	if (argc <= 1) {
		std::cout << "non input start argv" << std::endl;
		return;
	}

	std::string config_file_path = argv[1];
	auto _config = std::make_shared<config::config>(config_file_path);
	auto _center_config = _config->get_value_dict("center");
	if (argc >= 3) {
		_config = _config->get_value_dict(argv[2]);
	}

	std::shared_ptr<juggle::process> _center_process = std::make_shared<juggle::process>();
	std::shared_ptr<service::connectnetworkservice> _connectnetworkservice = std::make_shared<service::connectnetworkservice>(_center_process);

	auto center_ip = _center_config->get_value_string("ip");
	auto center_port = (short)_center_config->get_value_int("port");
	auto _center_ch = _connectnetworkservice->connect(center_ip, center_port);
	std::shared_ptr<dbproxy::centerproxy> _centerproxy = std::make_shared<dbproxy::centerproxy>(_center_ch);
	
	auto ip = _config->get_value_string("ip");
	auto port = (short)_config->get_value_int("port");
	_centerproxy->reg_server(ip, port, svr_uuid);

	std::shared_ptr<dbproxy::closehandle> _closehandle = std::make_shared<dbproxy::closehandle>();
	std::shared_ptr<module::center_call_server> _center_call_server = std::make_shared<module::center_call_server>();
	_center_call_server->sigclose_serverhandle.connect(boost::bind(&close_server, _closehandle));
	_center_call_server->sigreg_server_sucesshandle.connect(boost::bind(&reg_server_sucess, _centerproxy));
	_center_process->reg_module(_center_call_server);

	auto db_ip = _config->get_value_string("db_ip");
	auto db_port = (short)_config->get_value_int("db_port");
	auto db_name = _config->get_value_string("db_name");
	auto db_collection = _config->get_value_string("db_collection");
	auto _mongodb_proxy = std::make_shared<dbproxy::mongodb_proxy>(db_ip, db_port, db_name, db_collection);

	std::shared_ptr<juggle::process> _dbthings_process = std::make_shared<juggle::process>();
	std::shared_ptr<module::logic_call_dbproxy> _logic_call_dbproxy = std::make_shared<module::logic_call_dbproxy>();
	std::shared_ptr<dbproxy::logicsvrmanager> _logicsvrmanager = std::make_shared<dbproxy::logicsvrmanager>();
	_logic_call_dbproxy->sigreg_logichandle.connect(std::bind(&reg_logic, _logicsvrmanager, _closehandle, std::placeholders::_1));
	_logic_call_dbproxy->siglogic_closedhandle.connect(std::bind(&logic_closed, _closehandle));
	_logic_call_dbproxy->sigcreate_persisted_objecthandle.connect(std::bind(&logic_create_persisted_object, _logicsvrmanager, _mongodb_proxy, std::placeholders::_1, std::placeholders::_2));
	_logic_call_dbproxy->sigupdata_persisted_objecthandle.connect(std::bind(&logic_updata_persisted_object, _logicsvrmanager, _mongodb_proxy, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3));
	_logic_call_dbproxy->sigget_object_infohandle.connect(std::bind(&logic_get_object_info, _logicsvrmanager, _mongodb_proxy, std::placeholders::_1, std::placeholders::_2));
	_dbthings_process->reg_module(_logic_call_dbproxy);

	std::shared_ptr<module::hub_call_dbproxy> _hub_call_dbproxy = std::make_shared<module::hub_call_dbproxy>();
	std::shared_ptr<dbproxy::hubsvrmanager> _hubsvrmanager = std::make_shared<dbproxy::hubsvrmanager>();
	_hub_call_dbproxy->sigreg_hubhandle.connect(std::bind(&reg_hub, _hubsvrmanager, std::placeholders::_1));
	_hub_call_dbproxy->sigcreate_persisted_objecthandle.connect(std::bind(&hub_create_persisted_object, _hubsvrmanager, _mongodb_proxy, std::placeholders::_1, std::placeholders::_2));
	_hub_call_dbproxy->sigupdata_persisted_objecthandle.connect(std::bind(&hub_updata_persisted_object, _hubsvrmanager, _mongodb_proxy, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3));
	_hub_call_dbproxy->sigget_object_infohandle.connect(std::bind(&hub_get_object_info, _hubsvrmanager, _mongodb_proxy, std::placeholders::_1, std::placeholders::_2));
	_dbthings_process->reg_module(_hub_call_dbproxy);

	auto _acceptnetworkservice = std::make_shared<service::acceptnetworkservice>(ip, port, _dbthings_process);

	std::shared_ptr<service::juggleservice> _juggleservice = std::make_shared<service::juggleservice>();
	_juggleservice->add_process(_center_process);
	_juggleservice->add_process(_dbthings_process);

	int64_t tick = clock();
	int64_t tickcount = 0;

	std::shared_ptr<service::timerservice> _timerservice = std::make_shared<service::timerservice>(tick);

	while (true)
	{
		int64_t tmptick = (clock() & 0xffffffff);
		if (tmptick < tick)
		{
			tickcount += 1;
			tmptick = tmptick + tickcount * 0xffffffff;
		}
		tick = tmptick;

		if (_centerproxy->is_reg_sucess) {
			_acceptnetworkservice->poll(tick);
		}

		_connectnetworkservice->poll(tick);

		_juggleservice->poll(tick);
		_timerservice->poll(tick);

		if (_closehandle->is_close()) {
			std::cout << "server closed, dbproxy server " << svr_uuid << std::endl;
			break;
		}

		tmptick = (clock() & 0xffffffff);
		if (tmptick < tick)
		{
			tickcount += 1;
			tmptick = tmptick + tickcount * 0xffffffff;
		}
		int64_t ticktime = (tmptick - tick);
		tick = tmptick;

		if (ticktime < 50) {
			boost::thread::sleep(boost::get_system_time() + boost::posix_time::microseconds(15));
		}
	}
}
