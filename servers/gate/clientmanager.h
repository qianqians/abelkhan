/*
 * clientmanager.h
 *
 *  Created on: 2016-7-12
 *      Author: qianqians
 */
#ifndef _clientmanager_h
#define _clientmanager_h

#include <map>

#include <boost/shared_ptr.hpp>

#include <Ichannel.h>

#include <caller/gate_call_logiccaller.h>

namespace gate {

class clientmanager {
public:
	clientmanager() {
	}

	~clientmanager() {
	}

	void heartbeat_client(int64_t ticktime) {
		std::vector<std::shared_ptr<juggle::Ichannel> > heartbeat_client;
		for (auto item : client_server_time) {
			if ((item.second + 30 * 1000) < ticktime) {
				heartbeat_client.push_back(item.first);
			}
		}

		for (auto _client : heartbeat_client){
			auto client_uuid = client_uuid_map[_client];
			auto _caller = std::make_shared<caller::gate_call_logic>(client_logic_map[client_uuid]);
			_caller->client_disconnect(client_uuid);
		}

		for (auto _client : heartbeat_client) {
			client_server_time.erase(_client);
			auto client_uuid = client_uuid_map[_client];
			client_uuid_map.erase(_client);
			client_map.erase(client_uuid);
			client_logic_map.erase(client_uuid);
		}
	}

	void refresh_and_check_client(std::shared_ptr<juggle::Ichannel> _client, int64_t servertick, int64_t clienttick) {
		if (((clienttick - client_time[_client]) - (servertick - client_server_time[_client])) > 10 * 1000) {
			auto client_uuid = client_uuid_map[_client];
			auto _caller = std::make_shared<caller::gate_call_logic>(client_logic_map[client_uuid]);
			_caller->client_exception(client_uuid);
		}

		client_server_time[_client] = servertick;
		client_time[_client] = clienttick;
	}

	void reg_client(std::string uuid, std::shared_ptr<juggle::Ichannel> _client, int64_t servertick, int64_t clienttick) {
		client_map.insert(std::make_pair(uuid, _client));
		client_uuid_map.insert(std::make_pair(_client, uuid));
		client_server_time.insert(std::make_pair(_client, servertick));
	}

	void reg_client_logic(std::string uuid, std::shared_ptr<juggle::Ichannel> _logic) {
		client_logic_map.insert(std::make_pair(uuid, _logic));
	}

	bool has_client(std::shared_ptr<juggle::Ichannel> _client) {
		return client_uuid_map.find(_client) != client_uuid_map.end();
	}

	bool has_client(std::string uuid) {
		return client_map.find(uuid) != client_map.end();
	} 

	std::string get_client_uuid(std::shared_ptr<juggle::Ichannel> _client) {
		return client_uuid_map[_client];
	}

	std::shared_ptr<juggle::Ichannel> get_client_channel(std::string uuid) {
		return client_map[uuid];
	}
	
	bool has_client_logic(std::shared_ptr<juggle::Ichannel> _client) {
		auto find = client_uuid_map.find(_client);
		if (find == client_uuid_map.end()) {
			return false;
		}

		return client_logic_map.find(find->second) != client_logic_map.end();
	}

	std::shared_ptr<juggle::Ichannel> get_logic_channel(std::string uuid) {
		return client_logic_map[uuid];
	}

	void for_each_client(std::function<void(std::string, std::shared_ptr<juggle::Ichannel>, std::shared_ptr<juggle::Ichannel>)> fn) {
		for (auto client : client_logic_map){
			fn(client.first, client_map[client.first], client.second);
		}
	}

private:
	std::map<std::string, std::shared_ptr<juggle::Ichannel> > client_map;

	std::map<std::string, std::shared_ptr<juggle::Ichannel> > client_logic_map;

	std::map<std::shared_ptr<juggle::Ichannel>, std::string> client_uuid_map;

	std::map<std::shared_ptr<juggle::Ichannel>, int64_t > client_server_time;

	std::map<std::shared_ptr<juggle::Ichannel>, int64_t > client_time;

};

}

#endif //_clientmanager_h
