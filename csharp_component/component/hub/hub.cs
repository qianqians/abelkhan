using Abelkhan;
using ENet.Managed;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Hub
{
	public class Hub
	{
        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Exception ex = e.ExceptionObject as System.Exception;
            Log.Log.err("unhandle exception:{0}", ex.ToString());
        }

        public Hub(string config_file, string _hub_name, string _hub_type, string _router_type)
		{
            _config = new Abelkhan.Config(config_file);
			_center_config = _config.get_value_dict("center");
			_root_config = _config;
            _config = _config.get_value_dict(_hub_name);
            name = $"{_hub_name}_{Guid.NewGuid().ToString("N")}";
            type = _hub_type;
            router_type = _router_type;

            var log_level = _config.get_value_string("log_level");
            if (log_level == "trace")
            {
                Log.Log.logMode = Log.Log.enLogMode.trace;
            }
            else if (log_level == "debug")
            {
                Log.Log.logMode = Log.Log.enLogMode.debug;
            }
            else if (log_level == "info")
            {
                Log.Log.logMode = Log.Log.enLogMode.info;
            }
            else if (log_level == "warn")
            {
                Log.Log.logMode = Log.Log.enLogMode.warn;
            }
            else if (log_level == "err")
            {
                Log.Log.logMode = Log.Log.enLogMode.err;
            }
            var log_file = _config.get_value_string("log_file");
            Log.Log.logFile = log_file;
            var log_dir = _config.get_value_string("log_dir");
            Log.Log.logPath = log_dir;
            {
                if (!System.IO.Directory.Exists(log_dir))
                {
                    System.IO.Directory.CreateDirectory(log_dir);
                }
            }

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            _timer = new Service.Timerservice();
            _timer.refresh();

            _modules = new Common.ModuleManager();

            add_chs = new List<Abelkhan.Ichannel>();
            remove_chs = new List<Abelkhan.Ichannel>();

            _r = new Random();
            _dbproxys = new ConcurrentDictionary<string, DBProxyProxy>();

            var redismq_url = _root_config.get_value_string("redis_for_mq");
            if (!_root_config.has_key("redis_for_mq_pwd"))
            {
                _redis_mq_service = new Abelkhan.RedisMQ(_timer, redismq_url, string.Empty, name, 333);
            }
            else
            {
                _redis_mq_service = new Abelkhan.RedisMQ(_timer, redismq_url, _root_config.get_value_string("redis_for_mq_pwd"), name, 333);
            }

            var redis_for_cache = _root_config.get_value_string("redis_for_cache");
            if (!_root_config.has_key("redis_for_cache_pwd"))
            {
                _redis_handle = new Abelkhan.RedisHandle(redis_for_cache, string.Empty);
            }
            else
            {
                _redis_handle = new Abelkhan.RedisHandle(redis_for_cache, _root_config.get_value_string("redis_for_cache_pwd"));
            }
            

            _gates = new GateManager(_redis_mq_service);

            _closeHandle = new CloseHandle();
            _hubs = new HubManager();

            _hubs.on_hubproxy += (_proxy) =>
            {
                check_connnect_hub(_proxy);
                on_hubproxy?.Invoke(_proxy);
            };
            _hubs.on_hubproxy_reconn += (_proxy) =>
            {
                check_connnect_hub(_proxy);
                on_hubproxy_reconn?.Invoke(_proxy);
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
            _centerproxy = new CenterProxy(_center_ch);
            _centerproxy.reg_hub(() => {
                heartbeat(Service.Timerservice.Tick);
            });

            if (_config.has_key("tcp_inside_listen"))
            {
                var tcp_inside_listen = _config.get_value_bool("tcp_inside_listen");
                if (tcp_inside_listen)
                {
                    tcp_inside_address = new Addressinfo();
                    tcp_inside_address.host = _config.get_value_string("tcp_inside_host");
                    tcp_inside_address.port = (ushort)_config.get_value_int("tcp_inside_port");
                    _acceptservice = new Abelkhan.Acceptservice(tcp_inside_address.port);
                    Acceptservice.on_connect += (ch) => {
                        lock (add_chs)
                        {
                            add_chs.Add(ch);
                        }
                    };
                    _acceptservice.start();

                    flush_host();
                }
            }
           
            if (_config.has_key("tcp_listen"))
            {
                var tcp_listen = _config.get_value_bool("tcp_listen");
                if (tcp_listen)
                {
                    tcp_outside_address = new Addressinfo();
                    tcp_outside_address.host = _config.get_value_string("tcp_outside_host");
                    tcp_outside_address.port = (ushort)_config.get_value_int("tcp_outside_port");
                    _cryptacceptservice = new Abelkhan.CryptAcceptService(tcp_outside_address.port);
                    CryptAcceptService.on_connect += (ch) => {
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
                    websocket_outside_address = new Addressinfo();
                    websocket_outside_address.host = _config.get_value_string("websocket_outside_host");
                    websocket_outside_address.port = (ushort)_config.get_value_int("websocket_outside_port");
                    var is_ssl = _config.get_value_bool("is_ssl");
                    string pfx = "";
                    string pwd = "";
                    if (is_ssl)
                    {
                        pfx = _config.get_value_string("pfx");
                        pwd = _config.get_value_string("pwd");
                    }
                    _websocketacceptservice = new Abelkhan.WebsocketAcceptService(websocket_outside_address.port, is_ssl, pfx, pwd);
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
                    ManagedENet.Startup();

                    enet_outside_address = new Addressinfo();
                    enet_outside_address.host = _config.get_value_string("enet_outside_host");
                    enet_outside_address.port = (ushort)_config.get_value_int("enet_outside_port");

                    _enetservice = new Abelkhan.EnetService(enet_outside_address.host, enet_outside_address.port);
                    _enetservice.on_connect += (ch) => {
                        lock (add_chs)
                        {
                            add_chs.Add(ch);
                        }
                    };
                    _enetservice.start();
                }
            }

            if (_config.has_key("http_listen"))
            {
                var is_http_listen = _config.get_value_bool("http_listen");
                if (is_http_listen)
                {
                    http_outside_address = new Addressinfo();
                    http_outside_address.host = _config.get_value_string("http_outside_host");
                    http_outside_address.port = (ushort)_config.get_value_int("http_outside_port");
                    _httpservice = new Service.HttpService(http_outside_address.host, http_outside_address.port);
                    _httpservice.run();
                }
            }

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

        private async void check_connnect_hub(HubProxy _proxy)
        {
            if (OnCheckConnHub != null && OnCheckConnHub(_proxy.name) && _proxy._tcp_ch == null)
            {
                string host = await _redis_handle.GetStrData(name);
                var host_info = host.Split(":");
                var s = ConnectService.connect(IPAddress.Parse(host_info[0]), short.Parse(host_info[1]));
                var ch = new Abelkhan.RawChannel(s);
                var _caller = new Abelkhan.hub_call_hub_caller(ch, Abelkhan.ModuleMgrHandle._modulemng);
                _caller.reg_hub(name, type);

                _hubs.replace_hub(_proxy.name, _proxy.type, ch);
            }
        }

        private void flush_host(long _ = 0)
        {
            var host = $"{tcp_inside_address.host}:{tcp_inside_address.port}";
            _redis_handle.SetStrData(name, host, 60);

            _timer.addticktime(40000, flush_host);
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
            _centerproxy = new CenterProxy(_center_ch);
            if (await _centerproxy.reconn_reg_hub())
            {
                reconn_count = 0;
            }
        }

        public void heartbeat(long _)
        {
            do
            {
                if ((Service.Timerservice.Tick - _centerproxy.timetmp) > 9000)
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

            _cryptacceptservice?.close();
            _redis_mq_service?.close(); 
            _httpservice?.close();

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
            var _dbproxy = new DBProxyProxy(dbproxy_name, _db_ch);
            _dbproxy.reg_hub(name);
            _dbproxys.TryAdd(dbproxy_name, _dbproxy);

            if (_dbproxys.Count == 1)
            {
                _dbproxy.on_connect_dbproxy_sucessed += onConnectDBProxySucessed;
            }
        }

        public static uint randmon_uint(uint max)
        {
            return (uint)_r.NextInt64((int)max);
        }

        public static DBProxyProxy get_random_dbproxyproxy()
        {
            return _dbproxys.Values.ToArray()[randmon_uint((uint)_dbproxys.Count)];
        }

        public static DBProxyProxy get_dbproxy(string db_name)
        {
            return _dbproxys[db_name];
        }

        public static void dbproxy_closed(string db_name)
        {
            if (_dbproxys.Remove(db_name, out DBProxyProxy _p))
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
            if (!_hubs.get_hub(hub_name, out var _))
            {
                var ch = _redis_mq_service.connect(hub_name);
                var _caller = new Abelkhan.hub_call_hub_caller(ch, Abelkhan.ModuleMgrHandle._modulemng);
                _caller.reg_hub(name, type);
            }
        }

        public static async Task migrate_client(string client_uuid, string src_hub)
        {
            await on_migrate_client.Invoke(client_uuid, src_hub);
        }

        private async ValueTask<long> poll()
        {
            long tick_begin = _timer.refresh();

            while (Abelkhan.EventQueue.msgQue.TryDequeue(out Tuple<Abelkhan.Ichannel, ArrayList> _event))
            {
                Abelkhan.ModuleMgrHandle._modulemng.process_event(_event.Item1, _event.Item2);
            }

            if (remove_chs.Count > 0)
            {
                lock (remove_chs)
                {
                    foreach (var ch in remove_chs)
                    {
                        add_chs.Remove(ch);
                    }
                    remove_chs.Clear();
                }
            }

            _ = await _redis_mq_service.sendmsg_mq();

            Abelkhan.TinyTimer.poll();

            tick = (uint)(_timer.poll() - tick_begin);
            if (tick > 50)
            {
                Log.Log.trace("poll_tick:{0}", tick);
            }

            return tick;
        }

        private async Task _run()
        {
            while (!_closeHandle.is_close)
            {
                var ticktime = await poll();

                if (ticktime < 33)
                {
                    Thread.Sleep((int)(33 - ticktime));
                }
            }

            Log.Log.info("server closed, hub server:{0}", Hub.name);
            Log.Log.close();
        }

        private readonly object _run_mu = new();
        public async Task run()
        {
            if (!Monitor.TryEnter(_run_mu))
            {
                throw new Abelkhan.Exception("run mast at single thread!");
            }

            if (_config.has_key("prometheus_port"))
            {
                _prometheus = new Service.PrometheusMetric((short)_config.get_value_int("prometheus_port"));
                _prometheus.Start();
            }

            var _hub_msg_handle = new hub_msg_handle(_hubs, _gates);
            var _center_msg_handle = new center_msg_handle(this, _closeHandle, _centerproxy);
            var _dbproxy_msg_handle = new dbproxy_msg_handle();

            rerun:
            try
            {
                await _run();
            }
            catch (Abelkhan.Exception e)
            {
                Log.Log.err(e.Message);
                goto rerun;
            }
            catch (System.Exception e)
            {
                Log.Log.err("{0}", e);
                goto rerun;
            }

            Monitor.Exit(_run_mu);
        }

        public class Addressinfo
        {
            public string host;
            public ushort port;
        }

        public static uint tick;

        public static string name;
        public static string type;
        public static string router_type;
        public static Addressinfo tcp_outside_address = null;
        public static Addressinfo websocket_outside_address = null;
        public static Addressinfo enet_outside_address = null;
        public static Addressinfo http_outside_address = null;

        public static Addressinfo tcp_inside_address = null;

        public static Abelkhan.Config _config;
        public static Abelkhan.Config _root_config;
        public static Abelkhan.Config _center_config;

        public static Common.ModuleManager _modules;

        public static List<Abelkhan.Ichannel> add_chs;
        public static List<Abelkhan.Ichannel> remove_chs;

        public static bool is_support_take_over_svr = true;
        public static Abelkhan.RedisMQ _redis_mq_service;
        public static Abelkhan.RedisHandle _redis_handle;
        public static Service.PrometheusMetric _prometheus;

        private static Random _r;
        private static ConcurrentDictionary<string, DBProxyProxy> _dbproxys;

        public static CloseHandle _closeHandle;
        public static HubManager _hubs;
        public static GateManager _gates;
        public static Service.Timerservice _timer;

        public static Func<string, bool> OnCheckConnHub;
        public static Func<bool> OnCheckConnGate;

        private readonly Abelkhan.EnetService _enetservice;
        private readonly Abelkhan.CryptAcceptService _cryptacceptservice;
        private readonly Abelkhan.WebsocketAcceptService _websocketacceptservice;
        private readonly Service.HttpService _httpservice;

        private readonly Abelkhan.Acceptservice _acceptservice;

        private uint reconn_count = 0;
        private CenterProxy _centerproxy;

        private readonly gate_msg_handle _gate_msg_handle;
        private readonly client_msg_handle _client_msg_handle;

        public event Action<HubProxy> on_hubproxy;
        public event Action<HubProxy> on_hubproxy_reconn;

        public event Action<string> on_client_disconnect;
        public event Action<string> on_client_exception;
        public event Action<string> on_client_msg;

        public event Action<string> on_direct_client_disconnect;

        public static event Func<string, string, Task> on_migrate_client;

    }
}

