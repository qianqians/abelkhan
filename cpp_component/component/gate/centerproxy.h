/*
 * centerproxy.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _centerproxy_h
#define _centerproxy_h

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

	~centerproxy(){
	}

	void reg_server(std::string ip, short port, std::string uuid) {
		_center_caller->reg_server("gate", ip, port, uuid);
	}

public:
	bool is_reg_sucess;

private:
	std::shared_ptr<abelkhan::Ichannel> _center_ch;
	std::shared_ptr<abelkhan::center_caller> _center_caller;

};

}

#endif // !_centerproxy_h
