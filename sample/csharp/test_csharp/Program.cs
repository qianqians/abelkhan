using System;
using System.Collections.Generic;
using System.Threading;

namespace test_csharp
{
    class Program
    {
        private static List<string> client_list;
        private static abelkhan.test_s2c_caller _test_s2c_caller;

        static void heartbeat(long _) {
            foreach (var cuuid in client_list)
            {
                _test_s2c_caller.get_client(cuuid).ping().callBack(() =>
                {
                    log.log.trace("ping client:{0} sucessed!", cuuid);
                }, () =>
                {
                    log.log.err("ping client:{0} faild!", cuuid);
                }).timeout(3000, () => {
                    log.log.err("ping client:{0} timeout!", cuuid);
                });
            }
            hub.hub._timer.addticktime(3000, heartbeat);
        }

        static void Main(string[] args)
        {
            var _hub = new hub.hub(args[0], args[1], "test");
            client_list = new List<string>();

            _hub.onDBProxyInit += ()=> {
                hub.hub.get_random_dbproxyproxy().getCollection("test", "test").getObjectInfo(new MongoDB.Bson.BsonDocument(), (_array) => {
                    foreach (var doc in _array)
                    {
                        log.log.trace("getObjectInfo info:{0}!", doc.ToString());
                    }
                }, ()=> {
                    log.log.trace("getObjectInfo info end!");
                });
            };
            _hub.on_client_disconnect += (string cuuid) =>
            {
                client_list.Remove(cuuid);
            };
            _hub.on_client_exception += (string cuuid) =>
            {
                client_list.Remove(cuuid);
            };
            _hub.on_direct_client_disconnect += (string cuuid) =>
            {
                client_list.Remove(cuuid);
            };

            var _test_c2s_module = new abelkhan.test_c2s_module();
            _test_c2s_module.on_login += () => {
                log.log.trace("client:{0} login!", hub.hub._gates.current_client_uuid);
                client_list.Add(hub.hub._gates.current_client_uuid);

                hub.hub.get_random_dbproxyproxy().getCollection("test", "test").createPersistedObject(new MongoDB.Bson.BsonDocument() { {"svr", "test_csharp"}, {"cuuid", hub.hub._gates.current_client_uuid } }, (ret)=> {
                    log.log.trace("createPersistedObject ret:{0}", ret);
                });
            };
            _test_c2s_module.on_get_svr_host += () => {
                log.log.trace("get_svr_host!");
                var rsp = (abelkhan.test_c2s_get_svr_host_rsp)_test_c2s_module.rsp;
                rsp.rsp(hub.hub.tcp_outside_address.host, hub.hub.tcp_outside_address.port);
            };
            _test_c2s_module.on_get_websocket_svr_host += () => {
                log.log.trace("get_websocket_svr_host!");
                var rsp = (abelkhan.test_c2s_get_websocket_svr_host_rsp)_test_c2s_module.rsp;
                rsp.rsp(hub.hub.websocket_outside_address.host, hub.hub.websocket_outside_address.port);
            };

            _test_s2c_caller = new abelkhan.test_s2c_caller();
            hub.hub._timer.addticktime(3000, heartbeat);

            _hub.run();
        }
    }
}
