using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
		public imodule()
        {
		}

		public Response rsp = null;
	}
}

