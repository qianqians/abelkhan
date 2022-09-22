#include "dbproxy.h"

namespace abelkhan
{

/*this caller code is codegen by abelkhan codegen for cpp*/
std::shared_ptr<hub_call_dbproxy_rsp_cb> hub_call_dbproxy_caller::rsp_cb_hub_call_dbproxy_handle = nullptr;
hub_call_dbproxy_reg_hub_cb::hub_call_dbproxy_reg_hub_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_dbproxy_rsp_cb> _module_rsp_cb) {
    cb_uuid = _cb_uuid;
    module_rsp_cb = _module_rsp_cb;
}

std::shared_ptr<hub_call_dbproxy_reg_hub_cb> hub_call_dbproxy_reg_hub_cb::callBack(std::function<void()> cb, std::function<void()> err) {
    sig_reg_hub_cb.connect(cb);
    sig_reg_hub_err.connect(err);
    return shared_from_this();
}

void hub_call_dbproxy_reg_hub_cb::timeout(uint64_t tick, std::function<void()> timeout_cb) {
    auto _module_rsp_cb = module_rsp_cb;
    auto _cb_uuid = cb_uuid;
    TinyTimer::add_timer(tick, [_module_rsp_cb, _cb_uuid](){
        _module_rsp_cb->reg_hub_timeout(_cb_uuid);
    });
    sig_reg_hub_timeout.connect(timeout_cb);
}

hub_call_dbproxy_get_guid_cb::hub_call_dbproxy_get_guid_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_dbproxy_rsp_cb> _module_rsp_cb) {
    cb_uuid = _cb_uuid;
    module_rsp_cb = _module_rsp_cb;
}

std::shared_ptr<hub_call_dbproxy_get_guid_cb> hub_call_dbproxy_get_guid_cb::callBack(std::function<void(int64_t guid)> cb, std::function<void()> err) {
    sig_get_guid_cb.connect(cb);
    sig_get_guid_err.connect(err);
    return shared_from_this();
}

void hub_call_dbproxy_get_guid_cb::timeout(uint64_t tick, std::function<void()> timeout_cb) {
    auto _module_rsp_cb = module_rsp_cb;
    auto _cb_uuid = cb_uuid;
    TinyTimer::add_timer(tick, [_module_rsp_cb, _cb_uuid](){
        _module_rsp_cb->get_guid_timeout(_cb_uuid);
    });
    sig_get_guid_timeout.connect(timeout_cb);
}

hub_call_dbproxy_create_persisted_object_cb::hub_call_dbproxy_create_persisted_object_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_dbproxy_rsp_cb> _module_rsp_cb) {
    cb_uuid = _cb_uuid;
    module_rsp_cb = _module_rsp_cb;
}

std::shared_ptr<hub_call_dbproxy_create_persisted_object_cb> hub_call_dbproxy_create_persisted_object_cb::callBack(std::function<void()> cb, std::function<void()> err) {
    sig_create_persisted_object_cb.connect(cb);
    sig_create_persisted_object_err.connect(err);
    return shared_from_this();
}

void hub_call_dbproxy_create_persisted_object_cb::timeout(uint64_t tick, std::function<void()> timeout_cb) {
    auto _module_rsp_cb = module_rsp_cb;
    auto _cb_uuid = cb_uuid;
    TinyTimer::add_timer(tick, [_module_rsp_cb, _cb_uuid](){
        _module_rsp_cb->create_persisted_object_timeout(_cb_uuid);
    });
    sig_create_persisted_object_timeout.connect(timeout_cb);
}

hub_call_dbproxy_updata_persisted_object_cb::hub_call_dbproxy_updata_persisted_object_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_dbproxy_rsp_cb> _module_rsp_cb) {
    cb_uuid = _cb_uuid;
    module_rsp_cb = _module_rsp_cb;
}

std::shared_ptr<hub_call_dbproxy_updata_persisted_object_cb> hub_call_dbproxy_updata_persisted_object_cb::callBack(std::function<void()> cb, std::function<void()> err) {
    sig_updata_persisted_object_cb.connect(cb);
    sig_updata_persisted_object_err.connect(err);
    return shared_from_this();
}

