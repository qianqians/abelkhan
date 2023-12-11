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

#include <gc.h>
#include <spdlog/spdlog.h>
#include <hiredis/hiredis.h>
#include <hircluster.h>
#include <msgpack11.hpp>
#include <fmt/format.h>

#include <string_tools.h>
#include <ringque.h>

#include "redismqchannel.h"

namespace service {

class redismqserviceException : public std::exception{
public:
	redismqserviceException(std::string err_) {
		_err = err_;
	}

public:
	std::string _err;

};

struct redismqbuff {
	std::string ch_name;
	char* buf;
	size_t len;
};

class redismqservice : public std::enable_shared_from_this<redismqservice> {
private:
	std::string main_channle_name;
	std::string _redis_url;
	std::string _password;

	std::mutex _mu_wait_listen_channel_names;
	std::vector<std::string> wait_listen_channel_names;
	std::vector<std::string> listen_channel_names;
	
	std::mutex _mu_ch_map;
	std::unordered_map<std::string, std::shared_ptr<redismqchannel> > _ch_map;

	bool _is_cluster;
	redisClusterContext* _write_cluster_ctx = nullptr;
	redisClusterContext* _recv_cluster_ctx = nullptr;
	redisContext* _write_ctx = nullptr;
	redisContext* _recv_ctx = nullptr;

	bool run_flag = true;
	std::jthread th_send;
	std::jthread th_recv;

	concurrent::ringque<redismqbuff> send_data;
	concurrent::ringque<std::pair<std::string, msgpack11::MsgPack> > recv_data;

public:
	redismqservice(bool is_cluster, std::string _listen_channle_name, std::string redis_url, std::string password = "") {
		main_channle_name = _listen_channle_name;
		listen_channel_names.push_back(main_channle_name);

		_redis_url = redis_url;
		_password = password;

		_is_cluster = is_cluster;
		if (is_cluster) {
			init_write_cluster_ctx();
			init_recv_cluster_ctx();
		}
		else {
			_write_ctx = conn_redis();
			_recv_ctx = conn_redis();
		}
	}

	void take_over_svr(std::string svr_name) {
		std::lock_guard<std::mutex> l(_mu_wait_listen_channel_names);
		wait_listen_channel_names.push_back(svr_name);
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
		auto _ch_name_size = (uint32_t)main_channle_name.size();
		auto _totle_len = 4 + _ch_name_size + (uint32_t)len;
		auto _totle_buf = (char*)GC_malloc(_totle_len);
		_totle_buf[0] = _ch_name_size & 0xff;
		_totle_buf[1] = _ch_name_size >> 8 & 0xff;
		_totle_buf[2] = _ch_name_size >> 16 & 0xff;
		_totle_buf[3] = _ch_name_size >> 24 & 0xff;
		memcpy(&_totle_buf[4], main_channle_name.c_str(), _ch_name_size);
		memcpy(&_totle_buf[4 + _ch_name_size], data, len);

		redismqbuff buf;
		buf.ch_name = channle_name;
		buf.buf = _totle_buf;
		buf.len = _totle_len;
		send_data.push(buf);
	}

	void start() {
		th_recv = std::jthread([this]() {
			if (_recv_cluster_ctx) {
				thread_poll_cluster();
			}
			else if (_recv_ctx) {
				thread_poll_single();
			}
			else {
				spdlog::error("have no init redis context!");
				throw redismqserviceException("have no init redis context!");
			}
		});
		th_send = std::jthread([this]() {
			if (_write_cluster_ctx) {
				thread_send_cluster();
			}
			else if (_write_ctx) {
				thread_send_single();
			}
			else {
				spdlog::error("have no init redis context!");
				throw redismqserviceException("have no init redis context!");
			}
		});
	}

