﻿using System;
using System.Threading;

namespace hub
{
	public class hub
	{
		public hub(String[] args)
		{
			uuid = System.Guid.NewGuid().ToString();

            config = new config.config(args[0]);
			config.config _center_config = config.get_value_dict("center");
			if (args.Length > 1)
			{
                config = config.get_value_dict(args[1]);
			}

            var log_level = config.get_value_string("log_level");
            if (log_level == "debug")
            {
                log.log.logMode = log.log.enLogMode.Debug;
            }
            else if (log_level == "release")
            {
                log.log.logMode = log.log.enLogMode.Release;
            }
            var log_file = config.get_value_string("log_file");
            log.log.logFile = log_file;
            var log_dir = config.get_value_string("log_dir");
            log.log.logPath = log_dir;
            {
                if (!System.IO.Directory.Exists(log_dir))
                {
                    System.IO.Directory.CreateDirectory(log_dir);
                }
            }

            name = config.get_value_string("hub_name");

			closeHandle = new closehandle();
			modules = new common.modulemanager();

            var _hub_logic_process = new juggle.process();
            _connect_hub_service = new service.connectnetworkservice(_hub_logic_process);

            hubs = new hubmanager();

            _hub_msg_handle = new hub_msg_handle(modules, hubs);
            _hub_call_hub = new module.hub_call_hub();
            _hub_call_hub.onreg_hub += _hub_msg_handle.reg_hub;
            _hub_call_hub.onreg_hub_sucess += _hub_msg_handle.reg_hub_sucess;
            _hub_call_hub.onhub_call_hub_mothed += _hub_msg_handle.hub_call_hub_mothed;
            _hub_logic_process.reg_module(_hub_call_hub);

            var ip = config.get_value_string("ip");
            var port = (short)config.get_value_int("port");
            _accept_logic_service = new service.acceptnetworkservice(ip, port, _hub_logic_process);

			var center_ip = _center_config.get_value_string("ip");
			var center_port = (short)_center_config.get_value_int("port");
			_center_call_hub = new module.center_call_hub();
			_center_call_server = new module.center_call_server();
			var _center_process = new juggle.process();
			_center_process.reg_module(_center_call_hub);
			_center_process.reg_module(_center_call_server);
			_connect_center_service = new service.connectnetworkservice(_center_process);
			var center_ch = _connect_center_service.connect(center_ip, center_port);
			_centerproxy = new centerproxy(center_ch);
			_center_msg_handle = new center_msg_handle(this, closeHandle, _centerproxy);
			_center_call_server.onreg_server_sucess += _center_msg_handle.reg_server_sucess;
			_center_call_server.onclose_server += _center_msg_handle.close_server;
			_center_call_hub.ondistribute_server_address += _center_msg_handle.distribute_server_address;
            _center_call_hub.onreload += onReload_event;

            var _dbproxy_process = new juggle.process();
			_connect_dbproxy_service = new service.connectnetworkservice(_dbproxy_process);
			_dbproxy_msg_handle = new dbproxy_msg_handle(this);
			_dbproxy_call_hub = new module.dbproxy_call_hub();
			_dbproxy_call_hub.onreg_hub_sucess += _dbproxy_msg_handle.reg_hub_sucess;
			_dbproxy_call_hub.onack_create_persisted_object += _dbproxy_msg_handle.ack_create_persisted_object;
			_dbproxy_call_hub.onack_updata_persisted_object += _dbproxy_msg_handle.ack_updata_persisted_object;
            _dbproxy_call_hub.onack_get_object_count += _dbproxy_msg_handle.ack_get_object_count;
            _dbproxy_call_hub.onack_get_object_info += _dbproxy_msg_handle.ack_get_object_info;
			_dbproxy_call_hub.onack_get_object_info_end += _dbproxy_msg_handle.ack_get_object_info_end;
            _dbproxy_call_hub.onack_remove_object += _dbproxy_msg_handle.ack_remove_object;

            _dbproxy_process.reg_module(_dbproxy_call_hub);

			juggle.process _gate_process = new juggle.process();
			_gate_msg_handle = new gate_msg_handle(modules);
			_gate_call_hub = new module.gate_call_hub();
			_gate_call_hub.onreg_hub_sucess += _gate_msg_handle.reg_hub_sucess;
            _gate_call_hub.onclient_connect += _gate_msg_handle.client_connect;
            _gate_call_hub.onclient_disconnect += _gate_msg_handle.client_disconnect;
            _gate_call_hub.onclient_exception += _gate_msg_handle.client_exception;
            _gate_call_hub.onclient_call_hub += _gate_msg_handle.client_call_hub;
			_gate_process.reg_module (_gate_call_hub);
			_connect_gate_servcie = new service.connectnetworkservice (_gate_process);
			gates = new gatemanager (_connect_gate_servcie);

			_juggle_service = new service.juggleservice();
			_juggle_service.add_process(_hub_logic_process);
			_juggle_service.add_process(_center_process);
			_juggle_service.add_process(_dbproxy_process);
			_juggle_service.add_process (_gate_process);

			timer = new service.timerservice();

			_centerproxy.reg_hub(ip, port, uuid);
		}

