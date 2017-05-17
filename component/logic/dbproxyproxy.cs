using System;
using System.Collections;
using System.Collections.Generic;

namespace logic
{
	public class dbproxyproxy
	{
		public dbproxyproxy(service.connectnetworkservice _connect_)
		{
			_connect = _connect_;

			callback_set = new Dictionary<string, object>();
            end_cb_set = new Dictionary<string, object>();
        }

		public void connect(String ip, short port)
		{
			var ch = _connect.connect(ip, port);
			_logic_call_dbproxy = new caller.logic_call_dbproxy(ch);
		}

		public void reg_logic(String uuid)
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "begin connect dbproxy server");

			_logic_call_dbproxy.reg_logic(uuid);
		}

		public void createPersistedObject(Hashtable object_info, onCreatePersistedObjectHandle _handle)
		{
			var callbackid = System.Guid.NewGuid().ToString();
			_logic_call_dbproxy.create_persisted_object(object_info, callbackid);
			callback_set.Add(callbackid, (object)_handle);
		}

		public void updataPersistedObject(Hashtable query_json, Hashtable updata_info, onUpdataPersistedObjectHandle _handle)
		{
			var callbackid = System.Guid.NewGuid().ToString();
			_logic_call_dbproxy.updata_persisted_object(query_json, updata_info, callbackid);
			callback_set.Add(callbackid, (object)_handle);
		}

		public void getObjectInfo(Hashtable query_json, onGetObjectInfoHandle _handle, onGetObjectInfoEnd _end)
		{
			var callbackid = System.Guid.NewGuid().ToString();
			_logic_call_dbproxy.get_object_info(query_json, callbackid);
			callback_set.Add(callbackid, (object)_handle);
            end_cb_set.Add(callbackid, (object)_end);
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

        private service.connectnetworkservice _connect;
		private caller.logic_call_dbproxy _logic_call_dbproxy;
	}
}

