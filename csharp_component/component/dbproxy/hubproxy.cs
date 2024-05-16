using Abelkhan;
using MsgPack.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DBProxy
{
	public class HubProxy
	{
		public readonly Abelkhan.Ichannel _ch;
		public HubProxy(Abelkhan.Ichannel ch)
		{
			_ch = ch;
			_caller = new Abelkhan.dbproxy_call_hub_caller(ch, Abelkhan.ModuleMgrHandle._modulemng);
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

        private readonly Abelkhan.dbproxy_call_hub_caller _caller;
	}
}

