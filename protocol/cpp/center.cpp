#include "center.h"

namespace abelkhan
{

/*this caller code is codegen by abelkhan codegen for cpp*/
std::shared_ptr<center_rsp_cb> center_caller::rsp_cb_center_handle = nullptr;
center_reg_server_mq_cb::center_reg_server_mq_cb(uint64_t _cb_uuid, std::shared_ptr<center_rsp_cb> _module_rsp_cb) {
    cb_uuid = _cb_uuid;
    module_rsp_cb = _module_rsp_cb;
}

std::shared_ptr<center_reg_server_mq_cb> center_reg_server_mq_cb::callBack(std::function<void()> cb, std::function<void()> err) {
    sig_reg_server_mq_cb.connect(cb);
    sig_reg_server_mq_err.connect(err);
    return shared_from_this();
}

void center_reg_server_mq_cb::timeout(uint64_t tick, std::function<void()> timeout_cb) {
    auto _module_rsp_cb = module_rsp_cb;
    auto _cb_uuid = cb_uuid;
    TinyTimer::add_timer(tick, [_module_rsp_cb, _cb_uuid](){
        _module_rsp_cb->reg_server_mq_timeout(_cb_uuid);
    });
    sig_reg_server_mq_timeout.connect(timeout_cb);
}

center_reconn_reg_server_mq_cb::center_reconn_reg_server_mq_cb(uint64_t _cb_uuid, std::shared_ptr<center_rsp_cb> _module_rsp_cb) {
    cb_uuid = _cb_uuid;
    module_rsp_cb = _module_rsp_cb;
}

std::shared_ptr<center_reconn_reg_server_mq_cb> center_reconn_reg_server_mq_cb::callBack(std::function<void()> cb, std::function<void()> err) {
    sig_reconn_reg_server_mq_cb.connect(cb);
    sig_reconn_reg_server_mq_err.connect(err);
    return shared_from_this();
}

void center_reconn_reg_server_mq_cb::timeout(uint64_t tick, std::function<void()> timeout_cb) {
    auto _module_rsp_cb = module_rsp_cb;
    auto _cb_uuid = cb_uuid;
    TinyTimer::add_timer(tick, [_module_rsp_cb, _cb_uuid](){
        _module_rsp_cb->reconn_reg_server_mq_timeout(_cb_uuid);
    });
    sig_reconn_reg_server_mq_timeout.connect(timeout_cb);
}

center_heartbeat_cb::center_heartbeat_cb(uint64_t _cb_uuid, std::shared_ptr<center_rsp_cb> _module_rsp_cb) {
    cb_uuid = _cb_uuid;
    module_rsp_cb = _module_rsp_cb;
}

std::shared_ptr<center_heartbeat_cb> center_heartbeat_cb::callBack(std::function<void()> cb, std::function<void()> err) {
    sig_heartbeat_cb.connect(cb);
    sig_heartbeat_err.connect(err);
    return shared_from_this();
}

void center_heartbeat_cb::timeout(uint64_t tick, std::function<void()> timeout_cb) {
    auto _module_rsp_cb = module_rsp_cb;
    auto _cb_uuid = cb_uuid;
    TinyTimer::add_timer(tick, [_module_rsp_cb, _cb_uuid](){
        _module_rsp_cb->heartbeat_timeout(_cb_uuid);
    });
    sig_heartbeat_timeout.connect(timeout_cb);
}

std::shared_ptr<center_call_server_rsp_cb> center_call_server_caller::rsp_cb_center_call_server_handle = nullptr;
std::shared_ptr<center_call_hub_rsp_cb> center_call_hub_caller::rsp_cb_center_call_hub_handle = nullptr;
std::shared_ptr<gm_center_rsp_cb> gm_center_caller::rsp_cb_gm_center_handle = nullptr;

}
