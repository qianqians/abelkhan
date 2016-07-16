using System;
using System.Collections;
using System.Collections.Generic;

namespace hub
{
	public class dbproxyproxy
	{
		public dbproxyproxy(juggle.Ichannel ch)
		{
			callbackidcount = 0;
			_hub_call_dbproxy = new caller.hub_call_dbproxy(ch);
		}

		public void reg_hub(String uuid)
		{
			_hub_call_dbproxy.reg_hub(uuid);
		}

		public void createPersistedObject(String object_info, onCreatePersistedObjectHandle _handle)
		{
			create_persisted_object(object_info, callbackidcount);
			callback_set.Add(callbackidcount, (object)_handle);
			callbackidcount++;
		}

		public void updataPersistedObject(String query_object, String updata_info, onUpdataPersistedObjectHandle _handle)
		{
			updata_persisted_object(query_object, updata_info, callbackidcount);
			callback_set.Add(callbackidcount, (object)_handle);
			callbackidcount++;
		}

		public void getObjectInfo(String query_object, onGetObjectInfoHandle _handle)
		{
			get_object_info(query_object, callbackidcount);
			callback_set.Add(callbackidcount, (object)_handle);
			callbackidcount++;
		}

		private void create_persisted_object(String object_info, Int64 callbackid)
		{
			_hub_call_dbproxy.create_persisted_object(object_info, callbackid);	
		}

		private void updata_persisted_object(String query_object, String updata_info, Int64 callbackid)
		{
			_hub_call_dbproxy.updata_persisted_object(query_object, updata_info, callbackid);
		}

		private void get_object_info(String query_object, Int64 callbackid)
		{
			_hub_call_dbproxy.get_object_info(query_object, callbackid);
		}

		public object begin_callback(Int64 callbackid)
		{
			if (callback_set.ContainsKey(callbackid))
			{
				return callback_set[callbackid];
			}

			return null;
		}

		public void end_callback(Int64 callbackid)
		{
			if (callback_set.ContainsKey(callbackid))
			{
				callback_set.Remove(callbackid);
			}
		}

		public delegate void onCreatePersistedObjectHandle();
		public delegate void onUpdataPersistedObjectHandle();
		public delegate void onGetObjectInfoHandle(ArrayList obejctinfoarray);

		private Int64 callbackidcount;
		private Dictionary<Int64, object> callback_set;

		private caller.hub_call_dbproxy _hub_call_dbproxy;
	}
}

