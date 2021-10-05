/*
 * heartbeat_handle.h
 *
 *  Created on: 2016-7-14
 *      Author: qianqians
 */
#ifndef _heartbeat_handle_h
#define _heartbeat_handle_h

#include <timerservice.h>

#include "centerproxy.h"
#include "clientmanager.h"

void heartbeat_client(std::shared_ptr<gate::clientmanager> _clientmanager, std::shared_ptr<service::timerservice> _timerservice, int64_t tick) {
	_clientmanager->heartbeat_client(tick);
	_timerservice->addticktimer(10 * 1000, std::bind(&heartbeat_client, _clientmanager, _timerservice, std::placeholders::_1));
}

void heartbeat_center(std::shared_ptr<gate::centerproxy> _centerproxy, std::shared_ptr<service::timerservice> _timerservice, int64_t tick) {
	_centerproxy->heartbeat();
	_timerservice->addticktimer(5 * 1000, std::bind(&heartbeat_center, _centerproxy, _timerservice, std::placeholders::_1));
}

#endif //_heartbeat_handle_h
