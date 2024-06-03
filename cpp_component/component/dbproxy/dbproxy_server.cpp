/*
 * dbproxy_server.cpp
 * qianqians
 * 2024/5/30
 */
#include <gc.h>
#include <spdlog/spdlog.h>
#include <fmt/format.h>
#include <crossguid/guid.hpp>

#include <config.h>

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

dbproxy::dbproxy(std::string cfg_file, std::string cfg_name)
{
    GC_init();

    _root_config = std::make_shared<config::config>(cfg_file);
    _center_config = _root_config->get_value_dict("center");
    _config = _root_config->get_value_dict(cfg_name);

    reconn_count = 0;
    tick = 0;

    is_busy = false;

    name = fmt::format("{0}_{1}", cfg_name, xg::newGuid().str());

    auto file_path = _config->get_value_string("log_dir") + _config->get_value_string("log_file");
    auto log_level = _config->get_value_string("log_level");
    if (log_level == "trace") {
        _log::InitLog(file_path, spdlog::level::level_enum::trace);
    }
    else if (log_level == "debug") {
        _log::InitLog(file_path, spdlog::level::level_enum::debug);
    }
    else if (log_level == "info") {
        _log::InitLog(file_path, spdlog::level::level_enum::info);
    }
    else if (log_level == "warn") {
        _log::InitLog(file_path, spdlog::level::level_enum::warn);
    }
    else if (log_level == "error") {
        _log::InitLog(file_path, spdlog::level::level_enum::err);
    }

    if (_config->has_key("db_url"))
    {
        _mongodbproxy = std::make_shared<mongodbproxy>(_config->get_value_string("db_url"));
    }

    if (_config->has_key("index"))
    {
        auto _index_config = _config->get_value_list("index");
        for (int i = 0; i < _index_config->get_list_size(); i++)
        {
            auto index = _index_config->get_list_dict(i);
            auto db = index->get_value_string("db");
            auto collection = index->get_value_string("collection");
            auto key = index->get_value_string("key");
            auto is_unique = index->get_value_bool("is_unique");
            _mongodbproxy->create_index(db, collection, key, is_unique);
        }
    }
    if (_config->has_key("guid"))
    {
        auto _guid_config = _config->get_value_list("guid");
        for (int i = 0; i < _guid_config->get_list_size(); i++)
        {
            auto _guid_cfg = _guid_config->get_list_dict(i);
            auto _db = _guid_cfg->get_value_string("db");
            auto _collection = _guid_cfg->get_value_string("collection");
            auto _guid = _guid_cfg->get_value_int("guid");
            _mongodbproxy->check_int_guid(_db, _collection, _guid);
        }
    }

    _timer = std::make_shared<service::timerservice>();
    _timer->poll();

    _closeHandle = std::make_shared<close_handle>();
    _hubmanager = std::make_shared<hubmanager>();

    auto redismq_url = _root_config->get_value_string("redis_for_mq");
    auto redismq_is_cluster = _root_config->get_value_bool("redismq_is_cluster");
    if (_root_config->has_key("redis_for_mq_pwd")) {
        auto password = _root_config->get_value_string("redis_for_mq_pwd");
        _redis_mq_service = std::make_shared<service::redismqservice>(redismq_is_cluster, name, redismq_url, password);
    }
    else {
        _redis_mq_service = std::make_shared<service::redismqservice>(redismq_is_cluster, name, redismq_url);
    }
    _redis_mq_service->start();

    auto _center_ch = _redis_mq_service->connect(_center_config->get_value_string("name"));
    std::lock_guard<std::mutex> l(_add_chs_mu);
    add_chs.insert(_center_ch);

    _centerproxy = std::make_shared<centerproxy>(_center_ch);
    _centerproxy->reg_dbproxy([this]() {
        heartbeath_center(_timer->Tick);
    });
}

void dbproxy::reconnect_center()
{
    if (reconn_count > 5)
    {
        onCenterCrash.emit();
    }
    reconn_count++;

    std::lock_guard<std::mutex> l(_remove_chs_mu);
    {
        remove_chs.push_back(_centerproxy->_ch);
    }

    auto _center_ch = _redis_mq_service->connect(_center_config->get_value_string("name"));
    std::lock_guard<std::mutex> l1(_add_chs_mu);
    {
        add_chs.insert(_center_ch);
    }
    _centerproxy = std::make_shared<centerproxy>(_center_ch);
    _centerproxy->reconn_reg_dbproxy([this](bool _success) {
        if (_success) {
            reconn_count = 0;
        }
    });
}

void dbproxy::heartbeath_center(int64_t tick)
{
    do
    {
        if ((_timer->Tick - _centerproxy->timetmp) > 9000)
        {
            reconnect_center();
            break;
        }

        _centerproxy->heartbeath();

    } while (false);

    _timer->addticktimer(3000, std::bind(&dbproxy::heartbeath_center, this, std::placeholders::_1));
}

int64_t dbproxy::poll()
{
    auto begin = msec_time();

    try
    {
        abelkhan::TinyTimer::poll();
        _timer->poll();

        service::_modulemng->process_event();

        _log::file_logger->flush();

        if (remove_chs.size() > 0)
        {
            std::lock_guard<std::mutex> l(_remove_chs_mu);
            {
                for(auto ch : remove_chs)
                {
                    std::lock_guard<std::mutex> l1(_add_chs_mu);
                    {
                        add_chs.erase(ch);
                    }
                }
                remove_chs.clear();
            }
        }
    }
    catch (abelkhan::Exception e)
    {
        spdlog::error(e.what());
    }
    catch (std::exception e)
    {
        spdlog::error(e.what());
    }

    tick = static_cast<uint32_t>(msec_time() - begin);
    if (tick > 100)
    {
        is_busy = true;
    }
    else
    {
        is_busy = false;
    }

    return tick;
}

void dbproxy::_run()
{
    hub_msg_handle _hub_msg_handle(_hubmanager);
    center_msg_handle _center_msg_handle(_closeHandle, _centerproxy, _hubmanager);

    while (!_closeHandle->is_close())
    {
        auto tick = poll();

        if (tick < 33)
        {
            std::this_thread::sleep_for(std::chrono::milliseconds(33 - tick));
        }
    }
    spdlog::info("server closed, dbproxy server:{0}", name);

    _redis_mq_service->close();
    _log::file_logger->flush();

}

void dbproxy::run()
{
    if (!_run_mu.try_lock())
    {
        throw abelkhan::Exception("run mast at single thread!");
    }

    _run();

    _run_mu.unlock();
}


}

