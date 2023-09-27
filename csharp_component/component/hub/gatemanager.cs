using Abelkhan;
using MsgPack.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

namespace Hub
{
    public class DirectProxy
    {
        private readonly Abelkhan.hub_call_client_caller _hub_call_client_caller;

        public readonly string _cuuid;
        public readonly Abelkhan.Ichannel _direct_ch;

        public long _timetmp = 0;
        public long _theory_timetmp = 0;

        public DirectProxy(string cuuid_, Abelkhan.Ichannel direct_ch)
        {
            _cuuid = cuuid_;
            _direct_ch = direct_ch;

            _hub_call_client_caller = new Abelkhan.hub_call_client_caller(_direct_ch, Abelkhan.ModuleMgrHandle._modulemng);
        }

        public void call_client(byte[] rpc_argv)
        {
            _hub_call_client_caller.call_client(rpc_argv);
        }
    }

    public class GateManager
	{
        public string current_client_uuid;

        private readonly Dictionary<string, GateProxy> clients;

        private readonly Dictionary<string, GateProxy> _wait_destory_gateproxys;
        private readonly Dictionary<Abelkhan.Ichannel, GateProxy> ch_gateproxys;
        private readonly Dictionary<string, GateProxy> gates;

        private readonly Dictionary<string, DirectProxy> direct_clients;
        private readonly Dictionary<Abelkhan.Ichannel, DirectProxy> ch_direct_clients;

        private readonly MessagePackSerializer<ArrayList> _serializer = MessagePackSerializer.Get<ArrayList>();

        private readonly Abelkhan.RedisMQ _gate_redismq_conn;

        public GateManager(Abelkhan.RedisMQ _conn)
        {
            _gate_redismq_conn = _conn;

            clients = new Dictionary<string, GateProxy>();

            _wait_destory_gateproxys = new Dictionary<string, GateProxy>();
            ch_gateproxys = new Dictionary<Abelkhan.Ichannel, GateProxy>();
            gates = new Dictionary<string, GateProxy>();

            direct_clients = new Dictionary<string, DirectProxy>();
            ch_direct_clients = new Dictionary<Abelkhan.Ichannel, DirectProxy>();

            Hub._timer.addticktime(10000, heartbeat_tick_hub_health);
            Hub._timer.addticktime(10000, heartbeat_client);
        }

        public void connect_gate(String name)
        {
            var ch = _gate_redismq_conn.connect(name);
            if (ch != null)
            {
                var _proxy = new GateProxy(ch, name);

                if (gates.TryGetValue(name, out GateProxy _old_proxy))
                {
                    _wait_destory_gateproxys.Add(name, _old_proxy);
                    gates[name] = _proxy;
                }
                else
                {
                    gates.Add(name, _proxy);
                }

                ch_gateproxys.Add(ch, _proxy);
                Log.Log.info("connect gate:{0}", name);

                lock (Hub.add_chs)
                {
                    Hub.add_chs.Add(ch);
                }
                _proxy.reg_hub();
            }
        }

        public bool get_gateproxy(Abelkhan.Ichannel gate_ch, out GateProxy proxy)
		{
			return ch_gateproxys.TryGetValue(gate_ch, out proxy);
		}

        public bool get_gateproxy(string session_uuid, out GateProxy proxy)
        {
            return clients.TryGetValue(session_uuid, out proxy);
        }

        public string get_client_gate_name(string session_uuid)
        {
            clients.TryGetValue(session_uuid, out GateProxy _client_gate_proxy);
            if (_client_gate_proxy != null)
            {
                return _client_gate_proxy._name;
            }
            return null;
        }

        public event Action<string> on_gate_closed;
        public void gate_be_closed(string svr_name)
        {
            Log.Log.info("gate_be_closed name:{0}", svr_name);
            if (_wait_destory_gateproxys.Remove(svr_name, out GateProxy _old_proxy))
            {
                var remove = new List<string>();
                foreach (var it in clients)
                {
                    if (it.Value == _old_proxy)
                    {
                        remove.Add(it.Key);
                    }
                }
                foreach (var it in remove)
                {
                    clients.Remove(it);
                }

                ch_gateproxys.Remove(_old_proxy._ch);

                lock (Hub.remove_chs)
                {
                    Hub.remove_chs.Add(_old_proxy._ch);
                }
            }
            else
            {
                if (gates.Remove(svr_name, out GateProxy _proxy))
                {
                    var remove = new List<string>();
                    foreach (var it in clients)
                    {
                        if (it.Value == _proxy)
                        {
                            remove.Add(it.Key);
                        }
                    }
                    foreach (var it in remove)
                    {
                        clients.Remove(it);
                    }

                    ch_gateproxys.Remove(_proxy._ch);

                    lock (Hub.remove_chs)
                    {
                        Hub.remove_chs.Add(_proxy._ch);
                    }

                    on_gate_closed?.Invoke(svr_name);
                }
            }
        }

