/*
 * qianqians
 * 2022-5-8
 * gate.h
 */
#ifndef _GATE_H_
#define _GATE_H_

#include <mutex>

#include <abelkhan.h>
#include <timerservice.h>
#include <config.h>
#include <log.h>

#include <gc_poll.h>

#include "centerproxy.h"
#include "closehandle.h"
#include "clientmanager.h"
#include "hubsvrmanager.h"
#include "timer_handle.h"

namespace asio {

class io_context;
typedef io_context io_service;

} // namespace asio

namespace service {

class enetacceptservice;
class connectservice;
class acceptservice;
class webacceptservice;
class redismqservice;

}

namespace gate {

class center_msg_handle;
class hub_svr_msg_handle;
class client_msg_handle;

class gate_service : public std::enable_shared_from_this<gate_service> {
public:
	gate_service(std::string config_file_path, std::string config_name);

	void init();

	void run();

	void close_svr();

private:
	uint32_t poll();

	void init_center(std::shared_ptr<abelkhan::Ichannel> center_ch);

private:
	static void heartbeat_center(std::shared_ptr<gate_service> _gate_service, std::function<void()> reconn_func, int64_t tick);

public:
	concurrent::signals<void() > sig_close_server;
	concurrent::signals<void() > sig_center_crash;

	std::shared_ptr<closehandle> _closehandle;

public:
	name_info gate_name_info;

private:
	uint32_t reconn_count;
	uint32_t tick;

	std::shared_ptr<config::config> _root_config;
	std::shared_ptr<config::config> _center_config;
	std::shared_ptr<config::config> _config;

	std::shared_ptr<service::timerservice> _timerservice;
	std::shared_ptr<hubsvrmanager> _hubsvrmanager;
	std::shared_ptr<clientmanager> _clientmanager;

	std::shared_ptr<center_msg_handle> _center_msg_handle;
	std::shared_ptr<hub_svr_msg_handle> _hub_svr_msg_handle;
	std::shared_ptr<client_msg_handle> _client_msg_handle;

	std::shared_ptr<service::redismqservice> _hub_redismq_service;

	std::string center_ip;
	short center_port;
	std::shared_ptr<asio::io_service> io_service;
	std::shared_ptr<service::connectservice> _connectnetworkservice;
	std::shared_ptr<centerproxy> _centerproxy;

	std::shared_ptr<service::acceptservice> _client_service;
	std::shared_ptr<service::webacceptservice> _websocket_service;
	std::shared_ptr<service::enetacceptservice> _enet_service;

	std::mutex _run_mu;

};

}

#endif // _GATE_H_