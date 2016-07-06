/*
 * qianqians
 * 2016/7/5
 * acceptnetworkservice.h
 */
#ifndef _acceptnetworkservice_h
#define _acceptnetworkservice_h

#include <boost/asio.hpp>
#include <boost/signals2.hpp>
#include <boost/shared_ptr.hpp>

#include <process_.h>

#include "service.h"
#include "channel.h"

namespace service {

class acceptnetworkservice : public service {
public:
	acceptnetworkservice(std::string ip, short port, boost::shared_ptr<juggle::process> process_) : _acceptor(_service){
		_process = process_;

		_acceptor.open(boost::asio::ip::tcp::v4());
		_acceptor.bind(boost::asio::ip::tcp::endpoint(boost::asio::ip::address_v4::from_string(ip), port));
		_acceptor.listen();

		auto s = boost::make_shared<boost::asio::ip::tcp::socket>(_service);
		_acceptor.async_accept(*s, boost::bind(&acceptnetworkservice::onAccept, this, s, boost::asio::placeholders::error));
	}

	~acceptnetworkservice(){
	}

	boost::signals2::signal<void(boost::shared_ptr<channel>)> sigchannelconnect;
	void onAccept(boost::shared_ptr<boost::asio::ip::tcp::socket> s, const boost::system::error_code& error){
		if (!error) {
			auto ch = boost::make_shared<channel>(s);
			_process->reg_channel(ch);
			ch->sigdisconn.connect(boost::bind(&acceptnetworkservice::onChannelDisconn, this, _1));
			sigchannelconnect(ch);
		}
		else {
			s->close();
		}

		s = boost::make_shared<boost::asio::ip::tcp::socket>(_service);
		_acceptor.async_accept(*s, boost::bind(&acceptnetworkservice::onAccept, this, s, boost::asio::placeholders::error));
	}

	boost::signals2::signal<void(boost::shared_ptr<channel>)> sigchanneldisconnect;
	void onChannelDisconn(boost::shared_ptr<channel> ch) {
		if (!sigchanneldisconnect.empty()) {
			sigchanneldisconnect(ch);
		}

		_process->unreg_channel(ch);
	}

	void poll(int64_t tick) {
		_service.poll_one();
	}

private:
	boost::shared_ptr<juggle::process> _process;
	boost::asio::io_service _service;
	boost::asio::ip::tcp::acceptor _acceptor;

};

}

#endif // !_acceptnetworkservice_h