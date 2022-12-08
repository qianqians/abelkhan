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

#include "gate_server.h"
#include "centerproxy.h"
#include "hubsvrmanager.h"

namespace gate {

class center_msg_handle
{
private:
	std::shared_ptr<gate_service> _gate_service;
	std::shared_ptr<hubsvrmanager> _hubsvrmanager;
	std::shared_ptr<service::timerservice> _timerservice;
	std::shared_ptr<service::redismqservice> _hub_redismq_service;

	std::shared_ptr<abelkhan::center_call_server_module> _center_call_server_module;

public:
	center_msg_handle(std::shared_ptr<gate::gate_service> gate_service_, std::shared_ptr<hubsvrmanager> hubsvrmanager_, std::shared_ptr<service::timerservice> timerservice_, std::shared_ptr<service::redismqservice> hub_redismq_service_) {
		_gate_service = gate_service_;
		_hubsvrmanager = hubsvrmanager_;
		_timerservice = timerservice_;
		_hub_redismq_service = hub_redismq_service_;

		_center_call_server_module = std::make_shared<abelkhan::center_call_server_module>();
		_center_call_server_module->Init(service::_modulemng);
		_center_call_server_module->sig_close_server.connect(std::bind(&center_msg_handle::on_close_server, this));
		_center_call_server_module->sig_console_close_server.connect(std::bind(&center_msg_handle::console_close_server, this, std::placeholders::_1, std::placeholders::_2));
		_center_call_server_module->sig_svr_be_closed.connect(std::bind(&center_msg_handle::svr_be_closed, this, std::placeholders::_1, std::placeholders::_2));
		_center_call_server_module->sig_take_over_svr.connect(std::bind(&center_msg_handle::take_over_svr, this, std::placeholders::_1));
	}

	void on_close_server() {
		_timerservice->addticktimer(5000, [this](int64_t tick) {
			_gate_service->sig_close_server.emit();
		});
	}

	void console_close_server(std::string svr_type, std::string svr_name) {
		if (svr_type == "gate" && svr_name == _gate_service->gate_name_info.name) {
			_timerservice->addticktimer(5000, [this](int64_t tick) {
				_gate_service->sig_close_server.emit();
			});
		}
		else {
			svr_be_closed(svr_type, svr_name);
		}
	}

	void svr_be_closed(std::string svr_type, std::string svr_name) {
		if (svr_type == "hub") {
			service::gc_put([this, svr_name]() {
				_hubsvrmanager->unreg_hub(svr_name);
			});
		}
	}

	void take_over_svr(std::string svr_name) {
		_hub_redismq_service->take_over_svr(svr_name);
	}

};

}

#endif //_center_msg_handle_h
