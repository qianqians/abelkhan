using System;
using System.Threading;
using hub;

namespace test_hub
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
            
            Int64 oldtick = 0;
            Int64 tick = 0;
            while (true)
            {
                oldtick = tick;
                tick = _hub.poll();

                if (hub.hub.closeHandle.is_close)
                {
                    log.log.operation(new System.Diagnostics.StackFrame(true), tick, "server closed, hub server:{0}", hub.hub.uuid);
                    break;
                }
                
                Int64 ticktime = (tick - oldtick);
                if (ticktime < 50)
                {
                    Thread.Sleep(15);
                }
            }
        }
    }
}
