using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gate
{
	public class ClientProxy {
		public long _timetmp = 0;
		public long _theory_timetmp = 0;
		public string _cuuid;

		public HashSet<HubProxy> conn_hubproxys;

		public Abelkhan.Ichannel _ch;
		public Abelkhan.gate_call_client_caller _gate_call_client_caller;

		public ClientProxy(string cuuid, Abelkhan.Ichannel ch) {
			conn_hubproxys = new();

            _cuuid = cuuid;
			_ch = ch;
			_gate_call_client_caller = new Abelkhan.gate_call_client_caller(_ch, Abelkhan.ModuleMgrHandle._modulemng);
		}

		public void conn_hub(HubProxy hub_proxy) {
            if (conn_hubproxys.Contains(hub_proxy))
            {
                return;
            }
            conn_hubproxys.Add(hub_proxy);
        }

		public void unreg_hub(HubProxy hub_proxy) {
            conn_hubproxys.Remove(hub_proxy);
        }

		public void ntf_cuuid()
		{
			_gate_call_client_caller.ntf_cuuid(_cuuid);
		}

		public void ntf_reason(string reason)
		{
			_gate_call_client_caller.kick_off_reason(reason);
        }

        public void call_client(string hub_name, byte[] data)
		{
			_gate_call_client_caller.call_client(hub_name, data);
		}
        
		public void migrate_client_start(string src_hub, string _target_hub)
        {
            _gate_call_client_caller.migrate_client_start(src_hub, _target_hub);
        }

        public void migrate_client_done(string src_hub, string _target_hub)
		{
            _gate_call_client_caller.migrate_client_done(src_hub, _target_hub);
        }

		public void hub_loss(string hub_name)
		{
			_gate_call_client_caller.hub_loss(hub_name);
        }

		public bool is_xor_key_crypt()
		{
			return _ch.is_xor_key_crypt();
		}
	}

	public class ClientManager
	{
		private readonly Dictionary<string, ClientProxy> client_uuid_map = new();
		private readonly Dictionary<Abelkhan.Ichannel, ClientProxy> client_map = new();

		private readonly HubSvrManager _hubsvrmanager;

		public ClientManager(HubSvrManager _hubsvrmanager_)
		{
			_hubsvrmanager = _hubsvrmanager_;
		}

		public void heartbeat_client(long ticktime)
		{
			List<ClientProxy> remove_client = new();
			List<ClientProxy> exception_client = new();
			foreach (var item in client_map)
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
				Parallel.ForEach(_client.conn_hubproxys, hubproxy_ =>
				{
					hubproxy_.client_disconnect(_client._cuuid);
				});
				unreg_client(_client._ch);
			}

			foreach (var proxy in exception_client)
			{
                Parallel.ForEach(proxy.conn_hubproxys, hubproxy_ =>
                {
                    hubproxy_.client_exception(proxy._cuuid);
                });
            }
		}

		public ClientProxy reg_client(Abelkhan.Ichannel ch)
		{
			var cuuid = Guid.NewGuid().ToString("N");

			var client_proxy = new ClientProxy(cuuid, ch);

			client_uuid_map.Add(cuuid, client_proxy);
			client_map.Add(ch, client_proxy);

			client_proxy._timetmp = 0;

			return client_proxy;
		}

		public void unreg_client(Abelkhan.Ichannel _ch)
		{
			if (!client_map.ContainsKey(_ch))
			{
				return;
			}

			var _client = client_map[_ch];
			Log.Log.trace("unreg_client:{0}", _client._cuuid);

			if (client_uuid_map.ContainsKey(_client._cuuid))
			{
				client_uuid_map.Remove(_client._cuuid);
			}
			if (client_map.ContainsKey(_ch))
			{
				client_map.Remove(_ch);
			}
		}

		public bool has_client(Abelkhan.Ichannel ch)
		{
			return client_map.ContainsKey(ch);
		}

		public bool has_client(string uuid)
		{
			return client_uuid_map.ContainsKey(uuid);
		}

		public bool get_client(Abelkhan.Ichannel ch, out ClientProxy proxy)
		{
			return client_map.TryGetValue(ch, out proxy);
		}

		public bool get_client(string cuuid, out ClientProxy proxy)
		{
            return client_uuid_map.TryGetValue(cuuid, out proxy);
		}

		public void for_each_client(Action<string, ClientProxy> fn)
		{
			Parallel.ForEach(client_uuid_map, client =>
			{
                fn(client.Key, client.Value);
            });
        }
	}
}
