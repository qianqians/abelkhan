/*
 * centerproxy.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _centerproxy_h
#define _centerproxy_h

#include <iostream>

#include <Ichannel.h>
#include <factory.h>

#include <centercaller.h>
#include <hub_call_centercaller.h>

#include <spdlog/spdlog.h>

namespace hub{

class centerproxy {
public:
	centerproxy(std::shared_ptr<juggle::Ichannel> ch) {
		is_reg_sucess = false;
		_center_ch = ch;
		_center_caller = Fossilizid::pool::factory::create<caller::center>(_center_ch);
		_hub_call_center_caller = Fossilizid::pool::factory::create<caller::hub_call_center>(_center_ch);
	}

	~centerproxy(){
	}

	void reg_server(std::string ip, short port, std::string uuid) {
		spdlog::trace("begin connect center server");
		_center_caller->reg_server("hub", ip, port, uuid);
	}

	void closed() {
		_hub_call_center_caller->closed();
	}

public:
	bool is_reg_sucess;

private:
	std::shared_ptr<juggle::Ichannel> _center_ch;
	std::shared_ptr<caller::center> _center_caller;
	std::shared_ptr<caller::hub_call_center> _hub_call_center_caller;

};

}

#endif // !_centerproxy_h
