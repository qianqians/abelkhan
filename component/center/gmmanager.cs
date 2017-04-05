using System;
using System.Collections;
using System.Collections.Generic;

namespace center
{
	class gmmanager
	{
		public gmmanager()
		{
			gms = new Dictionary<juggle.Ichannel, String>();
		}

		public void reg_gm(String gmname, juggle.Ichannel ch)
		{
			gms.Add(ch, gmname);
		}

		public bool check_gm(String gmname, juggle.Ichannel ch)
		{
			if (!gms.ContainsKey (ch)) 
			{
				return false;
			}

			return gms[ch] == gmname;
		}
			
		private Dictionary<juggle.Ichannel, String> gms;
	}
}
