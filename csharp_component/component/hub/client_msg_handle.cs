﻿using abelkhan;
using System;
using System.Collections;
using System.IO;

namespace hub
{
	public class client_msg_handle
	{
		private readonly abelkhan.client_call_hub_module _client_call_hub_module;
		private readonly MsgPack.Serialization.MessagePackSerializer<ArrayList> _serialization;

        public client_msg_handle()
		{
            _serialization = MsgPack.Serialization.MessagePackSerializer.Get<ArrayList>();

            _client_call_hub_module = new abelkhan.client_call_hub_module(abelkhan.modulemng_handle._modulemng);
			_client_call_hub_module.on_connect_hub += connect_hub;
			_client_call_hub_module.on_heartbeats += heartbeats;
			_client_call_hub_module.on_call_hub += call_hub;
		}

		public void connect_hub(string client_cuuid)
        {
			Hub._gates.direct_client_connect(client_cuuid, _client_call_hub_module.current_ch.Value);
        }

		public void heartbeats()
        {
			var rsp = (abelkhan.client_call_hub_heartbeats_rsp)_client_call_hub_module.rsp.Value;
			if (Hub._gates.get_directproxy(_client_call_hub_module.current_ch.Value, out Directproxy _proxy))
            {
				if (_proxy._theory_timetmp == 0)
                {
					_proxy._theory_timetmp = service.Timerservice.Tick;
                }
                else
                {
					_proxy._theory_timetmp += 5000;
				}
				_proxy._timetmp = service.Timerservice.Tick;
			}
			rsp.rsp((ulong)service.Timerservice.Tick);
		}

		public Action<string> on_client_msg;
		public void call_hub(byte[] arpc_rgv)
		{
			if (Hub._gates.get_directproxy(_client_call_hub_module.current_ch.Value, out Directproxy _proxy))
			{
				try
				{
					using var st = MemoryStreamPool.mstMgr.GetStream();
                    st.Write(arpc_rgv);
                    st.Position = 0;

                    var _event = _serialization.Unpack(st);

                    var func = ((MsgPack.MessagePackObject)_event[0]).AsString();
                    var argvs = ((MsgPack.MessagePackObject)_event[1]).AsList();

                    Hub._gates.current_client_uuid = _proxy._cuuid;
                    Hub._modules.process_module_mothed(func, argvs);
                    on_client_msg?.Invoke(_proxy._cuuid);
                    Hub._gates.current_client_uuid = "";
                }
				catch (System.Exception e)
				{
					log.Log.err("call_hub exception:{0}", e);
					Hub._gates.direct_client_exception(_client_call_hub_module.current_ch.Value);
				}
			}
		}
	}
}

