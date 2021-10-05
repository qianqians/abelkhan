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

	public class imodule
	{
		private Dictionary<string, Action<ArrayList> > cbs = new Dictionary<string, Action<ArrayList> >();
		private MessagePackSerializer<ArrayList> serializer = MessagePackSerializer.Get<ArrayList>();

		public void reg_cb(string cb_name, Action<ArrayList> cb)
		{
			cbs.Add(cb_name, cb);
		}

		public void invoke(string cb_name, byte[] data)
        {
			if (cbs.TryGetValue(cb_name, out Action<ArrayList> cb))
			{
				var st = new MemoryStream();
				st.Write(data);
				st.Position = 0;
				
				var InArray = serializer.Unpack(st);

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

