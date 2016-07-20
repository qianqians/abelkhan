/*
 * svr_msg_handle.h
 *
 *  Created on: 2016-7-9
 *      Author: qianqians
 */
#ifndef _svr_msg_handle_h
#define _svr_msg_handle_h

#include <string>
#include <cstdint>

#include <boost/shared_ptr.hpp>

#include <caller/center_call_logiccaller.h>
#include <caller/center_call_hubcaller.h>
#include <caller/center_call_servercaller.h>

#include "svrmanager.h"

void reg_server(boost::shared_ptr<center::svrmanager> _svrmanager, boost::shared_ptr<center::logicsvrmanager> _logicsvrmanager, std::string type, std::string ip, int64_t port, std::string uuid) {
	auto _caller = boost::make_shared<caller::center_call_server>(juggle::current_ch);
	_caller->reg_server_sucess();

	_logicsvrmanager->for_each_logic([type, ip, port, uuid](boost::shared_ptr<juggle::Ichannel> ch) {
		auto _caller = boost::make_shared<caller::center_call_logic>(ch);
		_caller->distribute_server_address(type, ip, port, uuid);
	});

	if (type == "logic") {
		_logicsvrmanager->reg_channel(juggle::current_ch);

		auto _caller = boost::make_shared<caller::center_call_logic>(juggle::current_ch);
		_svrmanager->for_each_svr([_svrmanager, _caller](boost::shared_ptr<juggle::Ichannel> ch) {
			std::tuple<std::string, std::string, int64_t, std::string> info;
			if(_svrmanager->get_svr_info(info, ch)) {
				_caller->distribute_server_address(std::get<0>(info), std::get<1>(info), std::get<2>(info), std::get<3>(info));
			}
		});
	}

	if (type == "hub") {
		auto _caller = boost::make_shared<caller::center_call_hub>(juggle::current_ch);
		std::tuple<std::string, std::string, int64_t, std::string> info;
		if (_svrmanager->get_dbproxy_info(info)) {
			_caller->distribute_dbproxy_address(std::get<0>(info), std::get<1>(info), std::get<2>(info), std::get<3>(info));
		}
	}

	if (type == "dbproxy") {
		_svrmanager->for_each_hub([type, ip, port, uuid](boost::shared_ptr<juggle::Ichannel> ch) {
			auto _caller = boost::make_shared<caller::center_call_hub>(ch);
			_caller->distribute_dbproxy_address(type, ip, port, uuid);
		});
	}

	_svrmanager->reg_channel(juggle::current_ch, type, ip, port, uuid);

	std::cout << type << " server " << uuid << " connected" << std::endl;
}

#endif //_svr_msg_handle_h
