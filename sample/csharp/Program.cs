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
        }

        static void Main(string[] args)
        {
            var _hub = new hub.hub(args);

            client_list = new List<string>();

            var _test_c2s_module = new abelkhan.test_c2s_module();
            _test_c2s_module.on_login += () => {
                client_list.Add(hub.hub._gates.current_client_uuid);
            };
            _test_c2s_module.on_get_svr_host += () => {
                var rsp = (abelkhan.test_c2s_get_svr_host_rsp)_test_c2s_module.rsp;
                rsp.rsp("127.0.0.1", 3002);
            };

            _test_s2c_caller = new abelkhan.test_s2c_caller();
            hub.hub._timer.addticktime(3000, heartbeat);

            while (true)
            {
                var ticktime = _hub.poll();

                if (hub.hub._closeHandle.is_close)
                {
                    log.log.info("server closed, hub server:{0}", hub.hub.name);
                    break;
                }

                if (ticktime < 50)
                {
                    Thread.Sleep(5);
                }
            }
        }
    }
}
