/*
 * dbproxy_msg_handle.h
 *
 *  Created on: 2020-1-10
 *      Author: qianqians
 */
#ifndef _dbproxy_msg_handle_h
#define _dbproxy_msg_handle_h

#include <spdlog/spdlog.h>

#include "dbproxyproxy.h"

namespace db_msg {

void reg_hub_sucess() {
	spdlog::trace("connect db sucess");
	//this.hub.onConnectDB_event();
}

void ack_create_persisted_object(std::shared_ptr<hub::dbproxyproxy> proxy, std::string callbackid, bool is_create_sucess) {
	auto cb = proxy->create_persisted_object_callback[callbackid];
	cb(is_create_sucess);
	proxy->create_persisted_object_callback.erase(callbackid);
}

void ack_updata_persisted_objec(std::shared_ptr<hub::dbproxyproxy> proxy, std::string callbackid) {
	auto cb = proxy->updata_persisted_object_callback[callbackid];
	cb();
	proxy->updata_persisted_object_callback.erase(callbackid);
}

void ack_get_object_count(std::shared_ptr<hub::dbproxyproxy> proxy, std::string callbackid, uint64_t count) {
	auto cb = proxy->get_object_count_callback[callbackid];
	cb(count);
	proxy->get_object_count_callback.erase(callbackid);
}

void ack_get_object_info(std::shared_ptr<hub::dbproxyproxy> proxy, std::string callbackid, Fossilizid::JsonParse::JsonArray json_obejct_array) {
	auto cb = proxy->get_object_info_callback[callbackid];
	cb(json_obejct_array);
}

void ack_get_object_info_end(std::shared_ptr<hub::dbproxyproxy> proxy, std::string callbackid) {
	auto cb = proxy->get_object_info_end_callback[callbackid];
	cb();
	proxy->get_object_info_callback.erase(callbackid);
	proxy->get_object_info_end_callback.erase(callbackid);
}

void ack_remove_object(std::shared_ptr<hub::dbproxyproxy> proxy, std::string callbackid) {
	auto cb = proxy->remove_object_callback[callbackid];
	cb();
	proxy->remove_object_callback.erase(callbackid);
}

}


#endif //_dbproxy_msg_handle_h
