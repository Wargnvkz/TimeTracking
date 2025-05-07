using System;
using System.Linq;
using System.Text;

namespace TimeTrackingSAP
{
    namespace SAPClasses
    {
        public class MaintenanceOrderSAP
        {
            [SAPConnect.SAPGetTable("AUFNR")]
            public string OrderID;

            [SAPConnect.SAPGetTable("GSTRS", SAPType = SAPConnect.SAPType.DATE)]
            public DateTime PlannedStartDate;

            //[SAPConnect.SAPGetTable("GLTRS", SAPType = SAPConnect.SAPType.DATE)]
            //public DateTime PlannedEndDate;

            [SAPConnect.SAPGetTable("GSTRI", SAPType = SAPConnect.SAPType.DATE)]
            public DateTime StartDate;

            [SAPConnect.SAPGetTable("GSUZI", SAPType = SAPConnect.SAPType.TIME)]
            public TimeSpan StartTime;

            [SAPConnect.SAPGetTable("GETRI", SAPType = SAPConnect.SAPType.DATE)]
            public DateTime EndDate;

            [SAPConnect.SAPGetTable("GEUZI", SAPType = SAPConnect.SAPType.TIME)]
            public TimeSpan EndTime;

            [SAPConnect.SAPGetTable("AUFPL")]
            public string TechCardID;


            [SAPConnect.SAPExcludeFromReading]
            public DateTime StartDateTime
            {
                get
                {
                    return StartDate.Add(StartTime);
                }
            }

            [SAPConnect.SAPExcludeFromReading]
            public DateTime EndDateTime
            {
                get
                {
                    return EndDate.Add(EndTime);
                }
            }
        }

        public class TechOrderHeaderSAP
        {
            [SAPConnect.SAPGetTable("AUFNR")]
            public string OrderID;

            [SAPConnect.SAPGetTable("ILOAN")]
            public string TechPlaceNum;

            [SAPConnect.SAPGetTable("WARPL")]
            public string ActionID;
        }

        public class TechPlaceNumSAP
        {
            [SAPConnect.SAPGetTable("ILOAN")]
            public string TechPlaceNum;

            [SAPConnect.SAPGetTable("TPLNR")]
            public string TechPlaceName;
        }

        public class ActionSAP
        {
            [SAPConnect.SAPGetTable("WARPL")]
            public string ActionID;

            [SAPConnect.SAPGetTable("STRAT")]
            public string Strategy;

            [SAPConnect.SAPGetTable("WPTXT")]
            public string Text;

        }
        //AFVC
        public class MaintenanceOrderOperation
        {
            [SAPConnect.SAPGetTable("AUFPL")]
            public string TechCardID;
            [SAPConnect.SAPGetTable("APLZL")]
            public string Counter;
            [SAPConnect.SAPGetTable("LTXA1")]
            public string OperationName;
            [SAPConnect.SAPGetTable("ANZZL")]
            public string Number;
        }
        //AFVV
        public class MaintenanceOrderOperationDuration
        {
            [SAPConnect.SAPGetTable("AUFPL")]
            public string TechCardID;
            [SAPConnect.SAPGetTable("APLZL")]
            public string Counter;
            [SAPConnect.SAPGetTable("DAUNO")]
            public string DurationBase;
            [SAPConnect.SAPGetTable("DAUNE")]
            public string DurationBaseMeasureUnit;
            [SAPConnect.SAPGetTable("ARBEI")]
            public string DurationTotal;
            [SAPConnect.SAPGetTable("ARBEH")]
            public string DurationTotalMeasureUnit;
            [SAPConnect.SAPGetTable("ISMNW")]
            public string FactDuration;
        }

        //AUFK-order header
        public class MaintenanceTechOrderData
        {
            [SAPConnect.SAPGetTable("AUFNR")]
            public string OrderID;
            
            [SAPConnect.SAPGetTable("KTEXT")]
            public string HeaderText;
        }

        //AFVV+AFVC
        public class MaintenanceOrderOperationFull
        {
            public string TechCardID;
            public string Counter;
            public string OperationName;
            public string Number;
            public string DurationTotal;
            public string DurationTotalMeasureUnit;
            public string DurationBase;
            public string DurationBaseMeasureUnit;
            public string FactDuration;
            public DateTime PlannedStartDate;
            public DateTime StartDateTime;
            public DateTime EndDateTime;
            public int Equipment;
            public string Action;
            public string ActionText;
            public string OrderID;
        }

    }
}
