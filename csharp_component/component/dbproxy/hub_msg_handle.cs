﻿using Abelkhan;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace DBProxy
{
	public class hub_msg_handle
    {
        private CloseHandle _closehandle;
        private HubManager _hubmanager;
        private Abelkhan.hub_call_dbproxy_module _hub_call_dbproxy_module;

        public hub_msg_handle(HubManager _hubmanager_, CloseHandle _closehandle_)
		{
			_hubmanager = _hubmanager_;
            _closehandle = _closehandle_;

            _hub_call_dbproxy_module = new Abelkhan.hub_call_dbproxy_module(Abelkhan.ModuleMgrHandle._modulemng);
            _hub_call_dbproxy_module.on_reg_hub += reg_hub;
            _hub_call_dbproxy_module.on_get_guid += get_guid;
            _hub_call_dbproxy_module.on_create_persisted_object += create_persisted_object;
            _hub_call_dbproxy_module.on_updata_persisted_object += updata_persisted_object;
            _hub_call_dbproxy_module.on_find_and_modify += find_and_modify;
            _hub_call_dbproxy_module.on_remove_object += remove_object;
            _hub_call_dbproxy_module.on_get_object_count += get_object_count;
            _hub_call_dbproxy_module.on_get_object_info += get_object_info;
        }

        public void reg_hub(string hub_name)
		{
            Log.Log.trace("hub {0} connected", hub_name);
                
            var rsp = (Abelkhan.hub_call_dbproxy_reg_hub_rsp)_hub_call_dbproxy_module.rsp.Value;
            HubProxy _hubproxy = _hubmanager.reg_hub(_hub_call_dbproxy_module.current_ch.Value, hub_name);
            rsp.rsp();
        }

        private async void get_guid(string db, string collection)
        {
            Log.Log.trace("begin get_guid!");

            var rsp = (Abelkhan.hub_call_dbproxy_get_guid_rsp)_hub_call_dbproxy_module.rsp.Value;
            try
            {
                var guid = await DBProxy._mongodbproxy.get_guid(db, collection);
                if (guid > 0)
                {
                    rsp.rsp(guid);
                }
                else
                {
                    rsp.err();
                }
            }
            catch (System.Exception ex)
            {
                Log.Log.err("ex:{0}", ex);
                rsp.err();
            }


            Log.Log.trace("end get_guid");
        }

        public async void create_persisted_object(string db, string collection, byte[] object_info)
		{
            Log.Log.trace("begin create_persisted_object");

            var rsp = (Abelkhan.hub_call_dbproxy_create_persisted_object_rsp)_hub_call_dbproxy_module.rsp.Value;
            try
            {
                var is_create_sucess = await DBProxy._mongodbproxy.save(db, collection, object_info);
                if (is_create_sucess)
                {
                    rsp.rsp();
                }
                else
                {
                    rsp.err();
                }
            }
            catch (System.Exception ex)
            {
                Log.Log.err("ex:{0}", ex);
                rsp.err();
            }

            Log.Log.trace("end create_persisted_object");
        }

		public async void updata_persisted_object(string db, string collection, byte[] query_data, byte[] object_data, bool upsert)
		{
            Log.Log.trace("begin updata_persisted_object");

            var rsp = (Abelkhan.hub_call_dbproxy_updata_persisted_object_rsp)_hub_call_dbproxy_module.rsp.Value;
            try
            {
                var is_update_sucessed = await DBProxy._mongodbproxy.update(db, collection, query_data, object_data, upsert);
                if (is_update_sucessed)
                {
                    rsp.rsp();
                }
                else
                {
                    rsp.err();
                }
            }
            catch (System.Exception ex)
            {
                Log.Log.err("ex:{0}", ex);
                rsp.err();
            }

            Log.Log.trace("end updata_persisted_object");
        }

        public async void find_and_modify(string db, string collection, byte[] query_data, byte[] object_data, bool is_new, bool upsert)
        {
            Log.Log.trace("begin find_and_modify");

            var rsp = (Abelkhan.hub_call_dbproxy_find_and_modify_rsp)_hub_call_dbproxy_module.rsp.Value;
            try
            {
                var obj = await DBProxy._mongodbproxy.find_and_modify(db, collection, query_data, object_data, is_new, upsert);
                if (obj != null)
                {
                    using var st = MemoryStreamPool.mstMgr.GetStream();
                    var write = new MongoDB.Bson.IO.BsonBinaryWriter(st);
                    MongoDB.Bson.Serialization.BsonSerializer.Serialize(write, obj);
                    st.Position = 0;

                    rsp.rsp(st.ToArray());
                }
                else
                {
                    rsp.err();
                }
            }
            catch (System.Exception ex)
            {
                Log.Log.err("ex:{0}", ex);
                rsp.err();
            }

            Log.Log.trace("end find_and_modify");
        }

        public async void remove_object(string db, string collection, byte[] query_data)
        {
            Log.Log.trace("begin remove_object");

            var rsp = (Abelkhan.hub_call_dbproxy_remove_object_rsp)_hub_call_dbproxy_module.rsp.Value;
            try
            {
                var is_remove_sucessed = await DBProxy._mongodbproxy.remove(db, collection, query_data);
                if (is_remove_sucessed)
                {
                    rsp.rsp();
                }
                else
                {
                    rsp.err();
                }
            }
            catch (System.Exception ex)
            {
                Log.Log.err("ex:{0}", ex);
                rsp.err();
            }

            Log.Log.trace("end remove_object");
        }

        public async void get_object_count(string db, string collection, byte[] query_data)
        {
            Log.Log.trace("begin get_object_info");

            var rsp = (Abelkhan.hub_call_dbproxy_get_object_count_rsp)_hub_call_dbproxy_module.rsp.Value;
            try
            {
                var count = await DBProxy._mongodbproxy.count(db, collection, query_data);
                rsp.rsp((uint)count);
            }
            catch (System.Exception ex)
            {
                Log.Log.err("ex:{0}", ex);
                rsp.err();
            }

            Log.Log.trace("end get_object_info");
        }

		public async void get_object_info(string db, string collection, byte[] query_data, int _skip, int _limit, string _sort, bool _Ascending_, string callbackid)
        {
            Log.Log.trace("begin get_object_info");

            if (DBProxy._hubmanager.get_hub(_hub_call_dbproxy_module.current_ch.Value, out HubProxy _hubproxy))
            {
                try
                {
                    var _list = await DBProxy._mongodbproxy.find(db, collection, query_data, _limit, _skip, _sort, _Ascending_);

                    int count = 0;
                    if (_list.Count == 0)
                    {
                        _hubproxy.ack_get_object_info(callbackid, new MongoDB.Bson.BsonDocument { { "_list", _list } });
                    }
                    else
                    {
                        var _datalist = new MongoDB.Bson.BsonArray();
                        foreach (var data in _list)
                        {
                            _datalist.Add(data);

                            count++;

                            if (count >= 100)
                            {
                                _hubproxy.ack_get_object_info(callbackid, new MongoDB.Bson.BsonDocument { { "_list", _datalist } });

                                count = 0;
                                _datalist = new MongoDB.Bson.BsonArray();
                            }
                        }
                        if (count > 0 && count < 100)
                        {
                            _hubproxy.ack_get_object_info(callbackid, new MongoDB.Bson.BsonDocument { { "_list", _datalist } });
                        }
                    }
                    _hubproxy.ack_get_object_info_end(callbackid);
                }
                catch (System.Exception ex)
                {
                    Log.Log.err("ex:{0}", ex);
                }
            }
            else
            {
                Log.Log.err("hubproxy is null");
                _hubproxy.ack_get_object_info_end(callbackid);
            }

            Log.Log.trace("end get_object_info");
        }
	}
}

