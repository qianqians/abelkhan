﻿using MsgPack.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace hub
{
	public class gateproxy
	{
		public abelkhan.Ichannel _ch;
		private abelkhan.hub_call_gate_caller _hub_call_gate_caller;

		public gateproxy(abelkhan.Ichannel ch)
		{
			_ch = ch;
			_hub_call_gate_caller = new abelkhan.hub_call_gate_caller(ch, abelkhan.modulemng_handle._modulemng);
		}

		public void reg_hub()
        {
            log.log.trace("begin connect gate server");

			_hub_call_gate_caller.reg_hub(hub.name, hub.type).callBack(() =>
			{
				log.log.trace("connect gate server sucessed");
			}, () =>
			{
				log.log.trace("connect gate server faild");
			}).timeout(5 * 1000, () =>
			{
				log.log.trace("connect gate server timeout");
			});
		}

        public void disconnect_client(String uuid)
        {
			_hub_call_gate_caller.disconnect_client(uuid);
        }

        public void forward_hub_call_client(String uuid, String module, String func, ArrayList argv)
        {
			var _serializer = MessagePackSerializer.Get<ArrayList>();
			using (var st = new MemoryStream())
            {
				ArrayList _event = new ArrayList();
				_event.Add(module);
				_event.Add(func);
				_event.Add(argv);
				_serializer.Pack(st, _event);
				_hub_call_gate_caller.forward_hub_call_client(uuid, st.ToArray());
			}

        }

        public void forward_hub_call_group_client(List<string> uuids, String module, String func, ArrayList argv)
		{
			var _serializer = MessagePackSerializer.Get<ArrayList>();
			using (var st = new MemoryStream())
			{
				ArrayList _event = new ArrayList();
				_event.Add(module);
				_event.Add(func);
				_event.Add(argv);
				_serializer.Pack(st, _event);
				_hub_call_gate_caller.forward_hub_call_group_client(uuids, st.ToArray());
			}
		}

		public void forward_hub_call_global_client(String module, String func, ArrayList argv)
		{
			var _serializer = MessagePackSerializer.Get<ArrayList>();
			using (var st = new MemoryStream())
			{
				ArrayList _event = new ArrayList();
				_event.Add(module);
				_event.Add(func);
				_event.Add(argv);
				_serializer.Pack(st, _event);
				_hub_call_gate_caller.forward_hub_call_global_client(st.ToArray());
			}
		}
	}
}

