/*
 * logic_svr_msg_handle.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _logic_svr_msg_handle_h
#define _logic_svr_msg_handle_h

#include <caller/gate_call_logiccaller.h>
#include <caller/gate_call_clientcaller.h>

#include "logicsvrmanager.h"

void reg_logic(boost::shared_ptr<gate::logicsvrmanager> _logicsvrmanager, std::string uuid) {
	_logicsvrmanager->reg_channel(uuid, juggle::current_ch);

	boost::shared_ptr<caller::gate_call_logic> _caller = boost::make_shared<caller::gate_call_logic>(juggle::current_ch);
	_caller->reg_logic_sucess();

	std::cout << "logic server " << uuid << "connected" << std::endl;
}

void ack_client_connect_server(boost::shared_ptr<gate::clientmanager> _clientmanager, std::string uuid, std::string result) {
	if (_clientmanager->has_client(uuid)) {
		auto client_channel = _clientmanager->get_client_channel(uuid);

		boost::shared_ptr<caller::gate_call_client> _caller = boost::make_shared<caller::gate_call_client>(client_channel);
		_caller->ack_connect_server(result);
	}
}

void forward_logic_call_client(boost::shared_ptr<gate::clientmanager> _clientmanager, std::string uuid, std::string module, std::string func, std::string argv) {
	if (_clientmanager->has_client(uuid)) {
		auto client_channel = _clientmanager->get_client_channel(uuid);

		boost::shared_ptr<caller::gate_call_client> _caller = boost::make_shared<caller::gate_call_client>(client_channel);
		_caller->call_client(module, func, argv);
	}
}

#endif //_logic_svr_msg_handle_h
