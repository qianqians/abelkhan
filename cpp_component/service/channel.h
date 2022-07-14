
#ifndef _channel_h
#define _channel_h

#include <list>
#include <iostream>
#include <functional>
#include <thread>
#include <mutex>

#include <asio.hpp>
#include <msgpack11.hpp>

#include <signals.h>
#include <abelkhan.h>

#include <log.h>

#include "modulemng_handle.h"
#include "channel_encrypt_decrypt_ondata.h"

namespace service
{

class channel : public abelkhan::Ichannel, public std::enable_shared_from_this<channel> {
public:
	channel(std::shared_ptr<asio::ip::tcp::socket> _s)
	{
		s = _s;
		is_close = false;
	}

	void start()
	{
		ch_encrypt_decrypt_ondata = std::make_shared<channel_encrypt_decrypt_ondata>(shared_from_this());
		s->async_read_some(asio::buffer(read_buff, 2 * 1024), std::bind(&channel::onRecv, shared_from_this(), std::placeholders::_1, std::placeholders::_2));
	}

	void set_xor_key_crypt() {
		ch_encrypt_decrypt_ondata->set_xor_key_crypt();
	}

	bool is_xor_key_crypt() {
		return ch_encrypt_decrypt_ondata->is_compress_and_encrypt;
	}

	void normal_crypt(char* data, size_t len) {
		channel_encrypt_decrypt_ondata::xor_key_encrypt_decrypt(data, len);
	}

	virtual ~channel(){
	}

	concurrent::signals<void(std::shared_ptr<channel>)> sigondisconn;
	concurrent::signals<void(std::shared_ptr<channel>)> sigdisconn;

private:
	static void onRecv(std::shared_ptr<channel> ch, const asio::error_code& error, std::size_t bytes_transferred){
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

		ch->ch_encrypt_decrypt_ondata->recv(ch->read_buff, bytes_transferred);
		ch->s->async_read_some(asio::buffer(ch->read_buff, 2 * 1024), std::bind(&channel::onRecv, ch, std::placeholders::_1, std::placeholders::_2));
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

	void send(const char* data, size_t len)
	{
		if (is_close) {
			return;
		}

		if (!s->is_open()){
			return;
		}

		try {
			std::lock_guard<std::mutex> lock(_mutex);
			size_t offset = 0;
			while (offset < len) {
				try {
					offset += s->send(asio::buffer(&data[offset], len - offset));
				}
				catch (asio::system_error e) {
					if (e.code() == asio::error::would_block) {
						std::this_thread::sleep_for(std::chrono::milliseconds(1));
						continue;
					}
					else {
						spdlog::error("channel push error:{0}", e.what());
						break;
					}
				}
			}
		}
		catch (std::exception e) {
			spdlog::error("channel push exception error:{0}", e.what());
		}
	}

private:
	std::shared_ptr<asio::ip::tcp::socket> s;
	std::mutex _mutex;

	std::shared_ptr<channel_encrypt_decrypt_ondata> ch_encrypt_decrypt_ondata;
	char read_buff[2 * 1024];

	bool is_close;

};

}

#endif
