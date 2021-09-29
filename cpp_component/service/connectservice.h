
#ifndef _connectservice_h
#define _connectservice_h

#include <factory.h>

#include "channel.h"
#include "process_.h"

namespace service
{

class connectservice {
public:
	connectservice(std::shared_ptr<juggle::process> process)
	{
		_process = process;
	}

	std::shared_ptr<juggle::Ichannel> connect(std::string ip, short port)
	{
		auto s = Fossilizid::pool::factory::create<boost::asio::ip::tcp::socket>(_service);
		s->connect(boost::asio::ip::tcp::endpoint(boost::asio::ip::address::from_string(ip), port));
		auto ch = Fossilizid::pool::factory::create<channel>(s);

		_process->reg_channel(ch);
		ch->start();

		return ch;
	}

	void poll()
	{
		_service.poll();
	}

private:
	boost::asio::io_service _service;
	std::shared_ptr<juggle::process> _process;

};

}

#endif
