/*
 * client_msg_handle.h
 *
 *  Created on: 2016-7-12
 *      Author: qianqians
 */
#ifndef _client_msg_handle_h
#define _client_msg_handle_h

#include <gate.h>

#include <timerservice.h>
#include <modulemng_handle.h>

#include "hubsvrmanager.h"
#include "clientmanager.h"

namespace gate {

class client_msg_handle {
private:
	std::shared_ptr<clientmanager> _clientmanager;
	std::shared_ptr<hubsvrmanager> _hubsvrmanager;
	std::shared_ptr<service::timerservice> _timerservice;
	std::shared_ptr<abelkhan::client_call_gate_module> _client_call_gate_module;

public:
	client_msg_handle() {
		_client_call_gate_module = std::make_shared<abelkhan::client_call_gate_module>();
		_client_call_gate_module->Init(service::_modulemng);
		_client_call_gate_module->sig_heartbeats.connect(std::bind(&client_msg_handle::heartbeats, this));
		_client_call_gate_module->sig_get_hub_info.connect(std::bind(&client_msg_handle::get_hub_info, this, std::placeholders::_1));
		_client_call_gate_module->sig_forward_client_call_hub;
	}

	void heartbeats() {
		auto ch = _client_call_gate_module->current_ch;
		auto proxy = _clientmanager->get_client(ch);
		if (proxy != nullptr) {
			proxy->_timetmp = _timerservice->Tick;
		}
	}

	void get_hub_info(std::string hub_type) {
		auto rsp = std::static_pointer_cast<abelkhan::client_call_gate_get_hub_info_rsp>(_client_call_gate_module->rsp);
		auto list = _hubsvrmanager->get_hub_list(hub_type);
		rsp->rsp(list);
	}

	void forward_client_call_hub(std::string hub_name, std::vector<uint8_t> rpc_argv) {
		auto ch = _client_call_gate_module->current_ch;
		auto proxy = _clientmanager->get_client(ch);
		if (proxy != nullptr) {
			auto hubproxy_ = _hubsvrmanager->get_hub(hub_name);
			hubproxy_->client_call_hub(proxy->_cuuid, rpc_argv);
		}
	}
};

}

#endif //_client_msg_handle_h
