#ifndef _acceptservice_h
#define _acceptservice_h

#include <functional>

#include <boost/asio.hpp>

#include "objpool.h"
#include "channel.h"
#include "gc_poll.h"

namespace service
{

class acceptservice {
public:
	acceptservice(std::string ip, short port, std::shared_ptr<boost::asio::io_service> service) : _acceptor(*service){

		_service = service;

		boost::asio::ip::tcp::endpoint ep(boost::asio::ip::address::from_string("0.0.0.0"), port);
		_acceptor.open(ep.protocol());

		boost::asio::socket_base::reuse_address opt(true);
		_acceptor.set_option(opt);

		_acceptor.bind(ep);
		_acceptor.listen();

		auto s = std::make_shared<boost::asio::ip::tcp::socket>(*service);
		_acceptor.async_accept(*s, std::bind(&acceptservice::onAccept, this, s, std::placeholders::_1));
	}

	concurrent::signals<void(std::shared_ptr<abelkhan::Ichannel>)> sigchannelconnect;
	void onAccept(std::shared_ptr<boost::asio::ip::tcp::socket> s, boost::system::error_code ec) {
		if (ec) {
			s->close();
		}
		else {
			auto ch = _ch_pool.make_obj(s);
			ch->sigondisconn.connect(std::bind(&acceptservice::onChannelDisconn, this, std::placeholders::_1));
			ch->sigdisconn.connect(std::bind(&acceptservice::ChannelDisconn, this, std::placeholders::_1));
			ch->start();

			sigchannelconnect.emit(ch);
		}

		s = std::make_shared<boost::asio::ip::tcp::socket>(*_service);
		_acceptor.async_accept(*s, std::bind(&acceptservice::onAccept, this, s, std::placeholders::_1));
	}

	concurrent::signals<void(std::shared_ptr<abelkhan::Ichannel>)> sigchanneldisconnect;
	void onChannelDisconn(std::shared_ptr<channel> ch) {
		if (!sigchanneldisconnect.empty()) {
			sigchanneldisconnect.emit(ch);
		}
		_ch_pool.recycle(ch);
	}

	void ChannelDisconn(std::shared_ptr<channel> ch) {
		if (!sigchanneldisconnect.empty()) {
			sigchanneldisconnect.emit(ch);
		}
		_ch_pool.recycle(ch);
	}

private:
	std::shared_ptr<boost::asio::io_service> _service;
	boost::asio::ip::tcp::acceptor _acceptor;

	service::objpool<channel> _ch_pool;

};

}

#endif
