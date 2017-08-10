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
            clientproxy_hubproxy = new Dictionary<clientproxy, List<hubproxy> >();
			clientproxys_uuid = new Dictionary<clientproxy, string> ();
			client_server_time = new Dictionary<juggle.Ichannel, long>();
			client_time = new Dictionary<juggle.Ichannel, long>();
		}

		public clientproxy reg_client(string uuid, juggle.Ichannel ch, Int64 servertick, Int64 clienttick)
		{
			clientproxy _clientproxy = new clientproxy (ch);
            clientproxys_ch.Add(ch, _clientproxy);
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
            }
        }

        public void reg_client_hub(string uuid, hubproxy _hubproxy)
        {
            var _clientproxy = get_clientproxy(uuid);
            if (_clientproxy != null)
            {
                if (!clientproxy_hubproxy.ContainsKey(_clientproxy))
                {
                    clientproxy_hubproxy.Add(_clientproxy, new List<hubproxy>());
                }

                clientproxy_hubproxy[_clientproxy].Add(_hubproxy);
            }
        }

        public void unreg_client_hub(juggle.Ichannel ch)
        {
            if (clientproxys_ch.ContainsKey(ch))
            {
                clientproxy _proxy = clientproxys_ch[ch];
                if (clientproxy_hubproxy.ContainsKey(_proxy))
                {
                    clientproxy_hubproxy.Remove(_proxy);
                }
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

		public String get_client_uuid(clientproxy _clientproxy)
		{
			if (clientproxys_uuid.ContainsKey (_clientproxy))
			{
				return clientproxys_uuid[_clientproxy];
			}
			
			return null;
		}

		public void refresh_and_check_client(juggle.Ichannel _ch, Int64 servertick, Int64 clienttick)
        {
            if (client_time.ContainsKey(_ch) && client_server_time.ContainsKey(_ch))
            {
                if (((clienttick - client_time[_ch]) - (servertick - client_server_time[_ch])) > 10 * 1000)
                {
                    if (clientproxys_ch.ContainsKey(_ch))
                    {
                        var _client = clientproxys_ch[_ch];
                        var client_uuid = clientproxys_uuid[_client];
                        if (clientproxy_hubproxy.ContainsKey(_client))
                        {
                            var _hubs = clientproxy_hubproxy[_client];
                            foreach (var _hub in _hubs)
                            {
                                _hub.client_exception(client_uuid);
                            }
                        }
                    }
                }

                client_server_time[_ch] = servertick;
                client_time[_ch] = clienttick;
            }
		}

        public void tick_client(Int64 servertick)
        {
            var remove = new List<juggle.Ichannel>();
            foreach (KeyValuePair<juggle.Ichannel, Int64> kvp in client_server_time)
            {
                if ((servertick - kvp.Value) > 9 * 1000)
                {
                    var _client = clientproxys_ch[kvp.Key];
                    var client_uuid = clientproxys_uuid[_client];
                    if (clientproxy_hubproxy.ContainsKey(_client))
                    {
                        var _hubs = clientproxy_hubproxy[_client];
                        foreach(var _hub in _hubs)
                        {
                            _hub.client_disconnect(client_uuid);
                        }
                        clientproxy_hubproxy.Remove(_client);
                    }

                    remove.Add(kvp.Key);
                }
            }

            foreach (var ch in remove)
            {
                var _client = clientproxys_ch[ch];

                clientproxys_ch.Remove(ch);
                client_server_time.Remove(ch);
                client_time.Remove(ch);
                if (clientproxys_uuid.ContainsKey(_client))
                {
                    string uuid = clientproxys_uuid[_client];
                    clientproxys_uuid.Remove(_client);

                    clientproxys.Remove(uuid);
                }

                ch.disconnect();
            }

            gate.timer.addticktime(5 * 1000, gate.clients.tick_client);
        }

        private Dictionary<clientproxy, String> clientproxys_uuid;
		private Dictionary<juggle.Ichannel, clientproxy> clientproxys_ch;
		private Dictionary<String, clientproxy> clientproxys;
        
        private Dictionary<clientproxy, List<hubproxy> > clientproxy_hubproxy;

        private Dictionary<juggle.Ichannel, Int64 > client_server_time;

		private Dictionary<juggle.Ichannel, Int64 > client_time;

	}
}

