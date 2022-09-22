using MsgPack.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace dbproxy
{
    /*write event*/
    public class get_guid_event
    {
        public get_guid_event(hubproxy _hubproxy_, string _db, string _collection, string _guid_key, abelkhan.hub_call_dbproxy_get_guid_rsp _rsp)
        {
            _hubproxy = _hubproxy_;
            db = _db;
            collection = _collection;
            guid_key = _guid_key;
            rsp = _rsp;
        }

        public async void do_event()
        {
            var guid = await dbproxy._mongodbproxy.get_guid(db, collection, guid_key);
            if (guid >= 0)
            {
                rsp.rsp(guid);
            }
            else
            {
                rsp.err();
            }
        }

        public hubproxy _hubproxy;
        public string db;
        public string collection;
        public string guid_key;
        public abelkhan.hub_call_dbproxy_get_guid_rsp rsp;
    }

    public class create_event
    {
        public create_event(hubproxy _hubproxy_, string _db, string _collection, byte[] _msgpack_object_data, abelkhan.hub_call_dbproxy_create_persisted_object_rsp _rsp)
        {
            _hubproxy = _hubproxy_;
            db = _db;
            collection = _collection;
            msgpack_object_data = _msgpack_object_data;
            rsp = _rsp;
        }

        public async void do_event()
        {
            var is_create_sucess = await dbproxy._mongodbproxy.save(db, collection, msgpack_object_data);
            if (is_create_sucess)
            {
                rsp.rsp();
            }
            else
            {
                rsp.err();
            }
        }

        public hubproxy _hubproxy;
        public string db;
        public string collection;
        public byte[] msgpack_object_data;
        public abelkhan.hub_call_dbproxy_create_persisted_object_rsp rsp;
    }

    public class update_event
    {
        public update_event(hubproxy _hubproxy_, string _db, string _collection, byte[] _query_data, byte[] _object_data, bool _upsert, abelkhan.hub_call_dbproxy_updata_persisted_object_rsp _rsp)
        {
            _hubproxy = _hubproxy_;
            db = _db;
            collection = _collection;
            query_data = _query_data;
            object_data = _object_data;
            upsert = _upsert;
            rsp = _rsp;
        }

        public async void do_event()
        {
            var is_update_sucessed = await dbproxy._mongodbproxy.update(db, collection, query_data, object_data, upsert);
            if (is_update_sucessed)
            {
                rsp.rsp();
            }
            else
            {
                rsp.err();
            }
        }

        public hubproxy _hubproxy;
        public string db;
        public string collection;
        public byte[] query_data; 
        public byte[] object_data;
        public bool upsert;
        public abelkhan.hub_call_dbproxy_updata_persisted_object_rsp rsp;
    }

    public class find_and_modify_event
    {
        public find_and_modify_event(hubproxy _hubproxy_, string _db, string _collection, byte[] _query_data, byte[] _object_data, bool _is_new, bool _upsert, abelkhan.hub_call_dbproxy_find_and_modify_rsp _rsp)
        {
            _hubproxy = _hubproxy_;
            db = _db;
            collection = _collection;
            query_data = _query_data;
            object_data = _object_data;
            is_new = _is_new;
            upsert = _upsert;
            rsp = _rsp;
        }

        public async void do_event()
        {
            var obj = await dbproxy._mongodbproxy.find_and_modify(db, collection, query_data, object_data, is_new, upsert);
            if (obj != null)
            {
                using (var st = new MemoryStream())
                {
                    var write = new MongoDB.Bson.IO.BsonBinaryWriter(st);
                    MongoDB.Bson.Serialization.BsonSerializer.Serialize(write, obj);
                    st.Position = 0;

                    rsp.rsp(st.ToArray());
                }
            }
            else
            {
                rsp.err();
            }
        }

        public hubproxy _hubproxy;
        public string db;
        public string collection;
        public byte[] query_data;
        public byte[] object_data;
        public bool is_new;
        public bool upsert;
        public abelkhan.hub_call_dbproxy_find_and_modify_rsp rsp;
    }

    public class remove_event
    {
        public remove_event(hubproxy _hubproxy_, string _db, string _collection, byte[] _query_data, abelkhan.hub_call_dbproxy_remove_object_rsp _rsp)
        {
            _hubproxy = _hubproxy_;
            db = _db;
            collection = _collection;
            query_data = _query_data;
            rsp = _rsp;
        }

        public async void do_event()
        {
            var is_remove_sucessed = await dbproxy._mongodbproxy.remove(db, collection, query_data);
            if (is_remove_sucessed)
            {
                rsp.rsp();
            }
            else
            {
                rsp.err();
            }
        }

        public hubproxy _hubproxy;
        public string db;
        public string collection;
        public byte[] query_data; 
        public abelkhan.hub_call_dbproxy_remove_object_rsp rsp;
    }

    /*read event*/
    public class count_event
    {
        public count_event(hubproxy _hubproxy_, string _db, string _collection, byte[] _query_data, abelkhan.hub_call_dbproxy_get_object_count_rsp _rsp)
        {
            _hubproxy = _hubproxy_;
            db = _db;
            collection = _collection;
            query_data = _query_data;
            rsp = _rsp;
        }

        public async void do_event()
        {
            var count = await dbproxy._mongodbproxy.count(db, collection, query_data);
            rsp.rsp((uint)count);
        }

        public hubproxy _hubproxy;
        public string db;
        public string collection;
        public byte[] query_data;
        public abelkhan.hub_call_dbproxy_get_object_count_rsp rsp;
    }

    public class find_event
    {
        public find_event(hubproxy _hubproxy_, string _db, string _collection, byte[] _query_data, int _skip, int _limit, string _sort, bool _Ascending_, string _callbackid)
        {
            _hubproxy = _hubproxy_;
            db = _db;
            collection = _collection;
            query_data = _query_data;
            skip = _skip;
            limit = _limit;
            sort = _sort;
            _Ascending = _Ascending_;
            callbackid = _callbackid;
        }

        public async void do_event()
        {
            var _list = await dbproxy._mongodbproxy.find(db, collection, query_data, limit, skip, sort, _Ascending);

            int count = 0;
            if (_list.Count == 0)
            {
                _hubproxy.ack_get_object_info(callbackid, new MongoDB.Bson.BsonDocument { { "_list", _list } });
            }
            else
            {
                var _datalist = new MongoDB.Bson.BsonArray();
                foreach (var data in _list)
                {
                    _datalist.Add(data);

                    count++;

                    if (count >= 100)
                    {
                        _hubproxy.ack_get_object_info(callbackid, new MongoDB.Bson.BsonDocument { { "_list", _datalist } });

                        count = 0;
                        _datalist = new MongoDB.Bson.BsonArray();
                    }
                }
                if (count > 0 && count < 100)
                {
                    _hubproxy.ack_get_object_info(callbackid, new MongoDB.Bson.BsonDocument { { "_list", _datalist } });
                }
            }
            _hubproxy.ack_get_object_info_end(callbackid);
        }

        public hubproxy _hubproxy;
        public string db;
        public string collection;
        public byte[] query_data;
        public int skip;
        public int limit;
        public string sort;
        public bool _Ascending;
        public string callbackid;
    }
    
    public class db_collection_write_event
    {
        private Queue<get_guid_event> get_guid_event_list = new Queue<get_guid_event>();
        private Queue<create_event> create_event_list = new Queue<create_event>();
        private Queue<update_event> updata_event_list = new Queue<update_event>();
        private Queue<find_and_modify_event> find_and_modify_event_list = new Queue<find_and_modify_event>();
        private Queue<remove_event> remove_event_list = new Queue<remove_event>();

        public void push_get_guid_event(get_guid_event _event)
        {
            lock (get_guid_event_list)
            {
                get_guid_event_list.Enqueue(_event);
            }
        }

        public void push_create_event(create_event _event)
        {
            lock(create_event_list)
            {
                create_event_list.Enqueue(_event);
            }
        }

        public void push_updata_event(update_event _event)
        {
            lock (updata_event_list)
            {
                updata_event_list.Enqueue(_event);
            }
        }

        public void push_find_and_modify_event(find_and_modify_event _event)
        {
            lock (find_and_modify_event_list)
            {
                find_and_modify_event_list.Enqueue(_event);
            }
        }

        public void push_remove_event(remove_event _event)
        {
            lock (remove_event_list)
            {
                remove_event_list.Enqueue(_event);
            }
        }

        public Thread start()
        {
            Thread t = new Thread(() =>
            {
                while (!dbproxy._closeHandle.is_close())
                {
                    bool do_nothing = true;
                    lock (get_guid_event_list)
                    {
                        if (get_guid_event_list.Count > 0)
                        {
                            var _event = get_guid_event_list.Dequeue();
                            _event.do_event();
                            do_nothing = false;
                        }
                    }
                    lock (create_event_list)
                    {
                        if (create_event_list.Count > 0)
                        {
                            var _event = create_event_list.Dequeue();
                            _event.do_event();
                            do_nothing = false;
                        }
                    }
                    lock (updata_event_list)
                    {
                        if (updata_event_list.Count > 0)
                        {
                            var _event = updata_event_list.Dequeue();
                            _event.do_event();
                            do_nothing = false;
                        }
                    }
                    lock (find_and_modify_event_list)
                    {
                        if (find_and_modify_event_list.Count > 0)
                        {
                            var _event = find_and_modify_event_list.Dequeue();
                            _event.do_event();
                            do_nothing = false;
                        }
                    }
                    lock (remove_event_list)
                    {
                        if (remove_event_list.Count > 0)
                        {
                            var _event = remove_event_list.Dequeue();
                            _event.do_event();
                            do_nothing = false;
                        }
                    }
                    if (do_nothing)
                    {
                        Thread.Sleep(5);
                    }
                }
            });
            t.Start();

            return t;
        }
    }

    public class dbevent
    {
        private List<Thread> th_list = new List<Thread>();

        public void join_all()
        {
            foreach(var t in th_list)
            {
                t.Join();
            }
        }

        private Queue<count_event> count_event_list = new Queue<count_event>();
        private Queue<find_event> find_event_list = new Queue<find_event>();

        public void push_count_event(count_event _event)
        {
            lock (count_event_list)
            {
                count_event_list.Enqueue(_event);
            }
        }

        public void push_find_event(find_event _event)
        {
            lock (find_event_list)
            {
                find_event_list.Enqueue(_event);
            }
        }

        public void start()
        {
            for (int i = 0; i < 4; i++)
            {
                Thread t = new Thread(() =>
                {
                    while (!dbproxy._closeHandle.is_close())
                    {
                        bool do_nothing = true;
                        lock (count_event_list)
                        {
                            if (count_event_list.Count > 0)
                            {
                                var _event = count_event_list.Dequeue();
                                _event.do_event();
                                do_nothing = false;
                            }
                        }
                        lock (find_event_list)
                        {
                            if (find_event_list.Count > 0)
                            {
                                var _event = find_event_list.Dequeue();
                                _event.do_event();
                                do_nothing = false;
                            }
                        }
                        if (do_nothing)
                        {
                            Thread.Sleep(5);
                        }
                    }
                });
                t.Start();

                th_list.Add(t);
            }
        }

        public void push_get_guid_event(get_guid_event _event)
        {
            if (!collection_write_event_list.ContainsKey(_event.collection))
            {
                start_write(_event.collection);
            }

            collection_write_event_list[_event.collection].push_get_guid_event(_event);
        }

        public void push_create_event(create_event _event)
        {
            if (!collection_write_event_list.ContainsKey(_event.collection))
            {
                start_write(_event.collection);
            }

            collection_write_event_list[_event.collection].push_create_event(_event);
        }

        public void push_updata_event(update_event _event)
        {
            if (!collection_write_event_list.ContainsKey(_event.collection))
            {
                start_write(_event.collection);
            }

            collection_write_event_list[_event.collection].push_updata_event(_event);
        }

        public void push_find_and_modify_event(find_and_modify_event _event)
        {
            if (!collection_write_event_list.ContainsKey(_event.collection))
            {
                start_write(_event.collection);
            }

            collection_write_event_list[_event.collection].push_find_and_modify_event(_event);
        }

        public void push_remove_event(remove_event _event)
        {
            if (!collection_write_event_list.ContainsKey(_event.collection))
            {
                start_write(_event.collection);
            }

            collection_write_event_list[_event.collection].push_remove_event(_event);
        }

        private void start_write(string collection)
        {
            if (collection_write_event_list.ContainsKey(collection))
            {
                return;
            }

            var _write_event_list = new db_collection_write_event();
            th_list.Add(_write_event_list.start());

            collection_write_event_list.Add(collection, _write_event_list);
        }

        private Dictionary<string, db_collection_write_event> collection_write_event_list = new Dictionary<string, db_collection_write_event>();
    }
    
}

