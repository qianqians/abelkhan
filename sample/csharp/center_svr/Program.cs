using Log;
using System;
using System.Threading;

namespace center_svr
{
    class Program
    {
        static void Main(string[] args)
        {
            var _center = new Abelkhan.Center(args[0], args[1]);
            _center.on_svr_disconnect += (Abelkhan.SvrProxy _proxy) =>
            {
                Log.Log.err("svr:{0},{1} exception!", _proxy.type, _proxy.name);
            };

            Log.Log.trace("Center start ok");

            _center.run();
        }
    }
}
