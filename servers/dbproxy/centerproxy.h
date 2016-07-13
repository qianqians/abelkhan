/*
 * centerproxy.h
 *
 *  Created on: 2016-7-11
 *      Author: qianqians
 */
#ifndef _centerproxy_h
#define _centerproxy_h

#include <Ichannel.h>

#include <caller/centercaller.h>

namespace dbproxy{

class centerproxy {
public:
	centerproxy(boost::shared_ptr<juggle::Ichannel> ch) {
		is_reg_sucess = false;
		_center_ch = ch;
		_center_caller = boost::make_shared<caller::center>(_center_ch);
	}

	~centerproxy(){
	}

	void reg_server(std::string ip, short port, std::string uuid) {
		std::cout << "begin connect center server" << std::endl;
		_center_caller->reg_server("dbproxy", ip, port, uuid);
	}

public:
	bool is_reg_sucess;

private:
	boost::shared_ptr<juggle::Ichannel> _center_ch;
	boost::shared_ptr<caller::center> _center_caller;

};

}

#endif // !_centerproxy_h
