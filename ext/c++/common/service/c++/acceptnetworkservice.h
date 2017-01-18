/*
 * qianqians
 * 2016/7/5
 * acceptnetworkservice.h
 */
#ifndef _acceptnetworkservice_h
#define _acceptnetworkservice_h

#include <boost/asio.hpp>
#include <boost/signals2.hpp>

#include <memory>

#include <process_.h>

#include "service.h"
#include "channel.h"

namespace service {

class acceptnetworkservice : public service {
public:
	acceptnetworkservice(std::string ip, short port, std::shared_ptr<juggle::process> process_) : _acceptor(_service){
		_process = process_;

		_acceptor.open(boost::asio::ip::tcp::v4());
		_acceptor.bind(boost::asio::ip::tcp::endpoint(boost::asio::ip::address_v4::from_string(ip), port));
		_acceptor.listen();

		auto s = std::make_shared<boost::asio::ip::tcp::socket>(_service);
		_acceptor.async_accept(*s, std::bind(&acceptnetworkservice::onAccept, this, s, std::placeholders::_1));
	}

	~acceptnetworkservice(){
	}

	boost::signals2::signal<void(std::shared_ptr<channel>)> sigchannelconnect;
	void onAccept(std::shared_ptr<boost::asio::ip::tcp::socket> s, const boost::system::error_code& error){
		if (!error) {
			auto ch = std::make_shared<channel>(s);
			_process->reg_channel(ch);
			ch->sigdisconn.connect(std::bind(&acceptnetworkservice::onChannelDisconn, this, std::placeholders::_1));
			sigchannelconnect(ch);
		}
		else {
			s->close();
		}

		s = std::make_shared<boost::asio::ip::tcp::socket>(_service);
		_acceptor.async_accept(*s, std::bind(&acceptnetworkservice::onAccept, this, s, std::placeholders::_1));
	}

	boost::signals2::signal<void(std::shared_ptr<channel>)> sigchanneldisconnect;
	void onChannelDisconn(std::shared_ptr<channel> ch) {
		if (!sigchanneldisconnect.empty()) {
			sigchanneldisconnect(ch);
		}

		_process->unreg_channel(ch);
	}

	void poll(int64_t tick) {
		_service.poll_one();
	}

private:
	std::shared_ptr<juggle::process> _process;

	boost::asio::io_service _service;
	boost::asio::ip::tcp::acceptor _acceptor;

};

}

#endif // !_acceptnetworkservice_h