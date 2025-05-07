using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TimeTrackingDB;
using TimeTrackingLib;
using TimeTrackingSAP;

namespace TimeTracking.Reports
{
    public partial class StaffTimeTrackingReport : TimeTrackingDataForm
    {

        public StaffTimeTrackingReport()
        {
            InitializeComponent();

            //FillReport();
        }


        /*private void FillReport()
        {
            //var start = new DateTime(2023, 10, 21, 8, 01, 0);
            //var end = new DateTime(2023, 10, 21, 20, 0, 0);
            var start = dtpStartShiftDate.Value.Date;
            var end = dtpEndShiftDate.Value.Date.AddDays(1);
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
                lvFutureWorks.Items.Clear();
                lvFutureWorks.Columns.Clear();
                lvFutureWorks.Columns.Add("Дата", 100);
                lvFutureWorks.Columns.Add("Машина(ППР)", 500);
                lvFutureWorks.Columns.Add("Время выполнения(ППР)", 250);
                lvFutureWorks.Columns.Add("Время выполнения(прочие)", 250);
                lvFutureWorks.Columns.Add("Ч/ч", 200);
                double TotalSPM = 0;
                double TotalOtherWorks = 0;
                int AvailableTime = 0;
                var groupByDate = SPMOperations.GroupBy(o => new { o.StartDateTime.Date });
                foreach (var gbe in groupByDate)
                {
                    var Date = gbe.Key;
                    var DateEquipment = gbe.ToList();
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
                                if (operation.Action.Contains("ППР"))
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

                        var lvi = new ListViewItem(new string[] { $"{Date.Date.ToString("dd-MM-yyyy")}", Eq > 0 ? $"Машина {Eq} ({ActionText})" : ActionText, $"{(DurationInHoursSPM == 0 ? "" : TimeSpan.FromHours(DurationInHoursSPM).ToString())}", $"{(DurationInHoursOtherWorks == 0 ? "" : TimeSpan.FromHours(DurationInHoursOtherWorks).ToString())}", $"{(LaborCostOtherWorks + LaborCostSPM == 0 ? "" : TimeSpan.FromHours(LaborCostOtherWorks + LaborCostSPM).ToString())}" });
                        lvFutureWorks.Items.Add(lvi);
                        TotalSPM += LaborCostSPM;
                        TotalOtherWorks += LaborCostOtherWorks;

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

                    shift = shift.NextShift().NextShift();

                }
                var FullTotal = TotalSPM + TotalOtherWorks;
                lblTotalSPM.Text = Tools.TimeSpan2HMS(TimeSpan.FromHours(TotalSPM));
                lblTotalOthers.Text = Tools.TimeSpan2HMS(TimeSpan.FromHours(TotalOtherWorks));
                lblAvailable.Text = Tools.TimeSpan2HMS(TimeSpan.FromHours(AvailableTime));
                lblUsed.Text = Tools.TimeSpan2HMS(TimeSpan.FromHours(FullTotal));

                var dBalance = FullTotal / AvailableTime * 100;
                lblBalance.Text = dBalance.ToString("F2") + "%";
                lblBalance.ForeColor = dBalance < 100 ? (dBalance < 50 ? Color.Yellow : (dBalance < 90 ? Color.Black : Color.Black)) : (dBalance > 120 ? Color.Black : Color.Black);
                lblBalance.BackColor = dBalance < 100 ? (dBalance < 50 ? Color.Red : (dBalance < 90 ? Color.Yellow : Color.Green)) : (dBalance > 120 ? Color.Yellow : Color.Green);
            }
        }
        */
        private void FillReport()
        {
            //var start = new DateTime(2023, 10, 21, 8, 01, 0);
            //var end = new DateTime(2023, 10, 21, 20, 0, 0);
            var reportData=UsingTimeReportData.FillReport(dtpStartShiftDate.Value.Date, dtpEndShiftDate.Value.Date);
            lvFutureWorks.Items.Clear();
            lvFutureWorks.Columns.Clear();
            lvFutureWorks.Columns.Add("Дата", 100);
            lvFutureWorks.Columns.Add("Машина(ППР)", 500);
            lvFutureWorks.Columns.Add("Время выполнения(ППР)", 250);
            lvFutureWorks.Columns.Add("Время выполнения(прочие)", 250);
            lvFutureWorks.Columns.Add("Ч/ч", 200);
            foreach (var rd in reportData.Lines)
            {
                var lvi = new ListViewItem(new string[] { $"{rd.ShiftDate.Date.ToString("dd-MM-yyyy")}", rd.Text, $"{(rd.SPMDurationInHours== 0 ? "" : TimeSpan.FromHours(rd.SPMDurationInHours).ToString())}", $"{(rd.OtherWorkDurationInHours== 0 ? "" : TimeSpan.FromHours(rd.OtherWorkDurationInHours).ToString())}", $"{(rd.LaborCostInHours== 0 ? "" : TimeSpan.FromHours(rd.LaborCostInHours).ToString())}" });
                lvFutureWorks.Items.Add(lvi);
            }
            lblTotalSPM.Text = Tools.TimeSpan2HMS(TimeSpan.FromHours(reportData.TotalSPM));
            lblTotalOthers.Text = Tools.TimeSpan2HMS(TimeSpan.FromHours(reportData.TotalOtherWorks));
            lblAvailable.Text = Tools.TimeSpan2HMS(TimeSpan.FromHours(reportData.AvailableTime));
            lblUsed.Text = Tools.TimeSpan2HMS(TimeSpan.FromHours(reportData.FullTotal));

            var dBalance = reportData.BalancePercent;
            lblBalance.Text = dBalance.ToString("F2") + "%";
            lblBalance.ForeColor = Color.Black;// dBalance < 100 ? (dBalance < 60 ? Color.Black : (dBalance < 80 ? Color.Black : Color.Black)) : (dBalance > 120 ? Color.Black : Color.Black);
            lblBalance.BackColor = dBalance < 100 ? (dBalance < 60 ? Color.OrangeRed : (dBalance < 80 ? Color.Yellow : Color.LimeGreen)) : (dBalance > 120 ? Color.Yellow : Color.LimeGreen);


        }


        private double String2Double(string s)
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

        private DateTime CenterOfPeriod(DateTime start, DateTime end)
        {
            return start.AddTicks((end - start).Ticks / 2);
        }

        private double TimeToHours(string DurationBase, string DurationBaseMeasureUnit)
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

        private void btnReport_Click(object sender, EventArgs e)
        {
            FillReport();
        }
    }

}
