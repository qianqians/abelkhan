using System;
using System.Collections;
using System.Collections.Generic;

namespace service
{
	public class timerservice
	{
		public delegate void tickHandle(Int64 tick);
		public delegate void timeHandle(DateTime time);
		
		public timerservice()
		{
            tickHandledict = new SortedDictionary<Int64, List<tickHandle> >();
            addtickHandle = new Dictionary<Int64, List<tickHandle> >();

            timeHandledict = new Dictionary<week_day_time, List<timeHandle> >();
            addtimeHandle = new Dictionary<week_day_time, List<timeHandle> >();

            monthtimeHandledict = new Dictionary<month_day_time, List<timeHandle> >();
            addmonthtimeHandle = new Dictionary<month_day_time, List<timeHandle> >();

            loopdaytimeHandledict = new Dictionary<day_time, List<timeHandle> >();
            adddaytimeHandle = new Dictionary<day_time, List<timeHandle> >();
            loopdaytimeHandle = new Dictionary<day_time, List<timeHandle> >();

            loopweekdaytimeHandledict = new Dictionary<week_day_time, List<timeHandle> >();
            addloopweekdaytimeHandle = new Dictionary<week_day_time, List<timeHandle> >();
            loopweekdaytimeHandle = new Dictionary<week_day_time, List<timeHandle> >();

            Tick = (Int64)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

            loopdaytick = 0;
            loopweekdaytick = 0;
        }

		public Int64 poll()
		{
            Tick = (Int64)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

            foreach (var item in addtickHandle)
            {
                if (!tickHandledict.ContainsKey(item.Key))
                {
                    tickHandledict.Add(item.Key, new List<tickHandle>());
                }
                tickHandledict[item.Key].AddRange(item.Value);
            }
            addtickHandle.Clear();

            foreach (var item in addtimeHandle)
            {
                if (!timeHandledict.ContainsKey(item.Key))
                {
                    timeHandledict.Add(item.Key, new List<timeHandle>());
                }
                timeHandledict[item.Key].AddRange(item.Value);
            }
            addtimeHandle.Clear();

            foreach (var item in addmonthtimeHandle)
            {
                if (!monthtimeHandledict.ContainsKey(item.Key))
                {
                    monthtimeHandledict.Add(item.Key, new List<timeHandle>());
                }
                monthtimeHandledict[item.Key].AddRange(item.Value);
            }
            addmonthtimeHandle.Clear();

            foreach(var item in adddaytimeHandle)
            {
                if (!loopdaytimeHandledict.ContainsKey(item.Key))
                {
                    loopdaytimeHandledict.Add(item.Key, new List<timeHandle>());
                }
                loopdaytimeHandledict[item.Key].AddRange(item.Value);
            }
            adddaytimeHandle.Clear();

            foreach (var item in addloopweekdaytimeHandle)
            {
                if (!loopweekdaytimeHandledict.ContainsKey(item.Key))
                {
                    loopweekdaytimeHandledict.Add(item.Key, new List<timeHandle>());
                }
                loopweekdaytimeHandledict[item.Key].AddRange(item.Value);
            }
            addloopweekdaytimeHandle.Clear();

            try
            {
				List<Int64> list = new List<Int64>();
                
                foreach (var item in tickHandledict)
				{
					if (item.Key <= Tick)
					{
						list.Add(item.Key);

                        foreach (var handle in item.Value)
                        {
                            handle(Tick);
                        }
					}
				}

				foreach (var item in list)
				{
					tickHandledict.Remove(item);
				}
			}
            catch(System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), Tick, "System.Exceptio{0}", e);
            }

