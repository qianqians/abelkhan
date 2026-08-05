using System.Diagnostics;
 
namespace core;

public class Log
{
    public enum EmLogMode
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warn = 3,
        Err = 4,
    }

    public static void Trace(string log, params object[] argv)
    {
        if (LogMode > EmLogMode.Trace)
        {
            return;
        }
        Output(new StackFrame(1), TimerService.Tick, "trace", log, argv);
    }

    public static void Debug(string log, params object[] argv)
    {
        if (LogMode > EmLogMode.Debug)
        {
            return;
        }
        Output(new System.Diagnostics.StackFrame(1),TimerService.Tick, "debug", log, argv);
    }

    public static void Info(string log, params object[] argv)
    {
        if (LogMode > EmLogMode.Info)
        {
            return;
        }
        Output(new System.Diagnostics.StackFrame(1), TimerService.Tick, "info", log, argv);
    }

    public static void Warn(string log, params object[] argv)
    {
        if (LogMode > EmLogMode.Warn)
        {
            return;
        }
        Output(new System.Diagnostics.StackFrame(1), TimerService.Tick, "warn", log, argv);
    }

    public static void Error(string log, params object[] argv)
    {
        Output(new System.Diagnostics.StackFrame(1), TimerService.Tick, "err", log, argv);
    }

    private static void Output(StackFrame sf, long timestamp, string level, string log, params object[] argv)
    {
        var startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var time = startTime.AddMilliseconds(timestamp);

        lock (LogFile)
        {
            var realLogFile = $"{LogPath}/{LogFile}";
            {
                if (!File.Exists(realLogFile))
                {
                    var tmp = System.IO.File.Create(realLogFile);
                    tmp.Close();
                    _fs = new (realLogFile, true)
                    {
                        AutoFlush = true
                    };
                }
                _fs ??= new (realLogFile, true)
                {
                    AutoFlush = true
                };
                FileInfo fifo = new(realLogFile);
                if (fifo.Length > 1024 * 1024 * 32)
                {
                    _fs.Close();
                    var tmpFile = $"{realLogFile}.{time:yyyy_MM_dd_h_m_s}";
                    fifo.MoveTo(tmpFile);
                    var tmp = System.IO.File.Create(realLogFile);
                    tmp.Close();
                    _fs = new (realLogFile, true)
                    {
                        AutoFlush = true
                    };
                }
            }
            _fs.WriteLine($"[{time}] [{level}] [{sf.GetMethod()?.DeclaringType?.FullName}] [{sf.GetMethod()?.Name}]:{log}", argv);
        }
    }

    public static void Close()
    {
        _fs?.Close();
    }

    private static StreamWriter? _fs = null;
    public static EmLogMode LogMode = EmLogMode.Debug;
    public static string LogPath = Environment.CurrentDirectory;
    public static string LogFile = "log.txt";
}