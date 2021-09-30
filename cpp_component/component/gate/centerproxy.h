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

namespace gate{

class centerproxy {
public:
	centerproxy(std::shared_ptr<abelkhan::Ichannel> ch) {
		is_reg_sucess = false;
		_center_ch = ch;
		_center_caller = std::make_shared<abelkhan::center_caller>(_center_ch);
	}

	virtual ~centerproxy(){
	}

	void reg_server(std::string ip, short port, std::string uuid) {
		_center_caller->reg_server("gate", ip, port, uuid)->callBack([this](){
			is_reg_sucess = true;
			spdlog::trace("connect center server sucess!");
		}, [](){
			spdlog::trace("connect center server faild!");
		})->timeout(1000, []() {
			spdlog::trace("connect center server timeout!");
		});
	}

public:
	bool is_reg_sucess;

private:
	std::shared_ptr<abelkhan::Ichannel> _center_ch;
	std::shared_ptr<abelkhan::center_caller> _center_caller;

};

}

#endif // !_centerproxy_h
