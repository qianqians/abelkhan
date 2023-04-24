using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abelkhan
{
    public class RandomHelper
    {
        static private Random Random = new Random();

        static public int RandomInt(int max)
        {
            return Random.Next(max);
        }
    }
}
