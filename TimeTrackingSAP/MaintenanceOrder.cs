using System;
using System.Collections.Generic;
using TimeTrackingLib;

namespace TimeTrackingSAP
{
    public class MaintenanceOrder
    {
        public string OrderID;
        public DateTime PlannedStartDate;
        public DateTime StartDateTime;
        public DateTime EndDateTime;
        public int Equipment;
        public string Action;
        public string TechCardID;
        public string ActionText;

        public List<MaintenanceOrder> DivideOrderForShifts()
        {
            if (StartDateTime == DateTime.MinValue || EndDateTime == DateTime.MinValue)
            {
                return new List<MaintenanceOrder>();
            }
            var ReturnShifts = new List<MaintenanceOrder>();
            var currentOrder = this;
            Shift StartOrderShift, EndOrderShift;
            StartOrderShift = new Shift(currentOrder.StartDateTime);
            EndOrderShift = new Shift(currentOrder.EndDateTime);
            do
            {
                if (EndOrderShift > StartOrderShift)
                {
                    var EndOfCurrentShift = StartOrderShift.ShiftEndsAt();
                    var FirstPart = new MaintenanceOrder() { OrderID = currentOrder.OrderID, StartDateTime = currentOrder.StartDateTime, Equipment = currentOrder.Equipment, EndDateTime = EndOfCurrentShift };
                    var nextShift = StartOrderShift.NextShift();
                    currentOrder.StartDateTime = nextShift.ShiftStartsAt();
                    StartOrderShift = nextShift;
                    ReturnShifts.Add(FirstPart);
                }
                else
                {
                    if (ReturnShifts.Count == 0 || (currentOrder.EndDateTime - currentOrder.EndDateTime).TotalMinutes > 10)
                        ReturnShifts.Add(currentOrder);
                    break;
                }
            } while (StartOrderShift != EndOrderShift);
            return ReturnShifts;
        }
    }
}
