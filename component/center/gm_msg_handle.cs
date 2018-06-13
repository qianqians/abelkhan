using System;

namespace center
{
	class gm_msg_handle
	{
		public gm_msg_handle(gmmanager _gmmanager_, svrmanager _svrmanager_, hubmanager _hubmanager_)
		{
			_gmmanager = _gmmanager_;
			_svrmanager = _svrmanager_;
            _hubmanager = _hubmanager_;
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
                        if (_svrproxy.type != "dbproxy") {
                            _svrproxy.close_server();
                        }
					}
				);

                if (_hubmanager.checkAllHubClosed())
                {
                    _svrmanager.close_db();
                    center.closeHandle.is_close = true;
                }

                log.log.operation(new System.Diagnostics.StackFrame(), service.timerservice.Tick, "close clutter!");
			}
		}

        public void reload(String gmname, String argv)
        {
            if (_gmmanager.check_gm(gmname, juggle.Imodule.current_ch))
            {
                _hubmanager.for_each_hub(
                    (hubproxy _proxy) => {
                        _proxy.reload(argv);
                    }
                );

                log.log.operation(new System.Diagnostics.StackFrame(), service.timerservice.Tick, "reload!");
            }
        }

		private gmmanager _gmmanager;
		private svrmanager _svrmanager;
        private hubmanager _hubmanager;

    }
}
