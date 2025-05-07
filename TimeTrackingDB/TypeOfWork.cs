using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TimeTrackingDB
{
    public class TypeOfWork
    {
        [Key]
        public int TypeOfWorkId { get; set; }
        public string TypeOfWorkName { get; set; }
    }
}
