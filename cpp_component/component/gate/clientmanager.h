/*
 * clientmanager.h
 *
 *  Created on: 2016-7-12
 *      Author: qianqians
 */
#ifndef _clientmanager_h
#define _clientmanager_h

#include <map>
#include <set>
#include <boost/shared_ptr.hpp>

#include <Ichannel.h>

#include <gate_call_hubcaller.h>

#include "hubsvrmanager.h"
#include "gc_poll.h"

namespace gate {

class clientmanager {
public:
	clientmanager(std::shared_ptr<hubsvrmanager> _hubsvrmanager_) {
		_hubsvrmanager = _hubsvrmanager_;
	}

	~clientmanager() {
	}

	void enable_heartbeats(std::shared_ptr<juggle::Ichannel> _client) {
		heartbeats_client.insert(_client);
	}

	void disable_heartbeats(std::shared_ptr<juggle::Ichannel> _client) {
		heartbeats_client.erase(_client);
	}

	void heartbeat_client(int64_t ticktime) {
		std::vector<std::shared_ptr<juggle::Ichannel> > remove_client;
		for (auto item : client_server_time) {
			if ((item.second + 60 * 60 * 1000) < ticktime) {
				remove_client.push_back(item.first);
				continue;
			}

			if ((item.second + 20 * 1000) < ticktime) {
				if (heartbeats_client.find(item.first) != heartbeats_client.end()) {
					remove_client.push_back(item.first);
				}
			}
		}

		for (auto _client : remove_client){
			auto client_uuid = client_uuid_map[_client];
			_hubsvrmanager->for_each_hub([client_uuid](std::string hub_name, std::shared_ptr<juggle::Ichannel> hub_ch) {
				auto _caller = std::make_shared<caller::gate_call_hub>(hub_ch);
				_caller->client_disconnect(client_uuid);
			});
		}

		gate::gc_put([this, remove_client]() {
			for (auto _client : remove_client) {
				_client->disconnect();
			}
		});
	}

	void refresh_and_check_client(std::shared_ptr<juggle::Ichannel> _client, int64_t servertick, int64_t clienttick) {
		if (((clienttick - client_time[_client]) - (servertick - client_server_time[_client])) > 10 * 1000) {
			std::cout << "clienttick_new:" << clienttick << " clienttick_old:" << client_time[_client] << std::endl;
			std::cout << "servertick_new:" << servertick << " servertick_old:" << client_server_time[_client] << std::endl;

			auto client_uuid = client_uuid_map[_client];
			_hubsvrmanager->for_each_hub([client_uuid](std::string hub_name, std::shared_ptr<juggle::Ichannel> hub_ch) {
				auto _caller = std::make_shared<caller::gate_call_hub>(hub_ch);
				_caller->client_exception(client_uuid);
			});
		}

		client_server_time[_client] = servertick;
		client_time[_client] = clienttick;
	}

	void reg_client(std::string uuid, std::shared_ptr<juggle::Ichannel> _client, int64_t servertick, int64_t clienttick) {
		client_map.insert(std::make_pair(uuid, _client));
		client_uuid_map.insert(std::make_pair(_client, uuid));
		client_server_time.insert(std::make_pair(_client, servertick));
		client_time.insert(std::make_pair(_client, clienttick));
	}

	void unreg_client(std::shared_ptr<juggle::Ichannel> _client){
		if (client_uuid_map.find(_client) == client_uuid_map.end()){
			return;
		}

		auto _client_uuid = client_uuid_map[_client];
		std::cout << "unreg_client:" << _client_uuid << std::endl;

		if (client_map.find(_client_uuid) != client_map.end())
		{
			client_map.erase(_client_uuid);
		}
		if (client_server_time.find(_client) != client_server_time.end())
		{
			client_server_time.erase(_client);
		}
		if (client_time.find(_client) != client_time.end())
		{
			client_time.erase(_client);
		}
		if (client_uuid_map.find(_client) != client_uuid_map.end())
		{
			client_uuid_map.erase(_client);
		}
		if (heartbeats_client.find(_client) != heartbeats_client.end()) {
			heartbeats_client.erase(_client);
		}
	}

	bool has_client(std::shared_ptr<juggle::Ichannel> _client) {
		return client_uuid_map.find(_client) != client_uuid_map.end();
	}

	bool has_client(std::string uuid) {
		return client_map.find(uuid) != client_map.end();
	} 

	std::string get_client(std::shared_ptr<juggle::Ichannel> _client) {
		return client_uuid_map[_client];
	}

	std::shared_ptr<juggle::Ichannel> get_client(std::string uuid) {
		return client_map[uuid];
	}

	void for_each_client(std::function<void(std::string, std::shared_ptr<juggle::Ichannel>)> fn) {
		for (auto client : client_map){
			fn(client.first, client.second);
		}
	}

private:
	std::set<std::shared_ptr<juggle::Ichannel> > heartbeats_client;

	std::map<std::string, std::shared_ptr<juggle::Ichannel> > client_map;
	std::map<std::shared_ptr<juggle::Ichannel>, std::string> client_uuid_map;

	std::map<std::shared_ptr<juggle::Ichannel>, int64_t > client_server_time;
	std::map<std::shared_ptr<juggle::Ichannel>, int64_t > client_time;

	std::shared_ptr<hubsvrmanager> _hubsvrmanager;

};

}

#endif //_clientmanager_h
