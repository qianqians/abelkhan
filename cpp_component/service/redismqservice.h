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

#include <spdlog/spdlog.h>
#include <hiredis/hiredis.h>
#include <hircluster.h>
#include <msgpack11.hpp>

#include <string_tools.h>
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
	
	std::mutex _mu_ch_map;
	std::unordered_map<std::string, std::shared_ptr<redismqchannel> > _ch_map;

	concurrent::ringque<redismqbuff> send_data;
	concurrent::ringque<std::pair<std::string, msgpack11::MsgPack> > recv_data;

	redisClusterContext* _cluster_ctx = nullptr;
	redisContext* _ctx = nullptr;

	std::string _redis_url;
	std::string _password;

public:
	redismqservice(bool is_cluster, std::string _listen_channle_name, std::string redis_url, std::string password = "") {
		listen_channle_name = _listen_channle_name;
		_redis_url = redis_url;
		_password = password;

		if (is_cluster) {
			_cluster_ctx = redisClusterContextInit();
			redisClusterSetOptionAddNodes(_cluster_ctx, redis_url.c_str());
			if (!password.empty()) {
				redisClusterSetOptionPassword(_cluster_ctx, password.c_str());
			}
			redisClusterSetOptionRouteUseSlots(_cluster_ctx);
			redisClusterConnect2(_cluster_ctx);
			if (!_cluster_ctx || _cluster_ctx->err) {
				spdlog::error("redisClusterConnect2 faild url:{0}!", redis_url);
				throw redismqserviceException("redisClusterConnect2 faild!");
			}
		}
		else {
			re_conn_redis();
		}
	}

	void start() {
		th = std::jthread([this]() {
			if (_cluster_ctx) {
				thread_poll_cluster();
			}
			else if (_ctx) {
				thread_poll_single();
			}
			else {
				spdlog::error("have no init redis context!");
				throw redismqserviceException("have no init redis context!");
			}
		});
	}

	void close() {
		run_flag = false;
		th.join();
	}

	std::shared_ptr<abelkhan::Ichannel> connect(std::string ch_name)
	{
		std::lock_guard<std::mutex> l(_mu_ch_map);
		auto it = _ch_map.find(ch_name);
		if (it != _ch_map.end()) {
			return it->second;
		}

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
		std::pair<std::string, msgpack11::MsgPack> data;
		while (recv_data.pop(data)) {
			auto it = _ch_map.find(data.first);
			if (it != _ch_map.end()) {
				it->second->recv(data.second);
			}
			else {
				auto ch = std::make_shared<redismqchannel>(data.first, shared_from_this());
				ch->recv(data.second);
				_ch_map.insert(std::make_pair(data.first, ch));
			}
		}
	}

