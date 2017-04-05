using System;
using System.Collections;
using System.Collections.Generic;

namespace dbproxy
{
	public class logicmanager
	{
		public logicmanager()
		{
			logicproxys = new Dictionary<juggle.Ichannel, logicproxy>();
			lgoicproxys_uuid = new Dictionary<string, logicproxy>();
		}

		public logicproxy reg_logic(String uuid, juggle.Ichannel ch)
		{
			logicproxy _logproxy = new logicproxy (ch);
			lgoicproxys_uuid.Add(uuid, _logproxy);
			logicproxys.Add (ch, _logproxy);

			return _logproxy;
		}

		public logicproxy get_logic(juggle.Ichannel ch)
		{
			if (logicproxys.ContainsKey (ch)) 
			{
				return logicproxys [ch];
			}

			return null;
		}

		private Dictionary<juggle.Ichannel, logicproxy> logicproxys;
		private Dictionary<String, logicproxy> lgoicproxys_uuid;

	}
}

