using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace dbproxy
{
    /*write event*/
    public class create_event
    {
        public create_event(string _db, string _collection, Hashtable _object_info, string _callbackid)
        {
            db = _db;
            collection = _collection;
            object_info = _object_info;
            callbackid = _callbackid;
        }

        public void do_event()
        {
            dbproxy._mongodbproxy.save(db, collection, object_info);

            hubproxy _hubproxy = dbproxy._hubmanager.get_hub(juggle.Imodule.current_ch);
            if (_hubproxy == null)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "hubproxy is null");
                return;
            }
            _hubproxy.ack_create_persisted_object(callbackid);

        }

        public string db;
        public string collection;
        public Hashtable object_info;
        public string callbackid;
    }

    public class update_event
    {
        public update_event(string _db, string _collection, Hashtable _query_json, Hashtable _object_info, string _callbackid)
        {
            db = _db;
            collection = _collection;
            query_json = _query_json;
            object_info = _object_info;
            callbackid = _callbackid;
        }

        public void do_event()
        {
            dbproxy._mongodbproxy.update(db, collection, query_json, object_info);

            hubproxy _hubproxy = dbproxy._hubmanager.get_hub(juggle.Imodule.current_ch);
            if (_hubproxy == null)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "hubproxy is null");
                return;
            }
            _hubproxy.ack_updata_persisted_object(callbackid);
        }

        public string db;
        public string collection;
        public Hashtable query_json;
        public Hashtable object_info;
        public string callbackid;
    }

    public class remove_event
    {
        public remove_event(string _db, string _collection, Hashtable _query_json, string _callbackid)
        {
            db = _db;
            collection = _collection;
            query_json = _query_json;
            callbackid = _callbackid;
        }

        public void do_event()
        {
            dbproxy._mongodbproxy.remove(db, collection, query_json);

            hubproxy _hubproxy = dbproxy._hubmanager.get_hub(juggle.Imodule.current_ch);
            if (_hubproxy == null)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "hubproxy is null");
                return;
            }
            _hubproxy.ack_remove_object(callbackid);
        }

        public string db;
        public string collection;
        public Hashtable query_json;
        public string callbackid;
    }

    /*read event*/
    public class count_event
    {
        public count_event(string _db, string _collection, Hashtable _query_json, string _callbackid)
        {
            db = _db;
            collection = _collection;
            query_json = _query_json;
            callbackid = _callbackid;
        }

        public void do_event()
        {
            ArrayList _list = dbproxy._mongodbproxy.find(db, collection, query_json);

            hubproxy _hubproxy = dbproxy._hubmanager.get_hub(juggle.Imodule.current_ch);
            if (_hubproxy == null)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "hubproxy is null");
                return;
            }
            _hubproxy.ack_get_object_count(callbackid, _list.Count);
        }

        public string db;
        public string collection;
        public Hashtable query_json;
        public string callbackid;
    }

    public class find_event
    {
        public find_event(string _db, string _collection, Hashtable _query_json, string _callbackid)
        {
            db = _db;
            collection = _collection;
            query_json = _query_json;
            callbackid = _callbackid;
        }

        public void do_event()
        {
            ArrayList _list = dbproxy._mongodbproxy.find(db, collection, query_json);

            hubproxy _hubproxy = dbproxy._hubmanager.get_hub(juggle.Imodule.current_ch);
            if (_hubproxy == null)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "hubproxy is null");
                return;
            }

            int count = 0;
            ArrayList _datalist = new ArrayList();
            if (_list.Count == 0)
            {
                _hubproxy.ack_get_object_info(callbackid, _datalist);
            }
            else
            {
                foreach (var data in _list)
                {
                    _datalist.Add(data);

                    count++;

                    if (count >= 100)
                    {
                        _hubproxy.ack_get_object_info(callbackid, _datalist);

                        count = 0;
                        _datalist = new ArrayList();
                    }
                }
                if (count > 0 && count < 100)
                {
                    _hubproxy.ack_get_object_info(callbackid, _datalist);
                }
            }
            _hubproxy.ack_get_object_info_end(callbackid);
        }

        public string db;
        public string collection;
        public Hashtable query_json;
        public string callbackid;
    }
    
    public class db_collection_write_event
    {
        private Queue<create_event> create_event_list = new Queue<create_event>();
        private Queue<update_event> updata_event_list = new Queue<update_event>();
        private Queue<remove_event> remove_event_list = new Queue<remove_event>();

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

        public void push_remove_event(remove_event _event)
        {
            lock (remove_event_list)
            {
                remove_event_list.Enqueue(_event);
            }
        }

        public void start()
        {
            Thread t = new Thread(() =>
            {
                bool do_nothing = true;
                while (true)
                {
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
        }
    }

    public class dbevent
    {
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
                    bool do_nothing = true;
                    while (true)
                    {
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
            }
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
            _write_event_list.start();

            collection_write_event_list.Add(collection, _write_event_list);
        }

        private Dictionary<string, db_collection_write_event> collection_write_event_list = new Dictionary<string, db_collection_write_event>();
    }
    
}

