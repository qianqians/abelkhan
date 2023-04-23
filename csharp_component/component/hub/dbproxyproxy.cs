using abelkhan;
using System;
using System.Collections.Generic;
using System.IO;

namespace hub
{
	public class DBproxyproxy
	{
        public static Dictionary<String, Action<MongoDB.Bson.BsonArray> > onGetObjectInfo_callback_set;
        public static Dictionary<String, Action> onGetObjectInfo_end_cb_set;

        public string db_name;
        public abelkhan.Ichannel ch;
        private abelkhan.hub_call_dbproxy_caller _hub_call_dbproxy_caller;

        private System.Collections.Concurrent.ConcurrentDictionary<string, Collection> _Collections;

        public enum EM_DB_RESULT
        {
            EM_DB_SUCESSED = 0,
		    EM_DB_FAILD = 1,
		    EM_DB_TIMEOUT = 2,
	    }

        public DBproxyproxy(string _db_name, abelkhan.Ichannel _ch)
		{
            db_name = _db_name;
            ch = _ch;

            _hub_call_dbproxy_caller = new abelkhan.hub_call_dbproxy_caller(ch, abelkhan.modulemng_handle._modulemng);

            onGetObjectInfo_callback_set = new Dictionary<String, Action<MongoDB.Bson.BsonArray> >();
            onGetObjectInfo_end_cb_set = new Dictionary<String, Action>();

            _Collections = new System.Collections.Concurrent.ConcurrentDictionary<string, Collection>();
        }

        public event Action on_connect_dbproxy_sucessed;
		public void reg_hub(String name)
        {
            log.Log.trace("begin connect dbproxy server");

            _hub_call_dbproxy_caller.reg_hub(name).callBack(()=> {
                log.Log.trace("connect dbproxy server sucessed");
                on_connect_dbproxy_sucessed?.Invoke();
            }, ()=> {
                log.Log.trace("connect dbproxy server faild");
            }).timeout(5000, ()=> {
                log.Log.trace("connect dbproxy server timeout");
            });
		}

        public Collection getCollection(string db, string collection)
        {
            var name = string.Format("{0}_{1}", db, collection);
            if (!_Collections.TryGetValue(name, out Collection pcollection))
            {
                pcollection = new Collection(db, collection, this);
                _Collections.TryAdd(name, pcollection);
            }
           return pcollection;
        }

        public class Collection
        {
            public Collection(string db, string collection, DBproxyproxy proxy)
            {
                _db = db;
                _collection = collection;

                _dbproxy = proxy;
            }

            public void createPersistedObject(MongoDB.Bson.BsonDocument object_info, Action<EM_DB_RESULT> _handle)
            {
                using var st = MemoryStreamPool.mstMgr.GetStream();
                var write = new MongoDB.Bson.IO.BsonBinaryWriter(st);
                MongoDB.Bson.Serialization.BsonSerializer.Serialize(write, object_info);
                st.Position = 0;

                lock (_dbproxy)
                {
                    _dbproxy._hub_call_dbproxy_caller.create_persisted_object(_db, _collection, st.ToArray()).callBack(() =>
                    {
                        log.Log.trace("createPersistedObject sucessed!");
                        _handle(EM_DB_RESULT.EM_DB_SUCESSED);
                    }, () =>
                    {
                        log.Log.err("createPersistedObject faild");
                        _handle(EM_DB_RESULT.EM_DB_FAILD);
                    }).timeout(5000, () =>
                    {
                        log.Log.err("createPersistedObject timeout");
                        _handle(EM_DB_RESULT.EM_DB_TIMEOUT);
                    });
                }
            }

