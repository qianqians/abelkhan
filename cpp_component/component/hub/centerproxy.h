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
};

class centerproxy : public std::enable_shared_from_this<centerproxy> {
public:
	centerproxy(std::shared_ptr<abelkhan::Ichannel> ch, std::shared_ptr<service::timerservice> _timer) {
		is_reg_sucess = false;
		_timerservice = _timer;
		_center_ch = ch;
		_center_caller = std::make_shared<abelkhan::center_caller>(_center_ch, service::_modulemng);

		timetmp = _timerservice->Tick;
	}

	~centerproxy(){
	}

	void reg_server(std::string host, short port, std::string sub_type, struct name_info &_info) {
		spdlog::trace("begin connect center server!");
		_center_caller->reg_server("hub", _info.name, host, port)->callBack([this, &_info]() {
			spdlog::trace("connect center sucessed!");
			is_reg_sucess = true;
		}, []() {
			spdlog::trace("connect center failed!");
		})->timeout(5 * 1000, []() {
			spdlog::trace("connect center timeout!");
		});
	}

	void reg_server(std::string sub_type, struct name_info& _info) {
		spdlog::trace("begin connect center server!");
		_center_caller->reg_server_mq("hub", _info.name)->callBack([this, &_info]() {
			spdlog::trace("connect center sucessed!");
			is_reg_sucess = true;
			}, []() {
				spdlog::trace("connect center failed!");
			})->timeout(5 * 1000, []() {
				spdlog::trace("connect center timeout!");
				});
	}

	void reconn_reg_server(std::string host, short port, std::string sub_type, struct name_info& _info) {
		spdlog::trace("begin connect center server!");
		_center_caller->reconn_reg_server("hub", _info.name, host, port)->callBack([this, &_info]() {
			spdlog::trace("connect center sucessed!");
			is_reg_sucess = true;
		}, []() {
			spdlog::trace("connect center failed!");
		})->timeout(5 * 1000, []() {
			spdlog::trace("connect center timeout!");
		});
	}

	void reconn_reg_server(std::string sub_type, struct name_info& _info) {
		spdlog::trace("begin connect center server!");
		_center_caller->reconn_reg_server_mq("hub", _info.name)->callBack([this, &_info]() {
			spdlog::trace("connect center sucessed!");
			is_reg_sucess = true;
			}, []() {
				spdlog::trace("connect center failed!");
			})->timeout(5 * 1000, []() {
				spdlog::trace("connect center timeout!");
				});
	}
	
	void heartbeat(uint32_t tick) {
		spdlog::trace("heartbeat center!");
		auto this_ptr = shared_from_this();
		_center_caller->heartbeat(tick)->callBack([this_ptr] {
			spdlog::trace("heartbeat center server sucessed");
			this_ptr->timetmp = this_ptr->_timerservice->Tick;
		}, [] {
			spdlog::trace("heartbeat center server faild");
		})->timeout(5 * 1000, [] {
			spdlog::trace("heartbeat center server timeout");
		});;
	}

	void closed() {
		_center_caller->closed();
	}

public:
	bool is_reg_sucess;
	time_t timetmp;

private:
	std::shared_ptr<service::timerservice> _timerservice;
	std::shared_ptr<abelkhan::Ichannel> _center_ch;
	std::shared_ptr<abelkhan::center_caller> _center_caller;

};

}

#endif // !_centerproxy_h
