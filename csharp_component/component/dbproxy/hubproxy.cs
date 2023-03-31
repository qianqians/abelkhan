using abelkhan;
using MsgPack.Serialization;
using System;
using System.Collections;
using System.IO;

namespace dbproxy
{
	public class hubproxy
	{
		public readonly abelkhan.Ichannel _ch;
		public hubproxy(abelkhan.Ichannel ch)
		{
			_serializer = MessagePackSerializer.Get<ArrayList>();

			_ch = ch;
			_caller = new abelkhan.dbproxy_call_hub_caller(ch, abelkhan.modulemng_handle._modulemng);
		}

		public void ack_get_object_info(string callbackid, MongoDB.Bson.BsonDocument object_info)
		{
			using var stream = MemoryStreamPool.mstMgr.GetStream();
            var write = new MongoDB.Bson.IO.BsonBinaryWriter(stream);
            MongoDB.Bson.Serialization.BsonSerializer.Serialize(write, object_info);
            stream.Position = 0;

            _caller.ack_get_object_info(callbackid, stream.ToArray());
        }

		public void ack_get_object_info_end(string callbackid)
		{
			_caller.ack_get_object_info_end(callbackid);
		}

        private readonly abelkhan.dbproxy_call_hub_caller _caller;
		private readonly MessagePackSerializer<ArrayList> _serializer;
	}
}

