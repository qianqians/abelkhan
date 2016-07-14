/*
 * heartbeat_handle.h
 *
 *  Created on: 2016-7-14
 *      Author: qianqians
 */
#ifndef _heartbeat_handle_h
#define _heartbeat_handle_h

#include <boost/bind.hpp>
#include <timerservice.h>

#include "clientmanager.h"

void heartbeat_handle(boost::shared_ptr<gate::clientmanager> _clientmanager, boost::shared_ptr<service::timerservice> _timerservice, int64_t tick) {
	_clientmanager->heartbeat_client(tick);

	_timerservice->addticktime(tick + 5 * 60 * 1000, boost::bind(&heartbeat_handle, _clientmanager, _timerservice, _1));
}

#endif //_heartbeat_handle_h
