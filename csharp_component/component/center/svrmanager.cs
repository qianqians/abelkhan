﻿/*
 * svrmanager
 * 2020/6/2
 * qianqians
 */

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace abelkhan
{
    public class svrproxy
    {
        public readonly string type;
        public readonly string hub_type;
        public readonly string name;
        public readonly abelkhan.Ichannel ch;

        public long timetmp = service.timerservice.Tick;
        public bool is_mq = false;
        public bool is_closed = false;
        public uint tick = 0;

        private readonly abelkhan.center_call_server_caller _center_call_server_caller;

        public svrproxy(abelkhan.Ichannel _ch, string _type, string _hub_type, string _name)
        {
            type = _type;
            hub_type = _hub_type;
            name = _name;
            ch = _ch;
            is_mq = true;

            _center_call_server_caller = new abelkhan.center_call_server_caller(_ch, abelkhan.modulemng_handle._modulemng);
        }

        public event Action<svrproxy> on_svr_close;
        public void closed_svr()
        {
            on_svr_close?.Invoke(this);
        }

        public void close_server()
        {
            _center_call_server_caller.close_server();
        }

        public void console_close_server(string type, string name)
        {
            _center_call_server_caller.console_close_server(type, name);
        }

        public void take_over_svr(string svr_name)
        {
            _center_call_server_caller.take_over_svr(svr_name);
        }

        public void server_be_closed(string type, string name)
        {
            _center_call_server_caller.svr_be_closed(type, name);
        }
    }

    public class hubproxy
    {
        public readonly string type;
        public readonly string name;
        public readonly abelkhan.Ichannel ch;
        public bool is_closed;

        private readonly abelkhan.center_call_hub_caller _center_call_hub_caller;

        public hubproxy(abelkhan.Ichannel _ch, string _type, string _name)
        {
            type = _type;
            name = _name;
            ch = _ch;
            _center_call_hub_caller = new abelkhan.center_call_hub_caller(ch, abelkhan.modulemng_handle._modulemng);
        }

        public void distribute_server_mq(string type, string name)
        {
            _center_call_hub_caller.distribute_server_mq(type, name);
        }

        public void reload(string argv)
        {
            _center_call_hub_caller.reload(argv);
        }
    }

    public class svrmanager
    {
        private readonly List<svrproxy> dbproxys;
        private readonly List<svrproxy> new_svrproxys;
        private readonly List<hubproxy> new_hubproxys;
        private readonly Dictionary<abelkhan.Ichannel, hubproxy> hubproxys;
        private readonly Dictionary<string, List<hubproxy>> type_hubproxys;
        private readonly redis_handle _redis_handle;

        private readonly service.timerservice _timer;
        private readonly center _center;
        private readonly redis_mq _redis_mq_service;

        public readonly Dictionary<abelkhan.Ichannel, svrproxy> svrproxys;

        public svrmanager(service.timerservice timer, center _center_proxy, redis_mq redis_mq_service)
        {
            _timer = timer;
            _center = _center_proxy;
            _redis_mq_service = redis_mq_service;

            _redis_handle = new redis_handle(_center._root_cfg.get_value_string("redis_for_cache"));

            dbproxys = new List<svrproxy>();
            new_svrproxys = new List<svrproxy>();
            new_hubproxys = new List<hubproxy>();
            svrproxys = new Dictionary<abelkhan.Ichannel, svrproxy>();
            hubproxys = new Dictionary<Ichannel, hubproxy>();
            type_hubproxys = new Dictionary<string, List<hubproxy> >();
            closed_svr_list = new List<svrproxy>();

            load_svr_info();
            heartbeat_svr(service.timerservice.Tick);
        }

        private class svr_info
        {
            public string type;
            public string hub_type;
            public string name;
            public long timetmp = service.timerservice.Tick;
        }

        private async void load_svr_info()
        {
            var svr_info_list = await _redis_handle.GetData<List<svr_info> >("svr_info_list");
            if (svr_info_list != null)
            {
                foreach (var _svr_info in svr_info_list)
                {
                    var _ch = _redis_mq_service.connect(_svr_info.name);

                    var _svrproxy = new svrproxy(_ch, _svr_info.type, _svr_info.hub_type, _svr_info.name);
                    _svrproxy.timetmp = _svr_info.timetmp;
                    svrproxys[_ch] = _svrproxy;
                    if (_svr_info.type == "dbproxy")
                    {
                        dbproxys.Add(_svrproxy);
                    }
                    _svrproxy.on_svr_close += on_svr_close;

                    var _hubproxy = new hubproxy(_ch, _svr_info.hub_type, _svr_info.name);
                    hubproxys[_ch] = _hubproxy;

                    if (!type_hubproxys.TryGetValue(_svr_info.hub_type, out List<hubproxy> hubproxy_list))
                    {
                        hubproxy_list = new();
                        type_hubproxys[_svr_info.hub_type] = hubproxy_list;
                    }
                    hubproxy_list.Add(_hubproxy);
                }
            }
        }

        private async Task<bool> store_svr_info()
        {
            var svr_info_list = new List<svr_info>();
            foreach(var svr in svrproxys)
            {
                svr_info_list.Add(new svr_info()
                {
                    type = svr.Value.type,
                    hub_type = svr.Value.hub_type,
                    name = svr.Value.name,
                    timetmp = svr.Value.timetmp
                });
            }
            return await _redis_handle.SetData("svr_info_list", svr_info_list);
        }

        public void reg_svr(abelkhan.Ichannel ch, string type, string hub_type, string name, bool is_reconn = false)
        {
            var _svrproxy = new svrproxy(ch, type, hub_type, name);
            svrproxys[ch] = _svrproxy;
            if (type == "dbproxy")
            {
                dbproxys.Add(_svrproxy);
            }
            _svrproxy.on_svr_close += on_svr_close;

            if (!is_reconn)
            {
                new_svrproxys.Add(_svrproxy);
            }
        }

        public hubproxy reg_hub(abelkhan.Ichannel ch, string hub_type, string _name, bool is_reconn = false)
        {
            log.log.trace("reg_hub name:{0}", _name);

            var _hubproxy = new hubproxy(ch, hub_type, _name);
            hubproxys[ch] = _hubproxy;

            if (!type_hubproxys.TryGetValue(hub_type, out List<hubproxy> hubproxy_list))
            {
                hubproxy_list = new ();
                type_hubproxys[hub_type] = hubproxy_list;
            }
            hubproxy_list.Add(_hubproxy);

            if (!is_reconn)
            {
                new_hubproxys.Add(_hubproxy);
            }

            return _hubproxy;
        }

        public List<svrproxy> closed_svr_list;
        public void remove_closed_svr()
        {
            foreach (var _proxy in closed_svr_list)
            {
                if (svrproxys.ContainsKey(_proxy.ch))
                {
                    svrproxys.Remove(_proxy.ch);
                }

                if (dbproxys.Contains(_proxy))
                {
                    dbproxys.Remove(_proxy);
                }

                if (hubproxys.ContainsKey(_proxy.ch))
                {
                    hubproxys.Remove(_proxy.ch, out hubproxy _hubproxy);
                    if (type_hubproxys.TryGetValue(_proxy.hub_type, out List<hubproxy> hub_list))
                    {
                        hub_list.Remove(_hubproxy);
                    }
                }

                if (new_svrproxys.Contains(_proxy))
                {
                    new_svrproxys.Remove(_proxy);
                }

                foreach (var _hubproxy in new_hubproxys)
                {
                    if (_hubproxy.type == _proxy.type && _hubproxy.name == _proxy.name)
                    {
                        new_hubproxys.Remove(_hubproxy);
                        break;
                    }
                }
            }
            
            foreach (var _proxy in closed_svr_list)
            {
                on_svr_close_callback(_proxy);
            }

            closed_svr_list.Clear();
        }

        public event Action<svrproxy> on_svr_disconnect;
        public async void heartbeat_svr(long tick)
        {
            foreach (var _proxy in svrproxys)
            {
                if ((service.timerservice.Tick - _proxy.Value.timetmp) > 9000)
                {
                    on_svr_close(_proxy.Value);
                }
            }

            if (closed_svr_list.Count > 0)
            {
                foreach (var _proxy in closed_svr_list)
                {
                    on_svr_disconnect?.Invoke(_proxy);
                }
                remove_closed_svr();
            }

            await store_svr_info();

            _timer.addticktime(6000, heartbeat_svr);
        }

        public void on_svr_close(svrproxy _proxy)
        {
            closed_svr_list.Add(_proxy);
            
            for_each_svr((_proxy_tmp)=> {
                _proxy_tmp.server_be_closed(_proxy.type, _proxy.name);
            });
        }

        private Random _r = new ();
        private int randmon_uint(int max)
        {
            return (int)(_r.NextDouble() * max);
        }

        public void on_svr_close_callback(svrproxy _proxy)
        {
            svrproxy _replace = null;

            if (_proxy.type == "dbproxy")
            {
                if (dbproxys.Count > 0)
                {
                    _replace = dbproxys[randmon_uint(dbproxys.Count)];
                }
            }
            else if (_proxy.type == "hub")
            {
                if (type_hubproxys.TryGetValue(_proxy.hub_type, out List<hubproxy> type_hub_list))
                {
                    if (type_hub_list.Count > 0)
                    {
                        var _replace_hubproxy = type_hub_list[randmon_uint(type_hub_list.Count)];
                        svrproxys.TryGetValue(_replace_hubproxy.ch, out _replace);
                    }
                }
            }
            _replace?.take_over_svr(_proxy.name);
        }

        public void console_close_server(string type, string name)
        {
            svrproxy _close_svrproxy = null;
            foreach(var _proxy in svrproxys)
            {
                if (_proxy.Value.name == name)
                {
                    _close_svrproxy = _proxy.Value;
                    break;
                }
            }

            foreach(var _proxy in svrproxys)
            {
                _proxy.Value.console_close_server(_close_svrproxy.type, _close_svrproxy.name);
            }

            if (svrproxys.ContainsKey(_close_svrproxy.ch))
            {
                svrproxys.Remove(_close_svrproxy.ch);
            }

            if (dbproxys.Contains(_close_svrproxy))
            {
                dbproxys.Remove(_close_svrproxy);
            }

            if (hubproxys.ContainsKey(_close_svrproxy.ch))
            {
                hubproxys.Remove(_close_svrproxy.ch);
            }

            if (new_svrproxys.Contains(_close_svrproxy))
            {
                new_svrproxys.Remove(_close_svrproxy);
            }

            foreach (var _hubproxy in new_hubproxys)
            {
                if (_hubproxy.type == _close_svrproxy.type && _hubproxy.name == _close_svrproxy.name)
                {
                    new_hubproxys.Remove(_hubproxy);
                    break;
                }
            }

            lock (_center.remove_chs)
            {
                _center.remove_chs.Add(_close_svrproxy.ch);
            }
        }

        public bool check_all_hub_closed()
        {
            bool _all_closed = true;
            foreach (var _proxy in hubproxys.Values)
            {
                if (!_proxy.is_closed)
                {
                    _all_closed = false;
                }
            }
            return _all_closed;
        }

        public bool check_all_db_closed()
        {
            bool _all_closed = true;
            foreach (var _proxy in dbproxys)
            {
                if (!_proxy.is_closed)
                {
                    _all_closed = false;
                }
            }
            return _all_closed;
        }

        private bool is_ntf_db_close = false;
        public void close_db()
        {
            if (is_ntf_db_close)
            {
                return;
            }

            foreach (var _proxy in dbproxys)
            {
                _proxy.close_server();
            }

            is_ntf_db_close = true;
        }

        public void for_each_svr(Action<svrproxy> fn)
        {
            foreach (var _proxy in svrproxys.Values)
            {
                fn(_proxy);
            }
        }
        
        public void for_each_hub(Action<hubproxy> fn)
        {
            foreach (var _proxy in hubproxys.Values)
            {
                fn(_proxy);
            }
        }

        public void for_each_new_svr(Action<svrproxy> fn)
        {
            foreach (var _proxy in new_svrproxys)
            {
                fn(_proxy);
            }
        }

        public void for_each_new_hub(Action<hubproxy> fn)
        {
            foreach (var _proxy in new_hubproxys)
            {
                fn(_proxy);
            }
        }

        public bool get_svr(abelkhan.Ichannel ch, out svrproxy _proxy)
        {
            return svrproxys.TryGetValue(ch, out _proxy);
        }

        public bool get_hub(abelkhan.Ichannel ch, out hubproxy _proxy)
        {
            return hubproxys.TryGetValue(ch, out _proxy);
        }
    }
}