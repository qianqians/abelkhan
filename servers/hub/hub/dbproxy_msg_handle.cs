using System;
using System.Collections;

namespace hub
{
	public class dbproxy_msg_handle
	{
		public dbproxy_msg_handle(dbproxyproxy dbproxy_)
		{
			_dbproxy = dbproxy_;
		}

		public void ack_create_persisted_object(Int64 callbackid)
		{
			dbproxyproxy.onCreatePersistedObjectHandle _handle = (dbproxyproxy.onCreatePersistedObjectHandle)_dbproxy.begin_callback(callbackid);
			_handle();
			_dbproxy.end_callback(callbackid);
		}

		public void ack_updata_persisted_object(Int64 callbackid)
		{
			dbproxyproxy.onUpdataPersistedObjectHandle _handle = (dbproxyproxy.onUpdataPersistedObjectHandle)_dbproxy.begin_callback(callbackid);
			_handle();
			_dbproxy.end_callback(callbackid);
		}

		public void ack_get_object_info(Int64 callbackid, String json_obejct_array)
		{
			dbproxyproxy.onGetObjectInfoHandle _handle = (dbproxyproxy.onGetObjectInfoHandle)_dbproxy.begin_callback(callbackid);
			_handle((ArrayList)(System.Text.Json.Jsonparser.unpack(json_obejct_array)));
		}

		public void ack_get_object_info_end(Int64 callbackid)
		{
			_dbproxy.end_callback(callbackid);
		}

		private dbproxyproxy _dbproxy;
	}
}

