/*
 * gate_msg_handle.h
 *
 *  Created on: 2020-1-10
 *      Author: qianqians
 */
#ifndef _client_msg_handle_h
#define _client_msg_handle_h

#include <string>

#include <modulemng_handle.h>

#include <module.h>
#include <log.h>

#include "hub_service.h"
#include "gatemanager.h"

namespace hub{

class direct_client_msg_handle {
private:
	std::shared_ptr<abelkhan::client_call_hub_module> _client_call_hub_module;
	
	std::shared_ptr<hub::gatemanager> _gates;
	std::shared_ptr<hub::hub_service> _hub;

public:
	direct_client_msg_handle(std::shared_ptr<hub::gatemanager> gates_, std::shared_ptr<hub::hub_service> hub_) {
		_gates = gates_;
		_hub = hub_;

		_client_call_hub_module = std::make_shared<abelkhan::client_call_hub_module>();
		_client_call_hub_module->Init(service::_modulemng);
		_client_call_hub_module->sig_connect_hub.connect(std::bind(&direct_client_msg_handle::client_connect, this, std::placeholders::_1));
		_client_call_hub_module->sig_heartbeats.connect(std::bind(&direct_client_msg_handle::heartbeats, this));
		_client_call_hub_module->sig_call_hub.connect(std::bind(&direct_client_msg_handle::call_hub, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3));
	}

	void client_connect(std::string cuuid) {
		_gates->client_direct_connect(cuuid, _client_call_hub_module->current_ch);
	}

	void heartbeats() {
		auto _proxy = _gates->get_direct_client(_client_call_hub_module->current_ch);
		if (_proxy == nullptr) {
			spdlog::trace("heartbeats _proxy is nullptr!");
			return;
		}

		if (_proxy != nullptr) {
			if (_proxy->_theory_timetmp == 0) {
				_proxy->_theory_timetmp = _hub->_timerservice->Tick;
			}
			else {
				_proxy->_theory_timetmp += 5 * 1000;
			}
			_proxy->_timetmp = _hub->_timerservice->Tick;
		}
	}

	void call_hub(std::string _module, std::string func, std::vector<uint8_t> argv) {
		auto _proxy = _gates->get_direct_client(_client_call_hub_module->current_ch);
		if (_proxy == nullptr) {
			spdlog::trace("call_hub _proxy is nullptr!");
			return;
		}

		try {
			_gates->current_client_cuuid = _proxy->_cuuid;
			_hub->modules.process_module_mothed(_module, func, argv);
			_gates->current_client_cuuid = "";
		}
		catch (std::exception e) {
			spdlog::trace("call_hub exception:{0}", e.what());
			_hub->sig_client_exception.emit(_proxy->_cuuid);
		}
	}

};



}

#endif //_client_msg_handle_h
