using System;
using System.Threading;
using hub;

namespace test_hub
{
    class server
    {
        public delegate long poll_handle();

        static void Main(string[] args)
        {
            if (args.Length <= 0)
            {
                return;
            }

            hub.hub _hub = new hub.hub(args);
            
            while (true)
            {
                if (hub.hub.closeHandle.is_close)
                {
                    log.log.operation(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "server closed, hub server:{0}", hub.hub.uuid);
                    break;
                }

                poll_handle _poll_handle = _hub.poll;
                if (_poll_handle() < 50)
                {
                    Thread.Sleep(15);
                }
            }
        }
    }
}
