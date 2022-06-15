/*
 * clientmanager.h
 *
 *  Created on: 2016-7-12
 *      Author: qianqians
 */
#ifndef _clientmanager_h
#define _clientmanager_h

#include <vector>
#include <unordered_map>
#include <unordered_set>

#include <boost/uuid/uuid.hpp>
#include <boost/uuid/uuid_generators.hpp>
#include <boost/uuid/uuid_io.hpp>
#include <boost/lexical_cast.hpp>

#include <spdlog/spdlog.h>

#include <abelkhan.h>
#include <client.h>

#include "objpool.h"
#include "timerservice.h"
#include "hubsvrmanager.h"
#include "gc_poll.h"

namespace gate {

class clientproxy {
private:
	std::shared_ptr<abelkhan::gate_call_client_caller> _gate_call_client_caller;

public:
	int64_t _timetmp = 0;
	int64_t _theory_timetmp = 0;
	std::string _cuuid;
	std::shared_ptr<abelkhan::Ichannel> _ch;

	std::mutex _conn_hubproxys_mutex;
	std::vector<std::shared_ptr<hubproxy> > conn_hubproxys;

public:
	clientproxy(std::string& cuuid, std::shared_ptr<abelkhan::Ichannel> ch) {
		_cuuid = cuuid;
		_ch = ch;
		_gate_call_client_caller = std::make_shared<abelkhan::gate_call_client_caller>(ch, service::_modulemng);
	}

	virtual ~clientproxy() {
	}

	void conn_hub(std::shared_ptr<hubproxy> hub_proxy) {
		std::lock_guard<std::mutex> l(_conn_hubproxys_mutex);
		if (std::find(conn_hubproxys.begin(), conn_hubproxys.end(), hub_proxy) == conn_hubproxys.end()) {
			conn_hubproxys.push_back(hub_proxy);
		}
	}

	void ntf_cuuid() {
		_gate_call_client_caller->ntf_cuuid(_cuuid);
	}

	void call_client(std::string hub_name, std::vector<uint8_t>& data) {
		_gate_call_client_caller->call_client(hub_name, data);
	}

};

class clientmanager {
public:
	clientmanager(std::shared_ptr<hubsvrmanager> _hubsvrmanager_) {
		_hubsvrmanager = _hubsvrmanager_;
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

	std::shared_ptr<clientproxy> reg_client(std::shared_ptr<abelkhan::Ichannel> ch) {
		std::string cuuid = boost::lexical_cast<std::string>(boost::uuids::random_generator()());
		auto _client = _client_pool.make_obj(cuuid, ch);

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

		_client_pool.recycle(_client);
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

	std::shared_ptr<hubsvrmanager> _hubsvrmanager;

	service::objpool<clientproxy> _client_pool;

};

}

#endif //_clientmanager_h
