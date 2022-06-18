/*
 * gatemanager.h
 *
 *  Created on: 2016-7-12
 *      Author: qianqians
 */
#ifndef _gatemanager_h
#define _gatemanager_h

#include <memory>
#include <unordered_map>

#include <msgpack11.hpp>

#include <abelkhan.h>

#include <hub.h>
#include <gate.h>

#include "objpool.h"

namespace service {

class enetacceptservice;
class redismqservice;

}

namespace hub {

class hub_service;
class gateproxy {
public:
	std::string _gate_name;

	std::shared_ptr<abelkhan::Ichannel> _ch;
	std::shared_ptr<abelkhan::hub_call_gate_caller> _hub_call_gate_caller;

	std::shared_ptr<hub_service> _hub;

	concurrent::signals<void()> sig_reg_hub_sucessed;

public:
	gateproxy(std::shared_ptr<abelkhan::Ichannel> ch, std::shared_ptr<hub_service> hub, std::string gate_name);

	void reg_hub();

	std::shared_ptr<abelkhan::hub_call_gate_reverse_reg_client_hub_cb> reverse_reg_client_hub(std::string client_uuid);

	void disconnect_client(std::string& cuuid);

	void forward_hub_call_client(const std::string& cuuid, const std::vector<uint8_t>& rpc_argv);

	void forward_hub_call_group_client(const std::vector<std::string>& cuuids, const std::vector<uint8_t>& rpc_argv);

	void forward_hub_call_global_client(const std::vector<uint8_t>& rpc_argv);

};

class directproxy {
private:
	std::shared_ptr<abelkhan::hub_call_client_caller> _hub_call_client_caller;

public:
	int64_t _timetmp = 0;
	int64_t _theory_timetmp = 0;

	std::string _cuuid;
	std::shared_ptr<abelkhan::Ichannel> _direct_ch;

public:
	directproxy(std::string cuuid_, std::shared_ptr<abelkhan::Ichannel> direct_ch);

	void call_client(const std::vector<uint8_t>& rpc_argv);

};

class gatemanager {
public:
	gatemanager(std::shared_ptr<service::enetacceptservice> conn_, std::shared_ptr<hub_service> hub_);

	gatemanager(std::shared_ptr<service::redismqservice> conn_, std::shared_ptr<hub_service> hub_);

	void connect_gate(std::string gate_name, std::string ip, uint16_t port);

	void connect_gate(std::string gate_name);

	void client_connect(std::string client_uuid, std::shared_ptr<abelkhan::Ichannel> gate_ch);

	std::shared_ptr<gateproxy> client_seep(std::string client_uuid, std::string gate_name);

	void client_disconnect(std::string client_uuid);

	void client_direct_connect(std::string client_uuid, std::shared_ptr<abelkhan::Ichannel> direct_ch);

	void client_direct_disconnect(std::shared_ptr<abelkhan::Ichannel> direct_ch);

	std::shared_ptr<directproxy> get_direct_client(std::shared_ptr<abelkhan::Ichannel> direct_ch);

	void disconnect_client(std::string uuid);

	void heartbeat_client(int64_t ticktime);

	void call_client(const std::string& uuid, const std::string& func, const msgpack11::MsgPack::array& argvs);

	void call_group_client(const std::vector<std::string>& uuids, const std::string& func, const msgpack11::MsgPack::array& argvs);

	void call_global_client(const std::string& func, const msgpack11::MsgPack::array& argvs);

private:
	void Init();

public:
	thread_local static std::string current_client_cuuid;

private:
	std::shared_ptr<service::enetacceptservice> _conn_enet;
	std::shared_ptr<service::redismqservice> _conn_redismq;
	std::shared_ptr<hub_service> _hub;
	
	std::unordered_map<std::string, std::shared_ptr<gateproxy> > clients;

	std::unordered_map<std::string, std::shared_ptr<gateproxy> > wait_destory_gates;
	std::unordered_map<std::string, std::shared_ptr<gateproxy> > gates;
	std::unordered_map<std::shared_ptr<abelkhan::Ichannel>, std::shared_ptr<gateproxy> > ch_gates;

	std::unordered_map<std::string, std::shared_ptr<directproxy> > direct_clients;
	std::unordered_map<std::shared_ptr<abelkhan::Ichannel>, std::shared_ptr<directproxy> > ch_direct_clients;

	service::objpool<directproxy> _directproxy_pool;

};

}

#endif //_gatemanager_h
