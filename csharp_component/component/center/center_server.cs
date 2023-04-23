/*
 * center server
 * 2020/6/2
 * qianqians
 */
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace abelkhan
{
    public class Center
    {
        private readonly abelkhan.redis_mq _redis_mq_service;
        private readonly Acceptservice _accept_gm_service;
        private readonly GMmanager _gmmanager;
        private readonly List<abelkhan.Ichannel> add_chs;
        private readonly svr_msg_handle _svr_msg_handle;
        private readonly gm_msg_handle _gm_msg_handle;

        public readonly Svrmanager _svrmanager;
        public readonly List<abelkhan.Ichannel> remove_chs;
        public readonly Closehandle _closeHandle;
        public readonly service.Timerservice _timer; 
        public readonly abelkhan.Config _root_cfg;
        
        public event Action<Svrproxy> on_svr_disconnect;

        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            log.Log.err("unhandle exception:{0}", ex.ToString());
        }

        public Center(string cfg_file, string cfg_name)
        {
            _root_cfg = new Config(cfg_file);
            var _config = _root_cfg.get_value_dict(cfg_name);
            var name = _config.get_value_string("name");

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

            _timer = new service.Timerservice();

            add_chs = new List<abelkhan.Ichannel>();
            remove_chs = new List<Ichannel>();
            _closeHandle = new Closehandle();

            var redismq_url = _root_cfg.get_value_string("redis_for_mq");
            _redis_mq_service = new abelkhan.redis_mq(_timer, redismq_url, name, 333);

            _svrmanager = new Svrmanager(_timer, this, _redis_mq_service);
            _svr_msg_handle = new svr_msg_handle(_svrmanager, _closeHandle);
            _svrmanager.on_svr_disconnect += (proxy) =>
            {
                on_svr_disconnect?.Invoke(proxy);
            };

            _gmmanager = new GMmanager();
            _gm_msg_handle = new gm_msg_handle(_svrmanager, _gmmanager, _closeHandle);
            var gm_host = _config.get_value_string("gm_host");
            var gm_port = _config.get_value_int("gm_port");
            _accept_gm_service = new Acceptservice((ushort)gm_port);
            Acceptservice.on_connect += (abelkhan.Ichannel ch) =>{
                lock (add_chs)
                {
                    add_chs.Add(ch);
                }
            };
            _accept_gm_service.start();
        }

        private long poll()
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

                if (remove_chs.Count > 0)
                {
                    lock (remove_chs)
                    {
                        foreach (var _ch in remove_chs)
                        {
                            add_chs.Remove(_ch);
                        }
                        remove_chs.Clear();
                    }
                }

                if (_closeHandle.is_closing && _svrmanager.check_all_hub_closed())
                {
                    _svrmanager.close_db();

                    if (_svrmanager.check_all_db_closed())
                    {
                        _closeHandle.is_close = true;

                        _accept_gm_service.close();
                    }
                }
            }
            catch (abelkhan.Exception e)
            {
                log.Log.err("AbelkhanException:{0}", e.Message);
            }
            catch (System.Exception e)
            {
                log.Log.err("System.Exception:{0}", e);
            }

            long tick_end = _timer.refresh();
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
                    if (tick < 333)
                    {
                        Thread.Sleep((int)(333 - tick));
                    }
                }
                catch (System.Exception e)
                {
                    log.Log.err("error:{0}", e.Message);
                }
            }
            log.Log.close();

            Monitor.Exit(_run_mu);
        }
    }
}