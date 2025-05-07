using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TimeTrackingDB
{
    public class MalfunctionReasonType
    {
        [Key]
        public int MalfunctionReasonTypeID { get; set; }
        public string MalfunctionReasonTypeName { get; set; }
    }
    public class MalfunctionReasonProfile
    {
        [Key]
        public int MalfunctionReasonProfileID { get; set; }
        public int MalfunctionReasonTypeID { get;set;}
        public string MalfunctionReasonProfileName { get; set; }
    }

    public class MalfunctionReasonNode
    {
        [Key]
        public int MalfunctionReasonNodeID { get; set; }
        public int MalfunctionReasonProfileID { get;set;}
        public string MalfunctionReasonNodeName { get; set; }
    }

    public class MalfunctionReasonElement
    {
        [Key]
        public int MalfunctionReasonElementID { get; set; }
        public int MalfunctionReasonNodeID { get; set; }
        public string MalfunctionReasonElementName { get; set; }
    }

    public class MalfunctionReasonMalfunctionText
    {
        [Key]
        public int MalfunctionReasonMalfunctionTextID { get; set; }
        public int MalfunctionReasonTypeID { get; set; }
        public int? MalfunctionReasonProfileID { get; set; }
        public int? MalfunctionReasonNodeID { get; set; }
        public int? MalfunctionReasonElementID { get; set; }
        public string MalfunctionReasonMalfunctionTextName { get; set; }
    }
}
