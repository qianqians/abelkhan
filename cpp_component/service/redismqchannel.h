/*
 * qianqians
 * 2022-6-14
 * redismqchannel.h
 */

#ifndef _redismq_channel_h
#define _redismq_channel_h

#include <memory>
#include <string>

#include <abelkhan.h>

#include "channel_encrypt_decrypt_ondata.h"

namespace service {
	
class redismqservice;
class redismqchannel : public abelkhan::Ichannel, public std::enable_shared_from_this<redismqchannel> {
private:
	std::string channel_name;
	std::shared_ptr<redismqservice> service;

public:
	redismqchannel(std::string _channel_name, std::shared_ptr<redismqservice> _service);

	void disconnect()
	{
	}

	bool is_xor_key_crypt() {
		return false;
	}

	void normal_crypt(char* data, size_t len) {
	}

	void recv(msgpack11::MsgPack obj)
	{
		if (obj.is_array()) {
			_modulemng->enque_event(shared_from_this(), obj.array_items());
		}
		else {
			spdlog::error("redismqchannel recv parse MsgPack error");
		}
	}
	
	void send(const char* data, size_t len);

};

}

#endif //!_redismq_channel_h