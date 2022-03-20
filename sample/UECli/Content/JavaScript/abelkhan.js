"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports._modulemng = exports.modulemng = exports.Imodule = exports.Icaller = void 0;
const msgpack_1 = require("./msgpack");
class Icaller {
    module_name;
    ch;
    constructor(_module_name, _ch) {
        this.module_name = _module_name;
        this.ch = _ch;
    }
    call_module_method(method_name, argvs) {
        var _event = (0, msgpack_1.encode)([this.module_name, method_name, argvs]);
        var send_data = new Uint8Array(4 + _event.length);
        send_data[0] = _event.length & 0xff;
        send_data[1] = (_event.length >> 8) & 0xff;
        send_data[2] = (_event.length >> 16) & 0xff;
        send_data[3] = (_event.length >> 24) & 0xff;
        send_data.set(_event, 4);
        this.ch.send(send_data);
    }
}
exports.Icaller = Icaller;
class Imodule {
    module_name;
    constructor(_module_name) {
        this.module_name = _module_name;
    }
    methods = new Map();
    reg_method(method_name, method) {
        this.methods.set(method_name, method);
    }
    current_ch = null;
    rsp = null;
    process_event(_ch, _event) {
        this.current_ch = _ch;
        this.methods.get(_event[1]).call(this, _event[2]);
        this.current_ch = null;
    }
}
exports.Imodule = Imodule;
class modulemng {
    module_set = new Map();
    reg_module(_module) {
        this.module_set.set(_module.module_name, _module);
    }
    process_event(_ch, _event) {
        this.module_set.get(_event[0]).process_event(_ch, _event);
    }
}
exports.modulemng = modulemng;
exports._modulemng = new modulemng();
//# sourceMappingURL=abelkhan.js.map