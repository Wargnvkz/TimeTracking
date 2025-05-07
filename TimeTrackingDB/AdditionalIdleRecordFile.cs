using System.ComponentModel.DataAnnotations;

namespace TimeTrackingDB
{
    public class AdditionalIdleRecordFile
    {
        [Key]
        public int AdditionalIdleRecordFileID { get; set; }
        public int AdditionalIdleRecordID { get; set; }
        public string Filename { get; set; }
        public byte[] Data { get; set; }
    }
}
