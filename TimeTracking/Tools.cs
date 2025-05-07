using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTracking
{
    public class Tools
    {
        public static string TimeSpan2HMS(TimeSpan ts)
        {
            return $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
        }
    }
}
