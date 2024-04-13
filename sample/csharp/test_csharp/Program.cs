using Log;
using System;
using System.Collections.Generic;
using System.Threading;

namespace test_csharp
{
    class Program
    {
        private static List<string> client_list;
        private static Abelkhan.test_s2c_caller _test_s2c_caller;

        static void heartbeat(long _) {
            foreach (var cuuid in client_list)
            {
                _test_s2c_caller.get_client(cuuid).ping().callBack(() =>
                {
                    Log.Log.trace("ping client:{0} sucessed!", cuuid);
                }, () =>
                {
                    Log.Log.err("ping client:{0} faild!", cuuid);
                }).timeout(3000, () => {
                    Log.Log.err("ping client:{0} timeout!", cuuid);
                });
            }
            Hub.Hub._timer.addticktime(3000, heartbeat);
        }

        static void Main(string[] args)
        {
            var _hub = new Hub.Hub(args[0], args[1], "test");
            client_list = new List<string>();

            _hub.onDBProxyInit += ()=> {
                Hub.Hub.get_random_dbproxyproxy().getCollection("test", "test").getObjectInfo(new MongoDB.Bson.BsonDocument(), (_array) => {
                    foreach (var doc in _array)
                    {
                        Log.Log.trace("getObjectInfo info:{0}!", doc.ToString());
                    }
                }, ()=> {
                    Log.Log.trace("getObjectInfo info end!");
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

            var _test_c2s_module = new Abelkhan.test_c2s_module();
            _test_c2s_module.on_login += () => {
                Log.Log.trace("client:{0} login!", Hub.Hub._gates.current_client_uuid);
                client_list.Add(Hub.Hub._gates.current_client_uuid);

                Hub.Hub.get_random_dbproxyproxy().getCollection("test", "test").createPersistedObject(new MongoDB.Bson.BsonDocument() { {"svr", "test_csharp"}, {"cuuid", Hub.Hub._gates.current_client_uuid } }, (ret)=> {
                    Log.Log.trace("createPersistedObject ret:{0}", ret);
                });
            };
            _test_c2s_module.on_get_svr_host += () => {
                Log.Log.trace("get_svr_host!");
                var rsp = (Abelkhan.test_c2s_get_svr_host_rsp)_test_c2s_module.rsp;
                rsp.rsp(Hub.Hub.tcp_outside_address.host, Hub.Hub.tcp_outside_address.port);
            };
            _test_c2s_module.on_get_websocket_svr_host += () => {
                Log.Log.trace("get_websocket_svr_host!");
                var rsp = (Abelkhan.test_c2s_get_websocket_svr_host_rsp)_test_c2s_module.rsp;
                rsp.rsp(Hub.Hub.websocket_outside_address.host, Hub.Hub.websocket_outside_address.port);
            };

            _test_s2c_caller = new Abelkhan.test_s2c_caller();
            Hub.Hub._timer.addticktime(3000, heartbeat);

            _hub.run();
        }
    }
}
