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

	void recv(char* data, size_t length)
	{
		auto tmp_buff = (unsigned char*)data;
		uint32_t len = (uint32_t)tmp_buff[0] | ((uint32_t)tmp_buff[1] << 8) | ((uint32_t)tmp_buff[2] << 16) | ((uint32_t)tmp_buff[3] << 24);

		if ((len + 4) <= (uint32_t)length)
		{
			auto proto_buff = &tmp_buff[4];
			std::string err;
			auto obj = msgpack11::MsgPack::parse((const char*)proto_buff, len, err);
			if (!obj.is_array()) {
				spdlog::error("channel recv parse MsgPack error");
			}
			else {
				try
				{
					_modulemng->process_event(shared_from_this(), obj.array_items());
				}
				catch (std::exception e)
				{
					spdlog::error("channel do rpc callback error");
				}
			}
		}
	}
	
	void send(const char* data, size_t len);

};

}

#endif //!_redismq_channel_h