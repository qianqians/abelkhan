/*
 * hub.h
 *
 *  Created on: 2020-1-10
 *      Author: qianqians
 */
#ifndef _hub_h
#define _hub_h

#include <memory>

#include <abelkhan.h>
#include <timerservice.h>

#include <log.h>
#include <config.h>
#include <module.h>
#include <modulemanager.h>

#include "closehandle.h"
#include "hubsvrmanager.h"
#include "gatemanager.h"
#include "dbproxyproxy.h"

namespace boost {
namespace asio {

class io_context;
typedef io_context io_service;

} // namespace asio
} // namespace boost

namespace service {

class enetacceptservice;
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

	void connect_gate(std::string uuid, std::string ip, uint16_t port);

	void reg_hub(std::string hub_ip, uint16_t hub_port);

	void try_connect_db(std::string dbproxy_name, std::string dbproxy_ip, uint16_t dbproxy_port);

	void close_svr();

	int poll();

private:
	void heartbeat(int64_t tick);

public:
	std::string hub_name;
	std::string hub_type;
	bool is_busy;

	concurrent::signals<void(std::string) > sig_reload;
	concurrent::signals<void() > sig_close_server;

	concurrent::signals<void(std::string) > sig_client_disconnect;
	concurrent::signals<void(std::string) > sig_client_exception;

	concurrent::signals<void(std::string) > sig_direct_client_disconnect;

	concurrent::signals<void(std::shared_ptr<hubproxy>) > sig_hub_connect;

	concurrent::signals<void(std::string, std::string)> sig_hub_closed;
	concurrent::signals<void(std::string)> sig_gate_closed;
	
	concurrent::signals<void() > sig_dbproxy_init;
	concurrent::signals<void() > sig_extend_dbproxy_init;

	common::modulemanager modules;

	std::shared_ptr<dbproxyproxy> _dbproxyproxy;
	std::shared_ptr<dbproxyproxy> _extend_dbproxyproxy;

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

	std::shared_ptr<service::enetacceptservice> _hub_service;

	std::shared_ptr<boost::asio::io_service> _io_service;
	std::shared_ptr<service::connectservice> _center_service;
	std::shared_ptr<service::connectservice> _dbproxy_service;
	std::shared_ptr<service::acceptservice> _client_tcp_service;

	std::shared_ptr<service::webacceptservice> _client_websocket_service;

	std::shared_ptr<center_msg_handle> _center_msg_handle;
	std::shared_ptr<dbproxy_msg_handle> _dbproxy_msg_handle;
	std::shared_ptr<direct_client_msg_handle> _direct_client_msg_handle;
	std::shared_ptr<gate_msg_handle> _gate_msg_handle;
	std::shared_ptr<hub_svr_msg_handle> _hub_svr_msg_handle;

	std::shared_ptr<centerproxy> _centerproxy;

};

}

#endif //_hub_h
