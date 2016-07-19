using System;
using System.Collections;
using System.Collections.Generic;

namespace logic
{
	public class gatemanager
	{
		public gatemanager(service.connectnetworkservice _conn)
		{
			_gate_conn = _conn;
			current_client_uuid = "";
		}

		public void connect_gate(String uuid, String ip, short port)
		{
			var ch = _gate_conn.connect(ip, port);
			gates.Add(uuid, new gateproxy(ch));
			ch_gateproxys.Add(ch, gates[uuid]);
			gates[uuid].reg_logic();
		}

		public gateproxy get_gateproxy(juggle.Ichannel gate_ch)
		{
			if (ch_gateproxys.ContainsKey(gate_ch))
			{
				return ch_gateproxys[gate_ch];
			}

			return null;
		}

		public delegate void clientConnecthandle(String client_uuid);
		public event clientConnecthandle clientConnect;
		public void client_connect(String client_uuid, juggle.Ichannel gate_ch)
		{
			if (ch_gateproxys.ContainsKey(gate_ch))
			{
				var _proxy = ch_gateproxys[gate_ch];
				clients.Add(client_uuid, _proxy);

				clientConnect(client_uuid);
			}
		}

		public delegate void clientDisconnecthandle(String client_uuid);
		public event clientDisconnecthandle clientDisconnect;
		public void client_disconnect(String client_uuid)
		{
			if (clients.ContainsKey(client_uuid))
			{
				clients.Remove(client_uuid);
				clientDisconnect(client_uuid);
			}
		}

		public void call_client(String uuid, String module, String func, String argv)
		{
			if (clients.ContainsKey(uuid))
			{
				clients[uuid].forward_logic_call_client(uuid, module, func, argv);
			}
		}

		public void call_group_client(ArrayList uuids, String module, String func, String argv)
		{
			foreach (var _proxy in ch_gateproxys)
			{
				_proxy.Value.forward_logic_call_group_client(uuids, module, func, argv);
			}
		}

		public void call_global_client(String module, String func, String argv)
		{
			foreach (var _proxy in ch_gateproxys)
			{
				_proxy.Value.forward_logic_call_global_client(module, func, argv);
			}
		}
	
		public String current_client_uuid;

		private Dictionary<String, gateproxy> clients;

		private Dictionary<juggle.Ichannel, gateproxy> ch_gateproxys;
		private Dictionary<String, gateproxy> gates;
		private service.connectnetworkservice _gate_conn;
	}
}

