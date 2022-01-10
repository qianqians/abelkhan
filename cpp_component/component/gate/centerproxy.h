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
#include <center.h>

#include <modulemng_handle.h>

namespace gate{

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

	virtual ~centerproxy(){
	}

	void reg_server(std::string host, short port, struct name_info &_name_info) {
		_center_caller->reg_server("gate", "gate", host, port)->callBack([this, &_name_info](uint32_t serial, std::string name){
			_name_info.name = name;
			_name_info.serial = serial;
			is_reg_sucess = true;
			spdlog::trace("connect center server sucess!");
		}, [](){
			spdlog::trace("connect center server faild!");
		})->timeout(1000, []() {
			spdlog::trace("connect center server timeout!");
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
