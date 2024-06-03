/*
 * dbproxy_server.cpp
 * qianqians
 * 2024/5/30
 */
#include "dbproxy_server.h"

#include "hubmanager.h"
#include "centerproxy.h"
#include "closehandle.h"
#include "mongodbproxy.h"
#include "center_msg_handle.h"
#include "hub_msg_handle.h"

namespace dbproxy
{

std::string dbproxy::name;
bool dbproxy::is_busy;
uint32_t dbproxy::tick;
std::shared_ptr<close_handle> dbproxy::_closeHandle;
std::shared_ptr<hubmanager> dbproxy::_hubmanager;
std::shared_ptr<service::timerservice> dbproxy::_timer;
std::shared_ptr<mongodbproxy> dbproxy::_mongodbproxy;
std::shared_ptr<service::redismqservice> dbproxy::_redis_mq_service;

}