        public void client_connect(string client_uuid, Abelkhan.Ichannel gate_ch)
        {
            if (ch_gateproxys.TryGetValue(gate_ch, out GateProxy _proxy))
            {
                if (!clients.ContainsKey(client_uuid))
                {
                    clients.Add(client_uuid, _proxy);
                }
            }
            else
            {
                Log.Log.err("invaild gate:{0}, ch_gateproxys.count:{1}", gate_ch, ch_gateproxys.Count);
            }

            Log.Log.trace("client {0} connected", client_uuid);
        }

        public GateProxy client_seep(string client_uuid, string gate_name)
        {
            if (!gates.TryGetValue(gate_name, out GateProxy _proxy))
            {
                Log.Log.err("invaild gate name:{0}", gate_name);
                return null;
            }
            if (!clients.ContainsKey(client_uuid))
            {
                clients.Add(client_uuid, _proxy);
            }

            Log.Log.trace("client {0} seep!", client_uuid);

            return _proxy;
        }

        public event Action<string> clientDisconnect;
        public void client_disconnect(String client_uuid)
        {
            if (clients.Remove(client_uuid))
            {
                clientDisconnect?.Invoke(client_uuid);
            }
        }

        public event Action<string> clientException;
        public void client_exception(String client_uuid)
        {
            clientException?.Invoke(client_uuid);
        }

        public void direct_client_connect(String client_uuid, Abelkhan.Ichannel direct_ch)
        {
            if (direct_clients.Remove(client_uuid, out DirectProxy _directproxy))
            {
                ch_direct_clients.Remove(_directproxy._direct_ch);
            }

            Log.Log.trace("reg direct client:{0}", client_uuid);

            var _directproxy_new = new DirectProxy(client_uuid, direct_ch);
            direct_clients.Add(client_uuid, _directproxy_new);

            ch_direct_clients.Add(direct_ch, _directproxy_new);
        }

        public event Action<string> directClientDisconnect;
        public void direct_client_disconnect(Abelkhan.Ichannel direct_ch)
        {
            if (ch_direct_clients.Remove(direct_ch, out DirectProxy _proxy))
            {
                direct_clients.Remove(_proxy._cuuid);
                directClientDisconnect?.Invoke(_proxy._cuuid);
            }
        }

        public void direct_client_exception(Abelkhan.Ichannel direct_ch)
        {
            if (ch_direct_clients.TryGetValue(direct_ch, out DirectProxy _proxy))
            {
                clientException?.Invoke(_proxy._cuuid);
            }
        }

        void heartbeat_tick_hub_health(long ticktime)
        {
            foreach(var (_, proxy) in ch_gateproxys)
            {
                proxy.tick_hub_health();
            }

            Hub._timer.addticktime(10000, heartbeat_tick_hub_health);
        }

        void heartbeat_client(long ticktime)
        {
            List<DirectProxy> remove_client = new List<DirectProxy>();
            List<DirectProxy> exception_client = new List<DirectProxy>();
            foreach (var item in direct_clients)
            {
                var proxy = item.Value;
                if (proxy._timetmp > 0 && (proxy._timetmp + 10000) < ticktime)
                {
                    remove_client.Add(proxy);
                }
                if (proxy._timetmp > 0 && proxy._theory_timetmp > 0 && (proxy._theory_timetmp - proxy._timetmp) > 10000)
                {
                    exception_client.Add(proxy);
                }
            }

            foreach (var _client in remove_client)
            {
                lock (Hub.remove_chs)
                {
                    Hub.remove_chs.Add(_client._direct_ch);
                }
                direct_client_disconnect(_client._direct_ch);
            }

            foreach (var _client in exception_client)
            {
                direct_client_exception(_client._direct_ch);
            }

            Hub._timer.addticktime(10000, heartbeat_client);
        }

        public bool get_directproxy(Abelkhan.Ichannel direct_ch, out DirectProxy _proxy)
        {
            return ch_direct_clients.TryGetValue(direct_ch, out _proxy);
        }

        public void disconnect_client(String uuid)
        {
            if (clients.Remove(uuid, out GateProxy _proxy))
            {
                _proxy.disconnect_client(uuid);
            }

            if (direct_clients.Remove(uuid, out DirectProxy _directproxy)) {
                ch_direct_clients.Remove(_directproxy._direct_ch);
            }
        }

        public void call_client(String uuid, String func, ArrayList _argvs_list)
		{
            using var st = MemoryStreamPool.mstMgr.GetStream();
            ArrayList _event = new ArrayList
            {
                func,
                _argvs_list
            };
            _serializer.Pack(st, _event);
            st.Position = 0;
            var _rpc_bin = st.ToArray();

            if (direct_clients.TryGetValue(uuid, out DirectProxy _client))
            {
                _client.call_client(_rpc_bin);
            }
            else
            {
                if (clients.ContainsKey(uuid))
                {
                    clients[uuid].forward_hub_call_client(uuid, _rpc_bin);
                }
                else
                {
                    Log.Log.trace("no-exist client:", uuid);
                }
            }
        }

