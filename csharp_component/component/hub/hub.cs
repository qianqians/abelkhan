using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace hub
{
	public class hub
	{
		public hub(String[] args)
		{
            _config = new abelkhan.config(args[0]);
			_center_config = _config.get_value_dict("center");
			if (args.Length > 1)
			{
                _root_config = _config;
                _config = _config.get_value_dict(args[1]);
			}
            name = args[1];

            var log_level = _config.get_value_string("log_level");
            if (log_level == "trace")
            {
                log.log.logMode = log.log.enLogMode.trace;
            }
            else if (log_level == "debug")
            {
                log.log.logMode = log.log.enLogMode.debug;
            }
            else if (log_level == "info")
            {
                log.log.logMode = log.log.enLogMode.info;
            }
            else if (log_level == "warn")
            {
                log.log.logMode = log.log.enLogMode.warn;
            }
            else if (log_level == "err")
            {
                log.log.logMode = log.log.enLogMode.err;
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
            
            _timer = new service.timerservice();
            _timer.refresh();

            _modules = new common.modulemanager();

            add_chs = new List<abelkhan.Ichannel>();
            chs = new List<abelkhan.Ichannel>();
            remove_chs = new List<abelkhan.Ichannel>();

            var ip = _config.get_value_string("ip");
            var port = (ushort)_config.get_value_int("port");
            _enetservice = new abelkhan.enetservice(ip, port);

            _closeHandle = new closehandle();
            _hubs = new hubmanager();
            _gates = new gatemanager(_enetservice);

            _hubs.on_hubproxy += (_proxy) =>
            {
                on_hubproxy?.Invoke(_proxy);
            };
            _hubs.on_hub_closed += (string name, string type) =>
            {
                on_hub_closed?.Invoke(name, type);
            };

            _gates.clientDisconnect += (client_cuuid) =>
            {
                on_client_disconnect?.Invoke(client_cuuid);
            };
            _gates.clientException += (client_cuuid) =>
            {
                on_client_exception?.Invoke(client_cuuid);
            };
            _gates.directClientDisconnect += (client_cuuid) =>
            {
                on_direct_client_disconnect?.Invoke(client_cuuid);
            };

            var center_ip = _center_config.get_value_string("ip");
			var center_port = (short)_center_config.get_value_int("port");
			var _socket = abelkhan.connectservice.connect(System.Net.IPAddress.Parse(center_ip), center_port);
            var center_ch = new abelkhan.rawchannel(_socket);
            _centerproxy = new centerproxy(center_ch);
            lock (add_chs)
            {
                add_chs.Add(center_ch);
            }
            heartbeat(service.timerservice.Tick);

            if (_config.has_key("tcp_listen"))
            {
                var tcp_listen = _config.get_value_bool("tcp_listen");
                if (tcp_listen)
                {
                    var tcp_outside_ip = _config.get_value_string("tcp_outside_ip");
                    var tcp_outside_port = (ushort)_config.get_value_int("tcp_outside_port");
                    _cryptacceptservice = new abelkhan.cryptacceptservice(tcp_outside_port);
                    _cryptacceptservice.on_connect += (ch) => {
                        lock (add_chs)
                        {
                            add_chs.Add(ch);
                        }
                    };
                    _cryptacceptservice.start();
                }
            }

            if (_config.has_key("websocket_listen"))
            {
                var is_websocket_listen = _config.get_value_bool("websocket_listen");
                if (is_websocket_listen)
                {
                    var websocket_outside_ip = _config.get_value_string("websocket_outside_ip");
                    var websocket_outside_port = (ushort)_config.get_value_int("websocket_outside_port");
                    var is_ssl = _config.get_value_bool("is_ssl");
                    string pfx = "";
                    if (is_ssl) {
                        pfx = _config.get_value_string("pfx");
                    }
                    _websocketacceptservice = new abelkhan.websocketacceptservice(websocket_outside_port, is_ssl, pfx);
                    _websocketacceptservice.on_connect += (ch) =>
                    {
                        lock (add_chs)
                        {
                            add_chs.Add(ch);
                        }
                    };
                }
            }

            _hub_msg_handle = new hub_msg_handle(_hubs);
            _center_msg_handle = new center_msg_handle(this, _closeHandle, _centerproxy);
            _dbproxy_msg_handle = new dbproxy_msg_handle();
            _gate_msg_handle = new gate_msg_handle();

            _centerproxy.reg_hub(ip, port, name);
        }

        public void heartbeat(long _)
        {
            _centerproxy.heartbeat();
            _timer.addticktime(3 * 1000, heartbeat);
        }

        public event Action onConnectDB;
        public void onConnectDB_event()
        {
            onConnectDB?.Invoke();
        }

        public event Action onConnectExtendDB;
        public void onConnectExtendDB_event()
        {
            onConnectExtendDB?.Invoke();
        }

        public event Action onCloseServer;
        public void onCloseServer_event()
        {
            onCloseServer?.Invoke();
        }

        public void closeSvr()
        {
            _centerproxy.closed();

            _timer.addticktime(3 * 1000, (tick) =>
            {
                _closeHandle.is_close = true;
            });
        }

        public event Action<string> onReload;
        public void onReload_event(string argv)
        {
            onReload?.Invoke(argv);
        }

        public void connect_dbproxy(string dbproxy_name, string db_ip, short db_port)
		{
            if (_config.has_key("dbproxy") && _config.get_value_string("dbproxy") == dbproxy_name)
            {
                var dbproxy_cfg = _root_config.get_value_dict(dbproxy_name);
                var _db_ip = dbproxy_cfg.get_value_string("ip");
                var _db_port = (short)dbproxy_cfg.get_value_int("port");
                if (db_ip != _db_ip || db_port != _db_port)
                {
                    log.log.err("dbproxy:{0}, wrong ip:{1}, port:{2}!", dbproxy_name, db_ip, db_port);
                    return;
                }

                var _socket = abelkhan.connectservice.connect(System.Net.IPAddress.Parse(db_ip), db_port);
                var _db_ch = new abelkhan.rawchannel(_socket);
                _dbproxy = new dbproxyproxy(_db_ch);
                lock (add_chs)
                {
                    add_chs.Add(_db_ch);
                }
                _dbproxy.reg_hub(name);
                _dbproxy.on_connect_dbproxy_sucessed += onConnectDBProxySucessed;
            }
            else if (_config.has_key("extend_dbproxy") && _config.get_value_string("extend_dbproxy") == dbproxy_name)
            {
                var dbproxy_cfg = _root_config.get_value_dict(dbproxy_name);
                var _db_ip = dbproxy_cfg.get_value_string("ip");
                var _db_port = (short)dbproxy_cfg.get_value_int("port");
                if (db_ip != _db_ip || db_port != _db_port)
                {
                    log.log.err("dbproxy:{0}, wrong ip:{1}, port:{2}!", dbproxy_name, db_ip, db_port);
                    return;
                }

                var _socket = abelkhan.connectservice.connect(System.Net.IPAddress.Parse(db_ip), db_port);
                var _db_ch = new abelkhan.rawchannel(_socket);
                lock (add_chs)
                {
                    add_chs.Add(_db_ch);
                }
                _extend_dbproxy = new dbproxyproxy(_db_ch);
                _extend_dbproxy.reg_hub(name);
                _extend_dbproxy.on_connect_dbproxy_sucessed += onConnectExtendDBProxySucessed;
            }
        }

        public event Action onDBProxyInit;
        public void onConnectDBProxySucessed()
        {
            onDBProxyInit?.Invoke();
        }

        public event Action onExtendDBProxyInit;
        public void onConnectExtendDBProxySucessed()
        {
            onExtendDBProxyInit?.Invoke();
        }

        public void reg_hub(String hub_ip, short hub_port)
        {
            _enetservice.connect(hub_ip, (ushort)hub_port, (ch)=> {
                var _caller = new abelkhan.hub_call_hub_caller(ch, abelkhan.modulemng_handle._modulemng);
                _caller.reg_hub(name, type);
            });
        }

		public Int64 poll()
        {
            
            Int64 tick_begin = _timer.refresh();

            try
            {
                _timer.poll();

                lock (add_chs)
                {
                    foreach (var ch in add_chs)
                    {
                        chs.Add(ch);
                    }
                    add_chs.Clear();
                }

                foreach (var ch in chs)
                {
                    while (true)
                    {
                        ArrayList ev = null;
                        lock (ch)
                        {
                            ev = ch.pop();
                        }
                        if (ev == null)
                        {
                            break;
                        }
                        abelkhan.modulemng_handle._modulemng.process_event(ch, ev);
                    }
                }

                lock (remove_chs)
                {
                    foreach (var ch in remove_chs)
                    {
                        chs.Remove(ch);
                    }
                    remove_chs.Clear();
                }
            }
            catch(abelkhan.Exception e)
            {
                log.log.err(e.Message);
            }
            catch (System.Exception e)
            {
                log.log.err("{0}", e);
            }

            Int64 tick_end = _timer.refresh();
            Int64 poll_tick = tick_end - tick_begin;

            if (poll_tick > 50)
            {
                log.log.trace("poll_tick:{0}", poll_tick);
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

				if (_closeHandle.is_close)
                {
                    log.log.info("server closed, hub server:{0}", hub.name);
					break;
				}
                
				if (ticktime < 50)
				{
					Thread.Sleep(5);
				}
			}
		}

		public static string name;
		public static string type;

        public static abelkhan.config _config;
        public static abelkhan.config _root_config;
        public static abelkhan.config _center_config;

        public static common.modulemanager _modules;

        public static List<abelkhan.Ichannel> add_chs;
        public static List<abelkhan.Ichannel> chs;
        public static List<abelkhan.Ichannel> remove_chs;

        private abelkhan.enetservice _enetservice;
        private abelkhan.cryptacceptservice _cryptacceptservice;
        private abelkhan.websocketacceptservice _websocketacceptservice;

        public static closehandle _closeHandle;
        public static hubmanager _hubs;
        public static gatemanager _gates;
        public static dbproxyproxy _dbproxy;
        public static dbproxyproxy _extend_dbproxy;
        public static service.timerservice _timer;

        private centerproxy _centerproxy;

        private center_msg_handle _center_msg_handle;
        private dbproxy_msg_handle _dbproxy_msg_handle;
        private hub_msg_handle _hub_msg_handle;
        private gate_msg_handle _gate_msg_handle;

        public event Action<hubproxy> on_hubproxy;
        public event Action<string, string> on_hub_closed;

        public event Action<string> on_client_disconnect;
        public event Action<string> on_client_exception;

        public event Action<string> on_direct_client_disconnect;
    }
}

