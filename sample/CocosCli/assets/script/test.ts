
import { _decorator, Component, Node, log } from 'cc';
const { ccclass, property } = _decorator;

import * as cli from './client_handle';
import * as test_c2s from './test_c2s';
import * as test_s2c from './test_s2c';

/**
 * Predefined variables
 * Name = Test
 * DateTime = Wed Oct 27 2021 10:06:51 GMT+0800 (中国标准时间)
 * Author = anxiangqq
 * FileBasename = test.ts
 * FileBasenameNoExtension = test
 * URL = db://assets/script/test.ts
 * ManualUrl = https://docs.cocos.com/creator/3.3/manual/en/
 *
 */
 
@ccclass('Test')
export class Test extends Component {
    // [1]
    // dummy = '';
    private _cli:cli.client;

    private _test_c2s_caller:test_c2s.test_c2s_caller;
    private _test_s2c_module:test_s2c.test_s2c_module;

    // [2]
    // @property
    // serializableDummy = 0;

    start () {
        // [3]

        this._cli = new cli.client();
        this._test_c2s_caller = new test_c2s.test_c2s_caller(this._cli);
        
        this._test_s2c_module = new test_s2c.test_s2c_module(this._cli);
        this._test_s2c_module.cb_ping = ()=>{
            console.log("on_ping!");
            let rsp = this._test_s2c_module.rsp as test_s2c.test_s2c_ping_rsp;
            rsp.rsp();
        };

        this._cli.connect_gate("ws://127.0.0.1:3011");
        this._cli.onGateConnect = ()=>{
            console.log("connect gate sucessed!");
            this._cli.get_hub_info("test", (hub_info_list)=>{
                for (let hub_info of hub_info_list){
                    this._test_c2s_caller.get_hub(hub_info.hub_name).login();
                    this._test_c2s_caller.get_hub(hub_info.hub_name).get_svr_host().callBack((ip, port) =>
                    {
                        console.log("get_svr_host sucessed!");
                        console.log("connect_hub name:{0}, type:{1}, ip:{2}, port:{3}!", hub_info.hub_name, hub_info.hub_type, ip, port);
                        this._cli.connect_hub(hub_info.hub_name, hub_info.hub_type, "ws://" + ip + ":" + port);
                    }, () =>
                    {
                        console.log("get_svr_host faild!");
                    }).timeout(3000, ()=> {
                        console.log("get_svr_host timeout!");
                    });
                }

            });
        };
    }

    update (deltaTime: number) {
        // [4]
        this._cli.poll();
    }
}

/**
 * [1] Class member could be defined like this.
 * [2] Use `property` decorator if your want the member to be serializable.
 * [3] Your initialization goes here.
 * [4] Your update function goes here.
 *
 * Learn more about scripting: https://docs.cocos.com/creator/3.3/manual/en/scripting/
 * Learn more about CCClass: https://docs.cocos.com/creator/3.3/manual/en/scripting/ccclass.html
 * Learn more about life-cycle callbacks: https://docs.cocos.com/creator/3.3/manual/en/scripting/life-cycle-callbacks.html
 */
