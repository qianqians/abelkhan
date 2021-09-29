/*
 * hub_svr_msg_handle.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _hub_svr_msg_handle_h
#define _hub_svr_msg_handle_h

#include <Imodule.h>

#include <gate_call_hubcaller.h>
#include <gate_call_clientcaller.h>

#include "hubsvrmanager.h"
#include "clientmanager.h"

void reg_hub(std::shared_ptr<gate::hubsvrmanager> _hubmanager, std::string uuid, std::string hub_name){
	auto _hub_channel = juggle::current_ch;
	_hubmanager->reg_hub(hub_name, _hub_channel);

	auto _hubproxy = std::make_shared<caller::gate_call_hub>(_hub_channel);
	_hubproxy->reg_hub_sucess();
}

void connect_sucess(std::shared_ptr<gate::clientmanager> _clientmanager, std::shared_ptr<gate::hubsvrmanager> _hubmanager, std::string client_uuid){
	if (!_clientmanager->has_client(client_uuid)) {
		return;
	}

	std::string _hub_name = _hubmanager->get_hub(juggle::current_ch);
	if (_hub_name == "") {
		return;
	}

	auto _client_channel = _clientmanager->get_client(client_uuid);
	auto _client_caller = std::make_shared<caller::gate_call_client>(_client_channel);
	_client_caller->connect_hub_sucess(_hub_name);
}

void disconnect_client(std::shared_ptr<gate::clientmanager> _clientmanager, std::string client_uuid){
	if (!_clientmanager->has_client(client_uuid)) {
		return;
	}

	auto _client_channel = _clientmanager->get_client(client_uuid);
	_clientmanager->unreg_client(_client_channel);
	_client_channel->disconnect();
}

void forward_hub_call_client(std::shared_ptr<gate::clientmanager> _clientmanager, std::string uuid, std::string module, std::string func, std::shared_ptr<std::vector<boost::any> > argv) {
	if (!_clientmanager->has_client(uuid)) {
		return;
	}

	auto client_channel = _clientmanager->get_client(uuid);
	std::shared_ptr<caller::gate_call_client> _caller = std::make_shared<caller::gate_call_client>(client_channel);
	_caller->call_client(module, func, argv);
}

void forward_hub_call_group_client(std::shared_ptr<gate::clientmanager> _clientmanager, std::shared_ptr<std::vector<boost::any> > uuids, std::string module, std::string func, std::shared_ptr<std::vector<boost::any> > argv) {
	for (auto uuid : *uuids) {
		if (!_clientmanager->has_client(boost::any_cast<std::string>(uuid))) {
			continue;
		}

		auto client_channel = _clientmanager->get_client(boost::any_cast<std::string>(uuid));

		std::shared_ptr<caller::gate_call_client> _caller = std::make_shared<caller::gate_call_client>(client_channel);
		_caller->call_client(module, func, argv);
	}
}

void forward_hub_call_global_client(std::shared_ptr<gate::clientmanager> _clientmanager, std::string module, std::string func, std::shared_ptr<std::vector<boost::any> > argv) {
	_clientmanager->for_each_client([module, func, argv](std::string client_uuid, std::shared_ptr<juggle::Ichannel> client_ch) {
		std::shared_ptr<caller::gate_call_client> _caller = std::make_shared<caller::gate_call_client>(client_ch);
		_caller->call_client(module, func, argv);
	});
}

#endif //_hub_svr_msg_handle_h
