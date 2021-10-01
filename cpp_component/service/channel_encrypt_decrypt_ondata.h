#ifndef _channel_encrypt_decrypt_ondata_h
#define _channel_encrypt_decrypt_ondata_h

#include <abelkhan.h>
#include <log.h>

#include "modulemng_handle.h"

namespace service
{

class channel_encrypt_decrypt_ondata {
public:
	channel_encrypt_decrypt_ondata(std::shared_ptr<abelkhan::Ichannel> _ch)
	{
		ch = _ch;

		buff_size = 16 * 1024;
		buff_offset = 0;
		buff = (char*)malloc(buff_size);
		memset(buff, 0, buff_size);

		is_compress_and_encrypt = false;
	}

	void set_xor_key(uint64_t _xor_key) {
		is_compress_and_encrypt = true;

		xor_key[0] = (uint8_t)(_xor_key & 0xff);
		xor_key[1] = (uint8_t)((_xor_key >> 8) & 0xff);
		xor_key[2] = (uint8_t)((_xor_key >> 16) & 0xff);
		xor_key[3] = (uint8_t)((_xor_key >> 24) & 0xff);
		xor_key[4] = (uint8_t)((_xor_key >> 32) & 0xff);
		xor_key[5] = (uint8_t)((_xor_key >> 40) & 0xff);
		xor_key[6] = (uint8_t)((_xor_key >> 48) & 0xff);
		xor_key[7] = (uint8_t)((_xor_key >> 56) & 0xff);
	}

	void xor_key_encrypt_decrypt(char* data, size_t len) {
		for (size_t i = 0; i < len; i++) {
			data[i] ^= xor_key[i & 0x07];
		}
	}

	virtual ~channel_encrypt_decrypt_ondata(){
		free(buff);
	}

	bool is_compress_and_encrypt;

	void recv(const char * data, size_t len)
	{

		while ((buff_offset + len) > buff_size)
		{
			buff_size *= 2;
			auto new_buff = (char*)malloc(buff_size);
			memset(new_buff, 0, buff_size);
			memcpy(new_buff, buff, buff_offset);
			free(buff);
			buff = new_buff;
		}
		memcpy(buff + buff_offset, data, len);
		buff_offset += (int32_t)len;

		try{
			int32_t tmp_buff_len = buff_offset;
			int32_t tmp_buff_offset = 0;
			while (tmp_buff_len > (tmp_buff_offset + 4))
			{
				auto tmp_buff = (unsigned char *)buff + tmp_buff_offset;
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
					if (!obj.is_array()) {
						spdlog::error("channel recv parse MsgPack error");
						ch->disconnect();
						return;
					}
					try
					{
						_modulemng->process_event(ch, obj.array_items());
					}
					catch (std::exception e)
					{
						spdlog::error("channel do rpc callback error");
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
			if (tmp_buff_len > tmp_buff_offset)
			{
				auto new_buff = (char*)malloc(buff_size);
				memset(new_buff, 0, buff_size);
				memcpy(new_buff, &buff[tmp_buff_offset], buff_offset);
				free(buff);
				buff = new_buff;
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

	uint8_t xor_key[8] = { 0 };

};

}

#endif
