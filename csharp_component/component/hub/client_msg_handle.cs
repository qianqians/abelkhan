using System;
using System.Collections;
using System.IO;

namespace hub
{
	public class client_msg_handle
	{
		private abelkhan.client_call_hub_module _client_call_hub_module;

		public client_msg_handle()
		{
			_client_call_hub_module = new abelkhan.client_call_hub_module(abelkhan.modulemng_handle._modulemng);
			_client_call_hub_module.on_connect_hub += connect_hub;
			_client_call_hub_module.on_heartbeats += heartbeats;
			_client_call_hub_module.on_call_hub += call_hub;
		}

		public void connect_hub(string client_cuuid)
        {
			hub._gates.direct_client_connect(client_cuuid, _client_call_hub_module.current_ch);
        }

		public void heartbeats()
        {
			var rsp = (abelkhan.client_call_hub_heartbeats_rsp)_client_call_hub_module.rsp;
			var _proxy = hub._gates.get_directproxy(_client_call_hub_module.current_ch);
			if (_proxy != null)
            {
				if (_proxy._theory_timetmp == 0)
                {
					_proxy._theory_timetmp = service.timerservice.Tick;
                }
                else
                {
					_proxy._theory_timetmp += 5 * 1000;
				}
				_proxy._timetmp = service.timerservice.Tick;
			}
			rsp.rsp((ulong)service.timerservice.Tick);
		}

		public void call_hub(byte[] arpc_rgv)
		{
			var _proxy = hub._gates.get_directproxy(_client_call_hub_module.current_ch);
			if (_proxy != null)
			{
				try
				{
					using (var st = new MemoryStream())
					{
						st.Write(arpc_rgv);
						st.Position = 0;

						var _serialization = MsgPack.Serialization.MessagePackSerializer.Get<ArrayList>();
						var _event = _serialization.Unpack(st);

						var module = ((MsgPack.MessagePackObject)_event[0]).AsString();
						var func = ((MsgPack.MessagePackObject)_event[1]).AsString();
						var argvs = ((MsgPack.MessagePackObject)_event[2]).AsList();

						hub._gates.current_client_uuid = _proxy._cuuid;
						hub._modules.process_module_mothed(module, func, argvs);
						hub._gates.current_client_uuid = "";
					}
				}
				catch (Exception e)
				{
					log.log.err("call_hub exception:{0}", e);
					hub._gates.direct_client_exception(_client_call_hub_module.current_ch);
				}
			}
		}
	}
}

