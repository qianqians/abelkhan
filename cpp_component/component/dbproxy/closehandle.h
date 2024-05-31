/*
 * closehandle.h
 * qianqians
 * 2024/5/30
 */
#ifndef _closehandle_h_
#define _closehandle_h_

#include "dbproxy_server.h"

#include "hubmanager.h"

namespace dbproxy
{

class close_handle
{
public:
	close_handle()
	{
		_is_close = false;
		_is_closing = false;
	}

	bool is_close() {
		return dbproxy::_hubmanager->all_hub_closed() && _is_close;
	}

public:
	bool _is_close;
	bool _is_closing;
};

}

#endif // !_closehandle_h_
