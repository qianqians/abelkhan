using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace client
{
    class gateproxy
    {
        private abelkhan.Ichannel _ch;
        private abelkhan.client_call_gate_caller _client_call_gate_caller;

        public gateproxy(abelkhan.Ichannel ch)
        {
            _ch = ch;
            _client_call_gate_caller = new abelkhan.client_call_gate_caller(ch, abelkhan.modulemng_handle._modulemng);
        }

        public Action<ulong> onGateTime;
        public Action<abelkhan.Ichannel> onGateDisconnect;
        public void heartbeats()
        {
            _client_call_gate_caller.heartbeats().callBack((ulong _svr_timetmp)=> {
                onGateTime?.Invoke(_svr_timetmp);
            }, ()=> {}).timeout(5 * 1000, ()=> {
                onGateDisconnect?.Invoke(_ch);
            });
        }

        public void get_hub_info(string hub_type, Action<List<abelkhan.hub_info> > cb)
        {
            _client_call_gate_caller.get_hub_info(hub_type).callBack((hub_info) => {
                cb(hub_info);
            }, () => { }).timeout(5 * 1000, ()=> {
                onGateDisconnect?.Invoke(_ch);
            });
        }

        public void call_hub(string hub, string module, string func, ArrayList argv)
        {
            var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<ArrayList>();
            using (MemoryStream st = new MemoryStream())
            {
                var _event = new ArrayList();
                _event.Add(module);
                _event.Add(func);
                _event.Add(argv);
                _serialization.Pack(st, _event);
                st.Position = 0;

                _client_call_gate_caller.forward_client_call_hub(hub, st.ToArray());
            }
        }
    }

    class hubproxy
    {
        public string _hub_name;
        public string _hub_type;

        private abelkhan.Ichannel _ch;
        private abelkhan.client_call_hub_caller _client_call_hub_caller;

        public hubproxy(string hub_name, string hub_type, abelkhan.Ichannel ch)
        {
            _hub_name = hub_name;
            _hub_type = hub_type;

            _ch = ch;
            _client_call_hub_caller = new abelkhan.client_call_hub_caller(ch, abelkhan.modulemng_handle._modulemng);
        }

        public void connect_hub(string cuuid)
        {
            _client_call_hub_caller.connect_hub(cuuid);
        }

        public Action<string, ulong> onHubTime;
        public Action<abelkhan.Ichannel> onHubDisconnect;
        public void heartbeats()
        {
            _client_call_hub_caller.heartbeats().callBack((ulong _hub_timetmp) =>
            {
                onHubTime?.Invoke(_hub_name, _hub_timetmp);
            }, () => { }).timeout(5 * 1000, () =>
            {
                onHubDisconnect?.Invoke(_ch);
            });
        }

        public void call_hub(string module, string func, ArrayList argv)
        {
            var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<ArrayList>();
            using (MemoryStream st = new MemoryStream())
            {
                var _event = new ArrayList();
                _event.Add(module);
                _event.Add(func);
                _event.Add(argv);
                _serialization.Pack(st, _event);
                st.Position = 0;

                _client_call_hub_caller.call_hub(st.ToArray());
            }
        }
    }

	public class client
	{
        public event Action onGateDisConnect;
        public event Action<string> onHubDisConnect;

        public event Action<ulong> onGateTime;
        public event Action<string, ulong> onHubTime;

        public String uuid;
        public service.timerservice timer;
        public common.modulemanager modulemanager;

        public string current_hub;

        private Int64 _heartbeats;

        private gateproxy _gateproxy;
        private Dictionary<string, hubproxy> _hubproxy_set;
        private Dictionary<abelkhan.Ichannel, hubproxy> _ch_hubproxy_set;

        private List<abelkhan.Ichannel> add_chs;
        private List<abelkhan.Ichannel> chs;
        private List<abelkhan.Ichannel> remove_chs;

        private abelkhan.gate_call_client_module _gate_call_client_module;
        private abelkhan.hub_call_client_module _hub_call_client_module;

        public client()
		{
			timer = new service.timerservice();
			modulemanager = new common.modulemanager();

            _heartbeats = timer.refresh();

            _hubproxy_set = new Dictionary<string, hubproxy>();
            _ch_hubproxy_set = new Dictionary<abelkhan.Ichannel, hubproxy>();

            add_chs = new List<abelkhan.Ichannel>();
            chs = new List<abelkhan.Ichannel>();
            remove_chs = new List<abelkhan.Ichannel>();

            timer.addticktime(5 * 1000, heartbeats);

            _gate_call_client_module = new abelkhan.gate_call_client_module(abelkhan.modulemng_handle._modulemng);
            _gate_call_client_module.on_ntf_cuuid += ntf_cuuid;
            _gate_call_client_module.on_call_client += gate_call_client;

            _hub_call_client_module = new abelkhan.hub_call_client_module(abelkhan.modulemng_handle._modulemng);
            _hub_call_client_module.on_call_client += hub_call_client;
        }

        private void ntf_cuuid(string _uuid)
        {
            uuid = _uuid;

            onGateConnect?.Invoke();
        }

        private void gate_call_client(string hub_name, byte[] rpc_argv)
        {
            using (var st = new MemoryStream())
            {
                st.Write(rpc_argv);
                st.Position = 0;

                var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<ArrayList>();
                var _event = _serialization.Unpack(st);

                var module = (string)_event[0];
                var func = (string)_event[1];
                var argvs = (ArrayList)_event[2];

                current_hub = hub_name;
                modulemanager.process_module_mothed(module, func, argvs);
                current_hub = "";
            }
        }

        private void hub_call_client(byte[] rpc_argv)
        {
            using (var st = new MemoryStream())
            {
                st.Write(rpc_argv);
                st.Position = 0;

                var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<ArrayList>();
                var _event = _serialization.Unpack(st);

                var module = (string)_event[0];
                var func = (string)_event[1];
                var argvs = (ArrayList)_event[2];

                var _hubproxy = _ch_hubproxy_set[_hub_call_client_module.current_ch];

                current_hub = _hubproxy._hub_name;
                modulemanager.process_module_mothed(module, func, argvs);
                current_hub = "";
            }
        }

        private void heartbeats(long tick)
        {
            if (_gateproxy != null)
            {
                _gateproxy.heartbeats();
            }

            foreach (var _hubproxy in _hubproxy_set)
            {
                _hubproxy.Value.heartbeats();
            }
        }

        public void get_hub_info(string hub_type, Action<List<abelkhan.hub_info> > cb)
        {
            _gateproxy?.get_hub_info(hub_type, cb);
        }

        public void call_hub(string hub_name, string module, string func, ArrayList argv)
        {
            if (_hubproxy_set.TryGetValue(hub_name, out hubproxy _hubproxy))
            {
                _hubproxy.call_hub(module, func, argv);
                return;
            }

            if (_gateproxy != null)
            {
                _gateproxy.call_hub(hub_name, module, func, argv);
            }
        }

        public event Action onGateConnect;
        public event Action onGateConnectFaild;
        public void connect_gate(string ip, short port, long timeout)
        {
            connect(ip, port, timeout, (is_conn, ch) => {
                if (is_conn && ch != null)
                {
                    _gateproxy = new gateproxy(ch);
                    _gateproxy.onGateDisconnect += (ch) =>
                    {
                        lock (remove_chs)
                        {
                            remove_chs.Add(ch);
                        }
                        _gateproxy = null;

                        onGateDisConnect.Invoke();
                    };
                    _gateproxy.onGateTime += onGateTime;
                }
                else
                {
                    onGateConnectFaild?.Invoke();
                }
            });
        }

        public event Action<string> onHubConnect;
        public event Action<string> onHubConnectFaild;
        public void connect_hub(string hub_name, string hub_type, string ip, short port, long timeout)
        {
            connect(ip, port, timeout, (is_conn, ch) => { 
                if (is_conn && ch != null)
                {
                    var _hubproxy = new hubproxy(hub_name, hub_type, ch);
                    _hubproxy.onHubDisconnect += (ch) =>
                    {
                        lock (remove_chs)
                        {
                            remove_chs.Add(ch);
                        }

                        if (_ch_hubproxy_set.Remove(ch, out hubproxy _proxy))
                        {
                            _hubproxy_set.Remove(_proxy._hub_name);
                        }

                        onHubDisConnect?.Invoke(hub_name);
                    };
                    _hubproxy.onHubTime += onHubTime;
                    _hubproxy.connect_hub(uuid);

                    _hubproxy_set.Add(hub_name, _hubproxy);
                    _ch_hubproxy_set.Add(ch, _hubproxy);

                    onHubConnect?.Invoke(hub_name);
                }
                else
                {
                    onHubConnectFaild?.Invoke(hub_name);
                }
            });
        }

        private class socketConnectTmp
        {
            public Socket s;
            public string timeid;
            public Action<bool, abelkhan.Ichannel> cb;
        }

        private void connect(string ip, short port, long timeout, Action<bool, abelkhan.Ichannel> cb)
        {
            IPAddress address = IPAddress.Parse(ip);

            Socket s;
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                s = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            }

            socketConnectTmp s_cli = new socketConnectTmp();
            s_cli.s = s;
            s_cli.cb = cb;
            s_cli.timeid = timer.addticktime(timeout, (_) => {
                s.Close();
                onGateConnectFaild?.Invoke();
            });


            s.BeginConnect(address, port, new AsyncCallback(end_connect), s_cli);
        }

        private void end_connect(IAsyncResult ar)
        {
            var _s_cli = ar.AsyncState as socketConnectTmp;
            try
            {
                if (_s_cli != null && _s_cli.s != null)
                {
                    _s_cli.s.EndConnect(ar);

                    var ch = new abelkhan.rawchannel(_s_cli.s);
                    lock (add_chs)
                    {
                        add_chs.Add(ch);
                    }

                    _s_cli.cb(true, ch);
                }
            }
            catch (Exception)
            {
                _s_cli.cb(false, null);
            }
            finally
            {
                timer.deltimer(_s_cli.timeid);
            }
        }

        public Int64 poll()
        {
            Int64 tick_begin = timer.poll();

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

            Int64 tick_end = timer.refresh();

            return tick_end - tick_begin;
        }

    }
}

