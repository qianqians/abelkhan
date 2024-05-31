/*
 * centerproxy.cpp
 * qianqians
 * 2024/5/30
 */
#include "centerproxy.h"

namespace dbproxy
{
centerproxy::centerproxy(std::shared_ptr<abelkhan::Ichannel> ch) : _center_caller(ch, service::_modulemng)
{
	is_reg_sucess = false;
	_ch = ch;
}

void centerproxy::reg_dbproxy(std::function<void()> callback)
{
	spdlog::trace("begin connect center server");

	_center_caller.reg_server_mq("dbproxy", "dbproxy", dbproxy::name)->callBack([callback]() {
		callback();
	spdlog::trace("connect center server sucessed");
	}, []() {
		spdlog::trace("connect center server faild");
	})->timeout(5000, []() {
		spdlog::trace("connect center server timeout");
	});
}

void centerproxy::reconn_reg_dbproxy(std::function<void(bool)> callback)
{
	spdlog::trace("begin connect center server");

	_center_caller.reconn_reg_server_mq("dbproxy", "dbproxy", dbproxy::name)->callBack([callback]() {
		spdlog::trace("reconnect center server sucessed");
		callback(true);
	}, [callback]() {
		spdlog::error("reconnect center server faild");
		callback(false);
	})->timeout(5000, [callback]() {
		spdlog::error("reconnect center server timeout");
		callback(false);
	});
}

void centerproxy::heartbeath()
{
	_center_caller.heartbeat(dbproxy::tick)->callBack([this]() {
		spdlog::trace("heartbeat center server sucessed");
		timetmp = msec_time();
	}, []() {
		spdlog::error("heartbeat center server faild");
	})->timeout(5000, []() {
		spdlog::error("heartbeat center server timeout");
	});
	spdlog::trace("begin heartbeath center server tick:{0}!", msec_time());
}

void centerproxy::closed()
{
	_center_caller.closed();

}
}
