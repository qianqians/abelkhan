
#ifndef _acceptservice_h
#define _acceptservice_h

#include <list>
#include <functional>

#include <boost/asio.hpp>
#include <boost/signals2.hpp>

#include <factory.h>

#include "channel.h"
#include "process_.h"
#include "gc_poll.h"

namespace service
{

class acceptservice {
public:
	acceptservice(std::string ip, short port, std::shared_ptr<juggle::process> process) : _acceptor(_service){
		_process = process;

		boost::asio::ip::tcp::endpoint ep(boost::asio::ip::address::from_string(ip), port);
		_acceptor.open(ep.protocol());

		boost::asio::socket_base::reuse_address opt(true);
		_acceptor.set_option(opt);

		_acceptor.bind(ep);
		_acceptor.listen();

		auto s = Fossilizid::pool::factory::create<boost::asio::ip::tcp::socket>(_service);
		_acceptor.async_accept(*s, std::bind(&acceptservice::onAccept, this, s, std::placeholders::_1));
	}

	boost::signals2::signal<void(std::shared_ptr<juggle::Ichannel>)> sigchannelconnect;
	void onAccept(std::shared_ptr<boost::asio::ip::tcp::socket> s, boost::system::error_code ec) {
		if (ec) {
			s->close();
		}
		else {
			auto ch = Fossilizid::pool::factory::create<channel>(s);
			_process->reg_channel(ch);
			ch->sigondisconn.connect(std::bind(&acceptservice::onChannelDisconn, this, std::placeholders::_1));
			ch->sigdisconn.connect(std::bind(&acceptservice::ChannelDisconn, this, std::placeholders::_1));
			sigchannelconnect(ch);

			ch->start();
		}

		s = Fossilizid::pool::factory::create<boost::asio::ip::tcp::socket>(_service);
		_acceptor.async_accept(*s, std::bind(&acceptservice::onAccept, this, s, std::placeholders::_1));
	}

	boost::signals2::signal<void(std::shared_ptr<juggle::Ichannel>)> sigchanneldisconnect;
	void onChannelDisconn(std::shared_ptr<channel> ch) {
		if (!sigchanneldisconnect.empty()) {
			sigchanneldisconnect(ch);
		}

		service::gc_put([this, ch]() {
			_process->unreg_channel(ch);
		});
	}

	void ChannelDisconn(std::shared_ptr<channel> ch) {
		if (!sigchanneldisconnect.empty()) {
			sigchanneldisconnect(ch);
		}

		service::gc_put([this, ch]() {
			_process->unreg_channel(ch);
		});
	}

	void poll(){
		_service.poll();
	}

private:
	boost::asio::io_service _service;
	boost::asio::ip::tcp::acceptor _acceptor;

	std::shared_ptr<juggle::process> _process;

};

}

#endif
