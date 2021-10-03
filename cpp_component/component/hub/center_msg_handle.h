/*
 * center_msg_handle.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _center_msg_handle_h
#define _center_msg_handle_h

#include <modulemng_handle.h>
#include <timerservice.h>

#include "centerproxy.h"
#include "closehandle.h"
#include "hub.h"

namespace hub {

class center_msg_handle {
private:
	std::shared_ptr<hub::hub_service> _hub;

	std::shared_ptr<abelkhan::center_call_server_module> _center_call_server_module;
	std::shared_ptr<abelkhan::center_call_hub_module> _center_call_hub_module;

public:
	center_msg_handle(std::shared_ptr<hub::hub_service> hub_) {
		_hub = hub_;

		_center_call_server_module = std::make_shared<abelkhan::center_call_server_module>();
		_center_call_server_module->Init(service::_modulemng);
		_center_call_server_module->sig_close_server.connect(std::bind(&center_msg_handle::close_server, this));
		_center_call_server_module->sig_svr_be_closed.connect(std::bind(&center_msg_handle::svr_be_closed, this, std::placeholders::_1));

		_center_call_hub_module = std::make_shared<abelkhan::center_call_hub_module>();
		_center_call_hub_module->Init(service::_modulemng);
		_center_call_hub_module->sig_distribute_server_address.connect(std::bind(&center_msg_handle::distribute_server_address, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3, std::placeholders::_4));
		_center_call_hub_module->sig_reload.connect(std::bind(&center_msg_handle::reload, this, std::placeholders::_1));
	}

private:
	void close_server() {
		_hub->sig_close_server.emit();
	}

	void svr_be_closed(std::string svr_name) {
		_hub->sig_svr_be_closed.emit(svr_name);
	}

	void distribute_server_address(std::string type, std::string ip, int64_t port, std::string name) {
		if (type == "gate") {
			_hub->connect_gate(name, ip, (uint16_t)port);
		}
		if (type == "hub") {
			_hub->reg_hub(ip, (uint16_t)port);
		}
		if (type == "dbproxy") {
			_hub->try_connect_db(name, ip, (uint16_t)port);
		}
	}

	void reload(std::string argv) {
		_hub->sig_reload.emit(argv);
	}
};



}

#endif //_center_msg_handle_h
