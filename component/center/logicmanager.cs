using System;
using System.Collections;
using System.Collections.Generic;

namespace center
{
	class logicmanager
	{
		public logicmanager()
		{
			logicproxys = new Dictionary<juggle.Ichannel, Tuple<String, String,Int64, String, logicproxy> >();
		}

		public logicproxy reg_logic(juggle.Ichannel ch, String type, String ip, Int64 port, String uuid)
		{
			logicproxy _logicproxy = new logicproxy (ch);
			logicproxys.Add(ch, Tuple.Create(type, ip, port, uuid, _logicproxy));

			return _logicproxy;
		}

		public logicproxy get_logicproxy(juggle.Ichannel ch)
		{
			if (logicproxys.ContainsKey (ch)) 
			{
				return logicproxys [ch].Item5;
			}
			
			return null;
		}

		public void for_each_logic(Action<logicproxy> func)
		{
			foreach(Tuple<String, String,Int64, String, logicproxy> value in logicproxys.Values)
			{
				func(value.Item5);
			}
		}

		private Dictionary<juggle.Ichannel, Tuple<String, String,Int64, String, logicproxy> > logicproxys;
	}
}
