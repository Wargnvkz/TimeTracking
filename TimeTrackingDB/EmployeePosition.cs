using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TimeTrackingDB
{
    public class EmployeePosition
    {
        [Key]
        public int EmployeePositionID { get; set; }
        public bool IsAuxiliary { get; set; }
        public string EmployeePositionName { get; set; }
    }
}
