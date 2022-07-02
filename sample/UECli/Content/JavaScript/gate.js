"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.hub_call_gate_module = exports.hub_call_gate_reverse_reg_client_hub_rsp = exports.hub_call_gate_reg_hub_rsp = exports.client_call_gate_module = exports.client_call_gate_get_hub_info_rsp = exports.client_call_gate_heartbeats_rsp = exports.hub_call_gate_caller = exports.rsp_cb_hub_call_gate_handle = exports.hub_call_gate_rsp_cb = exports.hub_call_gate_reverse_reg_client_hub_cb = exports.hub_call_gate_reg_hub_cb = exports.client_call_gate_caller = exports.rsp_cb_client_call_gate_handle = exports.client_call_gate_rsp_cb = exports.client_call_gate_get_hub_info_cb = exports.client_call_gate_heartbeats_cb = exports.protcol_to_hub_info = exports.hub_info_to_protcol = exports.hub_info = void 0;
const abelkhan = require("./abelkhan");
/*this enum code is codegen by abelkhan codegen for ts*/
/*this struct code is codegen by abelkhan codegen for typescript*/
class hub_info {
    hub_name;
    hub_type;
    constructor() {
    }
}
exports.hub_info = hub_info;
function hub_info_to_protcol(_struct) {
    return _struct;
}
exports.hub_info_to_protcol = hub_info_to_protcol;
function protcol_to_hub_info(_protocol) {
    let _struct = new hub_info();
    for (const [key, val] of Object.entries(_protocol))
        if (key === "hub_name") {
            _struct.hub_name = val;
        }
        else if (key === "hub_type") {
            _struct.hub_type = val;
        }
    return _struct;
}
exports.protcol_to_hub_info = protcol_to_hub_info;
/*this caller code is codegen by abelkhan codegen for typescript*/
class client_call_gate_heartbeats_cb {
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
exports.client_call_gate_heartbeats_cb = client_call_gate_heartbeats_cb;
class client_call_gate_get_hub_info_cb {
    cb_uuid;
    module_rsp_cb;
    event_get_hub_info_handle_cb;
    event_get_hub_info_handle_err;
    event_get_hub_info_handle_timeout;
    constructor(_cb_uuid, _module_rsp_cb) {
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_get_hub_info_handle_cb = null;
        this.event_get_hub_info_handle_err = null;
        this.event_get_hub_info_handle_timeout = null;
    }
    callBack(_cb, _err) {
        this.event_get_hub_info_handle_cb = _cb;
        this.event_get_hub_info_handle_err = _err;
        return this;
    }
    timeout(tick, timeout_cb) {
        setTimeout(() => { this.module_rsp_cb.get_hub_info_timeout(this.cb_uuid); }, tick);
        this.event_get_hub_info_handle_timeout = timeout_cb;
    }
}
exports.client_call_gate_get_hub_info_cb = client_call_gate_get_hub_info_cb;
/*this cb code is codegen by abelkhan for ts*/
class client_call_gate_rsp_cb extends abelkhan.Imodule {
    map_heartbeats;
    map_get_hub_info;
    constructor(modules) {
        super("client_call_gate_rsp_cb");
        this.map_heartbeats = new Map();
        modules.reg_method("client_call_gate_rsp_cb_heartbeats_rsp", [this, this.heartbeats_rsp.bind(this)]);
        modules.reg_method("client_call_gate_rsp_cb_heartbeats_err", [this, this.heartbeats_err.bind(this)]);
        this.map_get_hub_info = new Map();
        modules.reg_method("client_call_gate_rsp_cb_get_hub_info_rsp", [this, this.get_hub_info_rsp.bind(this)]);
        modules.reg_method("client_call_gate_rsp_cb_get_hub_info_err", [this, this.get_hub_info_err.bind(this)]);
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
    get_hub_info_rsp(inArray) {
        let uuid = inArray[0];
        let _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24 = [];
        let _array_53b78086_1765_5879_87b4_63333838766a = [];
        for (let v_447f0ba1_16b5_5b5f_b754_ee583fc6f6a8 of inArray[1]) {
            _array_53b78086_1765_5879_87b4_63333838766a.push(protcol_to_hub_info(v_447f0ba1_16b5_5b5f_b754_ee583fc6f6a8));
        }
        _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.push(_array_53b78086_1765_5879_87b4_63333838766a);
        var rsp = this.try_get_and_del_get_hub_info_cb(uuid);
        if (rsp && rsp.event_get_hub_info_handle_cb) {
            rsp.event_get_hub_info_handle_cb.apply(null, _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
        }
    }
    get_hub_info_err(inArray) {
        let uuid = inArray[0];
        let _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24 = [];
        var rsp = this.try_get_and_del_get_hub_info_cb(uuid);
        if (rsp && rsp.event_get_hub_info_handle_err) {
            rsp.event_get_hub_info_handle_err.apply(null, _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
        }
    }
    get_hub_info_timeout(cb_uuid) {
        let rsp = this.try_get_and_del_get_hub_info_cb(cb_uuid);
        if (rsp) {
            if (rsp.event_get_hub_info_handle_timeout) {
                rsp.event_get_hub_info_handle_timeout.apply(null);
            }
        }
    }
    try_get_and_del_get_hub_info_cb(uuid) {
        var rsp = this.map_get_hub_info.get(uuid);
        this.map_get_hub_info.delete(uuid);
        return rsp;
    }
}
exports.client_call_gate_rsp_cb = client_call_gate_rsp_cb;
exports.rsp_cb_client_call_gate_handle = null;
class client_call_gate_caller extends abelkhan.Icaller {
    uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2 = Math.round(Math.random() * 1000);
    constructor(_ch, modules) {
        super("client_call_gate", _ch);
        if (exports.rsp_cb_client_call_gate_handle == null) {
            exports.rsp_cb_client_call_gate_handle = new client_call_gate_rsp_cb(modules);
        }
    }
    heartbeats() {
        let uuid_a514ca5f_2c67_5668_aac0_354397bdce36 = Math.round(this.uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2++);
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = [uuid_a514ca5f_2c67_5668_aac0_354397bdce36];
        this.call_module_method("client_call_gate_heartbeats", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
        let cb_heartbeats_obj = new client_call_gate_heartbeats_cb(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, exports.rsp_cb_client_call_gate_handle);
        if (exports.rsp_cb_client_call_gate_handle) {
            exports.rsp_cb_client_call_gate_handle.map_heartbeats.set(uuid_a514ca5f_2c67_5668_aac0_354397bdce36, cb_heartbeats_obj);
        }
        return cb_heartbeats_obj;
    }
    get_hub_info(hub_type) {
        let uuid_e9d2753f_7d38_512d_80ff_7aae13508048 = Math.round(this.uuid_2a41ded1_acf2_3b8c_95bc_f149a01703b2++);
        let _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24 = [uuid_e9d2753f_7d38_512d_80ff_7aae13508048];
        _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.push(hub_type);
        this.call_module_method("client_call_gate_get_hub_info", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
        let cb_get_hub_info_obj = new client_call_gate_get_hub_info_cb(uuid_e9d2753f_7d38_512d_80ff_7aae13508048, exports.rsp_cb_client_call_gate_handle);
        if (exports.rsp_cb_client_call_gate_handle) {
            exports.rsp_cb_client_call_gate_handle.map_get_hub_info.set(uuid_e9d2753f_7d38_512d_80ff_7aae13508048, cb_get_hub_info_obj);
        }
        return cb_get_hub_info_obj;
    }
    forward_client_call_hub(hub_name, rpc_argv) {
        let _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5 = [];
        _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5.push(hub_name);
        _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5.push(rpc_argv);
        this.call_module_method("client_call_gate_forward_client_call_hub", _argv_eb5e7a5e_3532_32ad_81f9_9b27aa6833e5);
    }
}
exports.client_call_gate_caller = client_call_gate_caller;
class hub_call_gate_reg_hub_cb {
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
exports.hub_call_gate_reg_hub_cb = hub_call_gate_reg_hub_cb;
class hub_call_gate_reverse_reg_client_hub_cb {
    cb_uuid;
    module_rsp_cb;
    event_reverse_reg_client_hub_handle_cb;
    event_reverse_reg_client_hub_handle_err;
    event_reverse_reg_client_hub_handle_timeout;
    constructor(_cb_uuid, _module_rsp_cb) {
        this.cb_uuid = _cb_uuid;
        this.module_rsp_cb = _module_rsp_cb;
        this.event_reverse_reg_client_hub_handle_cb = null;
        this.event_reverse_reg_client_hub_handle_err = null;
        this.event_reverse_reg_client_hub_handle_timeout = null;
    }
    callBack(_cb, _err) {
        this.event_reverse_reg_client_hub_handle_cb = _cb;
        this.event_reverse_reg_client_hub_handle_err = _err;
        return this;
    }
    timeout(tick, timeout_cb) {
        setTimeout(() => { this.module_rsp_cb.reverse_reg_client_hub_timeout(this.cb_uuid); }, tick);
        this.event_reverse_reg_client_hub_handle_timeout = timeout_cb;
    }
}
exports.hub_call_gate_reverse_reg_client_hub_cb = hub_call_gate_reverse_reg_client_hub_cb;
/*this cb code is codegen by abelkhan for ts*/
class hub_call_gate_rsp_cb extends abelkhan.Imodule {
    map_reg_hub;
    map_reverse_reg_client_hub;
    constructor(modules) {
        super("hub_call_gate_rsp_cb");
        this.map_reg_hub = new Map();
        modules.reg_method("hub_call_gate_rsp_cb_reg_hub_rsp", [this, this.reg_hub_rsp.bind(this)]);
        modules.reg_method("hub_call_gate_rsp_cb_reg_hub_err", [this, this.reg_hub_err.bind(this)]);
        this.map_reverse_reg_client_hub = new Map();
        modules.reg_method("hub_call_gate_rsp_cb_reverse_reg_client_hub_rsp", [this, this.reverse_reg_client_hub_rsp.bind(this)]);
        modules.reg_method("hub_call_gate_rsp_cb_reverse_reg_client_hub_err", [this, this.reverse_reg_client_hub_err.bind(this)]);
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
    reverse_reg_client_hub_rsp(inArray) {
        let uuid = inArray[0];
        let _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c = [];
        var rsp = this.try_get_and_del_reverse_reg_client_hub_cb(uuid);
        if (rsp && rsp.event_reverse_reg_client_hub_handle_cb) {
            rsp.event_reverse_reg_client_hub_handle_cb.apply(null, _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c);
        }
    }
    reverse_reg_client_hub_err(inArray) {
        let uuid = inArray[0];
        let _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c = [];
        _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c.push(inArray[1]);
        var rsp = this.try_get_and_del_reverse_reg_client_hub_cb(uuid);
        if (rsp && rsp.event_reverse_reg_client_hub_handle_err) {
            rsp.event_reverse_reg_client_hub_handle_err.apply(null, _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c);
        }
    }
    reverse_reg_client_hub_timeout(cb_uuid) {
        let rsp = this.try_get_and_del_reverse_reg_client_hub_cb(cb_uuid);
        if (rsp) {
            if (rsp.event_reverse_reg_client_hub_handle_timeout) {
                rsp.event_reverse_reg_client_hub_handle_timeout.apply(null);
            }
        }
    }
    try_get_and_del_reverse_reg_client_hub_cb(uuid) {
        var rsp = this.map_reverse_reg_client_hub.get(uuid);
        this.map_reverse_reg_client_hub.delete(uuid);
        return rsp;
    }
}
exports.hub_call_gate_rsp_cb = hub_call_gate_rsp_cb;
exports.rsp_cb_hub_call_gate_handle = null;
class hub_call_gate_caller extends abelkhan.Icaller {
    uuid_9796175c_1119_3833_bf31_5ee139b40edc = Math.round(Math.random() * 1000);
    constructor(_ch, modules) {
        super("hub_call_gate", _ch);
        if (exports.rsp_cb_hub_call_gate_handle == null) {
            exports.rsp_cb_hub_call_gate_handle = new hub_call_gate_rsp_cb(modules);
        }
    }
    reg_hub(hub_name, hub_type) {
        let uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106 = Math.round(this.uuid_9796175c_1119_3833_bf31_5ee139b40edc++);
        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106];
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.push(hub_name);
        _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7.push(hub_type);
        this.call_module_method("hub_call_gate_reg_hub", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
        let cb_reg_hub_obj = new hub_call_gate_reg_hub_cb(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, exports.rsp_cb_hub_call_gate_handle);
        if (exports.rsp_cb_hub_call_gate_handle) {
            exports.rsp_cb_hub_call_gate_handle.map_reg_hub.set(uuid_98c51fef_38ce_530a_b8e9_1adcd50b1106, cb_reg_hub_obj);
        }
        return cb_reg_hub_obj;
    }
    reverse_reg_client_hub(client_uuid) {
        let uuid_5352b179_7aef_5875_a08f_06381972529f = Math.round(this.uuid_9796175c_1119_3833_bf31_5ee139b40edc++);
        let _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c = [uuid_5352b179_7aef_5875_a08f_06381972529f];
        _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c.push(client_uuid);
        this.call_module_method("hub_call_gate_reverse_reg_client_hub", _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c);
        let cb_reverse_reg_client_hub_obj = new hub_call_gate_reverse_reg_client_hub_cb(uuid_5352b179_7aef_5875_a08f_06381972529f, exports.rsp_cb_hub_call_gate_handle);
        if (exports.rsp_cb_hub_call_gate_handle) {
            exports.rsp_cb_hub_call_gate_handle.map_reverse_reg_client_hub.set(uuid_5352b179_7aef_5875_a08f_06381972529f, cb_reverse_reg_client_hub_obj);
        }
        return cb_reverse_reg_client_hub_obj;
    }
    unreg_client_hub(client_uuid) {
        let _argv_3567e5c7_8e81_35c5_a6b6_c22d8e655aae = [];
        _argv_3567e5c7_8e81_35c5_a6b6_c22d8e655aae.push(client_uuid);
        this.call_module_method("hub_call_gate_unreg_client_hub", _argv_3567e5c7_8e81_35c5_a6b6_c22d8e655aae);
    }
    disconnect_client(client_uuid) {
        let _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85 = [];
        _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85.push(client_uuid);
        this.call_module_method("hub_call_gate_disconnect_client", _argv_4a07b4a0_1928_3c70_bef9_f3790d8c9a85);
    }
    forward_hub_call_client(client_uuid, rpc_argv) {
        let _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1 = [];
        _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1.push(client_uuid);
        _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1.push(rpc_argv);
        this.call_module_method("hub_call_gate_forward_hub_call_client", _argv_7d7a3c95_d5f5_386c_9a43_eadf3c9912b1);
    }
    forward_hub_call_group_client(client_uuids, rpc_argv) {
        let _argv_374b012d_0718_3f84_91f0_784b12f8189c = [];
        let _array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1 = [];
        for (let v_dfd11414_89c9_5adb_8977_69b93b30195b of client_uuids) {
            _array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1.push(v_dfd11414_89c9_5adb_8977_69b93b30195b);
        }
        _argv_374b012d_0718_3f84_91f0_784b12f8189c.push(_array_0b370787_bccf_3fe3_a7ff_d9e69112f3e1);
        _argv_374b012d_0718_3f84_91f0_784b12f8189c.push(rpc_argv);
        this.call_module_method("hub_call_gate_forward_hub_call_group_client", _argv_374b012d_0718_3f84_91f0_784b12f8189c);
    }
    forward_hub_call_global_client(rpc_argv) {
        let _argv_f69241c3_642a_3b51_bb37_cf638176493a = [];
        _argv_f69241c3_642a_3b51_bb37_cf638176493a.push(rpc_argv);
        this.call_module_method("hub_call_gate_forward_hub_call_global_client", _argv_f69241c3_642a_3b51_bb37_cf638176493a);
    }
}
exports.hub_call_gate_caller = hub_call_gate_caller;
/*this module code is codegen by abelkhan codegen for typescript*/
class client_call_gate_heartbeats_rsp extends abelkhan.Icaller {
    uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56;
    constructor(_ch, _uuid) {
        super("client_call_gate_rsp_cb", _ch);
        this.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56 = _uuid;
    }
    rsp(timetmp) {
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = [this.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56];
        _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4.push(timetmp);
        this.call_module_method("client_call_gate_rsp_cb_heartbeats_rsp", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
    }
    err() {
        let _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4 = [this.uuid_2c1e76dd_8bad_3bd6_a208_e15a8eb56f56];
        this.call_module_method("client_call_gate_rsp_cb_heartbeats_err", _argv_6fbd85be_a054_37ed_b3ea_cced2f90fda4);
    }
}
exports.client_call_gate_heartbeats_rsp = client_call_gate_heartbeats_rsp;
class client_call_gate_get_hub_info_rsp extends abelkhan.Icaller {
    uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b;
    constructor(_ch, _uuid) {
        super("client_call_gate_rsp_cb", _ch);
        this.uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b = _uuid;
    }
    rsp(hub_info) {
        let _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24 = [this.uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b];
        let _array_4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2 = [];
        for (let v_72192cc7_d049_3653_a25b_4eaf8d18d7e2 of hub_info) {
            _array_4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2.push(hub_info_to_protcol(v_72192cc7_d049_3653_a25b_4eaf8d18d7e2));
        }
        _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24.push(_array_4ca94c1e_3083_3fe9_a4f0_b4f03b01b0f2);
        this.call_module_method("client_call_gate_rsp_cb_get_hub_info_rsp", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
    }
    err() {
        let _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24 = [this.uuid_db7b7f0f_c3d0_380b_b51e_53fea108bc3b];
        this.call_module_method("client_call_gate_rsp_cb_get_hub_info_err", _argv_64f76bda_d44d_3aed_a6a4_d85fea361e24);
    }
}
exports.client_call_gate_get_hub_info_rsp = client_call_gate_get_hub_info_rsp;
class client_call_gate_module extends abelkhan.Imodule {
    modules;
    constructor(modules) {
        super("client_call_gate");
        this.modules = modules;
        this.modules.reg_method("client_call_gate_heartbeats", [this, this.heartbeats.bind(this)]);
        this.modules.reg_method("client_call_gate_get_hub_info", [this, this.get_hub_info.bind(this)]);
        this.modules.reg_method("client_call_gate_forward_client_call_hub", [this, this.forward_client_call_hub.bind(this)]);
        this.cb_heartbeats = null;
        this.cb_get_hub_info = null;
        this.cb_forward_client_call_hub = null;
    }
    cb_heartbeats;
    heartbeats(inArray) {
        let _cb_uuid = inArray[0];
        let _argv_ = [];
        this.rsp = new client_call_gate_heartbeats_rsp(this.current_ch, _cb_uuid);
        if (this.cb_heartbeats) {
            this.cb_heartbeats.apply(null, _argv_);
        }
        this.rsp = null;
    }
    cb_get_hub_info;
    get_hub_info(inArray) {
        let _cb_uuid = inArray[0];
        let _argv_ = [];
        _argv_.push(inArray[1]);
        this.rsp = new client_call_gate_get_hub_info_rsp(this.current_ch, _cb_uuid);
        if (this.cb_get_hub_info) {
            this.cb_get_hub_info.apply(null, _argv_);
        }
        this.rsp = null;
    }
    cb_forward_client_call_hub;
    forward_client_call_hub(inArray) {
        let _argv_ = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_forward_client_call_hub) {
            this.cb_forward_client_call_hub.apply(null, _argv_);
        }
    }
}
exports.client_call_gate_module = client_call_gate_module;
class hub_call_gate_reg_hub_rsp extends abelkhan.Icaller {
    uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67;
    constructor(_ch, _uuid) {
        super("hub_call_gate_rsp_cb", _ch);
        this.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67 = _uuid;
    }
    rsp() {
        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [this.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67];
        this.call_module_method("hub_call_gate_rsp_cb_reg_hub_rsp", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
    }
    err() {
        let _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7 = [this.uuid_d47a6c8a_5494_35bb_9bc5_60d20f624f67];
        this.call_module_method("hub_call_gate_rsp_cb_reg_hub_err", _argv_e096e269_1e08_36d1_9ba4_b7db8c8ff8a7);
    }
}
exports.hub_call_gate_reg_hub_rsp = hub_call_gate_reg_hub_rsp;
class hub_call_gate_reverse_reg_client_hub_rsp extends abelkhan.Icaller {
    uuid_ef84ff12_6e4a_39cd_896e_27f3ac82fa1a;
    constructor(_ch, _uuid) {
        super("hub_call_gate_rsp_cb", _ch);
        this.uuid_ef84ff12_6e4a_39cd_896e_27f3ac82fa1a = _uuid;
    }
    rsp() {
        let _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c = [this.uuid_ef84ff12_6e4a_39cd_896e_27f3ac82fa1a];
        this.call_module_method("hub_call_gate_rsp_cb_reverse_reg_client_hub_rsp", _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c);
    }
    err(err) {
        let _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c = [this.uuid_ef84ff12_6e4a_39cd_896e_27f3ac82fa1a];
        _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c.push(err);
        this.call_module_method("hub_call_gate_rsp_cb_reverse_reg_client_hub_err", _argv_03d844bd_f79a_3179_8f8b_9f0ed380f60c);
    }
}
exports.hub_call_gate_reverse_reg_client_hub_rsp = hub_call_gate_reverse_reg_client_hub_rsp;
class hub_call_gate_module extends abelkhan.Imodule {
    modules;
    constructor(modules) {
        super("hub_call_gate");
        this.modules = modules;
        this.modules.reg_method("hub_call_gate_reg_hub", [this, this.reg_hub.bind(this)]);
        this.modules.reg_method("hub_call_gate_reverse_reg_client_hub", [this, this.reverse_reg_client_hub.bind(this)]);
        this.modules.reg_method("hub_call_gate_unreg_client_hub", [this, this.unreg_client_hub.bind(this)]);
        this.modules.reg_method("hub_call_gate_disconnect_client", [this, this.disconnect_client.bind(this)]);
        this.modules.reg_method("hub_call_gate_forward_hub_call_client", [this, this.forward_hub_call_client.bind(this)]);
        this.modules.reg_method("hub_call_gate_forward_hub_call_group_client", [this, this.forward_hub_call_group_client.bind(this)]);
        this.modules.reg_method("hub_call_gate_forward_hub_call_global_client", [this, this.forward_hub_call_global_client.bind(this)]);
        this.cb_reg_hub = null;
        this.cb_reverse_reg_client_hub = null;
        this.cb_unreg_client_hub = null;
        this.cb_disconnect_client = null;
        this.cb_forward_hub_call_client = null;
        this.cb_forward_hub_call_group_client = null;
        this.cb_forward_hub_call_global_client = null;
    }
    cb_reg_hub;
    reg_hub(inArray) {
        let _cb_uuid = inArray[0];
        let _argv_ = [];
        _argv_.push(inArray[1]);
        _argv_.push(inArray[2]);
        this.rsp = new hub_call_gate_reg_hub_rsp(this.current_ch, _cb_uuid);
        if (this.cb_reg_hub) {
            this.cb_reg_hub.apply(null, _argv_);
        }
        this.rsp = null;
    }
    cb_reverse_reg_client_hub;
    reverse_reg_client_hub(inArray) {
        let _cb_uuid = inArray[0];
        let _argv_ = [];
        _argv_.push(inArray[1]);
        this.rsp = new hub_call_gate_reverse_reg_client_hub_rsp(this.current_ch, _cb_uuid);
        if (this.cb_reverse_reg_client_hub) {
            this.cb_reverse_reg_client_hub.apply(null, _argv_);
        }
        this.rsp = null;
    }
    cb_unreg_client_hub;
    unreg_client_hub(inArray) {
        let _argv_ = [];
        _argv_.push(inArray[0]);
        if (this.cb_unreg_client_hub) {
            this.cb_unreg_client_hub.apply(null, _argv_);
        }
    }
    cb_disconnect_client;
    disconnect_client(inArray) {
        let _argv_ = [];
        _argv_.push(inArray[0]);
        if (this.cb_disconnect_client) {
            this.cb_disconnect_client.apply(null, _argv_);
        }
    }
    cb_forward_hub_call_client;
    forward_hub_call_client(inArray) {
        let _argv_ = [];
        _argv_.push(inArray[0]);
        _argv_.push(inArray[1]);
        if (this.cb_forward_hub_call_client) {
            this.cb_forward_hub_call_client.apply(null, _argv_);
        }
    }
    cb_forward_hub_call_group_client;
    forward_hub_call_group_client(inArray) {
        let _argv_ = [];
        let _array_ = [];
        for (let v_ of inArray[0]) {
            _array_.push(v_);
        }
        _argv_.push(_array_);
        _argv_.push(inArray[1]);
        if (this.cb_forward_hub_call_group_client) {
            this.cb_forward_hub_call_group_client.apply(null, _argv_);
        }
    }
    cb_forward_hub_call_global_client;
    forward_hub_call_global_client(inArray) {
        let _argv_ = [];
        _argv_.push(inArray[0]);
        if (this.cb_forward_hub_call_global_client) {
            this.cb_forward_hub_call_global_client.apply(null, _argv_);
        }
    }
}
exports.hub_call_gate_module = hub_call_gate_module;
//# sourceMappingURL=gate.js.map