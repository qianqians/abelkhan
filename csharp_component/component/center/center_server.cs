﻿/*
 * center server
 * 2020/6/2
 * qianqians
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace abelkhan
{
    public class center
    {
        private abelkhan.redis_mq _redis_mq_service;
        private svr_msg_handle _svr_msg_handle;
        public svrmanager _svrmanager;
        private acceptservice _accept_gm_service;
        private gm_msg_handle _gm_msg_handle;
        private gmmanager _gmmanager;
        private List<abelkhan.Ichannel> add_chs;
        public List<abelkhan.Ichannel> remove_chs;
        private Int64 _timetmp;

        public closehandle _closeHandle;
        public service.timerservice _timer;

        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            log.log.err("unhandle exception:{0}", ex.ToString());
        }

        public center(string cfg_file, string cfg_name)
        {
            var _root_cfg = new config(cfg_file);
            var _config = _root_cfg.get_value_dict(cfg_name);
            var name = cfg_name;

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
            _timetmp = _timer.refresh();

            add_chs = new List<abelkhan.Ichannel>();
            remove_chs = new List<Ichannel>();
            _closeHandle = new closehandle();

            _svrmanager = new svrmanager(_timer, this);
            _svr_msg_handle = new svr_msg_handle(_svrmanager, _closeHandle);

            var redismq_url = _root_cfg.get_value_string("redis_for_mq");
            _redis_mq_service = new abelkhan.redis_mq(redismq_url, name);

            _gmmanager = new gmmanager();
            _gm_msg_handle = new gm_msg_handle(_svrmanager, _gmmanager, _closeHandle);
            var gm_host = _config.get_value_string("gm_host");
            var gm_port = _config.get_value_int("gm_port");
            _accept_gm_service = new acceptservice((ushort)gm_port);
            _accept_gm_service.on_connect += (abelkhan.Ichannel ch) =>{
                lock (add_chs)
                {
                    add_chs.Add(ch);
                }
            };
            _accept_gm_service.start();
        }

        public event Action<svrproxy> on_svr_disconnect;
        private Int64 poll()
        {
            var tick_begin = _timer.refresh();
            try
            {
                _timer.poll();

                while (true)
                {
                    if (!event_queue.msgQue.TryDequeue(out Tuple<Ichannel, ArrayList> _event))
                    {
                        break;
                    }
                    abelkhan.modulemng_handle._modulemng.process_event(_event.Item1, _event.Item2);
                }

                lock (_svrmanager.closed_svr_list)
                {
                    foreach (var _proxy in _svrmanager.closed_svr_list)
                    {
                        on_svr_disconnect?.Invoke(_proxy);
                    }
                }
                _svrmanager.remove_closed_svr();

                lock (remove_chs)
                {
                    foreach (var _ch in remove_chs)
                    {
                        add_chs.Remove(_ch);
                    }
                    remove_chs.Clear();
                }

                if (_closeHandle.is_closing && _svrmanager.check_all_hub_closed())
                {
                    _svrmanager.close_db();
                    _closeHandle.is_close = true;

                    _accept_gm_service.close();
                }
            }
            catch (abelkhan.Exception e)
            {
                log.log.err("AbelkhanException:{0}", e.Message);
            }
            catch (System.Exception e)
            {
                log.log.err("System.Exception:{0}", e);
            }

            Int64 tick_end = _timer.refresh();
            _timetmp = tick_end;
            return tick_end - tick_begin;
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
                try
                {
                    var tick = poll();
                    if (tick < 33)
                    {
                        Thread.Sleep((int)(33 - tick));
                    }
                }
                catch (System.Exception e)
                {
                    log.log.err("error:{0}", e.Message);
                }
            }

            Monitor.Exit(_run_mu);
        }
    }
}