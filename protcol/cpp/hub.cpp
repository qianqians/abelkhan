#include "hub.h"

namespace abelkhan
{

/*this caller code is codegen by abelkhan codegen for cpp*/
std::shared_ptr<hub_call_hub_rsp_cb> hub_call_hub_caller::rsp_cb_hub_call_hub_handle = nullptr;
hub_call_hub_reg_hub_cb::hub_call_hub_reg_hub_cb(uint64_t _cb_uuid, std::shared_ptr<hub_call_hub_rsp_cb> _module_rsp_cb) {
    cb_uuid = _cb_uuid;
    module_rsp_cb = _module_rsp_cb;
}

std::shared_ptr<hub_call_hub_reg_hub_cb> hub_call_hub_reg_hub_cb::callBack(std::function<void()> cb, std::function<void()> err) {
    sig_reg_hub_cb.connect(cb);
    sig_reg_hub_err.connect(err);
    return shared_from_this();
}

void hub_call_hub_reg_hub_cb::timeout(uint64_t tick, std::function<void()> timeout_cb) {
    TinyTimer::add_timer(tick, [this](){
        module_rsp_cb->reg_hub_timeout(cb_uuid);
    });
    sig_reg_hub_timeout.connect(timeout_cb);
}

std::shared_ptr<gate_call_hub_rsp_cb> gate_call_hub_caller::rsp_cb_gate_call_hub_handle = nullptr;
std::shared_ptr<client_call_hub_rsp_cb> client_call_hub_caller::rsp_cb_client_call_hub_handle = nullptr;
client_call_hub_heartbeats_cb::client_call_hub_heartbeats_cb(uint64_t _cb_uuid, std::shared_ptr<client_call_hub_rsp_cb> _module_rsp_cb) {
    cb_uuid = _cb_uuid;
    module_rsp_cb = _module_rsp_cb;
}

std::shared_ptr<client_call_hub_heartbeats_cb> client_call_hub_heartbeats_cb::callBack(std::function<void(uint64_t timetmp)> cb, std::function<void()> err) {
    sig_heartbeats_cb.connect(cb);
    sig_heartbeats_err.connect(err);
    return shared_from_this();
}

void client_call_hub_heartbeats_cb::timeout(uint64_t tick, std::function<void()> timeout_cb) {
    TinyTimer::add_timer(tick, [this](){
        module_rsp_cb->heartbeats_timeout(cb_uuid);
    });
    sig_heartbeats_timeout.connect(timeout_cb);
}

std::shared_ptr<hub_call_client_rsp_cb> hub_call_client_caller::rsp_cb_hub_call_client_handle = nullptr;

}
