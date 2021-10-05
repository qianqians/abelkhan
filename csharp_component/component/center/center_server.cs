﻿/*
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
        private List<channel> chs;
        private List<channel> add_chs;
        private Int64 _timetmp;

        public closehandle _closeHandle;
        public service.timerservice _timer;

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

            chs = new List<channel>();
            add_chs = new List<channel>();
            _closeHandle = new closehandle();
            _timer = new service.timerservice();

            _svrmanager = new svrmanager();
            _svr_msg_handle = new svr_msg_handle(_svrmanager, _closeHandle);
            var ip = _config.get_value_string("ip");
            var port = _config.get_value_int("port");
            _accept_svr_service = new acceptservice((ushort)port);
            _accept_svr_service.on_connect += (channel ch) => {
                lock (add_chs)
                {
                    add_chs.Add(ch);
                }
            };
            _accept_svr_service.start();

            _gmmanager = new gmmanager();
            _gm_msg_handle = new gm_msg_handle(_svrmanager, _gmmanager, _closeHandle);
            var gm_ip = _config.get_value_string("gm_ip");
            var gm_port = _config.get_value_int("gm_port");
            _accept_gm_service = new acceptservice((ushort)gm_port);
            _accept_gm_service.on_connect += (channel ch) =>{
                lock (add_chs)
                {
                    add_chs.Add(ch);
                }
            };
            _accept_gm_service.start();

            _timetmp = _timer.refresh();
            
        }

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
                        lock (ch._channel_onrecv.que)
                        {
                            ch._channel_onrecv.que.Dequeue();
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
                        chs.Remove((channel)_proxy.ch);
                    }
                }

                _svrmanager.remove_closed_svr();
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