void hub_call_dbproxy_updata_persisted_object_cb::timeout(uint64_t tick, std::function<void()> timeout_cb) {
    auto _module_rsp_cb = module_rsp_cb;
    auto _cb_uuid = cb_uuid;
    TinyTimer::add_timer(tick, [_module_rsp_cb, _cb_uuid](){
        _module_rsp_cb->updata_persisted_object_timeout(_cb_uuid);
    });
    sig_updata_persisted_object_timeout.connect(timeout_cb);
}

hub_call_dbproxy_find_and_modify_cb::hub_call_dbproxy_find_and_modify_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_dbproxy_rsp_cb> _module_rsp_cb) {
    cb_uuid = _cb_uuid;
    module_rsp_cb = _module_rsp_cb;
}

std::shared_ptr<hub_call_dbproxy_find_and_modify_cb> hub_call_dbproxy_find_and_modify_cb::callBack(std::function<void(std::vector<uint8_t> object_info)> cb, std::function<void()> err) {
    sig_find_and_modify_cb.connect(cb);
    sig_find_and_modify_err.connect(err);
    return shared_from_this();
}

void hub_call_dbproxy_find_and_modify_cb::timeout(uint64_t tick, std::function<void()> timeout_cb) {
    auto _module_rsp_cb = module_rsp_cb;
    auto _cb_uuid = cb_uuid;
    TinyTimer::add_timer(tick, [_module_rsp_cb, _cb_uuid](){
        _module_rsp_cb->find_and_modify_timeout(_cb_uuid);
    });
    sig_find_and_modify_timeout.connect(timeout_cb);
}

hub_call_dbproxy_remove_object_cb::hub_call_dbproxy_remove_object_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_dbproxy_rsp_cb> _module_rsp_cb) {
    cb_uuid = _cb_uuid;
    module_rsp_cb = _module_rsp_cb;
}

std::shared_ptr<hub_call_dbproxy_remove_object_cb> hub_call_dbproxy_remove_object_cb::callBack(std::function<void()> cb, std::function<void()> err) {
    sig_remove_object_cb.connect(cb);
    sig_remove_object_err.connect(err);
    return shared_from_this();
}

void hub_call_dbproxy_remove_object_cb::timeout(uint64_t tick, std::function<void()> timeout_cb) {
    auto _module_rsp_cb = module_rsp_cb;
    auto _cb_uuid = cb_uuid;
    TinyTimer::add_timer(tick, [_module_rsp_cb, _cb_uuid](){
        _module_rsp_cb->remove_object_timeout(_cb_uuid);
    });
    sig_remove_object_timeout.connect(timeout_cb);
}

hub_call_dbproxy_get_object_count_cb::hub_call_dbproxy_get_object_count_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_dbproxy_rsp_cb> _module_rsp_cb) {
    cb_uuid = _cb_uuid;
    module_rsp_cb = _module_rsp_cb;
}

std::shared_ptr<hub_call_dbproxy_get_object_count_cb> hub_call_dbproxy_get_object_count_cb::callBack(std::function<void(uint32_t count)> cb, std::function<void()> err) {
    sig_get_object_count_cb.connect(cb);
    sig_get_object_count_err.connect(err);
    return shared_from_this();
}

void hub_call_dbproxy_get_object_count_cb::timeout(uint64_t tick, std::function<void()> timeout_cb) {
    auto _module_rsp_cb = module_rsp_cb;
    auto _cb_uuid = cb_uuid;
    TinyTimer::add_timer(tick, [_module_rsp_cb, _cb_uuid](){
        _module_rsp_cb->get_object_count_timeout(_cb_uuid);
    });
    sig_get_object_count_timeout.connect(timeout_cb);
}

std::shared_ptr<dbproxy_call_hub_rsp_cb> dbproxy_call_hub_caller::rsp_cb_dbproxy_call_hub_handle = nullptr;

}
