/*
 * hub_svr_msg_handle.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _hub_svr_msg_handle_h
#define _hub_svr_msg_handle_h

#include <omp.h>

#include <abelkhan.h>

#include <hub.h>
#include <gate.h>

#include <modulemng_handle.h>
#include <buffer.h>

#include "hubsvrmanager.h"
#include "clientmanager.h"

namespace gate {

class hub_svr_msg_handle {
private:
	std::shared_ptr<clientmanager> _clientmanager;
	std::shared_ptr<hubsvrmanager> _hubsvrmanager;

	std::shared_ptr<abelkhan::hub_call_gate_module> _hub_call_gate_module;

public:
	hub_svr_msg_handle(std::shared_ptr<clientmanager> clientmanager_, std::shared_ptr<hubsvrmanager> hubsvrmanager_) {
		_clientmanager = clientmanager_;
		_hubsvrmanager = hubsvrmanager_;

		_hub_call_gate_module = std::make_shared<abelkhan::hub_call_gate_module>();
		_hub_call_gate_module->Init(service::_modulemng);
		_hub_call_gate_module->sig_reg_hub.connect(std::bind(&hub_svr_msg_handle::reg_hub, this, std::placeholders::_1, std::placeholders::_2));
		_hub_call_gate_module->sig_reverse_reg_client_hub.connect(std::bind(&hub_svr_msg_handle::reverse_reg_client_hub, this, std::placeholders::_1));
		_hub_call_gate_module->sig_unreg_client_hub.connect(std::bind(&hub_svr_msg_handle::unreg_client_hub, this, std::placeholders::_1));
		_hub_call_gate_module->sig_disconnect_client.connect(std::bind(&hub_svr_msg_handle::disconnect_client, this, std::placeholders::_1));
		_hub_call_gate_module->sig_forward_hub_call_client.connect(std::bind(&hub_svr_msg_handle::forward_hub_call_client, this, std::placeholders::_1, std::placeholders::_2));
		_hub_call_gate_module->sig_forward_hub_call_group_client.connect(std::bind(&hub_svr_msg_handle::forward_hub_call_group_client, this, std::placeholders::_1, std::placeholders::_2));
		_hub_call_gate_module->sig_forward_hub_call_global_client.connect(std::bind(&hub_svr_msg_handle::forward_hub_call_global_client, this, std::placeholders::_1));
	}

	virtual ~hub_svr_msg_handle() {
	}

	void reg_hub(std::string hub_name, std::string hub_type) {
		auto rsp = std::static_pointer_cast<abelkhan::hub_call_gate_reg_hub_rsp>(_hub_call_gate_module->rsp);
		auto ch = _hub_call_gate_module->current_ch;
		auto proxy = _hubsvrmanager->reg_hub(hub_name, hub_type, ch);
		rsp->rsp();
	}

	void reverse_reg_client_hub(std::string client_uuid) {
		auto rsp = std::static_pointer_cast<abelkhan::hub_call_gate_reverse_reg_client_hub_rsp>(_hub_call_gate_module->rsp);
		auto proxy = _clientmanager->get_client(client_uuid);
		if (proxy) {
			auto hub_proxy = _hubsvrmanager->get_hub(_hub_call_gate_module->current_ch);
			proxy->conn_hub(hub_proxy);
			rsp->rsp();
		}
		else {
			rsp->err(abelkhan::framework_error::enum_framework_client_not_exist);
		}
	}

	void unreg_client_hub(std::string client_uuid) {
		auto proxy = _clientmanager->get_client(client_uuid);
		if (proxy) {
			auto hub_proxy = _hubsvrmanager->get_hub(_hub_call_gate_module->current_ch);
			proxy->unreg_hub(hub_proxy);
		}
	}

	void disconnect_client(std::string cuuid) {
		auto proxy = _clientmanager->get_client(cuuid);
		if (proxy) {
			proxy->_ch->disconnect();
		}
	}

	void forward_hub_call_client(std::string cuuid, std::vector<uint8_t> rpc_argv) {
		auto ch = _hub_call_gate_module->current_ch;
		auto hub_proxy = _hubsvrmanager->get_hub(ch);
		auto client_proxy = _clientmanager->get_client(cuuid);
		if (client_proxy) {
			client_proxy->conn_hub(hub_proxy);
			client_proxy->call_client(hub_proxy->_hub_name, rpc_argv);
		}
		else {
			hub_proxy->client_disconnect(cuuid);
		}
	}

	void forward_hub_call_group_client(std::vector<std::string> cuuids, std::vector<uint8_t> rpc_argv) {
		auto ch = _hub_call_gate_module->current_ch;
		auto hub_proxy = _hubsvrmanager->get_hub(ch);

		std::vector<std::shared_ptr<abelkhan::Ichannel> > crypt_clients;
		std::vector<std::shared_ptr<abelkhan::Ichannel> > clients;
		for (auto cuuid : cuuids) {
			auto client_proxy = _clientmanager->get_client(cuuid);
			if (client_proxy != nullptr) {
				if (client_proxy->_ch->is_xor_key_crypt()) {
					crypt_clients.push_back(client_proxy->_ch);
				}
				else {
					clients.push_back(client_proxy->_ch);
				}
			}
			else {
				hub_proxy->client_disconnect(cuuid);
			}
		}

		msgpack11::MsgPack::array _argv_array;
		_argv_array.push_back(hub_proxy->_hub_name);
		_argv_array.push_back(rpc_argv);

		msgpack11::MsgPack::array event_;
		event_.push_back("gate_call_client_call_client");
		event_.push_back(_argv_array);
		msgpack11::MsgPack _pack(event_);
		auto data = _pack.dump();

		size_t len = data.size();
		size_t datasize = len + 4;
		auto _data = service::get_buffer(datasize);

		_data[0] = len & 0xff;
		_data[1] = len >> 8 & 0xff;
		_data[2] = len >> 16 & 0xff;
		_data[3] = len >> 24 & 0xff;
		memcpy(&_data[4], data.c_str(), data.size());
#pragma omp parallel for
		for (auto _client : clients) {
			_client->send((char*)_data, datasize);
		}

		service::channel_encrypt_decrypt_ondata::xor_key_encrypt_decrypt((char*)(&(_data[4])), len);
#pragma omp parallel for
		for (auto _client : crypt_clients) {
			_client->send((char*)_data, datasize);
		}
	}

	void forward_hub_call_global_client(std::vector<uint8_t> rpc_argv) {
		auto ch = _hub_call_gate_module->current_ch;
		auto hub_proxy = _hubsvrmanager->get_hub(ch);

		std::vector<std::shared_ptr<abelkhan::Ichannel> > crypt_chs;
		std::vector<std::shared_ptr<abelkhan::Ichannel> > chs;
		_clientmanager->for_each_client([this, &crypt_chs, &chs](std::string cuuid, std::shared_ptr<clientproxy> _client) {
			if (_client->_ch->is_xor_key_crypt()) {
				crypt_chs.push_back(_client->_ch);
			}
			else {
				chs.push_back(_client->_ch);
			}
		});

		msgpack11::MsgPack::array _argv_array;
		_argv_array.push_back(hub_proxy->_hub_name);
		_argv_array.push_back(rpc_argv);

		msgpack11::MsgPack::array event_;
		event_.push_back("gate_call_client_call_client");
		event_.push_back(_argv_array);
		msgpack11::MsgPack _pack(event_);
		auto data = _pack.dump();

		size_t len = data.size();
		size_t datasize = len + 4;
		auto _data = service::get_buffer(datasize);

		_data[0] = len & 0xff;
		_data[1] = len >> 8 & 0xff;
		_data[2] = len >> 16 & 0xff;
		_data[3] = len >> 24 & 0xff;
		memcpy(&_data[4], data.c_str(), data.size());
#pragma omp parallel for
		for (auto _client : chs) {
			_client->send((char*)_data, datasize);
		}

		service::channel_encrypt_decrypt_ondata::xor_key_encrypt_decrypt((char*)(&(_data[4])), len);
#pragma omp parallel for
		for (auto _client : crypt_chs) {
			_client->send((char*)_data, datasize);
		}
	}
};

}

#endif //_hub_svr_msg_handle_h
