using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTrackingLib
{
    public struct MaintananceShift
    {
        public int ShiftNumber;
        public MaintananceShift(DateTime Date)
        {
            ShiftNumber = (((int)((Date.Date - DateTime.MinValue).TotalDays + 1)) % 4) / 2 + 1;
        }
    }
}
