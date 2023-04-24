﻿using Abelkhan;
using ENet.Managed;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        public Hub(string config_file, string _hub_name, string _hub_type)
		{
            _config = new Abelkhan.Config(config_file);
			_center_config = _config.get_value_dict("center");
			_root_config = _config;
            _config = _config.get_value_dict(_hub_name);
            name = $"{_hub_name}_{Guid.NewGuid().ToString("N")}";
            type = _hub_type;

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
            _redis_mq_service = new Abelkhan.RedisMQ(_timer, redismq_url, name);
            _gates = new GateManager(_redis_mq_service);

            _closeHandle = new CloseHandle();
            _hubs = new HubManager();

            _hubs.on_hubproxy += (_proxy) =>
            {
                on_hubproxy?.Invoke(_proxy);
            };
            _hubs.on_hubproxy_reconn += (_proxy) =>
            {
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
                    if (is_ssl) {
                        pfx = _config.get_value_string("pfx");
                    }
                    _websocketacceptservice = new Abelkhan.WebsocketAcceptService(websocket_outside_address.port, is_ssl, pfx);
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

            if (_httpservice != null)
            {
                _httpservice.close();
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
            var ch = _redis_mq_service.connect(hub_name);
            var _caller = new Abelkhan.hub_call_hub_caller(ch, Abelkhan.ModuleMgrHandle._modulemng);
            _caller.reg_hub(name, type);
        }

        private List<Task> wait_task = new ();
        private long poll()
        {
            
            long tick_begin = _timer.refresh();

            try
            {
                _timer.poll();

                while (true)
                {
                    if (!Abelkhan.EventQueue.msgQue.TryDequeue(out Tuple<Abelkhan.Ichannel, ArrayList> _event))
                    {
                        break;
                    }
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

                Abelkhan.TinyTimer.poll();
            }
            catch (Abelkhan.Exception e)
            {
                Log.Log.err(e.Message);
            }
            catch (System.Exception e)
            {
                Log.Log.err("{0}", e);
            }

            long tick_end = _timer.refresh();
            tick = (uint)(tick_end - tick_begin);

            if (tick > 50)
            {
                Log.Log.trace("poll_tick:{0}", tick);
            }

            return tick;
        }

        private readonly object _run_mu = new();
        public void run()
        {
            if (!Monitor.TryEnter(_run_mu))
            {
                throw new Abelkhan.Exception("run mast at single thread!");
            }

            while (!_closeHandle.is_close)
            {
                var ticktime = poll();

                if (ticktime < 33)
                {
                    Thread.Sleep((int)(33 - ticktime));
                }
            }
            Log.Log.info("server closed, hub server:{0}", Hub.name);
            Log.Log.close();

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
        public static Addressinfo tcp_outside_address = null;
        public static Addressinfo websocket_outside_address = null;
        public static Addressinfo enet_outside_address = null;
        public static Addressinfo http_outside_address = null;

        public static Abelkhan.Config _config;
        public static Abelkhan.Config _root_config;
        public static Abelkhan.Config _center_config;

        public static Common.ModuleManager _modules;

        public static List<Abelkhan.Ichannel> add_chs;
        public static List<Abelkhan.Ichannel> remove_chs;

        public static bool is_support_take_over_svr = true;
        public static Abelkhan.RedisMQ _redis_mq_service;

        private static Random _r;
        private static ConcurrentDictionary<string, DBProxyProxy> _dbproxys;

        public static CloseHandle _closeHandle;
        public static HubManager _hubs;
        public static GateManager _gates;
        public static Service.Timerservice _timer;

        private readonly Abelkhan.EnetService _enetservice;
        private readonly Abelkhan.CryptAcceptService _cryptacceptservice;
        private readonly Abelkhan.WebsocketAcceptService _websocketacceptservice;
        private readonly Service.HttpService _httpservice;

        private uint reconn_count = 0;
        private CenterProxy _centerproxy;

        private readonly center_msg_handle _center_msg_handle;
        private readonly dbproxy_msg_handle _dbproxy_msg_handle;
        private readonly hub_msg_handle _hub_msg_handle;
        private readonly gate_msg_handle _gate_msg_handle;
        private readonly client_msg_handle _client_msg_handle;

        public event Action<HubProxy> on_hubproxy;
        public event Action<HubProxy> on_hubproxy_reconn;

        public event Action<string> on_client_disconnect;
        public event Action<string> on_client_exception;
        public event Action<string> on_client_msg;

        public event Action<string> on_direct_client_disconnect;
    }
}

