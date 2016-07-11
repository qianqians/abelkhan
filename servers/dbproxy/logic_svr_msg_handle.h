/*
 * logic_svr_msg_handle.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _logic_svr_msg_handle_h
#define _logic_svr_msg_handles_h

#include <boost/shared_ptr.hpp>

#include <string>
#include <cstdint>

#include <caller/dbproxy_call_logiccaller.h>

#include "logicsvrmanager.h"
#include "mongodb_proxy.h"

void save_object(boost::shared_ptr<dbproxy::logicsvrmanager> _logicsvrmanager, boost::shared_ptr<dbproxy::mongodb_proxy> _mongodb_proxy, std::string json_query, std::string json_info, int64_t callbackid) {
	if (!_logicsvrmanager->is_logic(juggle::current_ch)) {
		return;
	}

	_mongodb_proxy->update(json_query, json_info);

	boost::shared_ptr<caller::dbproxy_call_logic> _caller = boost::make_shared<caller::dbproxy_call_logic>(juggle::current_ch);
	_caller->save_object_sucess(callbackid);
}

void find_object(boost::shared_ptr<dbproxy::logicsvrmanager> _logicsvrmanager, boost::shared_ptr<dbproxy::mongodb_proxy> _mongodb_proxy, std::string json_query, int64_t callbackid) {
	if (!_logicsvrmanager->is_logic(juggle::current_ch)) {
		return;
	}

	auto s = _mongodb_proxy->find(0, 0, 0, json_query, "");

	if (s.size() > 1) {
		std::cout << "have multi num object" << std::endl;
		return;
	}

	boost::shared_ptr<caller::dbproxy_call_logic> _caller = boost::make_shared<caller::dbproxy_call_logic>(juggle::current_ch);
	_caller->ack_find_object(callbackid, s[0]);
}

#endif //_logic_svr_msg_handles_h
