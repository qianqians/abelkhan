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

void reg_server_sucess(boost::shared_ptr<dbproxy::centerproxy> _centerproxy) {
	_centerproxy->is_reg_sucess = true;

	std::cout << "connect center server sucess" << std::endl;
}

void close_server(boost::shared_ptr<dbproxy::closehandle> _closehandle) {
	_closehandle->is_closed = true;
}

#endif //_center_msg_handle_h
