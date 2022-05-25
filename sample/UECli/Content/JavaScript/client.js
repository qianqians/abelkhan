"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.gate_call_client_module = exports.gate_call_client_caller = exports.rsp_cb_gate_call_client_handle = exports.gate_call_client_rsp_cb = void 0;
const abelkhan = require("./abelkhan");
/*this enum code is codegen by abelkhan codegen for ts*/
/*this struct code is codegen by abelkhan codegen for typescript*/
/*this caller code is codegen by abelkhan codegen for typescript*/
/*this cb code is codegen by abelkhan for ts*/
class gate_call_client_rsp_cb extends abelkhan.Imodule {
    constructor(modules) {
        super("gate_call_client_rsp_cb");
    }
}
exports.gate_call_client_rsp_cb = gate_call_client_rsp_cb;
exports.rsp_cb_gate_call_client_handle = null;
class gate_call_client_caller extends abelkhan.Icaller {
    uuid_b84dd831_2e79_3280_a337_a69dd489e75f = Math.round(Math.random() * 1000);
    constructor(_ch, modules) {
        super("gate_call_client", _ch);
        if (exports.rsp_cb_gate_call_client_handle == null) {
            exports.rsp_cb_gate_call_client_handle = new gate_call_client_rsp_cb(modules);
        }
    }
    ntf_cuuid(cuuid) {
        let _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1 = [];
        _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1.push(cuuid);
        this.call_module_method("gate_call_client_ntf_cuuid", _argv_edc5d0e5_3fa8_3367_9d68_fa4111673ae1);
    }
    call_client(hub_name, rpc_argv) {
        let _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab = [];
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push(hub_name);
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push(rpc_argv);
        this.call_module_method("gate_call_client_call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
    }
}
exports.gate_call_client_caller = gate_call_client_caller;
/*this module code is codegen by abelkhan codegen for typescript*/
class gate_call_client_module extends abelkhan.Imodule {
    modules;
    constructor(modules) {
        super("gate_call_client");
        this.modules = modules;
        this.modules.reg_method("gate_call_client_ntf_cuuid", [this, this.ntf_cuuid.bind(this)]);
        this.modules.reg_method("gate_call_client_call_client", [this, this.call_client.bind(this)]);
        this.cb_ntf_cuuid = null;
        this.cb_call_client = null;
    }
    cb_ntf_cuuid;
    ntf_cuuid(inArray) {
        let _argv_ = [];
        _argv_.push(inArray[0]);
        if (this.cb_ntf_cuuid) {
            this.cb_ntf_cuuid.apply(null, _argv_);
        }
    }
    cb_call_client;
    call_client(inArray) {
        let _argv_ = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_call_client) {
            this.cb_call_client.apply(null, _argv_);
        }
    }
}
exports.gate_call_client_module = gate_call_client_module;
//# sourceMappingURL=client.js.map