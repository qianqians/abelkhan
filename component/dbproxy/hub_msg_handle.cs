using System;
using System.Collections;

namespace dbproxy
{
	public class hub_msg_handle
	{
		public hub_msg_handle(hubmanager _hubmanager_, mongodbproxy _mongodbproxy_)
		{
			_hubmanager = _hubmanager_;
			_mongodbproxy = _mongodbproxy_;
		}

		public void reg_hub(string uuid)
		{
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "hub {0} connected", uuid);

			hubproxy _hubproxy = _hubmanager.reg_hub(juggle.Imodule.current_ch, uuid);
			_hubproxy.reg_hub_sucess ();
		}

		public void create_persisted_object(string db, string collection, Hashtable object_info, string callbackid)
		{
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "begin create_persisted_object");

            dbproxy._dbevent.push_create_event(new create_event(db, collection, object_info, callbackid));

            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "end create_persisted_object");
        }

		public void updata_persisted_object(string db, string collection, Hashtable query_json, Hashtable object_info, string callbackid)
		{
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "begin updata_persisted_object");

            dbproxy._dbevent.push_updata_event(new update_event(db, collection, query_json, object_info, callbackid));

            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "end updata_persisted_object");
        }

        public void remove_object(string db, string collection, Hashtable query_json, string callbackid)
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "begin remove_object");

            dbproxy._dbevent.push_remove_event(new remove_event(db, collection, query_json, callbackid));

            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "end remove_object");
        }

        public void get_object_count(string db, string collection, Hashtable query_json, string callbackid)
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "begin get_object_info");

            dbproxy._dbevent.push_count_event(new count_event(db, collection, query_json, callbackid));

            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "end get_object_info");
        }

		public void get_object_info(string db, string collection, Hashtable query_json, string callbackid)
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "begin get_object_info");

            dbproxy._dbevent.push_find_event(new find_event(db, collection, query_json, callbackid));

            log.log.trace(new System.Diagnostics.StackFrame(true), service.timerservice.Tick, "end get_object_info");
        }

		private hubmanager _hubmanager;
		private mongodbproxy _mongodbproxy;
	}
}

