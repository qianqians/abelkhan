using System;
using System.Collections.Generic;
using System.Threading;

namespace test_csharp_service
{
	public class testserver
	{
		public testserver()
		{
			_service_list = new List<service.service>();

			module.test _module = new module.test();

			_testmoduleachieve = new testmoduleachieve();
			_module.ontest_func += _testmoduleachieve.test_func;
			
			_process = new juggle.process();
			_process.reg_module(_module);

			service.acceptnetworkservice _service = new service.acceptnetworkservice("127.0.0.1", 1234, _process);
			_service_list.Add(_service);

			service.juggleservice _juggleservice = new service.juggleservice();
			_juggleservice.add_process(_process);
			_service_list.Add(_juggleservice);
		}

		public static void Main()
		{
			var _server = new testserver();

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

				foreach (var item in _server._service_list)
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
		private testmoduleachieve _testmoduleachieve;
	}
}