private:
	void re_conn_redis() {
		auto ip_port = concurrent::split(_redis_url, ":");
		_ctx = redisConnect(ip_port[0].c_str(), std::stoi(ip_port[1]));
		if (!_ctx) {
			spdlog::error("redisConnect faild url:{0}!", _redis_url);
			throw redismqserviceException("redisConnect faild!");
		}

		if (!_password.empty()) {
			auto reply = (redisReply*)redisCommand(_ctx, "AUTH % s", _password.c_str());
			if (reply->type == REDIS_REPLY_ERROR) {
				spdlog::error("redisContext auth faild:{0}!", _password);
				throw redismqserviceException("redisContext auth faild!");
			}
		}
	}

	void thread_poll_cluster() {
		while (run_flag) {
			bool is_idle = true;
			redismqbuff data;
			if (send_data.pop(data)) {
				auto _reply = (redisReply*)redisClusterCommand(_cluster_ctx, "LPUSH %s %b", data.ch_name.c_str(), data.buf, (size_t)data.len);
				if (_reply->type == REDIS_REPLY_PUSH) {
				}
				else {
					spdlog::error(std::format("redis exception operate type:{0}, str:{1}", _reply->type, _reply->str));
				}
				freeReplyObject(_reply);
				free(static_cast<void*>(data.buf));
				is_idle = false;
			}

			{
				auto _reply = (redisReply*)redisClusterCommand(_cluster_ctx, "RPOP %s", listen_channle_name.c_str());

				if (_reply->type == REDIS_REPLY_STRING) {
					auto _buf = _reply->str;
					auto _ch_name_size = (uint32_t)_buf[0] | ((uint32_t)_buf[1] << 8) | ((uint32_t)_buf[2] << 16) | ((uint32_t)_buf[3] << 24);
					auto _ch_name = std::string(&_buf[4], _ch_name_size);
					auto _header_len = 4 + _ch_name_size;
					auto _msg_len = (uint32_t)_reply->len - _header_len;
					
					auto tmp_buff = (unsigned char*)_buf[_header_len];
					uint32_t len = (uint32_t)tmp_buff[0] | ((uint32_t)tmp_buff[1] << 8) | ((uint32_t)tmp_buff[2] << 16) | ((uint32_t)tmp_buff[3] << 24);
					std::string err;
					auto obj = msgpack11::MsgPack::parse((const char*)tmp_buff, len, err);
					recv_data.push(std::make_pair(_ch_name, obj));

					is_idle = false;
				}
				else if(_reply->type != REDIS_REPLY_NIL) {
					spdlog::error(std::format("redis exception operate type:{0}, str:{1}", _reply->type, _reply->str));
				}
				freeReplyObject(_reply);
			}

			if (is_idle) {
				std::this_thread::yield();
			}
		}
	}

	void redis_mq_send_data(redismqbuff data) {
		while (true) {
			auto _reply = (redisReply*)redisCommand(_ctx, "LPUSH %s %b", data.ch_name.c_str(), data.buf, (size_t)data.len);
			if (_reply) {
				if (_reply->type == REDIS_REPLY_PUSH || _reply->type == REDIS_REPLY_INTEGER) {
				}
				else {
					spdlog::error(std::format("redis exception operate type:{0}, str:{1}", _reply->type, _reply->str));
				}
				freeReplyObject(_reply);
				break;
			}
			else {
				try {
					redisFree(_ctx);
					re_conn_redis();
				}
				catch (redismqserviceException ex) {
					std::this_thread::yield();
				}
			}
		}
		free(static_cast<void*>(data.buf));
	}

	bool redis_mq_recv_data() {
		auto ret = false;

		auto _reply = (redisReply*)redisCommand(_ctx, "RPOP %s", listen_channle_name.c_str());
		if (_reply) {
			if (_reply->type == REDIS_REPLY_STRING) {
				auto _buf = _reply->str;
				auto _ch_name_size = (uint32_t)_buf[0] | ((uint32_t)_buf[1] << 8) | ((uint32_t)_buf[2] << 16) | ((uint32_t)_buf[3] << 24);
				auto _ch_name = std::string(&_buf[4], _ch_name_size);
				auto _header_len = 4 + _ch_name_size;
				auto _msg_len = (uint32_t)_reply->len - _header_len;

				auto tmp_buff = (unsigned char*)&_buf[_header_len];
				uint32_t len = (uint32_t)tmp_buff[0] | ((uint32_t)tmp_buff[1] << 8) | ((uint32_t)tmp_buff[2] << 16) | ((uint32_t)tmp_buff[3] << 24);
				std::string err;
				auto obj = msgpack11::MsgPack::parse((const char*)&tmp_buff[4], len, err);
				recv_data.push(std::make_pair(_ch_name, obj));

				ret = true;
			}
			else if (_reply->type != REDIS_REPLY_NIL) {
				spdlog::error(std::format("redis exception operate type:{0}, str:{1}", _reply->type, _reply->str));
			}
			freeReplyObject(_reply);
		}
		else {
			try {
				redisFree(_ctx);
				re_conn_redis();
			}
			catch (redismqserviceException ex) {
				std::this_thread::yield();
			}
		}

		return ret;
	}

	void thread_poll_single() {
		while (run_flag) {
			bool is_idle = true;
			redismqbuff data;
			if (send_data.pop(data)) {
				redis_mq_send_data(data);
				is_idle = false;
			}

			if (redis_mq_recv_data()){
				is_idle = false;	
			}

			if (is_idle) {
				std::this_thread::yield();
			}
		}
	}

};

}

#endif //_clientmanager_h
