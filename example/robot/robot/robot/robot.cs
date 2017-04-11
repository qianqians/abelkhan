using System;
using System.Threading;

namespace robot
{
    class test_robot
    {
        static robot _robot;

        static void Main(string[] args)
        {
            _robot = new robot(20);
            _robot.onConnectGate += onGateHandle;
            _robot.onAckGetLogic += onAckGetLogic;
            _robot.onConnectLogic += onConnectLogic;
            _robot.onConnectHub += onConnectHub;

            Int64 tick = Environment.TickCount;
            _robot.connect_server("127.0.0.1", 3236, tick);

            Int64 tickcount = 0;
            while (true)
            {
                Int64 tmptick = (Environment.TickCount & UInt32.MaxValue);
                if (tmptick < tick)
                {
                    tickcount += 1;
                    tmptick = tmptick + tickcount * UInt32.MaxValue;
                }
                tick = tmptick;

                _robot.poll(tick);

                tmptick = (Environment.TickCount & UInt32.MaxValue);
                if (tmptick < tick)
                {
                    tickcount += 1;
                    tmptick = tmptick + tickcount * UInt32.MaxValue;
                }
                Int64 ticktime = (tmptick - tick);
                tick = tmptick;

                if (ticktime < 50)
                {
                    Thread.Sleep(15);
                }
            } 
        }

        private static void onGateHandle()
        {
            Console.WriteLine("onGateHandle");

            var _proxy = _robot.get_client_proxy(juggle.Imodule.current_ch);
            _proxy.get_logic();
            _proxy.connect_hub("hub");
        }

        private static void onAckGetLogic(string _logic_uuid)
        {
            Console.WriteLine("onAckGetLogic:" + _logic_uuid);

            var _proxy = _robot.get_client_proxy(juggle.Imodule.current_ch);
            _proxy.connect_logic(_logic_uuid);
        }

        private static void onConnectLogic(string logic_uuid)
        {
            Console.WriteLine("onConnectLogic:" + logic_uuid);
        }

        private static void onConnectHub(string hub_name)
        {
            Console.WriteLine("onConnectHub:" + hub_name);
        }


    }
}
