using System;
using System.Threading;

namespace gate
{
	public class gate
	{
        public void onUdpChannelConnect(juggle.Ichannel ch)
        {
            udpchannels.add_udpchannel(ch);
        }

        public void onClientDissconnect(juggle.Ichannel ch)
        {
            clients.on_client_disconnect(ch);
        }

        public void onChannelConnect(juggle.Ichannel ch)
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "onChannelConnect");
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

            enable_heartbeats = _config.get_value_bool("heartbeats");

            timer = new service.timerservice();
            _hubmanager = new hubmanager();
            clients = new clientmanager ();
            udpchannels = new udpchannelmanager();

            _client_msg_handle = new client_msg_handle (clients, _hubmanager, timer);
			_client_call_gate = new module.client_call_gate ();
            _client_call_gate.onconnect_server += _client_msg_handle.connect_server;
			_client_call_gate.oncancle_server += _client_msg_handle.cancle_server;
            _client_call_gate.onenable_heartbeats += _client_msg_handle.enable_heartbeats;
            _client_call_gate.ondisable_heartbeats += _client_msg_handle.disable_heartbeats;
            _client_call_gate.onconnect_hub += _client_msg_handle.connect_hub;
            _client_call_gate.ondisconnect_hub += _client_msg_handle.disconnect_hub;
            _client_call_gate.onforward_client_call_hub += _client_msg_handle.forward_client_call_hub;
            _client_call_gate.onheartbeats += _client_msg_handle.heartbeats;
            var _client_process = new juggle.process();
			_client_process.reg_module(_client_call_gate);

            var outside_ip = _config.get_value_string("outside_ip");
			var outside_port = (short)_config.get_value_int("outside_port");
			_accept_client_service = new service.acceptnetworkservice(outside_ip, outside_port, _client_process);
            _accept_client_service.onChannelConnect += onChannelConnect;
            _accept_client_service.onChannelDisconnect += onClientDissconnect;
            
            _client_call_gate_fast = new module.client_call_gate_fast();
            _client_call_gate_fast.onrefresh_udp_end_point += _client_msg_handle.refresh_udp_end_point;
            _client_call_gate_fast.onconfirm_create_udp_link += _client_msg_handle.confirm_create_udp_link;
            var _udp_client_process = new juggle.process();
            _udp_client_process.reg_module(_client_call_gate_fast);

            var udp_outside_ip = _config.get_value_string("udp_outside_ip");
            var udp_outside_port = (short)_config.get_value_int("udp_outside_port");
            _udp_accept_service = new service.udpacceptnetworkservice(udp_outside_ip, udp_outside_port, _udp_client_process);
            _udp_accept_service.onChannelConnect += onUdpChannelConnect;

            _hub_msg_handle = new hub_msg_handle(_hubmanager, clients);
			_hub_call_gate = new module.hub_call_gate ();
			_hub_call_gate.onreg_hub += _hub_msg_handle.reg_hub;
            _hub_call_gate.onconnect_sucess += _hub_msg_handle.connect_sucess;
            _hub_call_gate.ondisconnect_client += _hub_msg_handle.disconnect_client;
            _hub_call_gate.onforward_hub_call_client += _hub_msg_handle.forward_hub_call_client;
            _hub_call_gate.onforward_hub_call_global_client += _hub_msg_handle.forward_hub_call_global_client;
			_hub_call_gate.onforward_hub_call_group_client += _hub_msg_handle.forward_hub_call_group_client;
            _hub_call_gate.onforward_hub_call_client_fast += _hub_msg_handle.forward_hub_call_client_fast;
            _hub_call_gate.onforward_hub_call_group_client_fast += _hub_msg_handle.forward_hub_call_group_client_fast;

            var _logic_hub_process = new juggle.process();
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
            _juggle_service.add_process(_udp_client_process);

            _centerproxy.reg_gate(inside_ip, inside_port, uuid);

            if (enable_heartbeats)
            {
                timer.addticktime(5 * 1000, clients.tick_client);
            }
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
            catch(System.Exception e)
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
				return;
			}

			gate _gate = new gate(args);

			Int64 oldtick = 0;
			Int64 tick = 0;
			while (true)
			{
                oldtick = tick;
                tick = _gate.poll();

				if (closeHandle.is_close)
                {
                    log.log.operation(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "server closed, gate server:{0}", gate.uuid);
					break;
				}
                
				Int64 ticktime = (tick - oldtick);
				if (ticktime < 50)
				{
					Thread.Sleep(15);
				}
			}
		}

		public static String name;
		public static String uuid;
		public static closehandle closeHandle;
        public static bool enable_heartbeats;

        private service.acceptnetworkservice _accept_client_service;
		private module.client_call_gate _client_call_gate;
        private service.udpacceptnetworkservice _udp_accept_service;
        private module.client_call_gate_fast _client_call_gate_fast;
        private client_msg_handle _client_msg_handle;
        public static clientmanager clients;
        public static udpchannelmanager udpchannels;

        private service.acceptnetworkservice _accept_logic_hub_service;
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

