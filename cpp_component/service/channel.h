
#ifndef _channel_h
#define _channel_h

#include <list>
#include <iostream>
#include <any>

#include <boost/asio.hpp>
#include <boost/thread.hpp>
#include <msgpack11.hpp>

#include <signals.h>
#include <abelkhan.h>

#include <log.h>

namespace service
{

class channel : public abelkhan::Ichannel, public std::enable_shared_from_this<channel> {
public:
	channel(std::shared_ptr<boost::asio::ip::tcp::socket> _s)
	{
		s = _s;

		buff_size = 16 * 1024;
		buff_offset = 0;
		buff = (char*)malloc(buff_size);
		memset(buff, 0, buff_size);

		is_close = false;
	}

	void start()
	{
		memset(read_buff, 0, 16 * 1024);
		s->async_read_some(boost::asio::buffer(read_buff, 16 * 1024), std::bind(&channel::onRecv, shared_from_this(), std::placeholders::_1, std::placeholders::_2));
	}

	virtual ~channel(){
		free(buff);
	}

	concurrent::signals<void(std::shared_ptr<channel>)> sigondisconn;
	concurrent::signals<void(std::shared_ptr<channel>)> sigdisconn;

	unsigned char xor_key;

private:
	static void onRecv(std::shared_ptr<channel> ch, const boost::system::error_code& error, std::size_t bytes_transferred){
		if (ch->is_close) {
			return;
		}

		if (error){
			ch->is_close = true;
			ch->sigondisconn.emit(ch);
			return;
		}

		if (bytes_transferred == 0){
			ch->is_close = true;
			ch->sigondisconn.emit(ch);
			return;
		}

		while ((ch->buff_offset + bytes_transferred) > ch->buff_size)
		{
			ch->buff_size *= 2;
			auto new_buff = (char*)malloc(ch->buff_size);
			memset(new_buff, 0, ch->buff_size);
			memcpy(new_buff, ch->buff, ch->buff_offset);
			free(ch->buff);
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

					auto proto_buff = &tmp_buff[4];
					std::string err;
					auto obj = msgpack11::MsgPack::parse((const char*)proto_buff, len, err);
					if (!obj.is_array()) {
						spdlog::error("channel recv parse MsgPack error");
						disconnect();
						return;
					}
					try
					{
						
					}
					catch (std::exception e)
					{
						spdlog::error("channel do rpc callback error");
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
				auto new_buff = (char*)malloc(buff_size);
				memset(new_buff, 0, buff_size);
				memcpy(new_buff, &buff[tmp_buff_offset], buff_offset);
				free(buff);
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
		sigdisconn.emit(shared_from_this());

		try
		{
			s->close();
		}
		catch (std::exception e) {
			spdlog::error("channel disconnect error:{0}", e.what());
		}
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
	std::shared_ptr<boost::asio::ip::tcp::socket> s;

	char read_buff[16 * 1024];

	char * buff;
	int32_t buff_size;
	int32_t buff_offset;

	bool is_close;

};

}

#endif
