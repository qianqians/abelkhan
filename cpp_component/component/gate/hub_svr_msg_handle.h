/*
 * hub_svr_msg_handle.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _hub_svr_msg_handle_h
#define _hub_svr_msg_handle_h

#include <abelkhan.h>

#include <hub.h>
#include <gate.h>

#include <modulemng_handle.h>

#include "hubsvrmanager.h"
#include "clientmanager.h"

namespace gate {

class hub_svr_msg_handle {
private:
	size_t _data_size;
	unsigned char* _data;

	std::shared_ptr<clientmanager> _clientmanager;
	std::shared_ptr<hubsvrmanager> _hubsvrmanager;

	std::shared_ptr<abelkhan::hub_call_gate_module> _hub_call_gate_module;

public:
	hub_svr_msg_handle(std::shared_ptr<clientmanager> clientmanager_, std::shared_ptr<hubsvrmanager> hubsvrmanager_) {
		_data_size = 8 * 1024;
		_data = (unsigned char*)malloc(_data_size);

		_clientmanager = clientmanager_;
		_hubsvrmanager = hubsvrmanager_;

		_hub_call_gate_module = std::make_shared<abelkhan::hub_call_gate_module>();
		_hub_call_gate_module->Init(service::_modulemng);
		_hub_call_gate_module->sig_reg_hub.connect(std::bind(&hub_svr_msg_handle::reg_hub, this, std::placeholders::_1, std::placeholders::_2));
		_hub_call_gate_module->sig_disconnect_client.connect(std::bind(&hub_svr_msg_handle::disconnect_client, this, std::placeholders::_1));
		_hub_call_gate_module->sig_forward_hub_call_client.connect(std::bind(&hub_svr_msg_handle::forward_hub_call_client, this, std::placeholders::_1, std::placeholders::_2));
		_hub_call_gate_module->sig_forward_hub_call_group_client.connect(std::bind(&hub_svr_msg_handle::forward_hub_call_group_client, this, std::placeholders::_1, std::placeholders::_2));
		_hub_call_gate_module->sig_forward_hub_call_global_client;
	}

	virtual ~hub_svr_msg_handle() {
		free(_data);
	}

	void reg_hub(std::string hub_name, std::string hub_type) {
		auto rsp = std::static_pointer_cast<abelkhan::hub_call_gate_reg_hub_rsp>(_hub_call_gate_module->rsp);
		auto ch = _hub_call_gate_module->current_ch;
		auto proxy = _hubsvrmanager->reg_hub(hub_name, hub_type, ch);
		rsp->rsp();
	}

	void disconnect_client(std::string cuuid) {
		auto proxy = _clientmanager->get_client(cuuid);
		proxy->_ch->disconnect();
	}

	void forward_hub_call_client(std::string cuuid, std::vector<uint8_t> rpc_argv) {
		auto ch = _hub_call_gate_module->current_ch;
		auto hub_proxy = _hubsvrmanager->get_hub(ch);
		auto client_proxy = _clientmanager->get_client(cuuid);
		if (client_proxy) {
			client_proxy->call_client(hub_proxy->_hub_name, rpc_argv);
		}
	}

	void forward_hub_call_group_client(std::vector<std::string> cuuids, std::vector<uint8_t> rpc_argv) {
		auto ch = _hub_call_gate_module->current_ch;
		auto hub_proxy = _hubsvrmanager->get_hub(ch);

		msgpack11::MsgPack::array _argv_array;
		_argv_array.push_back(hub_proxy->_hub_name);
		_argv_array.push_back(rpc_argv);

		msgpack11::MsgPack::array event_;
		event_.push_back("gate_call_client");
		event_.push_back("call_client");
		event_.push_back(_argv_array);
		msgpack11::MsgPack _pack(event_);
		auto data = _pack.dump();

		size_t len = data.size();
		if (_data_size < (len + 4)) {
			_data_size *= 2;
			free(_data);
			_data = (unsigned char*)malloc(_data_size);
		}
		_data[0] = len & 0xff;
		_data[1] = len >> 8 & 0xff;
		_data[2] = len >> 16 & 0xff;
		_data[3] = len >> 24 & 0xff;
		memcpy(&_data[4], data.c_str(), data.size());
		size_t datasize = len + 4;

		std::vector<std::shared_ptr<clientproxy> > clients;
		for (auto cuuid : cuuids) {
			auto client_proxy = _clientmanager->get_client(cuuid);
			if (client_proxy != nullptr) {
				clients.push_back(client_proxy);
			}
		}
		for (auto _client : clients) {
			_client->_ch->send((char*)_data, datasize);
		}
	}

	void forward_hub_call_global_client(std::vector<uint8_t> rpc_argv) {
		auto ch = _hub_call_gate_module->current_ch;
		auto hub_proxy = _hubsvrmanager->get_hub(ch);

		msgpack11::MsgPack::array _argv_array;
		_argv_array.push_back(hub_proxy->_hub_name);
		_argv_array.push_back(rpc_argv);

		msgpack11::MsgPack::array event_;
		event_.push_back("gate_call_client");
		event_.push_back("call_client");
		event_.push_back(_argv_array);
		msgpack11::MsgPack _pack(event_);
		auto data = _pack.dump();

		size_t len = data.size();
		if (_data_size < (len + 4)) {
			_data_size *= 2;
			free(_data);
			_data = (unsigned char*)malloc(_data_size);
		}
		_data[0] = len & 0xff;
		_data[1] = len >> 8 & 0xff;
		_data[2] = len >> 16 & 0xff;
		_data[3] = len >> 24 & 0xff;
		memcpy(&_data[4], data.c_str(), data.size());
		size_t datasize = len + 4;

		_clientmanager->for_each_client([this, datasize](std::string cuuid, std::shared_ptr<clientproxy> _client) {
			_client->_ch->send((char*)_data, datasize);
		});
	}
};

}

#endif //_hub_svr_msg_handle_h
