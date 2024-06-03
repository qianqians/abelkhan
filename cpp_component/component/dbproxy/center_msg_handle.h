/*
 * center_msg_handle.h
 * qianqians
 * 2024/5/30
 */
#ifndef _center_msg_handle_h_
#define _center_msg_handle_h_

#include <memory>
#include <string>

#include <modulemng_handle.h>
#include <center.h>

#include "closehandle.h"
#include "centerproxy.h"
#include "hubmanager.h"

namespace dbproxy
{

class center_msg_handle
{
private:
	std::shared_ptr<close_handle> _closehandle;
	std::shared_ptr<centerproxy> _centerproxy;
	std::shared_ptr<hubmanager> _hubs;

	std::shared_ptr<abelkhan::center_call_server_module> _center_call_server_module;

public:
	center_msg_handle(std::shared_ptr<close_handle> _closehandle_, std::shared_ptr<centerproxy> _centerproxy_, std::shared_ptr<hubmanager> _hubs_) {
		_closehandle = _closehandle_;
		_centerproxy = _centerproxy_;
		_hubs = _hubs_;

		_center_call_server_module = std::make_shared<abelkhan::center_call_server_module>();
		_center_call_server_module->Init(service::_modulemng);

		_center_call_server_module->sig_close_server.connect(std::bind(&center_msg_handle::close_server, this));
		_center_call_server_module->sig_console_close_server.connect(std::bind(&center_msg_handle::console_close_server, this, std::placeholders::_1, std::placeholders::_2));
		_center_call_server_module->sig_svr_be_closed.connect(std::bind(&center_msg_handle::svr_be_closed, this, std::placeholders::_1, std::placeholders::_2));
		_center_call_server_module->sig_take_over_svr.connect(std::bind(&center_msg_handle::take_over_svr, this, std::placeholders::_1));
	}

private:
	void close_server() {
		_closehandle->_is_closing = true;
		check_close_server();
	}

	void close_server_impl(int64_t tick) {
		_closehandle->_is_close = true;
	}

	void check_close_server() {
		if (_closehandle->_is_closing && _hubs->all_hub_closed())
		{
			_centerproxy->closed();
			dbproxy::_timer->addticktimer(3000, std::bind(&center_msg_handle::close_server_impl, this, std::placeholders::_1));
		}
	}

	void console_close_server(std::string svr_type, std::string svr_name) {
		if (svr_type == "dbproxy" && svr_name == dbproxy::name)
		{
			dbproxy::_timer->addticktimer(3000, std::bind(&center_msg_handle::close_server_impl, this, std::placeholders::_1));
		}
		else
		{
			if (svr_type == "hub")
			{
				_hubs->on_hub_closed(svr_name);
				check_close_server();
			}
		}
	}

	void svr_be_closed(std::string svr_type, std::string svr_name) {
		spdlog::trace("svr_be_closed svr_type:{0}, svr_name:{1}", svr_type, svr_name);

		if (svr_type == "hub")
		{
			_hubs->on_hub_closed(svr_name);
			check_close_server();
		}
	}

	void take_over_svr(std::string svr_name) {
		dbproxy::_redis_mq_service->take_over_svr(svr_name);
	}

};


}

#endif // !_center_msg_handle_h_
