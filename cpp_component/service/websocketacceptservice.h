
#ifndef _websocket_acceptservice_h
#define _websocket_acceptservice_h

#include <thread>
#include <functional>
#include <exception>

#include <boost/signals2.hpp>

#include "websocketchannel.h"
#include "process_.h"
#include "gc_poll.h"

namespace service
{

class webacceptservice {
public:
	webacceptservice(std::string ip, short port, bool is_ssl, std::string _certificate_chain_file, std::string _private_key_file, std::string _tmp_dh_file, std::shared_ptr<juggle::process> process)
		: th([this, is_ssl, port]() {
			if (is_ssl) {
				asio_tls_server = Fossilizid::pool::factory::create<websocketpp::server<websocketpp::config::asio_tls> >();
				asio_tls_server->init_asio();

				asio_tls_server->set_tls_init_handler(websocketpp::lib::bind(&webacceptservice::on_tls_init, this, websocketpp::lib::placeholders::_1));

				asio_tls_server->set_access_channels(websocketpp::log::alevel::none);
				asio_tls_server->set_error_channels(websocketpp::log::elevel::none);

				asio_tls_server->set_open_handler(websocketpp::lib::bind(&webacceptservice::onAccept, this, websocketpp::lib::placeholders::_1));
				asio_tls_server->set_close_handler(websocketpp::lib::bind(&webacceptservice::onChannelDisconn, this, websocketpp::lib::placeholders::_1));
				asio_tls_server->set_message_handler(websocketpp::lib::bind(&webacceptservice::onMsg, this, websocketpp::lib::placeholders::_1, websocketpp::lib::placeholders::_2));

				asio_tls_server->listen(port);
				asio_tls_server->start_accept();

				while (1) {
					try {
						asio_tls_server->run();
					}
					catch (std::exception e) {
						spdlog::error("err:{0}", e.what());
					}
				}
			}
			else {
				asio_server = Fossilizid::pool::factory::create<websocketpp::server<websocketpp::config::asio> >();
				asio_server->init_asio();

				asio_server->set_access_channels(websocketpp::log::alevel::none);
				asio_server->set_error_channels(websocketpp::log::elevel::none);

				asio_server->set_open_handler(websocketpp::lib::bind(&webacceptservice::onAccept, this, websocketpp::lib::placeholders::_1));
				asio_server->set_close_handler(websocketpp::lib::bind(&webacceptservice::onChannelDisconn, this, websocketpp::lib::placeholders::_1));
				asio_server->set_message_handler(websocketpp::lib::bind(&webacceptservice::onMsg, this, websocketpp::lib::placeholders::_1, websocketpp::lib::placeholders::_2));

				asio_server->listen(port);
				asio_server->start_accept();

				while (1) {
					try {
						asio_server->run();
					}
					catch (std::exception e) {
						spdlog::error("err:{0}", e.what());
					}
				}
			}
		}) 
	{
		_is_ssl = is_ssl;
		_process = process;

		certificate_chain_file = _certificate_chain_file;
		private_key_file = _private_key_file;
		tmp_dh_file = _tmp_dh_file;
	}

	websocketpp::lib::shared_ptr<websocketpp::lib::asio::ssl::context> on_tls_init(websocketpp::connection_hdl hdl) {
		namespace asio = websocketpp::lib::asio;

		auto ctx = Fossilizid::pool::factory::create<asio::ssl::context>(asio::ssl::context::sslv23);
		try {
			ctx->set_options(
				asio::ssl::context::default_workarounds | 
				asio::ssl::context::no_sslv2 |
				asio::ssl::context::single_dh_use);
	
			ctx->use_certificate_chain_file(certificate_chain_file);
			ctx->use_private_key_file(private_key_file, asio::ssl::context::pem);
			ctx->use_tmp_dh_file(tmp_dh_file);
		}
		catch (std::exception& e) {
			std::cout << "Exception: " << e.what() << std::endl;
		}
		return ctx;
	}

	boost::signals2::signal<void(std::shared_ptr<juggle::Ichannel>)> sigchannelconnectexception;
	boost::signals2::signal<void(std::shared_ptr<juggle::Ichannel>)> sigchannelconnect;
	void onAccept(websocketpp::connection_hdl hdl) {
		if (_is_ssl) {
			auto ch = std::make_shared<webchannel>(asio_tls_server, hdl);
			ch->sigconnexception.connect([this](std::shared_ptr<webchannel> _ch){
				_ch->disconnect();

				if (!sigchannelconnectexception.empty()) {
					sigchannelconnectexception(std::static_pointer_cast<juggle::Ichannel>(_ch));
				}

				service::gc_put([this, _ch]() {
					_process->unreg_channel(std::static_pointer_cast<juggle::Ichannel>(_ch));
				});
			});

			std::scoped_lock<std::mutex> l(_chs_mu);
			_chs.insert(std::make_pair(hdl.lock().get(), ch));

			_process->reg_channel(std::static_pointer_cast<juggle::Ichannel>(ch));

			if (!sigchannelconnect.empty()) {
				sigchannelconnect(std::static_pointer_cast<juggle::Ichannel>(ch));
			}
		}
		else {
			auto ch = std::make_shared<webchannel>(asio_server, hdl);
			ch->sigconnexception.connect([this](std::shared_ptr<webchannel> _ch) {
				_ch->disconnect();

				if (!sigchannelconnectexception.empty()) {
					sigchannelconnectexception(std::static_pointer_cast<juggle::Ichannel>(_ch));
				}

				service::gc_put([this, _ch]() {
					_process->unreg_channel(std::static_pointer_cast<juggle::Ichannel>(_ch));
				});
			});

			std::scoped_lock<std::mutex> l(_chs_mu);
			_chs.insert(std::make_pair(hdl.lock().get(), ch));

			_process->reg_channel(std::static_pointer_cast<juggle::Ichannel>(ch));
			
			if (!sigchannelconnect.empty()) {
				sigchannelconnect(std::static_pointer_cast<juggle::Ichannel>(ch));
			}
		}
	}

	boost::signals2::signal<void(std::shared_ptr<juggle::Ichannel>)> sigchanneldisconnect;
	void onChannelDisconn(websocketpp::connection_hdl hdl) {
		std::scoped_lock<std::mutex> l(_chs_mu);
		auto ch = _chs[hdl.lock().get()];
		_chs.erase(hdl.lock().get());
		spdlog::trace("onChannelDisconn _chs.size:{0}", _chs.size());

		if (!sigchanneldisconnect.empty()) {
			sigchanneldisconnect(std::static_pointer_cast<juggle::Ichannel>(ch));
		}

		service::gc_put([this, ch]() {
			_process->unreg_channel(std::static_pointer_cast<juggle::Ichannel>(ch));
		});
	}

	void onMsg(websocketpp::connection_hdl hdl, websocketpp::server<websocketpp::config::asio>::message_ptr msg)
	{
		std::string str_msg = msg->get_payload();

		std::scoped_lock<std::mutex> l(_chs_mu);
		auto ch = _chs[hdl.lock().get()];
		ch->recv(str_msg);
	}

	void poll(){
	}

private:
	std::shared_ptr<websocketpp::server<websocketpp::config::asio_tls> > asio_tls_server;
	std::shared_ptr<websocketpp::server<websocketpp::config::asio> > asio_server;

	bool _is_ssl;

	std::shared_ptr<juggle::process> _process;

	std::string certificate_chain_file;
	std::string private_key_file;
	std::string tmp_dh_file;

	std::mutex _chs_mu;
	std::map<void*, std::shared_ptr<webchannel> > _chs;

	std::thread th;

};

}

#endif
