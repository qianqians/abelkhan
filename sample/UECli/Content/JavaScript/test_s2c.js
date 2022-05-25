"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.test_s2c_module = exports.test_s2c_ping_rsp = void 0;
const client_handle = require("./client_handle");
/*this enum code is codegen by abelkhan codegen for ts*/
/*this struct code is codegen by abelkhan codegen for typescript*/
/*this module code is codegen by abelkhan codegen for typescript*/
class test_s2c_ping_rsp {
    uuid_94d71f95_a670_3916_89a9_44df18fb711b;
    hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d;
    _client_handle;
    constructor(_client_handle_, current_hub, _uuid) {
        this._client_handle = _client_handle_;
        this.hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d = current_hub;
        this.uuid_94d71f95_a670_3916_89a9_44df18fb711b = _uuid;
    }
    rsp() {
        let _argv_ca6794ee_a403_309d_b40e_f37578d53e8d = [this.uuid_94d71f95_a670_3916_89a9_44df18fb711b];
        this._client_handle.call_hub(this.hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d, "test_s2c_rsp_cb_ping_rsp", _argv_ca6794ee_a403_309d_b40e_f37578d53e8d);
    }
    err() {
        let _argv_ca6794ee_a403_309d_b40e_f37578d53e8d = [this.uuid_94d71f95_a670_3916_89a9_44df18fb711b];
        this._client_handle.call_hub(this.hub_name_ca6794ee_a403_309d_b40e_f37578d53e8d, "test_s2c_rsp_cb_ping_err", _argv_ca6794ee_a403_309d_b40e_f37578d53e8d);
    }
}
exports.test_s2c_ping_rsp = test_s2c_ping_rsp;
class test_s2c_module extends client_handle.imodule {
    _client_handle;
    constructor(_client_handle_) {
        super();
        this._client_handle = _client_handle_;
        this._client_handle._modulemng.add_method("test_s2c_ping", this.ping.bind(this));
        this.cb_ping = null;
    }
    cb_ping;
    ping(inArray) {
        let _cb_uuid = inArray[0];
        let _argv_ = [];
        this.rsp = new test_s2c_ping_rsp(this._client_handle, this._client_handle.current_hub, _cb_uuid);
        if (this.cb_ping) {
            this.cb_ping.apply(null, _argv_);
        }
        this.rsp = null;
    }
}
exports.test_s2c_module = test_s2c_module;
//# sourceMappingURL=test_s2c.js.map