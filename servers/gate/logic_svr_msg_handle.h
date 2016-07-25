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

	std::cout << "logic server " << uuid << " connected" << std::endl;
}

void ack_client_connect_server(boost::shared_ptr<gate::logicsvrmanager> _logicsvrmanager, boost::shared_ptr<gate::clientmanager> _clientmanager, boost::shared_ptr<service::timerservice> _timerservice, std::string uuid, std::string result) {
	if (_clientmanager->has_client(uuid)) {
		auto client_channel = _clientmanager->get_client_channel(uuid);

		if (result != "svr_is_busy")
		{
			boost::shared_ptr<caller::gate_call_client> _caller = boost::make_shared<caller::gate_call_client>(client_channel);
			_caller->ack_connect_server(result);
		}
		else {
			boost::shared_ptr<juggle::Ichannel> logic_channel = _logicsvrmanager->get_logic();

			_clientmanager->reg_client(uuid, juggle::current_ch, logic_channel, _timerservice->ticktime);

			boost::shared_ptr<caller::gate_call_logic> _caller = boost::make_shared<caller::gate_call_logic>(logic_channel);
			_caller->client_connect(uuid);
		}
	}
}

void forward_logic_call_client(boost::shared_ptr<gate::clientmanager> _clientmanager, std::string uuid, std::string module, std::string func, boost::shared_ptr<std::vector<boost::any> > argv) {
	if (_clientmanager->has_client(uuid)) {
		auto client_channel = _clientmanager->get_client_channel(uuid);

		boost::shared_ptr<caller::gate_call_client> _caller = boost::make_shared<caller::gate_call_client>(client_channel);
		_caller->call_client(module, func, argv);
	}
}

void forward_logic_call_group_client(boost::shared_ptr<gate::clientmanager> _clientmanager, boost::shared_ptr<std::vector<boost::any> > uuids, std::string module, std::string func, boost::shared_ptr<std::vector<boost::any> > argv) {
	for (auto uuid : *uuids) {
		if (_clientmanager->has_client(boost::any_cast<std::string>(uuid))) {
			auto client_channel = _clientmanager->get_client_channel(boost::any_cast<std::string>(uuid));

			boost::shared_ptr<caller::gate_call_client> _caller = boost::make_shared<caller::gate_call_client>(client_channel);
			_caller->call_client(module, func, argv);
		}
	}
}

void forward_logic_call_global_client(boost::shared_ptr<gate::clientmanager> _clientmanager, std::string module, std::string func, boost::shared_ptr<std::vector<boost::any> > argv) {
	_clientmanager->for_each_client([module, func, argv](std::string client_uuid, boost::shared_ptr<juggle::Ichannel> client_ch, boost::shared_ptr<juggle::Ichannel> logic_ch) {
		boost::shared_ptr<caller::gate_call_client> _caller = boost::make_shared<caller::gate_call_client>(client_ch);
		_caller->call_client(module, func, argv);
	});
}

#endif //_logic_svr_msg_handle_h
