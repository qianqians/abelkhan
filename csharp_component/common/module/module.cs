using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MsgPack.Serialization;

namespace common
{
	public class moduleException : System.Exception
	{
		public moduleException(string _err) : base(_err)
		{
		}
	}

	public class Response
	{
	}

	public class imodule
	{
		private Dictionary<string, Action<IList<MsgPack.MessagePackObject> > > cbs = new Dictionary<string, Action<IList<MsgPack.MessagePackObject> > >();
		private MessagePackSerializer<ArrayList> serializer = MessagePackSerializer.Get<ArrayList>();

		public Response rsp;

		public void reg_cb(string cb_name, Action<IList<MsgPack.MessagePackObject> > cb)
		{
			cbs.Add(cb_name, cb);
		}

		public void invoke(string cb_name, IList<MsgPack.MessagePackObject> InArray)
        {
			if (cbs.TryGetValue(cb_name, out Action<IList<MsgPack.MessagePackObject> > cb))
			{
				cb(InArray);
			}
			else
			{
				log.log.err("imodule.invoke unreg func name:{0}", cb_name);
				throw new moduleException(String.Format("imodule.invoke unreg func name:%s!", cb_name));
			}
		}
	}
}

