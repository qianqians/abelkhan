/*
 * centerproxy.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _centerproxy_h
#define _centerproxy_h

#include <spdlog/spdlog.h>

#include <abelkhan.h>

#include <modulemng_handle.h>

#include <center.h>

namespace hub{

struct name_info {
	std::string name;
	uint32_t serial;
};

class centerproxy {
public:
	centerproxy(std::shared_ptr<abelkhan::Ichannel> ch) {
		is_reg_sucess = false;

		_center_ch = ch;
		_center_caller = std::make_shared<abelkhan::center_caller>(_center_ch, service::_modulemng);
	}

	~centerproxy(){
	}

	void reg_server(std::string host, short port, std::string sub_type, struct name_info &_info) {
		spdlog::trace("begin connect center server!");
		_center_caller->reg_server("hub", sub_type, host, port)->callBack([this, &_info](uint32_t serial, std::string name) {
			_info.name = name;
			_info.serial = serial;
			spdlog::trace("connect center sucessed!");
			is_reg_sucess = true;
		}, []() {
			spdlog::trace("connect center failed!");
		})->timeout(5 * 1000, []() {
			spdlog::trace("connect center timeout!");
		});
	}
	
	void heartbeat() {
		spdlog::trace("heartbeat center!");
		_center_caller->heartbeat();
	}

	void closed() {
		_center_caller->closed();
	}

public:
	bool is_reg_sucess;

private:
	std::shared_ptr<abelkhan::Ichannel> _center_ch;
	std::shared_ptr<abelkhan::center_caller> _center_caller;

};

}

#endif // !_centerproxy_h
