
#ifndef _channel_h
#define _channel_h

#include <list>
#include <iostream>
#include <any>

#include <boost/asio.hpp>
#include <boost/signals2.hpp>
#include <boost/thread.hpp>

#include <angmalloc.h>

#include "JsonParse.h"
#include "Ichannel.h"
#include "compress_and_encrypt.h"

namespace service
{

class channel : public juggle::Ichannel, public std::enable_shared_from_this<channel> {
public:
	channel(std::shared_ptr<boost::asio::ip::tcp::socket> _s)
	{
		s = _s;

		buff_size = 16 * 1024;
		buff_offset = 0;
		buff = (char*)angmalloc(buff_size);
		memset(buff, 0, buff_size);

		is_close = false;
		is_compress_and_encrypt = false;
	}

	void start()
	{
		memset(read_buff, 0, 16 * 1024);
		s->async_read_some(boost::asio::buffer(read_buff, 16 * 1024), std::bind(&channel::onRecv, shared_from_this(), std::placeholders::_1, std::placeholders::_2));
	}

	~channel(){
		angfree(buff);
	}

	boost::signals2::signal<void(std::shared_ptr<channel>)> sigondisconn;
	boost::signals2::signal<void(std::shared_ptr<channel>)> sigdisconn;

	bool is_compress_and_encrypt;
	unsigned char xor_key;

private:
	static void onRecv(std::shared_ptr<channel> ch, const boost::system::error_code& error, std::size_t bytes_transferred){
		if (ch->is_close) {
			return;
		}

		if (error){
			ch->is_close = true;
			ch->sigondisconn(ch);
			return;
		}

		if (bytes_transferred == 0){
			ch->is_close = true;
			ch->sigondisconn(ch);
			return;
		}

		while ((ch->buff_offset + bytes_transferred) > ch->buff_size)
		{
			ch->buff_size *= 2;
			auto new_buff = (char*)angmalloc(ch->buff_size);
			memset(new_buff, 0, ch->buff_size);
			memcpy(new_buff, ch->buff, ch->buff_offset);
			angfree(ch->buff);
			ch->buff = new_buff;
		}
		memcpy(ch->buff + ch->buff_offset, ch->read_buff, bytes_transferred);
		ch->buff_offset += (int32_t)bytes_transferred;

		ch->recv();
	}

	void recv()
	{
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

					auto json_buff = &tmp_buff[4];
					if (is_compress_and_encrypt)
					{
						std::lock_guard<std::mutex> l(compress_and_encrypt::e_and_c_mutex);
						len = (uint32_t)compress_and_encrypt::encrypt_and_compress(json_buff, (size_t)len, xor_key);
						auto tmp_json_buff = new unsigned char[len];
						memcpy(tmp_json_buff, compress_and_encrypt::e_and_c_output_buff, len);
						json_buff = tmp_json_buff;
					}
					std::string json_str((char*)(json_buff), len);
					if (is_compress_and_encrypt)
					{
						delete[] json_buff;
					}
					try
					{
						Fossilizid::JsonParse::JsonObject obj;
						Fossilizid::JsonParse::unpacker(obj, json_str);
						que.push_back(std::any_cast<Fossilizid::JsonParse::JsonArray>(obj));
					}
					catch (Fossilizid::JsonParse::jsonformatexception e)
					{
						spdlog::error("channel recv jsonformatexception error:{0}", json_str);
						disconnect();

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
				auto new_buff = (char*)angmalloc(buff_size);
				memset(new_buff, 0, buff_size);
				memcpy(new_buff, &buff[tmp_buff_offset], buff_offset);
				angfree(buff);
				buff = new_buff;
			}

			memset(read_buff, 0, 16 * 1024);
			s->async_read_some(boost::asio::buffer(read_buff, 16 * 1024), boost::bind(&channel::onRecv, shared_from_this(), boost::placeholders::_1, boost::placeholders::_2));
		}
		catch (std::exception e) {
			spdlog::error("channel recv exception error:{0}", e.what());
			disconnect();
		}
	}

public:
	void disconnect() {
		is_close = true;
		sigdisconn(shared_from_this());

		try
		{
			s->close();
		}
		catch (std::exception e) {
			spdlog::error("channel disconnect error:{0}", e.what());
		}
	}

	bool pop(Fossilizid::JsonParse::JsonArray  & out)
	{
		if (que.empty())
		{
			return false;
		}

		out = que.front();
		que.pop_front();

		return true;
	}

	void push(Fossilizid::JsonParse::JsonArray in)
	{
		if (is_close) {
			return;
		}

		if (!s->is_open()){
			return;
		}

		try {
			std::string data;
			Fossilizid::JsonParse::pack(in, data);
			if (!is_compress_and_encrypt)
			{
				size_t len = data.size();
				unsigned char * _data = (unsigned char*)angmalloc(len + 4);
				_data[0] = len & 0xff;
				_data[1] = len >> 8 & 0xff;
				_data[2] = len >> 16 & 0xff;
				_data[3] = len >> 24 & 0xff;
				memcpy(&_data[4], data.c_str(), data.size());
				size_t datasize = len + 4;

				size_t offset = 0;
				while (offset < datasize) {
					try {
						offset += s->send(boost::asio::buffer(&_data[offset], datasize - offset));
					}
					catch (boost::system::system_error e) {
						if (e.code() == boost::asio::error::would_block) {
							boost::this_thread::sleep(boost::get_system_time() + boost::posix_time::microseconds(1));
							continue;
						}
						else {
							spdlog::error("channel push error:{0}", e.what());
							is_close = true;
							break;
						}
					}
				}

				angfree(_data);
			}
			else
			{
				std::lock_guard<std::mutex> l(compress_and_encrypt::c_and_e_mutex);
				auto len = compress_and_encrypt::compress_and_encrypt((unsigned char*)data.c_str(), data.size(), xor_key);
				unsigned char * _data = (unsigned char*)angmalloc(len + 4);
				_data[0] = len & 0xff;
				_data[1] = len >> 8 & 0xff;
				_data[2] = len >> 16 & 0xff;
				_data[3] = len >> 24 & 0xff;
				memcpy(&_data[4], compress_and_encrypt::c_and_e_output_buff, len);
				size_t datasize = len + 4;

				size_t offset = 0;
				while (offset < datasize) {
					try {
						offset += s->send(boost::asio::buffer(&_data[offset], datasize - offset));
					}
					catch (boost::system::system_error e) {
						if (e.code() == boost::asio::error::would_block) {
							boost::this_thread::sleep(boost::get_system_time() + boost::posix_time::microseconds(1));
							continue;
						}
						else {
							spdlog::error("channel push error:{0}", e.what());
							is_close = true;
							break;
						}
					}
				}

				angfree(_data);
			}
		}
		catch (std::exception e) {
			spdlog::error("channel push exception error:{0}", e.what());
			is_close = true;
		}
	}

private:
	std::list< Fossilizid::JsonParse::JsonArray > que;

	std::shared_ptr<boost::asio::ip::tcp::socket> s;

	char read_buff[16 * 1024];

	char * buff;
	int32_t buff_size;
	int32_t buff_offset;

	bool is_close;

};

}

#endif
