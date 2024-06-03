/*
 * hub_msg_handle.h
 * qianqians
 * 2024/5/30
 */
#ifndef _hub_msg_handle_h_
#define _hub_msg_handle_h_

#include <memory>
#include <string>
#include <exception>

#include <spdlog/spdlog.h>

#include <modulemng_handle.h>
#include <dbproxy.h>

#include "dbproxy_server.h"
#include "hubmanager.h"
#include "mongodbproxy.h"

namespace dbproxy
{

class hub_msg_handle
{
private:
    std::shared_ptr<hubmanager> _hubmanager;
    std::shared_ptr<abelkhan::hub_call_dbproxy_module> _hub_call_dbproxy_module;

public:
    hub_msg_handle(std::shared_ptr<hubmanager> _hubmanager_)
    {
        _hubmanager = _hubmanager_;

        _hub_call_dbproxy_module = std::make_shared<abelkhan::hub_call_dbproxy_module>();
        _hub_call_dbproxy_module->Init(service::_modulemng);

        _hub_call_dbproxy_module->sig_reg_hub.connect(std::bind(&hub_msg_handle::reg_hub, this, std::placeholders::_1));
        _hub_call_dbproxy_module->sig_get_guid.connect(std::bind(&hub_msg_handle::get_guid, this, std::placeholders::_1, std::placeholders::_2));
        _hub_call_dbproxy_module->sig_create_persisted_object.connect(std::bind(&hub_msg_handle::create_persisted_object, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3));
        _hub_call_dbproxy_module->sig_updata_persisted_object.connect(std::bind(&hub_msg_handle::updata_persisted_object, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3, std::placeholders::_4, std::placeholders::_5));
        _hub_call_dbproxy_module->sig_find_and_modify.connect(std::bind(&hub_msg_handle::find_and_modify, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3, std::placeholders::_4, std::placeholders::_5, std::placeholders::_6));
        _hub_call_dbproxy_module->sig_remove_object.connect(std::bind(&hub_msg_handle::remove_object, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3));
        _hub_call_dbproxy_module->sig_get_object_count.connect(std::bind(&hub_msg_handle::get_object_count, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3));
        _hub_call_dbproxy_module->sig_get_object_info.connect(std::bind(&hub_msg_handle::get_object_info, this, std::placeholders::_1, std::placeholders::_2, std::placeholders::_3, std::placeholders::_4, std::placeholders::_5, std::placeholders::_6, std::placeholders::_7, std::placeholders::_8));
    }

private:
    void reg_hub(std::string hub_name)
    {
        spdlog::trace("hub {0} connected", hub_name);

        auto rsp = std::static_pointer_cast<abelkhan::hub_call_dbproxy_reg_hub_rsp>(_hub_call_dbproxy_module->rsp);
        auto _ = _hubmanager->reg_hub(_hub_call_dbproxy_module->current_ch, hub_name);
        rsp->rsp();
    }

    void get_guid(std::string db, std::string collection)
    {
        spdlog::trace("begin get_guid!");

        auto rsp = std::static_pointer_cast<abelkhan::hub_call_dbproxy_get_guid_rsp>(_hub_call_dbproxy_module->rsp);
        try
        {
            auto guid = dbproxy::_mongodbproxy->get_guid(db, collection);
            if (guid > 0)
            {
                rsp->rsp(guid);
            }
            else
            {
                rsp->err();
            }
        }
        catch (std::exception ex)
        {
            spdlog::error("ex:{0}", ex.what());
            rsp->err();
        }

        spdlog::trace("end get_guid");
    }

    void create_persisted_object(std::string db, std::string collection, std::vector<uint8_t> object_info)
    {
        spdlog::trace("begin create_persisted_object");

        auto rsp = std::static_pointer_cast<abelkhan::hub_call_dbproxy_create_persisted_object_rsp>(_hub_call_dbproxy_module->rsp);
        try
        {
            auto is_create_sucess = dbproxy::_mongodbproxy->save(db, collection, object_info);
            if (is_create_sucess)
            {
                rsp->rsp();
            }
            else
            {
                rsp->err();
            }
        }
        catch (std::exception ex)
        {
            spdlog::error("ex:{0}", ex.what());
            rsp->err();
        }

        spdlog::trace("end create_persisted_object");
    }

    void updata_persisted_object(std::string db, std::string collection, std::vector<uint8_t> query_data, std::vector<uint8_t> object_data, bool upsert)
    {
        spdlog::trace("begin updata_persisted_object");

        auto rsp = std::static_pointer_cast<abelkhan::hub_call_dbproxy_updata_persisted_object_rsp>(_hub_call_dbproxy_module->rsp);
        try
        {
            auto is_update_sucessed = dbproxy::_mongodbproxy->update(db, collection, query_data, object_data, upsert);
            if (is_update_sucessed)
            {
                rsp->rsp();
            }
            else
            {
                rsp->err();
            }
        }
        catch (std::exception ex)
        {
            spdlog::error("ex:{0}", ex.what());
            rsp->err();
        }

        spdlog::trace("end updata_persisted_object");
    }

