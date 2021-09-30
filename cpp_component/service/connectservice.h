
#ifndef _connectservice_h
#define _connectservice_h

#include <abelkhan.h>

#include "channel.h"

namespace service
{

class connectservice {
public:
	connectservice(std::shared_ptr<boost::asio::io_service> service)
	{
		_service = service;
	}

	std::shared_ptr<abelkhan::Ichannel> connect(std::string ip, short port)
	{
		auto s = std::make_shared<boost::asio::ip::tcp::socket>(*_service);
		s->connect(boost::asio::ip::tcp::endpoint(boost::asio::ip::address::from_string(ip), port));
		auto ch = std::make_shared<channel>(s);
		ch->start();

		return ch;
	}

private:
	std::shared_ptr<boost::asio::io_service> _service

};

}

#endif
