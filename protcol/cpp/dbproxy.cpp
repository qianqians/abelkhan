#include "dbproxy.h"

namespace abelkhan
{

/*this caller code is codegen by abelkhan codegen for cpp*/
std::shared_ptr<dbproxy_call_hub_rsp_cb> dbproxy_call_hub_caller::rsp_cb_dbproxy_call_hub_handle = nullptr;
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
    TinyTimer::add_timer(tick, [this](){
        module_rsp_cb->reg_hub_timeout(cb_uuid);
    });
    sig_reg_hub_timeout.connect(timeout_cb);
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
    TinyTimer::add_timer(tick, [this](){
        module_rsp_cb->create_persisted_object_timeout(cb_uuid);
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
    TinyTimer::add_timer(tick, [this](){
        module_rsp_cb->updata_persisted_object_timeout(cb_uuid);
    });
    sig_updata_persisted_object_timeout.connect(timeout_cb);
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
    TinyTimer::add_timer(tick, [this](){
        module_rsp_cb->remove_object_timeout(cb_uuid);
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
    TinyTimer::add_timer(tick, [this](){
        module_rsp_cb->get_object_count_timeout(cb_uuid);
    });
    sig_get_object_count_timeout.connect(timeout_cb);
}


}
