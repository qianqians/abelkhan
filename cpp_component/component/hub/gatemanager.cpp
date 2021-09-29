/*
 * qianqians
 * 2020-1-10
 * gatemanager.cpp
 */
#include <factory.h>

#include <enetconnectservice.h>
#include <websocketchannel.h>

#include "gatemanager.h"
#include "hub.h"

namespace hub {
	
std::string current_client_uuid = ""; 

gateproxy::gateproxy(std::shared_ptr<juggle::Ichannel> ch, std::shared_ptr<hub_service> hub) {
	caller = Fossilizid::pool::factory::create<caller::hub_call_gate>(ch);
	_hub = hub;
}

void gateproxy::reg_hub() {
	caller->reg_hub(_hub->uuid, _hub->name);
}

void gateproxy::connect_sucess(std::string client_uuid) {
	caller->connect_sucess(client_uuid);
}

void gateproxy::disconnect_client(std::string uuid) {
	caller->disconnect_client(uuid);
}

void gateproxy::forward_hub_call_client(std::string uuid, std::string _module, std::string func, Fossilizid::JsonParse::JsonArray argv) {
	caller->forward_hub_call_client(uuid, _module, func, argv);
}

void gateproxy::forward_hub_call_group_client(Fossilizid::JsonParse::JsonArray uuids, std::string _module, std::string func, Fossilizid::JsonParse::JsonArray argv) {
	caller->forward_hub_call_group_client(uuids, _module, func, argv);
}

void gateproxy::forward_hub_call_global_client(std::string _module, std::string func, Fossilizid::JsonParse::JsonArray argv) {
	caller->forward_hub_call_global_client(_module, func, argv);
}

directproxy::directproxy(std::shared_ptr<service::webchannel> _direct_ch) {
	direct_ch = _direct_ch;
	caller = Fossilizid::pool::factory::create<caller::hub_call_client>(direct_ch);
}

void directproxy::dispose() {
	caller = nullptr;
}

void directproxy::call_client(const std::string& _module, const std::string& func, const Fossilizid::JsonParse::JsonArray& argv) {
	caller->call_client(_module, func, argv);
}

gatemanager::gatemanager(std::shared_ptr<service::enetconnectservice> _conn, std::shared_ptr<hub_service> _hub_) {
	conn = _conn;
	_hub = _hub_;

	current_client_uuid = "";
}

void gatemanager::connect_gate(std::string uuid, std::string ip, uint16_t port) {
	spdlog::trace("connect_gate ip:{0} port:{1}", ip, port);
	conn->connect(ip, port, [this, ip, port, uuid](std::shared_ptr<juggle::Ichannel> ch) {
		spdlog::trace("gate ip:{0}  port:{1} reg_hub", ip, port);
		gates[uuid] = Fossilizid::pool::factory::create<gateproxy>(ch, _hub);
		ch_gates[ch] = gates[uuid];
		gates[uuid]->reg_hub();
	});
}

void gatemanager::client_connect(std::string client_uuid, std::shared_ptr<juggle::Ichannel> gate_ch) {
	if (ch_gates.find(gate_ch) == ch_gates.end()) {
		return;
	}

	if (clients.find(client_uuid) != clients.end()) {
		return;
	}

	spdlog::trace("reg client:{0}", client_uuid);

	clients[client_uuid] = ch_gates[gate_ch];
	clients[client_uuid]->connect_sucess(client_uuid);

	_hub->sig_client_connect(client_uuid);
}

void gatemanager::client_direct_connect(std::string client_uuid, std::shared_ptr<service::webchannel> direct_ch) {
	std::scoped_lock l(_mu);

	if (direct_clients.find(client_uuid) != direct_clients.end()) {
		return;
	}

	spdlog::trace("reg direct client:{0}", client_uuid);

	ch_uuid_direct_clients.insert(std::make_pair(direct_ch, client_uuid));

	auto _directproxy = Fossilizid::pool::factory::create<directproxy>(direct_ch);
	direct_clients.insert(std::make_pair(client_uuid, _directproxy));

	_hub->sig_direct_client_connect(client_uuid);
}

void gatemanager::client_direct_disconnect(std::shared_ptr<juggle::Ichannel> direct_ch) {
	std::scoped_lock l(_mu);

	auto it = ch_uuid_direct_clients.find(direct_ch);
	if (it == ch_uuid_direct_clients.end()) {
		return;
	}

	direct_clients[it->second]->dispose();

	direct_clients.erase(it->second);
	ch_uuid_direct_clients.erase(it);

	spdlog::trace("client_direct_disconnect direct_clients.size:{0}, ch_uuid_direct_clients.size:{1}!", direct_clients.size(), ch_uuid_direct_clients.size());
}

void gatemanager::client_direct_exception(std::shared_ptr<juggle::Ichannel> direct_ch) {
	std::scoped_lock l(_mu);

	auto it = ch_uuid_direct_clients.find(direct_ch);
	if (it == ch_uuid_direct_clients.end()) {
		return;
	}

	spdlog::trace("client_direct_exception!");

	direct_clients[it->second]->dispose();

	direct_clients.erase(it->second);
	ch_uuid_direct_clients.erase(it);

	if (!_hub->sig_client_exception.empty()) {
		_hub->sig_client_exception(it->second);
	}
}

void gatemanager::client_disconnect(std::string client_uuid) {
	if (clients.find(client_uuid) == clients.end()) {
		return;
	}

	clients.erase(client_uuid);
	_hub->sig_client_disconnect(client_uuid);
}

void gatemanager::client_exception(std::string client_uuid) {
	_hub->sig_client_exception(client_uuid);
}

void gatemanager::disconnect_client(std::string uuid) {
	if (clients.find(uuid) == clients.end()) {
		return;
	}

	clients[uuid]->disconnect_client(uuid);
	clients.erase(uuid);
}

void gatemanager::call_client(const std::string& uuid, const std::string& _module, const std::string& func, const Fossilizid::JsonParse::JsonArray& argvs) {
	std::shared_lock l(_mu);

	auto it_direct_client = direct_clients.find(uuid);
	if (it_direct_client != direct_clients.end()) {
		it_direct_client->second->call_client(_module, func, argvs);
		return;
	}

	if (clients.find(uuid) == clients.end()) {
		return;
	}

	clients[uuid]->forward_hub_call_client(uuid, _module, func, argvs);
}

void gatemanager::call_group_client(const std::vector<std::string>& uuids, const std::string& _module, const std::string& func, const Fossilizid::JsonParse::JsonArray& argvs) {
	std::shared_lock l(_mu);
	
	static unsigned char* _data = nullptr;
	static size_t _data_len = 0;

	auto v = Fossilizid::JsonParse::Make_JsonArray();
	v->push_back("hub_call_client");
	v->push_back("call_client");
	v->push_back(Fossilizid::JsonParse::Make_JsonArray());
	(std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(_module);
	(std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(func);
	(std::any_cast<Fossilizid::JsonParse::JsonArray>((*v)[2]))->push_back(argvs);
	std::string data;
	Fossilizid::JsonParse::pack(v, data);
	size_t len = data.size();
	if (_data == nullptr) {
		_data = (unsigned char*)angmalloc(len + 4);
		_data_len = len + 4;
	}
	else if (_data_len < (len + 4)) {
		_data = (unsigned char*)angrealloc(_data, len + 4);
		_data_len = len + 4;
	}
	_data[0] = len & 0xff;
	_data[1] = len >> 8 & 0xff;
	_data[2] = len >> 16 & 0xff;
	_data[3] = len >> 24 & 0xff;
	memcpy(&_data[4], data.c_str(), data.size());
	size_t datasize = len + 4;
	//std::vector<std::string> tmp_uuids;
	for (auto _uuid : uuids) {
		auto it_direct_client = direct_clients.find(_uuid);
		if (it_direct_client != direct_clients.end()) {
			it_direct_client->second->direct_ch->send((char*)_data, datasize);
		}

		//tmp_uuids.push_back(_uuid);
	}

	//auto array_uuid = Fossilizid::JsonParse::Make_JsonArray();
	//std::vector<std::shared_ptr<gateproxy> > tmp_gates;
	//for (auto uuid : tmp_uuids) {
	//	auto it_gate = clients.find(uuid);
	//	if (it_gate == clients.end()) {
	//		continue;
	//	}

	//	array_uuid->push_back(uuid);

	//	if (std::find(tmp_gates.begin(), tmp_gates.end(), it_gate->second) != tmp_gates.end()) {
	//		continue;
	//	}

	//	tmp_gates.push_back(it_gate->second);
	//}

	//for (auto gate_proxy : tmp_gates) {
	//	gate_proxy->forward_hub_call_group_client(array_uuid, _module, func, argvs);
	//}
}

void gatemanager::call_global_client(std::string _module, std::string func, Fossilizid::JsonParse::JsonArray argvs) {
	std::shared_lock l(_mu);
	
	for (auto _client : direct_clients) {
		_client.second->call_client(_module, func, argvs);
	}

	for (auto gate : gates) {
		gate.second->forward_hub_call_global_client(_module, func, argvs);
	}
}

}