using System;
using System.Threading;
using hub;

namespace lobby
{
    class server
    {
        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                return;
            }

            hub.hub _hub = new hub.hub(args);

            players = new playermng();

            login _login = new login();
            hub.hub.modules.add_module("login", _login);

            Int64 tick = Environment.TickCount;
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

                _hub.poll(tick);

                if (hub.hub.closeHandle.is_close)
                {
                    Console.WriteLine("server closed, hub server " + hub.hub.uuid);
                    break;
                }

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

        public static playermng players;


    }
}
