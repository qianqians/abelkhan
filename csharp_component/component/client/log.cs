using System;
using System.Text;
using System.Diagnostics;

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

            StringBuilder debugStr = new StringBuilder();
            debugStr.AppendFormat("[{0}] [{1}] [{2}] [{3}]:{4}", time, level, sf.GetMethod().DeclaringType.FullName, sf.GetMethod().Name, log);
            var str = debugStr.ToString();

            alllog.AppendLine(str);
            logHandle(str);
        }

        static public void setLogHandle(Action<String> func)
        {
            logHandle = func;
        }

        static public string getAllLog()
        {
            return alllog.ToString();
        }

        static private StringBuilder alllog = new StringBuilder();
        static private Action<String> logHandle = (string strlog) => { System.Console.WriteLine(strlog); };

        static public enLogMode logMode = enLogMode.Debug;
    }
}
