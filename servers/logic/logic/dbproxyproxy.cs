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
		}

		public void connect(String ip, short port)
		{
			var ch = _connect.connect(ip, port);
			_logic_call_dbproxy = new caller.logic_call_dbproxy(ch);
		}

		public void reg_logic(String uuid)
		{
			Console.WriteLine("begin connect dbproxy server");
			_logic_call_dbproxy.reg_logic(uuid);
		}

		public void CreatePersistedObject(String object_info, onCreatePersistedObjectHandle _handle)
		{
			var callbackid = System.Guid.NewGuid().ToString();
			_logic_call_dbproxy.create_persisted_object(object_info, callbackid);
			callback_set.Add(callbackid, (object)_handle);
		}

		public void updataPersistedObject(String query_json, String updata_info, onUpdataPersistedObjectHandle _handle)
		{
			var callbackid = System.Guid.NewGuid().ToString();
			_logic_call_dbproxy.updata_persisted_object(query_json, updata_info, callbackid);
			callback_set.Add(callbackid, (object)_handle);
		}

		public void getObjectInfo(String query_json, onGetObjectInfoHandle _handle)
		{
			var callbackid = System.Guid.NewGuid().ToString();
			_logic_call_dbproxy.get_object_info(query_json, callbackid);
			callback_set.Add(callbackid, (object)_handle);
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

		public delegate void onCreatePersistedObjectHandle();
		public delegate void onUpdataPersistedObjectHandle();
		public delegate void onGetObjectInfoHandle(ArrayList obejctinfoarray);

		private Dictionary<String, object> callback_set;

		private service.connectnetworkservice _connect;
		private caller.logic_call_dbproxy _logic_call_dbproxy;
	}
}

