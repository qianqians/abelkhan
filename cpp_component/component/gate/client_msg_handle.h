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
	clientmanager* _clientmanager;
	hubsvrmanager* _hubsvrmanager;
	service::timerservice* _timerservice;

	std::shared_ptr<abelkhan::client_call_gate_module> _client_call_gate_module;

public:
	client_msg_handle(clientmanager* clientmanager_, hubsvrmanager* hubsvrmanager_, service::timerservice* timerservice_) {
		_clientmanager = clientmanager_;
		_hubsvrmanager = hubsvrmanager_;
		_timerservice = timerservice_;
		
		_client_call_gate_module = std::make_shared<abelkhan::client_call_gate_module>();
		_client_call_gate_module->Init(service::_modulemng);
		_client_call_gate_module->sig_heartbeats.connect(std::bind(&client_msg_handle::heartbeats, this));
		_client_call_gate_module->sig_get_hub_info.connect(std::bind(&client_msg_handle::get_hub_info, this, std::placeholders::_1));
		_client_call_gate_module->sig_forward_client_call_hub.connect(std::bind(&client_msg_handle::forward_client_call_hub, this, std::placeholders::_1, std::placeholders::_2));
		_client_call_gate_module->sig_migrate_client_confirm.connect(std::bind(&client_msg_handle::migrate_client_confirm, this, std::placeholders::_1, std::placeholders::_2));
	}

	void migrate_client_confirm(std::string src_hub, std::string _target_hub) {
		auto ch = _client_call_gate_module->current_ch;
		auto proxy = _clientmanager->get_client(ch);
		if (proxy != nullptr) {
			auto hubproxy_ = _hubsvrmanager->get_hub(src_hub);
			if (hubproxy_) {
				hubproxy_->migrate_client(proxy->_cuuid, _target_hub);
			}
		}
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

		auto ch = _client_call_gate_module->current_ch;
		auto proxy = _clientmanager->get_client(ch);
		abelkhan::hub_info _info;
		auto relust = _hubsvrmanager->get_hub_list(proxy->_cuuid, hub_type, _info);
		if (relust) {
			rsp->rsp(_info);
		}
		else {
			rsp->err();
		}
	}

	void forward_client_call_hub(std::string hub_name, std::vector<uint8_t> rpc_argv) {
		auto ch = _client_call_gate_module->current_ch;
		auto proxy = _clientmanager->get_client(ch);
		if (proxy != nullptr) {
			auto hubproxy_ = _hubsvrmanager->get_hub(hub_name);
			if (hubproxy_) {
				proxy->conn_hub(hubproxy_);
				hubproxy_->client_call_hub(proxy->_cuuid, rpc_argv);

				if (hubproxy_->_tick_time > 100)
				{
					auto r = _hubsvrmanager->rand() % 100;
					if (r < 20)
					{
						auto target_hub = _hubsvrmanager->hash_hubproxy(proxy->_cuuid, hubproxy_->_hub_type);
						auto target_hubproxy_ = _hubsvrmanager->get_hub(target_hub);
						if (target_hub != hub_name && target_hubproxy_->_tick_time <= 50)
						{
							proxy->migrate_client_start(hub_name, target_hub);
						}
					}
				}
			}
			else {

			}
		}
	}
};

}

#endif //_client_msg_handle_h
