/*
 * hub.h
 *
 *  Created on: 2020-1-10
 *      Author: qianqians
 */
#ifndef _hub_h
#define _hub_h

#include <boost/signals2.hpp>

#include <process_.h>
#include <Ichannel.h>
#include <Imodule.h>
#include <channel.h>
#include <juggleservice.h>
#include <timerservice.h>
#include <memory>

#include <log.h>
#include <config.h>
#include <module.h>
#include <modulemanager.h>

#include <hub_call_gatemodule.h>
#include <client_call_gatemodule.h>
#include <center_call_servermodule.h>

#include "closehandle.h"
#include "hubsvrmanager.h"
#include "gatemanager.h"

namespace service {

class enetacceptservice;
class enetconnectservice;
class connectservice;
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

	void try_connect_db(std::string dbproxy_ip, uint16_t dbproxy_port);

	void poll();

public:
	std::string uuid;
	std::string name;
	bool is_busy;

	boost::signals2::signal<void(std::string) > sig_client_connect;
	boost::signals2::signal<void(std::string) > sig_direct_client_connect;
	boost::signals2::signal<void(std::string) > sig_client_disconnect;
	boost::signals2::signal<void(std::string) > sig_client_exception;
	boost::signals2::signal<void(std::string) > sig_hub_connect;

	common::modulemanager modules;
	std::shared_ptr<closehandle> close_handle;
	std::shared_ptr<gatemanager> gates;
	std::shared_ptr<hubsvrmanager> hubs;
	std::shared_ptr<service::timerservice> _timerservice;

	std::shared_ptr<config::config> _center_config;
	std::shared_ptr<config::config> _root_config;
	std::shared_ptr<config::config> _config;

private:
	std::shared_ptr<service::enetacceptservice> _hub_service;
	std::shared_ptr<service::enetconnectservice> _gate_service;
	std::shared_ptr<service::connectservice> _center_service;
	std::shared_ptr<service::connectservice> _dbproxy_service;
	std::shared_ptr<service::webacceptservice> _client_service;

	std::shared_ptr<service::juggleservice> _juggleservice;

	std::shared_ptr<juggle::process> _center_process;

	std::shared_ptr<centerproxy> _centerproxy;
	std::shared_ptr<dbproxyproxy> _dbproxyproxy;

};

}

#endif //_hub_h
