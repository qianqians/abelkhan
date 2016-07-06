/*
 * qianqians
 * 2016/7/5
 * connectnetworkservice.h
 */
#ifndef _connectnetworkservice_h
#define _connectnetworkservice_h

#include <boost/asio.hpp>
#include <boost/signals2.hpp>
#include <boost/shared_ptr.hpp>

#include <process_.h>

#include "service.h"
#include "channel.h"

namespace service {

class connectnetworkservice :public service{
public:
	connectnetworkservice(boost::shared_ptr<juggle::process> process_) {
		_process = process_;
	}

	~connectnetworkservice(){
	}

	boost::shared_ptr<channel> connect(std::string ip, short port){
		try {
			boost::shared_ptr<boost::asio::ip::tcp::socket> s = boost::make_shared<boost::asio::ip::tcp::socket>(_service);
			//s->bind(boost::asio::ip::tcp::endpoint(boost::asio::ip::address_v4::from_string("127.0.0.1"), 0));
			s->connect(boost::asio::ip::tcp::endpoint(boost::asio::ip::address_v4::from_string(ip), port));

			auto ch = boost::make_shared<channel>(s);

			ch->sigdisconn.connect(boost::bind(&connectnetworkservice::onChannelDisconn, this, _1));

			_process->reg_channel(ch);

			return ch;
		}
		catch(...){
			return nullptr;
		}
	}

	boost::signals2::signal<void(boost::shared_ptr<channel>) > sigchanneldisconn;
	void onChannelDisconn(boost::shared_ptr<channel> ch){
		if (!sigchanneldisconn.empty()){
			sigchanneldisconn(ch);
		}
	}

	void poll(int64_t tick){
		_service.poll_one();
	}

private:
	boost::shared_ptr<juggle::process> _process;
	boost::asio::io_service _service;

};

}

#endif // !_connectnetworkservice_h