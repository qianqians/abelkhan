/*
 * gate_msg_handle.h
 *
 *  Created on: 2020-1-10
 *      Author: qianqians
 */
#ifndef _gate_msg_handle_h
#define _gate_msg_handle_h

#include <string>

#include <modulemng_handle.h>

#include <module.h>
#include <log.h>

#include "hub_service.h"
#include "gatemanager.h"

namespace hub{

class gate_msg_handle {
private:
	std::shared_ptr<abelkhan::gate_call_hub_module> _gate_call_hub_module;
	std::shared_ptr<hub_service> _hub;
	std::shared_ptr<gatemanager> _gates;

public:
	gate_msg_handle(std::shared_ptr<hub_service> hub_, std::shared_ptr<gatemanager> gates_) {
		_hub = hub_;
		_gates = gates_;

		_gate_call_hub_module = std::make_shared<abelkhan::gate_call_hub_module>();
		_gate_call_hub_module->Init(service::_modulemng);
		_gate_call_hub_module->sig_client_disconnect.connect(std::bind(&gate_msg_handle::client_disconnect, this, std::placeholders::_1));
		_gate_call_hub_module->sig_client_exception.connect(std::bind(&gate_msg_handle::client_exception, this, std::placeholders::_1));
		_gate_call_hub_module->sig_client_call_hub.connect(std::bind(&gate_msg_handle::client_call_hub, this, std::placeholders::_1, std::placeholders::_2));
	
	}

	void client_disconnect(std::string client_cuuid) {
		_gates->client_disconnect(client_cuuid);
	}

	void client_exception(std::string client_cuuid) {
		_hub->sig_client_exception.emit(client_cuuid);
	}

	void client_call_hub(std::string cuuid, std::vector<uint8_t> rpc_argv) {
		std::string err;
		auto _msgpack_obj = msgpack11::MsgPack::parse((char*)rpc_argv.data(), rpc_argv.size(), err);
		if (!_msgpack_obj.is_array()) {
			spdlog::trace("call_hub argv is not match!");
			_hub->sig_client_exception.emit(cuuid);
			return;
		}
		auto _event = _msgpack_obj.array_items();
		if (_event.size() != 3) {
			spdlog::trace("call_hub _event argv is not match num_3!");
			_hub->sig_client_exception.emit(cuuid);
			return;
		}

		try {
			auto module_name = _event[0].string_value();
			auto func_name = _event[1].string_value();
			auto argvs = _event[2].array_items();
			_gates->client_connect(cuuid, _gate_call_hub_module->current_ch);
			_gates->current_client_cuuid = cuuid;
			_hub->modules.process_module_mothed(module_name, func_name, argvs);
			_gates->current_client_cuuid = "";
		}
		catch (std::exception e) {
			spdlog::trace("client_call_hub exception:{0}", e.what());
			_hub->sig_client_exception.emit(cuuid);

		}
	}

};

}

#endif //_gate_msg_handle_h
