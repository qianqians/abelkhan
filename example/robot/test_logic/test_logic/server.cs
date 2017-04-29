using System;
using System.Threading;
using logic;

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

            logic.logic _logic = new logic.logic(args);
            
            Int64 oldtick = 0;
            Int64 tick = 0;
            while (true)
            {
                oldtick = tick;
                tick = _logic.poll();

                if (logic.logic.closeHandle.is_close)
                {
                    log.log.trace(new System.Diagnostics.StackFrame(true), tick, "server closed, logic server:{0}", logic.logic.uuid);
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
