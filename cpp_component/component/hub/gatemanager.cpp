/*
 * qianqians
 * 2020-1-10
 * gatemanager.cpp
 */
#include <enetacceptservice.h>
#include <modulemng_handle.h>

#include "gatemanager.h"
#include "hub_service.h"

namespace hub {
	
gateproxy::gateproxy(std::shared_ptr<abelkhan::Ichannel> ch, std::shared_ptr<hub_service> hub, std::string gate_name) {
	_ch = ch;
	_hub_call_gate_caller = std::make_shared<abelkhan::hub_call_gate_caller>(ch, service::_modulemng);
	_hub = hub;
	_gate_name = gate_name;
}

void gateproxy::reg_hub() {
	_hub_call_gate_caller->reg_hub(_hub->name_info.name, _hub->hub_type)->callBack([this]() {
		spdlog::trace("hub reg_hub to gate:{0} sucessed", _gate_name);
		sig_reg_hub_sucessed.emit();
	}, [this]() {
		spdlog::trace("hub reg_hub to gate:{0} faild", _gate_name);
	})->timeout(5 * 1000, [this]() {
		spdlog::trace("hub reg_hub to gate:{0} timeout", _gate_name);
	});
}
void gateproxy::disconnect_client(std::string& cuuid) {
	_hub_call_gate_caller->disconnect_client(cuuid);
}

void gateproxy::forward_hub_call_client(const std::string& cuuid, const std::vector<uint8_t>& _data_bin) {
	_hub_call_gate_caller->forward_hub_call_client(cuuid, _data_bin);
}

void gateproxy::forward_hub_call_group_client(const std::vector<std::string>& cuuids, const std::vector<uint8_t>& rpc_argv) {
	_hub_call_gate_caller->forward_hub_call_group_client(cuuids, rpc_argv);
}

void gateproxy::forward_hub_call_global_client(const std::vector<uint8_t>& rpc_argv) {
	_hub_call_gate_caller->forward_hub_call_global_client(rpc_argv);
}

directproxy::directproxy(std::string cuuid_, std::shared_ptr<abelkhan::Ichannel> direct_ch) {
	_hub_call_client_caller = std::make_shared<abelkhan::hub_call_client_caller>(direct_ch, service::_modulemng);
	
	_cuuid = cuuid_;
	_direct_ch = direct_ch;
}

void directproxy::call_client(const std::vector<uint8_t>& rpc_argv) {
	_hub_call_client_caller->call_client(rpc_argv);
}

gatemanager::gatemanager(std::shared_ptr<service::enetacceptservice> conn_, std::shared_ptr<hub_service> hub_) {
	_conn = conn_;
	_hub = hub_;

	_hub->sig_svr_be_closed.connect([this](std::string svr_type, std::string svr_name) {
		if (svr_type != "gate") {
			return;
		}

		auto old_it = wait_destory_gates.find(svr_name);
		if (old_it != wait_destory_gates.end()) {
			for (auto it = clients.begin(); it != clients.end(); ) {
				if (it->second == old_it->second) {
					it = clients.erase(it);
				}
				else {
					it++;
				}
			}

			ch_gates.erase(old_it->second->_ch);
			wait_destory_gates.erase(old_it);
		}
		else {
			for (auto it = clients.begin(); it != clients.end(); ) {
				if (it->second->_gate_name == svr_name) {
					it = clients.erase(it);
				}
				else {
					it++;
				}
			}

			auto it = gates.find(svr_name);
			if (it != gates.end()) {
				ch_gates.erase(it->second->_ch);
				gates.erase(it);

				_hub->sig_gate_closed.emit(svr_name);
			}
		}
	});

	_data_size = 8 * 1024;
	_data = (unsigned char*)malloc(_data_size);
	_crypt_data = (unsigned char*)malloc(_data_size);
}

void gatemanager::connect_gate(std::string gate_name, std::string host, uint16_t port) {
	spdlog::trace("connect_gate host:{0} port:{1}", host, port);
	auto ip = service::DNS(host);
	_conn->connect(ip, port, [this, host, port, gate_name](std::shared_ptr<abelkhan::Ichannel> ch) {
		spdlog::trace("gate host:{0}  port:{1} reg_hub", host, port);
		
		auto _gate_proxy = std::make_shared<gateproxy>(ch, _hub, gate_name);
		_gate_proxy->sig_reg_hub_sucessed.connect([this, gate_name, ch, _gate_proxy]() {

			auto it = gates.find(gate_name);
			if (it != gates.end()) {
				wait_destory_gates.insert(std::make_pair(it->first, it->second));
			}

			gates[gate_name] = _gate_proxy;
			ch_gates[ch] = _gate_proxy;
		});
		_gate_proxy->reg_hub();
	});
}

void gatemanager::client_connect(std::string client_uuid, std::shared_ptr<abelkhan::Ichannel> gate_ch) {
	if (ch_gates.find(gate_ch) == ch_gates.end()) {
		spdlog::trace("unreg gate channel!");
		return;
	}

	spdlog::trace("reg client:{0}", client_uuid);

	clients[client_uuid] = ch_gates[gate_ch];
}

void gatemanager::client_disconnect(std::string client_uuid) {
	if (clients.find(client_uuid) == clients.end()) {
		return;
	}

	clients.erase(client_uuid);
	_hub->sig_client_disconnect.emit(client_uuid);
}

void gatemanager::client_direct_connect(std::string client_uuid, std::shared_ptr<abelkhan::Ichannel> direct_ch) {
	auto it = direct_clients.find(client_uuid);
	if (it != direct_clients.end()) {
		ch_direct_clients.erase(it->second->_direct_ch);
		direct_clients.erase(it);
	}

	spdlog::trace("reg direct client:{0}", client_uuid);

	auto _directproxy = _directproxy_pool.make_obj(client_uuid, direct_ch);
	direct_clients.insert(std::make_pair(client_uuid, _directproxy));

	ch_direct_clients.insert(std::make_pair(direct_ch, _directproxy));
}

void gatemanager::client_direct_disconnect(std::shared_ptr<abelkhan::Ichannel> direct_ch) {
	auto it = ch_direct_clients.find(direct_ch);
	if (it == ch_direct_clients.end()) {
		return;
	}

	auto cuuid = it->second->_cuuid;
	_directproxy_pool.recycle(it->second);

	direct_clients.erase(cuuid);
	ch_direct_clients.erase(it);

	_hub->sig_direct_client_disconnect.emit(cuuid);


	spdlog::trace("client_direct_disconnect direct_clients.size:{0}, ch_direct_clients.size:{1}!", direct_clients.size(), ch_direct_clients.size());
}

std::shared_ptr<directproxy> gatemanager::get_direct_client(std::shared_ptr<abelkhan::Ichannel> direct_ch) {
	auto it = ch_direct_clients.find(direct_ch);
	if (it == ch_direct_clients.end()) {
		return nullptr;
	}

	return it->second;
}

void gatemanager::disconnect_client(std::string uuid) {
	auto it_gate = clients.find(uuid);
	if (it_gate != clients.end()) {
		it_gate->second->disconnect_client(uuid);
		clients.erase(it_gate);
	}

	auto it_direct = direct_clients.find(uuid);
	if (it_direct != direct_clients.end()) {
		it_direct->second->_direct_ch->disconnect();
		ch_direct_clients.erase(it_direct->second->_direct_ch);
		direct_clients.erase(it_direct);
	}
}

void gatemanager::heartbeat_client(int64_t ticktime) {
	std::vector<std::shared_ptr<directproxy> > remove_client;
	std::vector<std::shared_ptr<directproxy> > exception_client;
	for (auto item : direct_clients) {
		auto proxy = item.second;
		if (proxy->_timetmp > 0 && (proxy->_timetmp + 10 * 1000) < ticktime) {
			remove_client.push_back(proxy);
		}
		if (proxy->_timetmp > 0 && proxy->_theory_timetmp > 0 && (proxy->_theory_timetmp - proxy->_timetmp) > 10 * 1000) {
			exception_client.push_back(proxy);
		}
	}

	for (auto _client : remove_client) {
		client_direct_disconnect(_client->_direct_ch);
	}

	for (auto _client : exception_client) {
		_hub->sig_client_exception.emit(_client->_cuuid);
	}
}

void gatemanager::call_client(const std::string& cuuid, const std::string& func, const msgpack11::MsgPack::array& argvs) {
	msgpack11::MsgPack::array event_;
	event_.push_back(func);
	event_.push_back(argvs);
	msgpack11::MsgPack _pack(event_);
	auto data = _pack.dump();
	std::vector<uint8_t> _data_bin;
	_data_bin.resize(data.size());
	memcpy(_data_bin.data(), data.data(), data.size());
	
	auto it_direct_client = direct_clients.find(cuuid);
	if (it_direct_client != direct_clients.end()) {
		it_direct_client->second->call_client(_data_bin);
		return;
	}

	if (clients.find(cuuid) == clients.end()) {
		spdlog::error("unreg client cuuid:{0}", cuuid);
		return;
	}

	clients[cuuid]->forward_hub_call_client(cuuid, _data_bin);

}

void gatemanager::call_group_client(const std::vector<std::string>& cuuids, const std::string& func, const msgpack11::MsgPack::array& argvs) {
	msgpack11::MsgPack::array event_;
	event_.push_back(func);
	event_.push_back(argvs);
	msgpack11::MsgPack _pack(event_);
	auto data = _pack.dump();
	std::vector<uint8_t> _data_bin;
	_data_bin.resize(data.size());
	memcpy(_data_bin.data(), data.data(), data.size());

	std::vector<std::shared_ptr<directproxy> > directs;
	std::unordered_map<std::shared_ptr<gateproxy>, std::vector<std::string> > gate_clients;
	for (auto cuuid : cuuids) {
		auto it_direct_client = direct_clients.find(cuuid);
		if (it_direct_client != direct_clients.end()) {
			directs.push_back(it_direct_client->second);
			continue;
		}

		auto it_gate = clients.find(cuuid);
		if (it_gate != clients.end()) {
			auto it = gate_clients.find(it_gate->second);
			if (it == gate_clients.end()) {
				gate_clients.insert(std::make_pair(it_gate->second, std::vector<std::string>()));
				it = gate_clients.find(it_gate->second);
			}
			it->second.push_back(cuuid);
		}
	}

	{
		if (!directs.empty()) {
			msgpack11::MsgPack::array _argv_array;
			_argv_array.push_back(_data_bin);

			msgpack11::MsgPack::array event_;
			event_.push_back("hub_call_client_call_client");
			event_.push_back(_argv_array);
			msgpack11::MsgPack _pack(event_);
			auto data = _pack.dump();

			size_t len = data.size();
			if (_data_size < (len + 4)) {
				_data_size = ((len + 4 + _data_size - 1) / _data_size) * _data_size;
				free(_data);
				free(_crypt_data);
				_data = (unsigned char*)malloc(_data_size);
				_crypt_data = (unsigned char*)malloc(_data_size);
			}
			_data[0] = len & 0xff;
			_data[1] = len >> 8 & 0xff;
			_data[2] = len >> 16 & 0xff;
			_data[3] = len >> 24 & 0xff;
			memcpy(&_data[4], data.c_str(), data.size());
			size_t datasize = len + 4;

			memcpy(_crypt_data, _data, datasize);
			service::channel_encrypt_decrypt_ondata::xor_key_encrypt_decrypt((char*)(&(_crypt_data[4])), len);

			std::vector<std::shared_ptr<abelkhan::Ichannel> > crypt_chs;
			std::vector<std::shared_ptr<abelkhan::Ichannel> > chs;
			for (auto direct_proxy : directs) {
				if (direct_proxy->_direct_ch->is_xor_key_crypt()) {
					crypt_chs.push_back(direct_proxy->_direct_ch);
				}
				else {
					chs.push_back(direct_proxy->_direct_ch);
				}
			}

			for (auto ch : crypt_chs) {
				ch->send((char*)_crypt_data, datasize);
			}
			for (auto ch : chs) {
				ch->send((char*)_data, datasize);
			}
		}
	}

	{
		for (auto it : gate_clients) {
			it.first->forward_hub_call_group_client(it.second, _data_bin);
		}
	}
}

void gatemanager::call_global_client(const std::string& func, const msgpack11::MsgPack::array& argvs) {
	msgpack11::MsgPack::array event_;
	event_.push_back(func);
	event_.push_back(argvs);
	msgpack11::MsgPack _pack(event_);
	auto data = _pack.dump();
	std::vector<uint8_t> _data_bin;
	_data_bin.resize(data.size());
	memcpy(_data_bin.data(), data.data(), data.size());

	for (auto gate : gates) {
		gate.second->forward_hub_call_global_client(_data_bin);
	}
}

}