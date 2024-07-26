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
using System.Threading.Tasks;

namespace Abelkhan
{
    public class Center
    {
        public static Service.PrometheusMetric _prometheus;

        private readonly Abelkhan.RedisMQ _redis_mq_service;
        private readonly Acceptservice _accept_gm_service;
        private readonly GMManager _gmmanager;
        private readonly List<Abelkhan.Ichannel> add_chs;
        private readonly svr_msg_handle _svr_msg_handle;
        private readonly gm_msg_handle _gm_msg_handle;

        public readonly SvrManager _svrmanager;
        public readonly List<Abelkhan.Ichannel> remove_chs;
        public readonly CloseHandle _closeHandle;
        public readonly Service.Timerservice _timer; 
        public readonly Abelkhan.Config _root_cfg;
        public readonly Abelkhan.Config _config;

        public event Action<SvrProxy> on_svr_disconnect;

        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            Log.Log.err("unhandle exception:{0}", ex.ToString());
        }

        public Center(string cfg_file, string cfg_name)
        {
            _root_cfg = new Config(cfg_file);
            _config = _root_cfg.get_value_dict(cfg_name);
            var name = _config.get_value_string("name");

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

            add_chs = new List<Abelkhan.Ichannel>();
            remove_chs = new List<Ichannel>();
            _closeHandle = new CloseHandle();

            var redismq_url = _root_cfg.get_value_string("redis_for_mq");
            if (!_root_cfg.has_key("redis_for_mq_pwd"))
            {
                _redis_mq_service = new Abelkhan.RedisMQ(_timer, redismq_url, string.Empty, name, 333);
            }
            else
            {
                _redis_mq_service = new Abelkhan.RedisMQ(_timer, redismq_url, _root_cfg.get_value_string("redis_for_mq_pwd"), name, 333);
            }

            _svrmanager = new SvrManager(_timer, this, _redis_mq_service);
            _svr_msg_handle = new svr_msg_handle(_svrmanager, _closeHandle);
            _svrmanager.on_svr_disconnect += (proxy) =>
            {
                on_svr_disconnect?.Invoke(proxy);
            };

            _gmmanager = new GMManager();
            _gm_msg_handle = new gm_msg_handle(_svrmanager, _gmmanager, _closeHandle);
            var gm_host = _config.get_value_string("gm_host");
            var gm_port = _config.get_value_int("gm_port");
            _accept_gm_service = new Acceptservice((ushort)gm_port);
            Acceptservice.on_connect += (Abelkhan.Ichannel ch) =>{
                lock (add_chs)
                {
                    add_chs.Add(ch);
                }
            };
            _accept_gm_service.start();
        }

        private async ValueTask<long> poll()
        {
            var tick_begin = _timer.refresh();
            try
            {
                _timer.poll();

                while (Abelkhan.EventQueue.msgQue.TryDequeue(out Tuple<Abelkhan.Ichannel, ArrayList> _event))
                {
                    Abelkhan.ModuleMgrHandle._modulemng.process_event(_event.Item1, _event.Item2);
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

                _ = await _redis_mq_service.sendmsg_mq();

                if (_closeHandle.is_closing && _svrmanager.check_all_hub_closed())
                {
                    _svrmanager.close_db();

                    if (_svrmanager.check_all_db_closed())
                    {
                        _closeHandle.is_close = true;

                        _accept_gm_service.close();
                        _redis_mq_service.close();
                    }
                }
            }
            catch (Abelkhan.Exception e)
            {
                Log.Log.err("AbelkhanException:{0}", e.Message);
            }
            catch (System.Exception e)
            {
                Log.Log.err("System.Exception:{0}", e);
            }

            long tick_end = _timer.refresh();
            return tick_end - tick_begin;
        }

        private async Task _run()
        {
            if (_config.has_key("prometheus_port"))
            {
                _prometheus = new Service.PrometheusMetric((short)_config.get_value_int("prometheus_port"));
                _prometheus.Start();
            }

            while (!_closeHandle.is_close)
            {
                try
                {
                    var tick = await poll();
                    if (tick < 333)
                    {
                        Thread.Sleep((int)(333 - tick));
                    }
                }
                catch (System.Exception e)
                {
                    Log.Log.err("error:{0}", e.Message);
                }
            }
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
    }
}