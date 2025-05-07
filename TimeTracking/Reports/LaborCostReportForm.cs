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
    public partial class LaborCostReportForm : TimeTrackingDataForm
    {
        public LaborCostReportForm()
        {
            InitializeComponent();
            FillReport();
        }

        private void FillReport()
        {
            //var start = new DateTime(2023, 10, 21, 8, 01, 0);
            //var end = new DateTime(2023, 10, 21, 20, 0, 0);
            var start = dtpShiftDate.Value.Date;
            var end = dtpShiftDate.Value.Date;
            var StartShift = new Shift(start, false);
            var EndShift = new Shift(end, false);
            var res = MaintenanceOrders.GetMaintenanceOrderOperations(start, end, true).OrderBy(m => m.StartDateTime).ThenBy(m1=>m1.Equipment).ToList();
            lvFutureWorks.Items.Clear();
            lvFutureWorks.Columns.Clear();
            lvFutureWorks.Columns.Add("Машина", 100);
            lvFutureWorks.Columns.Add("Название операции", 500);
            lvFutureWorks.Columns.Add("Время выполнения", 200);
            lvFutureWorks.Columns.Add("Работников", 200);
            double TotalLaborCost = 0;
            double TotalSPM = 0;
            double TotalOtherWorks = 0;
            for (int i = 0; i < res.Count; i++)
            {
                var r = res[i];
                //var work = $"{r.OrderID}\t{r.TechCardID}\t{r.StartDateTime}\t{r.EndDateTime}\t{r.Equipment}\t{r.Action}\t{r.OperationName}\t{r.DurationBase} {r.DurationBaseMeasureUnit}*{r.Number} чел={r.DurationTotal} {r.DurationTotalMeasureUnit}\t({(r.EndDateTime - r.StartDateTime).TotalMinutes.ToString("F0")})";
                double DurationInHours;
                int WorkersCounter = Convert.ToInt32(String2Double(r.Number));
                switch (r.DurationBaseMeasureUnit.Trim())
                {
                    case "MIN":
                        DurationInHours = String2Double(r.DurationBase) / 60;
                        break;
                    case "H":
                        DurationInHours = String2Double(r.DurationBase);
                        break;
                    default:
                        DurationInHours = 0;
                        break;
                }
                var lvi = new ListViewItem(new string[] { $"Машина {r.Equipment}", r.OperationName.Trim(), $"{TimeSpan.FromHours(DurationInHours)}", $"{WorkersCounter} человек(а)" });
                lvFutureWorks.Items.Add(lvi);
                TotalLaborCost += DurationInHours * WorkersCounter;
                if (r.Action?.Contains("ППР")??false)
                {
                    TotalSPM += DurationInHours;
                }
                else
                {
                    TotalOtherWorks += DurationInHours;
                }

            }

            int AvailableTime;
            using (var database = new DB())
            {
                var NotAuxilaryEmployeePositions = database.EmployeePositions.Where(ep => !ep.IsAuxiliary).Select(ep => ep.EmployeePositionID).ToList();
                var EmployeeShifts = new Dictionary<int, List<Employee>>()
                {
                    {0, database.Employees.Where(e => e.Shift == 0 && NotAuxilaryEmployeePositions.Contains(e.EmployeePositionID)).ToList() },
                    {1, database.Employees.Where(e => e.Shift == 1 && NotAuxilaryEmployeePositions.Contains(e.EmployeePositionID)).ToList() },
                    {2, database.Employees.Where(e => e.Shift == 2 && NotAuxilaryEmployeePositions.Contains(e.EmployeePositionID)).ToList() },

                };
                var supervisors = database.Supervisors.Where(s => new int[] { 1, 2 }.Contains(s.MaintenanceShift)).ToDictionary(s => s.MaintenanceShift);


                var shift = StartShift;
                var MaintenanceEmployees = database.MaintainShiftEmployees.Where(e => e.ShiftDate >= StartShift.ShiftDate && e.ShiftDate <= EndShift.ShiftDate && NotAuxilaryEmployeePositions.Contains(e.EmployeePositionID)).ToList();
                //while (shift <= EndShift)
                //{
                var MaintananceShift = new MaintananceShift(shift.ShiftDate);
                var employees = MaintenanceEmployees.FindAll(me => me.ShiftDate == shift.ShiftDate);
                if (employees.Count != 0)
                {
                    AvailableTime = employees.Sum(e => e.WorkingHours + e.AdditionalHours);
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

                //}
            }
            lblTotalSPM.Text = Tools.TimeSpan2HMS(TimeSpan.FromHours(TotalSPM));
            lblTotalOthers.Text = Tools.TimeSpan2HMS(TimeSpan.FromHours(TotalOtherWorks));
            lblAvailable.Text = Tools.TimeSpan2HMS(TimeSpan.FromHours(AvailableTime));
            lblLaborCost.Text = Tools.TimeSpan2HMS(TimeSpan.FromHours(TotalLaborCost));

            var dBalance = AvailableTime - TotalLaborCost;
            lblBalance.Text = Tools.TimeSpan2HMS(TimeSpan.FromHours(dBalance));
            lblBalance.BackColor = dBalance < 0 ? Color.Yellow : SystemColors.Control;
            lblBalance.ForeColor = dBalance < 0 ? Color.Red : Color.Black;
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

        private void dtpShiftDate_ValueChanged(object sender, EventArgs e)
        {
            FillReport();
        }
    }
}
