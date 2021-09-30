#ifndef _websocket_channel_h
#define _websocket_channel_h

#include <websocketpp/config/asio.hpp>
#include <websocketpp/config/asio_no_tls.hpp>
#include <websocketpp/connection.hpp>
#include <websocketpp/server.hpp>

#include <spdlog/spdlog.h>

#include <signals.h>
#include <abelkhan.h>

#include "channel_encrypt_decrypt_ondata.h"

namespace service
{

class webchannel : public abelkhan::Ichannel, public std::enable_shared_from_this<webchannel> {
public:
	webchannel(std::shared_ptr<websocketpp::server<websocketpp::config::asio_tls> > _server, websocketpp::connection_hdl _hdl)
	{
		asio_tls_server = _server;
		hdl = _hdl;
		is_ssl = true;

		is_close = false;
	}

	webchannel(std::shared_ptr<websocketpp::server<websocketpp::config::asio> > _server, websocketpp::connection_hdl _hdl)
	{
		asio_server = _server;
		hdl = _hdl;
		is_ssl = false;

		is_close = false;
	}

	virtual ~webchannel() {
		spdlog::trace("webchannel destruction obj!");
		free(_data);
	}

	void Init() {
		_data_size = 8 * 1024;
		_data = (unsigned char*)malloc(_data_size);

		ch_encrypt_decrypt_ondata = std::make_shared<channel_encrypt_decrypt_ondata>(shared_from_this());
	}

	concurrent::signals<void(std::shared_ptr<webchannel>)> sigconnexception;

	void recv(std::string resv_data)
	{
		if (is_close) {
			return;
		}

		try {
			ch_encrypt_decrypt_ondata->recv(resv_data.data(), resv_data.size());
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

	void send(std::string& data)
	{
		if (is_close) {
			return;
		}

		try {
			size_t len = data.size();
			if (_data_size < (len + 4)) {
				_data_size *= 2;
				free(_data);
				_data = (unsigned char*)malloc(_data_size);
			}
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
		}
		catch (std::exception e) {
			spdlog::error("webchannel push error:{0}", e.what());
			is_close = true;
		}
	}
	
public:
	websocketpp::connection_hdl hdl;

private:
	std::shared_ptr<websocketpp::server<websocketpp::config::asio_tls> > asio_tls_server;
	std::shared_ptr<websocketpp::server<websocketpp::config::asio> > asio_server;
	bool is_ssl;

	std::shared_ptr<channel_encrypt_decrypt_ondata> ch_encrypt_decrypt_ondata;

	unsigned char* _data;
	size_t _data_size;

	bool is_close;

};

}

#endif
