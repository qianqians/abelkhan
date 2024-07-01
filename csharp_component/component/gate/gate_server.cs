using Abelkhan;
using ENet.Managed;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gate {

    public class GateService {
        public event Action sig_close_server;
        public event Action sig_center_crash;

        public CloseHandle _closehandle;

        public name_info gate_name_info;

	    private uint reconn_count;
        private uint tick;

        private readonly Config _root_config;
        private readonly Config _center_config;
        private readonly Config _config;

        private Service.Timerservice _timerservice;
        private HubSvrManager _hubsvrmanager;
        private ClientManager _clientmanager;

        private Abelkhan.RedisMQ _hub_redismq_service;
        private Abelkhan.RedisHandle _redis_handle;

        private Abelkhan.Acceptservice _hub_service;

        private CenterProxy _centerproxy;

        private Abelkhan.CryptAcceptService _client_service;
        private Abelkhan.WebsocketAcceptService _websocket_service;
        private Abelkhan.EnetService _enet_service;

        private static readonly List<Abelkhan.Ichannel> add_chs = new();
        private static readonly List<Abelkhan.Ichannel> remove_chs = new();

        public void on_close_server()
        {
            sig_close_server.Invoke();

            _timerservice.addticktime(5000, (long tick) => {
                close_svr();
            });
        }

        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Exception ex = e.ExceptionObject as System.Exception;
            Log.Log.err("unhandle exception:{0}", ex.ToString());
        }

        public GateService(string config_file_path, string config_name)
        {
            _root_config = new Config(config_file_path);
            _center_config = _root_config.get_value_dict("center");
            _config = _root_config.get_value_dict(config_name);

            gate_name_info.name = $"{config_name}_{Guid.NewGuid().ToString("N")}";
            reconn_count = 0;
            tick = 0;

            init();
        }

        public void init()
        {
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

            _timerservice = new Service.Timerservice();
            _timerservice.refresh();

            _hubsvrmanager = new HubSvrManager();
            _clientmanager = new ClientManager(_hubsvrmanager);
            _closehandle = new CloseHandle();

            var redismq_url = _root_config.get_value_string("redis_for_mq");
            if (!_root_config.has_key("redis_for_mq_pwd"))
            {
                _hub_redismq_service = new Abelkhan.RedisMQ(_timerservice, redismq_url, string.Empty, gate_name_info.name, 333);
            }
            else
            {
                _hub_redismq_service = new Abelkhan.RedisMQ(_timerservice, redismq_url, _root_config.get_value_string("redis_for_mq_pwd"), gate_name_info.name, 333);
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

            var _center_ch = _hub_redismq_service.connect(_center_config.get_value_string("name"));
            init_center(_center_ch);

            if (_config.has_key("tcp_inside_listen"))
            {
                var is_tcp_listen = _config.get_value_bool("tcp_inside_listen");
                if (is_tcp_listen)
                {
                    var tcp_inside_host = _config.get_value_string("tcp_inside_host");
                    var tcp_inside_port = (ushort)_config.get_value_int("tcp_inside_port");
                    _hub_service = new Abelkhan.Acceptservice(tcp_inside_port);
                    Acceptservice.on_connect += (ch) => {
                        lock (add_chs)
                        {
                            add_chs.Add(ch);
                        }
                    };
                    _hub_service.start();

                    heartbeat_flush_host(0);
                }
            }

            if (_config.has_key("tcp_listen"))
            {
                var is_tcp_listen = _config.get_value_bool("tcp_listen");
                if (is_tcp_listen)
                {
                    var tcp_outside_host = _config.get_value_string("tcp_outside_host");
                    var tcp_outside_port = (ushort)_config.get_value_int("tcp_outside_port");
                    _client_service = new Abelkhan.CryptAcceptService(tcp_outside_port);
                    CryptAcceptService.on_connect += (ch) => {
                        lock (add_chs)
                        {
                            add_chs.Add(ch);
                        }
                    };
                    _client_service.start();
                }
            }

            if (_config.has_key("websocket_listen"))
            {
                var is_websocket_listen = _config.get_value_bool("websocket_listen");
                if (is_websocket_listen)
                {
                    var websocket_outside_host = _config.get_value_string("websocket_outside_host");
                    var websocket_outside_port = (ushort)_config.get_value_int("websocket_outside_port");
                    var is_ssl = _config.get_value_bool("is_ssl");
                    string pfx = "";
                    if (is_ssl)
                    {
                        pfx = _config.get_value_string("pfx");
                    }
                    _websocket_service = new Abelkhan.WebsocketAcceptService(websocket_outside_port, is_ssl, pfx);
                    _websocket_service.on_connect += (ch) =>
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

                    var enet_outside_host = _config.get_value_string("enet_outside_host");
                    var enet_outside_port = (ushort)_config.get_value_int("enet_outside_port");

                    _enet_service = new Abelkhan.EnetService(enet_outside_host, enet_outside_port);
                    _enet_service.on_connect += (ch) => {
                        lock (add_chs)
                        {
                            add_chs.Add(ch);
                        }
                    };
                    _enet_service.start();
                }
            }

            _timerservice.addticktime(10 * 1000, heartbeat_client);
        }

        private async Task<long> poll()
        {

            long tick_begin = _timerservice.refresh();

            try
            {
                _timerservice.poll();

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

                _ = await _hub_redismq_service.sendmsg_mq();

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

            long tick_end = _timerservice.refresh();
            tick = (uint)(tick_end - tick_begin);

            if (tick > 50)
            {
                Log.Log.trace("poll_tick:{0}", tick);
            }

            return tick;
        }

        private async Task _run()
        {
            if (_config.has_key("prometheus_port"))
            {
                var _prometheus = new Service.PrometheusMetric((short)_config.get_value_int("prometheus_port"));
                _prometheus.Start();
            }

            var _center_msg_handle = new center_msg_handle(this, _timerservice);
            var _hub_svr_msg_handle = new hub_svr_msg_handle(_clientmanager, _hubsvrmanager);
            var _client_msg_handle = new client_msg_handle(_clientmanager, _hubsvrmanager);

            while (!_closehandle.is_closed)
            {
                var ticktime = await poll();

                if (ticktime < 33)
                {
                    Thread.Sleep((int)(33 - ticktime));
                }
            }
            Log.Log.info("server closed, gate server:{0}", gate_name_info.name);
            Log.Log.close();
        }

        private readonly object _run_mu = new();
        public void run()
        {
            if (!Monitor.TryEnter(_run_mu))
            {
                throw new Abelkhan.Exception("run mast at single thread!");
            }

            _run().Wait();

            Monitor.Exit(_run_mu);
        }


        void close_svr()
        {
            _centerproxy.closed();

            if (_hub_service != null)
            {
                _hub_service.close();
            }

            if (_client_service != null)
            {
                _client_service.close();
            }

            if (_hub_redismq_service != null)
            {
                _hub_redismq_service.close();
            }

            if (_enet_service != null)
            {
                _enet_service.stop();
                ManagedENet.Shutdown();
            }

            _timerservice.addticktime(3000, (tick) =>
            {
                _closehandle.is_closed = true;
            });
        }

        private void heartbeat_client(long tick)
        {
            _clientmanager.heartbeat_client(tick);
            _timerservice.addticktime(10000, heartbeat_client);
        }

        private void heartbeat_center(Action reconn_func, long _tmp_tick)
        {
            do
            {
                if ((_tmp_tick - _centerproxy.timetmp) > 9000)
                {
                    reconn_func();
                    break;
                }

                _centerproxy.heartbeat(tick);

            } while (false);

            if (!_closehandle.is_closed)
            {
                _timerservice.addticktime(3000, (_) =>
                {
                    heartbeat_center(reconn_func, Service.Timerservice.Tick);
                });
            }
        }

        private void init_center(Abelkhan.Ichannel center_ch)
        {
            _centerproxy = new CenterProxy(center_ch);
            _centerproxy.reg_server(gate_name_info, () => {
                heartbeat_center(() => {
                    if (reconn_count > 5)
                    {
                        Log.Log.err("connect center faild count:{0}!", reconn_count);
                        sig_center_crash.Invoke();
                    }

                    ++reconn_count;

                    var _center_ch = _hub_redismq_service.connect(_center_config.get_value_string("name"));
                    _centerproxy = new CenterProxy(center_ch);
                    _centerproxy.reconn_reg_server(gate_name_info, () => {
                        reconn_count = 0;
                    });

                }, Service.Timerservice.Tick);
            });
        }

        private void heartbeat_flush_host(long tick)
        {
            var host = $"{_config.get_value_string("tcp_inside_host")}:{_config.get_value_int("tcp_inside_port")}";
            _redis_handle.SetStrData(gate_name_info.name, host, 60);

            _timerservice.addticktime(40000, heartbeat_flush_host);
        }

};

}