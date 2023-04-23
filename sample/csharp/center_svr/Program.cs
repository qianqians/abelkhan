using System;
using System.Threading;

namespace center_svr
{
    class Program
    {
        static void Main(string[] args)
        {
            var _center = new abelkhan.Center(args[0], args[1]);
            _center.on_svr_disconnect += (abelkhan.Svrproxy _proxy) =>
            {
                log.Log.err("svr:{0},{1} exception!", _proxy.type, _proxy.name);
            };

            log.Log.trace("Center start ok");

            _center.run();
        }
    }
}
