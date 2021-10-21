﻿using System.Diagnostics;
using System;

namespace log
{
    public class log
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
            output(new System.Diagnostics.StackFrame(1), service.timerservice.Tick, "trace", log, agrvs);
        }

        static public void debug(string log, params object[] agrvs)
        {
            output(new System.Diagnostics.StackFrame(1), service.timerservice.Tick, "debug", log, agrvs);
        }

        static public void info(string log, params object[] agrvs)
        {
            output(new System.Diagnostics.StackFrame(1), service.timerservice.Tick, "info", log, agrvs);
        }

        static public void warn(string log, params object[] agrvs)
        {
            output(new System.Diagnostics.StackFrame(1), service.timerservice.Tick, "warn", log, agrvs);
        }

        static public void err(string log, params object[] agrvs)
        {
            output(new System.Diagnostics.StackFrame(1), service.timerservice.Tick, "err", log, agrvs);
        }

        static void output(StackFrame sf, Int64 tmptime, string level, string log, params object[] agrvs)
        {
            if (level == "trace" && logMode > enLogMode.trace)
            {
                return;
            }
            else if (level == "debug" && logMode > enLogMode.debug)
            {
                return;
            }
            else if (level == "info" && logMode > enLogMode.info)
            {
                return;
            }
            else if (level == "warn" && logMode > enLogMode.warn)
            {
                return;
            }
            
            log = string.Format(log, agrvs);

            System.DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            System.DateTime time = startTime.AddMilliseconds((double)tmptime);

            string strlog = string.Format("[{0}] [{1}] [{2}] [{3}]:{4}", time, level, sf.GetMethod().DeclaringType.FullName, sf.GetMethod().Name, log);

            lock (logFile)
            {
                string realLogFile = logPath + "/" + logFile;

                {
                    if (!System.IO.File.Exists(realLogFile))
                    {
                        var tmp = System.IO.File.Create(realLogFile);
                        tmp.Close();
                    }
                    System.IO.FileInfo finfo = new System.IO.FileInfo(realLogFile);
                    if (finfo.Length > 1024 * 1024 * 32)
                    {
                        string tmpfile = realLogFile + string.Format(".{0}", time.ToString("yyyy_MM_dd_h_m_s"));
                        finfo.MoveTo(tmpfile);
                        var tmp = System.IO.File.Create(realLogFile);
                        tmp.Close();
                    }
                }

                Console.WriteLine(strlog);

                System.IO.StreamWriter fs = new System.IO.StreamWriter(realLogFile, true);
                fs.WriteLine(strlog);
                fs.Close();
            }
        }

        static public string logPath = System.Environment.CurrentDirectory;
        static public string logFile = "log.txt";
        static public enLogMode logMode = enLogMode.debug;
    }
}