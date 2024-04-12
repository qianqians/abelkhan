using System.Diagnostics;
using System;

namespace Log
{
    public static class Log
    {
        public enum enLogMode
        {
            trace = 0,
            debug = 1,
            info = 2,
            warn = 3,
            err = 4,
        }

        static public void trace(string log, params object[] agrvs)
        {
            if (logMode > enLogMode.trace)
            {
                return;
            }
            output(new System.Diagnostics.StackFrame(1), Service.Timerservice.Tick, "trace", log, agrvs);
        }

        static public void debug(string log, params object[] agrvs)
        {
            if (logMode > enLogMode.debug)
            {
                return;
            }
            output(new System.Diagnostics.StackFrame(1), Service.Timerservice.Tick, "debug", log, agrvs);
        }

        static public void info(string log, params object[] agrvs)
        {
            if (logMode > enLogMode.info)
            {
                return;
            }
            output(new System.Diagnostics.StackFrame(1), Service.Timerservice.Tick, "info", log, agrvs);
        }

        static public void warn(string log, params object[] agrvs)
        {
            if (logMode > enLogMode.warn)
            {
                return;
            }
            output(new System.Diagnostics.StackFrame(1), Service.Timerservice.Tick, "warn", log, agrvs);
        }

        static public void err(string log, params object[] agrvs)
        {
            output(new System.Diagnostics.StackFrame(1), Service.Timerservice.Tick, "err", log, agrvs);
        }

        static void output(StackFrame sf, long tmptime, string level, string log, params object[] agrvs)
        {
            log = string.Format(log, agrvs);

            var startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var time = startTime.AddMilliseconds((double)tmptime);

            var strlog = $"[{time}] [{level}] [{sf.GetMethod().DeclaringType.FullName}] [{sf.GetMethod().Name}]:{log}";

            lock (logFile)
            {
                var realLogFile = logPath + "/" + logFile;
                {
                    if (!System.IO.File.Exists(realLogFile))
                    {
                        var tmp = System.IO.File.Create(realLogFile);
                        tmp.Close();
                        fs = new (realLogFile, true)
                        {
                            AutoFlush = true
                        };
                    }
                    if (fs == null)
                    {
                        fs = new (realLogFile, true)
                        {
                            AutoFlush = true
                        };
                    }
                    System.IO.FileInfo finfo = new System.IO.FileInfo(realLogFile);
                    if (finfo.Length > 1024 * 1024 * 32)
                    {
                        fs.Close();
                        var tmpfile = $"{realLogFile}.{time.ToString("yyyy_MM_dd_h_m_s")}";
                        finfo.MoveTo(tmpfile);
                        var tmp = System.IO.File.Create(realLogFile);
                        tmp.Close();
                        fs = new (realLogFile, true)
                        {
                            AutoFlush = true
                        };
                    }
                }
                fs.WriteLine(strlog);
            }
        }

        public static void close()
        {
            fs?.Close();
        }

        private static System.IO.StreamWriter fs = null;
        public static enLogMode logMode = enLogMode.debug;
        public static string logPath = Environment.CurrentDirectory;
        public static string logFile = "log.txt";
    }
}