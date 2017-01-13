using System;
using System.Collections;
using System.Collections.Generic;

namespace gate
{
	public class logicmanager
	{
		public logicmanager()
		{
			logicproxys = new Dictionary<juggle.Ichannel, logicproxy>();
			logicproxys_uuid = new Dictionary<string, logicproxy> ();
		}

		public logicproxy reg_logic(String uuid, juggle.Ichannel ch)
		{
			var _logicproxy = new logicproxy(ch);
			logicproxys.Add(ch, _logicproxy);
			logicproxys_uuid.Add (uuid, _logicproxy);

			return _logicproxy;
		}

		public logicproxy get_logic(juggle.Ichannel ch)
		{
			if (logicproxys.ContainsKey (ch)) 
			{
				return logicproxys [ch];
			}
			
			return null;
		}

		public logicproxy get_logic()
		{
			Random rand = new Random ();
			int r = rand.Next (0, logicproxys.Values.Count);

			logicproxy _logicproxy = null;
			foreach (var _c in logicproxys.Values) 
			{
				if (r == 0) 
				{
					_logicproxy = _c;
				}

				r--;
			}

			return _logicproxy;
		}

		private Dictionary<juggle.Ichannel, logicproxy> logicproxys;
		private Dictionary<String, logicproxy> logicproxys_uuid;
	}
}

