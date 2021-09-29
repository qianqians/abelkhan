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
#include "hub.h"

namespace hub_msg {

void reg_hub(std::shared_ptr<hub::hubsvrmanager> hubs, std::string hub_name) {
	auto _proxy = hubs->reg_hub(hub_name, juggle::current_ch);
	_proxy->reg_hub_sucess();
}

void reg_hub_sucess() {
	spdlog::trace("connect hub sucess");
}

void hub_call_hub_mothed(std::shared_ptr<hub::hub_service> _hub, std::string module_name, std::string func_name, Fossilizid::JsonParse::JsonArray argvs) {
	_hub->modules.process_module_mothed(module_name, func_name, argvs);
}

}

#endif //_hub_svr_msg_handle_h
