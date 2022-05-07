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
};

class centerproxy {
public:
	centerproxy(std::shared_ptr<abelkhan::Ichannel> ch, std::shared_ptr<service::timerservice> _timer) {
		is_reg_sucess = false;
		_center_ch = ch;
		_timerservice = _timer;
		_center_caller = std::make_shared<abelkhan::center_caller>(_center_ch, service::_modulemng);

		timetmp = _timerservice->Tick;
	}

	virtual ~centerproxy(){
	}

	void reg_server(std::string host, short port, struct name_info &_name_info) {
		_center_caller->reg_server("gate", _name_info.name, host, port)->callBack([this, &_name_info](){
			is_reg_sucess = true;
			spdlog::trace("connect center server sucess!");
		}, [](){
			spdlog::trace("connect center server faild!");
		})->timeout(1000, []() {
			spdlog::trace("connect center server timeout!");
		});
	}

	void reconn_reg_server(std::string host, short port, struct name_info& _name_info) {
		_center_caller->reconn_reg_server("gate", _name_info.name, host, port)->callBack([this, &_name_info]() {
			is_reg_sucess = true;
			spdlog::trace("connect center server sucess!");
		}, []() {
			spdlog::trace("connect center server faild!");
		})->timeout(1000, []() {
			spdlog::trace("connect center server timeout!");
		});
	}

	void heartbeat() {
		spdlog::trace("heartbeat center!");
		_center_caller->heartbeat(tick)->callBack([this] {
			spdlog::trace("heartbeat center server sucessed");
			timetmp = _timerservice->Tick;
		}, [] {
			spdlog::trace("heartbeat center server faild");
		})->timeout(5 * 1000, [] {
			spdlog::trace("heartbeat center server timeout");
		});
	}

	void closed() {
		_center_caller->closed();
	}

public:
	bool is_reg_sucess;
	uint32_t tick;
	time_t timetmp ;

private:
	std::shared_ptr<abelkhan::Ichannel> _center_ch;
	std::shared_ptr<service::timerservice> _timerservice;
	std::shared_ptr<abelkhan::center_caller> _center_caller;

};

extern std::shared_ptr<gate::centerproxy> _centerproxy;

}

#endif // !_centerproxy_h
