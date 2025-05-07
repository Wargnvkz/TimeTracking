using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TimeTrackingDB
{
    public class AdditionalIdleRecord
    {
        [Key]
        public int AdditionalIdleRecordID { get; set; }
        public int EquipmentIdleID { get; set; }
        public int UserID { get; set; }
        public DateTime RecordDateTimeCreation { get; set; }
        public string Text { get; set; }
    }
}
