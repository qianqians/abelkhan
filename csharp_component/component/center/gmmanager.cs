/*
 * gmmanager
 * 2020/6/2
 * qianqians
 */
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace abelkhan
{
	public class gmmanager
    {
		private Dictionary<abelkhan.Ichannel, string> gms;

		public gmmanager()
        {
			gms = new Dictionary<abelkhan.Ichannel, string>();
        }

		public void reg_gm(string gm_name, abelkhan.Ichannel ch)
        {
			gms.Add(ch, gm_name);
        }

		public bool check_gm(string gm_name, abelkhan.Ichannel ch)
        {
			if (!gms.TryGetValue(ch, out string _gm_name))
            {
				return false;
            }

			return _gm_name == gm_name;
		}
    }
}