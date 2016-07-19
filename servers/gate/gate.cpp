/*
 * qianqians
 * 2016-7-5
 * test_cpp_service_server.cpp
 */
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

#include <module/logic_call_gatemodule.h>
#include <module/client_call_gatemodule.h>
#include <module/center_call_servermodule.h>

#include "centerproxy.h"
#include "closehandle.h"
#include "clientmanager.h"
#include "logicsvrmanager.h"
#include "heartbeat_handle.h"
#include "center_msg_handle.h"
#include "logic_svr_msg_handle.h"
#include "client_msg_handle.h"

void main(int argc, char * argv[]) {
	auto svr_uuid = boost::lexical_cast<std::string>(boost::uuids::random_generator()());
	
	if (argc <= 1) {
		std::cout << "non input start argv" << std::endl;
		return;
	}

	std::string config_file_path = argv[1];
	auto _config = boost::make_shared<config::config>(config_file_path);
	auto _center_config = _config->get_value_dict("center");
	if (argc >= 3) {
		_config = _config->get_value_dict(argv[2]);
	}

	int64_t tick = clock();
	int64_t tickcount = 0;

	boost::shared_ptr<service::timerservice> _timerservice = boost::make_shared<service::timerservice>(tick);

	boost::shared_ptr<juggle::process> _center_process = boost::make_shared<juggle::process>();
	auto _connectnetworkservice = boost::make_shared<service::connectnetworkservice>(_center_process);
	auto center_ip = _center_config->get_value_string("ip");
	auto center_port = (short)_center_config->get_value_int("port");
	auto _center_ch = _connectnetworkservice->connect(center_ip, center_port);
	auto _centerproxy = boost::make_shared<gate::centerproxy>(_center_ch);
	auto inside_ip = _config->get_value_string("inside_ip");
	auto inside_port = (short)_config->get_value_int("inside_port");
	_centerproxy->reg_server(inside_ip, inside_port, svr_uuid);

	boost::shared_ptr<gate::closehandle> _closehandle = boost::make_shared<gate::closehandle>();
	auto _center_call_server = boost::make_shared<module::center_call_server>();
	_center_call_server->sigreg_server_sucesshandle.connect(boost::bind(&reg_server_sucess, _centerproxy));
	_center_call_server->sigclose_serverhandle.connect(boost::bind(&close_server, _closehandle));
	_center_process->reg_module(_center_call_server);

	auto _logic_process = boost::make_shared<juggle::process>();
	auto _logic_call_gate = boost::make_shared<module::logic_call_gate>();
	auto _logicsvrmanager = boost::make_shared<gate::logicsvrmanager>();
	auto _clientmanager = boost::make_shared<gate::clientmanager>();
	_logic_call_gate->sigreg_logichandle.connect(boost::bind(&reg_logic, _logicsvrmanager, _1));
	_logic_call_gate->sigack_client_connect_serverhandle.connect(boost::bind(&ack_client_connect_server, _logicsvrmanager, _clientmanager, _timerservice, _1, _2));
	_logic_call_gate->sigforward_logic_call_clienthandle.connect(boost::bind(&forward_logic_call_client, _clientmanager, _1, _2, _3, _4));
	_logic_call_gate->sigforward_logic_call_group_clienthandle.connect(boost::bind(&forward_logic_call_group_client, _clientmanager, _1, _2, _3, _4));
	_logic_call_gate->sigforward_logic_call_global_clienthandle.connect(boost::bind(&forward_logic_call_global_client, _clientmanager, _1, _2, _3));
	_logic_process->reg_module(_logic_call_gate);
	auto _logic_service = boost::make_shared<service::acceptnetworkservice>(inside_ip, inside_port, _logic_process);

	_timerservice->addticktime(tick + 5 * 60 * 1000, boost::bind(&heartbeat_handle, _clientmanager, _timerservice, _1));

	auto _client_process = boost::make_shared<juggle::process>();
	auto _client_call_gate = boost::make_shared<module::client_call_gate>();
	_client_call_gate->sigconnect_serverhandle.connect(boost::bind(&connect_server, _logicsvrmanager, _clientmanager, _timerservice, _1));
	_client_call_gate->sigcancle_serverhandle.connect(boost::bind(&cancle_server, _clientmanager));
	_client_call_gate->sigforward_client_call_logichandle.connect(boost::bind(&forward_client_call_logic, _clientmanager, _1, _2, _3));
	_client_call_gate->sigheartbeatshandle.connect(boost::bind(&heartbeats, _clientmanager, _timerservice));
	_client_process->reg_module(_client_call_gate);

	auto outside_ip = _config->get_value_string("outside_ip");
	auto outside_port = (short)_config->get_value_int("outside_port");
	auto _client_service = boost::make_shared<service::acceptnetworkservice>(outside_ip, outside_port, _logic_process);

	boost::shared_ptr<service::juggleservice> _juggleservice = boost::make_shared<service::juggleservice>();
	_juggleservice->add_process(_center_process);
	_juggleservice->add_process(_logic_process);
	_juggleservice->add_process(_client_process);
	
	while (true)
	{
		int64_t tmptick = (clock() & 0xffffffff);
		if (tmptick < tick)
		{
			tickcount += 1;
			tmptick = tmptick + tickcount * 0xffffffff;
		}
		tick = tmptick;

		_connectnetworkservice->poll(tick);
		_juggleservice->poll(tick);

		if (_centerproxy->is_reg_sucess) {
			_logic_service->poll(tick);
			_client_service->poll(tick);
			_timerservice->poll(tick);
		}

		if (_closehandle->is_closed) {
			std::cout << "server closed, gate server " << svr_uuid << std::endl;
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

		Sleep(15);

	}
}
