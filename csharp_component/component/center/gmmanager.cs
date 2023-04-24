/*
 * gmmanager
 * 2020/6/2
 * qianqians
 */
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Abelkhan
{
	public class GMManager
    {
		private Dictionary<Abelkhan.Ichannel, string> gms;

		public GMManager()
        {
			gms = new Dictionary<Abelkhan.Ichannel, string>();
        }

		public void reg_gm(string gm_name, Abelkhan.Ichannel ch)
        {
			gms.Add(ch, gm_name);
        }

		public bool check_gm(string gm_name, Abelkhan.Ichannel ch)
        {
			if (!gms.TryGetValue(ch, out string _gm_name))
            {
				return false;
            }

			return _gm_name == gm_name;
		}
    }
}