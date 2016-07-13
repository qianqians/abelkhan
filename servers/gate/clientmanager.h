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

namespace gate {

class clientmanager {
public:
	clientmanager() {
	}

	~clientmanager() {
	}

	void refresh_client(boost::shared_ptr<juggle::Ichannel> _client, int64_t ticktime) {
		client_time[_client] = ticktime;
	}

	void reg_client(std::string uuid, boost::shared_ptr<juggle::Ichannel> _client, boost::shared_ptr<juggle::Ichannel> _logic, int64_t ticktime) {
		client_map.insert(std::make_pair(uuid, std::make_pair(_client, _logic)));
		client_uuid_map.insert(std::make_pair(_client, uuid));
		client_time.insert(std::make_pair(_client, ticktime));
	}

	bool has_client(boost::shared_ptr<juggle::Ichannel> _client) {
		return client_uuid_map.find(_client) != client_uuid_map.end();
	}

	bool has_client(std::string uuid) {
		return client_map.find(uuid) != client_map.end();
	} 

	std::string get_client_uuid(boost::shared_ptr<juggle::Ichannel> _client) {
		return client_uuid_map[_client];
	}

	boost::shared_ptr<juggle::Ichannel> get_client_channel(std::string uuid) {
		return client_map[uuid].first;
	}

	boost::shared_ptr<juggle::Ichannel> get_logic_channel(std::string uuid) {
		return client_map[uuid].second;
	}

private:
	// first client_channel, second logic_channel
	std::map<std::string, std::pair<boost::shared_ptr<juggle::Ichannel>, boost::shared_ptr<juggle::Ichannel> > > client_map;

	std::map<boost::shared_ptr<juggle::Ichannel>, std::string> client_uuid_map;

	std::map<boost::shared_ptr<juggle::Ichannel>, int64_t> client_time;

};

}

#endif //_clientmanager_h
