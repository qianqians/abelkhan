using System;
using System.Collections;
using System.Collections.Generic;

namespace hub
{
	public class dbproxyproxy
	{
		public dbproxyproxy(juggle.Ichannel ch)
		{
			_hub_call_dbproxy = new caller.hub_call_dbproxy(ch);
			callback_set = new Dictionary<string, object>();
            end_cb_set = new Dictionary<string, object>();
        }

		public void reg_hub(String uuid)
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "begin connect dbproxy server");

			_hub_call_dbproxy.reg_hub(uuid);
		}

        public Collection getCollection(string db, string collection)
        {
           return new Collection(db, collection, this);
        }

        public class Collection
        {
            public Collection(string db, string collection, dbproxyproxy proxy)
            {
                _db = db;
                _collection = collection;

                _dbproxy = proxy;
            }

            public void createPersistedObject(Hashtable object_info, onCreatePersistedObjectHandle _handle)
            {
                var callbackid = System.Guid.NewGuid().ToString();
                create_persisted_object(object_info, callbackid);
                _dbproxy.callback_set.Add(callbackid, (object)_handle);
            }

            public void updataPersistedObject(Hashtable query_json, Hashtable updata_info, onUpdataPersistedObjectHandle _handle)
            {
                var callbackid = System.Guid.NewGuid().ToString();
                updata_persisted_object(query_json, updata_info, callbackid);
                _dbproxy.callback_set.Add(callbackid, (object)_handle);
            }

            public void getObjectInfo(Hashtable query_json, onGetObjectInfoHandle _handle, onGetObjectInfoEnd _end)
            {
                var callbackid = System.Guid.NewGuid().ToString();
                get_object_info(query_json, callbackid);
                _dbproxy.callback_set.Add(callbackid, (object)_handle);
                _dbproxy.end_cb_set.Add(callbackid, (object)_end);
            }

            private void create_persisted_object(Hashtable object_info, String callbackid)
            {
                _dbproxy._hub_call_dbproxy.create_persisted_object(_db, _collection, object_info, callbackid);
            }

            private void updata_persisted_object(Hashtable query_object, Hashtable updata_info, String callbackid)
            {
                _dbproxy._hub_call_dbproxy.updata_persisted_object(_db, _collection, query_object, updata_info, callbackid);
            }

            private void get_object_info(Hashtable query_object, String callbackid)
            {
                _dbproxy._hub_call_dbproxy.get_object_info(_db, _collection, query_object, callbackid);
            }

            private string _db;
            private string _collection;
            private dbproxyproxy _dbproxy;
        }

		public object begin_callback(String callbackid)
		{
			if (callback_set.ContainsKey(callbackid))
			{
				return callback_set[callbackid];
			}

			return null;
		}

		public void end_callback(String callbackid)
		{
			if (callback_set.ContainsKey(callbackid))
			{
				callback_set.Remove(callbackid);
			}
		}

        public object end_get_object_info_callback(String callbackid)
        {
            end_callback(callbackid);

            if (end_cb_set.ContainsKey(callbackid))
            {
                return end_cb_set[callbackid];
            }

            return null;
        }

        public delegate void onCreatePersistedObjectHandle();
		public delegate void onUpdataPersistedObjectHandle();
		public delegate void onGetObjectInfoHandle(ArrayList obejctinfoarray);
        public delegate void onGetObjectInfoEnd();

        private Dictionary<String, object> callback_set;
        private Dictionary<String, object> end_cb_set;

		private caller.hub_call_dbproxy _hub_call_dbproxy;
	}
}

