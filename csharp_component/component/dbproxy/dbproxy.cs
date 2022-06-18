using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace dbproxy
{
	public class dbproxy
	{
        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            log.log.err("unhandle exception:{0}", ex.ToString());
        }

        public dbproxy(string cfg_file, string cfg_name)
		{
			is_busy = false;

            abelkhan.config _config = new abelkhan.config(cfg_file);
            abelkhan.config _center_config = _config.get_value_dict("center");
            _config = _config.get_value_dict(cfg_name);

            name = cfg_name;

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

            if (_config.has_key("db_ip") && _config.has_key("db_port"))
            {
                var db_ip = _config.get_value_string("db_ip");
                var db_port = (short)_config.get_value_int("db_port");

                _mongodbproxy = new mongodbproxy(db_ip, db_port);
            }
            else if (_config.has_key("db_url"))
            {
                _mongodbproxy = new mongodbproxy(_config.get_value_string("db_url"));
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
            
            _timer = new service.timerservice();
            _timer.refresh();

            add_chs = new List<abelkhan.Ichannel>();
            remove_chs = new List<abelkhan.Ichannel>();
            
            _closeHandle = new closehandle();
            _hubmanager = new hubmanager();
            _dbevent = new dbevent();
            _dbevent.start();

            host = _config.get_value_string("host");
            port = (ushort)_config.get_value_int("port");
			_acceptservice = new abelkhan.acceptservice(port);
            _acceptservice.on_connect += (abelkhan.Ichannel ch) => {
                lock (add_chs)
                {
                    add_chs.Add(ch);
                }
            };
            _acceptservice.start();
            _hub_msg_handle = new hub_msg_handle(_hubmanager, _closeHandle);

            center_host = _center_config.get_value_string("host");
			center_port = (short)_center_config.get_value_int("port");
            var _socket = abelkhan.connectservice.connect(System.Net.Dns.GetHostAddresses(center_host)[0], center_port);
            var _center_ch = new abelkhan.rawchannel(_socket);
            lock (add_chs)
            {
                add_chs.Add(_center_ch);
            }
            _centerproxy = new centerproxy(_center_ch);
            _center_msg_handle = new center_msg_handle(_closeHandle, _hubmanager);
            _centerproxy.reg_dbproxy(host, port);

            heartbeath_center(service.timerservice.Tick);
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

            var _socket = abelkhan.connectservice.connect(System.Net.Dns.GetHostAddresses(center_host)[0], center_port);
            var _center_ch = new abelkhan.rawchannel(_socket);
            lock (add_chs)
            {
                add_chs.Add(_center_ch);
            }
            _centerproxy = new centerproxy(_center_ch);
            if (await _centerproxy.reconn_reg_dbproxy(host, port))
            {
                reconn_count = 0;
            }
        }

        private void heartbeath_center(Int64 tick)
        {
            do
            {
                if ((service.timerservice.Tick - _centerproxy.timetmp) > 6 * 1000)
                {
                    reconnect_center();
                    break;
                }

                _centerproxy.heartbeath();

            } while (false);

            _timer.addticktime(3 * 1000, heartbeath_center);
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

        public void run()
        {
            while (!_closeHandle.is_close())
            {
                var tick = (uint)poll();

                if (tick < 33)
                {
                    Thread.Sleep((int)(33 - tick));
                }
            }
            log.log.info("server closed, dbproxy server:{0}", dbproxy.name);

            _acceptservice.close();
            dbproxy._dbevent.join_all();
        }

		public static String name;
		public static bool is_busy;
        public static uint tick;
		public static closehandle _closeHandle;
        public static dbevent _dbevent;
        public static hubmanager _hubmanager;
        public static service.timerservice _timer;
        public static mongodbproxy _mongodbproxy;

        private List<abelkhan.Ichannel> add_chs;
        public static List<abelkhan.Ichannel> remove_chs;

        private string host;
        private ushort port;

        private string center_host;
        private short center_port;

        private uint reconn_count = 0;

        private hub_msg_handle _hub_msg_handle;
        private center_msg_handle _center_msg_handle;

        public abelkhan.acceptservice _acceptservice;
		private centerproxy _centerproxy;

	}
}

