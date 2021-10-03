/*
 * dbproxy_msg_handle.h
 *
 *  Created on: 2020-1-10
 *      Author: qianqians
 */
#ifndef _dbproxy_msg_handle_h
#define _dbproxy_msg_handle_h

#include <spdlog/spdlog.h>

#include <modulemng_handle.h>

#include "dbproxyproxy.h"

namespace hub {

class dbproxy_msg_handle {
private:
	std::shared_ptr<abelkhan::dbproxy_call_hub_module> _dbproxy_call_hub_module;

public:
	dbproxy_msg_handle() {
		_dbproxy_call_hub_module = std::make_shared<abelkhan::dbproxy_call_hub_module>();
		_dbproxy_call_hub_module->Init(service::_modulemng);
		_dbproxy_call_hub_module->sig_ack_get_object_info.connect(std::bind(&dbproxy_msg_handle::ack_get_object_info, this, std::placeholders::_1, std::placeholders::_2));
	}

	void ack_get_object_info(std::string callbackid, std::vector<uint8_t> bin_obejct_array) {
		std::string err;
		auto doc = msgpack11::MsgPack::parse((const char*)bin_obejct_array.data(), bin_obejct_array.size(), err);
		if (doc.is_array()) {
			auto cb = dbproxyproxy::get_object_info_callback.find(callbackid);
			if (cb != dbproxyproxy::get_object_info_callback.end()) {
				cb->second(doc.array_items());
			}
			else {
				spdlog::error("unreg getObjectInfo callback id:{0}!", callbackid);
			}
		}
		else {
			spdlog::error("getObjectInfo return is not array!");
		}
	}

	void ack_get_object_info_end(std::string callbackid) {
		auto cb = dbproxyproxy::get_object_info_end_callback.find(callbackid);
		if (cb != dbproxyproxy::get_object_info_end_callback.end()) {
			cb->second();
			dbproxyproxy::get_object_info_callback.erase(callbackid);
			dbproxyproxy::get_object_info_end_callback.erase(callbackid);
		}
		else {
			spdlog::error("unreg getObjectInfo end callback id:{0}!", callbackid);
		}
	}
};



}


#endif //_dbproxy_msg_handle_h
