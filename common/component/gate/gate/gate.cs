using System;
using System.Threading;

namespace gate
{
	public class gate
	{
        public void onClientDissconnect(juggle.Ichannel ch)
        {
            _clientmanager.unreg_client(ch);
        }

		public gate(String[] args)
		{
			uuid = System.Guid.NewGuid().ToString();

			config.config _config = new config.config(args[0]);
			config.config _center_config = _config.get_value_dict("center");
			if (args.Length > 1)
			{
				_config = _config.get_value_dict(args[1]);
			}

			closeHandle = new closehandle();

			timer = new service.timerservice();
			_logicmanager = new logicmanager();
            _hubmanager = new hubmanager();
            _clientmanager = new clientmanager ();

			_client_msg_handle = new client_msg_handle (_clientmanager, _logicmanager, _hubmanager, timer);
			_client_call_gate = new module.client_call_gate ();
			_client_call_gate.onconnect_server += _client_msg_handle.connect_server;
			_client_call_gate.oncancle_server += _client_msg_handle.cancle_server;
            _client_call_gate.onforward_client_call_logic += _client_msg_handle.forward_client_call_logic;
            _client_call_gate.onforward_client_call_hub += _client_msg_handle.forward_client_call_hub;
            _client_call_gate.onheartbeats += _client_msg_handle.heartbeats;
			var _client_process = new juggle.process();
			_client_process.reg_module (_client_call_gate);

			var outside_ip = _config.get_value_string("outside_ip");
			var outside_port = (short)_config.get_value_int("outside_port");
			_accept_client_service = new service.acceptnetworkservice(outside_ip, outside_port, _client_process);
            _accept_client_service.onChannelDisconnect += onClientDissconnect;

            _logic_msg_handle = new logic_msg_handle(_logicmanager, _clientmanager);
			_logic_call_gate = new module.logic_call_gate();
			_logic_call_gate.onreg_logic += _logic_msg_handle.reg_logic;
			_logic_call_gate.onack_client_get_logic += _logic_msg_handle.ack_client_get_logic;
			_logic_call_gate.onforward_logic_call_client += _logic_msg_handle.forward_logic_call_client;
			_logic_call_gate.onforward_logic_call_global_client += _logic_msg_handle.forward_logic_call_global_client;
			_logic_call_gate.onforward_logic_call_group_client += _logic_msg_handle.forward_logic_call_group_client;

			_hub_msg_handle = new hub_msg_handle(_hubmanager, _clientmanager);
			_hub_call_gate = new module.hub_call_gate ();
			_hub_call_gate.onreg_hub += _hub_msg_handle.reg_hub;
			_hub_call_gate.onforward_hub_call_global_client += _hub_msg_handle.forward_hub_call_global_client;
			_hub_call_gate.onforward_hub_call_group_client += _hub_msg_handle.forward_hub_call_group_client;

			var _logic_hub_process = new juggle.process();
			_logic_hub_process.reg_module(_logic_call_gate);
			_logic_hub_process.reg_module(_hub_call_gate);
			var inside_ip = _config.get_value_string("inside_ip");
			var inside_port = (short)_config.get_value_int("inside_port");
			_accept_logic_hub_service = new service.acceptnetworkservice(inside_ip, inside_port, _logic_hub_process);

			var center_ip = _center_config.get_value_string("ip");
			var center_port = (short)_center_config.get_value_int("port");
			_center_call_server = new module.center_call_server();
			var _center_process = new juggle.process();
			_center_process.reg_module(_center_call_server);
			_connect_center_service = new service.connectnetworkservice(_center_process);
			var center_ch = _connect_center_service.connect(center_ip, center_port);
			_centerproxy = new centerproxy(center_ch);
			_center_msg_handle = new center_msg_handle(closeHandle, _centerproxy);
			_center_call_server.onreg_server_sucess += _center_msg_handle.reg_server_sucess;
			_center_call_server.onclose_server += _center_msg_handle.close_server;

			_juggle_service = new service.juggleservice();
			_juggle_service.add_process(_logic_hub_process);
			_juggle_service.add_process(_center_process);
			_juggle_service.add_process(_client_process);

			_centerproxy.reg_gate(inside_ip, inside_port, uuid);

            timer.addticktime(60 * 1000, _clientmanager.tick_client);
        }

		public void poll(Int64 tick)
		{
			_juggle_service.poll(tick);
			timer.poll(tick);
			_accept_logic_hub_service.poll(tick);
			_connect_center_service.poll(tick);
			_accept_client_service.poll(tick);
		}

		private static void Main(String[] args)
		{
			if (args.Length <= 0)
			{
				return;
			}

			gate _gate = new gate(args);

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

				_gate.poll(tick);

				if (closeHandle.is_close)
				{
					Console.WriteLine("server closed, hub server " + gate.uuid);
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

				if (ticktime < 50)
				{
					Thread.Sleep(15);
				}
			}
		}

		public static String name;

		public static String uuid;

		public static closehandle closeHandle;

		private service.acceptnetworkservice _accept_client_service;
		private module.client_call_gate _client_call_gate;
		private client_msg_handle _client_msg_handle;
		private clientmanager _clientmanager;

		private service.acceptnetworkservice _accept_logic_hub_service;
		private module.logic_call_gate _logic_call_gate;
		private logic_msg_handle _logic_msg_handle;
		private logicmanager _logicmanager;
		private module.hub_call_gate _hub_call_gate;
		private hub_msg_handle _hub_msg_handle;
		private hubmanager _hubmanager;

		private service.connectnetworkservice _connect_center_service;
		private module.center_call_server _center_call_server;
		private centerproxy _centerproxy;
		private center_msg_handle _center_msg_handle;

		private service.juggleservice _juggle_service;
		public static service.timerservice timer;
	}
}

