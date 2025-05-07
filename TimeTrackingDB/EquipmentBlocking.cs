using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TimeTrackingDB
{
    public class EquipmentBlocking
    {
        [Key]
        public int EquipmentBlockID { get; set; }
        public int EquipmentNumber { get; set; }
        public int? MalfunctionReasonTypeID { get; set; }
        public int? MalfunctionReasonProfileID { get; set; }
        public int? MalfunctionReasonNodeID { get; set; }
        public int? MalfunctionReasonElementID { get; set; }
        public int? MalfunctionReasonMalfunctionTextID { get; set; }
        public string MalfunctionReasonMalfunctionTextComment { get; set; }
    }
}
