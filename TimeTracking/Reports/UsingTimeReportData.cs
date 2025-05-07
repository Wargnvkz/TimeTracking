using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TimeTrackingDB;
using TimeTrackingLib;
using TimeTrackingSAP;

namespace TimeTracking.Reports
{
    public class UsingTimeReportData
    {
        /// <summary>
        /// Общее время ППМ в часах
        /// </summary>
        public double TotalSPM;
        /// <summary>
        /// Общее время других работ в часах
        /// </summary>
        public double TotalOtherWorks;
        /// <summary>
        /// Возможное время работы, всего за период
        /// </summary>
        public double AvailableTime;
        /// <summary>
        /// Сумма времени занятого работой
        /// </summary>
        public double FullTotal => TotalSPM + TotalOtherWorks;
        /// <summary>
        /// Строки выполненных работ для отчета
        /// </summary>
        public List<UsingTimeReportLine> Lines = new List<UsingTimeReportLine>();

        public Dictionary<DateTime, UsingTimeReportData> ShiftDateStatistics = new Dictionary<DateTime, UsingTimeReportData>();
        /// <summary>
        /// Сколько времени от общего времени занимала работа
        /// </summary>
        public double BalancePercent { get { return FullTotal / AvailableTime * 100; } }

        public static UsingTimeReportData FillReport(DateTime FirstDay, DateTime LastDay)
        {
            var result = new UsingTimeReportData();
            //var start = new DateTime(2023, 10, 21, 8, 01, 0);
            //var end = new DateTime(2023, 10, 21, 20, 0, 0);
            var start = FirstDay.Date;
            var end = LastDay.Date.AddDays(1);
            var StartShift = new Shift(start, false);
            var EndShift = new Shift(end, false);
            using (var database = new DB())
            {
                var SPMOperations = MaintenanceOrders.GetMaintenanceOrderOperations(start, end, false);
                if (SPMOperations != null && SPMOperations.Count > 1)
                {
                    SPMOperations = SPMOperations.OrderBy(m => m.StartDateTime).ToList();//.FindAll(o => o.Equipment > 0);
                }
                var EquipmentIdles = database.EquipmentIdles.Where(ei => ei.ShiftStart >= start && ei.ShiftStart <= end).ToList();

                result.TotalSPM = 0;
                result.TotalOtherWorks = 0;
                result.AvailableTime = 0;
                var groupByDate = SPMOperations.GroupBy(o => new { o.StartDateTime.Date });
                foreach (var gbe in groupByDate)
                {
                    var Date = gbe.Key;
                    var DateEquipment = gbe.ToList();
                    var shiftDateResult = new UsingTimeReportData();
                    result.ShiftDateStatistics.Add(Date.Date, shiftDateResult);
                    shiftDateResult.TotalSPM = 0;
                    shiftDateResult.TotalOtherWorks = 0;
                    shiftDateResult.AvailableTime = 0;
                    var groupByEquipment = DateEquipment.OrderBy(de => de.Equipment).ThenBy(de1 => de1.ActionText).GroupBy(de => new { de.Equipment, de.ActionText });
                    foreach (var geDateEquipment in groupByEquipment)
                    {
                        var Eq = geDateEquipment.Key.Equipment;
                        var ActionText = geDateEquipment.Key.ActionText;
                        var EquipmentOperations = geDateEquipment.ToList();
                        var groupByOrders = EquipmentOperations.GroupBy(o => o.OrderID);
                        double DurationInHoursSPM = 0;
                        double DurationInHoursOtherWorks = 0;
                        double LaborCostSPM = 0;
                        double LaborCostOtherWorks = 0;
                        foreach (var grOrder in groupByOrders)
                        {
                            var operations = grOrder.ToList();
                            //var minCounter = operations.Min(o => o.Counter);
                            //var baseOperation = operations.Find(o => o.Counter == minCounter);
                            foreach (var operation in operations)
                            {
                                var Number = String2Double(operation.Number);
                                if (operation.Action?.Contains("ППР") ?? false)
                                {
                                    var operationDuration = TimeToHours(operation.DurationBase, operation.DurationBaseMeasureUnit);

                                    DurationInHoursSPM += operationDuration;
                                    LaborCostSPM += operationDuration * Number;
                                }
                                else
                                {
                                    var centerOfOperation = CenterOfPeriod(operation.StartDateTime, operation.EndDateTime);

                                    var idle = EquipmentIdles.Find(ei => ei.EquipmentNumber == Eq && ei.IdleStart <= centerOfOperation && ei.IdleEnd >= centerOfOperation);
                                    if (idle != null)
                                    {
                                        var operationTimeInHours = (idle.IdleEnd.Value - idle.IdleStart.Value).TotalHours;
                                        DurationInHoursOtherWorks += operationTimeInHours;
                                        LaborCostOtherWorks += operationTimeInHours * Number;

                                    }
                                    else
                                    {
                                        var operationTimeInHours = TimeToHours(operation.DurationBase, operation.DurationBaseMeasureUnit);
                                        DurationInHoursOtherWorks += operationTimeInHours;
                                        LaborCostOtherWorks += operationTimeInHours * Number;
                                    }

                                }
                            }
                        }
                        var utrl = new UsingTimeReportLine();
                        utrl.ShiftDate = Date.Date;
                        utrl.Text = Eq > 0 ? $"Машина {Eq} ({ActionText})" : ActionText;
                        utrl.SPMDurationInHours = DurationInHoursSPM;
                        utrl.OtherWorkDurationInHours = DurationInHoursOtherWorks;
                        utrl.LaborCostInHours = LaborCostSPM + LaborCostOtherWorks;
                        result.Lines.Add(utrl);
                        result.TotalSPM += LaborCostSPM;
                        result.TotalOtherWorks += LaborCostOtherWorks;

                        shiftDateResult.TotalSPM += LaborCostSPM;
                        shiftDateResult.TotalOtherWorks += LaborCostOtherWorks;
                        shiftDateResult.Lines.Add(utrl);

                    }
                }

                // считаем кто работал в этих сменах
                var NotAuxilaryEmployeePositions = database.EmployeePositions.Where(ep => !ep.IsAuxiliary).Select(ep => ep.EmployeePositionID).ToList();
                var EmployeeShifts = new Dictionary<int, List<Employee>>()
                {
                    {0, database.Employees.Where(e => e.Shift == 0 && NotAuxilaryEmployeePositions.Contains(e.EmployeePositionID)).ToList() },
                    {1, database.Employees.Where(e => e.Shift == 1 && NotAuxilaryEmployeePositions.Contains(e.EmployeePositionID)).ToList() },
                    {2, database.Employees.Where(e => e.Shift == 2 && NotAuxilaryEmployeePositions.Contains(e.EmployeePositionID)).ToList() },

                };


                var MaintenanceEmployees = database.MaintainShiftEmployees.Where(e => e.ShiftDate >= StartShift.ShiftDate && e.ShiftDate <= EndShift.ShiftDate && NotAuxilaryEmployeePositions.Contains(e.EmployeePositionID)).ToList();
                var shift = StartShift;
                while (shift < EndShift)
                {
                    var MaintananceShift = new MaintananceShift(shift.ShiftDate);
                    var employees = MaintenanceEmployees.FindAll(me => me.ShiftDate == shift.ShiftDate);
                    double AvailableTime = 0;
                    if (employees.Count != 0)
                    {
                        AvailableTime += employees.Sum(e => e.WorkingHours + e.AdditionalHours);

                    }
                    else
                    {
                        AvailableTime = EmployeeShifts[MaintananceShift.ShiftNumber].Sum(e => e.WorkingHours);
                        if (shift.ShiftDate.DayOfWeek != DayOfWeek.Saturday && shift.ShiftDate.DayOfWeek != DayOfWeek.Sunday)
                        {
                            AvailableTime += EmployeeShifts[0].Sum(e => e.WorkingHours);
                        }

                    }
                    result.AvailableTime += AvailableTime;
                    if (result.ShiftDateStatistics.ContainsKey(shift.ShiftDate))
                    {
                        result.ShiftDateStatistics[shift.ShiftDate].AvailableTime = AvailableTime;
                    }

                    shift = shift.NextShift().NextShift();

                }
                //result.FullTotal = result.TotalSPM + result.TotalOtherWorks;
            }
            return result;
        }
        private static double String2Double(string s)
        {
            double res;
            if (double.TryParse(s, System.Globalization.NumberStyles.Float, CultureInfo.CurrentCulture, out res))
            {
                return res;
            }
            else
            {
                if (double.TryParse(s, System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("ru-ru"), out res))
                {
                    return res;
                }
                else
                {
                    if (double.TryParse(s, System.Globalization.NumberStyles.Float, CultureInfo.GetCultureInfo("en-us"), out res))
                    {
                        return res;
                    }
                    else
                    {
                        if (double.TryParse(s, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out res))
                        {
                            return res;
                        }
                        else
                        {
                            return 0;
                        }
                    }

                }

            }
        }

        private static DateTime CenterOfPeriod(DateTime start, DateTime end)
        {
            return start.AddTicks((end - start).Ticks / 2);
        }

        private static double TimeToHours(string DurationBase, string DurationBaseMeasureUnit)
        {
            double DurationInHours;
            switch (DurationBaseMeasureUnit.Trim())
            {
                case "MIN":
                    DurationInHours = String2Double(DurationBase) / 60;
                    break;
                case "H":
                    DurationInHours = String2Double(DurationBase);
                    break;
                default:
                    DurationInHours = 0;
                    break;
            }
            return DurationInHours;
        }

        public class UsingTimeReportLine
        {
            /// <summary>
            /// Дата смены
            /// </summary>
            public DateTime ShiftDate;
            /// <summary>
            /// Текст работы
            /// </summary>
            public string Text;
            /// <summary>
            /// длительность ППМ в часах
            /// </summary>
            public double SPMDurationInHours;
            /// <summary>
            /// Длительность других работ в часах
            /// </summary>
            public double OtherWorkDurationInHours;
            /// <summary>
            /// Общие трудозатраты, ч/ч
            /// </summary>
            public double LaborCostInHours;
        }
    }
}
