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
		}

		public void reg_hub(String uuid)
		{
			Console.WriteLine("begin connect dbproxy server");
			_hub_call_dbproxy.reg_hub(uuid);
		}

		public void createPersistedObject(String object_info, onCreatePersistedObjectHandle _handle)
		{
			var callbackid = System.Guid.NewGuid().ToString();
			create_persisted_object(object_info, callbackid);
			callback_set.Add(callbackid, (object)_handle);
		}

		public void updataPersistedObject(String query_json, String updata_info, onUpdataPersistedObjectHandle _handle)
		{
			var callbackid = System.Guid.NewGuid().ToString();
			updata_persisted_object(query_json, updata_info, callbackid);
			callback_set.Add(callbackid, (object)_handle);
		}

		public void getObjectInfo(String query_json, onGetObjectInfoHandle _handle)
		{
			var callbackid = System.Guid.NewGuid().ToString();
			get_object_info(query_json, callbackid);
			callback_set.Add(callbackid, (object)_handle);
		}

		private void create_persisted_object(String object_info, String callbackid)
		{
			_hub_call_dbproxy.create_persisted_object(object_info, callbackid);	
		}

		private void updata_persisted_object(String query_object, String updata_info, String callbackid)
		{
			_hub_call_dbproxy.updata_persisted_object(query_object, updata_info, callbackid);
		}

		private void get_object_info(String query_object, String callbackid)
		{
			_hub_call_dbproxy.get_object_info(query_object, callbackid);
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

		private caller.hub_call_dbproxy _hub_call_dbproxy;
	}
}

