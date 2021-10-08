/*
 * hub_svr_msg_handle.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _hub_svr_msg_handle_h
#define _hub_svr_msg_handle_h

#include <module.h>
#include <modulemng_handle.h>

#include <hub.h>

#include "hubsvrmanager.h"
#include "hub_service.h"

namespace hub {

class hub_svr_msg_handle {
private:
	std::shared_ptr<hub::hub_service> _hub;
	std::shared_ptr<hub::hubsvrmanager> _hubmng;
	std::shared_ptr<abelkhan::hub_call_hub_module> _hub_call_hub_module;

public:
	hub_svr_msg_handle(std::shared_ptr<hub::hub_service> hub_, std::shared_ptr<hub::hubsvrmanager> hubmng_) {
		_hub = hub_;
		_hubmng = hubmng_;

		_hub_call_hub_module = std::make_shared<abelkhan::hub_call_hub_module>();
		_hub_call_hub_module->Init(service::_modulemng);
		_hub_call_hub_module->sig_reg_hub.connect(std::bind(&hub_svr_msg_handle::reg_hub, this, std::placeholders::_1, std::placeholders::_2));
		_hub_call_hub_module->sig_hub_call_hub_mothed.connect(std::bind(&hub_svr_msg_handle::hub_call_hub_mothed, this, std::placeholders::_1));
	}

	void reg_hub(std::string hub_name, std::string hub_type) {
		auto rsp = std::static_pointer_cast<abelkhan::hub_call_hub_reg_hub_rsp>(_hub_call_hub_module->rsp);
		_hubmng->reg_hub(hub_name, hub_type, _hub_call_hub_module->current_ch);
		rsp->rsp();
	}

	void hub_call_hub_mothed(std::vector<uint8_t> rpc_argvs) {
		try {
			std::string err;
			auto InArray = msgpack11::MsgPack::parse((char*)rpc_argvs.data(), rpc_argvs.size(), err);
			if (!InArray.is_array()) {
				spdlog::trace("hub_svr_msg_handle hub_call_hub_mothed argv is not match!!");
				return;
			}

			auto module_name = InArray[0].string_value();
			auto func_name = InArray[1].string_value();
			auto argvs = InArray[2].array_items();

			_hubmng->current_hubproxy = _hubmng->get_hub(_hub_call_hub_module->current_ch);
			_hub->modules.process_module_mothed(module_name, func_name, argvs);
			_hubmng->current_hubproxy = nullptr;
		}
		catch (std::exception e) {
			spdlog::trace("hub_call_hub_mothed exception:{0}", e.what());
		}
	}
	
};

}

#endif //_hub_svr_msg_handle_h
