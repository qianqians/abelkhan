using System;
using System.Collections;
using System.Collections.Generic;

namespace lobby
{
    class playermng
    {
        public playermng()
        {
            players = new Dictionary<string, playerproxy>();
            players_uuid = new Dictionary<string, playerproxy>();
        }

        public playerproxy reg_player(string uuid, Hashtable _data)
        {
            if (!players.ContainsKey((string)_data["token"]) && !players_uuid.ContainsKey(uuid))
            {
                playerproxy _proxy = new playerproxy(uuid, _data);

                players.Add((string)_data["token"], _proxy);
                players_uuid.Add(uuid, _proxy);

                return _proxy;
            }

            Console.WriteLine("error player register info");

            return null;
        }

        public string relogin(string token, string client_uuid)
        {
            playerproxy _proxy = get_player_token(token);

            if (_proxy != null)
            {
                string old_uuid = _proxy.relogin(client_uuid);

                if (players_uuid.ContainsKey(old_uuid))
                {
                    players_uuid.Remove(old_uuid);
                }
                else
                {
                    Console.WriteLine("relogin:error player register info");
                }

                players_uuid.Add(client_uuid, _proxy);

                return old_uuid;
            }

            return "";
        }

        public bool has_player(string token)
        {
            if (players.ContainsKey(token))
            {
                return true;
            }

            return false;
        }

        public playerproxy get_player_token(string token)
        {
            if (players.ContainsKey(token))
            {
                return players[token];
            }

            return null;
        }

        public playerproxy get_player_uuid(string uuid)
        {
            foreach(var item in players_uuid)
            {
                Console.WriteLine(item.Key + ":" + item.Value);
            }
            if (players_uuid.ContainsKey(uuid))
            {
                return players_uuid[uuid];
            }

            return null;
        }

        private Dictionary<string, playerproxy> players;
        private Dictionary<string, playerproxy> players_uuid;
    }
}
