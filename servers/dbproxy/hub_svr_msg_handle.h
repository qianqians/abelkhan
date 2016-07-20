/*
 * hub_svr_msg_handle.h
 *
 *  Created on: 2016-7-14
 *      Author: qianqians
 */
#ifndef _hub_svr_msg_handle_h
#define _hub_svr_msg_handle_h

#include <boost/shared_ptr.hpp>

#include <iostream>
#include <string>
#include <cstdint>

#include <Imodule.h>

#include <caller/dbproxy_call_hubcaller.h>

#include "hubsvrmanager.h"
#include "mongodb_proxy.h"
#include "closehandle.h"

void reg_hub(boost::shared_ptr<dbproxy::hubsvrmanager> _hubsvrmanager, std::string uuid) {
	_hubsvrmanager->reg_channel(uuid, juggle::current_ch);

	boost::shared_ptr<caller::dbproxy_call_hub> _caller = boost::make_shared<caller::dbproxy_call_hub>(juggle::current_ch);
	_caller->reg_hub_sucess();

	std::cout << "hub server " << uuid << " connected" << std::endl;
}

void hub_create_persisted_object(boost::shared_ptr<dbproxy::hubsvrmanager> _hubsvrmanager, boost::shared_ptr<dbproxy::mongodb_proxy> _mongodb_proxy, std::string object_info, std::string callbackid) {
	if (!_hubsvrmanager->is_hub(juggle::current_ch)) {
		return;
	}

	_mongodb_proxy->save(object_info);

	boost::shared_ptr<caller::dbproxy_call_hub> _caller = boost::make_shared<caller::dbproxy_call_hub>(juggle::current_ch);
	_caller->ack_create_persisted_object(callbackid);
}

void hub_updata_persisted_object(boost::shared_ptr<dbproxy::hubsvrmanager> _hubsvrmanager, boost::shared_ptr<dbproxy::mongodb_proxy> _mongodb_proxy, std::string query_json, std::string object_info, std::string callbackid) {
	if (!_hubsvrmanager->is_hub(juggle::current_ch)) {
		return;
	}

	_mongodb_proxy->update(query_json, object_info);

	boost::shared_ptr<caller::dbproxy_call_hub> _caller = boost::make_shared<caller::dbproxy_call_hub>(juggle::current_ch);
	_caller->ack_updata_persisted_object(callbackid);
}

void hub_get_object_info(boost::shared_ptr<dbproxy::hubsvrmanager> _hubsvrmanager, boost::shared_ptr<dbproxy::mongodb_proxy> _mongodb_proxy, std::string query_json, std::string callbackid) {
	if (!_hubsvrmanager->is_hub(juggle::current_ch)) {
		return;
	}

	auto s = _mongodb_proxy->find(0, 0, 0, query_json, "");
	
	boost::shared_ptr<caller::dbproxy_call_hub> _caller = boost::make_shared<caller::dbproxy_call_hub>(juggle::current_ch);

	int count = 0;
	std::string json = "[";
	for (auto sjson : s){
		json += sjson;
		count++;

		if (count < 100) {
			json += ",";
		}
		else {
			json += "]";
			_caller->ack_get_object_info(callbackid, json);
			
			count = 0;
			json = "[";
		}
	}
	if (count > 0 && count < 100) {
		json.replace(json.size() - 1, 1, 1, ']');
		_caller->ack_get_object_info(callbackid, json);
	}

	_caller->ack_get_object_info_end(callbackid);
}

#endif //_hub_svr_msg_handle_h
