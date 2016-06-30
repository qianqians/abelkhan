using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MsgPack;
using MsgPack.Serialization;

namespace test_csharp_msgpack
{
	public class test_msgpack
	{
		public static void Main()
		{
			var serializer = SerializationContext.Default.GetSerializer<ArrayList>();

			ArrayList list = new ArrayList();

			list.Add(1);
			list.Add("str");

			MemoryStream stream = new MemoryStream();
			serializer.Pack(stream, list);
			stream.Flush();
			stream.Position = 0;
			// Unpack from stream.
			ArrayList unpackedObject = serializer.Unpack(stream);

			foreach (var item in unpackedObject)
			{
				Console.WriteLine("connected server completed" + item);
			}


		}


	}
}

