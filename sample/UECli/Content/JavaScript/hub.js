"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.hub_call_client_module = exports.client_call_hub_module = exports.client_call_hub_heartbeats_rsp = exports.gate_call_hub_module = exports.hub_call_hub_module = exports.hub_call_hub_reg_hub_rsp = exports.hub_call_client_caller = exports.rsp_cb_hub_call_client_handle = exports.hub_call_client_rsp_cb = exports.client_call_hub_caller = exports.rsp_cb_client_call_hub_handle = exports.client_call_hub_rsp_cb = exports.client_call_hub_heartbeats_cb = exports.gate_call_hub_caller = exports.rsp_cb_gate_call_hub_handle = exports.gate_call_hub_rsp_cb = exports.hub_call_hub_caller = exports.rsp_cb_hub_call_hub_handle = exports.hub_call_hub_rsp_cb = exports.hub_call_hub_reg_hub_cb = void 0;
const abelkhan = require("./abelkhan");
/*this enum code is codegen by abelkhan codegen for ts*/
/*this struct code is codegen by abelkhan codegen for typescript*/
/*this caller code is codegen by abelkhan codegen for typescript*/
class hub_call_hub_reg_hub_cb {
    cb_uuid;
    module_rsp_cb;
    event_reg_hub_handle_cb;
    event_reg_hub_handle_err;
    event_reg_hub_handle_timeout;
    constructor(_cb_uuid, _module_rsp_cb) {
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_reg_hub_handle_cb = null;
        this.event_reg_hub_handle_err = null;
        this.event_reg_hub_handle_timeout = null;
    }
    callBack(_cb, _err) {
        this.event_reg_hub_handle_cb = _cb;
        this.event_reg_hub_handle_err = _err;
        return this;
    }
    timeout(tick, timeout_cb) {
        setTimeout(() => { this.module_rsp_cb.reg_hub_timeout(this.cb_uuid); }, tick);
        this.event_reg_hub_handle_timeout = timeout_cb;
    }
}
exports.hub_call_hub_reg_hub_cb = hub_call_hub_reg_hub_cb;
/*this cb code is codegen by abelkhan for ts*/
class hub_call_hub_rsp_cb extends abelkhan.Imodule {
    map_reg_hub;
    constructor(modules) {
        super("hub_call_hub_rsp_cb");
        modules.reg_module(this);
        this.map_reg_hub = new Map();
        this.reg_method("reg_hub_rsp", this.reg_hub_rsp.bind(this));
        this.reg_method("reg_hub_err", this.reg_hub_err.bind(this));
    }
    reg_hub_rsp(inArray) {
        let uuid = inArray[0];
        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [];
        var rsp = this.try_get_and_del_reg_hub_cb(uuid);
        if (rsp && rsp.event_reg_hub_handle_cb) {
            rsp.event_reg_hub_handle_cb.apply(null, _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        }
    }
    reg_hub_err(inArray) {
        let uuid = inArray[0];
        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [];
        var rsp = this.try_get_and_del_reg_hub_cb(uuid);
        if (rsp && rsp.event_reg_hub_handle_err) {
            rsp.event_reg_hub_handle_err.apply(null, _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        }
    }
    reg_hub_timeout(cb_uuid) {
        let rsp = this.try_get_and_del_reg_hub_cb(cb_uuid);
        if (rsp) {
            if (rsp.event_reg_hub_handle_timeout) {
                rsp.event_reg_hub_handle_timeout.apply(null);
            }
        }
    }
    try_get_and_del_reg_hub_cb(uuid) {
        var rsp = this.map_reg_hub.get(uuid);
        this.map_reg_hub.delete(uuid);
        return rsp;
    }
}
exports.hub_call_hub_rsp_cb = hub_call_hub_rsp_cb;
exports.rsp_cb_hub_call_hub_handle = null;
class hub_call_hub_caller extends abelkhan.Icaller {
    uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f = Math.round(Math.random() * 1000);
    constructor(_ch, modules) {
        super("hub_call_hub", _ch);
        if (exports.rsp_cb_hub_call_hub_handle == null) {
            exports.rsp_cb_hub_call_hub_handle = new hub_call_hub_rsp_cb(modules);
        }
    }
    reg_hub(hub_name, hub_type) {
        let uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = Math.round(this.uuid_c5ce2cc4_e178_3cb8_ba26_976964de368f++);
        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106];
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.push(hub_name);
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.push(hub_type);
        this.call_module_method("reg_hub", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        let cb_reg_hub_obj = new hub_call_hub_reg_hub_cb(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, exports.rsp_cb_hub_call_hub_handle);
        if (exports.rsp_cb_hub_call_hub_handle) {
            exports.rsp_cb_hub_call_hub_handle.map_reg_hub.set(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, cb_reg_hub_obj);
        }
        return cb_reg_hub_obj;
    }
    hub_call_hub_mothed(rpc_argv) {
        let _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e = [];
        _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e.push(rpc_argv);
        this.call_module_method("hub_call_hub_mothed", _argv_a9f78ac2_6f35_36c5_8d6f_32629449149e);
    }
}
exports.hub_call_hub_caller = hub_call_hub_caller;
/*this cb code is codegen by abelkhan for ts*/
class gate_call_hub_rsp_cb extends abelkhan.Imodule {
    constructor(modules) {
        super("gate_call_hub_rsp_cb");
        modules.reg_module(this);
    }
}
exports.gate_call_hub_rsp_cb = gate_call_hub_rsp_cb;
exports.rsp_cb_gate_call_hub_handle = null;
class gate_call_hub_caller extends abelkhan.Icaller {
    uuid_e1565384_c90b_3a02_ae2e_d0d91b2758d1 = Math.round(Math.random() * 1000);
    constructor(_ch, modules) {
        super("gate_call_hub", _ch);
        if (exports.rsp_cb_gate_call_hub_handle == null) {
            exports.rsp_cb_gate_call_hub_handle = new gate_call_hub_rsp_cb(modules);
        }
    }
    client_disconnect(client_uuid) {
        let _argv_0b9435aa_3d03_3778_acfb_c7bfbd4f3e60 = [];
        _argv_0b9435aa_3d03_3778_acfb_c7bfbd4f3e60.push(client_uuid);
        this.call_module_method("client_disconnect", _argv_0b9435aa_3d03_3778_acfb_c7bfbd4f3e60);
    }
    client_exception(client_uuid) {
        let _argv_706b1331_3629_3681_9d39_d2ef3b6675ed = [];
        _argv_706b1331_3629_3681_9d39_d2ef3b6675ed.push(client_uuid);
        this.call_module_method("client_exception", _argv_706b1331_3629_3681_9d39_d2ef3b6675ed);
    }
    client_call_hub(client_uuid, rpc_argv) {
        let _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263 = [];
        _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263.push(client_uuid);
        _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263.push(rpc_argv);
        this.call_module_method("client_call_hub", _argv_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263);
    }
}
exports.gate_call_hub_caller = gate_call_hub_caller;
class client_call_hub_heartbeats_cb {
    cb_uuid;
    module_rsp_cb;
    event_heartbeats_handle_cb;
    event_heartbeats_handle_err;
    event_heartbeats_handle_timeout;
    constructor(_cb_uuid, _module_rsp_cb) {
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_heartbeats_handle_cb = null;
        this.event_heartbeats_handle_err = null;
        this.event_heartbeats_handle_timeout = null;
    }
    callBack(_cb, _err) {
        this.event_heartbeats_handle_cb = _cb;
        this.event_heartbeats_handle_err = _err;
        return this;
    }
    timeout(tick, timeout_cb) {
        setTimeout(() => { this.module_rsp_cb.heartbeats_timeout(this.cb_uuid); }, tick);
        this.event_heartbeats_handle_timeout = timeout_cb;
    }
}
exports.client_call_hub_heartbeats_cb = client_call_hub_heartbeats_cb;
/*this cb code is codegen by abelkhan for ts*/
class client_call_hub_rsp_cb extends abelkhan.Imodule {
    map_heartbeats;
    constructor(modules) {
        super("client_call_hub_rsp_cb");
        modules.reg_module(this);
        this.map_heartbeats = new Map();
        this.reg_method("heartbeats_rsp", this.heartbeats_rsp.bind(this));
        this.reg_method("heartbeats_err", this.heartbeats_err.bind(this));
    }
    heartbeats_rsp(inArray) {
        let uuid = inArray[0];
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = [];
        _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.push(inArray[1]);
        var rsp = this.try_get_and_del_heartbeats_cb(uuid);
        if (rsp && rsp.event_heartbeats_handle_cb) {
            rsp.event_heartbeats_handle_cb.apply(null, _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        }
    }
    heartbeats_err(inArray) {
        let uuid = inArray[0];
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = [];
        var rsp = this.try_get_and_del_heartbeats_cb(uuid);
        if (rsp && rsp.event_heartbeats_handle_err) {
            rsp.event_heartbeats_handle_err.apply(null, _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        }
    }
    heartbeats_timeout(cb_uuid) {
        let rsp = this.try_get_and_del_heartbeats_cb(cb_uuid);
        if (rsp) {
            if (rsp.event_heartbeats_handle_timeout) {
                rsp.event_heartbeats_handle_timeout.apply(null);
            }
        }
    }
    try_get_and_del_heartbeats_cb(uuid) {
        var rsp = this.map_heartbeats.get(uuid);
        this.map_heartbeats.delete(uuid);
        return rsp;
    }
}
exports.client_call_hub_rsp_cb = client_call_hub_rsp_cb;
exports.rsp_cb_client_call_hub_handle = null;
class client_call_hub_caller extends abelkhan.Icaller {
    uuid_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263 = Math.round(Math.random() * 1000);
    constructor(_ch, modules) {
        super("client_call_hub", _ch);
        if (exports.rsp_cb_client_call_hub_handle == null) {
            exports.rsp_cb_client_call_hub_handle = new client_call_hub_rsp_cb(modules);
        }
    }
    connect_hub(client_uuid) {
        let _argv_dc2ee339_bef5_3af9_a492_592ba4f08559 = [];
        _argv_dc2ee339_bef5_3af9_a492_592ba4f08559.push(client_uuid);
        this.call_module_method("connect_hub", _argv_dc2ee339_bef5_3af9_a492_592ba4f08559);
    }
    heartbeats() {
        let uuid_a514ca5f_2c67_5668_aac0_354397bdce36 = Math.round(this.uuid_e4b1f5c3_57b2_3ae3_b088_1e3a5d705263++);
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = [uuid_a514ca5f_2c67_5668_aac0_354397bdce36];
        this.call_module_method("heartbeats", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        let cb_heartbeats_obj = new client_call_hub_heartbeats_cb(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, exports.rsp_cb_client_call_hub_handle);
        if (exports.rsp_cb_client_call_hub_handle) {
            exports.rsp_cb_client_call_hub_handle.map_heartbeats.set(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, cb_heartbeats_obj);
        }
        return cb_heartbeats_obj;
    }
    call_hub(rpc_argv) {
        let _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3 = [];
        _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3.push(rpc_argv);
        this.call_module_method("call_hub", _argv_c06f6974_e54a_3491_ae66_1e1861dd19e3);
    }
}
exports.client_call_hub_caller = client_call_hub_caller;
/*this cb code is codegen by abelkhan for ts*/
class hub_call_client_rsp_cb extends abelkhan.Imodule {
    constructor(modules) {
        super("hub_call_client_rsp_cb");
        modules.reg_module(this);
    }
}
exports.hub_call_client_rsp_cb = hub_call_client_rsp_cb;
exports.rsp_cb_hub_call_client_handle = null;
class hub_call_client_caller extends abelkhan.Icaller {
    uuid_44e0e3b5_d5d3_3ab4_87a3_bdf8d8aefeeb = Math.round(Math.random() * 1000);
    constructor(_ch, modules) {
        super("hub_call_client", _ch);
        if (exports.rsp_cb_hub_call_client_handle == null) {
            exports.rsp_cb_hub_call_client_handle = new hub_call_client_rsp_cb(modules);
        }
    }
    call_client(rpc_argv) {
        let _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab = [];
        _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab.push(rpc_argv);
        this.call_module_method("call_client", _argv_623087d1_9b59_38f3_9ea7_54d2c06e5bab);
    }
}
exports.hub_call_client_caller = hub_call_client_caller;
/*this module code is codegen by abelkhan codegen for typescript*/
class hub_call_hub_reg_hub_rsp extends abelkhan.Icaller {
    uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67;
    constructor(_ch, _uuid) {
        super("hub_call_hub_rsp_cb", _ch);
        this.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67 = _uuid;
    }
    rsp() {
        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [this.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67];
        this.call_module_method("reg_hub_rsp", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
    }
    err() {
        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [this.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67];
        this.call_module_method("reg_hub_err", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
    }
}
exports.hub_call_hub_reg_hub_rsp = hub_call_hub_reg_hub_rsp;
class hub_call_hub_module extends abelkhan.Imodule {
    modules;
    constructor(modules) {
        super("hub_call_hub");
        this.modules = modules;
        this.modules.reg_module(this);
        this.reg_method("reg_hub", this.reg_hub.bind(this));
        this.reg_method("hub_call_hub_mothed", this.hub_call_hub_mothed.bind(this));
        this.cb_reg_hub = null;
        this.cb_hub_call_hub_mothed = null;
    }
    cb_reg_hub;
    reg_hub(inArray) {
        let _cb_uuid = inArray[0];
        let _argv_ = [];
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        this.rsp = new hub_call_hub_reg_hub_rsp(this.current_ch, _cb_uuid);
        if (this.cb_reg_hub) {
            this.cb_reg_hub.apply(null, _argv_);
        }
        this.rsp = null;
    }
    cb_hub_call_hub_mothed;
    hub_call_hub_mothed(inArray) {
        let _argv_ = [];
        _argv_.push(inArray[0]);
        if (this.cb_hub_call_hub_mothed) {
            this.cb_hub_call_hub_mothed.apply(null, _argv_);
        }
    }
}
exports.hub_call_hub_module = hub_call_hub_module;
class gate_call_hub_module extends abelkhan.Imodule {
    modules;
    constructor(modules) {
        super("gate_call_hub");
        this.modules = modules;
        this.modules.reg_module(this);
        this.reg_method("client_disconnect", this.client_disconnect.bind(this));
        this.reg_method("client_exception", this.client_exception.bind(this));
        this.reg_method("client_call_hub", this.client_call_hub.bind(this));
        this.cb_client_disconnect = null;
        this.cb_client_exception = null;
        this.cb_client_call_hub = null;
    }
    cb_client_disconnect;
    client_disconnect(inArray) {
        let _argv_ = [];
        _argv_.push(inArray[0]);
        if (this.cb_client_disconnect) {
            this.cb_client_disconnect.apply(null, _argv_);
        }
    }
    cb_client_exception;
    client_exception(inArray) {
        let _argv_ = [];
        _argv_.push(inArray[0]);
        if (this.cb_client_exception) {
            this.cb_client_exception.apply(null, _argv_);
        }
    }
    cb_client_call_hub;
    client_call_hub(inArray) {
        let _argv_ = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_client_call_hub) {
            this.cb_client_call_hub.apply(null, _argv_);
        }
    }
}
exports.gate_call_hub_module = gate_call_hub_module;
class client_call_hub_heartbeats_rsp extends abelkhan.Icaller {
    uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56;
    constructor(_ch, _uuid) {
        super("client_call_hub_rsp_cb", _ch);
        this.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56 = _uuid;
    }
    rsp(timetmp) {
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = [this.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56];
        _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.push(timetmp);
        this.call_module_method("heartbeats_rsp", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
    }
    err() {
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = [this.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56];
        this.call_module_method("heartbeats_err", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
    }
}
exports.client_call_hub_heartbeats_rsp = client_call_hub_heartbeats_rsp;
class client_call_hub_module extends abelkhan.Imodule {
    modules;
    constructor(modules) {
        super("client_call_hub");
        this.modules = modules;
        this.modules.reg_module(this);
        this.reg_method("connect_hub", this.connect_hub.bind(this));
        this.reg_method("heartbeats", this.heartbeats.bind(this));
        this.reg_method("call_hub", this.call_hub.bind(this));
        this.cb_connect_hub = null;
        this.cb_heartbeats = null;
        this.cb_call_hub = null;
    }
    cb_connect_hub;
    connect_hub(inArray) {
        let _argv_ = [];
        _argv_.push(inArray[0]);
        if (this.cb_connect_hub) {
            this.cb_connect_hub.apply(null, _argv_);
        }
    }
    cb_heartbeats;
    heartbeats(inArray) {
        let _cb_uuid = inArray[0];
        let _argv_ = [];
        this.rsp = new client_call_hub_heartbeats_rsp(this.current_ch, _cb_uuid);
        if (this.cb_heartbeats) {
            this.cb_heartbeats.apply(null, _argv_);
        }
        this.rsp = null;
    }
    cb_call_hub;
    call_hub(inArray) {
        let _argv_ = [];
        _argv_.push(inArray[0]);
        if (this.cb_call_hub) {
            this.cb_call_hub.apply(null, _argv_);
        }
    }
}
exports.client_call_hub_module = client_call_hub_module;
class hub_call_client_module extends abelkhan.Imodule {
    modules;
    constructor(modules) {
        super("hub_call_client");
        this.modules = modules;
        this.modules.reg_module(this);
        this.reg_method("call_client", this.call_client.bind(this));
        this.cb_call_client = null;
    }
    cb_call_client;
    call_client(inArray) {
        let _argv_ = [];
        _argv_.push(inArray[0]);
        if (this.cb_call_client) {
            this.cb_call_client.apply(null, _argv_);
        }
    }
}
exports.hub_call_client_module = hub_call_client_module;
//# sourceMappingURL=hub.js.map