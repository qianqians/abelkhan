using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace hub
{
	public class dbproxyproxy
	{
        public static Dictionary<String, Action<ArrayList> > onGetObjectInfo_callback_set;
        public static Dictionary<String, Action> onGetObjectInfo_end_cb_set;

        private abelkhan.hub_call_dbproxy_caller _hub_call_dbproxy_caller;

        private Collection _Collection;

        public enum EM_DB_RESULT
        {
            EM_DB_SUCESSED = 0,
		    EM_DB_FAILD = 1,
		    EM_DB_TIMEOUT = 2,
	    }

        public dbproxyproxy(abelkhan.Ichannel ch)
		{
            _hub_call_dbproxy_caller = new abelkhan.hub_call_dbproxy_caller(ch, abelkhan.modulemng_handle._modulemng);

            onGetObjectInfo_callback_set = new Dictionary<String, Action<ArrayList>>();
            onGetObjectInfo_end_cb_set = new Dictionary<String, Action>();
        }

        public event Action on_connect_dbproxy_sucessed;
		public void reg_hub(String name)
        {
            log.log.trace("begin connect dbproxy server");

            _hub_call_dbproxy_caller.reg_hub(name).callBack(()=> {
                log.log.trace("connect dbproxy server sucessed");
                _Collection = new Collection(this);
                on_connect_dbproxy_sucessed?.Invoke();
            }, ()=> {
                log.log.trace("connect dbproxy server faild");
            }).timeout(5 * 1000, ()=> {
                log.log.trace("connect dbproxy server timeout");
            });
		}

        public Collection getCollection(string db, string collection)
        {
            _Collection.set_db_collection(db, collection);
           return _Collection;
        }

        public class Collection
        {
            public Collection(dbproxyproxy proxy)
            {
                _dbproxy = proxy;
            }

            public void set_db_collection(string db, string collection)
            {
                _db = db;
                _collection = collection;
            }

            public void createPersistedObject(MsgPack.MessagePackObjectDictionary object_info, Action<EM_DB_RESULT> _handle)
            {
                using (var st = new MemoryStream())
                {
                    var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<MsgPack.MessagePackObjectDictionary>();
                    _serialization.Pack(st, object_info);
                    st.Position = 0;
                    _dbproxy._hub_call_dbproxy_caller.create_persisted_object(_db, _collection, st.ToArray()).callBack(()=> {
                        log.log.trace("createPersistedObject sucessed!");
                        _handle(EM_DB_RESULT.EM_DB_SUCESSED);
                    }, ()=> {
                        log.log.err("createPersistedObject faild");
                        _handle(EM_DB_RESULT.EM_DB_FAILD);
                    }).timeout(5 * 1000, ()=> {
                        log.log.err("createPersistedObject timeout");
                        _handle(EM_DB_RESULT.EM_DB_TIMEOUT);
                    });
                }
            }

            public void updataPersistedObject(MsgPack.MessagePackObjectDictionary query_info, MsgPack.MessagePackObjectDictionary updata_info, bool is_upsert, Action<EM_DB_RESULT> _handle)
            {
                using (MemoryStream st_query = new MemoryStream(), st_update = new MemoryStream())
                {
                    var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<MsgPack.MessagePackObjectDictionary>();
                    
                    _serialization.Pack(st_query, query_info);
                    st_query.Position = 0;

                    _serialization.Pack(st_update, updata_info);
                    st_update.Position = 0;

                    _dbproxy._hub_call_dbproxy_caller.updata_persisted_object(_db, _collection, st_query.ToArray(), st_update.ToArray(), is_upsert).callBack(() =>
                    {
                        log.log.trace("updataPersistedObject sucessed!");
                        _handle(EM_DB_RESULT.EM_DB_SUCESSED);
                    }, () =>
                    {
                        log.log.trace("updataPersistedObject faild!");
                        _handle(EM_DB_RESULT.EM_DB_FAILD);
                    }).timeout(5 * 1000, () =>
                    {
                        log.log.trace("updataPersistedObject timeout!");
                        _handle(EM_DB_RESULT.EM_DB_TIMEOUT);
                    });
                }
            }

            public void findAndModifyObject(MsgPack.MessagePackObjectDictionary query_info, MsgPack.MessagePackObjectDictionary updata_info, bool _new, bool is_upsert, Action<EM_DB_RESULT, MsgPack.MessagePackObjectDictionary> _handle)
            {
                using (MemoryStream st_query = new MemoryStream(), st_update = new MemoryStream())
                {
                    var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<MsgPack.MessagePackObjectDictionary>();

                    _serialization.Pack(st_query, query_info);
                    st_query.Position = 0;

                    _serialization.Pack(st_update, updata_info);
                    st_update.Position = 0;

                    _dbproxy._hub_call_dbproxy_caller.find_and_modify(_db, _collection, st_query.ToArray(), st_update.ToArray(), _new, is_upsert).callBack((byte[] obj) =>
                    {
                        log.log.trace("findAndModifyObject sucessed!");

                        var st_obj = new MemoryStream();
                        st_obj.Write(obj);
                        st_obj.Position = 0;
                        var obj_table = _serialization.Unpack(st_obj);
                        _handle(EM_DB_RESULT.EM_DB_SUCESSED, obj_table);
                    }, () =>
                    {
                        log.log.trace("findAndModifyObject faild!");
                        _handle(EM_DB_RESULT.EM_DB_FAILD, null);
                    }).timeout(5 * 1000, () =>
                    {
                        log.log.trace("findAndModifyObject timeout!");
                        _handle(EM_DB_RESULT.EM_DB_TIMEOUT, null);
                    });
                }
            }

            public void getObjectCount(MsgPack.MessagePackObjectDictionary query_json, Action<EM_DB_RESULT, uint> _handle)
            {
                using (var st = new MemoryStream())
                {
                    var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<MsgPack.MessagePackObjectDictionary>();
                    _serialization.Pack(st, query_json);
                    st.Position = 0;
                    _dbproxy._hub_call_dbproxy_caller.get_object_count(_db, _collection, st.ToArray()).callBack((count)=> {
                        log.log.trace("getObjectCount sucessed!");
                        _handle(EM_DB_RESULT.EM_DB_SUCESSED, count);
                    }, ()=> {
                        log.log.trace("getObjectCount faild!");
                        _handle(EM_DB_RESULT.EM_DB_FAILD, 0);
                    }).timeout(5 * 1000, ()=> {
                        log.log.trace("getObjectCount timeout!");
                        _handle(EM_DB_RESULT.EM_DB_TIMEOUT, 0);
                    });
                }
            }

            public void getObjectInfo(MsgPack.MessagePackObjectDictionary query_obj, Action<ArrayList> _handle, Action _end)
            {
                using (var st = new MemoryStream())
                {
                    var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<MsgPack.MessagePackObjectDictionary>();
                    _serialization.Pack(st, query_obj);
                    st.Position = 0;

                    var callbackid = System.Guid.NewGuid().ToString();

                    _dbproxy._hub_call_dbproxy_caller.get_object_info(_db, _collection, st.ToArray(), callbackid);
                    dbproxyproxy.onGetObjectInfo_callback_set.Add(callbackid, _handle);
                    dbproxyproxy.onGetObjectInfo_end_cb_set.Add(callbackid, _end);
                }
            }

            public void removeObject(MsgPack.MessagePackObjectDictionary query_obj, Action<EM_DB_RESULT> _handle)
            {
                using (var st = new MemoryStream())
                {
                    var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<MsgPack.MessagePackObjectDictionary>();
                    _serialization.Pack(st, query_obj);
                    st.Position = 0;

                    _dbproxy._hub_call_dbproxy_caller.remove_object(_db, _collection, st.ToArray()).callBack(()=> {
                        log.log.trace("removeObject sucessed!");
                        _handle(EM_DB_RESULT.EM_DB_SUCESSED);
                    },()=> {
                        log.log.trace("removeObject faild!");
                        _handle(EM_DB_RESULT.EM_DB_FAILD);
                    }).timeout(5 * 1000, ()=>{
                        log.log.trace("removeObject timeout!");
                        _handle(EM_DB_RESULT.EM_DB_TIMEOUT);
                    });
                }
            }

            private string _db;
            private string _collection;
            private dbproxyproxy _dbproxy;
        }
	}
}

