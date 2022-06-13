using System;
using System.Collections;

namespace dbproxy
{
	public class hub_msg_handle
    {
        private closehandle _closehandle;
        private hubmanager _hubmanager;
        private abelkhan.hub_call_dbproxy_module _hub_call_dbproxy_module;

        public hub_msg_handle(hubmanager _hubmanager_, closehandle _closehandle_)
		{
			_hubmanager = _hubmanager_;
            _closehandle = _closehandle_;

            _hub_call_dbproxy_module = new abelkhan.hub_call_dbproxy_module(abelkhan.modulemng_handle._modulemng);
            _hub_call_dbproxy_module.on_reg_hub += reg_hub;
            _hub_call_dbproxy_module.on_create_persisted_object += create_persisted_object;
            _hub_call_dbproxy_module.on_updata_persisted_object += updata_persisted_object;
            _hub_call_dbproxy_module.on_find_and_modify += find_and_modify;
            _hub_call_dbproxy_module.on_remove_object += remove_object;
            _hub_call_dbproxy_module.on_get_object_count += get_object_count;
            _hub_call_dbproxy_module.on_get_object_info += get_object_info;
        }

		public void reg_hub(string hub_name)
		{
            log.log.trace("hub {0} connected", hub_name);
                
            var rsp = (abelkhan.hub_call_dbproxy_reg_hub_rsp)_hub_call_dbproxy_module.rsp;
            hubproxy _hubproxy = _hubmanager.reg_hub(_hub_call_dbproxy_module.current_ch.Value, hub_name);
            rsp.rsp();
        }

		public void create_persisted_object(string db, string collection, byte[] object_info)
		{
            log.log.trace("begin create_persisted_object");

            hubproxy _hubproxy = dbproxy._hubmanager.get_hub(_hub_call_dbproxy_module.current_ch.Value);
            if (_hubproxy == null)
            {
                log.log.err("hubproxy is null");
                return;
            }
            var rsp = (abelkhan.hub_call_dbproxy_create_persisted_object_rsp)_hub_call_dbproxy_module.rsp;

            dbproxy._dbevent.push_create_event(new create_event(_hubproxy, db, collection, object_info, rsp));

            log.log.trace("end create_persisted_object");
        }

		public void updata_persisted_object(string db, string collection, byte[] query_data, byte[] object_data, bool upsert)
		{
            log.log.trace("begin updata_persisted_object");

            hubproxy _hubproxy = dbproxy._hubmanager.get_hub(_hub_call_dbproxy_module.current_ch.Value);
            if (_hubproxy == null)
            {
                log.log.err("hubproxy is null");
                return;
            }
            var rsp = (abelkhan.hub_call_dbproxy_updata_persisted_object_rsp)_hub_call_dbproxy_module.rsp;

            dbproxy._dbevent.push_updata_event(new update_event(_hubproxy, db, collection, query_data, object_data, upsert, rsp));

            log.log.trace("end updata_persisted_object");
        }

        public void find_and_modify(string db, string collection, byte[] query_data, byte[] object_data, bool is_new, bool upsert)
        {
            log.log.trace("begin find_and_modify");

            hubproxy _hubproxy = dbproxy._hubmanager.get_hub(_hub_call_dbproxy_module.current_ch.Value);
            if (_hubproxy == null)
            {
                log.log.err("hubproxy is null");
                return;
            }
            var rsp = (abelkhan.hub_call_dbproxy_find_and_modify_rsp)_hub_call_dbproxy_module.rsp;

            dbproxy._dbevent.push_find_and_modify_event(new find_and_modify_event(_hubproxy, db, collection, query_data, object_data, is_new, upsert, rsp));

            log.log.trace("end find_and_modify");
        }

        public void remove_object(string db, string collection, byte[] query_data)
        {
            log.log.trace("begin remove_object");

            hubproxy _hubproxy = dbproxy._hubmanager.get_hub(_hub_call_dbproxy_module.current_ch.Value);
            if (_hubproxy == null)
            {
                log.log.err("hubproxy is null");
                return;
            }
            var rsp = (abelkhan.hub_call_dbproxy_remove_object_rsp)_hub_call_dbproxy_module.rsp;

            dbproxy._dbevent.push_remove_event(new remove_event(_hubproxy, db, collection, query_data, rsp));

            log.log.trace("end remove_object");
        }

        public void get_object_count(string db, string collection, byte[] query_data)
        {
            log.log.trace("begin get_object_info");

            hubproxy _hubproxy = dbproxy._hubmanager.get_hub(_hub_call_dbproxy_module.current_ch.Value);
            if (_hubproxy == null)
            {
                log.log.err("hubproxy is null");
                return;
            }
            var rsp = (abelkhan.hub_call_dbproxy_get_object_count_rsp)_hub_call_dbproxy_module.rsp;

            dbproxy._dbevent.push_count_event(new count_event(_hubproxy, db, collection, query_data, rsp));

            log.log.trace("end get_object_info");
        }

		public void get_object_info(string db, string collection, byte[] query_data, int _skip, int _limit, string _sort, bool _Ascending_, string callbackid)
        {
            log.log.trace("begin get_object_info");

            hubproxy _hubproxy = dbproxy._hubmanager.get_hub(_hub_call_dbproxy_module.current_ch.Value);
            if (_hubproxy == null)
            {
                log.log.err("hubproxy is null");
                return;
            }

            dbproxy._dbevent.push_find_event(new find_event(_hubproxy, db, collection, query_data, _skip, _limit, _sort, _Ascending_, callbackid));

            log.log.trace("end get_object_info");
        }
	}
}

