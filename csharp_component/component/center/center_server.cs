/*
 * center server
 * 2020/6/2
 * qianqians
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace abelkhan
{
    public class center
    {
        private acceptservice _accept_svr_service;
        private svr_msg_handle _svr_msg_handle;
        public svrmanager _svrmanager;
        private acceptservice _accept_gm_service;
        private gm_msg_handle _gm_msg_handle;
        private gmmanager _gmmanager;
        private List<abelkhan.Ichannel> chs;
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

            chs = new List<abelkhan.Ichannel>();
            add_chs = new List<abelkhan.Ichannel>();
            remove_chs = new List<Ichannel>();
            _closeHandle = new closehandle();

            _svrmanager = new svrmanager(_timer, this);
            _svr_msg_handle = new svr_msg_handle(_svrmanager, _closeHandle);
            var host = _config.get_value_string("host");
            var port = _config.get_value_int("port");
            _accept_svr_service = new acceptservice((ushort)port);
            _accept_svr_service.on_connect += (abelkhan.Ichannel ch) => {
                lock (add_chs)
                {
                    add_chs.Add(ch);
                }
            };
            _accept_svr_service.start();

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
        public Int64 poll()
        {
            var tick_begin = _timer.refresh();
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

                lock (_svrmanager.closed_svr_list)
                {
                    foreach (var _proxy in _svrmanager.closed_svr_list)
                    {
                        chs.Remove(_proxy.ch);
                        on_svr_disconnect?.Invoke(_proxy);
                    }
                }

                _svrmanager.remove_closed_svr();

                lock (remove_chs)
                {
                    foreach (var _ch in remove_chs)
                    {
                        chs.Remove(_ch);
                    }
                    remove_chs.Clear();
                }

                if (_closeHandle.is_closing && _svrmanager.check_all_hub_closed())
                {
                    _svrmanager.close_db();
                    _closeHandle.is_close = true;

                    _accept_svr_service.close();
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
    }
}