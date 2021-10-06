using MsgPack.Serialization;
using System;
using System.Collections;
using System.IO;

namespace dbproxy
{
	public class hubproxy
	{
		public abelkhan.Ichannel _ch;
		public hubproxy(abelkhan.Ichannel ch)
		{
			_serializer = MessagePackSerializer.Get<ArrayList>();

			_ch = ch;
			_caller = new abelkhan.dbproxy_call_hub_caller(ch, abelkhan.modulemng_handle._modulemng);
		}

		public void ack_get_object_info(string callbackid, ArrayList object_info)
		{
			using (var stream = new MemoryStream()) {
				_serializer.Pack(stream, object_info);
				stream.Position = 0;
				_caller.ack_get_object_info(callbackid, stream.ToArray());
			}
		}

		public void ack_get_object_info_end(string callbackid)
		{
			_caller.ack_get_object_info_end(callbackid);
		}

        private abelkhan.dbproxy_call_hub_caller _caller;
		private MessagePackSerializer<ArrayList> _serializer;
	}
}

