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
		_client_call_hub_module->sig_call_hub.connect(std::bind(&direct_client_msg_handle::call_hub, this, std::placeholders::_1));
	}

	void client_connect(std::string cuuid) {
		_gates->client_direct_connect(cuuid, _client_call_hub_module->current_ch);
	}

	void heartbeats() {
		auto rsp = std::static_pointer_cast<abelkhan::client_call_hub_heartbeats_rsp>(_client_call_hub_module->rsp);
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
		rsp->rsp(_hub->_timerservice->Tick);
	}

	void call_hub(std::vector<uint8_t> rpc_argvs) {
		auto _proxy = _gates->get_direct_client(_client_call_hub_module->current_ch);
		if (_proxy == nullptr) {
			spdlog::trace("call_hub _proxy is nullptr!");
			return;
		}

		try {
			std::string err;
			auto InArray = msgpack11::MsgPack::parse((char*)rpc_argvs.data(), rpc_argvs.size(), err);
			if (!InArray.is_array()) {
				spdlog::trace("hub_svr_msg_handle hub_call_hub_mothed argv is not match!!");
				return;
			}

			auto func = InArray[0].string_value();
			auto argvs = InArray[1].array_items();

			gatemanager::current_client_cuuid = _proxy->_cuuid;
			_hub->modules.process_module_mothed(func, argvs);
			gatemanager::current_client_cuuid = "";
		}
		catch (std::exception e) {
			spdlog::trace("call_hub exception:{0}", e.what());
			_hub->sig_client_exception.emit(_proxy->_cuuid);
		}
	}

};



}

#endif //_client_msg_handle_h