            public void updataPersistedObject(MongoDB.Bson.BsonDocument query_info, MongoDB.Bson.BsonDocument updata_info, bool is_upsert, Action<EM_DB_RESULT> _handle)
            {
                using var st_query = MemoryStreamPool.mstMgr.GetStream();
                var write_query = new MongoDB.Bson.IO.BsonBinaryWriter(st_query);
                MongoDB.Bson.Serialization.BsonSerializer.Serialize(write_query, query_info);
                st_query.Position = 0;

                using var st_update = MemoryStreamPool.mstMgr.GetStream();
                var write_update = new MongoDB.Bson.IO.BsonBinaryWriter(st_update);
                MongoDB.Bson.Serialization.BsonSerializer.Serialize(write_update, updata_info);
                st_update.Position = 0;

                lock (_dbproxy)
                {
                    _dbproxy._hub_call_dbproxy_caller.updata_persisted_object(_db, _collection, st_query.ToArray(), st_update.ToArray(), is_upsert).callBack(() =>
                    {
                        log.Log.trace("updataPersistedObject sucessed!");
                        _handle(EM_DB_RESULT.EM_DB_SUCESSED);
                    }, () =>
                    {
                        log.Log.trace("updataPersistedObject faild!");
                        _handle(EM_DB_RESULT.EM_DB_FAILD);
                    }).timeout(5000, () =>
                    {
                        log.Log.trace("updataPersistedObject timeout!");
                        _handle(EM_DB_RESULT.EM_DB_TIMEOUT);
                    });
                }
            }

            public void findAndModifyObject(MongoDB.Bson.BsonDocument query_info, MongoDB.Bson.BsonDocument updata_info, bool _new, bool is_upsert, Action<EM_DB_RESULT, MongoDB.Bson.BsonDocument> _handle)
            {
                using var st_query = MemoryStreamPool.mstMgr.GetStream();
                var write_query = new MongoDB.Bson.IO.BsonBinaryWriter(st_query);
                MongoDB.Bson.Serialization.BsonSerializer.Serialize(write_query, query_info);

                using var st_update = MemoryStreamPool.mstMgr.GetStream();
                var write_update = new MongoDB.Bson.IO.BsonBinaryWriter(st_update);
                MongoDB.Bson.Serialization.BsonSerializer.Serialize(write_update, updata_info);

                lock (_dbproxy)
                {
                    _dbproxy._hub_call_dbproxy_caller.find_and_modify(_db, _collection, st_query.ToArray(), st_update.ToArray(), _new, is_upsert).callBack((byte[] obj) =>
                    {
                        log.Log.trace("findAndModifyObject sucessed!");

                        var obj_table = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(obj);
                        _handle(EM_DB_RESULT.EM_DB_SUCESSED, obj_table);
                    }, () =>
                    {
                        log.Log.trace("findAndModifyObject faild!");
                        _handle(EM_DB_RESULT.EM_DB_FAILD, null);
                    }).timeout(5000, () =>
                    {
                        log.Log.trace("findAndModifyObject timeout!");
                        _handle(EM_DB_RESULT.EM_DB_TIMEOUT, null);
                    });
                }
            }

            public void getObjectCount(MongoDB.Bson.BsonDocument query_json, Action<EM_DB_RESULT, uint> _handle)
            {
                using var st = MemoryStreamPool.mstMgr.GetStream();
                var write = new MongoDB.Bson.IO.BsonBinaryWriter(st);
                MongoDB.Bson.Serialization.BsonSerializer.Serialize(write, query_json);
                st.Position = 0;

                lock (_dbproxy)
                {
                    _dbproxy._hub_call_dbproxy_caller.get_object_count(_db, _collection, st.ToArray()).callBack((count) =>
                    {
                        log.Log.trace("getObjectCount sucessed!");
                        _handle(EM_DB_RESULT.EM_DB_SUCESSED, count);
                    }, () =>
                    {
                        log.Log.trace("getObjectCount faild!");
                        _handle(EM_DB_RESULT.EM_DB_FAILD, 0);
                    }).timeout(5000, () =>
                    {
                        log.Log.trace("getObjectCount timeout!");
                        _handle(EM_DB_RESULT.EM_DB_TIMEOUT, 0);
                    });
                }
            }

