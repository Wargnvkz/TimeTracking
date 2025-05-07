using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TimeTrackingDB
{
    public class MaintainShiftSupervisor
    {
        [Key]
        public int MaintainShiftSupervisorID { get; set; }
        //public virtual Shift Shift { get; set; }
        public DateTime? ShiftDate { get; set; }
        public bool IsNightShift { get; set; }
        public string FIO { get; set; }
    }
}
