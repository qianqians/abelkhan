using System;
using System.Threading;

namespace center_svr
{
    class Program
    {
        static void Main(string[] args)
        {
            var _center = new abelkhan.center(args[0], args[1]);
            _center.on_svr_disconnect += (abelkhan.svrproxy _proxy) =>
            {
                log.log.err("svr:{0},{1} exception!", _proxy.type, _proxy.name);
            };

            log.log.trace("Center start ok");

            _center.run();
        }
    }
}
