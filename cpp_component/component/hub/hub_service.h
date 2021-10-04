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

namespace service {

class enetacceptservice;
class connectservice;
class acceptservice;
class webacceptservice;

}

namespace hub{

class dbproxyproxy;
class centerproxy;
class hub_service : public std::enable_shared_from_this<hub_service> {
public:
	hub_service(std::string config_file_path, std::string config_name);

	void init();

	void connect_center();

	void connect_gate(std::string uuid, std::string ip, uint16_t port);

	void reg_hub(std::string hub_ip, uint16_t hub_port);

	void try_connect_db(std::string dbproxy_name, std::string dbproxy_ip, uint16_t dbproxy_port);

	void poll();

public:
	std::string hub_name;
	std::string hub_type;
	bool is_busy;

	concurrent::signals<void(std::string) > sig_reload;
	concurrent::signals<void(std::string) > sig_svr_be_closed;
	concurrent::signals<void() > sig_close_server;

	concurrent::signals<void(std::string) > sig_client_disconnect;
	concurrent::signals<void(std::string) > sig_client_exception;

	concurrent::signals<void(std::shared_ptr<hubproxy>) > sig_hub_connect;
	
	concurrent::signals<void() > sig_dbproxy_init;
	concurrent::signals<void() > sig_extend_dbproxy_init;

	common::modulemanager modules;
	std::shared_ptr<dbproxyproxy> _dbproxyproxy;
	std::shared_ptr<dbproxyproxy> _extend_dbproxyproxy;

	std::shared_ptr<closehandle> close_handle;
	std::shared_ptr<gatemanager> gates;
	std::shared_ptr<hubsvrmanager> hubs;
	std::shared_ptr<service::timerservice> _timerservice;

	std::shared_ptr<config::config> _center_config;
	std::shared_ptr<config::config> _root_config;
	std::shared_ptr<config::config> _config;

private:
	std::shared_ptr<service::enetacceptservice> _hub_service;
	std::shared_ptr<service::connectservice> _center_service;
	std::shared_ptr<service::connectservice> _dbproxy_service;
	std::shared_ptr<service::webacceptservice> _client_service;

	std::shared_ptr<centerproxy> _centerproxy;

};

}

#endif //_hub_h
