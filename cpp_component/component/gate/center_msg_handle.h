/*
 * center_msg_handle.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _center_msg_handle_h
#define _center_msg_handle_h

#include <modulemng_handle.h>

#include "centerproxy.h"
#include "closehandle.h"

namespace gate {

class center_msg_handle
{
private:
	std::shared_ptr<gate::closehandle> _closehandle;
	std::shared_ptr<abelkhan::center_call_server_module> _center_call_server_module;

public:
	center_msg_handle(std::shared_ptr<gate::closehandle> closehandle) {
		_closehandle = closehandle;

		_center_call_server_module = std::make_shared<abelkhan::center_call_server_module>();
		_center_call_server_module->Init(service::_modulemng);
		_center_call_server_module->sig_close_server.connect(std::bind(&center_msg_handle::on_close_server, this));
	}

	void on_close_server() {
		_closehandle->is_closed = true;
	}

};

}

#endif //_center_msg_handle_h
