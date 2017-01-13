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

void reg_server_sucess(std::shared_ptr<hub::centerproxy> _centerproxy) {
	_centerproxy->is_reg_sucess = true;

	std::cout << "connect center server sucess" << std::endl;
}

void close_server(std::shared_ptr<hub::closehandle> _closehandle) {
	_closehandle->is_closed = true;
}

#endif //_center_msg_handle_h
