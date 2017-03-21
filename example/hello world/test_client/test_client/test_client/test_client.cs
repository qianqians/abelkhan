using System;
using System.Threading;

namespace client
{
    class test_client
    {
        static client _client;

        static void Main(string[] args)
        {
            _client = new client();

            login _login = new login();
            _client.modulemanager.add_module("login", _login);

            Int64 tick = Environment.TickCount;
            _client.connect_server("127.0.0.1", 3236, tick);

            _client.onConnectGate += onGeteHandle;
            _client.onConnectHub += onConnectHub;
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

                _client.poll(tick);

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

        private static void onGeteHandle()
        {
            Console.WriteLine("onGeteHandle");
            _client.connect_hub("lobby");
        }

        private static void onConnectHub(string hub_name)
        {
            Console.WriteLine("onConnectHub");
            _client.call_hub("lobby", "login", "player_login", "1234-1234", "qianqians");
        }


    }
}
