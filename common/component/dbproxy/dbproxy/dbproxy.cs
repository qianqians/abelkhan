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

			closeHandle = new closehandle();

			var db_ip = _config.get_value_string("db_ip");
			var db_port = (short)_config.get_value_int("db_port");
			var db = _config.get_value_string("db_name");
			var collection = _config.get_value_string("db_collection");

			_mongodbproxy = new mongodbproxy(db_ip, db_port, db, collection);

			var ip = _config.get_value_string("ip");
			var port = (short)_config.get_value_int("port");

			_hub_call_dbproxy = new module.hub_call_dbproxy();
			_hub_process = new juggle.process();
			_hub_process.reg_module(_hub_call_dbproxy);
			_hub_acceptnetworkservice = new service.acceptnetworkservice(ip, port, _hub_process);
			_hubmanager = new hubmanager ();
			_hub_msg_handle = new hub_msg_handle(_hubmanager, _mongodbproxy);
			_hub_call_dbproxy.onreg_hub += _hub_msg_handle.reg_hub;
			_hub_call_dbproxy.oncreate_persisted_object += _hub_msg_handle.create_persisted_object;
			_hub_call_dbproxy.onupdata_persisted_object += _hub_msg_handle.updata_persisted_object;
			_hub_call_dbproxy.onget_object_info += _hub_msg_handle.get_object_info;

			_logic_call_dbproxy = new module.logic_call_dbproxy();
			_logic_process = new juggle.process();
			_logic_acceptnetworkservice = new service.acceptnetworkservice(ip, port, _logic_process);
			_logicmanager = new logicmanager();
			_logic_msg_handle = new logic_msg_handle(_logicmanager, _mongodbproxy, closeHandle);
			_logic_call_dbproxy.onreg_logic += _logic_msg_handle.reg_logic;
			_logic_call_dbproxy.onlogic_closed += _logic_msg_handle.logic_closed;
			_logic_call_dbproxy.oncreate_persisted_object += _logic_msg_handle.create_persisted_object;
			_logic_call_dbproxy.onupdata_persisted_object += _logic_msg_handle.updata_persisted_object;
			_logic_call_dbproxy.onget_object_info += _logic_msg_handle.get_object_info;

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
			_juggle_service.add_process(_hub_process);
			_juggle_service.add_process(_logic_process);
			_juggle_service.add_process(_center_process);

			timer = new service.timerservice();

			_centerproxy.reg_dbproxy(ip, port, uuid);
		}

		public void poll(Int64 tick)
		{
			_juggle_service.poll(tick);
			timer.poll(tick);

			_hub_acceptnetworkservice.poll(tick);
			_logic_acceptnetworkservice.poll(tick);
			_center_connectnetworkservice.poll(tick);
		}
        
		private static void Main(String[] args)
		{
			if (args.Length <= 0)
			{
				System.Console.WriteLine ("non input start argv");
				return;
			}

			dbproxy _dbproxy = new dbproxy(args);

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

				_dbproxy.poll(tick);

				if (closeHandle.is_close())
				{
					Console.WriteLine("server closed, hub server " + dbproxy.uuid);
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
		private juggle.process _hub_process;
		private service.acceptnetworkservice _hub_acceptnetworkservice;

		private logicmanager _logicmanager;
		private logic_msg_handle _logic_msg_handle;
		private module.logic_call_dbproxy _logic_call_dbproxy;
		private juggle.process _logic_process;
		private service.acceptnetworkservice _logic_acceptnetworkservice;

		private centerproxy _centerproxy;
		private module.center_call_server _center_call_server;
		private juggle.process _center_process;
		private center_msg_handle _center_msg_handle;
		private service.connectnetworkservice _center_connectnetworkservice;

		private service.juggleservice _juggle_service;
		public static service.timerservice timer;
	}
}

