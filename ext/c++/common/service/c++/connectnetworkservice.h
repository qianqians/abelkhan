/*
 * qianqians
 * 2016/7/5
 * connectnetworkservice.h
 */
#ifndef _connectnetworkservice_h
#define _connectnetworkservice_h

#include <memory>
#include <functional>

#include <boost/asio.hpp>
#include <boost/signals2.hpp>

#include <process_.h>

#include "service.h"
#include "channel.h"

namespace service {

class connectnetworkservice :public service{
public:
	connectnetworkservice(std::shared_ptr<juggle::process> process_) {
		_process = process_;
	}

	~connectnetworkservice(){
	}

	std::shared_ptr<channel> connect(std::string ip, short port){
		try {
			std::shared_ptr<boost::asio::ip::tcp::socket> s = std::make_shared<boost::asio::ip::tcp::socket>(_service);
			s->connect(boost::asio::ip::tcp::endpoint(boost::asio::ip::address_v4::from_string(ip), port));

			auto ch = std::make_shared<channel>(s);

			ch->sigdisconn.connect(std::bind(&connectnetworkservice::onChannelDisconn, this, std::placeholders::_1));

			_process->reg_channel(ch);

			return ch;
		}
		catch(...){
			return nullptr;
		}
	}

	boost::signals2::signal<void(std::shared_ptr<channel>) > sigchanneldisconn;
	void onChannelDisconn(std::shared_ptr<channel> ch){
		if (!sigchanneldisconn.empty()){
			sigchanneldisconn(ch);
		}
	}

	void poll(int64_t tick){
		_service.poll_one();
	}

private:
	std::shared_ptr<juggle::process> _process;
	boost::asio::io_service _service;

};

}

#endif // !_connectnetworkservice_h