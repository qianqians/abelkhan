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

			while (true)
            {
                var tick = (uint)_dbproxy.poll();

                if (dbproxy.dbproxy._closeHandle.is_close())
                {
                    log.log.info("server closed, dbproxy server:{0}", dbproxy.dbproxy.name);
                    _dbproxy._acceptservice.close();
                    break;
                }

                if (tick < 50)
                {
                    Thread.Sleep(5);
                }
            }

            dbproxy.dbproxy._dbevent.join_all();
		}
	}
}
