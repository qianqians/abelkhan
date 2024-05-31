/*
 * centerproxy.h
 * qianqians
 * 2024/5/30
 */
#ifndef  _centerproxy_h_
#define _centerproxy_h_

#include <spdlog/spdlog.h>

#include <abelkhan.h>
#include <modulemng_handle.h>

#include <msec_time.h>

#include <center.h>

#include "dbproxy_server.h"

namespace dbproxy
{

class centerproxy
{
public:
	bool is_reg_sucess;
	int64_t timetmp = msec_time();
	std::shared_ptr<abelkhan::Ichannel> _ch;

private:
	abelkhan::center_caller _center_caller;

public:
	centerproxy(std::shared_ptr<abelkhan::Ichannel> ch);

	void reg_dbproxy(std::function<void()> callback);

	void reconn_reg_dbproxy(std::function<void(bool)> callback);

	void heartbeath();

	void closed();
};

}

#endif // !_centerproxy_h_