"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.TsNetGameInstance = void 0;
const UE = require("ue");
const cli = require("./client_handle");
const test_c2s = require("./test_c2s");
const test_s2c = require("./test_s2c");
class TsNetGameInstance extends UE.TickGameInstance {
    _cli;
    _test_c2s_caller;
    _test_s2c_module;
    ReceiveInit() {
        console.log("TsNetGameInstance Init!");
        super.ReceiveInit();
        this.InitNet();
    }
    InitNet() {
        console.log("TsNetGameInstance InitNet!");
        this._cli = new cli.client(this);
        this._test_c2s_caller = new test_c2s.test_c2s_caller(this._cli);
        this._test_s2c_module = new test_s2c.test_s2c_module(this._cli);
        this._test_s2c_module.cb_ping = () => {
            console.log("on_ping!");
            let rsp = this._test_s2c_module.rsp;
            rsp.rsp();
        };
        if (this._cli.connect_gate("127.0.0.1", 3001)) {
            this._cli.onGateConnect = () => {
                console.log("connect gate sucessed!");
                this._cli.get_hub_info("test", (hub_info_list) => {
                    for (let hub_info of hub_info_list) {
                        this._test_c2s_caller.get_hub(hub_info.hub_name).login();
                        this._test_c2s_caller.get_hub(hub_info.hub_name).get_svr_host().callBack((ip, port) => {
                            console.log("get_svr_host sucessed!");
                            console.log("connect_hub name:{0}, type:{1}, ip:{2}, port:{3}!", hub_info.hub_name, hub_info.hub_type, ip, port);
                            this._cli.connect_hub(hub_info.hub_name, hub_info.hub_type, ip, port);
                        }, () => {
                            console.log("get_svr_host faild!");
                        }).timeout(3000, () => {
                            console.log("get_svr_host timeout!");
                        });
                    }
                });
            };
        }
    }
}
exports.TsNetGameInstance = TsNetGameInstance;
exports.default = TsNetGameInstance;
//# sourceMappingURL=TsNetGameInstance.js.map