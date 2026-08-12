// ReSharper disable MemberCanBePrivate.Global
namespace core;

public class TimerService
{
	public TimerService()
	{
        Tick = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

        _loopDayTick = 0;
        _loopWeekDayTick = 0;

        AddTickTime(33, PollDayTimeHandleImpl);
        AddTickTime(33, PollTimeHandleImpl);
        AddTickTime(33, PollMonthTimeHandleImpl);
        AddTickTime(33, PollLoopDayTimeHandleImpl);
        AddTickTime(33, PollLoopWeekDayTimeHandleImpl);
    }

    private static long Refresh()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }

    public static long WeekEndTimestamp()
    {
        var now = DateTime.Now;
        var dayOfWeek = Convert.ToInt32(now.DayOfWeek.ToString("d"));
        dayOfWeek = dayOfWeek <= 0 ? 7 : dayOfWeek;
        var endOfWeek = now.AddDays(7 - dayOfWeek).Date;
        _ = endOfWeek.AddHours(23 - endOfWeek.Hour);
        _ = endOfWeek.AddMinutes(59 - endOfWeek.Minute);
        _ = endOfWeek.AddSeconds(59 - endOfWeek.Second);

        return (long)(endOfWeek - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }

    private void AddTickHandleImpl()
    {
        lock (_addTickHandle)
        {
            if (_addTickHandle.Count > 0)
            {
                foreach (var item in _addTickHandle)
                {
                    var process = item.Key;
                    while (_tickHandleDict.ContainsKey(process))
                    {
                        process++;
                    }
                    _tickHandleDict.Add(process, item.Value);
                }
                _addTickHandle.Clear();
            }
        }
    }

    private readonly List<long> _listTmpTickHandle = new();
    private void PollTickHandleImpl()
    {
        AddTickHandleImpl();

        foreach (var (key, value) in _tickHandleDict)
        {
            if (key <= Tick)
            {
                _listTmpTickHandle.Add(key);

                var impl = value;
                if (impl.IsDel)
                {
                    continue;
                }

                var handle = impl.Handle as Action<long>;
                try
                {
                    handle?.Invoke(Tick);
                }
                catch (System.Exception e)
                {
                    Log.Error("System.Exception:{0}", e);
                }
            }
            else
            {
                break;
            }
        }

        if (_listTmpTickHandle.Count <= 0)
        {
            return;
        }

        foreach (var item in _listTmpTickHandle)
        {
            _tickHandleDict.Remove(item);
        }
        _listTmpTickHandle.Clear();
    }

    private void AddDayTimeHandleImpl()
    {
        lock (_addDayTimeHandle)
        {
            foreach (var (key, value) in _addDayTimeHandle)
            {
                if (!_dayTimeHandleDict.ContainsKey(key))
                {
                    _dayTimeHandleDict.Add(key, new List<HandleImpl>());
                }
                _dayTimeHandleDict[key].AddRange(value);
            }
            _addDayTimeHandle.Clear();
        }
    }

    private void PollDayTimeHandleImpl(long tick)
    {
        AddDayTimeHandleImpl();

        var t = DateTime.Now;
        List<DayTime> list = new();
        foreach (var item in _dayTimeHandleDict)
        {
            if (item.Key.Hour == t.Hour && item.Key.Minute == t.Minute && item.Key.Second <= t.Second)
            {
                list.Add(item.Key);

                foreach (var impl in item.Value)
                {
                    if (impl.IsDel)
                    {
                        continue;
                    }

                    var handle = impl.Handle as Action<DateTime>;

                    try
                    {
                        handle?.Invoke(t);
                    }
                    catch (System.Exception e)
                    {
                        Log.Error("System.Exception:{0}", e);
                    }
                }
            }
        }
        foreach (var item in list)
        {
            _dayTimeHandleDict.Remove(item);
        }

        AddTickTime(888, PollDayTimeHandleImpl);
    }

    private void AddTimeHandleImpl()
    {
        lock (_addTimeHandle)
        {
            foreach (var (key, value) in _addTimeHandle)
            {
                if (!_timeHandleDict.ContainsKey(key))
                {
                    _timeHandleDict.Add(key, new List<HandleImpl>());
                }
                _timeHandleDict[key].AddRange(value);
            }
            _addTimeHandle.Clear();
        }
    }

    private void PollTimeHandleImpl(long tick)
    {
        AddTimeHandleImpl();

        List<WeekDayTime> list = new();
        var t = DateTime.Now;
        foreach (var (key, value) in _timeHandleDict)
        {
            if (key.Day == t.DayOfWeek && key.Hour == t.Hour && key.Minute == t.Minute && key.Second <= t.Second)
            {
                list.Add(key);

                foreach (var impl in value)
                {
                    if (impl.IsDel)
                    {
                        continue;
                    }

                    var handle = impl.Handle as Action<DateTime>;

                    try
                    {
                        handle?.Invoke(t);
                    }
                    catch (System.Exception e)
                    {
                        Log.Error("System.Exceptio{0}", e);
                    }
                }
            }
        }
        foreach (var item in list)
        {
            _timeHandleDict.Remove(item);
        }

        AddTickTime(888, PollTimeHandleImpl);
    }

    private void AddMonthTimeHandleImpl()
    {
        lock (_addMonthTimeHandle)
        {
            foreach (var (key, value) in _addMonthTimeHandle)
            {
                if (!_monthTimeHandleDict.ContainsKey(key))
                {
                    _monthTimeHandleDict.Add(key, new List<HandleImpl>());
                }
                _monthTimeHandleDict[key].AddRange(value);
            }
            _addMonthTimeHandle.Clear();
        }
    }

    private void PollMonthTimeHandleImpl(long tick)
    {
        AddMonthTimeHandleImpl();

        List<MonthDayTime> list = new();
        var t = DateTime.Now;
        foreach (var (key, value) in _monthTimeHandleDict)
        {
            if (key.Month == t.Month && key.Day == t.Day && key.Hour == t.Hour && key.Minute == t.Minute && key.Second == t.Second)
            {
                list.Add(key);

                foreach (var impl in value)
                {
                    if (impl.IsDel)
                    {
                        continue;
                    }

                    var handle = impl.Handle as Action<DateTime>;

                    try
                    {
                        handle?.Invoke(t);
                    }
                    catch (System.Exception e)
                    {
                        Log.Error("System.Exception:{0}", e);
                    }
                }
            }
        }
        foreach (var item in list)
        {
            _monthTimeHandleDict.Remove(item);
        }

        AddTickTime(888, PollMonthTimeHandleImpl);
    }

    private void AddLoopDayTimeHandleImpl()
    {
        lock (_addLoopDayTimeHandle)
        {
            foreach (var (key, value) in _addLoopDayTimeHandle)
            {
                if (!_loopDayTimeHandleDict.ContainsKey(key))
                {
                    _loopDayTimeHandleDict.Add(key, new List<HandleImpl>());
                }
                _loopDayTimeHandleDict[key].AddRange(value);
            }
            _addLoopDayTimeHandle.Clear();
        }
    }

    private void PollLoopDayTimeHandleImpl(long tick)
    {
        AddLoopDayTimeHandleImpl();

        var t = DateTime.Now;
        if (t is { Hour: 0, Minute: 0 } && (Tick - _loopDayTick) >= 24 * 60 * 60 * 1000)
        {
            foreach (var item in _loopDayTimeHandle)
            {
                if (!_loopDayTimeHandleDict.ContainsKey(item.Key))
                {
                    _loopDayTimeHandleDict.Add(item.Key, new List<HandleImpl>());
                }
                _loopDayTimeHandleDict[item.Key].AddRange(item.Value);
            }
            _loopDayTimeHandle.Clear();

            _loopDayTick = Tick;
        }

        List<DayTime> list = new();
        foreach (var (key, value) in _loopDayTimeHandleDict)
        {
            if (key.Hour == t.Hour && key.Minute == t.Minute && key.Second <= t.Second)
            {
                list.Add(key);

                foreach (var impl in value)
                {
                    if (impl.IsDel)
                    {
                        continue;
                    }

                    var handle = impl.Handle as Action<DateTime>;
                    try
                    {
                        handle?.Invoke(t);
                    }
                    catch (System.Exception e)
                    {
                        Log.Error("System.Exception:{0}", e);
                    }
                }
            }
        }

        foreach (var item in list)
        {
            if (!_loopDayTimeHandle.ContainsKey(item))
            {
                _loopDayTimeHandle.Add(item, new List<HandleImpl>());
            }
            foreach (var impl in _loopDayTimeHandleDict[item])
            {
                if (impl.IsDel)
                {
                    continue;
                }

                _loopDayTimeHandle[item].Add(impl);
            }
            _loopDayTimeHandleDict.Remove(item);
        }

        AddTickTime(888, PollLoopDayTimeHandleImpl);
    }

    private void AddLoopWeekDayTimeHandleImpl()
    {
        lock (_addLoopWeekDayTimeHandle)
        {
            foreach (var (key, value) in _addLoopWeekDayTimeHandle)
            {
                if (!_loopWeekDayTimeHandleDict.ContainsKey(key))
                {
                    _loopWeekDayTimeHandleDict.Add(key, new List<HandleImpl>());
                }
                _loopWeekDayTimeHandleDict[key].AddRange(value);
            }
            _addLoopWeekDayTimeHandle.Clear();
        }
    }

    private void PollLoopWeekDayTimeHandleImpl(long tick)
    {
        AddLoopWeekDayTimeHandleImpl();

        var t = DateTime.Now;
        if (t is { DayOfWeek: DayOfWeek.Sunday, Hour: 0, Minute: 0, Second: 0 } && (Tick - _loopWeekDayTick) >= 7 * 24 * 60 * 60 * 1000)
        {
            foreach (var (key, value) in _loopWeekDayTimeHandle)
            {
                if (!_loopWeekDayTimeHandleDict.ContainsKey(key))
                {
                    _loopWeekDayTimeHandleDict.Add(key, new List<HandleImpl>());
                }
                _loopWeekDayTimeHandleDict[key].AddRange(value);
            }
            _loopWeekDayTimeHandle.Clear();

            _loopWeekDayTick = Tick;
        }

        List<WeekDayTime> list = new();
        foreach (var (key, value) in _loopWeekDayTimeHandleDict)
        {
            if (key.Day == t.DayOfWeek && key.Hour == t.Hour && key.Minute == t.Minute && key.Second == t.Second)
            {
                list.Add(key);

                foreach (var impl in value)
                {
                    if (impl.IsDel)
                    {
                        continue;
                    }

                    var handle = impl.Handle as Action<DateTime>;

                    try
                    {
                        handle?.Invoke(t);
                    }
                    catch (System.Exception e)
                    {
                        Log.Error("System.Exceptio{0}", e);
                    }
                }
            }
        }

        foreach (var item in list)
        {
            if (!_loopWeekDayTimeHandle.ContainsKey(item))
            {
                _loopWeekDayTimeHandle.Add(item, new List<HandleImpl>());
            }
            foreach (var impl in _loopWeekDayTimeHandleDict[item])
            {
                if (impl.IsDel)
                {
                    continue;
                }

                _loopWeekDayTimeHandle[item].Add(impl);
            }
            _loopWeekDayTimeHandleDict.Remove(item);
        }

        AddTickTime(888, PollLoopWeekDayTimeHandleImpl);
    }

    public void Poll()
    {
        Tick = Refresh();
        PollTickHandleImpl();
        Tick = Refresh();
    }

	public object AddTickTime(long process, Action<long> handle)
	{
        process += Tick;
        var impl = new HandleImpl(handle);
        lock (_addTickHandle)
        {
            while (_addTickHandle.ContainsKey(process)){ process++; }
            _addTickHandle.Add(process, impl);
        }
        return impl;
	}

    public object AddDayTime(int hour, int minute, int second, Action<DateTime> handle)
    {
        var key = new DayTime()
        {
            Hour = hour,
            Minute = minute,
            Second = second,
        };
        var impl = new HandleImpl(handle);
        lock (_addDayTimeHandle)
        {
            if (!_addDayTimeHandle.ContainsKey(key))
            {
                _addDayTimeHandle.Add(key, new List<HandleImpl>());
            }
            _addDayTimeHandle[key].Add(impl);
        }
        return impl;
    }

    public object AddWeekDayTime(System.DayOfWeek day, int hour, int minute, int second, Action<DateTime> handle)
    {
        var key = new WeekDayTime()
        {
            Day = day,
            Hour = hour,
            Minute = minute,
            Second = second,
        };
        var impl = new HandleImpl(handle);
        lock (_addTimeHandle)
        {
            if (!_addTimeHandle.ContainsKey(key))
            {
                _addTimeHandle.Add(key, new List<HandleImpl>());
            }
            _addTimeHandle[key].Add(impl);
        }
        return impl;
    }

    public object AddMonthDayTime(int month, int day, int hour, int minute, int second, Action<DateTime> handle)
    {
        var key = new MonthDayTime()
        {
            Month = month,
            Day = day,
            Hour = hour,
            Minute = minute,
            Second = second,
        };
        var impl = new HandleImpl(handle);
        lock (_addMonthTimeHandle)
        {
            if (!_addMonthTimeHandle.ContainsKey(key))
            {
                _addMonthTimeHandle.Add(key, new List<HandleImpl>());
            }
            _addMonthTimeHandle[key].Add(impl);
        }
        return impl;
    }

    public object AddLoopDayTime(int hour, int minute, int second, Action<DateTime> handle)
    {
        var key = new DayTime()
        {
            Hour = hour,
            Minute = minute,
            Second = second,
        };
        var impl = new HandleImpl(handle);
        lock (_addLoopDayTimeHandle)
        {
            if (!_addLoopDayTimeHandle.ContainsKey(key))
            {
                _addLoopDayTimeHandle.Add(key, new List<HandleImpl>());
            }
            _addLoopDayTimeHandle[key].Add(impl);
        }
        return impl;
    }

    public object AddLoopWeekDayTime(System.DayOfWeek day, int hour, int minute, int second, Action<DateTime> handle)
    {
        var key = new WeekDayTime()
        {
            Day = day,
            Hour = hour,
            Minute = minute,
            Second = second,
        };
        var impl = new HandleImpl(handle);
        lock (_addLoopWeekDayTimeHandle)
        {
            if (!_addLoopWeekDayTimeHandle.ContainsKey(key))
            {
                _addLoopWeekDayTimeHandle.Add(key, new List<HandleImpl>());
            }
            _addLoopWeekDayTimeHandle[key].Add(impl);
        }
        return impl;
    }

    public void DelTimer(object impl)
    {
        (impl as HandleImpl)?.IsDel = true;
    }

    public static long Tick;

    class HandleImpl
    {
        public HandleImpl(Action<long> handle)
        {
            IsDel = false;
            Handle = handle;
        }

        public HandleImpl(Action<DateTime> handle)
        {
            IsDel = false; 
            Handle = handle;
        }

        public bool IsDel;
        public readonly object Handle;
    }

    struct MonthDayTime
    {
        public int Month;
        public int Day;
        public int Hour;
        public int Minute;
        public int Second;

        public override int GetHashCode()
        {
            return (int)Day * 24 * 3600 + Hour * 3600 + Minute * 60 + Second;
        }

        public override bool Equals(object? obj)
        {
            if (null == obj)
            {
                return false;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            MonthDayTime tmp = (MonthDayTime)obj;
            if (Month == tmp.Month &&
                Day == tmp.Day &&
                Hour == tmp.Hour &&
                Minute == tmp.Minute &&
                Second == tmp.Second)
            {
                return true;
            }

            return false;
        }
    }

    struct WeekDayTime
    {
        public System.DayOfWeek Day;
        public int Hour;
        public int Minute;
        public int Second;

        public override int GetHashCode()
        {
            return (int)Day * 24 * 3600 + Hour * 3600 + Minute * 60 + Second;
        }

        public override bool Equals(object? obj)
        {
            if (null == obj)
            {
                return false;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            WeekDayTime tmp = (WeekDayTime)obj;
            if (Day == tmp.Day &&
                Hour == tmp.Hour &&
                Minute == tmp.Minute &&
                Second == tmp.Second)
            {
                return true;
            }

            return false;
        }
    }

    struct DayTime
    {
        public int Hour;
        public int Minute;
        public int Second;

        public override int GetHashCode()
        {
            return (int)Hour * 3600 + Minute * 60 + Second;
        }

        public override bool Equals(object? obj)
        {
            if (null == obj)
            {
                return false;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            DayTime tmp = (DayTime)obj;
            if (Hour == tmp.Hour &&
                Minute == tmp.Minute &&
                Second == tmp.Second)
            {
                return true;
            }

            return false;
        }
    }

    private readonly SortedDictionary<long, HandleImpl> _tickHandleDict = new();
    private readonly Dictionary<long, HandleImpl> _addTickHandle = new();

    private readonly Dictionary<MonthDayTime, List<HandleImpl>> _monthTimeHandleDict = new();
    private readonly Dictionary<MonthDayTime, List<HandleImpl>> _addMonthTimeHandle = new();

    private readonly Dictionary<WeekDayTime, List<HandleImpl>> _timeHandleDict  = new();
    private readonly Dictionary<WeekDayTime, List<HandleImpl>> _addTimeHandle = new();

    private readonly Dictionary<DayTime, List<HandleImpl>> _loopDayTimeHandleDict = new();
    private readonly Dictionary<DayTime, List<HandleImpl>> _addLoopDayTimeHandle  = new();
    private readonly Dictionary<DayTime, List<HandleImpl>> _loopDayTimeHandle = new();
    private long _loopDayTick;

    private readonly Dictionary<DayTime, List<HandleImpl>> _dayTimeHandleDict = new();
    private readonly Dictionary<DayTime, List<HandleImpl>> _addDayTimeHandle  = new();

    private readonly Dictionary<WeekDayTime, List<HandleImpl>> _loopWeekDayTimeHandleDict = new();
    private readonly Dictionary<WeekDayTime, List<HandleImpl>> _addLoopWeekDayTimeHandle = new();
    private readonly Dictionary<WeekDayTime, List<HandleImpl>> _loopWeekDayTimeHandle = new();
    private long _loopWeekDayTick;
}