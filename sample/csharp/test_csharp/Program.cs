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
                    log.Log.trace("ping client:{0} sucessed!", cuuid);
                }, () =>
                {
                    log.Log.err("ping client:{0} faild!", cuuid);
                }).timeout(3000, () => {
                    log.Log.err("ping client:{0} timeout!", cuuid);
                });
            }
            hub.Hub._timer.addticktime(3000, heartbeat);
        }

        static void Main(string[] args)
        {
            var _hub = new hub.Hub(args[0], args[1], "test");
            client_list = new List<string>();

            _hub.onDBProxyInit += ()=> {
                hub.Hub.get_random_dbproxyproxy().getCollection("test", "test").getObjectInfo(new MongoDB.Bson.BsonDocument(), (_array) => {
                    foreach (var doc in _array)
                    {
                        log.Log.trace("getObjectInfo info:{0}!", doc.ToString());
                    }
                }, ()=> {
                    log.Log.trace("getObjectInfo info end!");
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
                log.Log.trace("client:{0} login!", hub.Hub._gates.current_client_uuid);
                client_list.Add(hub.Hub._gates.current_client_uuid);

                hub.Hub.get_random_dbproxyproxy().getCollection("test", "test").createPersistedObject(new MongoDB.Bson.BsonDocument() { {"svr", "test_csharp"}, {"cuuid", hub.Hub._gates.current_client_uuid } }, (ret)=> {
                    log.Log.trace("createPersistedObject ret:{0}", ret);
                });
            };
            _test_c2s_module.on_get_svr_host += () => {
                log.Log.trace("get_svr_host!");
                var rsp = (abelkhan.test_c2s_get_svr_host_rsp)_test_c2s_module.rsp;
                rsp.rsp(hub.Hub.tcp_outside_address.host, hub.Hub.tcp_outside_address.port);
            };
            _test_c2s_module.on_get_websocket_svr_host += () => {
                log.Log.trace("get_websocket_svr_host!");
                var rsp = (abelkhan.test_c2s_get_websocket_svr_host_rsp)_test_c2s_module.rsp;
                rsp.rsp(hub.Hub.websocket_outside_address.host, hub.Hub.websocket_outside_address.port);
            };

            _test_s2c_caller = new abelkhan.test_s2c_caller();
            hub.Hub._timer.addticktime(3000, heartbeat);

            _hub.run();
        }
    }
}
