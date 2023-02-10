/*
 * hub.h
 *
 *  Created on: 2020-1-10
 *      Author: qianqians
 */
#ifndef _hub_h
#define _hub_h

#include <memory>
#include <mutex>
#include <random>

#include <abelkhan.h>
#include <timerservice.h>

#include <log.h>
#include <config.h>
#include <module.h>
#include <modulemanager.h>

#include "closehandle.h"
#include "centerproxy.h"
#include "hubsvrmanager.h"
#include "gatemanager.h"
#include "dbproxyproxy.h"

namespace asio {

class io_context;
typedef io_context io_service;

} // namespace asio

namespace service {

class enetacceptservice;
class redismqservice;
class connectservice;
class acceptservice;
class webacceptservice;

}

namespace hub{

class centerproxy;

class center_msg_handle;
class dbproxy_msg_handle;
class direct_client_msg_handle;
class gate_msg_handle;
class hub_svr_msg_handle;

class hub_service : public std::enable_shared_from_this<hub_service> {
public:
	hub_service(std::string config_file_path, std::string config_name, std::string hub_type_);

	void init();

	void connect_center();

	void connect_gate(std::string gate_name);

	void reg_hub(std::string hub_name);

	void try_connect_db(std::string dbproxy_name);

	uint32_t random(uint32_t max);

	std::shared_ptr<dbproxyproxy> get_random_dbproxy();

	std::shared_ptr<dbproxyproxy> get_dbproxy(std::string db_name);

	void run();

	void close_svr();

	void set_support_take_over_svr(bool is_support);

private:
	void reconnect_center();

	void svr_closed(std::string svr_type, std::string svr_name);

	uint32_t poll();

	static void heartbeat(std::shared_ptr<hub_service> this_ptr, int64_t tick);

public:
	std::string hub_type;
	hub::name_info name_info;

	class addressinfo {
	public:
		std::string host;
		uint16_t port;
	};
	std::shared_ptr<addressinfo> tcp_address_info = nullptr;
	std::shared_ptr<addressinfo> websocket_address_info = nullptr;
	std::shared_ptr<addressinfo> enet_address_info = nullptr;

	uint32_t tick;
	bool is_busy;

	concurrent::signals<void(std::string) > sig_reload;
	concurrent::signals<void() > sig_close_server;

	concurrent::signals<void(std::string) > sig_client_disconnect;
	concurrent::signals<void(std::string) > sig_client_exception;
	concurrent::signals<void(std::string) > sig_client_msg;

	concurrent::signals<void(std::string) > sig_direct_client_disconnect;
	
	concurrent::signals<void(std::shared_ptr<hubproxy>) > sig_hub_reconnect;
	concurrent::signals<void(std::shared_ptr<hubproxy>) > sig_hub_connect;

	concurrent::signals<void(std::string)> sig_gate_closed;
	
	concurrent::signals<void() > sig_dbproxy_init;

	concurrent::signals<void() > sig_center_crash;

	common::modulemanager modules;

	std::shared_ptr<gatemanager> _gatemng;
	std::shared_ptr<hubsvrmanager> _hubmng;

	std::shared_ptr<closehandle> _close_handle;
	std::shared_ptr<service::timerservice> _timerservice;

	std::shared_ptr<config::config> _center_config;
	std::shared_ptr<config::config> _root_config;
	std::shared_ptr<config::config> _config;

private:
	friend class center_msg_handle;
	friend class gatemanager;
	friend class hubsvrmanager;

	concurrent::signals<void(std::string, std::string) > sig_svr_be_closed;

	bool is_support_take_over_svr = true;
	std::shared_ptr<service::redismqservice> _hub_redismq_service;

	std::shared_ptr<asio::io_service> _io_service;
	std::shared_ptr<service::connectservice> _center_service;
	std::shared_ptr<service::connectservice> _dbproxy_service;
	std::shared_ptr<service::acceptservice> _client_tcp_service;

	std::shared_ptr<service::webacceptservice> _client_websocket_service;
	std::shared_ptr<service::enetacceptservice> _enet_service;

	std::shared_ptr<center_msg_handle> _center_msg_handle;
	std::shared_ptr<dbproxy_msg_handle> _dbproxy_msg_handle;
	std::shared_ptr<direct_client_msg_handle> _direct_client_msg_handle;
	std::shared_ptr<gate_msg_handle> _gate_msg_handle;
	std::shared_ptr<hub_svr_msg_handle> _hub_svr_msg_handle;

	std::shared_ptr<centerproxy> _centerproxy;
	uint32_t reconn_count;

	std::map<std::string, std::shared_ptr<dbproxyproxy> > _dbproxyproxys;

	std::mt19937_64 e;

	std::mutex _run_mu;

};

}

#endif //_hub_h
