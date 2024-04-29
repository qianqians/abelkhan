#ifndef _channel_encrypt_decrypt_ondata_h
#define _channel_encrypt_decrypt_ondata_h

#include <gc.h>

#include <abelkhan.h>
#include <log.h>
#include <buffer.h>

#include "modulemng_handle.h"

namespace service
{

class channel_encrypt_decrypt_ondata {
public:
	channel_encrypt_decrypt_ondata(std::shared_ptr<abelkhan::Ichannel> _ch)
	{
		ch = _ch;

		buff_size = 0;
		buff_offset = 0;
		buff = nullptr;

		is_compress_and_encrypt = false;
	}

	void set_xor_key_crypt() {
		is_compress_and_encrypt = true;
	}

	static void xor_key_encrypt_decrypt(char* data, size_t len) {
		uint8_t xor_key[4] = { 0 };
		xor_key[0] = len & 0xff;
		xor_key[1] = (len >> 8) & 0xff;
		xor_key[2] = (len >> 16) & 0xff;
		xor_key[3] = (len >> 24) & 0xff;
		
		uint8_t base = 0;
		if (xor_key[0] != 0) {
			base = xor_key[0];
		}
		else if (xor_key[1] != 0) {
			base = xor_key[1];
		}
		else if (xor_key[2] != 0) {
			base = xor_key[2];
		}
		else if (xor_key[3] != 0) {
			base = xor_key[3];
		}
		
		if (xor_key[0] == 0) {
			xor_key[0] = base + base % 3;
		}
		if (xor_key[1] == 0) {
			xor_key[1] = base + base % 7;
		}
		if (xor_key[2] == 0) {
			xor_key[2] = base + base % 13;
		}
		if (xor_key[3] == 0) {
			xor_key[3] = base + base % 17;
		}
		
		for (size_t i = 0; i < len; i++) {
			data[i] ^= xor_key[i % 4];
		}
	}

	virtual ~channel_encrypt_decrypt_ondata(){
	}

	bool is_compress_and_encrypt;

	void recv(const char * data, size_t len)
	{
		auto tmp_buffer = buff_offset > 0 ? service::get_buffer(buff_offset + len) : data;
		if (buff_offset > 0) {
			memcpy((void*)tmp_buffer, buff, buff_offset);
			memcpy((void*)(tmp_buffer + buff_offset), data, len);
		}
		buff_offset += (int32_t)len;

		try{
			int32_t tmp_buff_len = buff_offset;
			int32_t tmp_buff_offset = 0;
			while (tmp_buff_len > (tmp_buff_offset + 4))
			{
				auto tmp_buff = (unsigned char *)tmp_buffer + tmp_buff_offset;
				uint32_t len = (uint32_t)tmp_buff[0] | ((uint32_t)tmp_buff[1] << 8) | ((uint32_t)tmp_buff[2] << 16) | ((uint32_t)tmp_buff[3] << 24);

				if ((len + tmp_buff_offset + 4) <= (uint32_t)tmp_buff_len)
				{
					tmp_buff_offset += len + 4;

					auto proto_buff = &tmp_buff[4];
					if (is_compress_and_encrypt) {
						xor_key_encrypt_decrypt((char*)proto_buff, len);
					}

					std::string err;
					auto obj = msgpack11::MsgPack::parse((const char*)proto_buff, len, err);
					if (obj.is_array()) {
						_modulemng->enque_event(ch, obj.array_items());
					}
					else {
						spdlog::error("channel recv parse MsgPack error");
						ch->disconnect();
						return;
					}
				}
				else
				{
					break;
				}
			}

			buff_offset = tmp_buff_len - tmp_buff_offset;
			if (buff_offset > 0)
			{
				if (buff_offset > buff_size) {
					buff_size = (buff_offset + 1023) / 1024 * 1024;
					buff = (char*)GC_malloc(buff_size);
				}
				if (buff) {
					memcpy(buff, &tmp_buffer[tmp_buff_offset], buff_offset);
				}
			}
		}
		catch (std::exception e) {
			spdlog::error("channel recv exception error:{0}", e.what());
			ch->disconnect();
		}
	}

private:
	std::shared_ptr<abelkhan::Ichannel> ch;

	char * buff;
	int32_t buff_size;
	int32_t buff_offset;

};

}

#endif
