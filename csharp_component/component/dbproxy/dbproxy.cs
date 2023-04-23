using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace dbproxy
{
	public class DBproxy
	{
        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            log.Log.err("unhandle exception:{0}", ex.ToString());
        }

        public DBproxy(string cfg_file, string cfg_name)
		{
			is_busy = false;

            _root_config = new abelkhan.Config(cfg_file);
            _center_config = _root_config.get_value_dict("center");
            var _config = _root_config.get_value_dict(cfg_name);

            name = $"{cfg_name}_{Guid.NewGuid().ToString("N")}";

            var log_level = _config.get_value_string("log_level");
            if (log_level == "trace")
            {
                log.Log.logMode = log.Log.enLogMode.trace;
            }
            else if (log_level == "debug")
            {
                log.Log.logMode = log.Log.enLogMode.debug;
            }
            else if (log_level == "info")
            {
                log.Log.logMode = log.Log.enLogMode.info;
            }
            else if (log_level == "warn")
            {
                log.Log.logMode = log.Log.enLogMode.warn;
            }
            else if (log_level == "err")
            {
                log.Log.logMode = log.Log.enLogMode.err;
            }
            var log_file = _config.get_value_string("log_file");
            log.Log.logFile = log_file;
            var log_dir = _config.get_value_string("log_dir");
            log.Log.logPath = log_dir;
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

                _mongodbproxy = new service.Mongodbproxy(db_ip, db_port);
            }
            else if (_config.has_key("db_url"))
            {
                _mongodbproxy = new service.Mongodbproxy(_config.get_value_string("db_url"));
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
            
            _timer = new service.Timerservice();
            _timer.refresh();

            add_chs = new List<abelkhan.Ichannel>();
            remove_chs = new List<abelkhan.Ichannel>();
            
            _closeHandle = new Closehandle();
            _hubmanager = new Hubmanager();

            var redismq_url = _root_config.get_value_string("redis_for_mq");
            _redis_mq_service = new abelkhan.redis_mq(_timer, redismq_url, name);

            var _center_ch = _redis_mq_service.connect(_center_config.get_value_string("name"));
            lock (add_chs)
            {
                add_chs.Add(_center_ch);
            }
            _centerproxy = new Centerproxy(_center_ch);
            _centerproxy.reg_dbproxy(() => {
                heartbeath_center(service.Timerservice.Tick);
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
            _centerproxy = new Centerproxy(_center_ch);
            if (await _centerproxy.reconn_reg_dbproxy())
            {
                reconn_count = 0;
            }
        }

        private void heartbeath_center(long tick)
        {
            do
            {
                if ((service.Timerservice.Tick - _centerproxy.timetmp) > 9000)
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
                    if (!abelkhan.event_queue.msgQue.TryDequeue(out Tuple<abelkhan.Ichannel, ArrayList> _event))
                    {
                        break;
                    }
                    abelkhan.modulemng_handle._modulemng.process_event(_event.Item1, _event.Item2);
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
                abelkhan.TinyTimer.poll();
            }
            catch (abelkhan.Exception e)
            {
                log.Log.err(e.Message);
            }
            catch (System.Exception e)
            {
                log.Log.err("{0}", e);
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
                throw new abelkhan.Exception("run mast at single thread!");
            }

            while (!_closeHandle.is_close())
            {
                var tick = (uint)poll();

                if (tick < 33)
                {
                    Thread.Sleep((int)(33 - tick));
                }
            }
            log.Log.info("server closed, dbproxy server:{0}", DBproxy.name);

            log.Log.close();

            Monitor.Exit(_run_mu);
        }

		public static String name;
		public static bool is_busy;
        public static uint tick;
		public static Closehandle _closeHandle;
        public static Hubmanager _hubmanager;
        public static service.Timerservice _timer;
        public static service.Mongodbproxy _mongodbproxy;
        public static abelkhan.redis_mq _redis_mq_service;

        public readonly abelkhan.Config _root_config;
        public readonly abelkhan.Config _center_config;

        private readonly List<abelkhan.Ichannel> add_chs;
        private readonly List<abelkhan.Ichannel> remove_chs;

        private readonly hub_msg_handle _hub_msg_handle;
        private readonly center_msg_handle _center_msg_handle;

        private uint reconn_count = 0;

		private Centerproxy _centerproxy;

	}
}

