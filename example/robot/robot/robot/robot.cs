using System;
using System.Threading;

namespace robot
{
    class test_robot
    {
        static robot _robot;

        static void Main(string[] args)
        {
            _robot = new robot(args);
            _robot.onConnectGate += onGateHandle;
            _robot.onConnectHub += onConnectHub;
            
            _robot.connect_server(service.timerservice.Tick);

            Int64 oldtick = 0;
            Int64 tick = 0;
            while (true)
            {
                oldtick = tick;
                tick = _robot.poll();
                
                Int64 ticktime = (tick - oldtick);
                if (ticktime < 50)
                {
                    Thread.Sleep(15);
                }
            } 
        }

        private static void onGateHandle()
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "onGateHandle");

            var _proxy = _robot.get_client_proxy(juggle.Imodule.current_ch);
            _proxy.connect_hub("hub");
        }

        private static void onConnectHub(string hub_name)
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "onConnectHub:{0}", hub_name);
        }


    }
}
