using System;
using System.Threading;

namespace center
{
	class center
	{
		public center(string[] args)
		{
			uuid = System.Guid.NewGuid().ToString();

			config.config _config = new config.config(args[0]);
			if (args.Length > 1)
			{
				_config = _config.get_value_dict(args[1]);
			}

            closeHandle = new closehandle();

            var log_level = _config.get_value_string("log_level");
            if (log_level == "debug")
            {
                log.log.logMode = log.log.enLogMode.Debug;
            }
            else if (log_level == "release")
            {
                log.log.logMode = log.log.enLogMode.Release;
            }
            var log_file = _config.get_value_string("log_file");
            log.log.logFile = log_file;
            var log_dir = _config.get_value_string("log_dir");
            log.log.logPath = log_dir;
            {
                if (!System.IO.Directory.Exists(log_dir))
                {
                    System.IO.Directory.CreateDirectory(log_dir);
                }
            }

            _svrmanager = new svrmanager ();
			_hubmanager = new hubmanager ();

            var _clutter = new clutter();

			juggle.process _center_process = new juggle.process ();

			_svr_call_center = new module.center ();
			_svr_msg_handle = new svr_msg_handle (_svrmanager, _hubmanager, _clutter);
			_svr_call_center.onreg_server += _svr_msg_handle.reg_server;
			_center_process.reg_module (_svr_call_center);

            var _hub_call_center = new module.hub_call_center();
            var _hub_msg_handle = new hub_msg_handle(_svrmanager, _hubmanager, _clutter);
            _hub_call_center.onclosed += _hub_msg_handle.closed;
            _center_process.reg_module(_hub_call_center);

            var ip = _config.get_value_string("ip");
			var port = (short)_config.get_value_int("port");
			_accept_svr_service = new service.acceptnetworkservice (ip, port, _center_process);

			_gmmanager = new gmmanager ();

			juggle.process _gm_process = new juggle.process ();

			_gm_center = new module.gm_center ();
			_gm_msg_handle = new gm_msg_handle(_gmmanager, _svrmanager, _hubmanager, _clutter);
			_gm_center.onconfirm_gm += _gm_msg_handle.confirm_gm;
			_gm_center.onclose_clutter += _gm_msg_handle.close_clutter;
            _gm_center.onclose_zone += _gm_msg_handle.close_zone;
            _gm_center.onreload += _gm_msg_handle.reload;
            _gm_process.reg_module(_gm_center);

			var gm_ip = _config.get_value_string("gm_ip");
			var gm_port = (short)_config.get_value_int("gm_port");
			_accept_gm_service = new service.acceptnetworkservice (gm_ip, gm_port, _gm_process);

			timer = new service.timerservice ();

			_juggle_service = new service.juggleservice ();
			_juggle_service.add_process (_center_process);
			_juggle_service.add_process (_gm_process);
		}

		public Int64 poll()
        {
            Int64 tick_begin = timer.poll();

            try
            {
                _juggle_service.poll(tick_begin);
            }
            catch (juggle.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), tick_begin, e.Message);
            }
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), tick_begin, "{0}", e);
            }

            System.GC.Collect();

            Int64 tick_end = timer.refresh();

            return tick_end - tick_begin;
        }

		private static void Main (string[] args)
		{
			if (args.Length <= 0)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "non input start argv");
				return;
			}

			center _center = new center(args);

            while (true)
			{
                var tmp = _center.poll();

                if (closeHandle.is_close)
                {
                    System.Threading.Thread.Sleep(100);
                    return;
                }

                if (tmp < 50)
                {
                    System.Threading.Thread.Sleep(15);
                }
			}
		}

		private String uuid;

        public static closehandle closeHandle;

        private service.acceptnetworkservice _accept_svr_service;

		private module.center _svr_call_center;
		private svr_msg_handle _svr_msg_handle;
		private svrmanager _svrmanager;

		private hubmanager _hubmanager;

		private service.acceptnetworkservice _accept_gm_service;
		private module.gm_center _gm_center;
		private gm_msg_handle _gm_msg_handle;
		private gmmanager _gmmanager;

		private service.juggleservice _juggle_service;
		private service.timerservice timer;
	}
}
