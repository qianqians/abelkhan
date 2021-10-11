using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace dbproxy
{
	public class dbproxy
	{
		public dbproxy(String[] args)
		{
			is_busy = false;

            abelkhan.config _config = new abelkhan.config(args[0]);
            abelkhan.config _center_config = _config.get_value_dict("center");
			if (args.Length > 1)
			{
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

            chs = new List<abelkhan.Ichannel>();
            add_chs = new List<abelkhan.Ichannel>();
            remove_chs = new List<abelkhan.Ichannel>();
            
            _closeHandle = new closehandle();
            _hubmanager = new hubmanager();
            _dbevent = new dbevent();
            _dbevent.start();

            var ip = _config.get_value_string("ip");
            var port = (ushort)_config.get_value_int("port");
			_acceptservice = new abelkhan.acceptservice(port);
            _acceptservice.on_connect += (abelkhan.Ichannel ch) => {
                lock (add_chs)
                {
                    add_chs.Add(ch);
                }
            };
            _acceptservice.start();
            _hub_msg_handle = new hub_msg_handle(_hubmanager, _closeHandle);

            var center_ip = _center_config.get_value_string("ip");
			var center_port = (short)_center_config.get_value_int("port");
            var _socket = abelkhan.connectservice.connect(System.Net.IPAddress.Parse(center_ip), center_port);
            var _center_ch = new abelkhan.rawchannel(_socket);
            lock (add_chs)
            {
                add_chs.Add(_center_ch);
            }
            _centerproxy = new centerproxy(_center_ch);
            _center_msg_handle = new center_msg_handle(_closeHandle, _hubmanager);
            _centerproxy.reg_dbproxy(ip, (short)port, name);

            heartbeath_center(service.timerservice.Tick);
        }

        private void heartbeath_center(Int64 tick)
        {
            _centerproxy.heartbeath();
            _timer.addticktime(3 * 1000, heartbeath_center);
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
            catch (abelkhan.Exception e)
            {
                log.log.err(e.Message);
            }
            catch (System.Exception e)
            {
                log.log.err("{0}", e);
            }

            Int64 tick_end = _timer.refresh();

            return tick_end - tick_begin;
        }
        
		private static void Main(String[] args)
		{
			if (args.Length <= 0)
			{
                log.log.err("non input start argv");
				return;
			}

			dbproxy _dbproxy = new dbproxy(args);
            
			while (true)
			{
                var ticktime = _dbproxy.poll();

				if (_closeHandle.is_close())
                {
                    log.log.info("server closed, dbproxy server:{0}", dbproxy.name);
					break;
				}
                
				if (ticktime > 200)
				{
					is_busy = true;
				}
				else 
				{
					is_busy = false;
				}

				if (ticktime < 50)
				{
					Thread.Sleep(5);
				}
			}

            dbproxy._dbevent.join_all();
        }

		public static String name;
		public static bool is_busy;
		public static closehandle _closeHandle;
        public static dbevent _dbevent;
        public static hubmanager _hubmanager;
        public static service.timerservice _timer;
        public static mongodbproxy _mongodbproxy;

        private List<abelkhan.Ichannel> chs;
        private List<abelkhan.Ichannel> add_chs;
        public static List<abelkhan.Ichannel> remove_chs;

        private hub_msg_handle _hub_msg_handle;
        private center_msg_handle _center_msg_handle;

        private abelkhan.acceptservice _acceptservice;
		private centerproxy _centerproxy;

	}
}

