using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace TimeTrackingDB
{
    public class MaintainShiftEmployee
    {
        [Key]
        public int MaintainShiftEmployeeID { get; set; }
        //public virtual Shift Shift { get; set; }
        public DateTime ShiftDate { get; set; }
        public int EmployeePositionID { get; set; }
        public string FIO { get; set; }
        public int WorkingHours { get; set; }
        public int AdditionalHours { get; set; }
    }
}
