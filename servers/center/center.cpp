/*
 * qianqians
 * 2016-7-9
 * center.cpp
 */
#include <boost/uuid/uuid.hpp>
#include <boost/uuid/uuid_generators.hpp>
#include <boost/uuid/uuid_io.hpp>
#include <boost/lexical_cast.hpp>
#include <boost/thread.hpp>

#include <acceptnetworkservice.h>
#include <process_.h>
#include <Ichannel.h>
#include <Imodule.h>
#include <channel.h>
#include <juggleservice.h>
#include <timerservice.h>

#include <config.h>

#include <module/centermodule.h>
#include <module/gm_centermodule.h>
#include <module/logic_call_centermodule.h>

#include "svrmanager.h"
#include "logicsvrmanager.h"
#include "logic_svr_msg_handle.h"
#include "svr_msg_handle.h"
#include "gm_msg_handle.h"

void main(int argc, char * argv[]) {
	auto svr_uuid = boost::lexical_cast<std::string>(boost::uuids::random_generator()());

	if (argc <= 1) {
		std::cout << "non input start argv" << std::endl;
		return;
	}

	std::string config_file_path = argv[1];
	auto _config = std::make_shared<config::config>(config_file_path);
	if (argc >= 3) {
		_config = _config->get_value_dict(argv[2]);
	}

	std::shared_ptr<juggle::process> _svr_process = std::make_shared<juggle::process>();
	
	std::shared_ptr<center::svrmanager> _svrmanager = std::make_shared<center::svrmanager>();
	std::shared_ptr<center::logicsvrmanager> _logicsvrmanager = std::make_shared<center::logicsvrmanager>();
	std::shared_ptr<module::center> _center = std::make_shared<module::center>();
	_center->sigreg_serverhandle.connect(std::bind(&reg_server, _svrmanager, _logicsvrmanager, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3, std::placeholders::_4));
	_svr_process->reg_module(_center);
	
	std::shared_ptr<module::logic_call_center> _logic_call_center = std::make_shared<module::logic_call_center>();
	_logic_call_center->sigreq_get_server_addresshandle.connect(std::bind(&req_get_server_address, _logicsvrmanager, _svrmanager, std::placeholders::_1, std::placeholders::_2));
	_svr_process->reg_module(_logic_call_center);

	auto host_ip = _config->get_value_string("ip");
	auto host_port = (short)_config->get_value_int("port");
	auto _host_service = std::make_shared<service::acceptnetworkservice>(host_ip, host_port, _svr_process);

	std::shared_ptr<center::gmmanager> _gmmanager = std::make_shared<center::gmmanager>();
	std::shared_ptr<juggle::process> _gm_process = std::make_shared<juggle::process>();
	std::shared_ptr<module::gm_center> _gm_center = std::make_shared<module::gm_center>();
	_gm_center->sigconfirm_gmhandle.connect(std::bind(&confirm_gm, _gmmanager, std::placeholders::_1));
	_gm_center->sigclose_clutterhandle.connect(std::bind(&close_clutter, _gmmanager, _svrmanager, std::placeholders::_1));
	_gm_process->reg_module(_gm_center);

	auto gm_ip = _config->get_value_string("gm_ip");
	auto gm_port = (short)_config->get_value_int("gm_port");
	auto _gm_service = std::make_shared<service::acceptnetworkservice>(gm_ip, gm_port, _gm_process);

	std::shared_ptr<service::juggleservice> _juggleservice = std::make_shared<service::juggleservice>();
	_juggleservice->add_process(_svr_process);
	_juggleservice->add_process(_gm_process);

	int64_t tick = clock();
	int64_t tickcount = 0;

	std::shared_ptr<service::timerservice> _timerservice = std::make_shared<service::timerservice>(tick);

	while (true)
	{
		int64_t tmptick = (clock() & 0xffffffff);
		if (tmptick < tick)
		{
			tickcount += 1;
			tmptick = tmptick + tickcount * 0xffffffff;
		}
		tick = tmptick;

		try {
			_host_service->poll(tick);
			_gm_service->poll(tick);
			_juggleservice->poll(tick);
			_timerservice->poll(tick);
		}
		catch (std::exception e) {
			std::cout << "system exception:" << e.what() << std::endl;
			break;
		}

		tmptick = (clock() & 0xffffffff);
		if (tmptick < tick)
		{
			tickcount += 1;
			tmptick = tmptick + tickcount * 0xffffffff;
		}
		int64_t ticktime = (tmptick - tick);
		tick = tmptick;

		if (ticktime < 50) {
			boost::thread::sleep(boost::get_system_time() + boost::posix_time::microseconds(15));
		}
	}
}
