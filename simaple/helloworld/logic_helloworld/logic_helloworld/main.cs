using System;
using System.Threading;

namespace main
{
	public class main
	{
		public main(String[] args)
		{
			_logic = new logic.logic(args);
		}
		
		public static void Main(String[] args)
		{
			if (args.Length <= 0)
			{
				return;
			}

			main _main = new main(args);

			helloworld _helloworld = new helloworld();
			logic.logic.modules.add_module("helloworld", _helloworld);

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

				_main._logic.poll(tick);

				if (logic.logic.closeHandle.is_close)
				{
					Console.WriteLine("server closed, hub server " + logic.logic.uuid);
					break;
				}

				tmptick = (Environment.TickCount & UInt32.MaxValue);
				if (tmptick < tick)
				{
					tickcount += 1;
					tmptick = tmptick + tickcount * UInt32.MaxValue;
				}
				Int64 ticktime = (tmptick - tick);
				tick = tmptick;

				if (ticktime > 200)
				{
					logic.logic.is_busy = true;
				}
				else
				{
					logic.logic.is_busy = false;
				}

				if (ticktime < 50)
				{
					Thread.Sleep(15);
				}
			}
		}

		private logic.logic _logic;
	}
}

