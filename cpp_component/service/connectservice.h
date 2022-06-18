
#ifndef _connectservice_h
#define _connectservice_h

#include <abelkhan.h>

#include "channel.h"

namespace service
{

class connectservice {
public:
	connectservice(std::shared_ptr<asio::io_service> service)
	{
		_service = service;
	}

	void connect(std::string ip, short port, std::function<void(std::shared_ptr<abelkhan::Ichannel>)> callback)
	{
		auto s = std::make_shared<asio::ip::tcp::socket>(*_service);
		s->async_connect(asio::ip::tcp::endpoint(asio::ip::address::from_string(ip), port), [callback, s] (const asio::error_code &ec) {
			if (!ec) {
				auto ch = std::make_shared<channel>(s);
				ch->start();

				callback(ch);
			}
		});
	}

private:
	std::shared_ptr<asio::io_service> _service;

};

}

#endif
