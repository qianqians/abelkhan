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
	}

	void Init() {
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

	void send(char* data, size_t len)
	{
		if (is_close) {
			return;
		}

		try {
			if (is_ssl) {
				asio_tls_server->send(hdl, data, len, websocketpp::frame::opcode::binary);
			}
			else {
				asio_server->send(hdl, data, len, websocketpp::frame::opcode::binary);
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

	bool is_close;

};

}

#endif
