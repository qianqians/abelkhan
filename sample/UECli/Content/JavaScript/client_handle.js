"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const puerts_1 = require("puerts");
const msgpack_1 = require("./msgpack");
const abelkhan = require("./abelkhan");
const gate = require("./gate");
const hub = require("./hub");
const _client = require("./client");
class imodule {
    constructor() {
        this.methods = new Map();
        this.rsp = null;
    }
    reg_cb(method_name, method) {
        this.methods.set(method_name, method);
    }
    invoke(cb_name, _event) {
        this.methods.get(cb_name).call(this, _event);
    }
}
exports.imodule = imodule;
class modulemng {
    constructor() {
        this.module_set = new Map();
    }
    add_module(module_name, _module) {
        this.module_set.set(module_name, _module);
    }
    process_event(module_name, func_name, _event) {
        this.module_set.get(module_name).invoke(func_name, _event);
    }
}
exports.modulemng = modulemng;
class gateproxy {
    constructor(ch) {
        this.onGateTime = null;
        this.onGateDisconnect = null;
        this._ch = ch;
        this._client_call_gate_caller = new gate.client_call_gate_caller(ch, abelkhan._modulemng);
    }
    heartbeats() {
        let that = this;
        this._client_call_gate_caller.heartbeats().callBack((_svr_timetmp) => {
            if (that.onGateTime) {
                that.onGateTime(_svr_timetmp);
            }
        }, () => { }).timeout(5 * 1000, () => {
            if (that.onGateDisconnect) {
                that.onGateDisconnect(this._ch);
            }
        });
    }
    get_hub_info(hub_type, cb) {
        let that = this;
        this._client_call_gate_caller.get_hub_info(hub_type).callBack((hub_info) => {
            cb(hub_info);
        }, () => { }).timeout(5 * 1000, () => {
            that.onGateDisconnect(this._ch);
        });
    }
    call_hub(hub, module, func, argv) {
        let _event = [module, func, argv];
        this._client_call_gate_caller.forward_client_call_hub(hub, msgpack_1.encode(_event));
    }
}
class hubproxy {
    constructor(hub_name, hub_type, ch) {
        this.onHubTime = null;
        this.onHubDisconnect = null;
        this._hub_name = hub_name;
        this._hub_type = hub_type;
        this._ch = ch;
        this._client_call_hub_caller = new hub.client_call_hub_caller(ch, abelkhan._modulemng);
    }
    connect_hub(cuuid) {
        this._client_call_hub_caller.connect_hub(cuuid);
    }
    heartbeats() {
        let that = this;
        this._client_call_hub_caller.heartbeats().callBack((_hub_timetmp) => {
            if (that.onHubTime) {
                that.onHubTime(that._hub_name, _hub_timetmp);
            }
        }, () => { }).timeout(5 * 1000, () => {
            if (that.onHubDisconnect) {
                that.onHubDisconnect(that._ch);
            }
        });
    }
    call_hub(module, func, argv) {
        let _event = [module, func, argv];
        this._client_call_hub_caller.call_hub(msgpack_1.encode(_event));
    }
}
class wschannel {
    constructor(_ws, _net_handle) {
        this.events = [];
        this.offset = 0;
        this.data = null;
        this.ws = _ws;
        this.net_handle = _net_handle;
    }
    recv(evt) {
        let u8data = new Uint8Array(evt);
        var new_data = new Uint8Array(this.offset + u8data.byteLength);
        if (this.data !== null) {
            new_data.set(this.data);
        }
        new_data.set(u8data, this.offset);
        while (new_data.length > 4) {
            var len = new_data[0] | new_data[1] << 8 | new_data[2] << 16 | new_data[3] << 24;
            if ((len + 4) > new_data.length) {
                break;
            }
            var key0 = new_data[0];
            var key1 = new_data[1];
            if (key1 == 0) {
                key1 = (key0 + key0 % 3) & 0xff;
            }
            var key2 = new_data[2];
            if (key2 == 0) {
                key2 = (key0 + key0 % 5) & 0xff;
            }
            var key3 = new_data[3];
            if (key3 == 0) {
                key3 = (key0 + key0 % 7) & 0xff;
            }
            var str_bytes = new_data.subarray(4, (len + 4));
            let i = 0;
            while (i < str_bytes.length) {
                if ((i % 4) == 0) {
                    str_bytes[i] ^= key0;
                }
                else if ((i % 4) == 1) {
                    str_bytes[i] ^= key1;
                }
                else if ((i % 4) == 2) {
                    str_bytes[i] ^= key2;
                }
                else if ((i % 4) == 3) {
                    str_bytes[i] ^= key3;
                }
                i++;
            }
            try {
                let ev = msgpack_1.decode(str_bytes);
                this.events.push(ev);
            }
            catch (e) {
                console.log(e);
            }
            if (new_data.length > (len + 4)) {
                var _data = new Uint8Array(new_data.length - (len + 4));
                _data.set(new_data.subarray(len + 4));
                new_data = _data;
            }
            else {
                new_data = null;
                break;
            }
        }
        this.data = new_data;
        if (new_data !== null) {
            this.offset = new_data.length;
        }
        else {
            this.offset = 0;
        }
    }
    send(send_data) {
        var key0 = send_data[0];
        var key1 = send_data[1];
        if (key1 == 0) {
            key1 = (key0 + key0 % 3) & 0xff;
        }
        var key2 = send_data[2];
        if (key2 == 0) {
            key2 = (key0 + key0 % 5) & 0xff;
        }
        var key3 = send_data[3];
        if (key3 == 0) {
            key3 = (key0 + key0 % 7) & 0xff;
        }
        let i = 4;
        while (i < send_data.length) {
            if ((i % 4) == 0) {
                send_data[i] ^= key0;
            }
            else if ((i % 4) == 1) {
                send_data[i] ^= key1;
            }
            else if ((i % 4) == 2) {
                send_data[i] ^= key2;
            }
            else if ((i % 4) == 3) {
                send_data[i] ^= key3;
            }
            i++;
        }
        this.net_handle.Send(this.ws, send_data.buffer);
    }
    disconnect() {
    }
    pop() {
        if (this.events.length === 0) {
            return null;
        }
        return this.events.shift();
    }
}
exports.wschannel = wschannel;
class client {
    constructor(net_instance) {
        this.onGateDisConnect = null;
        this.onHubDisConnect = null;
        this.onGateTime = null;
        this.onHubTime = null;
        this._gateproxy = null;
        this._modulemng = new modulemng();
        this._hubproxy_set = new Map();
        this._ch_hubproxy_set = new Map();
        this._conn = net_instance;
        this._conn.NotifyNetMsg.Bind((s, buffer) => {
            let ch = this._map_ch.get(s);
            if (ch) {
                ch.recv(puerts_1.$unref(buffer));
                while (true) {
                    let ev = ch.pop();
                    if (!ev) {
                        break;
                    }
                    abelkhan._modulemng.process_event(ch, ev);
                }
            }
        });
        this._map_ch = new Map();
        setInterval(() => {
            this.heartbeats();
        }, 5 * 1000);
        this._gate_call_client_module = new _client.gate_call_client_module(abelkhan._modulemng);
        this._gate_call_client_module.cb_ntf_cuuid = this.ntf_cuuid.bind(this);
        this._gate_call_client_module.cb_call_client = this.gate_call_client.bind(this);
        this._hub_call_client_module = new hub.hub_call_client_module(abelkhan._modulemng);
        this._hub_call_client_module.cb_call_client = this.hub_call_client.bind(this);
    }
    ntf_cuuid(_uuid) {
        this.uuid = _uuid;
        if (this.onGateConnect) {
            this.onGateConnect();
        }
    }
    gate_call_client(hub_name, rpc_argv) {
        let _event = msgpack_1.decode(rpc_argv);
        let module = _event[0];
        let func = _event[1];
        let argvs = _event[2];
        this.current_hub = hub_name;
        this._modulemng.process_event(module, func, argvs);
        this.current_hub = "";
    }
    hub_call_client(rpc_argv) {
        let _event = msgpack_1.decode(rpc_argv);
        let module = _event[0];
        let func = _event[1];
        let argvs = _event[2];
        let _hubproxy = this._ch_hubproxy_set.get(this._hub_call_client_module.current_ch);
        this.current_hub = _hubproxy._hub_name;
        this._modulemng.process_event(module, func, argvs);
        this.current_hub = "";
    }
    heartbeats() {
        if (this._gateproxy) {
            this._gateproxy.heartbeats();
        }
        this._hubproxy_set.forEach((value, _) => {
            value.heartbeats();
        });
    }
    get_hub_info(hub_type, cb) {
        if (this._gateproxy) {
            this._gateproxy.get_hub_info(hub_type, cb);
        }
    }
    call_hub(hub_name, module, func, argv) {
        if (this._hubproxy_set.has(hub_name)) {
            let _hubproxy = this._hubproxy_set.get(hub_name);
            _hubproxy.call_hub(module, func, argv);
            return;
        }
        if (this._gateproxy != null) {
            this._gateproxy.call_hub(hub_name, module, func, argv);
        }
    }
    connect_gate(ip, port) {
        let s = this._conn.Connect(ip, port);
        if (s < 0) {
            return false;
        }
        let ch = new wschannel(s, this._conn);
        console.log("connect gate ip:" + ip + " port:" + port);
        this._map_ch.set(s, ch);
        this._gateproxy = new gateproxy(ch);
        this._gateproxy.onGateDisconnect = (ch) => {
            this._map_ch.delete(s);
            this._gateproxy = null;
            if (this.onGateDisConnect) {
                this.onGateDisConnect();
            }
        };
        this._gateproxy.onGateTime = (tick) => {
            if (this.onGateTime) {
                this.onGateTime(tick);
            }
        };
        return true;
    }
    connect_hub(hub_name, hub_type, ip, port) {
        let s = this._conn.Connect(ip, port);
        let ch = new wschannel(s, this._conn);
        console.log("connect hub ip:" + ip + " port:" + port);
        this._map_ch.set(s, ch);
        let _hubproxy = new hubproxy(hub_name, hub_type, ch);
        _hubproxy.onHubDisconnect = (ch) => {
            this._map_ch.delete(s);
            if (this._ch_hubproxy_set.has(ch)) {
                let _proxy = this._ch_hubproxy_set.get(ch);
                this._hubproxy_set.delete(_proxy._hub_name);
                this._ch_hubproxy_set.delete(ch);
            }
            if (this.onHubDisConnect) {
                this.onHubDisConnect(hub_name);
            }
        };
        _hubproxy.onHubTime = (hub_name, tick) => {
            if (this.onHubTime) {
                this.onHubTime(hub_name, tick);
            }
        };
        _hubproxy.connect_hub(this.uuid);
        this._hubproxy_set.set(hub_name, _hubproxy);
        this._ch_hubproxy_set.set(ch, _hubproxy);
        if (this.onHubConnect) {
            this.onHubConnect(hub_name);
        }
    }
}
exports.client = client;
//# sourceMappingURL=client_handle.js.map