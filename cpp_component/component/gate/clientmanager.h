/*
 * clientmanager.h
 *
 *  Created on: 2016-7-12
 *      Author: qianqians
 */
#ifndef _clientmanager_h
#define _clientmanager_h

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
	concurrent::ringque<std::pair<char*, int> > wait_send_buf;

public:
	int64_t _timetmp = 0;
	int64_t _theory_timetmp = 0;
	std::string _cuuid;

	std::mutex _conn_hubproxys_mutex;
	std::vector<std::shared_ptr<hubproxy> > conn_hubproxys;

	std::shared_ptr<abelkhan::Ichannel> _ch;
	int index1 = 0;
	int index2 = 0;
	abelkhan::gate_call_client_caller _gate_call_client_caller;

	std::shared_ptr<clientmanager> _cli_mgr = nullptr;

public:
	clientproxy() : _gate_call_client_caller(nullptr, service::_modulemng) {
	}

	clientproxy(const clientproxy & _) : _gate_call_client_caller(nullptr, service::_modulemng) {
	}

	void init(std::string& cuuid, std::shared_ptr<abelkhan::Ichannel> ch, int _index1, int _index2, std::shared_ptr<clientmanager> cli_mgr) {
		wait_send_buf.clear();
		conn_hubproxys.clear();

		_cuuid = cuuid;
		_ch = ch;
		_gate_call_client_caller.reset_channel(ch);

		index1 = _index1;
		index2 = _index2;

		_cli_mgr = cli_mgr;
	}

	virtual ~clientproxy() {
	}

	void conn_hub(std::shared_ptr<hubproxy> hub_proxy) {
		std::lock_guard<std::mutex> l(_conn_hubproxys_mutex);
		if (std::find(conn_hubproxys.begin(), conn_hubproxys.end(), hub_proxy) == conn_hubproxys.end()) {
			conn_hubproxys.push_back(hub_proxy);
		}
	}

	void unreg_hub(std::shared_ptr<hubproxy> hub_proxy) {
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
		std::pair<char*, int> buf; ;
		while (wait_send_buf.pop(buf)) {
			_ch->send(buf.first, buf.second);
		}
	}

};

class clientmanager : public std::enable_shared_from_this<clientmanager> {
public:
	clientmanager(std::shared_ptr<hubsvrmanager> _hubsvrmanager_) {
		_hubsvrmanager = _hubsvrmanager_;

		client_proxy_pool.resize(1);
		client_proxy_pool[0].resize(8192);

		client_proxy_recycle_pool.resize(1);
		for (int i = 0; i < 8192; i++)
		{
			client_proxy_recycle_pool[0].push(i);
		}

		wait_send_cli.resize(1);
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

		for (auto _client : remove_client){
			for (auto hubproxy_ : _client->conn_hubproxys) {
				hubproxy_->client_disconnect(_client->_cuuid);
			}
			unreg_client(_client->_ch);
		}

		for (auto proxy : exception_client) {
			for (auto hubproxy_ : proxy->conn_hubproxys) {
				hubproxy_->client_exception(proxy->_cuuid);
			}
		}
	}

	void scaler_client_proxy_pool() {
		auto index = client_proxy_pool.size();
		client_proxy_pool.push_back(std::vector<clientproxy>());
		client_proxy_pool[index].resize(8192);

		client_proxy_recycle_pool.push_back(std::priority_queue<int, std::vector<int>, std::greater<int> >());
		for (int i = 0; i < 8192; i++)
		{
			client_proxy_recycle_pool[index].push(i);
		}

		wait_send_cli.push_back(std::set<int>());
	}

	std::pair<int, int> pop_client_proxy_from_pool() {
		int index1 = 0, index2 = 0;
		for (; index1 < client_proxy_recycle_pool.size(); ++index1)
		{
			if (!client_proxy_recycle_pool[index1].empty()) {
				index2 = client_proxy_recycle_pool[index1].top();
				client_proxy_recycle_pool[index1].pop();

				return { index1, index2 };
			}
		}

		scaler_client_proxy_pool();

		index2 = client_proxy_recycle_pool[index1].top();
		client_proxy_recycle_pool[index1].pop();

		return { index1, index2 };
	}

	static void recycle_client_proxy(std::shared_ptr<clientmanager> _cli_mgr, clientproxy * _proxy) {
		_cli_mgr->client_proxy_recycle_pool[_proxy->index1].push(_proxy->index2);
	}

	std::shared_ptr<clientproxy> reg_client(std::shared_ptr<abelkhan::Ichannel> ch) {
		std::string cuuid = xg::newGuid().str();

		auto [index1, index2] = pop_client_proxy_from_pool();
		auto client_proxy = &client_proxy_pool[index1][index2];
		client_proxy->init(cuuid, ch, index1, index2, shared_from_this());
		auto _client = std::shared_ptr<clientproxy>(client_proxy, std::bind(&clientmanager::recycle_client_proxy, shared_from_this(), std::placeholders::_1));

		client_map.insert(std::make_pair(cuuid, _client));
		client_uuid_map.insert(std::make_pair(ch, _client));

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

	void mark_client_proxy_send(int index1, int index2) {
		wait_send_cli[index1].insert(index2);
	}

	void client_proxy_send() {
		for (int index1 = 0; index1 < wait_send_cli.size(); index1++) {
			for (int index2 : wait_send_cli[index1]) {
				auto client_proxy = &client_proxy_pool[index1][index2];
				client_proxy->done_send();
			}
		}
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

	std::vector<std::vector<clientproxy> > client_proxy_pool;
	std::vector<std::priority_queue<int, std::vector<int>, std::greater<int> > > client_proxy_recycle_pool;

	std::vector<std::set<int> > wait_send_cli;

	std::shared_ptr<hubsvrmanager> _hubsvrmanager;

};

}

#endif //_clientmanager_h
