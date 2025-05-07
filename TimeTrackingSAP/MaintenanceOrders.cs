using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTrackingSAP
{
    public class MaintenanceOrders
    {
        private static List<SAPClasses.MaintenanceOrderSAP> GetMaintenanceOrders(DateTime From, DateTime To, bool FutureOrders)
        {
            if (From == DateTime.MinValue) From = new DateTime(1900, 1, 1);
            if (To == DateTime.MinValue) To = DateTime.Now;

            var techCardMaintananceType = SAPConnect.AppData.Instance.GetTable("TCA01", new List<string>() { "PLNTY" }, new List<string>() { "FLG_INST = 'X'" });
            techCardMaintananceType.Add("A");
            techCardMaintananceType.Add("E");
            techCardMaintananceType=techCardMaintananceType.Distinct().ToList();

            var techcardOption = "PLNTY IN (" + String.Join(", ", techCardMaintananceType.Select(tc=>"'"+tc+"'")) + ")";

            var fromStr = From.ToString("yyyyMMdd");
            var toStr = To.ToString("yyyyMMdd");
            List<SAPClasses.MaintenanceOrderSAP> orders;
            if (FutureOrders)
            {
                orders = SAPConnect.AppData.Instance.GetTable<SAPClasses.MaintenanceOrderSAP>("AFKO", $"GSTRS >= '{fromStr}' AND GSTRS <= '{toStr}' AND {techcardOption}");
                orders = orders.FindAll(o => o.StartDateTime==DateTime.MinValue || o.EndDateTime== DateTime.MinValue);
            }
            else
            {
                orders = SAPConnect.AppData.Instance.GetTable<SAPClasses.MaintenanceOrderSAP>("AFKO", $"GSTRI >= '{fromStr}' AND GETRI <= '{toStr}' AND {techcardOption}"); //(PLNTY = 'A' OR PLNTY = 'E')
                orders = orders.FindAll(o => o.StartDateTime <= To && o.StartDateTime >= From);
            }
            return orders;
        }
        private static List<SAPClasses.TechOrderHeaderSAP> GetOrderHeaders(List<string> OrderIDs)
        {
            if (OrderIDs == null || OrderIDs.Count == 0) return new List<SAPClasses.TechOrderHeaderSAP>();
            var op = "AUFNR IN (" + String.Join(", ", OrderIDs.Select(o => "'" + o + "'")) + ")";
            var res = SAPConnect.AppData.Instance.GetTable<SAPClasses.TechOrderHeaderSAP>("AFIH", op);
            return res;
        }
        private static List<SAPClasses.MaintenanceTechOrderData> GetTechOrderData(List<string> OrderIDs)
        {
            if (OrderIDs == null || OrderIDs.Count == 0) return new List<SAPClasses.MaintenanceTechOrderData>();
            var op = "AUFNR IN (" + String.Join(", ", OrderIDs.Select(o => "'" + o + "'")) + ")";
            var res = SAPConnect.AppData.Instance.GetTable<SAPClasses.MaintenanceTechOrderData>("AUFK", op);
            return res;
        }
        private static List<SAPClasses.TechPlaceNumSAP> GetTechPlaces(List<string> TechPlacesNums)
        {
            if (TechPlacesNums == null || TechPlacesNums.Count == 0) return new List<SAPClasses.TechPlaceNumSAP>();
            var op = "ILOAN IN (" + String.Join(", ", TechPlacesNums.Select(tp => "'" + tp + "'")) + ") AND TPLNR LIKE 'PL%'";
            var res = SAPConnect.AppData.Instance.GetTable<SAPClasses.TechPlaceNumSAP>("ILOA", op);
            return res;
        }
        private static List<SAPClasses.ActionSAP> GetActions()
        {
            var res = SAPConnect.AppData.Instance.GetTable<SAPClasses.ActionSAP>("MPLA");
            return res;
        }

        public static List<MaintenanceOrder> GetOrders(DateTime From, DateTime To,bool FutureOrders)
        {
            var res = new List<MaintenanceOrder>();
            var orders = GetMaintenanceOrders(From, To, FutureOrders);
            if (orders.Count == 0) return res;
            var OrderIDs = orders.Select(o => o.OrderID).ToList();
            var orderHeaders = GetOrderHeaders(OrderIDs).ToDictionary(o => o.OrderID);

            var TechPlaceNums = orderHeaders.Select(o => o.Value.TechPlaceNum).ToList();
            var workPlaces = GetTechPlaces(TechPlaceNums).ToDictionary(wp => wp.TechPlaceNum);
            var Actions = GetActions().ToDictionary(a => a.ActionID);
            var techOrderData = GetTechOrderData(OrderIDs).ToDictionary(o=>o.OrderID);

            foreach (var o in orders)
            {
                var m = new MaintenanceOrder();
                m.OrderID = o.OrderID;
                m.PlannedStartDate = o.PlannedStartDate;
                m.StartDateTime = o.StartDateTime;
                m.EndDateTime = o.EndDateTime;
                m.Equipment = -1;
                m.TechCardID = o.TechCardID;

                if (orderHeaders.ContainsKey(o.OrderID))
                {
                    var header = orderHeaders[o.OrderID];
                    var techPlaceNum = header.TechPlaceNum;
                    if (workPlaces.ContainsKey(techPlaceNum))
                    {
                        var wp = workPlaces[techPlaceNum];
                        var wpN = wp.TechPlaceName.Trim().Substring(2);
                        if (int.TryParse(wpN, out int eqNum))
                        {
                            m.Equipment = eqNum;
                        }
                    }
                    if (Actions.ContainsKey(header.ActionID))
                    {
                        var action = Actions[header.ActionID];
                        m.Action = action.Strategy.Trim();
                        m.ActionText = action.Text;
                    }
                    else
                    {
                        if (techOrderData.ContainsKey(o.OrderID))
                            m.ActionText = techOrderData[o.OrderID].HeaderText;
                    }
                }


                res.Add(m);
            }
            return res;
        }

        public static List<SAPClasses.MaintenanceOrderOperationFull> GetMaintenanceOrderOperations(DateTime From, DateTime To, bool FutureOrders)
        {
            var orders = GetOrders(From, To, FutureOrders);//.FindAll(o => o.Equipment != -1);
            if (orders.Count == 0) return new List<SAPClasses.MaintenanceOrderOperationFull>();
            var TechCardIDs = orders.Select(o => "'" + o.TechCardID + "'").ToList();
            var TechCardOption = String.Join(", ", TechCardIDs);
            var operations = SAPConnect.AppData.Instance.GetTable<SAPClasses.MaintenanceOrderOperation>("AFVC", "AUFPL IN (" + TechCardOption + ")");
            var operationsDurations = SAPConnect.AppData.Instance.GetTable<SAPClasses.MaintenanceOrderOperationDuration>("AFVV", "AUFPL IN (" + TechCardOption + ")");
            if (operations.Count == 0 || operationsDurations.Count == 0) return new List<SAPClasses.MaintenanceOrderOperationFull>();
            var res = (from op in operations
                       join opD in operationsDurations on new { op.TechCardID, op.Counter } equals new { opD.TechCardID, opD.Counter }
                       join order in orders on op.TechCardID equals order.TechCardID
                       select new SAPClasses.MaintenanceOrderOperationFull()
                       {
                           TechCardID = op.TechCardID,
                           Counter = op.Counter,
                           OperationName = op.OperationName,
                           Number = op.Number,
                           DurationBase = opD.DurationBase,
                           DurationTotal = opD.DurationTotal,
                           FactDuration = opD.FactDuration,
                           DurationBaseMeasureUnit = opD.DurationBaseMeasureUnit,
                           DurationTotalMeasureUnit = opD.DurationTotalMeasureUnit,
                           PlannedStartDate=order.PlannedStartDate,
                           StartDateTime = order.StartDateTime,
                           EndDateTime = order.EndDateTime,
                           Equipment = order.Equipment,
                           Action = order.Action,
                           ActionText=order.ActionText,
                           OrderID = order.OrderID
                       }).ToList();
            return res;
        }

    }
}
