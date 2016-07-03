using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace test_csharp_service_client
{
	public class test_client
	{
		public test_client()
		{
			_caller_set = new Hashtable();

			_service_list = new List<service.service>();

			_process = new juggle.process();

			service.connectnetworkservice _service = new service.connectnetworkservice(_process);
			service.channel ch = _service.connect("127.0.0.1", 1234);
			caller.test _caller = new caller.test(ch);
			_caller.test_func("test", 1);
			Console.WriteLine("test {0:D}", 1);
			_service_list.Add(_service);
		}

		public static void Main()
		{
			var _client = new test_client();

			Int64 tick = Environment.TickCount;
			Int64 tickcount = 0;

			while (true)
			{
				Int64 tmptick = (Environment.TickCount & UInt32.MaxValue);
				if (tmptick < tick)
				{
					tickcount += 1;
					tmptick = tmptick + tickcount * UInt32.MaxValue;
				}
				tick = tmptick;

				foreach (var item in _client._service_list)
				{
					item.poll(tick);
				}

				tmptick = (Environment.TickCount & UInt32.MaxValue);
				if (tmptick < tick)
				{
					tickcount += 1;
					tmptick = tmptick + tickcount * UInt32.MaxValue;
				}
				Int64 ticktime = (tmptick - tick);
				tick = tmptick;

				if (ticktime < 100)
				{
					Thread.Sleep(15);
				}
			}
		}
	

		private List<service.service> _service_list;
		private juggle.process _process;
		private Hashtable _caller_set;
	}
}

