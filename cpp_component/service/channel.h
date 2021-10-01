
#ifndef _channel_h
#define _channel_h

#include <list>
#include <iostream>
#include <functional>

#include <boost/asio.hpp>
#include <boost/thread.hpp>
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
	channel(std::shared_ptr<boost::asio::ip::tcp::socket> _s)
	{
		s = _s;
		is_close = false;
	}

	void start()
	{
		ch_encrypt_decrypt_ondata = std::make_shared<channel_encrypt_decrypt_ondata>(shared_from_this());

		memset(read_buff, 0, 16 * 1024);
		s->async_read_some(boost::asio::buffer(read_buff, 16 * 1024), std::bind(&channel::onRecv, shared_from_this(), std::placeholders::_1, std::placeholders::_2));
	}

	void set_xor_key(uint64_t _xor_key) {
		ch_encrypt_decrypt_ondata->set_xor_key(_xor_key);
	}

	virtual ~channel(){
	}

	concurrent::signals<void(std::shared_ptr<channel>)> sigondisconn;
	concurrent::signals<void(std::shared_ptr<channel>)> sigdisconn;

	uint8_t xor_key[8] = {0};

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

		ch->ch_encrypt_decrypt_ondata->recv(ch->read_buff, bytes_transferred);

		memset(ch->read_buff, 0, 16 * 1024);
		ch->s->async_read_some(boost::asio::buffer(ch->read_buff, 16 * 1024), std::bind(&channel::onRecv, ch, std::placeholders::_1, std::placeholders::_2));
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

	void send(char* data, size_t len)
	{
		if (is_close) {
			return;
		}

		if (!s->is_open()){
			return;
		}

		try {
			if (ch_encrypt_decrypt_ondata->is_compress_and_encrypt)
			{
				ch_encrypt_decrypt_ondata->xor_key_encrypt_decrypt(&data[4], len - 4);
			}
			
			size_t offset = 0;
			while (offset < len) {
				try {
					offset += s->send(boost::asio::buffer(&data[offset], len - offset));
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
		}
		catch (std::exception e) {
			spdlog::error("channel push exception error:{0}", e.what());
			is_close = true;
		}
	}

private:
	std::shared_ptr<boost::asio::ip::tcp::socket> s;

	std::shared_ptr<channel_encrypt_decrypt_ondata> ch_encrypt_decrypt_ondata;
	char read_buff[8 * 1024];

	bool is_close;

};

}

#endif
