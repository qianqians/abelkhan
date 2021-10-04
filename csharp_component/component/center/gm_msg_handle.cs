using System;

namespace center
{
	class gm_msg_handle
	{
		public gm_msg_handle(gmmanager _gmmanager_, svrmanager _svrmanager_, hubmanager _hubmanager_, clutter _clutter_)
		{
			_gmmanager = _gmmanager_;
			_svrmanager = _svrmanager_;
            _hubmanager = _hubmanager_;
            _clutter = _clutter_;
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

                if (_hubmanager.checkAllHubClosed()) {
                    _svrmanager.close_db();
                    center.closeHandle.is_close = true;
                }

                log.log.operation(new System.Diagnostics.StackFrame(), service.timerservice.Tick, "close clutter!");
			}
		}

        public void close_zone(String gmname, Int64 zone_id)
        {
            if (_gmmanager.check_gm(gmname, juggle.Imodule.current_ch))
            {
                var _zone = _clutter.get_zone((int)zone_id);

                _zone.is_closed = true;

                _zone.for_each_svr(
                    (svrimpl _svrimpl) => {
                        if (_svrimpl.type != "dbproxy")
                        {
                            _svrimpl.close_server();
                        }
                    }
                );

                if (_zone.checkAllHubClosed())
                {
                    _zone.close_db();
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
        private clutter _clutter;

    }
}
