/*
 * qianqians
 * 2016-7-5
 * gate.cpp
 */
#include <boost/uuid/uuid.hpp>
#include <boost/uuid/uuid_generators.hpp>
#include <boost/uuid/uuid_io.hpp>
#include <boost/lexical_cast.hpp>
#include <boost/thread.hpp>

#include <websocketacceptservice.h>
#include <acceptservice.h>
#include <connectservice.h>
#include <process_.h>
#include <Ichannel.h>
#include <Imodule.h>
#include <channel.h>
#include <juggleservice.h>
#include <timerservice.h>

#include <config.h>

#include <hub_call_gatemodule.h>
#include <client_call_gatemodule.h>
#include <center_call_servermodule.h>

#include "centerproxy.h"
#include "closehandle.h"
#include "clientmanager.h"
#include "hubsvrmanager.h"
#include "timer_handle.h"
#include "center_msg_handle.h"
#include "hub_svr_msg_handle.h"
#include "client_msg_handle.h"
#include "gc_poll.h"

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

	std::shared_ptr<service::timerservice> _timerservice = std::make_shared<service::timerservice>();

	auto inside_ip = _config->get_value_string("inside_ip");
	auto inside_port = (short)_config->get_value_int("inside_port");
	auto _hub_process = std::make_shared<juggle::process>();
	auto _hub_call_gate = std::make_shared<module::hub_call_gate>();
	auto _hubsvrmanager = std::make_shared<gate::hubsvrmanager>();
	auto _clientmanager = std::make_shared<gate::clientmanager>(_hubsvrmanager);
	_hub_call_gate->sig_reg_hub.connect(boost::bind(&reg_hub, _hubsvrmanager, _1, _2));
	_hub_call_gate->sig_connect_sucess.connect(boost::bind(&connect_sucess, _clientmanager, _hubsvrmanager, _1));
	_hub_call_gate->sig_disconnect_client.connect(boost::bind(&disconnect_client, _clientmanager, _1));
	_hub_call_gate->sig_forward_hub_call_client.connect(boost::bind(&forward_hub_call_client, _clientmanager, _1, _2, _3, _4));
	_hub_call_gate->sig_forward_hub_call_group_client.connect(boost::bind(&forward_hub_call_group_client, _clientmanager, _1, _2, _3, _4));
	_hub_call_gate->sig_forward_hub_call_global_client.connect(boost::bind(&forward_hub_call_global_client, _clientmanager, _1, _2, _3));
	_hub_process->reg_module(_hub_call_gate);
	auto _hub_service = std::make_shared<service::acceptservice>(inside_ip, inside_port, _hub_process);

	auto _client_process = std::make_shared<juggle::process>();
	auto _client_call_gate = std::make_shared<module::client_call_gate>();
	_client_call_gate->sig_connect_server.connect(boost::bind(&connect_server, _hubsvrmanager, _clientmanager, _timerservice, _1, _2));
	_client_call_gate->sig_cancle_server.connect(boost::bind(&cancle_server, _clientmanager));
	_client_call_gate->sig_connect_hub.connect(boost::bind(&connect_hub, _hubsvrmanager, _clientmanager, _1));
	_client_call_gate->sig_enable_heartbeats.connect(boost::bind(&enable_heartbeats, _clientmanager));
	_client_call_gate->sig_disable_heartbeats.connect(boost::bind(&disable_heartbeats, _clientmanager));
	_client_call_gate->sig_heartbeats.connect(boost::bind(&heartbeats, _clientmanager, _timerservice, _1));
	_client_call_gate->sig_forward_client_call_hub.connect(boost::bind(&forward_client_call_hub, _clientmanager, _hubsvrmanager, _1, _2, _3, _4));
	_client_process->reg_module(_client_call_gate);
	auto outside_ip = _config->get_value_string("outside_ip");
	auto outside_port = (short)_config->get_value_int("outside_port");
	auto _client_service = std::make_shared<service::webacceptservice>(outside_ip, outside_port, _client_process);
	_client_service->sigchannelconnect.connect([](std::shared_ptr<juggle::Ichannel> ch) {
		auto uuid = boost::lexical_cast<std::string>(boost::uuids::random_generator()());

		auto _client_proxy = std::make_shared<caller::gate_call_client>(ch);
		_client_proxy->ntf_uuid(uuid);
	});
	_client_service->sigchanneldisconnect.connect([_clientmanager](std::shared_ptr<juggle::Ichannel> ch) {
		gate::gc_put([_clientmanager, ch]() {
			_clientmanager->unreg_client(ch);
		});
	});

	std::shared_ptr<juggle::process> _center_process = std::make_shared<juggle::process>();
	auto _connectnetworkservice = std::make_shared<service::connectservice>(_center_process);
	auto center_ip = _center_config->get_value_string("ip");
	auto center_port = (short)_center_config->get_value_int("port");
	auto _center_ch = _connectnetworkservice->connect(center_ip, center_port);
	auto _centerproxy = std::make_shared<gate::centerproxy>(_center_ch);
	std::shared_ptr<gate::closehandle> _closehandle = std::make_shared<gate::closehandle>();
	auto _center_call_server = std::make_shared<module::center_call_server>();
	_center_call_server->sig_reg_server_sucess.connect(std::bind(&reg_server_sucess, _centerproxy));
	_center_call_server->sig_close_server.connect(std::bind(&close_server, _closehandle));
	_center_process->reg_module(_center_call_server);
	_centerproxy->reg_server(inside_ip, inside_port, svr_uuid);

	std::shared_ptr<service::juggleservice> _juggleservice = std::make_shared<service::juggleservice>();
	_juggleservice->add_process(_center_process);
	_juggleservice->add_process(_hub_process);
	_juggleservice->add_process(_client_process);

	_timerservice->addticktimer(5 * 1000, std::bind(&heartbeat_handle, _clientmanager, _timerservice, std::placeholders::_1));

	while (true){
		clock_t begin = clock();
		try {
			_connectnetworkservice->poll();
			_hub_service->poll();
			_client_service->poll();

			_juggleservice->poll();

			_timerservice->poll();
		}
		catch(std::exception e) {
			std::cout << e.what() << std::endl;
		}

		try {
			_client_service->gc_poll();
			gate::gc_poll();
		}
		catch (std::exception e) {
			std::cout << e.what() << std::endl;
		}

		if (_closehandle->is_closed) {
			std::cout << "server closed, gate server " << svr_uuid << std::endl;
			break;
		}
		
		if ((clock() - begin) < 50){
			boost::this_thread::sleep(boost::get_system_time() + boost::posix_time::microseconds(5));
		}
	}
}
