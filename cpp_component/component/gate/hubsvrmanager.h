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

namespace gate {

class hubproxy {
private:
	std::string _hub_name;
	std::shared_ptr<abelkhan::gate_call_hub_caller> _gate_call_hub_caller;

public:
	hubproxy(std::string& hub_name, std::shared_ptr<abelkhan::Ichannel> ch) {
		_hub_name = hub_name;
		_gate_call_hub_caller = std::make_shared<abelkhan::gate_call_hub_caller>(ch, service::_modulemng);
	}

	const std::string hub_name() {
		return _hub_name;
	}

	void client_disconnect(std::string& client_cuuid) {
		_gate_call_hub_caller->client_disconnect(client_cuuid);
	}

	void client_call_hub(std::string& client_cuuid, std::vector<uint8_t>& data) {
		_gate_call_hub_caller->client_call_hub(client_cuuid, data);
	}
};

class hubsvrmanager {
public:
	hubsvrmanager() {
	}

	virtual ~hubsvrmanager(){
	}

	std::shared_ptr<hubproxy> reg_hub(std::string hub_name, std::shared_ptr<abelkhan::Ichannel> ch) {
		auto _hubproxy = std::make_shared<hubproxy>(hub_name, ch);

		hub_name_proxy.insert(std::make_pair(hub_name, _hubproxy));
		hub_channel_name.insert(std::make_pair(ch, hub_name));

		return _hubproxy;
	}

	std::shared_ptr<hubproxy> get_hub(std::string hub_name) {
		if (hub_name_proxy.find(hub_name) == hub_name_proxy.end()){
			return nullptr;
		}
		return hub_name_proxy[hub_name];
	}

	std::string get_hub(std::shared_ptr<abelkhan::Ichannel> hub_channel) {
		if (hub_channel_name.find(hub_channel) == hub_channel_name.end()) {
			return "";
		}
		return hub_channel_name[hub_channel];
	}

	void for_each_hub(std::function<void(std::string hub_name, std::shared_ptr<hubproxy> proxy)> fn){
		for (auto it : hub_name_proxy) {
			fn(it.first, it.second);
		}
	}

private:
	std::map<std::string, std::shared_ptr<hubproxy> > hub_name_proxy;
	std::map<std::shared_ptr<abelkhan::Ichannel>, std::string> hub_channel_name;

};

}

#endif //_hubsvrmanager_h
