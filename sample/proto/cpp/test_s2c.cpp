#include "test_s2c.h"

namespace abelkhan
{

/*this caller code is codegen by abelkhan codegen for cpp*/
std::shared_ptr<test_s2c_rsp_cb> test_s2c_caller::rsp_cb_test_s2c_handle = nullptr;
test_s2c_ping_cb::test_s2c_ping_cb(uint64_t _cb_uuid, std::shared_ptr<test_s2c_rsp_cb> _module_rsp_cb) {
    cb_uuid = _cb_uuid;
    module_rsp_cb = _module_rsp_cb;
}

std::shared_ptr<test_s2c_ping_cb> test_s2c_ping_cb::callBack(std::function<void()> cb, std::function<void()> err) {
    sig_ping_cb.connect(cb);
    sig_ping_err.connect(err);
    return shared_from_this();
}

void test_s2c_ping_cb::timeout(uint64_t tick, std::function<void()> timeout_cb) {
    auto _module_rsp_cb = module_rsp_cb;
    auto _cb_uuid = cb_uuid;
    TinyTimer::add_timer(tick, [_module_rsp_cb, _cb_uuid](){
        _module_rsp_cb->ping_timeout(_cb_uuid);
    });
    sig_ping_timeout.connect(timeout_cb);
}


}
