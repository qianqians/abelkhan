using System;
using System.Threading;

namespace hub
{
	public class hub
	{
		public hub(String[] args)
		{
			uuid = System.Guid.NewGuid().ToString();

			config.config _config = new config.config(args[0]);
			config.config _center_config = _config.get_value_dict("center");
			if (args.Length > 1)
			{
				_config = _config.get_value_dict(args[1]);
			}

			name = _config.get_value_string("hub_name");

			_closehandle = new closehandle();

			logics = new logicmanager();
			_modulemanager = new common.modulemanager();

			var ip = _config.get_value_string("ip");
			var port = (short)_config.get_value_int("port");
			_logic_msg_handle = new logic_msg_handle(_modulemanager, logics);
			_logic_call_hub = new module.hub();
			_logic_call_hub.onlogic_call_hub_mothed += _logic_msg_handle.logic_call_hub_mothed;
			_logic_call_hub.onreg_logic += _logic_msg_handle.reg_logic;
			var _logic_process = new juggle.process();
			_logic_process.reg_module(_logic_call_hub);
			_accept_logic_service = new service.acceptnetworkservice(ip, port, _logic_process);

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
			_centerproxy.reg_hub(ip, port, uuid);
			_center_msg_handle = new center_msg_handle(this, _closehandle, _centerproxy);
			_center_call_server.onreg_server_sucess += _center_msg_handle.reg_server_sucess;
			_center_call_server.onclose_server += _center_msg_handle.close_server;
			_center_call_hub.ondistribute_dbproxy_address += _center_msg_handle.distribute_dbproxy_address;

			_juggle_service = new service.juggleservice();
			_juggle_service.add_process(_logic_process);
			_juggle_service.add_process(_center_process);

			timer = new service.timerservice();
		}

		public void connect_dbproxy(String db_ip, short db_port)
		{
			var _dbproxy_process = new juggle.process();
			_connect_dbproxy_service = new service.connectnetworkservice(_dbproxy_process);
			var _db_ch = _connect_dbproxy_service.connect(db_ip, db_port);
			dbproxy = new dbproxyproxy(_db_ch);
			dbproxy.reg_hub(uuid);

			_dbproxy_msg_handle = new dbproxy_msg_handle(dbproxy);
			_dbproxy_call_hub = new module.dbproxy_call_hub();
			_dbproxy_call_hub.onreg_hub_sucess += _dbproxy_msg_handle.reg_hub_sucess;
			_dbproxy_call_hub.onack_create_persisted_object += _dbproxy_msg_handle.ack_create_persisted_object;
			_dbproxy_call_hub.onack_updata_persisted_object += _dbproxy_msg_handle.ack_updata_persisted_object;
			_dbproxy_call_hub.onack_get_object_info += _dbproxy_msg_handle.ack_get_object_info;
			_dbproxy_call_hub.onack_get_object_info_end += _dbproxy_msg_handle.ack_get_object_info_end;
			_dbproxy_process.reg_module(_dbproxy_call_hub);

			_juggle_service.add_process(_dbproxy_process);
		}

		private void poll(Int64 tick)
		{
			_juggle_service.poll(tick);
			timer.poll(tick);
			_accept_logic_service.poll(tick);
			_connect_center_service.poll(tick);
			_connect_dbproxy_service.poll(tick);
		}

		public static void Main(String[] args)
		{
			if (args.Length <= 0)
			{
				return;
			}

			hub _hub = new hub(args);

			Int64 tick = Environment.TickCount;
			Int64 tickcount = 0;
			while (true)
			{
				Int64 tmptick = (Environment.TickCount & UInt32.MaxValue);
				if (tmptick < tick)
				{
					tickcount += 1;
					tmptick = tmptick + tickcount * UInt32.MaxValue;
				}
				tick = tmptick;

				_hub.poll(tick);

				if (_hub._closehandle.is_close)
				{
					Console.WriteLine("server closed, hub server " + _hub.uuid);
					break;
				}

				tmptick = (Environment.TickCount & UInt32.MaxValue);
				if (tmptick < tick)
				{
					tickcount += 1;
					tmptick = tmptick + tickcount * UInt32.MaxValue;
				}
				Int64 ticktime = (tmptick - tick);
				tick = tmptick;

				if (ticktime < 100)
				{
					Thread.Sleep(15);
				}
			}
		}

		public static String name;

		private String uuid;

		private closehandle _closehandle;

		private service.acceptnetworkservice _accept_logic_service;
		private module.hub _logic_call_hub;
		private logic_msg_handle _logic_msg_handle;
		private common.modulemanager _modulemanager;
		public static logicmanager logics;

		private service.connectnetworkservice _connect_center_service;
		private module.center_call_hub _center_call_hub;
		private module.center_call_server _center_call_server;
		private centerproxy _centerproxy;
		private center_msg_handle _center_msg_handle;

		private service.connectnetworkservice _connect_dbproxy_service;
		private module.dbproxy_call_hub _dbproxy_call_hub;
		public static dbproxyproxy dbproxy;
		private dbproxy_msg_handle _dbproxy_msg_handle;

		private service.juggleservice _juggle_service;
		public static service.timerservice timer;
	}
}

