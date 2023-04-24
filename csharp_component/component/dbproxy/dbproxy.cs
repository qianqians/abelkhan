using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace DBProxy
{
	public class DBProxy
	{
        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            Log.Log.err("unhandle exception:{0}", ex.ToString());
        }

        public DBProxy(string cfg_file, string cfg_name)
		{
			is_busy = false;

            _root_config = new Abelkhan.Config(cfg_file);
            _center_config = _root_config.get_value_dict("center");
            var _config = _root_config.get_value_dict(cfg_name);

            name = $"{cfg_name}_{Guid.NewGuid().ToString("N")}";

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

            if (_config.has_key("db_ip") && _config.has_key("db_port"))
            {
                var db_ip = _config.get_value_string("db_ip");
                var db_port = (short)_config.get_value_int("db_port");

                _mongodbproxy = new Service.Mongodbproxy(db_ip, db_port);
            }
            else if (_config.has_key("db_url"))
            {
                _mongodbproxy = new Service.Mongodbproxy(_config.get_value_string("db_url"));
            }

            if (_config.has_key("index"))
            {
                var _index_config = _config.get_value_list("index");
                for (int i = 0; i < _index_config.get_list_size(); i++)
                {
                    var index = _index_config.get_list_dict(i);
                    var db = index.get_value_string("db");
                    var collection = index.get_value_string("collection");
                    var key = index.get_value_string("key");
                    var is_unique = index.get_value_bool("is_unique");
                    _mongodbproxy.create_index(db, collection, key, is_unique);
                }
            }
            if (_config.has_key("guid"))
            {
                var _guid_config = _config.get_value_list("guid");
                for (int i = 0; i < _guid_config.get_list_size(); i++)
                {
                    var _guid_cfg = _guid_config.get_list_dict(i);
                    var _db = _guid_cfg.get_value_string("db");
                    var _collection = _guid_cfg.get_value_string("collection");
                    var _guid = _guid_cfg.get_value_int("guid");
                    _mongodbproxy.check_int_guid(_db, _collection, _guid);
                }
            }
            
            _timer = new Service.Timerservice();
            _timer.refresh();

            add_chs = new List<Abelkhan.Ichannel>();
            remove_chs = new List<Abelkhan.Ichannel>();
            
            _closeHandle = new CloseHandle();
            _hubmanager = new HubManager();

            var redismq_url = _root_config.get_value_string("redis_for_mq");
            _redis_mq_service = new Abelkhan.RedisMQ(_timer, redismq_url, name);

            var _center_ch = _redis_mq_service.connect(_center_config.get_value_string("name"));
            lock (add_chs)
            {
                add_chs.Add(_center_ch);
            }
            _centerproxy = new CenterProxy(_center_ch);
            _centerproxy.reg_dbproxy(() => {
                heartbeath_center(Service.Timerservice.Tick);
            });

            _hub_msg_handle = new hub_msg_handle(_hubmanager, _closeHandle);
            _center_msg_handle = new center_msg_handle(_closeHandle, _centerproxy, _hubmanager);
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
            if (await _centerproxy.reconn_reg_dbproxy())
            {
                reconn_count = 0;
            }
        }

        private void heartbeath_center(long tick)
        {
            do
            {
                if ((Service.Timerservice.Tick - _centerproxy.timetmp) > 9000)
                {
                    reconnect_center();
                    break;
                }

                _centerproxy.heartbeath();

            } while (false);

            _timer.addticktime(3000, heartbeath_center);
        }

		private long poll()
        {
            long tick_begin = _timer.refresh();

            try
            {
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

                _timer.poll();
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

            tick = (uint)(_timer.refresh() - tick_begin);
            if (tick > 100)
            {
                is_busy = true;
            }
            else
            {
                is_busy = false;
            }

            return tick;
        }

        private object _run_mu = new object();
        public void run()
        {
            if (!Monitor.TryEnter(_run_mu))
            {
                throw new Abelkhan.Exception("run mast at single thread!");
            }

            while (!_closeHandle.is_close())
            {
                var tick = (uint)poll();

                if (tick < 33)
                {
                    Thread.Sleep((int)(33 - tick));
                }
            }
            Log.Log.info("server closed, dbproxy server:{0}", DBProxy.name);

            Log.Log.close();

            Monitor.Exit(_run_mu);
        }

		public static String name;
		public static bool is_busy;
        public static uint tick;
		public static CloseHandle _closeHandle;
        public static HubManager _hubmanager;
        public static Service.Timerservice _timer;
        public static Service.Mongodbproxy _mongodbproxy;
        public static Abelkhan.RedisMQ _redis_mq_service;

        public readonly Abelkhan.Config _root_config;
        public readonly Abelkhan.Config _center_config;

        private readonly List<Abelkhan.Ichannel> add_chs;
        private readonly List<Abelkhan.Ichannel> remove_chs;

        private readonly hub_msg_handle _hub_msg_handle;
        private readonly center_msg_handle _center_msg_handle;

        private uint reconn_count = 0;

		private CenterProxy _centerproxy;

	}
}

