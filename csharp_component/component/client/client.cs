using Abelkhan;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class GateProxy
    {
        private Abelkhan.Ichannel _ch;
        private Abelkhan.client_call_gate_caller _client_call_gate_caller;

        public GateProxy(Abelkhan.Ichannel ch)
        {
            _ch = ch;
            _client_call_gate_caller = new Abelkhan.client_call_gate_caller(ch, Abelkhan.ModuleMgrHandle._modulemng);
        }

        public Action<ulong> onGateTime;
        public Action<Abelkhan.Ichannel> onGateDisconnect;
        public void heartbeats()
        {
            _client_call_gate_caller.heartbeats().callBack((ulong _svr_timetmp)=> {
                onGateTime?.Invoke(_svr_timetmp);
            }, ()=> {}).timeout(5000, ()=> {
                onGateDisconnect?.Invoke(_ch);
            });
        }

        public void get_hub_info(string hub_type, Action<Abelkhan.hub_info> cb)
        {
            _client_call_gate_caller.get_hub_info(hub_type).callBack((hub_info) => {
                cb(hub_info);
            }, () => { 
                
            }).timeout(5000, ()=> {
                onGateDisconnect?.Invoke(_ch);
            });
        }

        public void call_hub(string hub, string func, ArrayList argv)
        {
            var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<List<MsgPack.MessagePackObject>>();
            using (MemoryStream st = MemoryStreamPool.mstMgr.GetStream())
            {
                var _event = new List<MsgPack.MessagePackObject>
                {
                    func,
                    MsgPack.MessagePackObject.FromObject(argv)
                };
                _serialization.Pack(st, _event);
                st.Position = 0;

                _client_call_gate_caller.forward_client_call_hub(hub, st.ToArray());
            }
        }

        public void migrate_client_confirm(string src_hub, string target_hub)
        {
            _client_call_gate_caller.migrate_client_confirm(src_hub, target_hub);
        }
    }

    class HubProxy
    {
        public string _hub_name;
        public string _hub_type;

        private Abelkhan.Ichannel _ch;
        private Abelkhan.client_call_hub_caller _client_call_hub_caller;

        public HubProxy(string hub_name, string hub_type, Abelkhan.Ichannel ch)
        {
            _hub_name = hub_name;
            _hub_type = hub_type;

            _ch = ch;
            _client_call_hub_caller = new Abelkhan.client_call_hub_caller(ch, Abelkhan.ModuleMgrHandle._modulemng);
        }

        public void connect_hub(string cuuid)
        {
            _client_call_hub_caller.connect_hub(cuuid);
        }

        public Action<string, ulong> onHubTime;
        public Action<Abelkhan.Ichannel> onHubDisconnect;
        public void heartbeats()
        {
            _client_call_hub_caller.heartbeats().callBack((ulong _hub_timetmp) =>
            {
                onHubTime?.Invoke(_hub_name, _hub_timetmp);
            }, () => { }).timeout(5000, () =>
            {
                onHubDisconnect?.Invoke(_ch);
            });
        }

        public void call_hub(string func, ArrayList argv)
        {
            var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<List<MsgPack.MessagePackObject>>();
            using (MemoryStream st = MemoryStreamPool.mstMgr.GetStream())
            {
                var _event = new List<MsgPack.MessagePackObject>
                {
                    func,
                    MsgPack.MessagePackObject.FromObject(argv)
                };
                _serialization.Pack(st, _event);
                st.Position = 0;

                _client_call_hub_caller.call_hub(st.ToArray());
            }
        }
    }

	public class Client
	{
        public event Action onGateDisConnect;
        public event Action<string> onHubDisConnect;

        public event Action<ulong> onGateTime;
        public event Action<string, ulong> onHubTime;

        public String uuid;
        public Service.Timerservice timer;
        public Common.ModuleManager modulemanager;

        public string current_hub;

        private Int64 _heartbeats;

        private GateProxy _gateproxy;
        private Dictionary<string, HubProxy> _hubproxy_set;
        private Dictionary<Abelkhan.Ichannel, HubProxy> _ch_hubproxy_set;

        private List<Abelkhan.Ichannel> add_chs;
        private List<Abelkhan.Ichannel> remove_chs;

        private Abelkhan.gate_call_client_module _gate_call_client_module;
        private Abelkhan.hub_call_client_module _hub_call_client_module;

        public Client()
		{
			timer = new Service.Timerservice();
			modulemanager = new Common.ModuleManager();

            _heartbeats = timer.refresh();

            _hubproxy_set = new Dictionary<string, HubProxy>();
            _ch_hubproxy_set = new Dictionary<Abelkhan.Ichannel, HubProxy>();

            add_chs = new List<Abelkhan.Ichannel>();
            remove_chs = new List<Abelkhan.Ichannel>();

            timer.addticktime(5000, heartbeats);

            _gate_call_client_module = new Abelkhan.gate_call_client_module(Abelkhan.ModuleMgrHandle._modulemng);
            _gate_call_client_module.on_ntf_cuuid += ntf_cuuid;
            _gate_call_client_module.on_call_client += gate_call_client;
            _gate_call_client_module.on_migrate_client_start += _gate_call_client_module_on_migrate_client_start;
            _gate_call_client_module.on_migrate_client_done += _gate_call_client_module_on_migrate_client_done;
            _gate_call_client_module.on_hub_loss += _gate_call_client_module_on_hub_loss;

            _hub_call_client_module = new Abelkhan.hub_call_client_module(Abelkhan.ModuleMgrHandle._modulemng);
            _hub_call_client_module.on_call_client += hub_call_client;
        }

        public event Action<string> onHubLoss;
        private void _gate_call_client_module_on_hub_loss(string hub_name)
        {
            onHubLoss?.Invoke(hub_name);
        }

        public event Action<string, string> onMigrateClientDone;
        private void _gate_call_client_module_on_migrate_client_done(string _src_hub, string _target_hub)
        {
            onMigrateClientDone?.Invoke(_src_hub, _target_hub);
        }

        public event Action<string, string> onMigrateClientStart;
        private void _gate_call_client_module_on_migrate_client_start(string _src_hub, string _target_hub)
        {
            onMigrateClientStart?.Invoke(_src_hub, _target_hub);
        }

        private void ntf_cuuid(string _uuid)
        {
            uuid = _uuid;

            onGateConnect?.Invoke();
        }

        private void gate_call_client(string hub_name, byte[] rpc_argv)
        {
            using (var st = MemoryStreamPool.mstMgr.GetStream())
            {
                st.Write(rpc_argv, 0, rpc_argv.Length);
                st.Position = 0;

                var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<ArrayList>();
                var _event = _serialization.Unpack(st);

                var func = ((MsgPack.MessagePackObject)_event[0]).AsString();
                var argvs = ((MsgPack.MessagePackObject)_event[1]).AsList();

                current_hub = hub_name;
                modulemanager.process_module_mothed(func, argvs);
                current_hub = "";
            }
        }

        private void hub_call_client(byte[] rpc_argv)
        {
            using (var st = MemoryStreamPool.mstMgr.GetStream())
            {
                st.Write(rpc_argv, 0, rpc_argv.Length);
                st.Position = 0;

                var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<ArrayList>();
                var _event = _serialization.Unpack(st);

                var func = ((MsgPack.MessagePackObject)_event[0]).AsString();
                var argvs = ((MsgPack.MessagePackObject)_event[1]).AsList();

                var _hubproxy = _ch_hubproxy_set[_hub_call_client_module.current_ch.Value];

                current_hub = _hubproxy._hub_name;
                modulemanager.process_module_mothed(func, argvs);
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

            timer.addticktime(5000, heartbeats);
        }

        public void get_hub_info(string hub_type, Action<Abelkhan.hub_info> cb)
        {
            _gateproxy?.get_hub_info(hub_type, cb);
        }

        public void call_hub(string hub_name, string func, ArrayList argv)
        {
            if (_hubproxy_set.TryGetValue(hub_name, out HubProxy _hubproxy))
            {
                _hubproxy.call_hub(func, argv);
                return;
            }

            if (_gateproxy != null)
            {
                _gateproxy.call_hub(hub_name, func, argv);
            }
        }

        public void migrate_client_confirm(string src_hub, string target_hub)
        {
            _gateproxy.migrate_client_confirm(src_hub, target_hub);
        }

        public event Action onGateConnect;
        public event Action onGateConnectFaild;
        public void connect_gate(string ip, ushort port, long timeout)
        {
            connect(ip, port, timeout, (is_conn, ch) => {
                if (is_conn && ch != null)
                {
                    _gateproxy = new GateProxy(ch);
                    _gateproxy.onGateDisconnect += (ch) =>
                    {
                        lock (remove_chs)
                        {
                            remove_chs.Add(ch);
                        }
                        _gateproxy = null;

                        onGateDisConnect?.Invoke();
                    };
                    _gateproxy.onGateTime += (tick) =>
                    {
                        onGateTime?.Invoke(tick);
                    };
                }
                else
                {
                    onGateConnectFaild?.Invoke();
                }
            });
        }

        public event Action<string> onHubConnect;
        public event Action<string> onHubConnectFaild;
        public void connect_hub(string hub_name, string hub_type, string ip, ushort port, long timeout)
        {
            connect(ip, port, timeout, (is_conn, ch) => { 
                if (is_conn && ch != null)
                {
                    var _hubproxy = new HubProxy(hub_name, hub_type, ch);
                    _hubproxy.onHubDisconnect += (ch) =>
                    {
                        lock (remove_chs)
                        {
                            remove_chs.Add(ch);
                        }

                        if (_ch_hubproxy_set.ContainsKey(ch))
                        {
                            var _proxy = _ch_hubproxy_set[ch];
                            _hubproxy_set.Remove(_proxy._hub_name);
                            _ch_hubproxy_set.Remove(ch);
                        }

                        onHubDisConnect?.Invoke(hub_name);
                    };
                    _hubproxy.onHubTime += (hub_name, tick) =>
                    {
                        onHubTime?.Invoke(hub_name, tick);
                    };
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
            public object timeid;
            public Action<bool, Abelkhan.Ichannel> cb;
        }

        private void connect(string ip, ushort port, long timeout, Action<bool, Abelkhan.Ichannel> cb)
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

                    var ch = new Abelkhan.CryptRawChannel(_s_cli.s);
                    lock (add_chs)
                    {
                        add_chs.Add(ch);
                    }

                    _s_cli.cb(true, ch);
                }
            }
            catch (System.Exception)
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

            
            while (true)
            {
                if (!Abelkhan.EventQueue.msgQue.TryDequeue(out Tuple<Abelkhan.Ichannel, ArrayList> _event))
                {
                    break;
                }
                Abelkhan.ModuleMgrHandle._modulemng.process_event(_event.Item1, _event.Item2);
            }

            lock (remove_chs)
            {
                foreach (var ch in remove_chs)
                {
                    add_chs.Remove(ch);
                }
                remove_chs.Clear();
            }

            Abelkhan.TinyTimer.poll();
			
            Int64 tick_end = timer.refresh();

            return tick_end - tick_begin;
        }

    }
}

