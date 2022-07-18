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
	client_msg_handle(std::shared_ptr<clientmanager> clientmanager_, std::shared_ptr<hubsvrmanager> hubsvrmanager_, std::shared_ptr<service::timerservice> timerservice_) {
		_clientmanager = clientmanager_;
		_hubsvrmanager = hubsvrmanager_;
		_timerservice = timerservice_;
		
		_client_call_gate_module = std::make_shared<abelkhan::client_call_gate_module>();
		_client_call_gate_module->Init(service::_modulemng);
		_client_call_gate_module->sig_heartbeats.connect(std::bind(&client_msg_handle::heartbeats, this));
		_client_call_gate_module->sig_get_hub_info.connect(std::bind(&client_msg_handle::get_hub_info, this, std::placeholders::_1));
		_client_call_gate_module->sig_forward_client_call_hub.connect(std::bind(&client_msg_handle::forward_client_call_hub, this, std::placeholders::_1, std::placeholders::_2));
	}

	void heartbeats() {
		auto rsp = std::static_pointer_cast<abelkhan::client_call_gate_heartbeats_rsp>(_client_call_gate_module->rsp);
		auto ch = _client_call_gate_module->current_ch;
		auto proxy = _clientmanager->get_client(ch);
		if (proxy != nullptr) {
			if (proxy->_theory_timetmp == 0) {
				proxy->_theory_timetmp = _timerservice->Tick;
			}
			else {
				proxy->_theory_timetmp += 5000;
			}
			proxy->_timetmp = _timerservice->Tick;
		}
		rsp->rsp(_timerservice->Tick);
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
			if (hubproxy_) {
				proxy->conn_hub(hubproxy_);
				hubproxy_->client_call_hub(proxy->_cuuid, rpc_argv);
			}
		}
	}
};

}

#endif //_client_msg_handle_h
