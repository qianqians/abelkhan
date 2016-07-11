/*
 * gm_msg_handle.h
 *
 *  Created on: 2015-6-9
 *      Author: qianqians
 */
#ifndef _gm_msg_handle_h
#define _gm_msg_handle_h

#include <string>

#include <boost/shared_ptr.hpp>

#include <Imodule.h>

#include <caller/center_call_servercaller.h>

#include "gmmanager.h"
#include "svrmanager.h"

void confirm_gm(boost::shared_ptr<center::gmmanager> _gmmanager, std::string gmname) {
	_gmmanager->reg_channel(gmname, juggle::current_ch);
}

void close_clutter(boost::shared_ptr<center::gmmanager> _gmmanager, boost::shared_ptr<center::svrmanager> _svrmanager, std::string gmname) {
	auto ch = _gmmanager->get_channel(gmname);
	if (ch != juggle::current_ch) {
		std::cout << "connection error gm:" << gmname << std::endl;
	}

	_svrmanager->for_each_svr([](boost::shared_ptr<juggle::Ichannel> ch) {
		boost::shared_ptr<caller::center_call_server> _caller = boost::make_shared<caller::center_call_server>(ch);
		_caller->close_server();
	});
}

#endif //_gm_msg_handle_h