            try
			{
				List<week_day_time> list = new List<week_day_time>();

				DateTime t = DateTime.Now;
				foreach (var item in timeHandledict)
				{
					if (item.Key.day == t.DayOfWeek && item.Key.hour == t.Hour && item.Key.minute == t.Minute && item.Key.second == t.Second)
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
                log.log.error(new System.Diagnostics.StackFrame(true), Tick, "System.Exceptio{0}", e);
            }

            try
            {
                List<month_day_time> list = new List<month_day_time>();

                DateTime t = DateTime.Now;
                foreach (var item in monthtimeHandledict)
                {
                    if (item.Key.month == t.Month && item.Key.day == t.Day && item.Key.hour == t.Hour && item.Key.minute == t.Minute && item.Key.second == t.Second)
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
                    monthtimeHandledict.Remove(item);
                }
            }
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), Tick, "System.Exceptio{0}", e);
            }

            try
            {
                List<day_time> list = new List<day_time>();

                DateTime t = DateTime.Now;
                foreach (var item in loopdaytimeHandledict)
                {
                    if (item.Key.hour == t.Hour && item.Key.minute == t.Minute && item.Key.second == t.Second)
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
                    if (!loopdaytimeHandle.ContainsKey(item))
                    {
                        loopdaytimeHandle.Add(item, new List<timeHandle>());
                    }
                    loopdaytimeHandle[item].AddRange(loopdaytimeHandledict[item]);
                    loopdaytimeHandledict.Remove(item);
                }

                if (t.Hour == 0 && t.Minute == 0 && t.Second == 0 &&
                    (Tick - loopdaytick) >= 24 * 60 * 60 * 1000 )
                {
                    foreach(var item in loopdaytimeHandle)
                    {
                        if (!loopdaytimeHandledict.ContainsKey(item.Key))
                        {
                            loopdaytimeHandledict.Add(item.Key, new List<timeHandle>());
                        }
                        loopdaytimeHandledict[item.Key].AddRange(item.Value);
                    }
                    loopdaytimeHandle.Clear();

                    loopdaytick = Tick;
                }
            }
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), Tick, "System.Exceptio{0}", e);
            }

            try
            {
                List<week_day_time> list = new List<week_day_time>();

                DateTime t = DateTime.Now;
                foreach (var item in loopweekdaytimeHandledict)
                {
                    if (item.Key.day == t.DayOfWeek && item.Key.hour == t.Hour && item.Key.minute == t.Minute && item.Key.second == t.Second)
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
                    if (!loopweekdaytimeHandle.ContainsKey(item))
                    {
                        loopweekdaytimeHandle.Add(item, new List<timeHandle>());
                    }
                    loopweekdaytimeHandle[item].AddRange(loopweekdaytimeHandledict[item]);
                    loopweekdaytimeHandledict.Remove(item);
                }

                if (t.DayOfWeek == DayOfWeek.Sunday && t.Hour == 0 && t.Minute == 0 && t.Second == 0 &&
                    (Tick - loopweekdaytick) >= 7 * 24 * 60 * 60 * 1000)
                {
                    foreach(var item in loopweekdaytimeHandle)
                    {
                        if (!loopweekdaytimeHandledict.ContainsKey(item.Key))
                        {
                            loopweekdaytimeHandledict.Add(item.Key, new List<timeHandle>());
                        }
                        loopweekdaytimeHandledict[item.Key].AddRange(item.Value);
                    }
                    loopweekdaytimeHandle.Clear();

                    loopweekdaytick = Tick;
                }
            }
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), Tick, "System.Exceptio{0}", e);
            }

            return Tick;
        }

		public void addticktime(Int64 process, tickHandle handle)
		{
            if (!addtickHandle.ContainsKey(process))
            {
                addtickHandle.Add(process, new List<tickHandle>());
            }
            addtickHandle[process].Add( handle);
		}

        public void addweekdaytime(System.DayOfWeek day, int hour, int minute, int second, timeHandle handle)
        {
            week_day_time key = new week_day_time();
            key.day = day;
            key.hour = hour;
            key.minute = minute;
            key.second = second;
            if (!addtimeHandle.ContainsKey(key))
            {
                addtimeHandle.Add(key, new List<timeHandle>());
            }
            addtimeHandle[key].Add(handle);
        }

        public void addmonthdaytime(int month, int day, int hour, int minute, int second, timeHandle handle)
        {
            month_day_time key = new month_day_time();
            key.month = month;
            key.day = day;
            key.hour = hour;
            key.minute = minute;
            key.second = second;
            if (!addmonthtimeHandle.ContainsKey(key))
            {
                addmonthtimeHandle.Add(key, new List<timeHandle>());
            }
            addmonthtimeHandle[key].Add(handle);
        }

        public void addloopdaytime(int hour, int minute, int second, timeHandle handle)
        {
            day_time key = new day_time();
            key.hour = hour;
            key.minute = minute;
            key.second = second;
            if (!adddaytimeHandle.ContainsKey(key))
            {
                adddaytimeHandle.Add(key, new List<timeHandle>());
            }
            adddaytimeHandle[key].Add(handle);
        }

        public void addloopweekdaytime(System.DayOfWeek day, int hour, int minute, int second, timeHandle handle)
        {
            week_day_time key = new week_day_time();
            key.day = day;
            key.hour = hour;
            key.minute = minute;
            key.second = second;
            if (!addloopweekdaytimeHandle.ContainsKey(key))
            {
                addloopweekdaytimeHandle.Add(key, new List<timeHandle>());
            }
            addloopweekdaytimeHandle[key].Add(handle);
        }

        static public Int64 Tick;

		private SortedDictionary<Int64, List<tickHandle> > tickHandledict;
        private Dictionary<Int64, List<tickHandle>> addtickHandle;

        struct month_day_time
        {
            public int month;
            public int day;
            public int hour;
            public int minute;
            public int second;

            public override int GetHashCode()
            {
                return (int)day * 24 * 3600 + hour * 3600 + minute * 60 + second;
            }

            public override bool Equals(object obj)
            {
                if (null == obj)
                {
                    return false;
                }
                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                month_day_time tmp = (month_day_time)obj;
                if (month == tmp.month &&
                    day == tmp.day &&
                    hour == tmp.hour &&
                    minute == tmp.minute &&
                    second == tmp.second)
                {
                    return true;
                }

                return false;
            }
        }
        private Dictionary<month_day_time, List<timeHandle> > monthtimeHandledict;
        private Dictionary<month_day_time, List<timeHandle> > addmonthtimeHandle;

        struct week_day_time
        {
            public System.DayOfWeek day;
            public int hour;
            public int minute;
            public int second;

            public override int GetHashCode()
            {
                return (int)day * 24 * 3600 + hour * 3600 + minute * 60 + second;
            }

            public override bool Equals(object obj)
            {
                if (null == obj)
                {
                    return false;
                }
                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                week_day_time tmp = (week_day_time)obj;
                if (day == tmp.day &&
                    hour == tmp.hour &&
                    minute == tmp.minute &&
                    second == tmp.second)
                {
                    return true;
                }

                return false;
            }
        }
        private Dictionary<week_day_time, List<timeHandle> > timeHandledict;
        private Dictionary<week_day_time, List<timeHandle> > addtimeHandle;

        struct day_time
        {
            public int hour;
            public int minute;
            public int second;

            public override int GetHashCode()
            {
                return (int)hour * 3600 + minute * 60 + second;
            }

            public override bool Equals(object obj)
            {
                if (null == obj)
                {
                    return false;
                }
                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                day_time tmp = (day_time)obj;
                if (hour == tmp.hour &&
                    minute == tmp.minute &&
                    second == tmp.second)
                {
                    return true;
                }

                return false;
            }
        }
        private Dictionary<day_time, List<timeHandle> > loopdaytimeHandledict;
        private Dictionary<day_time, List<timeHandle> > adddaytimeHandle;
        private Dictionary<day_time, List<timeHandle> > loopdaytimeHandle;
        private Int64 loopdaytick;

        private Dictionary<week_day_time, List<timeHandle> > loopweekdaytimeHandledict;
        private Dictionary<week_day_time, List<timeHandle> > addloopweekdaytimeHandle;
        private Dictionary<week_day_time, List<timeHandle> > loopweekdaytimeHandle;
        private Int64 loopweekdaytick;
    }
}

