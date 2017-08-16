using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_timerservice
{
    class Program
    {
        static void Main(string[] args)
        {
            service.timerservice service = new service.timerservice();

            service.addweekdaytime(DayOfWeek.Monday, 17, 12, 0, (DateTime tim) => { Console.WriteLine("addweekdaytime test"); });
            service.addmonthdaytime(8, 7, 17, 16, 0, (DateTime tim) => { Console.WriteLine("addmonthdaytime test"); });
            service.addloopdaytime(17, 17, 0, (DateTime tim) => { Console.WriteLine("addloopdaytime test"); });
            service.addloopweekdaytime(DayOfWeek.Monday, 17, 18, 0, (DateTime tim) => { Console.WriteLine("addloopweekdaytime test"); });

            while (true)
            {
                service.poll();
            }
        }
    }
}
