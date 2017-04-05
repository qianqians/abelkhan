using System;

namespace center
{
	class gm_msg_handle
	{
		public gm_msg_handle(gmmanager _gmmanager_, svrmanager _svrmanager_)
		{
			_gmmanager = _gmmanager_;
			_svrmanager = _svrmanager_;
		}

		public void confirm_gm(String gm_name)
		{
			_gmmanager.reg_gm(gm_name, juggle.Imodule.current_ch);
		}

		public void close_clutter(String gmname)
		{
			if (_gmmanager.check_gm (gmname, juggle.Imodule.current_ch)) 
			{
				_svrmanager.for_each_svr(
					(svrproxy _svrproxy) => {
						_svrproxy.close_server();
					}
				);
			}
		}

		private gmmanager _gmmanager;
		private svrmanager _svrmanager;
	}
}
