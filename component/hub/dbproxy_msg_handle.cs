using System;
using System.Collections;

namespace hub
{
	public class dbproxy_msg_handle
	{
		public dbproxy_msg_handle()
		{
		}

		public void reg_hub_sucess()
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "connect dbproxy server sucess");
		}

		public void ack_create_persisted_object(String callbackid)
		{
			dbproxyproxy.onCreatePersistedObjectHandle _handle = (dbproxyproxy.onCreatePersistedObjectHandle)hub.dbproxy.begin_callback(callbackid);
			_handle();
			hub.dbproxy.end_callback(callbackid);
		}

		public void ack_updata_persisted_object(String callbackid)
		{
			dbproxyproxy.onUpdataPersistedObjectHandle _handle = (dbproxyproxy.onUpdataPersistedObjectHandle)hub.dbproxy.begin_callback(callbackid);
			_handle();
            hub.dbproxy.end_callback(callbackid);
		}

		public void ack_get_object_info(String callbackid, ArrayList json_obejct_array)
		{
			dbproxyproxy.onGetObjectInfoHandle _handle = (dbproxyproxy.onGetObjectInfoHandle)hub.dbproxy.begin_callback(callbackid);
			_handle(json_obejct_array);
		}

		public void ack_get_object_info_end(String callbackid)
		{
            dbproxyproxy.onGetObjectInfoEnd _end = (dbproxyproxy.onGetObjectInfoEnd)hub.dbproxy.end_get_object_info_callback(callbackid);
            _end();
        }
	}
}

