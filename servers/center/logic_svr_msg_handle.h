/*
 * logic_svr_msg_handle.h
 *
 *  Created on: 2016-7-9
 *      Author: qianqians
 */
#ifndef _logic_svr_msg_handle_h
#define _logic_svr_msg_handle_h

#include <string>
#include <cstdint>

#include <boost/shared_ptr.hpp>

#include <caller/center_call_logiccaller.h>

#include "logicsvrmanager.h"

void req_get_server_address(boost::shared_ptr<center::logicsvrmanager> _logicsvrmanager, boost::shared_ptr<center::svrmanager> _svrmanager, std::string uuid, std::string callbackid) {
	if (!_logicsvrmanager->is_logic(juggle::current_ch)) {
		std::cout << "not a logic channel call this function" << std::endl;
		return;
	}

	auto _caller = boost::make_shared<caller::center_call_logic>(juggle::current_ch);

	std::tuple<std::string, std::string, int64_t, std::string> info;
	if (_svrmanager->get_svr_info(info, uuid)) {
		_caller->ack_get_server_address(true, std::get<0>(info), std::get<1>(info), std::get<2>(info), std::get<3>(info), callbackid);
	}
	else {
		_caller->ack_get_server_address(false, "", "", 0, "", callbackid);
	}

}

#endif //_logic_svr_msg_handle_h
