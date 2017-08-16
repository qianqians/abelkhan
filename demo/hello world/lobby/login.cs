using System;
using System.Collections;
using common;

namespace lobby
{
    class login : imodule
    {
        public void player_login(string token, string wechat_name)
        {
            var client_uuid = hub.hub.gates.current_client_uuid;

            Console.WriteLine("player_login " + client_uuid);

            if (server.players.has_player(token))
            {
                string old_uuid = server.players.relogin(token, client_uuid);

                hub.hub.gates.call_client(client_uuid, "login", "login_sucess");
                hub.hub.gates.call_client(old_uuid, "login", "other_login");

                /*if () 如果玩家在游戏中
                {

                }*/
            }
            else
            {
                Hashtable _query = new Hashtable();
                _query.Add("token", token);
                _query.Add("wechat_name", wechat_name);
                hub.hub.dbproxy.getCollection("test", "objects").getObjectInfo(_query, (ArrayList date_list) => { query_player_info(client_uuid, token, wechat_name, date_list); }, ()=> { });
            }
        }

        void query_player_info(string uuid, string token, string wechat_name, ArrayList date_list)
        {
            Console.WriteLine("db rsp " + uuid + token + wechat_name);

            if (date_list.Count > 1)
            {
                Console.WriteLine("error: repeate token");
            }

            if (date_list.Count == 0)
            {
                Hashtable _data = new Hashtable();
                _data.Add("token", token);
                _data.Add("wechat_name", wechat_name);
                hub.hub.dbproxy.getCollection("test", "objects").createPersistedObject(_data, ()=> { create_player(uuid, _data); });
            }
            else
            {
                reg_client_info(uuid, (Hashtable)date_list[0]);
            }

        }

        void create_player(string uuid, Hashtable _data)
        {
            reg_client_info(uuid, _data);
        }

        void reg_client_info(string uuid, Hashtable _data)
        {
            server.players.reg_player(uuid, _data);

            hub.hub.gates.call_client(uuid, "login", "login_sucess");
            hub.hub.gates.call_client_fast(uuid, "login", "udp_link_sucess");

            Console.WriteLine("rsp client " + uuid);
        }
    }
}