        public delegate void onConnectDBHandle();
        public event onConnectDBHandle onConnectDB;
        public void onConnectDB_event()
        {
            if (onConnectDB != null)
            {
                onConnectDB();
            }
        }

        public delegate void onCloseServerHandle();
        public event onCloseServerHandle onCloseServer;
        public void onCloseServer_event()
        {
            if (onCloseServer != null)
            {
                onCloseServer();
            }
            _centerproxy.closed();

            closeHandle.is_close = true;
        }

        public delegate void onReloadHandle(string argv);
        public event onReloadHandle onReload;
        public void onReload_event(string argv)
        {
            if (onReload != null)
            {
                onReload(argv);
            }
        }

        public void connect_dbproxy(String db_ip, short db_port)
		{
			var _db_ch = _connect_dbproxy_service.connect(db_ip, db_port);
			dbproxy = new dbproxyproxy(_db_ch);
			dbproxy.reg_hub(uuid);
		}

        public void reg_hub(String hub_ip, short hub_port)
        {
            var ch = _connect_hub_service.connect(hub_ip, hub_port);
            caller.hub_call_hub _caller = new caller.hub_call_hub(ch);
            _caller.reg_hub(name);
        }

		public Int64 poll()
        {
            Int64 tick_begin = timer.poll();
            Int64 tmp = timer.refresh();

            try
            {
                _juggle_service.poll(tick_begin);
            }
            catch(juggle.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), tick_begin, e.Message);
            }
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), tick_begin, "{0}", e);
            }

            //System.GC.Collect();

            Int64 tick_end = timer.refresh();
            Int64 poll_tick = tick_end - tick_begin;

            if (poll_tick > 50)
            {
                Int64 timer_poll_tick = tmp - tick_begin;
                Int64 juggle_service_poll_tick = tick_end - tmp;

                log.log.trace(new System.Diagnostics.StackFrame(true), tick_end, "timer_tick:{0}, juggle_service_poll:{1}", timer_poll_tick, juggle_service_poll_tick);
            }

            return poll_tick;
        }

		private static void Main(String[] args)
		{
			if (args.Length <= 0)
			{
				return;
			}

			hub _hub = new hub(args);
            
			while (true)
            {
                var ticktime = _hub.poll();

				if (closeHandle.is_close)
                {
                    log.log.operation(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "server closed, hub server:{0}", hub.uuid);
					break;
				}
                
				if (ticktime < 50)
				{
					Thread.Sleep(5);
				}
			}
		}

		public static String name;

		public static String uuid;

		public static closehandle closeHandle;

        public static config.config config;

		private service.acceptnetworkservice _accept_logic_service;
        private service.connectnetworkservice _connect_hub_service;
        private module.hub_call_hub _hub_call_hub;
        private hub_msg_handle _hub_msg_handle;
        public static common.modulemanager modules;
        public static hubmanager hubs;

		private service.connectnetworkservice _connect_center_service;
		private module.center_call_hub _center_call_hub;
		private module.center_call_server _center_call_server;
		private centerproxy _centerproxy;
		private center_msg_handle _center_msg_handle;

		private service.connectnetworkservice _connect_dbproxy_service;
		private module.dbproxy_call_hub _dbproxy_call_hub;
		public static dbproxyproxy dbproxy;
		private dbproxy_msg_handle _dbproxy_msg_handle;

		private service.connectnetworkservice _connect_gate_servcie;
		private module.gate_call_hub _gate_call_hub;
		public static gatemanager gates;
		private gate_msg_handle _gate_msg_handle;

		private service.juggleservice _juggle_service;
		public static service.timerservice timer;
	}
}

