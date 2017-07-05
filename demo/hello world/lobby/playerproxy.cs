using System;
using System.Collections;
using System.Collections.Generic;

namespace lobby
{
    class playerproxy
    {
        public playerproxy(string client_uuid, Hashtable _data)
        {
            uuid = client_uuid;
            player_info = _data;
        }

        public string relogin(string client_uuid)
        {
            string tmp = uuid;
            uuid = client_uuid;

            return tmp;
        }

        public string uuid;
        public Hashtable player_info;
    }
}
