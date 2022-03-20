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
 
export class Test {
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

        
    }

    update (deltaTime: number) {
        // [4]
        
    }
}