        public void call_group_client(List<string> uuids, String func, ArrayList _argvs_list)
        {
            var _direct_clients = new List<Abelkhan.Ichannel>();
            var _direct_clients_crypt = new List<Abelkhan.Ichannel>();
            var tmp_gates = new Dictionary<GateProxy, List<string> >();
            foreach (var _uuid in uuids)
            {
                if (direct_clients.TryGetValue(_uuid, out DirectProxy _client))
                {
                    if (_client._direct_ch.is_xor_key_crypt())
                    {
                        _direct_clients_crypt.Add(_client._direct_ch);
                    }
                    else
                    {
                        _direct_clients.Add(_client._direct_ch);
                    }
                    continue;
                }

                if (clients.TryGetValue(_uuid, out GateProxy _proxy))
                {
                    if (!tmp_gates.ContainsKey(_proxy))
                    {
                        tmp_gates.Add(_proxy, new List<string>());
                    }
                    tmp_gates[_proxy].Add(_uuid);
                }
            }

            using var st = MemoryStreamPool.mstMgr.GetStream();
            ArrayList _rpc_argv = new ArrayList
            {
                func,
                _argvs_list
            };
            _serializer.Pack(st, _rpc_argv);
            st.Position = 0;
            var _rpc_bin = st.ToArray();

            foreach (var _proxy in tmp_gates)
            {
                _proxy.Key.forward_hub_call_group_client(_proxy.Value, _rpc_bin);
            }

            if (_direct_clients.Count > 0)
            {
                using var st_event = MemoryStreamPool.mstMgr.GetStream();
                var _direct_rpc_argv = new ArrayList
                {
                    _rpc_bin
                };
                ArrayList _event = new ArrayList
                {
                    "hub_call_client_call_client",
                    _direct_rpc_argv
                };
                _serializer.Pack(st_event, _event);
                st_event.Position = 0;
                var data = st_event.ToArray();

                using var st_send = MemoryStreamPool.mstMgr.GetStream();
                var _tmplenght = data.Length;
                st_send.WriteByte((byte)(_tmplenght & 0xff));
                st_send.WriteByte((byte)((_tmplenght >> 8) & 0xff));
                st_send.WriteByte((byte)((_tmplenght >> 16) & 0xff));
                st_send.WriteByte((byte)((_tmplenght >> 24) & 0xff));
                st_send.Write(data, 0, _tmplenght);
                st_send.Position = 0;
                var buf = st_send.ToArray();

                foreach (var _client in _direct_clients)
                {
                    _client.send(buf);
                }
            }

            if (_direct_clients_crypt.Count > 0)
            {
                using var st_event = MemoryStreamPool.mstMgr.GetStream();
                var _direct_rpc_argv = new ArrayList
                {
                    _rpc_bin
                };
                ArrayList _event = new ArrayList
                {
                    "hub_call_client_call_client",
                    _direct_rpc_argv
                };
                _serializer.Pack(st_event, _event);
                st_event.Position = 0;
                var data = st_event.ToArray();

                using var st_send = MemoryStreamPool.mstMgr.GetStream();
                var _tmplenght = data.Length;
                st_send.WriteByte((byte)(_tmplenght & 0xff));
                st_send.WriteByte((byte)((_tmplenght >> 8) & 0xff));
                st_send.WriteByte((byte)((_tmplenght >> 16) & 0xff));
                st_send.WriteByte((byte)((_tmplenght >> 24) & 0xff));
                st_send.Write(data, 0, _tmplenght);
                st_send.Position = 0;
                var buf = st_send.ToArray();

                using var st_send_crypt = MemoryStreamPool.mstMgr.GetStream();
                st_send_crypt.Write(st_send.GetBuffer());
                st_send_crypt.Position = 0;
                var crypt_buf = st_send_crypt.ToArray();
                Abelkhan.Crypt.crypt_func_send(crypt_buf);

                foreach (var _client in _direct_clients_crypt)
                {
                    _client.send(crypt_buf);
                }
            }
        }

		public void call_global_client(String func, ArrayList _argvs_list)
		{
            var st = MemoryStreamPool.mstMgr.GetStream();
            ArrayList _event = new ArrayList
            {
                func,
                _argvs_list
            };
            _serializer.Pack(st, _event);
            st.Position = 0;
            var _rpc_bin = st.ToArray();
            foreach (var _proxy in ch_gateproxys)
			{
				_proxy.Value.forward_hub_call_global_client(_rpc_bin);
			}
		}
	}
}

