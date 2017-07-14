using System.Diagnostics;
using System;

namespace log
{
    public class log
	{
        public enum enLogMode
        {
            Debug = 0,
            Release = 1,
        }

        static public void trace(StackFrame st, Int64 tmptime, string log, params object[] agrvs)
        {
            if (logMode <= enLogMode.Debug)
            {
                output(st, tmptime, "trace", log, agrvs);
            }
        }

        static public void error(StackFrame st, Int64 tmptime, string log, params object[] agrvs)
        {
            output(st, tmptime, "error", log, agrvs);
        }

        static public void operation(StackFrame st, Int64 tmptime, string log, params object[] agrvs)
        {
            output(st, tmptime, "operation", log, agrvs);
        }

        static void output(StackFrame sf, Int64 tmptime, string level, string log, params object[] agrvs)
        {
            log = string.Format(log, agrvs);

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
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
                    if (finfo.Length > 1024 * 1024 * 16) 
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
        static public enLogMode logMode = enLogMode.Debug;
    }
}
