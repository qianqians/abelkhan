using System.Collections;
using System.Collections.Generic;

namespace lobby
{
    class playermng
    {
        public playermng()
        {
            players = new Dictionary<string, playerproxy>();
        }

        public playerproxy reg_player(string uuid, Hashtable _data)
        {
            if (!players.ContainsKey((string)_data["token"]))
            {
                playerproxy _proxy = new playerproxy(uuid, _data);
                players.Add((string)_data["token"], _proxy);

                return _proxy;
            }

            return null; 
        }

        public playerproxy find_player(string token)
        {
            if (players.ContainsKey(token))
            {
                return players[token];
            }

            return null;
        }

        private Dictionary<string, playerproxy> players;
    }
}
