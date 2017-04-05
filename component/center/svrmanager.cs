using System;
using System.Collections;
using System.Collections.Generic;

namespace center
{
	class svrmanager
	{
		public svrmanager()
		{
			svrproxys = new Dictionary<juggle.Ichannel, Tuple<String, String,Int64, String, svrproxy> >();
			svrproxys_uuid = new Dictionary<String, Tuple<String, String,Int64, String, svrproxy> >();
		}

		public svrproxy reg_svr(juggle.Ichannel ch, String type, String ip, Int64 port, String uuid)
		{
			svrproxy _svrproxy = new svrproxy(ch);
			svrproxys.Add(ch, Tuple.Create(type, ip, port, uuid, _svrproxy));
			svrproxys_uuid.Add(uuid, Tuple.Create(type, ip, port, uuid, _svrproxy));

			return _svrproxy;
		}

		public Tuple<String, String,Int64, String, svrproxy> get_svr_info(String uuid)
		{
			if (svrproxys_uuid.ContainsKey (uuid)) 
			{
				return svrproxys_uuid[uuid];
			}
			
			return null;
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

		private Dictionary<juggle.Ichannel, Tuple<String, String,Int64, String, svrproxy> > svrproxys;
		private Dictionary<String, Tuple<String, String,Int64, String, svrproxy> > svrproxys_uuid;
	}
}
