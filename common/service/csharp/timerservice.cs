using System;
using System.Collections;
using System.Collections.Generic;

namespace service
{
	public class timerservice : service
	{
		public delegate void tickHandle(Int64 tick);
		public delegate void timeHandle(DateTime time);
		
		public timerservice()
		{
			tickHandledict = new Dictionary<Int64, tickHandle>();
			timeHandledict = new Dictionary<DateTime, timeHandle>();
		}

		public void poll(Int64 tick)
		{
			{
				List<Int64> list = new List<Int64>();

				foreach (var item in tickHandledict)
				{
					if (item.Key >= tick)
					{
						list.Add(item.Key);

						item.Value(tick);
					}
				}

				foreach (var item in list)
				{
					tickHandledict.Remove(item);
				}
			}

			{
				List<DateTime> list = new List<DateTime>();

				DateTime t = DateTime.Now;
				foreach (var item in timeHandledict)
				{
					if (item.Key.Year == t.Year && item.Key.Month == t.Month && item.Key.Day == t.Day)
					{
						list.Add(item.Key);

						item.Value(t);
					}
				}

				foreach (var item in list)
				{
					timeHandledict.Remove(item);
				}
			}
		}

		public void addticktime(Int64 process, tickHandle handle)
		{
			tickHandledict.Add(process, handle);
		}

		public void adddatetime(DateTime process, timeHandle handle)
		{
			timeHandledict.Add(process, handle);
		}

		private Dictionary<Int64, tickHandle> tickHandledict;
		private Dictionary<DateTime, timeHandle> timeHandledict;
	}
}

