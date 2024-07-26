/*
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

namespace Abelkhan
{
    public class SvrProxy
    {
        public readonly string type;
        public readonly string hub_type;
        public readonly string name;
        public readonly Abelkhan.Ichannel ch;

        public long timetmp = Service.Timerservice.Tick;
        public bool is_mq = false;
        public bool is_closed = false;
        public uint tick = 0;

        private readonly Abelkhan.center_call_server_caller _center_call_server_caller;

        public SvrProxy(Abelkhan.Ichannel _ch, string _type, string _hub_type, string _name)
        {
            type = _type;
            hub_type = _hub_type;
            name = _name;
            ch = _ch;
            is_mq = true;

            _center_call_server_caller = new Abelkhan.center_call_server_caller(_ch, Abelkhan.ModuleMgrHandle._modulemng);
        }

        public event Action<SvrProxy> on_svr_close;
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

    public class HubProxy
    {
        public readonly string type;
        public readonly string name;
        public readonly Abelkhan.Ichannel ch;
        public bool is_closed;

        private readonly Abelkhan.center_call_hub_caller _center_call_hub_caller;

        public HubProxy(Abelkhan.Ichannel _ch, string _type, string _name)
        {
            type = _type;
            name = _name;
            ch = _ch;
            _center_call_hub_caller = new Abelkhan.center_call_hub_caller(ch, Abelkhan.ModuleMgrHandle._modulemng);
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

    public class SvrManager
    {
        private readonly List<SvrProxy> dbproxys;
        private readonly List<SvrProxy> new_svrproxys;
        private readonly List<HubProxy> new_hubproxys;
        private readonly Dictionary<Abelkhan.Ichannel, HubProxy> hubproxys;
        private readonly Dictionary<string, List<HubProxy>> type_hubproxys;
        private readonly RedisHandle _redis_handle;

        private readonly Service.Timerservice _timer;
        private readonly Center _center;
        private readonly RedisMQ _redis_mq_service;

        public readonly Dictionary<Abelkhan.Ichannel, SvrProxy> svrproxys;

        public SvrManager(Service.Timerservice timer, Center _center_proxy, RedisMQ redis_mq_service)
        {
            _timer = timer;
            _center = _center_proxy;
            _redis_mq_service = redis_mq_service;

            if (!_center._root_cfg.has_key("redis_for_mq_pwd"))
            {
                _redis_handle = new RedisHandle(_center._root_cfg.get_value_string("redis_for_cache"), string.Empty);
            }
            else
            {
                _redis_handle = new RedisHandle(_center._root_cfg.get_value_string("redis_for_cache"), _center._root_cfg.get_value_string("redis_for_mq_pwd"));
            }

            dbproxys = new List<SvrProxy>();
            new_svrproxys = new List<SvrProxy>();
            new_hubproxys = new List<HubProxy>();
            svrproxys = new Dictionary<Abelkhan.Ichannel, SvrProxy>();
            hubproxys = new Dictionary<Ichannel, HubProxy>();
            type_hubproxys = new Dictionary<string, List<HubProxy> >();
            closed_svr_list = new List<SvrProxy>();

            load_svr_info();
            heartbeat_svr(Service.Timerservice.Tick);
        }

        private class svr_info
        {
            public string type;
            public string hub_type;
            public string name;
            public long timetmp = Service.Timerservice.Tick;
        }

        private async void load_svr_info()
        {
            try
            {
                var svr_info_list = await _redis_handle.GetData<List<svr_info>>("svr_info_list");
                if (svr_info_list != null)
                {
                    foreach (var _svr_info in svr_info_list)
                    {
                        var _ch = _redis_mq_service.connect(_svr_info.name);

                        var _svrproxy = new SvrProxy(_ch, _svr_info.type, _svr_info.hub_type, _svr_info.name);
                        _svrproxy.timetmp = _svr_info.timetmp;
                        svrproxys[_ch] = _svrproxy;
                        if (_svr_info.type == "dbproxy")
                        {
                            dbproxys.Add(_svrproxy);
                        }
                        _svrproxy.on_svr_close += on_svr_close;

                        if (_svr_info.type == "hub")
                        {
                            var _hubproxy = new HubProxy(_ch, _svr_info.hub_type, _svr_info.name);
                            hubproxys[_ch] = _hubproxy;

                            if (!type_hubproxys.TryGetValue(_svr_info.hub_type, out List<HubProxy> hubproxy_list))
                            {
                                hubproxy_list = new();
                                type_hubproxys[_svr_info.hub_type] = hubproxy_list;
                            }
                            hubproxy_list.Add(_hubproxy);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
            }
        }

        private async ValueTask<bool> store_svr_info()
        {
            try
            {
                var svr_info_list = new List<svr_info>();
                foreach (var svr in svrproxys)
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
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
                return false;
            }
        }

        public void reg_svr(Abelkhan.Ichannel ch, string type, string hub_type, string name, bool is_reconn = false)
        {
            try
            {
                var _svrproxy = new SvrProxy(ch, type, hub_type, name);
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
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
            }
        }

        public HubProxy reg_hub(Abelkhan.Ichannel ch, string hub_type, string _name, bool is_reconn = false)
        {
            try
            {
                Log.Log.trace("reg_hub name:{0}", _name);

                var _hubproxy = new HubProxy(ch, hub_type, _name);
                hubproxys[ch] = _hubproxy;

                if (!type_hubproxys.TryGetValue(hub_type, out List<HubProxy> hubproxy_list))
                {
                    hubproxy_list = new();
                    type_hubproxys[hub_type] = hubproxy_list;
                }
                hubproxy_list.Add(_hubproxy);

                if (!is_reconn)
                {
                    new_hubproxys.Add(_hubproxy);
                }

                return _hubproxy;
            }
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
                return null;
            }
        }

        public List<SvrProxy> closed_svr_list;
        public void remove_closed_svr()
        {
            try
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
                        hubproxys.Remove(_proxy.ch, out HubProxy _hubproxy);
                        if (type_hubproxys.TryGetValue(_proxy.hub_type, out List<HubProxy> hub_list))
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
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
            }
        }

        public event Action<SvrProxy> on_svr_disconnect;
        public async void heartbeat_svr(long tick)
        {
            try
            {
                foreach (var _proxy in svrproxys)
                {
                    if ((Service.Timerservice.Tick - _proxy.Value.timetmp) > 9000)
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
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
            }
        }

        public void on_svr_close(SvrProxy _proxy)
        {
            try
            {
                closed_svr_list.Add(_proxy);

                for_each_svr((_proxy_tmp) =>
                {
                    _proxy_tmp.server_be_closed(_proxy.type, _proxy.name);
                });
            }
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
            }
        }

        private Random _r = new ();
        private int randmon_uint(int max)
        {
            return (int)(_r.NextDouble() * max);
        }

        public void on_svr_close_callback(SvrProxy _proxy)
        {
            try
            {
                SvrProxy _replace = null;

                if (_proxy.type == "dbproxy")
                {
                    if (dbproxys.Count > 0)
                    {
                        _replace = dbproxys[randmon_uint(dbproxys.Count)];
                    }
                }
                else if (_proxy.type == "hub")
                {
                    if (type_hubproxys.TryGetValue(_proxy.hub_type, out List<HubProxy> type_hub_list))
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
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
            }
        }

        public void console_close_server(string type, string name)
        {
            try
            {
                SvrProxy _close_svrproxy = null;
                foreach (var _proxy in svrproxys)
                {
                    if (_proxy.Value.name == name)
                    {
                        _close_svrproxy = _proxy.Value;
                        break;
                    }
                }

                foreach (var _proxy in svrproxys)
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
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
            }
        }

        public bool check_all_hub_closed()
        {
            try
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
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
                return false;
            }
        }

        public bool check_all_db_closed()
        {
            try
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
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
                return false;
            }
        }

        private bool is_ntf_db_close = false;
        public void close_db()
        {
            try
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
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
            }
        }

        public void for_each_svr(Action<SvrProxy> fn)
        {
            try
            {
                foreach (var _proxy in svrproxys.Values)
                {
                    fn(_proxy);
                }
            }
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
            }
        }
        
        public void for_each_hub(Action<HubProxy> fn)
        {
            try
            {
                foreach (var _proxy in hubproxys.Values)
                {
                    fn(_proxy);
                }
            }
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
            }
        }

        public void for_each_new_svr(Action<SvrProxy> fn)
        {
            try
            {
                foreach (var _proxy in new_svrproxys)
                {
                    fn(_proxy);
                }
            }
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
            }
        }

        public void for_each_new_hub(Action<HubProxy> fn)
        {
            try
            {
                foreach (var _proxy in new_hubproxys)
                {
                    fn(_proxy);
                }
            }
            catch (Exception e)
            {
                Log.Log.err(e.ToString());
            }
        }

        public bool get_svr(Abelkhan.Ichannel ch, out SvrProxy _proxy)
        {
            return svrproxys.TryGetValue(ch, out _proxy);
        }

        public bool get_hub(Abelkhan.Ichannel ch, out HubProxy _proxy)
        {
            return hubproxys.TryGetValue(ch, out _proxy);
        }
    }
}