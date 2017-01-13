using System;

namespace main
{
	public class helloworld : common.imodule
	{
		public helloworld()
		{
		}

		public void call_helloworld(String argv)
		{
			Console.WriteLine("call_helloworld " + argv);
		}

	}
}

