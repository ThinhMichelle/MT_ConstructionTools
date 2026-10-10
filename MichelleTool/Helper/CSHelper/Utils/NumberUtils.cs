using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichelleTool
{
    public static class NumberUtils
    {
        private static double EPSINOL = 0.00000001;
        public static bool IsEquals(double v1, double v2)
        {
            return Math.Abs(v1 - v2) < EPSINOL ? true : false;
        }
    }
}
