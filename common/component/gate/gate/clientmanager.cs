using System;
using System.Collections;
using System.Collections.Generic;

namespace gate
{
	public class clientmanager
	{
		public clientmanager()
		{
			clientproxys = new Dictionary<string, clientproxy> ();
			clientproxys_ch = new Dictionary<juggle.Ichannel, clientproxy> ();
			clientproxy_logicproxy = new Dictionary<clientproxy, logicproxy> ();
			clientproxys_uuid = new Dictionary<clientproxy, string> ();
			client_server_time = new Dictionary<juggle.Ichannel, long>();
			client_time = new Dictionary<juggle.Ichannel, long>();
		}

		public clientproxy reg_client(string uuid, juggle.Ichannel ch, Int64 servertick, Int64 clienttick)
		{
			clientproxy _clientproxy = new clientproxy (ch);
            clientproxys.Add(uuid, _clientproxy);
            clientproxys_uuid.Add(_clientproxy, uuid);
            client_server_time.Add (ch, servertick);
			client_time.Add (ch, clienttick);

			return _clientproxy;
		}

        public void unreg_client(juggle.Ichannel ch)
        {
            if (clientproxys_ch.ContainsKey(ch))
            {
                clientproxy _proxy = clientproxys_ch[ch];
                clientproxys_ch.Remove(ch);
                client_server_time.Remove(ch);
                client_time.Remove(ch);

                if (clientproxys_uuid.ContainsKey(_proxy))
                {
                    string uuid = clientproxys_uuid[_proxy];
                    clientproxys_uuid.Remove(_proxy);

                    clientproxys.Remove(uuid);
                }
                if (clientproxy_logicproxy.ContainsKey(_proxy))
                {
                    clientproxy_logicproxy.Remove(_proxy);
                }
            }
        }

		public void reg_client_logic(string uuid, logicproxy _logicproxy)
		{
			var _clientproxy = get_clientproxy (uuid);
			if (_clientproxy != null)
			{
				clientproxy_logicproxy.Add (_clientproxy, _logicproxy);
			}
		}

		public bool has_client(string uuid)
		{
			return clientproxys.ContainsKey (uuid);
		}

		public void for_each_client(Action<clientproxy> func)
		{
			foreach (clientproxy _clientproxy in clientproxys.Values) 
			{
				func (_clientproxy);
			}
		}

		public clientproxy get_clientproxy(String uuid)
		{
			if (clientproxys.ContainsKey (uuid))
			{
				return clientproxys [uuid];
			}
			
			return null;
		}

		public clientproxy get_clientproxy(juggle.Ichannel ch)
		{
			if (clientproxys_ch.ContainsKey (ch))
			{
				return clientproxys_ch [ch];
			}

			return null;
		}

		public logicproxy get_clientproxy_logicproxy(juggle.Ichannel ch)
		{
			if (clientproxys_ch.ContainsKey (ch))
			{
				clientproxy _clientproxy = clientproxys_ch [ch];

				if (clientproxy_logicproxy.ContainsKey(_clientproxy))
				{
					return clientproxy_logicproxy[_clientproxy];
				}

				return null;
			}

			return null;
		}

		public String get_client_uuid(clientproxy _clientproxy)
		{
			if (clientproxys_uuid.ContainsKey (_clientproxy))
			{
				return clientproxys_uuid[_clientproxy];
			}
			
			return null;
		}

		public void refresh_and_check_client(juggle.Ichannel _ch, Int64 servertick, Int64 clienttick) {
			if (((clienttick - client_time[_ch]) - (servertick - client_server_time[_ch])) > 10 * 1000) {
				var _client = clientproxys_ch [_ch];
				var client_uuid = clientproxys_uuid[_client];
				if (clientproxy_logicproxy.ContainsKey (_client))
				{
					var _logic = clientproxy_logicproxy [_client];
					_logic.client_exception (client_uuid);
				}
			}

			client_server_time[_ch] = servertick;
			client_time[_ch] = clienttick;
		}

		private Dictionary<clientproxy, String> clientproxys_uuid;
		private Dictionary<juggle.Ichannel, clientproxy> clientproxys_ch;
		private Dictionary<String, clientproxy> clientproxys;

		private Dictionary<clientproxy, logicproxy> clientproxy_logicproxy;

		private Dictionary<juggle.Ichannel, Int64 > client_server_time;

		private Dictionary<juggle.Ichannel, Int64 > client_time;

	}
}

