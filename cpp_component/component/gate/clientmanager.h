/*
 * clientmanager.h
 *
 *  Created on: 2016-7-12
 *      Author: qianqians
 */
#ifndef _clientmanager_h
#define _clientmanager_h

#include <omp.h>

#include <vector>
#include <queue>
#include <set>
#include <unordered_map>

#include <crossguid/guid.hpp>

#include <spdlog/spdlog.h>

#include <abelkhan.h>
#include <client.h>

#include "timerservice.h"
#include "hubsvrmanager.h"
#include "gc_poll.h"

namespace gate {

class clientmanager;
class clientproxy {
private:
	mutable concurrent::ringque<std::pair<char*, int> > wait_send_buf;

public:
	int64_t _timetmp = 0;
	int64_t _theory_timetmp = 0;
	std::string _cuuid;

	std::mutex _conn_hubproxys_mutex;
	std::vector<hubproxy*> conn_hubproxys;

	std::shared_ptr<abelkhan::Ichannel> _ch;
	int index = 0;
	abelkhan::gate_call_client_caller _gate_call_client_caller;

	clientmanager* _cli_mgr = nullptr;

public:
	clientproxy() : _gate_call_client_caller(nullptr, service::_modulemng) {
	}

	clientproxy(const clientproxy & other) : _gate_call_client_caller(nullptr, service::_modulemng) {
		std::pair<char*, int> buf;
		while (other.wait_send_buf.pop(buf)) {
			wait_send_buf.push(buf);
		}

		conn_hubproxys = other.conn_hubproxys;

		_timetmp = other._timetmp;
		_theory_timetmp = other._theory_timetmp;
		_cuuid = other._cuuid;
		_ch = other._ch;
		_gate_call_client_caller.reset_channel(_ch);
		index = other.index;
		_cli_mgr = other._cli_mgr;
	}

	void init(std::string& cuuid, std::shared_ptr<abelkhan::Ichannel> ch, int _index, clientmanager* cli_mgr) {
		wait_send_buf.clear();
		conn_hubproxys.clear();

		_cuuid = cuuid;
		_ch = ch;
		_gate_call_client_caller.reset_channel(ch);

		index = _index;

		_cli_mgr = cli_mgr;
	}

	virtual ~clientproxy() {
	}

	void conn_hub(hubproxy* hub_proxy) {
		std::lock_guard<std::mutex> l(_conn_hubproxys_mutex);
		if (std::find(conn_hubproxys.begin(), conn_hubproxys.end(), hub_proxy) == conn_hubproxys.end()) {
			conn_hubproxys.push_back(hub_proxy);
		}
	}

	void unreg_hub(hubproxy* hub_proxy) {
		std::lock_guard<std::mutex> l(_conn_hubproxys_mutex);
		auto it = std::find(conn_hubproxys.begin(), conn_hubproxys.end(), hub_proxy);
		if (it == conn_hubproxys.end()) {
			conn_hubproxys.erase(it);
		}
	}

	void ntf_cuuid() {
		_gate_call_client_caller.ntf_cuuid(_cuuid);
	}

	void call_client(std::string hub_name, std::vector<uint8_t>& data) {
		_gate_call_client_caller.call_client(hub_name, data);
	}

	void send_buf(char* _data, int datasize);

	bool is_xor_key_crypt() {
		return _ch->is_xor_key_crypt();
	}

	void done_send() {
		std::pair<char*, int> buf;
		while (wait_send_buf.pop(buf)) {
			_ch->send(buf.first, buf.second);
		}
	}

};

class clientmanager {
public:
	clientmanager(size_t _limit_client, hubsvrmanager* _hubsvrmanager_) {
		_hubsvrmanager = _hubsvrmanager_;

		client_proxy_pool.resize(_limit_client);
		for (int i = 0; i < _limit_client; i++)
		{
			client_proxy_recycle_pool.push(i);
		}
	}

	virtual ~clientmanager() {
	}

