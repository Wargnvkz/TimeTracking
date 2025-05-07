using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TimeTrackingDB
{
    public class Operator
    {
        [Key]
        public int OperatorID { get; set; }
        public string ShiftNumber { get; set; }
        public string OperatorName { get; set; }
    }
}
