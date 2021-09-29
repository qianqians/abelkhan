
#ifndef _websocket_channel_h
#define _websocket_channel_h

#include <boost/any.hpp>
#include <boost/signals2.hpp>

#include <websocketpp/config/asio.hpp>
#include <websocketpp/config/asio_no_tls.hpp>
#include <websocketpp/connection.hpp>
#include <websocketpp/server.hpp>

#include <spdlog/spdlog.h>

#include <angmalloc.h>

#include "ringque.h"
#include "JsonParse.h"
#include "Ichannel.h"

namespace service
{

class webchannel : public juggle::Ichannel, public std::enable_shared_from_this<webchannel> {
private:
	static int count;

public:
	webchannel(std::shared_ptr<websocketpp::server<websocketpp::config::asio_tls> > _server, websocketpp::connection_hdl _hdl)
	{
		asio_tls_server = _server;
		hdl = _hdl;
		is_ssl = true;

		buff_size = 16 * 1024;
		buff_offset = 0;
		buff = (char*)angmalloc(buff_size);
		memset(buff, 0, buff_size);

		is_close = false;

		++count;
	}

	webchannel(std::shared_ptr<websocketpp::server<websocketpp::config::asio> > _server, websocketpp::connection_hdl _hdl)
	{
		asio_server = _server;
		hdl = _hdl;
		is_ssl = false;

		buff_size = 16 * 1024;
		buff_offset = 0;
		buff = (char*)angmalloc(buff_size);
		memset(buff, 0, buff_size);

		is_close = false;

		++count;
	}

	virtual ~webchannel() {
		spdlog::trace("webchannel destruction obj count:{0}!", --count);
		angfree(buff);
	}

	boost::signals2::signal<void(std::shared_ptr<webchannel>)> sigconnexception;

	void recv(std::string resv_data)
	{
		if (is_close) {
			return;
		}

		try {
			while ((buff_offset + resv_data.size()) > buff_size)
			{
				buff_size *= 2;
				auto new_buff = (char*)angmalloc(buff_size);
				memset(new_buff, 0, buff_size);
				memcpy(new_buff, buff, buff_offset);
				angfree(buff);
				buff = new_buff;
			}
			memcpy(buff + buff_offset, resv_data.c_str(), resv_data.size());
			buff_offset += (int32_t)resv_data.size();

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
					std::string json_str((char*)(json_buff), len);
					try
					{
						spdlog::trace("recv:{0}", json_str);
						Fossilizid::JsonParse::JsonObject obj;
						Fossilizid::JsonParse::unpacker(obj, json_str);
						que.push(std::any_cast<Fossilizid::JsonParse::JsonArray>(obj));
					}
					catch (Fossilizid::JsonParse::jsonformatexception e)
					{
						spdlog::error("webchannel recv error:{0}", json_str);
						sigconnexception(shared_from_this());

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
				auto new_buff = new char[buff_size];
				memset(new_buff, 0, buff_size);
				memcpy(new_buff, &buff[tmp_buff_offset], buff_offset);
				angfree(buff);
				buff = new_buff;
			}
		}
		catch (std::exception e) {
			spdlog::error("webchannel recv exception error:{0}", e.what());
			disconnect();
		}
	}

public:
	void disconnect() {
		is_close = true;

		try
		{
			if (is_ssl) {
				asio_tls_server->close(hdl, websocketpp::close::status::normal, "");
			}
			else {
				asio_server->close(hdl, websocketpp::close::status::normal, "");
			}
		}
		catch (std::exception e) {
			spdlog::error("webchannel disconnect error:{0}", e.what());
		}
	}

	bool pop(Fossilizid::JsonParse::JsonArray  & out)
	{
		if (que.empty())
		{
			return false;
		}

		return que.pop(out);
	}

	void push(Fossilizid::JsonParse::JsonArray in)
	{
		if (is_close) {
			return;
		}

		try {
			std::string data;
			Fossilizid::JsonParse::pack(in, data);
			size_t len = data.size();
			unsigned char * _data = (unsigned char*)angmalloc(len + 4);
			_data[0] = len & 0xff;
			_data[1] = len >> 8 & 0xff;
			_data[2] = len >> 16 & 0xff;
			_data[3] = len >> 24 & 0xff;
			memcpy(&_data[4], data.c_str(), data.size());
			size_t datasize = len + 4;

			if (is_ssl) {
				asio_tls_server->send(hdl, _data, datasize, websocketpp::frame::opcode::binary);
			}
			else {
				asio_server->send(hdl, _data, datasize, websocketpp::frame::opcode::binary);
			}

			angfree(_data);

			spdlog::trace("push:{0}", data);
		}
		catch (std::exception e) {
			spdlog::error("webchannel push error:{0}", e.what());
			is_close = true;
		}
	}

	void send(char* data, size_t data_size) {
		if (is_ssl) {
			asio_tls_server->send(hdl, data, data_size, websocketpp::frame::opcode::binary);
		}
		else {
			asio_server->send(hdl, data, data_size, websocketpp::frame::opcode::binary);
		}
	}
	
public:
	websocketpp::connection_hdl hdl;

private:
	concurrent::ringque< Fossilizid::JsonParse::JsonArray > que;

	std::shared_ptr<websocketpp::server<websocketpp::config::asio_tls> > asio_tls_server;
	std::shared_ptr<websocketpp::server<websocketpp::config::asio> > asio_server;
	bool is_ssl;

	char * buff;
	int32_t buff_size;
	int32_t buff_offset;

	bool is_close;

};

}

#endif
