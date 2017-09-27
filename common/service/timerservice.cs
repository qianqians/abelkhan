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
            delimpldict = new Dictionary<string, HandleImpl>();

            tickHandledict = new SortedDictionary<Int64, List<HandleImpl> >();
            addtickHandle = new Dictionary<Int64, List<HandleImpl> >();

            timeHandledict = new Dictionary<week_day_time, List<HandleImpl> >();
            addtimeHandle = new Dictionary<week_day_time, List<HandleImpl> >();

            monthtimeHandledict = new Dictionary<month_day_time, List<HandleImpl> >();
            addmonthtimeHandle = new Dictionary<month_day_time, List<HandleImpl> >();

            loopdaytimeHandledict = new Dictionary<day_time, List<HandleImpl> >();
            adddaytimeHandle = new Dictionary<day_time, List<HandleImpl> >();
            loopdaytimeHandle = new Dictionary<day_time, List<HandleImpl> >();

            loopweekdaytimeHandledict = new Dictionary<week_day_time, List<HandleImpl> >();
            addloopweekdaytimeHandle = new Dictionary<week_day_time, List<HandleImpl> >();
            loopweekdaytimeHandle = new Dictionary<week_day_time, List<HandleImpl> >();

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
                    tickHandledict.Add(item.Key, new List<HandleImpl>());
                }
                tickHandledict[item.Key].AddRange(item.Value);
            }
            addtickHandle.Clear();

            foreach (var item in addtimeHandle)
            {
                if (!timeHandledict.ContainsKey(item.Key))
                {
                    timeHandledict.Add(item.Key, new List<HandleImpl>());
                }
                timeHandledict[item.Key].AddRange(item.Value);
            }
            addtimeHandle.Clear();

            foreach (var item in addmonthtimeHandle)
            {
                if (!monthtimeHandledict.ContainsKey(item.Key))
                {
                    monthtimeHandledict.Add(item.Key, new List<HandleImpl>());
                }
                monthtimeHandledict[item.Key].AddRange(item.Value);
            }
            addmonthtimeHandle.Clear();

            foreach(var item in adddaytimeHandle)
            {
                if (!loopdaytimeHandledict.ContainsKey(item.Key))
                {
                    loopdaytimeHandledict.Add(item.Key, new List<HandleImpl>());
                }
                loopdaytimeHandledict[item.Key].AddRange(item.Value);
            }
            adddaytimeHandle.Clear();

            foreach (var item in addloopweekdaytimeHandle)
            {
                if (!loopweekdaytimeHandledict.ContainsKey(item.Key))
                {
                    loopweekdaytimeHandledict.Add(item.Key, new List<HandleImpl>());
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

                        foreach (var impl in item.Value)
                        {
                            if (impl.is_del)
                            {
                                continue;
                            }

                            var handle = impl.handle as tickHandle;

                            try
                            {
                                handle(Tick);
                            }
                            catch (System.Exception e)
                            {
                                log.log.error(new System.Diagnostics.StackFrame(true), Tick, "System.Exceptio{0}", e);
                            }
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

                        foreach (var impl in item.Value)
                        {
                            if (impl.is_del)
                            {
                                continue;
                            }

                            var handle = impl.handle as timeHandle;

                            try
                            {
                                handle(t);
                            }
                            catch (System.Exception e)
                            {
                                log.log.error(new System.Diagnostics.StackFrame(true), Tick, "System.Exceptio{0}", e);
                            }
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

                        foreach (var impl in item.Value)
                        {
                            if (impl.is_del)
                            {
                                continue;
                            }

                            var handle = impl.handle as timeHandle;

                            try
                            {
                                handle(t);
                            }
                            catch (System.Exception e)
                            {
                                log.log.error(new System.Diagnostics.StackFrame(true), Tick, "System.Exceptio{0}", e);
                            }
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
                DateTime t = DateTime.Now;
                if (t.Hour == 0 && t.Minute == 0 && t.Second == 0 &&
                    (Tick - loopdaytick) >= 24 * 60 * 60 * 1000)
                {
                    foreach (var item in loopdaytimeHandle)
                    {
                        if (!loopdaytimeHandledict.ContainsKey(item.Key))
                        {
                            loopdaytimeHandledict.Add(item.Key, new List<HandleImpl>());
                        }
                        loopdaytimeHandledict[item.Key].AddRange(item.Value);
                    }
                    loopdaytimeHandle.Clear();

                    loopdaytick = Tick;
                }

                List<day_time> list = new List<day_time>();
                foreach (var item in loopdaytimeHandledict)
                {
                    if (item.Key.hour == t.Hour && item.Key.minute == t.Minute && item.Key.second == t.Second)
                    {
                        list.Add(item.Key);

                        foreach (var impl in item.Value)
                        {
                            if (impl.is_del)
                            {
                                continue;
                            }

                            var handle = impl.handle as timeHandle;

                            try
                            {
                                handle(t);
                            }
                            catch (System.Exception e)
                            {
                                log.log.error(new System.Diagnostics.StackFrame(true), Tick, "System.Exceptio{0}", e);
                            }
                        }
                    }
                }

                foreach (var item in list)
                {
                    if (!loopdaytimeHandle.ContainsKey(item))
                    {
                        loopdaytimeHandle.Add(item, new List<HandleImpl>());
                    }
                    foreach (var impl in loopdaytimeHandledict[item])
                    {
                        if (impl.is_del)
                        {
                            continue;
                        }

                        loopdaytimeHandle[item].Add(impl);
                    }
                    loopdaytimeHandledict.Remove(item);
                }
            }
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), Tick, "System.Exceptio{0}", e);
            }

            try
            {
                DateTime t = DateTime.Now;
                if (t.DayOfWeek == DayOfWeek.Sunday && t.Hour == 0 && t.Minute == 0 && t.Second == 0 &&
                    (Tick - loopweekdaytick) >= 7 * 24 * 60 * 60 * 1000)
                {
                    foreach (var item in loopweekdaytimeHandle)
                    {
                        if (!loopweekdaytimeHandledict.ContainsKey(item.Key))
                        {
                            loopweekdaytimeHandledict.Add(item.Key, new List<HandleImpl>());
                        }
                        loopweekdaytimeHandledict[item.Key].AddRange(item.Value);
                    }
                    loopweekdaytimeHandle.Clear();

                    loopweekdaytick = Tick;
                }

                List<week_day_time> list = new List<week_day_time>();
                foreach (var item in loopweekdaytimeHandledict)
                {
                    if (item.Key.day == t.DayOfWeek && item.Key.hour == t.Hour && item.Key.minute == t.Minute && item.Key.second == t.Second)
                    {
                        list.Add(item.Key);

                        foreach (var impl in item.Value)
                        {
                            if (impl.is_del)
                            {
                                continue;
                            }

                            var handle = impl.handle as timeHandle;

                            try
                            {
                                handle(t);
                            }
                            catch (System.Exception e)
                            {
                                log.log.error(new System.Diagnostics.StackFrame(true), Tick, "System.Exceptio{0}", e);
                            }
                        }
                    }
                }

                foreach (var item in list)
                {
                    if (!loopweekdaytimeHandle.ContainsKey(item))
                    {
                        loopweekdaytimeHandle.Add(item, new List<HandleImpl>());
                    }
                    foreach (var impl in loopweekdaytimeHandledict[item])
                    {
                        if (impl.is_del)
                        {
                            continue;
                        }

                        loopweekdaytimeHandle[item].Add(impl);
                    }
                    loopweekdaytimeHandledict.Remove(item);
                }
            }
            catch (System.Exception e)
            {
                log.log.error(new System.Diagnostics.StackFrame(true), Tick, "System.Exceptio{0}", e);
            }

            return Tick;
        }

		public String addticktime(Int64 process, tickHandle handle)
		{
            process += Tick;
            if (!addtickHandle.ContainsKey(process))
            {
                addtickHandle.Add(process, new List<HandleImpl>());
            }

            var impl = new HandleImpl(handle);
            var uuid = System.Guid.NewGuid().ToString();
            addtickHandle[process].Add(impl);
            delimpldict.Add(uuid, impl);

            return uuid;
		}

        public String addweekdaytime(System.DayOfWeek day, int hour, int minute, int second, timeHandle handle)
        {
            week_day_time key = new week_day_time();
            key.day = day;
            key.hour = hour;
            key.minute = minute;
            key.second = second;
            if (!addtimeHandle.ContainsKey(key))
            {
                addtimeHandle.Add(key, new List<HandleImpl>());
            }

            var impl = new HandleImpl(handle);
            var uuid = System.Guid.NewGuid().ToString();
            addtimeHandle[key].Add(impl);
            delimpldict.Add(uuid, impl);

            return uuid;
        }

        public String addmonthdaytime(int month, int day, int hour, int minute, int second, timeHandle handle)
        {
            month_day_time key = new month_day_time();
            key.month = month;
            key.day = day;
            key.hour = hour;
            key.minute = minute;
            key.second = second;
            if (!addmonthtimeHandle.ContainsKey(key))
            {
                addmonthtimeHandle.Add(key, new List<HandleImpl>());
            }

            var impl = new HandleImpl(handle);
            var uuid = System.Guid.NewGuid().ToString();
            addmonthtimeHandle[key].Add(impl);
            delimpldict.Add(uuid, impl);

            return uuid;
        }

        public String addloopdaytime(int hour, int minute, int second, timeHandle handle)
        {
            day_time key = new day_time();
            key.hour = hour;
            key.minute = minute;
            key.second = second;
            if (!adddaytimeHandle.ContainsKey(key))
            {
                adddaytimeHandle.Add(key, new List<HandleImpl>());
            }

            var impl = new HandleImpl(handle);
            var uuid = System.Guid.NewGuid().ToString();
            adddaytimeHandle[key].Add(impl);
            delimpldict.Add(uuid, impl);

            return uuid;
        }

        public String addloopweekdaytime(System.DayOfWeek day, int hour, int minute, int second, timeHandle handle)
        {
            week_day_time key = new week_day_time();
            key.day = day;
            key.hour = hour;
            key.minute = minute;
            key.second = second;
            if (!addloopweekdaytimeHandle.ContainsKey(key))
            {
                addloopweekdaytimeHandle.Add(key, new List<HandleImpl>());
            }

            var impl = new HandleImpl(handle);
            var uuid = System.Guid.NewGuid().ToString();
            addloopweekdaytimeHandle[key].Add(impl);
            delimpldict.Add(uuid, impl);

            return uuid;
        }

        public void deltimer(String uuid)
        {
            if (delimpldict.ContainsKey(uuid))
            {
                delimpldict[uuid].is_del = true;
            }
        }

        static public Int64 Tick;

        class HandleImpl
        {
            public HandleImpl(tickHandle handle_)
            {
                is_del = false;
                handle = handle_;
            }

            public HandleImpl(timeHandle handle_)
            {
                is_del = false; 
                handle = handle_;
            }

            public bool is_del;
            public object handle;
        }

        private Dictionary<String, HandleImpl> delimpldict;

		private SortedDictionary<Int64, List<HandleImpl> > tickHandledict;
        private Dictionary<Int64, List<HandleImpl>> addtickHandle;

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
        private Dictionary<month_day_time, List<HandleImpl> > monthtimeHandledict;
        private Dictionary<month_day_time, List<HandleImpl> > addmonthtimeHandle;

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
        private Dictionary<week_day_time, List<HandleImpl> > timeHandledict;
        private Dictionary<week_day_time, List<HandleImpl> > addtimeHandle;

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
        private Dictionary<day_time, List<HandleImpl> > loopdaytimeHandledict;
        private Dictionary<day_time, List<HandleImpl> > adddaytimeHandle;
        private Dictionary<day_time, List<HandleImpl> > loopdaytimeHandle;
        private Int64 loopdaytick;

        private Dictionary<week_day_time, List<HandleImpl> > loopweekdaytimeHandledict;
        private Dictionary<week_day_time, List<HandleImpl> > addloopweekdaytimeHandle;
        private Dictionary<week_day_time, List<HandleImpl> > loopweekdaytimeHandle;
        private Int64 loopweekdaytick;
    }
}

