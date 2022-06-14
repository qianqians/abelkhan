/*
 * qianqians
 * 2022-6-14
 * redismqchannel.cpp
 */

#include "redismqservice.h"
#include "redismqchannel.h"

namespace service {
	
redismqchannel::redismqchannel(std::string _channel_name, std::shared_ptr<redismqservice> _service) {
	channel_name = _channel_name;
	service = _service;
}

void redismqchannel::send(const char* data, size_t len) {
	service->sendmsg(channel_name, data, len);
}

}