import uuid 
import json
from threading import Timer
from collections.abc import Callable
import hub
import abelkhan
import redis_mq
import centerproxy

class hub_svr(object):
    def __init__(self, config_file:str, _hub_name:str, _hub_type:str) -> None:
        self.name = f"{_hub_name}_{uuid.uuid1().hex}"
        self.type = _hub_type

        self.__is_closed__ = False

        self.add_chs : list[abelkhan.Ichannel] = []
        self.remove_chs : list[abelkhan.Ichannel] = []

        self.modulemng = abelkhan.modulemng()

        self.cfg = self.__open_config__(config_file)
        self.center_cfg = self.cfg.get("center")

        self.redis_mq_service = redis_mq.redis_mq(self.self.get("redis_for_mq"), self.modulemng)

        self.reconn_count = 0
        _center_ch = self.redis_mq_service.connect(self.center_cfg.get("name"))
        self.add_chs.append(_center_ch)
        self.centerproxy = centerproxy.centerproxy(_center_ch, self.modulemng, self)
        self.centerproxy.reg_hub(self.heartbeat)

        self.onCenterCrash : Callable[[]] = None
        self.onCloseServer : Callable[[]] = None
        self.onReload : Callable[[str]] = None

    def __open_config__(self, config_file:str) -> dict:
        f = open(config_file)
        config = f.read()
        return json.loads(config)

    def heartbeat(self):
        t = Timer(3000, self.heartbeat)
        t.start()

        if (abelkhan.timetmp() - self.centerproxy.timetmp) > 9000:
            self.reconnect_center()

        self.centerproxy.heartbeat()

    def reset_reconn_count(self):
        self.reconn_count = 0

    def reconnect_center(self):
        if self.reconn_count > 5:
            if self.onCenterCrash:
                self.onCenterCrash()

        self.reconn_count += 1
        self.remove_chs.append(self.centerproxy.ch)

        _center_ch = self.redis_mq_service.connect(self.center_cfg.get("name"))
        self.add_chs.append(_center_ch)
        self.centerproxy = centerproxy.centerproxy(_center_ch, self.modulemng, self)
        self.centerproxy.reconn_reg_hub(lambda : self.reset_reconn_count())

    def onCloseServer_event(self):
        if self.onCloseServer:
            self.onCloseServer()

    def __set_closed__(self):
        self.__is_closed__ = True

    def closeSvr(self):
        self.centerproxy.closed()

        t = Timer(3000, self.__set_closed__)
        t.start()

    def onReload_event(self, argv:str):
        if self.onReload:
            self.onReload(argv)

    def reg_hub(self, hub_name:str):
        ch = self.redis_mq_service.connect(hub_name)
        _caller = hub.hub_call_hub_caller(ch, self.modulemng)
        _caller.reg_hub(self.name, self.type)
    
    def poll(self):
        _tick_begin = abelkhan.timetmp()

        try:
            self.redis_mq_service.recvmsg_mq()

            for ch in self.remove_chs:
                del self.add_chs[self.add_chs.index(ch)]
                
        except Exception as ex:
            print(ex)

        return abelkhan.timetmp() - _tick_begin

'''{
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
            _dbproxys = new ConcurrentDictionary<string, dbproxyproxy>();

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
                if ((service.timerservice.Tick - _centerproxy.timetmp) > 9000)
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

        private long poll()
        {
            
            long tick_begin = _timer.refresh();

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

            long tick_end = _timer.refresh();
            tick = (uint)(tick_end - tick_begin);

            if (tick > 50)
            {
                log.log.trace("poll_tick:{0}", tick);
            }

            return tick;
        }

        private readonly object _run_mu = new();
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
            log.log.close();

            Monitor.Exit(_run_mu);
        }

        public class addressinfo
        {
            public string host;
            public ushort port;
        }

        public static uint tick;

        public static string name;
        public static string type;
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

        public static closehandle _closeHandle;
        public static hubmanager _hubs;
        public static gatemanager _gates;
        public static service.timerservice _timer;

        private readonly abelkhan.enetservice _enetservice;
        private readonly abelkhan.cryptacceptservice _cryptacceptservice;
        private readonly abelkhan.websocketacceptservice _websocketacceptservice;

        private uint reconn_count = 0;
        private centerproxy _centerproxy;

        private readonly center_msg_handle _center_msg_handle;
        private readonly dbproxy_msg_handle _dbproxy_msg_handle;
        private readonly hub_msg_handle _hub_msg_handle;
        private readonly gate_msg_handle _gate_msg_handle;
        private readonly client_msg_handle _client_msg_handle;

        public event Action<hubproxy> on_hubproxy;
        public event Action<hubproxy> on_hubproxy_reconn;

        public event Action<string> on_client_disconnect;
        public event Action<string> on_client_exception;
        public event Action<string> on_client_msg;

        public event Action<string> on_direct_client_disconnect;
    }
}
'''
