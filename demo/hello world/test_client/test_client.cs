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

            _client.onDisConnect += () => {
                log.log.trace(new System.Diagnostics.StackFrame(), service.timerservice.Tick, "disconnect");
            };

            login _login = new login();
            _client.modulemanager.add_module("login", _login);

            Int64 tick = service.timerservice.Tick;
            _client.connect_server("127.0.0.1", 3236, "127.0.0.1", 3237, tick);

            _client.onConnectGate += onGeteHandle;
            _client.onConnectHub += onConnectHub;
            
            Int64 oldtick = 0;
            while (true)
            {
                oldtick = tick;
                tick = _client.poll();

                
                Int64 ticktime = (tick - oldtick);
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
