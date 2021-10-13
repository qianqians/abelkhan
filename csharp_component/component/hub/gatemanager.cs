using MsgPack.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace hub
{
    public class directproxy
    {
        private abelkhan.hub_call_client_caller _hub_call_client_caller;

        public string _cuuid;
        public abelkhan.Ichannel _direct_ch;

        public long _timetmp = 0;
        public long _theory_timetmp = 0;

        public directproxy(string cuuid_, abelkhan.Ichannel direct_ch)
        {
            _cuuid = cuuid_;
            _direct_ch = direct_ch;

            _hub_call_client_caller = new abelkhan.hub_call_client_caller(_direct_ch, abelkhan.modulemng_handle._modulemng);
        }

        public void call_client(byte[] rpc_argv)
        {
            _hub_call_client_caller.call_client(rpc_argv);
        }
    }

    public class gatemanager
	{
        public String current_client_uuid;

        private Dictionary<String, gateproxy> clients;

        private Dictionary<abelkhan.Ichannel, gateproxy> ch_gateproxys;
        private Dictionary<String, gateproxy> gates;

        private Dictionary<string, directproxy> direct_clients;
        private Dictionary<abelkhan.Ichannel, directproxy> ch_direct_clients;

        private abelkhan.enetservice _gate_conn;

        public gatemanager(abelkhan.enetservice _conn)
		{
			_gate_conn = _conn;
			current_client_uuid = "";

			clients = new Dictionary<string, gateproxy>();
			ch_gateproxys = new Dictionary<abelkhan.Ichannel, gateproxy>();
			gates = new Dictionary<string, gateproxy>();

            direct_clients = new Dictionary<string, directproxy>();
            ch_direct_clients = new Dictionary<abelkhan.Ichannel, directproxy>();

            hub._timer.addticktime(10 * 1000, heartbeat_client);
        }

		public void connect_gate(String name, String ip, ushort port)
		{
			_gate_conn.connect(ip, port, (ch)=> {
                var _proxy = new gateproxy(ch);
                gates.Add(name, _proxy);
                ch_gateproxys.Add(ch, _proxy);
                lock (hub.add_chs)
                {
                    hub.add_chs.Add(ch);
                }
                _proxy.reg_hub();
            });
		}

		public gateproxy get_gateproxy(abelkhan.Ichannel gate_ch)
		{
			if (ch_gateproxys.ContainsKey(gate_ch))
			{
				return ch_gateproxys[gate_ch];
			}
			return null;
		}

        public event Action<string> on_gate_closed;
        public void gate_be_closed(string svr_name)
        {
            if (gates.Remove(svr_name, out gateproxy _proxy))
            {
                foreach (var it in clients)
                {
                    if (it.Value == _proxy)
                    {
                        clients.Remove(it.Key);
                        break;
                    }
                }
                ch_gateproxys.Remove(_proxy._ch);

                lock (hub.remove_chs)
                {
                    hub.remove_chs.Add(_proxy._ch);
                }

                on_gate_closed?.Invoke(svr_name);
            }
        }

        public void client_connect(String client_uuid, abelkhan.Ichannel gate_ch)
        {
            if (!ch_gateproxys.ContainsKey(gate_ch))
            {
                log.log.err("invaild gate");
                return;
            }
            clients[client_uuid] = ch_gateproxys[gate_ch];

            log.log.trace("client {0} connected", client_uuid);
        }

        public event Action<string> clientDisconnect;
        public void client_disconnect(String client_uuid)
        {
            if (clients.ContainsKey(client_uuid))
            {
                clients.Remove(client_uuid);
                clientDisconnect?.Invoke(client_uuid);
            }
        }

        public event Action<string> clientException;
        public void client_exception(String client_uuid)
        {
            clientException?.Invoke(client_uuid);
        }

        public void direct_client_connect(String client_uuid, abelkhan.Ichannel direct_ch)
        {
            if (direct_clients.Remove(client_uuid, out directproxy _directproxy))
            {
                ch_direct_clients.Remove(_directproxy._direct_ch);
            }

            log.log.trace("reg direct client:{0}", client_uuid);

            var _directproxy_new = new directproxy(client_uuid, direct_ch);
            direct_clients.Add(client_uuid, _directproxy_new);

            ch_direct_clients.Add(direct_ch, _directproxy_new);
        }

        public event Action<string> directClientDisconnect;
        public void direct_client_disconnect(abelkhan.Ichannel direct_ch)
        {
            if (ch_direct_clients.Remove(direct_ch, out directproxy _proxy))
            {
                direct_clients.Remove(_proxy._cuuid);
                directClientDisconnect?.Invoke(_proxy._cuuid);
            }
        }

        public void direct_client_exception(abelkhan.Ichannel direct_ch)
        {
            if (ch_direct_clients.TryGetValue(direct_ch, out directproxy _proxy))
            {
                clientException?.Invoke(_proxy._cuuid);
            }
        }

        void heartbeat_client(long ticktime)
        {
            List<directproxy> remove_client = new List<directproxy>();
            List<directproxy> exception_client = new List<directproxy>();
            foreach (var item in direct_clients)
            {
                var proxy = item.Value;
                if (proxy._timetmp > 0 && (proxy._timetmp + 10 * 1000) < ticktime)
                {
                    remove_client.Add(proxy);
                }
                if (proxy._timetmp > 0 && proxy._theory_timetmp > 0 && (proxy._theory_timetmp - proxy._timetmp) > 10 * 1000)
                {
                    exception_client.Add(proxy);
                }
            }

            foreach (var _client in remove_client)
            {
                lock (hub.remove_chs)
                {
                    hub.remove_chs.Add(_client._direct_ch);
                }
                direct_client_disconnect(_client._direct_ch);
            }

            foreach (var _client in exception_client)
            {
                direct_client_exception(_client._direct_ch);
            }
        }

        public directproxy get_directproxy(abelkhan.Ichannel direct_ch)
        {
            if (ch_direct_clients.TryGetValue(direct_ch, out directproxy _proxy))
            {
                return _proxy;
            }
            return null;
        }

        public void disconnect_client(String uuid)
        {
            if (clients.Remove(uuid, out gateproxy _proxy))
            {
                _proxy.disconnect_client(uuid);
            }

            if (direct_clients.Remove(uuid, out directproxy _directproxy)) {
                ch_direct_clients.Remove(_directproxy._direct_ch);
            }
        }

        public void call_client(String uuid, String module, String func, ArrayList _argvs_list)
		{
            if (direct_clients.TryGetValue(uuid, out directproxy _client))
            {
                using (var st = new MemoryStream())
                {
                    var _serializer = MessagePackSerializer.Get<ArrayList>();

                    ArrayList _event = new ArrayList();
                    _event.Add(module);
                    _event.Add(func);
                    _event.Add(_argvs_list);
                    _serializer.Pack(st, _event);
                    st.Position = 0;

                    _client.call_client(st.ToArray());
                }
            }

            if (clients.ContainsKey(uuid))
			{
                clients[uuid].forward_hub_call_client(uuid, module, func, _argvs_list);
            }
            else
            {
                log.log.trace("no-exist client:", uuid);
            }
        }

        public void call_group_client(List<string> uuids, String module, String func, ArrayList _argvs_list)
        {
            var _direct_clients = new List<directproxy>();
            var tmp_gates = new Dictionary<gateproxy, List<string> >();
            foreach (var _uuid in uuids)
            {
                if (direct_clients.TryGetValue(_uuid, out directproxy _client))
                {
                    _direct_clients.Add(_client);
                    continue;
                }

                if (clients.TryGetValue(_uuid, out gateproxy _proxy))
                {
                    if (!tmp_gates.ContainsKey(_proxy))
                    {
                        tmp_gates.Add(_proxy, new List<string>());
                    }
                    tmp_gates[_proxy].Add(_uuid);
                }
            }

            foreach (var _client in _direct_clients)
            {
                using (MemoryStream st = new MemoryStream(), st_event = new MemoryStream(), st_send = new MemoryStream())
                {
                    var _serializer = MessagePackSerializer.Get<ArrayList>();

                    ArrayList _rpc_argv = new ArrayList();
                    _rpc_argv.Add(module);
                    _rpc_argv.Add(func);
                    _rpc_argv.Add(_argvs_list);
                    _serializer.Pack(st, _rpc_argv);
                    st.Position = 0;

                    ArrayList _event = new ArrayList();
                    _event.Add("hub_call_client");
                    _event.Add("call_client");
                    _event.Add(st.ToArray());
                    _serializer.Pack(st_event, _rpc_argv);
                    st_event.Position = 0;
                    var data = st_event.ToArray();

                    var _tmplenght = data.Length;
                    st_send.WriteByte((byte)(_tmplenght & 0xff));
                    st_send.WriteByte((byte)((_tmplenght >> 8) & 0xff));
                    st_send.WriteByte((byte)((_tmplenght >> 16) & 0xff));
                    st_send.WriteByte((byte)((_tmplenght >> 24) & 0xff));
                    st_send.Write(data, 0, _tmplenght);
                    st_send.Position = 0;

                    _client._direct_ch.send(st_send.ToArray());
                }
            }

            foreach (var _proxy in tmp_gates)
			{
				_proxy.Key.forward_hub_call_group_client(_proxy.Value, module, func, _argvs_list);
			}
		}

		public void call_global_client(String module, String func, ArrayList _argvs_list)
		{
            foreach (var _proxy in ch_gateproxys)
			{
				_proxy.Value.forward_hub_call_global_client(module, func, _argvs_list);
			}
		}
	}
}