    void find_and_modify(std::string db, std::string collection, std::vector<uint8_t> query_data, std::vector<uint8_t> object_data, bool is_new, bool upsert)
    {
        spdlog::trace("begin find_and_modify");

        auto rsp = std::static_pointer_cast<abelkhan::hub_call_dbproxy_find_and_modify_rsp>(_hub_call_dbproxy_module->rsp);
        try
        {
            auto obj = dbproxy::_mongodbproxy->find_and_modify(db, collection, query_data, object_data, is_new, upsert);
            if (obj != nullptr)
            {
                std::vector<uint8_t> st;
                st.resize(obj->len);
                memcpy(st.data(), bson_get_data(obj), obj->len);
                bson_destroy(obj);

                rsp->rsp(st);
            }
            else
            {
                rsp->err();
            }
        }
        catch (std::exception ex)
        {
            spdlog::error("ex:{0}", ex.what());
            rsp->err();
        }

        spdlog::trace("end find_and_modify");
    }

    void remove_object(std::string db, std::string collection, std::vector<uint8_t> query_data)
    {
        spdlog::trace("begin remove_object");

        auto rsp = std::static_pointer_cast<abelkhan::hub_call_dbproxy_remove_object_rsp>(_hub_call_dbproxy_module->rsp);
        try
        {
            auto is_remove_sucessed = dbproxy::_mongodbproxy->remove(db, collection, query_data);
            if (is_remove_sucessed)
            {
                rsp->rsp();
            }
            else
            {
                rsp->err();
            }
        }
        catch (std::exception ex)
        {
            spdlog::error("ex:{0}", ex.what());
            rsp->err();
        }

        spdlog::trace("end remove_object");
    }

    void get_object_count(std::string db, std::string collection, std::vector<uint8_t> query_data)
    {
        spdlog::trace("begin get_object_info");

        auto rsp = std::static_pointer_cast<abelkhan::hub_call_dbproxy_get_object_count_rsp>(_hub_call_dbproxy_module->rsp);
        try
        {
            auto count = dbproxy::_mongodbproxy->count(db, collection, query_data);
            rsp->rsp((uint32_t)count);
        }
        catch (std::exception ex)
        {
            spdlog::error("ex:{0}", ex.what());
            rsp->err();
        }

        spdlog::trace("end get_object_info");
    }

    void get_object_info(std::string db, std::string collection, std::vector<uint8_t> query_data, int _skip, int _limit, std::string _sort, bool _Ascending_, std::string callbackid)
    {
        spdlog::trace("begin get_object_info");

        std::shared_ptr<hubproxy> _hubproxy;
        if (dbproxy::_hubmanager->get_hub(_hub_call_dbproxy_module->current_ch, _hubproxy))
        {
            try
            {
                auto c = dbproxy::_mongodbproxy->find(db, collection, query_data, _limit, _skip, _sort, _Ascending_);

                int count = 0;
                int total_count = 0;
                std::vector<bson_t*> _datalist;
                const bson_t *doc;
                while (mongoc_cursor_next(c, &doc))
                {
                    _datalist.push_back(bson_copy(doc));

                    count++;
                    total_count++;
                    if (count >= 100)
                    {
                        bson_t parent = BSON_INITIALIZER;
                        bson_array_builder_t* bab;
                        bson_append_array_builder_begin(&parent, "_list", count, &bab);
                        for (auto it : _datalist) {
                            bson_array_builder_append_document(bab, it);
                        }
                        bson_append_array_builder_end(&parent, bab);
                        _hubproxy->ack_get_object_info(callbackid, &parent);

                        count = 0;
                        for (auto it : _datalist) {
                            bson_destroy(it);
                        }
                        _datalist.clear();
                        bson_destroy(&parent);
                    }
                }
                if (total_count <= 0 || count > 0)
                {
                    bson_t parent = BSON_INITIALIZER;
                    bson_array_builder_t* bab;
                    bson_append_array_builder_begin(&parent, "_list", count, &bab);
                    for (auto it : _datalist) {
                        bson_array_builder_append_document(bab, it);
                    }
                    bson_append_array_builder_end(&parent, bab);
                    _hubproxy->ack_get_object_info(callbackid, &parent);

                    count = 0;
                    for (auto it : _datalist) {
                        bson_destroy(it);
                    }
                    _datalist.clear();
                    bson_destroy(&parent);
                }

                _hubproxy->ack_get_object_info_end(callbackid);
            }
            catch (std::exception ex)
            {
                spdlog::error("ex:{0}", ex.what());
            }
        }
        else
        {
            spdlog::error("hubproxy is null");
        }

        spdlog::trace("end get_object_info");
    }
};

}

#endif // !_hub_msg_handle_h_