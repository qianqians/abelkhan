using Log;
using System;
using System.Threading;

namespace dbproxy_svr
{
    class Program
    {
		static void Main(string[] args)
		{
            var _dbproxy = new DBProxy.DBProxy(args[0], args[1]);

			Log.Log.trace("dbproxy start ok");

			_dbproxy.run();
		}
	}
}
