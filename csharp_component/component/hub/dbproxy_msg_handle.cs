using System;
using System.Collections;
using System.IO;

namespace Hub
{
	public class dbproxy_msg_handle
	{
		private Abelkhan.dbproxy_call_hub_module _dbproxy_call_hub_module;

		public dbproxy_msg_handle()
		{
			_dbproxy_call_hub_module = new Abelkhan.dbproxy_call_hub_module(Abelkhan.ModuleMgrHandle._modulemng);
			_dbproxy_call_hub_module.on_ack_get_object_info += ack_get_object_info;
			_dbproxy_call_hub_module.on_ack_get_object_info_end += ack_get_object_info_end;
		}

		public void ack_get_object_info(String callbackid, byte[] obejct_array)
		{
			if (DBProxyProxy.onGetObjectInfo_callback_set.TryGetValue(callbackid, out Action<MongoDB.Bson.BsonArray> cb))
            {
				var objs = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<MongoDB.Bson.BsonDocument>(obejct_array);
				cb(objs.GetValue("_list").AsBsonArray);
            }
		}

		public void ack_get_object_info_end(String callbackid)
		{
			if (DBProxyProxy.onGetObjectInfo_end_cb_set.Remove(callbackid, out Action _end))
			{
				_end();
				DBProxyProxy.onGetObjectInfo_callback_set.Remove(callbackid);
			}
        }
	}
}

