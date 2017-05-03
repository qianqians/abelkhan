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
			_logicmanager = new logicmanager ();
			_hubmanager = new hubmanager ();

			juggle.process _center_process = new juggle.process ();

			_svr_call_center = new module.center ();
			_svr_msg_handle = new svr_msg_handle (_svrmanager, _hubmanager, _logicmanager);
			_svr_call_center.onreg_server += _svr_msg_handle.reg_server;
			_center_process.reg_module (_svr_call_center);

			_logic_call_center = new module.logic_call_center ();
			_logic_msg_handle = new logic_svr_msg_handle (_svrmanager, _logicmanager);
			_logic_call_center.onreq_get_server_address += _logic_msg_handle.req_get_server_address;
			_center_process.reg_module (_logic_call_center);

			var ip = _config.get_value_string("ip");
			var port = (short)_config.get_value_int("port");
			_accept_svr_service = new service.acceptnetworkservice (ip, port, _center_process);

			_gmmanager = new gmmanager ();

			juggle.process _gm_process = new juggle.process ();

			_gm_center = new module.gm_center ();
			_gm_msg_handle = new gm_msg_handle(_gmmanager, _svrmanager);
			_gm_center.onconfirm_gm += _gm_msg_handle.confirm_gm;
			_gm_center.onclose_clutter += _gm_msg_handle.close_clutter;
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
            Int64 tick = timer.poll();

            try
            {
                _juggle_service.poll(tick);
            }
            catch (juggle.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), tick, e.Message);
            }

            _accept_svr_service.poll(tick);
            _accept_gm_service.poll(tick);

            return tick;
        }

		private static void Main (string[] args)
		{
			if (args.Length <= 0)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "non input start argv");
				return;
			}

			center _center = new center(args);

            Int64 old_tick = 0;
            Int64 tick = 0;
            while (true)
			{
                old_tick = tick;
                tick = _center.poll();

                Int64 tmp = tick - old_tick;
                if (tmp < 50)
                {
                    System.Threading.Thread.Sleep(15);
                }
			}
		}

		private String uuid;

		private service.acceptnetworkservice _accept_svr_service;

		private module.center _svr_call_center;
		private svr_msg_handle _svr_msg_handle;
		private svrmanager _svrmanager;

		private module.logic_call_center _logic_call_center;
		private logic_svr_msg_handle _logic_msg_handle;
		private logicmanager _logicmanager;

		private hubmanager _hubmanager;

		private service.acceptnetworkservice _accept_gm_service;
		private module.gm_center _gm_center;
		private gm_msg_handle _gm_msg_handle;
		private gmmanager _gmmanager;

		private service.juggleservice _juggle_service;
		private service.timerservice timer;
	}
}
