using System;
using System.Threading;

namespace dbproxy
{
	public class dbproxy
	{
		public dbproxy(String[] args)
		{
			is_busy = false;

			uuid = System.Guid.NewGuid().ToString();

			config.config _config = new config.config(args[0]);
			config.config _center_config = _config.get_value_dict("center");
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

            closeHandle = new closehandle();

			var db_ip = _config.get_value_string("db_ip");
			var db_port = (short)_config.get_value_int("db_port");

			_mongodbproxy = new mongodbproxy(db_ip, db_port);

			var ip = _config.get_value_string("ip");
			var port = (short)_config.get_value_int("port");
            
			_hub_call_dbproxy = new module.hub_call_dbproxy();
			_process = new juggle.process();
			_process.reg_module(_hub_call_dbproxy);
			_acceptnetworkservice = new service.acceptnetworkservice(ip, port, _process);

			_hubmanager = new hubmanager ();
			_hub_msg_handle = new hub_msg_handle(_hubmanager, _mongodbproxy);
			_hub_call_dbproxy.onreg_hub += _hub_msg_handle.reg_hub;
			_hub_call_dbproxy.oncreate_persisted_object += _hub_msg_handle.create_persisted_object;
			_hub_call_dbproxy.onupdata_persisted_object += _hub_msg_handle.updata_persisted_object;
            _hub_call_dbproxy.onget_object_count += _hub_msg_handle.get_object_count;
            _hub_call_dbproxy.onget_object_info += _hub_msg_handle.get_object_info;
            _hub_call_dbproxy.onremove_object += _hub_msg_handle.remove_object;

            var center_ip = _center_config.get_value_string("ip");
			var center_port = (short)_center_config.get_value_int("port");
			_center_call_server = new module.center_call_server();
			_center_process = new juggle.process();
			_center_process.reg_module(_center_call_server);
			_center_connectnetworkservice = new service.connectnetworkservice(_center_process);
			var _center_ch = _center_connectnetworkservice.connect(center_ip, center_port);
			_centerproxy = new centerproxy(_center_ch);
			_center_msg_handle = new center_msg_handle(closeHandle, _centerproxy);
			_center_call_server.onclose_server += _center_msg_handle.close_server;
			_center_call_server.onreg_server_sucess += _center_msg_handle.reg_server_sucess;

			_juggle_service = new service.juggleservice();
			_juggle_service.add_process(_process);
			_juggle_service.add_process(_center_process);

			timer = new service.timerservice();

			_centerproxy.reg_dbproxy(ip, port, uuid);
		}

		public Int64 poll()
        {
            Int64 tick = timer.poll();

            try
            {
                _juggle_service.poll(tick);
            }
            catch(juggle.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), tick, e.Message);
            }
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), tick, "{0}", e);
            }

            System.GC.Collect();

            return tick;
        }
        
		private static void Main(String[] args)
		{
			if (args.Length <= 0)
			{
                log.log.error(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "non input start argv");
				return;
			}

			dbproxy _dbproxy = new dbproxy(args);

			Int64 oldtick = 0;
			Int64 tick = 0;
			while (true)
			{
                oldtick = tick;
                tick = _dbproxy.poll();

				if (closeHandle.is_close())
                {
                    log.log.operation(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "server closed, dbproxy server:{0}", dbproxy.uuid);
					break;
				}
                
				Int64 ticktime = (tick - oldtick);
				if (ticktime > 200)
				{
					is_busy = true;
				}
				else 
				{
					is_busy = false;
				}
				if (ticktime < 50)
				{
					Thread.Sleep(15);
				}
			}
		}

		public static String uuid;
		public static bool is_busy;

		public static closehandle closeHandle;

		private mongodbproxy _mongodbproxy;

		private hubmanager _hubmanager;
		private hub_msg_handle _hub_msg_handle;
		private module.hub_call_dbproxy _hub_call_dbproxy;

		private juggle.process _process;
		private service.acceptnetworkservice _acceptnetworkservice;

		private centerproxy _centerproxy;
		private module.center_call_server _center_call_server;
		private juggle.process _center_process;
		private center_msg_handle _center_msg_handle;
		private service.connectnetworkservice _center_connectnetworkservice;

		private service.juggleservice _juggle_service;
		public static service.timerservice timer;
	}
}

