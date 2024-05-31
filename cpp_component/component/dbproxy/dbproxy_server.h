/*
 * dbproxy_server.h
 * qianqians
 * 2024/5/30
 */
#ifndef _dbproxy_server_h_
#define _dbproxy_server_h_

#include <string>

#include <timerservice.h>
#include <redismqservice.h>
#include <config.h>

namespace dbproxy
{
class close_handle;
class hubmanager;
class mongodbproxy;
class centerproxy;
class dbproxy
{
public:
    static std::string name;
    static bool is_busy;
    static uint32_t tick;
    static std::shared_ptr<close_handle> _closeHandle;
    static std::shared_ptr<hubmanager> _hubmanager;
    static std::shared_ptr<service::timerservice> _timer;
    static std::shared_ptr<mongodbproxy> _mongodbproxy;
    static std::shared_ptr<service::redismqservice> _redis_mq_service;

    std::shared_ptr<config::config> _root_config;
    std::shared_ptr<config::config> _center_config;
    std::shared_ptr<config::config> _config;

private:
    std::vector<abelkhan::Ichannel> add_chs;
    std::vector<abelkhan::Ichannel> remove_chs;

    uint32_t reconn_count = 0;

    std::shared_ptr<centerproxy> _centerproxy;
};

}

#endif // !_dbproxy_server_h_