﻿using System;
using System.Collections;

namespace hub
{
	public class dbproxy_msg_handle
	{
		public dbproxy_msg_handle(hub _hub)
		{
            _hub_ = _hub;
        }

		public void reg_hub_sucess()
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "connect dbproxy server sucess");

            _hub_.onConnectDB_event();
		}

		public void ack_create_persisted_object(String callbackid, bool is_create_sucess)
		{
			dbproxyproxy.onCreatePersistedObjectHandle _handle = (dbproxyproxy.onCreatePersistedObjectHandle)hub.dbproxy.begin_callback(callbackid);
			_handle(is_create_sucess);
			hub.dbproxy.end_callback(callbackid);
		}

		public void ack_updata_persisted_object(String callbackid)
		{
			dbproxyproxy.onUpdataPersistedObjectHandle _handle = (dbproxyproxy.onUpdataPersistedObjectHandle)hub.dbproxy.begin_callback(callbackid);
			_handle();
            hub.dbproxy.end_callback(callbackid);
		}

        public void ack_get_object_count(String callbackid, Int64 count)
        {
            dbproxyproxy.onGetObjectCountHandle _handle = (dbproxyproxy.onGetObjectCountHandle)hub.dbproxy.begin_callback(callbackid);
            _handle(count);
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

        public void ack_remove_object(String callbackid)
        {
            dbproxyproxy.onRemoveObjectHandle _handle = (dbproxyproxy.onRemoveObjectHandle)hub.dbproxy.begin_callback(callbackid);
            _handle();
        }

        private hub _hub_;
	}
}

