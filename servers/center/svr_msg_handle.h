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

#include "svrmanager.h"

void reg_server(boost::shared_ptr<center::svrmanager> _svrmanager, boost::shared_ptr<center::logicsvrmanager> _logicsvrmanager, std::string type, std::string ip, int64_t port, std::string uuid) {
	_svrmanager->reg_channel(juggle::current_ch, type, ip, port, uuid);

	if (type == "logic") {
		_logicsvrmanager->reg_channel(juggle::current_ch);
	}

	std::cout << type << " server" << uuid << " connected" << std::endl;
}

#endif //_svr_msg_handle_h
