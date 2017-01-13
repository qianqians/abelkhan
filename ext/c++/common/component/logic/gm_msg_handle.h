/*
 * gm_msg_handle.h
 *
 *  Created on: 2015-6-9
 *      Author: qianqians
 */
#ifndef _gm_msg_handle_h
#define _gm_msg_handle_h

#include <string>
#include <memory>

#include <Imodule.h>

#include <caller/center_call_servercaller.h>

#include "gmmanager.h"
#include "svrmanager.h"

void confirm_gm(std::shared_ptr<center::gmmanager> _gmmanager, std::string gmname) {
	_gmmanager->reg_channel(gmname, juggle::current_ch);
}

void close_clutter(std::shared_ptr<center::gmmanager> _gmmanager, std::shared_ptr<center::svrmanager> _svrmanager, std::string gmname) {
	auto ch = _gmmanager->get_channel(gmname);
	if (ch != juggle::current_ch) {
		std::cout << "connection error gm:" << gmname << std::endl;
	}

	_svrmanager->for_each_svr([](std::shared_ptr<juggle::Ichannel> ch) {
		std::shared_ptr<caller::center_call_server> _caller = std::make_shared<caller::center_call_server>(ch);
		_caller->close_server();
	});
}

#endif //_gm_msg_handle_h