	void heartbeat_client(int64_t ticktime) {
		std::vector<std::shared_ptr<clientproxy> > remove_client;
		std::vector<std::shared_ptr<clientproxy> > exception_client;
		for (auto item : client_map) {
			auto proxy = item.second;
			if (proxy->_timetmp > 0 && (proxy->_timetmp + 10 * 1000) < ticktime) {
				remove_client.push_back(proxy);
			}
			if (proxy->_timetmp > 0 && proxy->_theory_timetmp > 0 && (proxy->_theory_timetmp - proxy->_timetmp) > 10 * 1000) {
				exception_client.push_back(proxy);
			}
		}

		for (auto& _client : remove_client) {
			for (auto& hubproxy_ : _client->conn_hubproxys) {
				hubproxy_->client_disconnect(_client->_cuuid);
			}
			unreg_client(_client->_ch);
		}

		for (auto& proxy : exception_client) {
			for (auto& hubproxy_ : proxy->conn_hubproxys) {
				hubproxy_->client_exception(proxy->_cuuid);
			}
		}
	}

	int pop_client_proxy_from_pool() {
		if (!client_proxy_recycle_pool.empty()) {
			auto index = client_proxy_recycle_pool.top();
			client_proxy_recycle_pool.pop();

			return index;
		}
		return -1;
	}

	static void recycle_client_proxy(clientmanager* _cli_mgr, clientproxy * _proxy) {
		_proxy->_ch = nullptr;
		_proxy->_cli_mgr = nullptr;

		_cli_mgr->client_proxy_recycle_pool.push(_proxy->index);
	}

	std::shared_ptr<clientproxy> reg_client(std::shared_ptr<abelkhan::Ichannel> ch) {
		auto cuuid = xg::newGuid().str();
		auto _cli_mgr = this;

		auto index = pop_client_proxy_from_pool();
		if (index < 0) {
			return nullptr;
		}

		auto client_proxy = &client_proxy_pool[index];
		client_proxy->init(cuuid, ch, index, _cli_mgr);
		auto _client = std::shared_ptr<clientproxy>(client_proxy, std::bind(&clientmanager::recycle_client_proxy, _cli_mgr, std::placeholders::_1));

		client_map.insert(std::make_pair(cuuid, _client));
		client_uuid_map.insert(std::make_pair(ch, _client));

		_client->_timetmp = 0;

		return _client;
	}

	void unreg_client(std::shared_ptr<abelkhan::Ichannel> _ch){
		if (client_uuid_map.find(_ch) == client_uuid_map.end()){
			return;
		}

		auto _client = client_uuid_map[_ch];
		spdlog::trace("unreg_client:{0}", _client->_cuuid);

		if (client_map.find(_client->_cuuid) != client_map.end())
		{
			client_map.erase(_client->_cuuid);
		}
		if (client_uuid_map.find(_ch) != client_uuid_map.end())
		{
			client_uuid_map.erase(_ch);
		}
	}

	void mark_client_proxy_send(int index) {
		wait_send_cli.insert(index);
	}

	void client_proxy_send() {
		omp_set_nested(1);
#pragma omp parallel for
		for (int index = 0; index < wait_send_cli.size(); ++index) {
			client_proxy_pool[index].done_send();
		}
		wait_send_cli.clear();
	}

	bool has_client(std::shared_ptr<abelkhan::Ichannel> ch) {
		return client_uuid_map.find(ch) != client_uuid_map.end();
	}

	bool has_client(std::string uuid) {
		return client_map.find(uuid) != client_map.end();
	} 

	std::shared_ptr<clientproxy> get_client(std::shared_ptr<abelkhan::Ichannel> ch) {
		auto it = client_uuid_map.find(ch);
		if (it != client_uuid_map.end()) {
			return it->second;
		}
		return nullptr;
	}

	std::shared_ptr<clientproxy> get_client(std::string cuuid) {
		auto it = client_map.find(cuuid);
		if (it != client_map.end()) {
			return it->second;
		}
		return nullptr;
	}

	void for_each_client(std::function<void(std::string, std::shared_ptr<clientproxy>)> fn) {
		for (auto client : client_map){
			fn(client.first, client.second);
		}
	}

private:
	std::unordered_map<std::string, std::shared_ptr<clientproxy> > client_map;
	std::unordered_map<std::shared_ptr<abelkhan::Ichannel>, std::shared_ptr<clientproxy> > client_uuid_map;

	std::vector<clientproxy> client_proxy_pool;
	std::priority_queue<int, std::vector<int>, std::greater<int> > client_proxy_recycle_pool;

	std::set<int> wait_send_cli;

	hubsvrmanager* _hubsvrmanager;

};

}

#endif //_clientmanager_h
