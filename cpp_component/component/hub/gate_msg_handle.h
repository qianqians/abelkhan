/*
 * gate_msg_handle.h
 *
 *  Created on: 2020-1-10
 *      Author: qianqians
 */
#ifndef _gate_msg_handle_h
#define _gate_msg_handle_h

#include <iostream>
#include <string>

#include <Imodule.h>

#include "hub.h"
#include "gatemanager.h"

namespace gate_msg{

void reg_hub_sucess() {
	spdlog::trace("connect gate sucess");
}

void client_connect(std::shared_ptr<hub::gatemanager> gates, std::string client_uuid) {
	spdlog::trace("client connect:{0}", client_uuid);
	gates->client_connect(client_uuid, juggle::current_ch);
}

void client_disconnect(std::shared_ptr<hub::gatemanager> gates, std::string client_uuid) {
	gates->client_disconnect(client_uuid);
}

void client_exception(std::shared_ptr<hub::gatemanager> gates, std::string client_uuid) {
	gates->client_exception(client_uuid);
}

void client_call_hub(std::shared_ptr<hub::hub_service> _hub, std::string uuid, std::string _module, std::string func, Fossilizid::JsonParse::JsonArray argv) {
	hub::current_client_uuid = uuid;
	_hub->modules.process_module_mothed(_module, func, argv);
	hub::current_client_uuid = "";
}

}

#endif //_gate_msg_handle_h
