/*
 * center_msg_handle.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _center_msg_handle_h
#define _center_msg_handle_h

#include "centerproxy.h"
#include "closehandle.h"
#include "hub.h"

namespace center_msg {

void reg_server_sucess(std::shared_ptr<hub::centerproxy> _centerproxy) {
	_centerproxy->is_reg_sucess = true;

	spdlog::trace("connect center server sucess");
}

void close_server(std::shared_ptr<hub::closehandle> _closehandle) {
	_closehandle->is_closed = true;
}

void distribute_server_address(std::shared_ptr<hub::hub_service> _hub, std::string type, std::string ip, int64_t port, std::string uuid) {
	if (type == "gate") {
		_hub->connect_gate(uuid, ip, (uint16_t)port);
	}
	if (type == "hub") {
		_hub->reg_hub(ip, (uint16_t)port);
	}
	if (type == "dbproxy") {
		_hub->try_connect_db(ip, (uint16_t)port);
	}
}

void reload(std::shared_ptr<hub::hub_service> _hub, std::string argv) {

}

}

#endif //_center_msg_handle_h
