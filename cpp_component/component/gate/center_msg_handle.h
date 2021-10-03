/*
 * center_msg_handle.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _center_msg_handle_h
#define _center_msg_handle_h

#include <timerservice.h>
#include <modulemng_handle.h>
#include <gc_poll.h>

#include "centerproxy.h"
#include "hubsvrmanager.h"
#include "closehandle.h"

namespace gate {

class center_msg_handle
{
private:
	std::shared_ptr<closehandle> _closehandle;
	std::shared_ptr<hubsvrmanager> _hubsvrmanager;
	std::shared_ptr<service::timerservice> _timerservice;

	std::shared_ptr<abelkhan::center_call_server_module> _center_call_server_module;

public:
	center_msg_handle(std::shared_ptr<gate::closehandle> closehandle_, std::shared_ptr<hubsvrmanager> hubsvrmanager_, std::shared_ptr<service::timerservice> timerservice_) {
		_closehandle = closehandle_;
		_hubsvrmanager = hubsvrmanager_;
		_timerservice = timerservice_;

		_center_call_server_module = std::make_shared<abelkhan::center_call_server_module>();
		_center_call_server_module->Init(service::_modulemng);
		_center_call_server_module->sig_close_server.connect(std::bind(&center_msg_handle::on_close_server, this));
		_center_call_server_module->sig_svr_be_closed;
	}

	void on_close_server() {
		_timerservice->addticktimer(5 * 1000, [this](int64_t tick) {
			_closehandle->is_closed = true;
		});
	}

	void svr_be_closed(std::string svr_name) {
		service::gc_put([this, svr_name]() {
			_hubsvrmanager->unreg_hub(svr_name);
		});
	}

};

}

#endif //_center_msg_handle_h
