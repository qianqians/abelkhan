﻿/*
 * svrmanager
 * 2020/6/2
 * qianqians
 */

using System;
using System.Collections.Generic;

namespace abelkhan
{
    public class svrproxy
    {
        public string type;
        public string name;
        public string ip;
        public ushort port;
        public long timetmp = service.timerservice.Tick;
        public bool is_closed = false;

        public abelkhan.Ichannel ch;
        private abelkhan.center_call_server_caller _center_call_server_caller;

        public svrproxy(abelkhan.Ichannel _ch, string _type, string _name, string _ip, ushort _port)
        {
            type = _type;
            name = _name;
            ip = _ip;
            port = _port;
            ch = _ch;

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

        public void server_be_closed(string type, string name)
        {
            _center_call_server_caller.svr_be_closed(type, name);
        }
    }

    public class hubproxy
    {
        public string type;
        public string name;
        public bool is_closed;
        private abelkhan.center_call_hub_caller _center_call_hub_caller;

        public hubproxy(abelkhan.Ichannel ch, string _type, string _name)
        {
            type = _type;
            name = _name;
            _center_call_hub_caller = new abelkhan.center_call_hub_caller(ch, abelkhan.modulemng_handle._modulemng);
        }

        public void distribute_server_address(string type, string name, string ip, ushort port)
        {
            _center_call_hub_caller.distribute_server_address(type, name, ip, port);
        }

        public void reload(string argv)
        {
            _center_call_hub_caller.reload(argv);
        }
    }

    public class svrmanager
    {
        private List<svrproxy> dbproxys;
        public Dictionary<abelkhan.Ichannel, svrproxy> svrproxys;
        private Dictionary<abelkhan.Ichannel, hubproxy> hubproxys;
        private service.timerservice _timer;

        public svrmanager(service.timerservice timer)
        {
            dbproxys = new List<svrproxy>();
            svrproxys = new Dictionary<abelkhan.Ichannel, svrproxy>();
            hubproxys = new Dictionary<Ichannel, hubproxy>();
            _timer = timer;
            closed_svr_list = new List<svrproxy>();

            heartbeat_svr(service.timerservice.Tick);
        }

        public void reg_svr(abelkhan.Ichannel ch, string type, string name, string ip, ushort port)
        {
            var _svrproxy = new svrproxy(ch, type, name, ip, port);
            svrproxys.Add(ch, _svrproxy);
            if (type == "dbproxy")
            {
                dbproxys.Add(_svrproxy);
            }
            _svrproxy.on_svr_close += on_svr_close;
        }

        public hubproxy reg_hub(abelkhan.Ichannel ch, string _type, string _name)
        {
            var _hubproxy = new hubproxy(ch, _type, _name);
            hubproxys.Add(ch, _hubproxy);
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
                    hubproxys.Remove(_proxy.ch);
                }
            }
            closed_svr_list.Clear();
        }
        
        public void heartbeat_svr(Int64 tick)
        {
            foreach (var _proxy in svrproxys)
            {
                if ((service.timerservice.Tick - _proxy.Value.timetmp) > 6 * 1000)
                {
                    on_svr_close(_proxy.Value);
                    closed_svr_list.Add(_proxy.Value);
                }
            }
            _timer.addticktime(6 * 1000, heartbeat_svr);
        }

        public void on_svr_close(svrproxy _proxy)
        {
            closed_svr_list.Add(_proxy);
            
            for_each_svr((_proxy_tmp)=> {
                _proxy_tmp.server_be_closed(_proxy.type, _proxy.name);
            });
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

        public void close_db()
        {
            foreach (var _proxy in dbproxys)
            {
                _proxy.close_server();
            }
        }

        public void for_each_svr(Action<svrproxy> fn){
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
        public svrproxy get_svr(abelkhan.Ichannel ch)
        {
            if (svrproxys.TryGetValue(ch, out svrproxy _proxy))
            {
                return _proxy;
            }

            return null;
        }

        public hubproxy get_hub(abelkhan.Ichannel ch)
        {
            if (hubproxys.TryGetValue(ch, out hubproxy _proxy))
            {
                return _proxy;
            }

            return null;
        }
    }
}