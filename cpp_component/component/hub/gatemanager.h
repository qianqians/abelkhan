/*
 * gatemanager.h
 *
 *  Created on: 2016-7-12
 *      Author: qianqians
 */
#ifndef _gatemanager_h
#define _gatemanager_h

#include <map>
#include <set>
#include <memory>
#include <shared_mutex>

#include <absl/container/node_hash_map.h>

#include <Ichannel.h>

#include <hub_call_gatecaller.h>
#include <hub_call_clientcaller.h>

namespace service {

class webchannel;
class enetconnectservice;

}

namespace hub {

extern std::string current_client_uuid;

class hub_service;
class gateproxy {
public:
	std::shared_ptr<caller::hub_call_gate> caller;
	std::shared_ptr<hub_service> _hub;

	gateproxy(std::shared_ptr<juggle::Ichannel> ch, std::shared_ptr<hub_service> hub);

	void reg_hub();

	void connect_sucess(std::string client_uuid);

	void disconnect_client(std::string uuid);

	void forward_hub_call_client(std::string uuid, std::string _module, std::string func, Fossilizid::JsonParse::JsonArray argv);

	void forward_hub_call_group_client(Fossilizid::JsonParse::JsonArray uuids, std::string _module, std::string func, Fossilizid::JsonParse::JsonArray argv);

	void forward_hub_call_global_client(std::string _module, std::string func, Fossilizid::JsonParse::JsonArray argv);

};

class directproxy {
private:
	std::shared_ptr<caller::hub_call_client> caller;

public:
	std::shared_ptr<service::webchannel> direct_ch;

public:
	directproxy(std::shared_ptr<service::webchannel> direct_ch);

	void call_client(const std::string& _module, const std::string& func, const Fossilizid::JsonParse::JsonArray& argv);

	void dispose();

};

class gatemanager {
public:
	gatemanager(std::shared_ptr<service::enetconnectservice> _conn, std::shared_ptr<hub_service> _hub_);

	void connect_gate(std::string uuid, std::string ip, uint16_t port);

	void client_connect(std::string client_uuid, std::shared_ptr<juggle::Ichannel> gate_ch);

	void client_direct_connect(std::string client_uuid, std::shared_ptr<service::webchannel> direct_ch);

	void client_direct_disconnect(std::shared_ptr<juggle::Ichannel> direct_ch);

	void client_direct_exception(std::shared_ptr<juggle::Ichannel> direct_ch);

	void client_disconnect(std::string client_uuid);

	void client_exception(std::string client_uuid);

	void disconnect_client(std::string uuid);

	void call_client(const std::string& uuid, const std::string& _module, const std::string& func, const Fossilizid::JsonParse::JsonArray& argvs);

	void call_group_client(const std::vector<std::string>& uuids, const std::string& _module, const std::string& func, const Fossilizid::JsonParse::JsonArray& argvs);

	void call_global_client(std::string _module, std::string func, Fossilizid::JsonParse::JsonArray argvs);

private:
	std::shared_ptr<service::enetconnectservice> conn;
	std::shared_ptr<hub_service> _hub;
	
	std::shared_mutex _mu;

	absl::node_hash_map<std::string, std::shared_ptr<gateproxy> > clients;
	absl::node_hash_map<std::string, std::shared_ptr<gateproxy> > gates;
	absl::node_hash_map<std::shared_ptr<juggle::Ichannel>, std::shared_ptr<gateproxy> > ch_gates;

	absl::node_hash_map<std::string, std::shared_ptr<directproxy> > direct_clients;
	absl::node_hash_map<std::shared_ptr<juggle::Ichannel>, std::string> ch_uuid_direct_clients;

};

}

#endif //_gatemanager_h
