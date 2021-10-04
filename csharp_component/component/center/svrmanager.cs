using System;
using System.Collections.Generic;

namespace center
{
	class svrmanager
	{
		public svrmanager()
		{
			svrproxys = new Dictionary<juggle.Ichannel, Tuple<String, String,Int64, String, svrproxy> >();
            _dbproxys = new List<svrproxy>();

        }

		public svrproxy reg_svr(juggle.Ichannel ch, String type, String ip, Int64 port, String uuid)
		{
			svrproxy _svrproxy = new svrproxy(ch, type);
			svrproxys.Add(ch, Tuple.Create(type, ip, port, uuid, _svrproxy));

            if (type == "dbproxy")
            {
                _dbproxys.Add(_svrproxy);
            }

            return _svrproxy;
		}

        public void close_db()
        {
            foreach (var _dbproxy in _dbproxys)
            {
                _dbproxy.close_server();
            }
        }

		public void for_each_svr_info(Action<String, String, Int64, String> func)
		{
			foreach(Tuple<String, String, Int64, String, svrproxy> value in svrproxys.Values)
			{
				func (value.Item1, value.Item2, value.Item3, value.Item4);
			}
		}

		public void for_each_svr(Action<svrproxy> func)
		{
			foreach(Tuple<String, String, Int64, String, svrproxy> value in svrproxys.Values)
			{
				func (value.Item5);
			}
		}

        public Tuple<String, String, Int64, String, svrproxy> get_svr(juggle.Ichannel ch)
        {
            if (svrproxys.ContainsKey(ch))
            {
                return svrproxys[ch];
            }

            return null;
        }

        private List<svrproxy> _dbproxys;
        private Dictionary<juggle.Ichannel, Tuple<String, String,Int64, String, svrproxy> > svrproxys;
	}
}
