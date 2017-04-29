using System;

namespace test_log
{
    class Program
    {
        static void Main(string[] args)
        {
            log.log.trace(new System.Diagnostics.StackFrame(true), (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds, "123 {0}", "456");
        }
    }
}
