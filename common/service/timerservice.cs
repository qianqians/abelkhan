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
            tickHandledict = new SortedDictionary<Int64, List<tickHandle> >();
			timeHandledict = new Dictionary<DateTime, List<timeHandle> >();
            addtickHandle = new Dictionary<Int64, List<tickHandle>>();
            addtimeHandle = new Dictionary<DateTime, List<timeHandle>>();
        }

		public void poll(Int64 tick)
		{
            foreach (var item in addtickHandle)
            {
                if (!tickHandledict.ContainsKey(item.Key))
                {
                    tickHandledict.Add(item.Key, new List<tickHandle>());
                }
                foreach (var t in item.Value)
                {
                    tickHandledict[item.Key].Add(t);
                }
            }
            foreach (var item in addtimeHandle)
            {
                if (!timeHandledict.ContainsKey(item.Key))
                {
                    timeHandledict.Add(item.Key, new List<timeHandle>());
                }
                foreach (var t in item.Value)
                {
                    timeHandledict[item.Key].Add(t);
                }
            }

            Tick = tick;

            try
            {
				List<Int64> list = new List<Int64>();
                
                foreach (var item in tickHandledict)
				{
					if (item.Key <= tick)
					{
						list.Add(item.Key);

                        foreach (var handle in item.Value)
                        {
                            handle(tick);
                        }
					}
                    else
                    {
                        break;
                    }
				}

				foreach (var item in list)
				{
					tickHandledict.Remove(item);
				}
			}
            catch(System.Exception e)
            {
                Console.WriteLine("System.Exceptio{0}", e);
            }

            try
			{
				List<DateTime> list = new List<DateTime>();

				DateTime t = DateTime.Now;
				foreach (var item in timeHandledict)
				{
					if (item.Key.Year == t.Year && item.Key.Month == t.Month && item.Key.Day == t.Day)
					{
						list.Add(item.Key);

                        foreach (var handle in item.Value)
                        {
                            handle(t);
                        }
                    }
				}

				foreach (var item in list)
				{
					timeHandledict.Remove(item);
				}
			}
            catch (System.Exception e)
            {
                Console.WriteLine("System.Exceptio{0}", e);
            }
        }

		public void addticktime(Int64 process, tickHandle handle)
		{
            if (!addtickHandle.ContainsKey(process))
            {
                addtickHandle.Add(process, new List<tickHandle>());
            }
            addtickHandle[process].Add( handle);
		}

		public void adddatetime(DateTime process, timeHandle handle)
		{
            if (!addtimeHandle.ContainsKey(process))
            {
                addtimeHandle.Add(process, new List<timeHandle>());
            }
            addtimeHandle[process].Add(handle);
        }

        public Int64 Tick;

		private SortedDictionary<Int64, List<tickHandle> > tickHandledict;
        private Dictionary<Int64, List<tickHandle>> addtickHandle;
        private Dictionary<DateTime, List<timeHandle> > timeHandledict;
        private Dictionary<DateTime, List<timeHandle> > addtimeHandle;
    }
}

