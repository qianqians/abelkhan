"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.test_c2s_hubproxy = exports.test_c2s_caller = exports.test_c2s_rsp_cb = exports.test_c2s_get_websocket_svr_host_cb = exports.test_c2s_get_svr_host_cb = void 0;
const client_handle = require("./client_handle");
/*this enum code is codegen by abelkhan codegen for ts*/
/*this struct code is codegen by abelkhan codegen for typescript*/
/*this caller code is codegen by abelkhan codegen for typescript*/
class test_c2s_get_svr_host_cb {
    cb_uuid;
    module_rsp_cb;
    event_get_svr_host_handle_cb;
    event_get_svr_host_handle_err;
    event_get_svr_host_handle_timeout;
    constructor(_cb_uuid, _module_rsp_cb) {
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_get_svr_host_handle_cb = null;
        this.event_get_svr_host_handle_err = null;
        this.event_get_svr_host_handle_timeout = null;
    }
    callBack(_cb, _err) {
        this.event_get_svr_host_handle_cb = _cb;
        this.event_get_svr_host_handle_err = _err;
        return this;
    }
    timeout(tick, timeout_cb) {
        setTimeout(() => { this.module_rsp_cb.get_svr_host_timeout(this.cb_uuid); }, tick);
        this.event_get_svr_host_handle_timeout = timeout_cb;
    }
}
exports.test_c2s_get_svr_host_cb = test_c2s_get_svr_host_cb;
class test_c2s_get_websocket_svr_host_cb {
    cb_uuid;
    module_rsp_cb;
    event_get_websocket_svr_host_handle_cb;
    event_get_websocket_svr_host_handle_err;
    event_get_websocket_svr_host_handle_timeout;
    constructor(_cb_uuid, _module_rsp_cb) {
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_get_websocket_svr_host_handle_cb = null;
        this.event_get_websocket_svr_host_handle_err = null;
        this.event_get_websocket_svr_host_handle_timeout = null;
    }
    callBack(_cb, _err) {
        this.event_get_websocket_svr_host_handle_cb = _cb;
        this.event_get_websocket_svr_host_handle_err = _err;
        return this;
    }
    timeout(tick, timeout_cb) {
        setTimeout(() => { this.module_rsp_cb.get_websocket_svr_host_timeout(this.cb_uuid); }, tick);
        this.event_get_websocket_svr_host_handle_timeout = timeout_cb;
    }
}
exports.test_c2s_get_websocket_svr_host_cb = test_c2s_get_websocket_svr_host_cb;
/*this cb code is codegen by abelkhan for ts*/
class test_c2s_rsp_cb extends client_handle.imodule {
    map_get_svr_host;
    map_get_websocket_svr_host;
    constructor(modules) {
        super();
        this.map_get_svr_host = new Map();
        modules.add_method("test_c2s_rsp_cb_get_svr_host_rsp", this.get_svr_host_rsp.bind(this));
        modules.add_method("test_c2s_rsp_cb_get_svr_host_err", this.get_svr_host_err.bind(this));
        this.map_get_websocket_svr_host = new Map();
        modules.add_method("test_c2s_rsp_cb_get_websocket_svr_host_rsp", this.get_websocket_svr_host_rsp.bind(this));
        modules.add_method("test_c2s_rsp_cb_get_websocket_svr_host_err", this.get_websocket_svr_host_err.bind(this));
    }
    get_svr_host_rsp(inArray) {
        let uuid = inArray[0];
        let _argv_abbb842f_52d0_34e7_9d8d_642d072db165 = [];
        _argv_abbb842f_52d0_34e7_9d8d_642d072db165.push(inArray[1]);
        _argv_abbb842f_52d0_34e7_9d8d_642d072db165.push(inArray[2]);
        var rsp = this.try_get_and_del_get_svr_host_cb(uuid);
        if (rsp && rsp.event_get_svr_host_handle_cb) {
            rsp.event_get_svr_host_handle_cb.apply(null, _argv_abbb842f_52d0_34e7_9d8d_642d072db165);
        }
    }
    get_svr_host_err(inArray) {
        let uuid = inArray[0];
        let _argv_abbb842f_52d0_34e7_9d8d_642d072db165 = [];
        var rsp = this.try_get_and_del_get_svr_host_cb(uuid);
        if (rsp && rsp.event_get_svr_host_handle_err) {
            rsp.event_get_svr_host_handle_err.apply(null, _argv_abbb842f_52d0_34e7_9d8d_642d072db165);
        }
    }
    get_svr_host_timeout(cb_uuid) {
        let rsp = this.try_get_and_del_get_svr_host_cb(cb_uuid);
        if (rsp) {
            if (rsp.event_get_svr_host_handle_timeout) {
                rsp.event_get_svr_host_handle_timeout.apply(null);
            }
        }
    }
    try_get_and_del_get_svr_host_cb(uuid) {
        var rsp = this.map_get_svr_host.get(uuid);
        this.map_get_svr_host.delete(uuid);
        return rsp;
    }
    get_websocket_svr_host_rsp(inArray) {
        let uuid = inArray[0];
        let _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f = [];
        _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f.push(inArray[1]);
        _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f.push(inArray[2]);
        var rsp = this.try_get_and_del_get_websocket_svr_host_cb(uuid);
        if (rsp && rsp.event_get_websocket_svr_host_handle_cb) {
            rsp.event_get_websocket_svr_host_handle_cb.apply(null, _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f);
        }
    }
    get_websocket_svr_host_err(inArray) {
        let uuid = inArray[0];
        let _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f = [];
        var rsp = this.try_get_and_del_get_websocket_svr_host_cb(uuid);
        if (rsp && rsp.event_get_websocket_svr_host_handle_err) {
            rsp.event_get_websocket_svr_host_handle_err.apply(null, _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f);
        }
    }
    get_websocket_svr_host_timeout(cb_uuid) {
        let rsp = this.try_get_and_del_get_websocket_svr_host_cb(cb_uuid);
        if (rsp) {
            if (rsp.event_get_websocket_svr_host_handle_timeout) {
                rsp.event_get_websocket_svr_host_handle_timeout.apply(null);
            }
        }
    }
    try_get_and_del_get_websocket_svr_host_cb(uuid) {
        var rsp = this.map_get_websocket_svr_host.get(uuid);
        this.map_get_websocket_svr_host.delete(uuid);
        return rsp;
    }
}
exports.test_c2s_rsp_cb = test_c2s_rsp_cb;
let rsp_cb_test_c2s_handle = null;
class test_c2s_caller {
    _hubproxy;
    constructor(_client) {
        if (rsp_cb_test_c2s_handle == null) {
            rsp_cb_test_c2s_handle = new test_c2s_rsp_cb(_client._modulemng);
        }
        this._hubproxy = new test_c2s_hubproxy(_client);
    }
    get_hub(hub_name) {
        this._hubproxy.hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5 = hub_name;
        return this._hubproxy;
    }
}
exports.test_c2s_caller = test_c2s_caller;
class test_c2s_hubproxy {
    uuid_c233fb06_7c62_3839_a7d5_edade25b16c5 = Math.round(Math.random() * 1000);
    hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5;
    _client_handle;
    constructor(client_handle_) {
        this._client_handle = client_handle_;
    }
    login() {
        let _argv_d3bb20a7_d0fc_3440_bb9e_b3cc0630e2d1 = [];
        this._client_handle.call_hub(this.hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5, "test_c2s_login", _argv_d3bb20a7_d0fc_3440_bb9e_b3cc0630e2d1);
    }
    get_svr_host() {
        let uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65 = Math.round(this.uuid_c233fb06_7c62_3839_a7d5_edade25b16c5++);
        let _argv_abbb842f_52d0_34e7_9d8d_642d072db165 = [uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65];
        this._client_handle.call_hub(this.hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5, "test_c2s_get_svr_host", _argv_abbb842f_52d0_34e7_9d8d_642d072db165);
        let cb_get_svr_host_obj = new test_c2s_get_svr_host_cb(uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65, rsp_cb_test_c2s_handle);
        if (rsp_cb_test_c2s_handle) {
            rsp_cb_test_c2s_handle.map_get_svr_host.set(uuid_7d3daecb_6f7c_5aba_96f4_8c3441412b65, cb_get_svr_host_obj);
        }
        return cb_get_svr_host_obj;
    }
    get_websocket_svr_host() {
        let uuid_4c3154db_d59e_53aa_8765_bd54308cf4a5 = Math.round(this.uuid_c233fb06_7c62_3839_a7d5_edade25b16c5++);
        let _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f = [uuid_4c3154db_d59e_53aa_8765_bd54308cf4a5];
        this._client_handle.call_hub(this.hub_name_c233fb06_7c62_3839_a7d5_edade25b16c5, "test_c2s_get_websocket_svr_host", _argv_ea3a8af7_4bd0_3344_a846_4962c0e7c00f);
        let cb_get_websocket_svr_host_obj = new test_c2s_get_websocket_svr_host_cb(uuid_4c3154db_d59e_53aa_8765_bd54308cf4a5, rsp_cb_test_c2s_handle);
        if (rsp_cb_test_c2s_handle) {
            rsp_cb_test_c2s_handle.map_get_websocket_svr_host.set(uuid_4c3154db_d59e_53aa_8765_bd54308cf4a5, cb_get_websocket_svr_host_obj);
        }
        return cb_get_websocket_svr_host_obj;
    }
}
exports.test_c2s_hubproxy = test_c2s_hubproxy;
//# sourceMappingURL=test_c2s.js.map