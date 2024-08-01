using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace OfflineMsg
{
    public class OfflineMsgMgr
    {
        private string _db_name;
        private string _db_collection;

        private Dictionary<string, Action<offline_msg> > _callback_dict;

        public OfflineMsgMgr(string db_name, string db_collection)
        {
            _db_name = db_name;
            _db_collection = db_collection;

            _callback_dict = new Dictionary<string, Action<offline_msg> >();
        }

        private Hub.DBProxyProxy.Collection Collection
        {
            get { return Hub.Hub.get_random_dbproxyproxy().getCollection(_db_name, _db_collection); }
        }

        public struct offline_msg
        {
            public string msg_guid;
            public string player_guid;
            public long send_timetmp;
            public string msg_type;
            public byte[] msg;
        }

        private Task<List<offline_msg> > get_role_offline_msg(string player_guid)
        {
            var task = new TaskCompletionSource<List<offline_msg> >();

            var ret_list = new List<offline_msg>();
            var _query = new Abelkhan.DBQueryHelper();
            _query.condition("player_guid", player_guid);
            Collection.getObjectInfo(_query.query(), (_msg_array) => {
                if (_msg_array.Count > 0)
                {
                    foreach (var bson_msg in _msg_array)
                    {
                        var msg = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<offline_msg>(bson_msg as BsonDocument);
                        ret_list.Add(msg);
                    }
                }

            }, () => {
                ret_list.Sort();
                task.SetResult(ret_list);
            });

            return task.Task;
        }

        public async ValueTask process_offline_msg(string player_guid)
        {
            try
            {
                var msg_list = await get_role_offline_msg(player_guid);
                foreach (var msg in msg_list)
                {
                    if (_callback_dict.TryGetValue(msg.msg_type, out Action<offline_msg> callback))
                    {
                        callback.Invoke(msg);
                    }
                    else
                    {
                        Log.Log.err("unsuport msg.msg_type:{0}", msg.msg_type);
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Log.err("process_offline_msg ex:{0}", ex);
            }
        }

        public Task<bool> send_offline_msg(offline_msg msg)
        {
            var task = new TaskCompletionSource<bool>();

            var _data = new Abelkhan.SaveDataHelper();
            _data.set(msg);
            Collection.createPersistedObject(_data.data(), (_result) =>
            {
                if (_result != Hub.DBProxyProxy.EM_DB_RESULT.EM_DB_SUCESSED)
                {
                    Log.Log.err("send_offline_msg faild, msg:{0}", msg.ToJson());
                    task.SetResult(false);
                }
                else
                {
                    task.SetResult(true);
                }
            });

            return task.Task;
        }

        public void del_offline_msg(string msg_guid)
        {
            var _query = new Abelkhan.DBQueryHelper();
            _query.condition("msg_guid", msg_guid);
            Collection.removeObject(_query.query(), (_result) => { 
                if (_result != Hub.DBProxyProxy.EM_DB_RESULT.EM_DB_SUCESSED)
                {
                    Log.Log.err("del_offline_msg faild, msg_guid:{0}", msg_guid);
                }
            });
        }

        public void register_offline_msg_callback(string msg_type, Action<offline_msg> callback)
        {
            _callback_dict[msg_type] = callback;
        }
    }
}