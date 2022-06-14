/*
 * redismqservice.h
 *
 *  Created on: 2022-6-13
 *      Author: qianqians
 */
#ifndef _redismq_service_h
#define _redismq_service_h

#include <thread>
#include <string>
#include <memory>
#include <list>
#include <unordered_map>

#include <hircluster.h>

#include <spdlog/spdlog.h>

#include <ringque.h>

#include "redismqchannel.h"

namespace service {

class redismqserviceException : public std::exception{
public:
	redismqserviceException(std::string err_) : std::exception(err_.c_str()) {
		_err = err_;
	}

public:
	std::string _err;

};

struct redismqbuff {
	std::string ch_name;
	char* buf;
	uint32_t len;
};

class redismqservice : public std::enable_shared_from_this<redismqservice> {
private:
	bool run_flag = true;
	std::jthread th;
	std::string listen_channle_name;
	
	std::unordered_map<std::string, std::shared_ptr<redismqchannel> > _ch_map;

	concurrent::ringque<redismqbuff> send_data;
	concurrent::ringque<redismqbuff> recv_data;

	redisClusterContext* ctx;

public:
	redismqservice(std::string _listen_channle_name, std::string redis_url, std::string password = "") {
		listen_channle_name = _listen_channle_name;

		ctx = redisClusterContextInit();
		redisClusterSetOptionAddNodes(ctx, redis_url.c_str());
		if (!password.empty()) {
			redisClusterSetOptionPassword(ctx, password.c_str());
		}
		redisClusterSetOptionRouteUseSlots(ctx);
		redisClusterConnect2(ctx);
		if (!ctx || ctx->err) {
			spdlog::error("redisClusterConnect2 faild url:{0}!", redis_url);
			throw redismqserviceException("redisClusterConnect2 faild!");
		}
	}

	void start() {
		th = std::jthread([this]() {
			thread_poll();
		});
	}

	std::shared_ptr<abelkhan::Ichannel> connect(std::string ch_name)
	{
		auto ch = std::make_shared<redismqchannel>(ch_name, shared_from_this());
		_ch_map.insert(std::make_pair(ch_name, ch));
		return ch;
	}

	void sendmsg(std::string channle_name, const char* data, size_t len) {
		auto _ch_name_size = (uint32_t)listen_channle_name.size();
		auto _totle_len = 4 + _ch_name_size + (uint32_t)len;
		auto _totle_buf = (char*)malloc(_totle_len);
		_totle_buf[0] = _ch_name_size & 0xff;
		_totle_buf[1] = _ch_name_size >> 8 & 0xff;
		_totle_buf[2] = _ch_name_size >> 16 & 0xff;
		_totle_buf[3] = _ch_name_size >> 24 & 0xff;
		memcpy(&_totle_buf[4], listen_channle_name.c_str(), _ch_name_size);
		memcpy(&_totle_buf[4 + _ch_name_size], data, len);

		redismqbuff buf;
		buf.ch_name = channle_name;
		buf.buf = _totle_buf;
		buf.len = _totle_len;
		send_data.push(buf);
	}

	void poll() {
		redismqbuff data;
		while (recv_data.pop(data)) {
			auto it = _ch_map.find(data.ch_name);
			if (it != _ch_map.end()) {
				it->second->recv(const_cast<char*>(data.buf), data.len);
			}
			else {
				auto ch = std::make_shared<redismqchannel>(data.ch_name, shared_from_this());
				ch->recv(const_cast<char*>(data.buf), data.len);
				_ch_map.insert(std::make_pair(data.ch_name, ch));
			}
			free(static_cast<void*>(data.buf));
		}
	}

private:
	void thread_poll() {
		while (run_flag) {
			bool is_idle = true;
			redismqbuff data;
			if (send_data.pop(data)) {
				while (true) {
					auto _reply = (redisReply*)redisClusterCommand(ctx, "LPUSH %s %b", data.ch_name.c_str(), data.buf, data.len);
					if (_reply->type == REDIS_REPLY_PUSH) {
						freeReplyObject(_reply);
						free(static_cast<void*>(data.buf));
						break;
					}
					else {
						spdlog::error(std::format("redis exception operate type:{0}, str:{1}", _reply->type, _reply->str));
						freeReplyObject(_reply);
					}
				}
				is_idle = false;
			}

			while (true) {
				auto _reply = (redisReply*)redisClusterCommand(ctx, "RPOP %s", listen_channle_name.c_str());

				if (_reply->type == REDIS_REPLY_STRING) {
					auto _buf = _reply->str;
					auto _ch_name_size = (uint32_t)_buf[0] | ((uint32_t)_buf[1] << 8) | ((uint32_t)_buf[2] << 16) | ((uint32_t)_buf[3] << 24);
					auto _ch_name = std::string(&_buf[4], _ch_name_size);
					auto _header_len = 4 + _ch_name_size;
					auto _msg_len = (uint32_t)_reply->len - _header_len;
					auto _msg_buf = (char*)malloc(_msg_len);
					memcpy(_msg_buf, &_buf[_header_len], _msg_len);

					redismqbuff buf;
					buf.ch_name = _ch_name;
					buf.buf = _msg_buf;
					buf.len = _msg_len;
					recv_data.push(buf);

					is_idle = false;

					freeReplyObject(_reply);
					break;
				}
				else if(_reply->type != REDIS_REPLY_NIL) {
					spdlog::error(std::format("redis exception operate type:{0}, str:{1}", _reply->type, _reply->str));
					freeReplyObject(_reply);
				}
			}

			if (is_idle) {
				std::this_thread::yield();
			}
		}
	}


};

}

#endif //_clientmanager_h
