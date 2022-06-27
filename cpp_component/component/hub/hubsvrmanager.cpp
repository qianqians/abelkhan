/*
 * qianqians
 * 2016-7-12
 * hubsvrmanager.cpp
 */
#include <modulemng_handle.h>

#include "hubsvrmanager.h"
#include "hub_service.h"

namespace hub {
	
hubproxy::hubproxy(std::string hub_name, std::string hub_type, std::shared_ptr<abelkhan::Ichannel> hub_ch) {
	_hub_name = hub_name;
	_hub_type = hub_type;

	_hub_ch = hub_ch;
	_hub_call_hub_caller = std::make_shared<abelkhan::hub_call_hub_caller>(hub_ch, service::_modulemng);
}

void hubproxy::call_hub(const std::string& module_name, const std::string& func_name, const msgpack11::MsgPack::array& argvs) {
	msgpack11::MsgPack::array event_;
	event_.push_back(module_name);
	event_.push_back(func_name);
	event_.push_back(argvs);
	msgpack11::MsgPack _pack(event_);
	auto data = _pack.dump();
	std::vector<uint8_t> _data_bin;
	_data_bin.resize(data.size());
	memcpy(_data_bin.data(), data.data(), data.size());

	_hub_call_hub_caller->hub_call_hub_mothed(_data_bin);
}

void hubproxy::client_seep(std::string client_uuid, std::string gate_name) {
	_hub_call_hub_caller->seep_client_gate(client_uuid, gate_name);
}

hubsvrmanager::hubsvrmanager(std::shared_ptr<hub_service> _hub_) {
	_hub = _hub_;

	_hub->sig_svr_be_closed.connect([this](std::string svr_type, std::string hub_name) {
		if (svr_type != "hub") {
			return;
		}

		auto old_it = wait_destory_hubproxys.find(hub_name);
		if (old_it != wait_destory_hubproxys.end()) {
			ch_hubproxys.erase(old_it->second->_hub_ch);
			wait_destory_hubproxys.erase(old_it);
		}
		else {
			auto it = hubproxys.find(hub_name);
			if (it != hubproxys.end()) {
				auto _proxy = it->second;

				ch_hubproxys.erase(_proxy->_hub_ch);
				hubproxys.erase(it);

				_hub->sig_hub_closed.emit(_proxy->_hub_name, _proxy->_hub_type);
			}
		}
	});
}

void hubsvrmanager::reg_hub(std::string hub_name, std::string hub_type, std::shared_ptr<abelkhan::Ichannel> ch) {
	auto _proxy = std::make_shared<hubproxy>(hub_name, hub_type, ch);

	auto it = hubproxys.find(hub_name);
	if (it != hubproxys.end()) {
		wait_destory_hubproxys.insert(std::make_pair(it->first, it->second));
		_hub->sig_hub_reconnect.emit(_proxy);
	}
	else {
		_hub->sig_hub_connect.emit(_proxy);
	}
	hubproxys[hub_name] = _proxy;
	ch_hubproxys[ch] = _proxy;
}

void hubsvrmanager::call_hub(const std::string& hub_name, const std::string& module_name, const std::string& func_name, const msgpack11::MsgPack::array& argvs) {
	auto it_hubproxy = hubproxys.find(hub_name);
	if (it_hubproxy == hubproxys.end()) {
		spdlog::error("unreg hub:{0}!", hub_name);
		return;
	}

	it_hubproxy->second->call_hub(module_name, func_name, argvs);
}

std::shared_ptr<hubproxy> hubsvrmanager::get_hub(std::shared_ptr<abelkhan::Ichannel> ch) {
	auto it_hubproxy = ch_hubproxys.find(ch);
	if (it_hubproxy != ch_hubproxys.end()) {
		return it_hubproxy->second;
	}

	return nullptr;
}

}