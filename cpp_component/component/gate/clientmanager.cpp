/*
 * qianqians
 * 2016-7-12
 * clientmanager.cpp
 */
#include "clientmanager.h"

namespace gate {

void clientproxy::send_buf(char* _data, int datasize) {
	wait_send_buf.push(std::make_pair(_data, datasize));
	_cli_mgr->mark_client_proxy_send(index);
}

}