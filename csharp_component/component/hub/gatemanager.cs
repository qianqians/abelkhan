using System;
using System.Collections;
using System.Collections.Generic;

namespace hub
{
    class directproxy
    {
        private abelkhan.hub_call_client_caller _hub_call_client_caller;

        public string _cuuid;
        public abelkhan.Ichannel _direct_ch;

        public directproxy(string cuuid_, abelkhan.Ichannel direct_ch)
        {
            _cuuid = cuuid_;
            _direct_ch = direct_ch;

            _hub_call_client_caller = new abelkhan.hub_call_client_caller(_direct_ch, abelkhan.modulemng_handle._modulemng);
        }

        void call_client(byte[] rpc_argv)
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

        public void call_client(String uuid, String module, String func, params object[] _argvs)
		{
			if (clients.ContainsKey(uuid))
			{
                ArrayList _argvs_list = new ArrayList();
                foreach (var o in _argvs)
                {
                    _argvs_list.Add(o);
                }

                clients[uuid].forward_hub_call_client(uuid, module, func, _argvs_list);
            }
            else
            {
                log.log.trace("no-exist client:", uuid);
            }
        }

        public void call_group_client(List<string> uuids, String module, String func, params object[] _argvs)
        {
            ArrayList _argvs_list = new ArrayList();
            foreach (var o in _argvs)
            {
                _argvs_list.Add(o);
            }

            var tmp_gates = new Dictionary<gateproxy, List<string> >();
            foreach (var _uuid in uuids)
            {
                if (clients.TryGetValue(_uuid, out gateproxy _proxy))
                {
                    if (!tmp_gates.ContainsKey(_proxy))
                    {
                        tmp_gates.Add(_proxy, new List<string>());
                    }
                    tmp_gates[_proxy].Add(_uuid);
                }
            }

            foreach (var _proxy in tmp_gates)
			{
				_proxy.Key.forward_hub_call_group_client(_proxy.Value, module, func, _argvs_list);
			}
		}

		public void call_global_client(String module, String func, params object[] _argvs)
		{
            ArrayList _argvs_list = new ArrayList();
            foreach (var o in _argvs)
            {
                _argvs_list.Add(o);
            }

            foreach (var _proxy in ch_gateproxys)
			{
				_proxy.Value.forward_hub_call_global_client(module, func, _argvs_list);
			}
		}
	}
}