            public void getObjectInfo(MongoDB.Bson.BsonDocument query_obj, Action<MongoDB.Bson.BsonArray> _handle, Action _end)
            {
                using var st = MemoryStreamPool.mstMgr.GetStream();
                var write = new MongoDB.Bson.IO.BsonBinaryWriter(st);
                MongoDB.Bson.Serialization.BsonSerializer.Serialize(write, query_obj);
                st.Position = 0;

                var callbackid = System.Guid.NewGuid().ToString();

                lock (_dbproxy)
                {
                    _dbproxy._hub_call_dbproxy_caller.get_object_info(_db, _collection, st.ToArray(), 0, 0, "", false, callbackid);
                    DBproxyproxy.onGetObjectInfo_callback_set.Add(callbackid, _handle);
                    DBproxyproxy.onGetObjectInfo_end_cb_set.Add(callbackid, _end);
                }
            }

            public void getObjectInfoEx(MongoDB.Bson.BsonDocument query_obj, int skip, int limit, string sort, bool _Ascending, Action<MongoDB.Bson.BsonArray> _handle, Action _end)
            {
                using var st = MemoryStreamPool.mstMgr.GetStream();
                var write = new MongoDB.Bson.IO.BsonBinaryWriter(st);
                MongoDB.Bson.Serialization.BsonSerializer.Serialize(write, query_obj);
                st.Position = 0;

                var callbackid = System.Guid.NewGuid().ToString();

                lock (_dbproxy)
                {
                    _dbproxy._hub_call_dbproxy_caller.get_object_info(_db, _collection, st.ToArray(), skip, limit, sort, _Ascending, callbackid);
                    DBproxyproxy.onGetObjectInfo_callback_set.Add(callbackid, _handle);
                    DBproxyproxy.onGetObjectInfo_end_cb_set.Add(callbackid, _end);
                }
            }

            public void getGuid(Action<EM_DB_RESULT, long> _handle)
            {
                lock (_dbproxy)
                {
                    _dbproxy._hub_call_dbproxy_caller.get_guid(_db, _collection).callBack((guid) => 
                    {
                        log.Log.trace("getGuid sucessed!");
                        _handle(EM_DB_RESULT.EM_DB_SUCESSED, guid);
                    }, () => 
                    {
                        log.Log.trace("getGuid faild!");
                        _handle(EM_DB_RESULT.EM_DB_FAILD, -1);
                    }).timeout(5000, () => 
                    {
                        log.Log.trace("getGuid timeout!");
                        _handle(EM_DB_RESULT.EM_DB_TIMEOUT, -1);
                    });
                }
            }

            public void removeObject(MongoDB.Bson.BsonDocument query_obj, Action<EM_DB_RESULT> _handle)
            {
                using var st = MemoryStreamPool.mstMgr.GetStream();
                var write = new MongoDB.Bson.IO.BsonBinaryWriter(st);
                MongoDB.Bson.Serialization.BsonSerializer.Serialize(write, query_obj);
                st.Position = 0;

                lock (_dbproxy)
                {
                    _dbproxy._hub_call_dbproxy_caller.remove_object(_db, _collection, st.ToArray()).callBack(() =>
                    {
                        log.Log.trace("removeObject sucessed!");
                        _handle(EM_DB_RESULT.EM_DB_SUCESSED);
                    }, () =>
                    {
                        log.Log.trace("removeObject faild!");
                        _handle(EM_DB_RESULT.EM_DB_FAILD);
                    }).timeout(5000, () =>
                    {
                        log.Log.trace("removeObject timeout!");
                        _handle(EM_DB_RESULT.EM_DB_TIMEOUT);
                    });
                }
            }

            private string _db;
            private string _collection;
            private DBproxyproxy _dbproxy;
        }
	}
}

