/*
 * hubsvrmanager.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _hubsvrmanager_h
#define _hubsvrmanager_h

#include <string>
#include <map>

#include <abelkhan.h>
#include <modulemng_handle.h>

#include <hub.h>
#include <gate.h>

namespace gate {

class hubproxy {
public:
	std::string _hub_name;
	std::string _hub_type;
	uint32_t _tick_time;
	std::shared_ptr<abelkhan::Ichannel> _ch;

private:
	abelkhan::gate_call_hub_caller _gate_call_hub_caller;

public:
	hubproxy(std::string& hub_name, std::string& hub_type, std::shared_ptr<abelkhan::Ichannel> ch) : _gate_call_hub_caller(ch, service::_modulemng) {
		_hub_name = hub_name;
		_hub_type = hub_type;
		_ch = ch;
	}

	void client_disconnect(std::string& client_cuuid) {
		_gate_call_hub_caller.client_disconnect(client_cuuid);
	}

	void client_exception(std::string& client_cuuid) {
		_gate_call_hub_caller.client_exception(client_cuuid);
	}

	void client_call_hub(std::string& client_cuuid, std::vector<uint8_t>& data) {
		_gate_call_hub_caller.client_call_hub(client_cuuid, data);
	}
};

class hubsvrmanager {
public:
	hubsvrmanager() {
	}

	virtual ~hubsvrmanager(){
	}

	std::shared_ptr<hubproxy> reg_hub(std::string hub_name, std::string& hub_type, std::shared_ptr<abelkhan::Ichannel> ch) {
		auto _hubproxy = std::make_shared<hubproxy>(hub_name, hub_type, ch);

		auto it = hub_name_proxy.find(hub_name);
		if (it != hub_name_proxy.end()) {
			wait_destory_proxy.insert(std::make_pair(it->first, it->second));
			hub_name_proxy[hub_name] = _hubproxy;
		}
		else {
			hub_name_proxy.insert(std::make_pair(hub_name, _hubproxy));
			hub_channel_name.insert(std::make_pair(ch, hub_name));
		}

		return _hubproxy;
	}

	void unreg_hub(std::string hub_name) {
		auto old_id = wait_destory_proxy.find(hub_name);
		if (old_id != wait_destory_proxy.end()) {
			hub_channel_name.erase(old_id->second->_ch);
			wait_destory_proxy.erase(old_id);
		}
		else {
			auto it = hub_name_proxy.find(hub_name);
			if (it != hub_name_proxy.end()) {
				spdlog::trace("hubsvrmanager unreg_hub:{0}!", hub_name);

				hub_channel_name.erase(it->second->_ch);
				hub_name_proxy.erase(it);
			}
		}
	}

	std::shared_ptr<hubproxy> get_hub(std::string hub_name) {
		if (hub_name_proxy.find(hub_name) == hub_name_proxy.end()){
			return nullptr;
		}
		return hub_name_proxy[hub_name];
	}

	std::shared_ptr<hubproxy> get_hub(std::shared_ptr<abelkhan::Ichannel> hub_channel) {
		if (hub_channel_name.find(hub_channel) == hub_channel_name.end()) {
			return nullptr;
		}
		return get_hub(hub_channel_name[hub_channel]);
	}

	abelkhan::hub_info get_hub_list(std::string hub_type) {
		abelkhan::hub_info _info;

		uint32_t tick_time_tmp = INT32_MAX;
		for (auto it : hub_name_proxy) {
			if (it.second->_hub_type != hub_type) {
				continue;
			}
			if (it.second->_tick_time > 100) {
				continue;
			}
			
			if (it.second->_tick_time < tick_time_tmp) {
				tick_time_tmp = it.second->_tick_time;

				_info.hub_name = it.second->_hub_name;
				_info.hub_type = it.second->_hub_type;
			}
		}
		
		return _info;
	}

	void for_each_hub(std::function<void(std::string hub_name, std::shared_ptr<hubproxy> proxy)> fn){
		for (auto it : hub_name_proxy) {
			fn(it.first, it.second);
		}
	}

private:
	std::unordered_map<std::string, std::shared_ptr<hubproxy> > wait_destory_proxy;
	std::unordered_map<std::string, std::shared_ptr<hubproxy> > hub_name_proxy;
	std::unordered_map<std::shared_ptr<abelkhan::Ichannel>, std::string> hub_channel_name;

};

}

#endif //_hubsvrmanager_h
