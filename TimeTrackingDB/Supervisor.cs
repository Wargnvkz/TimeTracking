using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TimeTrackingDB
{
    public class Supervisor
    {
        [Key]
        public int SupervisorID{ get; set; }
        public string FIO { get; set; }
        public int MaintenanceShift { get; set; }

    }
}
