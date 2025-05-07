using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TimeTrackingDB
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }
        //public virtual Shift Shift { get; set; }
        public int Shift { get; set; }
        public int EmployeePositionID { get; set; }
        public string FIO { get; set; }
        public int WorkingHours { get; set; }
    }
}
