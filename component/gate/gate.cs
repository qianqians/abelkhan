using System;
using System.Threading;

namespace gate
{
	public class gate
	{
        public void onClientDissconnect(juggle.Ichannel ch)
        {
            clients.on_client_disconnect(ch);
        }

        public void onChannelConnect(juggle.Ichannel ch)
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "onChannelConnect");

            service.channel _ch = ch as service.channel;
            _ch.compress_and_encrypt = (byte[] input) => { return common.compress_and_encrypt.CompressAndEncrypt(input, xor_key); };
            _ch.unencrypt_and_uncompress = (byte[] input) => { return common.compress_and_encrypt.UnEncryptAndUnCompress(input, xor_key); };

            clients.add_wait_channel(ch);
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
            xor_key = (byte)(_config.get_value_int("key") % 256);

            timer = new service.timerservice();
            _hubmanager = new hubmanager();
            clients = new clientmanager ();

            _client_msg_handle = new client_msg_handle (clients, _hubmanager, timer);
			_client_call_gate = new module.client_call_gate ();
            _client_call_gate.onconnect_server += _client_msg_handle.connect_server;
			_client_call_gate.oncancle_server += _client_msg_handle.cancle_server;
            _client_call_gate.onenable_heartbeats += _client_msg_handle.enable_heartbeats;
            _client_call_gate.ondisable_heartbeats += _client_msg_handle.disable_heartbeats;
            _client_call_gate.onforward_client_call_hub += _client_msg_handle.forward_client_call_hub;
            _client_call_gate.onheartbeats += _client_msg_handle.heartbeats;
            var _client_process = new juggle.process();
			_client_process.reg_module(_client_call_gate);

            var outside_ip = _config.get_value_string("outside_ip");
			var outside_port = (short)_config.get_value_int("outside_port");
			_accept_client_service = new service.acceptnetworkservice(outside_ip, outside_port, _client_process);
            _accept_client_service.onChannelConnect += onChannelConnect;
            _accept_client_service.onChannelDisconnect += onClientDissconnect;

            _hub_msg_handle = new hub_msg_handle(_hubmanager, clients);
			_hub_call_gate = new module.hub_call_gate ();
			_hub_call_gate.onreg_hub += _hub_msg_handle.reg_hub;
            _hub_call_gate.onconnect_sucess += _hub_msg_handle.connect_sucess;
            _hub_call_gate.ondisconnect_client += _hub_msg_handle.disconnect_client;
            _hub_call_gate.onforward_hub_call_client += _hub_msg_handle.forward_hub_call_client;
            _hub_call_gate.onforward_hub_call_global_client += _hub_msg_handle.forward_hub_call_global_client;
			_hub_call_gate.onforward_hub_call_group_client += _hub_msg_handle.forward_hub_call_group_client;

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

            _centerproxy.reg_gate(inside_ip, inside_port, uuid, (int)_config.get_value_int("zone_id"));

            if (enable_heartbeats)
            {
                timer.addticktime(5 * 1000, clients.tick_client);
            }
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
            catch(System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), tick_begin, "{0}", e);
            }

            System.GC.Collect();

            Int64 tick_end = timer.refresh();

            return tick_end - tick_begin;
        }

		private static void Main(String[] args)
		{
			if (args.Length <= 0)
			{
				return;
			}

			gate _gate = new gate(args);
            
			while (true)
			{
                var ticktime = _gate.poll();

				if (closeHandle.is_close)
                {
                    log.log.operation(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "server closed, gate server:{0}", gate.uuid);
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
        public static bool enable_heartbeats;

        public byte xor_key;

        private service.acceptnetworkservice _accept_client_service;
		private module.client_call_gate _client_call_gate;
        private client_msg_handle _client_msg_handle;
        public static clientmanager clients;

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

