using System;
using System.Threading;

namespace dbproxy_svr
{
    class Program
    {
		static void Main(string[] args)
		{
            var _dbproxy = new dbproxy.dbproxy(args[0], args[1]);

			log.log.trace("dbproxy start ok");

			_dbproxy.run();
		}
	}
}
