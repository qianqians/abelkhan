using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private client.Client _client;
    private abelkhan.test_c2s_caller _test_c2s_caller;

    private abelkhan.test_s2c_module _test_s2c_module;

    // Start is called before the first frame update
    void Start()
    {
        _client = new client.Client();
        _test_c2s_caller = new abelkhan.test_c2s_caller(_client);

        _test_s2c_module = new abelkhan.test_s2c_module(_client);
        _test_s2c_module.on_ping += () => {
            Debug.Log("on_ping!");
            var rsp = (abelkhan.test_s2c_ping_rsp)_test_s2c_module.rsp;
            rsp.rsp();
        };

        _client.connect_gate("127.0.0.1", 3001, 3000);
        _client.onGateConnect += () => {
            Debug.Log("connect gate sucessed!");
            _client.get_hub_info("test", (hub_info)=> {
                Debug.Log("get_hub_info sucessed!");
                _test_c2s_caller.get_hub(hub_info.hub_name).login();
                _test_c2s_caller.get_hub(hub_info.hub_name).get_svr_host().callBack((ip, port) =>
                {
                    Debug.Log("get_svr_host sucessed!");
                    Debug.Log(string.Format("connect_hub name:{0}, type:{1}, ip:{2}, port:{3}!", hub_info.hub_name, hub_info.hub_type, ip, port));
                    _client.connect_hub(hub_info.hub_name, hub_info.hub_type, ip, port, 3000);
                }, () =>
                {
                    Debug.Log("get_svr_host faild!");
                }).timeout(3000, ()=> {
                    Debug.Log(string.Format("get_svr_host timeout! name:{0}, type:{1}", hub_info.hub_name, hub_info.hub_type));
                });
            });
        };
        _client.onGateConnectFaild += () => {
            Debug.Log("connect gate faild!");
        };
        _client.onHubConnect += (hub_name) => {
            Debug.Log(string.Format("connect hub:{0} sucessed!", hub_name));
        };
        _client.onHubConnectFaild += (hub_name) => {
            Debug.Log(string.Format("connect hub:{0} faild!", hub_name));
        };

    }

    // Update is called once per frame
    void Update()
    {
        _client.poll();
    }
}
