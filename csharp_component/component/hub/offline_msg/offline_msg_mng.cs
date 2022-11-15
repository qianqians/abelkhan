using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace offline_msg
{
    public class offline_msg_mng
    {
        private hub.dbproxyproxy _dbproxy;
        private string _db_name;
        private string _db_collection;

        private Dictionary<string, Action<offline_msg> > _callback_dict;

        public offline_msg_mng(hub.dbproxyproxy dbproxy, string db_name, string db_collection)
        {
            _dbproxy = dbproxy;
            _db_name = db_name;
            _db_collection = db_collection;

            _callback_dict = new Dictionary<string, Action<offline_msg> >();
        }

        private hub.dbproxyproxy.Collection Collection
        {
            get { return _dbproxy.getCollection(_db_name, _db_collection); }
        }

        public struct offline_msg
        {
            public string msg_guid;
            public string role_account;
            public long send_timetmp;
            public string msg_type;
            public byte[] msg;
        }

        private Task<List<offline_msg> > get_role_offline_msg(string account)
        {
            var task = new TaskCompletionSource<List<offline_msg> >();

            var ret_list = new List<offline_msg>();
            var _query = new abelkhan.DBQueryHelper();
            _query.condition("role_account", account);
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

        public async Task process_offline_msg(string account)
        {
            var msg_list = await get_role_offline_msg(account);
            foreach (var msg in msg_list)
            {
                if (_callback_dict.TryGetValue(msg.msg_type, out Action<offline_msg> callback))
                {
                    callback.Invoke(msg);
                }
                else
                {
                    log.log.err("unsuport msg.msg_type:{0}", msg.msg_type);
                }
            }
        }

        public Task<bool> send_offline_msg(offline_msg msg)
        {
            var task = new TaskCompletionSource<bool>();

            var _data = new abelkhan.SaveDataHelper();
            _data.set(msg);
            Collection.createPersistedObject(_data.data(), (_result) =>
            {
                if (_result != hub.dbproxyproxy.EM_DB_RESULT.EM_DB_SUCESSED)
                {
                    log.log.err("send_offline_msg faild, msg:{0}", msg.ToJson());
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
            var _query = new abelkhan.DBQueryHelper();
            _query.condition("msg_guid", msg_guid);
            Collection.removeObject(_query.query(), (_result) => { 
                if (_result != hub.dbproxyproxy.EM_DB_RESULT.EM_DB_SUCESSED)
                {
                    log.log.err("del_offline_msg faild, msg_guid:{0}", msg_guid);
                }
            });
        }

        public void register_offline_msg_callback(string msg_type, Action<offline_msg> callback)
        {
            _callback_dict[msg_type] = callback;
        }
    }
}