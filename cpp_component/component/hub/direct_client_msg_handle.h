/*
 * gate_msg_handle.h
 *
 *  Created on: 2020-1-10
 *      Author: qianqians
 */
#ifndef _client_msg_handle_h
#define _client_msg_handle_h

#include <iostream>
#include <string>

#include <Imodule.h>

#include "hub.h"
#include "gatemanager.h"
#include "log.h"

namespace client_msg{

void client_connect(std::shared_ptr<hub::gatemanager> gates, std::string uuid) {
	gates->client_direct_connect(uuid, std::static_pointer_cast<service::webchannel>(juggle::current_ch));
}

void call_hub(std::shared_ptr<hub::hub_service> _hub, std::string uuid, std::string _module, std::string func, Fossilizid::JsonParse::JsonArray argv) {
	hub::current_client_uuid = uuid;
	try {
		_hub->modules.process_module_mothed(_module, func, argv);
	}
	catch (std::exception e) {
		spdlog::trace("call_hub exception:{0}", e.what());
		_hub->sig_client_exception(uuid);
	}
	hub::current_client_uuid = "";
}

}

#endif //_client_msg_handle_h
