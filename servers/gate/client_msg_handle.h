/*
 * client_msg_handle.h
 *
 *  Created on: 2016-7-12
 *      Author: qianqians
 */
#ifndef _client_msg_handle_h
#define _client_msg_handle_h

#include <caller/gate_call_logiccaller.h>

#include "logicsvrmanager.h"
#include "clientmanager.h"

void connect_server(std::shared_ptr<gate::logicsvrmanager> _logicsvrmanager, std::shared_ptr<gate::clientmanager> _clientmanager, std::shared_ptr<service::timerservice> _timerservice, std::string uuid) {
	std::shared_ptr<juggle::Ichannel> logic_channel = _logicsvrmanager->get_logic();

	if (!_clientmanager->has_client(uuid)) {
		std::cout << "client " << uuid << " connected" << std::endl;

		_clientmanager->reg_client(uuid, juggle::current_ch, logic_channel, _timerservice->ticktime);
	
		std::shared_ptr<caller::gate_call_logic> _caller = std::make_shared<caller::gate_call_logic>(logic_channel);
		_caller->client_connect(uuid);
	}
}

void cancle_server(std::shared_ptr<gate::clientmanager> _clientmanager) {
	if (_clientmanager->has_client(juggle::current_ch)) {
		std::string uuid = _clientmanager->get_client_uuid(juggle::current_ch);

		auto _logic = _clientmanager->get_logic_channel(uuid);

		std::shared_ptr<caller::gate_call_logic> _caller = std::make_shared<caller::gate_call_logic>(_logic);
		_caller->client_disconnect(uuid);
	}
}

void forward_client_call_logic(std::shared_ptr<gate::clientmanager> _clientmanager, std::string module_name, std::string func_name, std::shared_ptr<std::vector<boost::any> > argv) {
	if (_clientmanager->has_client(juggle::current_ch)) {
		std::string uuid = _clientmanager->get_client_uuid(juggle::current_ch);
		auto _logic = _clientmanager->get_logic_channel(uuid);

		std::shared_ptr<caller::gate_call_logic> _caller = std::make_shared<caller::gate_call_logic>(_logic);
		_caller->client_call_logic(uuid, module_name, func_name, argv);
	}
}

void heartbeats(std::shared_ptr<gate::clientmanager> _clientmanager, std::shared_ptr<service::timerservice> _timerservice) {
	if (_clientmanager->has_client(juggle::current_ch)) {
		_clientmanager->refresh_client(juggle::current_ch, _timerservice->ticktime);
	}
}

#endif //_client_msg_handle_h
