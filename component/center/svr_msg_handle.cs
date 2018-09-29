using System;

namespace center
{
	class svr_msg_handle
	{
		public svr_msg_handle(svrmanager _svrmanager_, hubmanager _hubmanager_, clutter _clutter_)
		{
			_svrmanager = _svrmanager_;
			_hubmanager = _hubmanager_;
            _clutter = _clutter_;

        }

		public void reg_server(String type, String ip, Int64 port, String uuid, Int64 zone_id)
		{
            if (type != "dbproxy")
            {
                if (zone_id != 0)
                {
                    var _tmp_zone = _clutter.get_zone((int)zone_id);
                    _tmp_zone.for_each_hub_ch(
                        (juggle.Ichannel ch) =>
                        {
                            _hubmanager.get_hub(ch).distribute_server_address(type, ip, port, uuid);
                        }
                    );
                }
                else
                {
                    _hubmanager.for_each_hub(
                        (hubproxy _hubproxy) =>
                        {
                            _hubproxy.distribute_server_address(type, ip, port, uuid);
                        }
                    );
                }
            }

			if (type == "hub") {
				hubproxy _hubproxy = _hubmanager.reg_hub (juggle.Imodule.current_ch, type, ip, port, uuid, (int)zone_id);

                if (zone_id != 0)
                {
                    var _tmp_zone = _clutter.get_zone((int)zone_id);
                    _tmp_zone.for_each_svr_ch(
                        (juggle.Ichannel ch) =>
                        {
                            var tmp_svr_info = _svrmanager.get_svr(ch);
                            if (tmp_svr_info.Item1 == "dbproxy")
                            {
                                return;
                            }
                            _hubproxy.distribute_server_address(tmp_svr_info.Item1, tmp_svr_info.Item2, tmp_svr_info.Item3, tmp_svr_info.Item4);
                        }
                    );
                }
                else
                {
                    _svrmanager.for_each_svr_info(
                        (String _type, String _ip, Int64 _port, String _uuid) =>
                        {
                            if (_type == "dbproxy")
                            {
                                return;
                            }
                            _hubproxy.distribute_server_address(_type, _ip, _port, _uuid);
                        }
                    );
                }
			}

			if (type == "dbproxy") {
			}
			
			svrproxy _svrproxy = _svrmanager.reg_svr(juggle.Imodule.current_ch, type, ip, port, uuid);
			_svrproxy.reg_server_sucess();

            var _zone = _clutter.get_zone((int)zone_id);
            _zone.reg_svr(juggle.Imodule.current_ch, type);

            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "{0} server {1} connected", type, uuid);
		}

		private svrmanager _svrmanager;
		private hubmanager _hubmanager;
        private clutter _clutter;

    }
}
