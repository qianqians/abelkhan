using System.Diagnostics;

namespace client;

/// <summary>
/// 客户端日志。
/// 刻意不依赖 core（core 会带进 MongoDB/Consul/Redis/AspNetCore 一堆服务端依赖），
/// 默认输出到控制台，<see cref="LogFile"/> 非空时同时追加到文件。
/// </summary>
public static class Log
{
    public enum EmLogMode
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warn = 3,
        Err = 4,
    }

    public static EmLogMode LogMode = EmLogMode.Debug;
    public static string LogPath = Environment.CurrentDirectory;
    /// <summary>为空表示只输出到控制台</summary>
    public static string LogFile = string.Empty;

    private static readonly object Lock = new();
    private static StreamWriter? _fs;

    public static void Debug(string log)
    {
        if (LogMode > EmLogMode.Debug)
        {
            return;
        }
        Output(new StackFrame(1), "debug", log);
    }

    public static void Info(string log)
    {
        Output(new StackFrame(1), "info", log);
    }

    public static void Warn(string log)
    {
        if (LogMode > EmLogMode.Warn)
        {
            return;
        }
        Output(new StackFrame(1), "warn", log);
    }

    public static void Error(string log)
    {
        Output(new StackFrame(1), "err", log);
    }

    public static void Close()
    {
        lock (Lock)
        {
            _fs?.Close();
            _fs = null;
        }
    }

    private static void Output(StackFrame sf, string level, string log)
    {
        var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] [{sf.GetMethod()?.DeclaringType?.Name}] [{sf.GetMethod()?.Name}]:{log}";
        lock (Lock)
        {
            Console.WriteLine(line);

            if (string.IsNullOrEmpty(LogFile))
            {
                return;
            }

            try
            {
                _fs ??= new StreamWriter(Path.Combine(LogPath, LogFile), true) { AutoFlush = true };
                _fs.WriteLine(line);
            }
            catch
            {
                // 日志写文件失败不影响客户端运行
            }
        }
    }
}
