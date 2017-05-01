using System;
using System.Threading;

namespace logic
{
	public class logic
	{
		public logic(String[] args)
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

			modules = new common.modulemanager();

			_dbproxy_call_logic = new module.dbproxy_call_logic();
			_dbproxy_process = new juggle.process();
			_dbproxy_process.reg_module(_dbproxy_call_logic);
			_dbproxy_connectnetworkservice = new service.connectnetworkservice(_dbproxy_process);
			dbproxy = new dbproxyproxy(_dbproxy_connectnetworkservice);
			_dbproxy_msg_handle = new dbproxy_msg_handle(dbproxy);
			_dbproxy_call_logic.onreg_logic_sucess += _dbproxy_msg_handle.reg_logic_sucess;
			_dbproxy_call_logic.onack_create_persisted_object += _dbproxy_msg_handle.ack_create_persisted_object;
			_dbproxy_call_logic.onack_updata_persisted_object += _dbproxy_msg_handle.ack_updata_persisted_object;
			_dbproxy_call_logic.onack_get_object_info += _dbproxy_msg_handle.ack_get_object_info;
			_dbproxy_call_logic.onack_get_object_info_end += _dbproxy_msg_handle.ack_get_object_info_end;

			_hub_call_logic = new module.hub_call_logic();
			_hub_process = new juggle.process();
			_hub_process.reg_module(_hub_call_logic);
			_hub_connectnetworkservice = new service.connectnetworkservice(_hub_process);
			hubs = new hubmanager(_hub_connectnetworkservice);
			_hub_msg_handle = new hub_msg_handle(modules);
			_hub_call_logic.onreg_logic_sucess_and_notify_hub_nominate += _hub_msg_handle.reg_logic_sucess_and_notify_hub_nominate;
			_hub_call_logic.onhub_call_logic_mothed += _hub_msg_handle.hub_call_logic_mothed;

			var ip = _config.get_value_string("ip");
			var port = (short)_config.get_value_int("port");
			_logic_call_logic = new module.logic_call_logic();
			_logic_process = new juggle.process();
			_logic_connectnetworkservice = new service.connectnetworkservice(_logic_process);
			_logic_acceptnetworkservice = new service.acceptnetworkservice(ip, port, _logic_process);
			logics = new logicmanager(_logic_connectnetworkservice);
			_logic_msg_handle = new logic_msg_handle(modules);
			_logic_call_logic.onreg_logic += _logic_msg_handle.on_reg_logic;
			_logic_call_logic.onack_reg_logic += _logic_msg_handle.on_ack_reg_logic;
			_logic_call_logic.onlogic_call_logic_mothed += _logic_msg_handle.logic_call_logic_mothed;

			_gate_call_logic = new module.gate_call_logic();
			_gate_process = new juggle.process();
			_gate_process.reg_module(_gate_call_logic);
			_gate_connectnetworkservice = new service.connectnetworkservice(_gate_process);
			gates = new gatemanager(_gate_connectnetworkservice);
			_gate_msg_handle = new gate_msg_handle(modules);
			_gate_call_logic.onreg_logic_sucess += _gate_msg_handle.onreg_logic_sucess;
			_gate_call_logic.onclient_get_logic += _gate_msg_handle.client_get_logic;
            _gate_call_logic.onclient_connect += _gate_msg_handle.client_connect;
            _gate_call_logic.onclient_disconnect += _gate_msg_handle.client_disconnect;
            _gate_call_logic.onclient_exception += _gate_msg_handle.client_exception;
            _gate_call_logic.onclient_call_logic += _gate_msg_handle.client_call_logic;

			var center_ip = _center_config.get_value_string("ip");
			var center_port = (short)_center_config.get_value_int("port");
			_center_call_server = new module.center_call_server();
			_center_call_logic = new module.center_call_logic();
			_center_process = new juggle.process();
			_center_process.reg_module(_center_call_server);
			_center_process.reg_module(_center_call_logic);
			_center_connectnetworkservice = new service.connectnetworkservice(_center_process);
			var _center_ch = _center_connectnetworkservice.connect(center_ip, center_port);
			_centerproxy = new centerproxy(_center_ch);
			_center_msg_handle = new center_msg_handle(closeHandle, _centerproxy);
			_center_call_server.onclose_server += _center_msg_handle.close_server;
			_center_call_server.onreg_server_sucess += _center_msg_handle.reg_server_sucess;
			_center_call_logic.ondistribute_server_address += _center_msg_handle.distribute_server_address;
			_center_call_logic.onack_get_server_address += _center_msg_handle.ack_get_server_address;

			_juggle_service = new service.juggleservice();
			_juggle_service.add_process(_dbproxy_process);
			_juggle_service.add_process(_hub_process);
			_juggle_service.add_process(_gate_process);
			_juggle_service.add_process(_logic_process);
			_juggle_service.add_process(_center_process);

			timer = new service.timerservice();

			_centerproxy.reg_logic(ip, port, uuid);
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
                juggle.Imodule.current_ch.disconnect();
                log.log.error(new System.Diagnostics.StackFrame(true), tick, e.Message);
            }

            _dbproxy_connectnetworkservice.poll(tick);
			_hub_connectnetworkservice.poll(tick);
			_gate_connectnetworkservice.poll(tick);
			_logic_acceptnetworkservice.poll(tick);
			_center_connectnetworkservice.poll(tick);

            return tick;
        }
        
		private static void Main(String[] args)
		{
			if (args.Length <= 0)
			{
				return;
			}

			logic _logic = new logic(args);

			Int64 oldtick = 0;
			Int64 tick = 0;
			while (true)
			{
                oldtick = tick;
                tick = _logic.poll();

				if (closeHandle.is_close)
                {
                    log.log.operation(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "server closed, logic server:{0}", logic.uuid);
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

		public static common.modulemanager modules;

		public static dbproxyproxy dbproxy;
		private dbproxy_msg_handle _dbproxy_msg_handle;
		private module.dbproxy_call_logic _dbproxy_call_logic;
		private juggle.process _dbproxy_process;
		private service.connectnetworkservice _dbproxy_connectnetworkservice;

		public static hubmanager hubs;
		private hub_msg_handle _hub_msg_handle;
		private module.hub_call_logic _hub_call_logic;
		private juggle.process _hub_process;
		private service.connectnetworkservice _hub_connectnetworkservice;

		public static gatemanager gates;
		private gate_msg_handle _gate_msg_handle;
		private module.gate_call_logic _gate_call_logic;
		private juggle.process _gate_process;
		private service.connectnetworkservice _gate_connectnetworkservice;

		public static logicmanager logics;
		private logic_msg_handle _logic_msg_handle;
		private module.logic_call_logic _logic_call_logic;
		private juggle.process _logic_process;
		private service.connectnetworkservice _logic_connectnetworkservice;
		private service.acceptnetworkservice _logic_acceptnetworkservice;

		private centerproxy _centerproxy;
		private module.center_call_logic _center_call_logic;
		private module.center_call_server _center_call_server;
		private juggle.process _center_process;
		private center_msg_handle _center_msg_handle;
		private service.connectnetworkservice _center_connectnetworkservice;

		private service.juggleservice _juggle_service;
		public static service.timerservice timer;
	}
}