	void close() {
		run_flag = false;
		th_recv.join();
		th_send.join();
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
	void refresh_listen_list() {
		std::lock_guard<std::mutex> l(_mu_wait_listen_channel_names);
		for (auto svr_name : wait_listen_channel_names) {
			listen_channel_names.push_back(svr_name);
		}
		wait_listen_channel_names.clear();
	}

	void init_write_cluster_ctx() {
		_write_cluster_ctx = redisClusterContextInit();
		redisClusterSetOptionAddNodes(_write_cluster_ctx, _redis_url.c_str());
		if (!_password.empty()) {
			redisClusterSetOptionPassword(_write_cluster_ctx, _password.c_str());
		}
		redisClusterSetOptionRouteUseSlots(_write_cluster_ctx);
		redisClusterConnect2(_write_cluster_ctx);
		if (!_write_cluster_ctx || _write_cluster_ctx->err) {
			spdlog::error("redisClusterConnect2 faild url:{0}!", _redis_url);
			throw redismqserviceException("redisClusterConnect2 faild!");
		}
	}

	void init_recv_cluster_ctx() {
		_recv_cluster_ctx = redisClusterContextInit();
		redisClusterSetOptionAddNodes(_recv_cluster_ctx, _redis_url.c_str());
		if (!_password.empty()) {
			redisClusterSetOptionPassword(_recv_cluster_ctx, _password.c_str());
		}
		redisClusterSetOptionRouteUseSlots(_recv_cluster_ctx);
		redisClusterConnect2(_recv_cluster_ctx);
		if (!_recv_cluster_ctx || _recv_cluster_ctx->err) {
			spdlog::error("redisClusterConnect2 faild url:{0}!", _redis_url);
			throw redismqserviceException("redisClusterConnect2 faild!");
		}
	}

	void redis_cluster_send_data(std::string ch_name, char* data, size_t len) {
		auto _reply = (redisReply*)redisClusterCommand(_write_cluster_ctx, "LPUSH %s %b", ch_name.c_str(), data, len);
		if (_reply) {
			if (_reply->type == REDIS_REPLY_PUSH || _reply->type == REDIS_REPLY_INTEGER) {
			}
			else {
				spdlog::error(fmt::format("redis exception operate type:{0}, str:{1}", _reply->type, _reply->str));
			}
			freeReplyObject(_reply);
		}
		else {
			spdlog::error("redis exception operate type:{0}, str:{1}", _write_cluster_ctx->err, _write_cluster_ctx->errstr);
			init_write_cluster_ctx();
		}
	}

	void redis_cluster_recv_data() {
		std::string cmd = "BRPOP";
		for (auto channel_name : listen_channel_names) {
			cmd += " " + channel_name;
		}
		cmd += " 1";
		while (true) {
			auto _reply = (redisReply*)redisClusterCommand(_recv_cluster_ctx, cmd.c_str());
			if (_reply) {
				if (_reply->type == REDIS_REPLY_ARRAY) {
					auto _buf = _reply->element[1]->str;
					auto _ch_name_size = (uint32_t)_buf[0] | ((uint32_t)_buf[1] << 8) | ((uint32_t)_buf[2] << 16) | ((uint32_t)_buf[3] << 24);
					auto _ch_name = std::string(&_buf[4], _ch_name_size);
					auto _header_len = 4 + _ch_name_size;

					auto tmp_buff = (unsigned char*)&_buf[_header_len];
					uint32_t len = (uint32_t)tmp_buff[0] | ((uint32_t)tmp_buff[1] << 8) | ((uint32_t)tmp_buff[2] << 16) | ((uint32_t)tmp_buff[3] << 24);
					std::string err;
					auto obj = msgpack11::MsgPack::parse((const char*)tmp_buff, len, err);
					recv_data.push(std::make_pair(_ch_name, obj));

				}
				else if (_reply->type != REDIS_REPLY_NIL) {
					spdlog::error(fmt::format("redis exception operate type:{0}, str:{1}", _reply->type, _reply->str));
				}
				else {
					freeReplyObject(_reply);
					break;
				}
				freeReplyObject(_reply);
			}
			else {
				spdlog::error("redis exception operate type:{0}, str:{1}", _recv_cluster_ctx->err, _recv_cluster_ctx->errstr);
				init_recv_cluster_ctx();
			}
		}
	}

	void thread_poll_cluster() {
		int sleep_time = 1;
		while (run_flag) {
			refresh_listen_list();
			redis_cluster_recv_data();
		}
	}

	void thread_send_cluster() {
		int sleep_time = 1;
		while (run_flag) {
			bool is_idle = true;

			redismqbuff buf;
			while (send_data.pop(buf)) {
				redis_cluster_send_data(buf.ch_name, buf.buf, buf.len);

				is_idle = false;
				sleep_time = 1;
			}

			if (is_idle) {
				if (sleep_time < 32) {
					sleep_time *= 2;
				}
				std::this_thread::sleep_for(std::chrono::milliseconds(sleep_time));
			}
		}
	}
	
	redisContext* conn_redis() {
		auto ip_port = concurrent::split(_redis_url, ":");
		auto _ctx = redisConnect(ip_port[0].c_str(), std::stoi(ip_port[1]));
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
		return _ctx;
	}

	bool re_conn_redis(redisContext* _ctx) {
		if (redisReconnect(_ctx) == REDIS_OK) {
			if (!_password.empty()) {
				auto reply = (redisReply*)redisCommand(_ctx, "AUTH % s", _password.c_str());
				if (reply->type == REDIS_REPLY_ERROR) {
					spdlog::error("redisContext auth faild:{0}!", _password);
					throw redismqserviceException("redisContext auth faild!");
				}
			}
			return true;
		}
		return false;
	}

	void redis_mq_send_data(std::string ch_name, char* data, size_t len) {
		while (true) {
			auto _reply = (redisReply*)redisCommand(_write_ctx, "LPUSH %s %b", ch_name.c_str(), data, len);
			if (_reply) {
				if (_reply->type == REDIS_REPLY_PUSH || _reply->type == REDIS_REPLY_INTEGER) {
				}
				else {
					spdlog::error(fmt::format("redis exception operate type:{0}, str:{1}", _reply->type, _reply->str));
				}
				freeReplyObject(_reply);
				break;
			}
			else {
				int wait_time = 8;
				while (!re_conn_redis(_write_ctx)) {
					try {
						std::this_thread::sleep_for(std::chrono::milliseconds(wait_time));
					}
					catch (redismqserviceException ex) {
						std::this_thread::sleep_for(std::chrono::milliseconds(wait_time));
					}
					wait_time *= 2;
				}
			}
		}
	}

	void redis_mq_recv_data() {
		std::string cmd = "BRPOP";
		for (auto channel_name : listen_channel_names) {
			cmd += " " + channel_name;
		}
		cmd += " 1";
		while (true) {
			auto _reply = (redisReply*)redisCommand(_recv_ctx, cmd.c_str());
			if (_reply) {
				if (_reply->type == REDIS_REPLY_ARRAY) {
					auto _buf = _reply->element[1]->str;
					auto _ch_name_size = (uint32_t)_buf[0] | ((uint32_t)_buf[1] << 8) | ((uint32_t)_buf[2] << 16) | ((uint32_t)_buf[3] << 24);
					auto _ch_name = std::string(&_buf[4], _ch_name_size);
					auto _header_len = 4 + _ch_name_size;

					auto tmp_buff = (unsigned char*)&_buf[_header_len];
					uint32_t len = (uint32_t)tmp_buff[0] | ((uint32_t)tmp_buff[1] << 8) | ((uint32_t)tmp_buff[2] << 16) | ((uint32_t)tmp_buff[3] << 24);
					std::string err;
					auto obj = msgpack11::MsgPack::parse((const char*)&tmp_buff[4], len, err);
					recv_data.push(std::make_pair(_ch_name, obj));
				}
				else if (_reply->type != REDIS_REPLY_NIL) {
					spdlog::error(fmt::format("redis exception operate type:{0}, str:{1}", _reply->type, _reply->str));
				}
				else {
					freeReplyObject(_reply);
					break;
				}
				freeReplyObject(_reply);
			}
			else {
				int wait_time = 8;
				while (!re_conn_redis(_recv_ctx)) {
					try {
						std::this_thread::sleep_for(std::chrono::milliseconds(wait_time));
					}
					catch (redismqserviceException ex) {
						std::this_thread::sleep_for(std::chrono::milliseconds(wait_time));
					}
					wait_time *= 2;
				}
			}
		}
	}

	void thread_poll_single() {
		while (run_flag) {
			refresh_listen_list();
			redis_mq_recv_data();
		}
	}

	void thread_send_single() {
		int sleep_time = 1;
		while (run_flag) {
			bool is_idle = true;

			redismqbuff buf;
			while (send_data.pop(buf)) {
				redis_mq_send_data(buf.ch_name, buf.buf, buf.len);

				is_idle = false;
				sleep_time = 1;
			}

			if (is_idle) {
				if (sleep_time < 32) {
					sleep_time *= 2;
				}
				std::this_thread::sleep_for(std::chrono::milliseconds(sleep_time));
			}
		}
	}
};

}

#endif //_clientmanager_h
