using System;
using System.Collections;
using System.IO;

namespace hub
{
	public class dbproxy_msg_handle
	{
		private abelkhan.dbproxy_call_hub_module _dbproxy_call_hub_module;

		public dbproxy_msg_handle()
		{
			_dbproxy_call_hub_module = new abelkhan.dbproxy_call_hub_module(abelkhan.modulemng_handle._modulemng);
			_dbproxy_call_hub_module.on_ack_get_object_info += ack_get_object_info;
			_dbproxy_call_hub_module.on_ack_get_object_info_end += ack_get_object_info_end;
		}

		public void ack_get_object_info(String callbackid, byte[] obejct_array)
		{
			if (dbproxyproxy.onGetObjectInfo_callback_set.TryGetValue(callbackid, out Action<ArrayList> cb))
            {
				var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<ArrayList>();
				using (var st = new MemoryStream())
                {
					st.Write(obejct_array);
					st.Position = 0;

					var objs = _serialization.Unpack(st);
					cb(objs);
				}
            }
		}

		public void ack_get_object_info_end(String callbackid)
		{
			if (dbproxyproxy.onGetObjectInfo_end_cb_set.Remove(callbackid, out Action _end))
			{
				_end();
				dbproxyproxy.onGetObjectInfo_callback_set.Remove(callbackid);
			}
        }
	}
}

