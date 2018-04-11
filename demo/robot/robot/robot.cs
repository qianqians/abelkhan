using System;
using System.Threading;

namespace robot
{
    class test_robot
    {
        public delegate long poll_handle();

        static robot _robot;
        static void Main(string[] args)
        {
            _robot = new robot(args);
            _robot.onConnectGate += onGateHandle;
            _robot.onConnectHub += onConnectHub;
            
            _robot.connect_server(service.timerservice.Tick);

            poll_handle _poll_handle = _robot.poll;
            while (true)
            {
                if (_poll_handle() < 50)
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
