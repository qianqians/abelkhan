using ENet.Managed;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace hub
{
	public class hub
	{
        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            log.log.err("unhandle exception:{0}", ex.ToString());
        }

        public hub(string config_file, string _hub_name, string _hub_type)
		{
            _config = new abelkhan.config(config_file);
			_center_config = _config.get_value_dict("center");
			_root_config = _config;
            _config = _config.get_value_dict(_hub_name);
            name = $"{_hub_name}_{Guid.NewGuid().ToString("N")}";
            type = _hub_type;

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

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            _timer = new service.timerservice();
            _timer.refresh();

            _modules = new common.modulemanager();

            add_chs = new List<abelkhan.Ichannel>();
            remove_chs = new List<abelkhan.Ichannel>();

            _r = new Random();
            _dbproxys = new ConcurrentDictionary< string, dbproxyproxy>();

            var redismq_url = _root_config.get_value_string("redis_for_mq");
            _redis_mq_service = new abelkhan.redis_mq(redismq_url, name);
            _gates = new gatemanager(_redis_mq_service);

            _closeHandle = new closehandle();
            _hubs = new hubmanager();

            _hubs.on_hubproxy += (_proxy) =>
            {
                on_hubproxy?.Invoke(_proxy);
            };
            _hubs.on_hubproxy_reconn += (_proxy) =>
            {
                on_hubproxy_reconn?.Invoke(_proxy);
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

            var _center_ch = _redis_mq_service.connect(_center_config.get_value_string("name"));
            lock (add_chs)
            {
                add_chs.Add(_center_ch);
            }
            _centerproxy = new centerproxy(_center_ch);
            _centerproxy.reg_hub(() => {
                heartbeat(service.timerservice.Tick);
            });
           
            if (_config.has_key("tcp_listen"))
            {
                var tcp_listen = _config.get_value_bool("tcp_listen");
                if (tcp_listen)
                {
                    tcp_outside_address = new addressinfo();
                    tcp_outside_address.host = _config.get_value_string("tcp_outside_host");
                    tcp_outside_address.port = (ushort)_config.get_value_int("tcp_outside_port");
                    _cryptacceptservice = new abelkhan.cryptacceptservice(tcp_outside_address.port);
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
                    websocket_outside_address = new addressinfo();
                    websocket_outside_address.host = _config.get_value_string("websocket_outside_host");
                    websocket_outside_address.port = (ushort)_config.get_value_int("websocket_outside_port");
                    var is_ssl = _config.get_value_bool("is_ssl");
                    string pfx = "";
                    if (is_ssl) {
                        pfx = _config.get_value_string("pfx");
                    }
                    _websocketacceptservice = new abelkhan.websocketacceptservice(websocket_outside_address.port, is_ssl, pfx);
                    _websocketacceptservice.on_connect += (ch) =>
                    {
                        lock (add_chs)
                        {
                            add_chs.Add(ch);
                        }
                    };
                }
            }

            if (_config.has_key("enet_listen"))
            {
                var is_enet_listen = _config.get_value_bool("enet_listen");
                if (is_enet_listen)
                {
                    enet_outside_address = new addressinfo();
                    enet_outside_address.host = _config.get_value_string("enet_outside_host");
                    enet_outside_address.port = (ushort)_config.get_value_int("enet_outside_port");
                    _enetservice = new abelkhan.enetservice(enet_outside_address.host, enet_outside_address.port);
                    _enetservice.on_connect += (ch) => {
                        lock (add_chs)
                        {
                            add_chs.Add(ch);
                        }
                    };
                    _enetservice.start();
                }
            }

            _hub_msg_handle = new hub_msg_handle(_hubs, _gates);
            _center_msg_handle = new center_msg_handle(this, _closeHandle, _centerproxy);
            _dbproxy_msg_handle = new dbproxy_msg_handle();
            _gate_msg_handle = new gate_msg_handle();
            _client_msg_handle = new client_msg_handle();

            _gate_msg_handle.on_client_msg += (uuid) =>
            {
                on_client_msg?.Invoke(uuid);
            };
            _client_msg_handle.on_client_msg += (uuid) =>
            {
                on_client_msg?.Invoke(uuid);
            };
        }

        public void set_support_take_over_svr(bool is_support)
        {
            is_support_take_over_svr = is_support;
        }

        public Action onCenterCrash;
        private async void reconnect_center()
        {
            if (reconn_count > 5)
            {
                onCenterCrash?.Invoke();
            }

            reconn_count++;

            lock (remove_chs)
            {
                remove_chs.Add(_centerproxy._ch);
            }

            var _center_ch = _redis_mq_service.connect(_center_config.get_value_string("name"));
            lock (add_chs)
            {
                add_chs.Add(_center_ch);
            }
            _centerproxy = new centerproxy(_center_ch);
            if (await _centerproxy.reconn_reg_hub())
            {
                reconn_count = 0;
            }
        }

        public void heartbeat(long _)
        {
            do
            {
                if ((service.timerservice.Tick - _centerproxy.timetmp) > 6000)
                {
                    reconnect_center();
                    break;
                }

                _centerproxy.heartbeat();

            } while (false);

            _timer.addticktime(3000, heartbeat);
        }

        public event Action onCloseServer;
        public void onCloseServer_event()
        {
            onCloseServer?.Invoke();
        }

        public void closeSvr()
        {
            _centerproxy.closed();

            if (_cryptacceptservice != null)
            {
                _cryptacceptservice.close();
            }

            if (_redis_mq_service != null)
            {
                _redis_mq_service.close();
            }

            if (_enetservice != null)
            {
                _enetservice.stop();
                ManagedENet.Shutdown();
            }

            _timer.addticktime(3000, (tick) =>
            {
                _closeHandle.is_close = true;
            });
        }

        public event Action<string> onReload;
        public void onReload_event(string argv)
        {
            onReload?.Invoke(argv);
        }

        public void connect_dbproxy(string dbproxy_name)
        {
            var _db_ch = _redis_mq_service.connect(dbproxy_name);
            lock (add_chs)
            {
                add_chs.Add(_db_ch);
            }
            var _dbproxy = new dbproxyproxy(dbproxy_name, _db_ch);
            _dbproxy.reg_hub(name);
            _dbproxys.TryAdd(dbproxy_name, _dbproxy);

            if (_dbproxys.Count == 1)
            {
                _dbproxy.on_connect_dbproxy_sucessed += onConnectDBProxySucessed;
            }
        }

        public static uint randmon_uint(uint max)
        {
            return (uint)(_r.NextDouble() * max);
        }

        public static dbproxyproxy get_random_dbproxyproxy()
        {
            return _dbproxys.Values.ToArray()[randmon_uint((uint)_dbproxys.Count)];
        }

        public static dbproxyproxy get_dbproxy(string db_name)
        {
            return _dbproxys[db_name];
        }

        public static void dbproxy_closed(string db_name)
        {
            if (_dbproxys.Remove(db_name, out dbproxyproxy _p))
            {
                lock (remove_chs)
                {
                    remove_chs.Add(_p.ch);
                }
            }
        }

        public event Action onDBProxyInit;
        public void onConnectDBProxySucessed()
        {
            onDBProxyInit?.Invoke();
        }

        public void reg_hub(String hub_name)
        {
            var ch = _redis_mq_service.connect(hub_name);
            var _caller = new abelkhan.hub_call_hub_caller(ch, abelkhan.modulemng_handle._modulemng);
            _caller.reg_hub(name, type);
        }

        private Int64 poll()
        {
            
            Int64 tick_begin = _timer.refresh();

            try
            {
                _timer.poll();

                while (true)
                {
                    if (!abelkhan.event_queue.msgQue.TryDequeue(out Tuple<abelkhan.Ichannel, ArrayList> _event))
                    {
                        break;
                    }
                    abelkhan.modulemng_handle._modulemng.process_event(_event.Item1, _event.Item2);
                }

                lock (remove_chs)
                {
                    foreach (var ch in remove_chs)
                    {
                        add_chs.Remove(ch);
                    }
                    remove_chs.Clear();
                }

                abelkhan.TinyTimer.poll();
            }
            catch (abelkhan.Exception e)
            {
                log.log.err(e.Message);
            }
            catch (System.Exception e)
            {
                log.log.err("{0}", e);
            }

            Int64 tick_end = _timer.refresh();
            tick = (uint)(tick_end - tick_begin);

            if (tick > 50)
            {
                log.log.trace("poll_tick:{0}", tick);
            }

            return tick;
        }

        private object _run_mu = new object();
        public void run()
        {
            if (!Monitor.TryEnter(_run_mu))
            {
                throw new abelkhan.Exception("run mast at single thread!");
            }

            while (!_closeHandle.is_close)
            {
                var ticktime = poll();

                if (ticktime < 33)
                {
                    Thread.Sleep((int)(33 - ticktime));
                }
            }
            log.log.info("server closed, hub server:{0}", hub.name);

            Monitor.Exit(_run_mu);
        }

        public class addressinfo
        {
            public string host;
            public ushort port;
        }

		public static string name;
        public static string type;
        public static uint tick;
        public static addressinfo tcp_outside_address = null;
        public static addressinfo websocket_outside_address = null;
        public static addressinfo enet_outside_address = null;

        public static abelkhan.config _config;
        public static abelkhan.config _root_config;
        public static abelkhan.config _center_config;

        public static common.modulemanager _modules;

        public static List<abelkhan.Ichannel> add_chs;
        public static List<abelkhan.Ichannel> remove_chs;

        public static bool is_support_take_over_svr = true;
        public static abelkhan.redis_mq _redis_mq_service;

        private static Random _r;
        private static ConcurrentDictionary<string, dbproxyproxy> _dbproxys;

        private abelkhan.enetservice _enetservice;
        private abelkhan.cryptacceptservice _cryptacceptservice;
        private abelkhan.websocketacceptservice _websocketacceptservice;

        public static closehandle _closeHandle;
        public static hubmanager _hubs;
        public static gatemanager _gates;
        public static service.timerservice _timer;

        private uint reconn_count = 0;
        private centerproxy _centerproxy;

        private center_msg_handle _center_msg_handle;
        private dbproxy_msg_handle _dbproxy_msg_handle;
        private hub_msg_handle _hub_msg_handle;
        private gate_msg_handle _gate_msg_handle;
        private client_msg_handle _client_msg_handle;

        public event Action<hubproxy> on_hubproxy;
        public event Action<hubproxy> on_hubproxy_reconn;
        public event Action<string, string> on_hub_closed;

        public event Action<string> on_client_disconnect;
        public event Action<string> on_client_exception;
        public event Action<string> on_client_msg;

        public event Action<string> on_direct_client_disconnect;
    }
}

