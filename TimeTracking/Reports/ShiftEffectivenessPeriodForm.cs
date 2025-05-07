using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using TimeTrackingDB;
using TimeTrackingLib;

namespace TimeTracking.Reports
{
    public partial class ShiftEffectivenessPeriodForm : TimeTrackingDataForm
    {
        Dictionary<TimeTrackingDB.Supervisor, ListViewItem> ShiftToListView;
        public ShiftEffectivenessPeriodForm()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            var now = DateTime.Now;
            dtpFrom.Value = now.Date.AddDays(-(now.Day - 1));
            dtpTo.Value = now.Date;
            lvShifts.CheckBoxes = true;
            using (var db = new TimeTrackingDB.DB())
            {
                var supervisors = db.Supervisors.Where(s => s.MaintenanceShift != 0).OrderBy(s => s.MaintenanceShift).ToList();
                ShiftToListView = new Dictionary<TimeTrackingDB.Supervisor, ListViewItem>();
                lvShifts.Columns.Clear();
                lvShifts.Columns.Add("Инженер", 200);
                lvShifts.Columns.Add("Смена", 150);
                lvShifts.Columns.Add("Ср. эффективность", 200);
                foreach (var supervisor in supervisors)
                {
                    var lvi = new ListViewItem(new string[] { supervisor.FIO, $"Смена {supervisor.MaintenanceShift}", "" });
                    lvi.Tag = supervisor;
                    lvi.Checked = true;
                    ShiftToListView.Add(supervisor, lvi);
                    lvShifts.Items.Add(lvi);
                }
            }
            chBalanceGraph.ChartAreas[0].AxisX.LabelStyle.Format = "dd:MM:yyyy";
            //chBalanceGraph.ChartAreas[0].AxisX.MinorGrid.Interval=
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            var results = TimeTracking.Reports.UsingTimeReportData.FillReport(dtpFrom.Value.Date, dtpTo.Value.Date);
            chBalanceGraph.Series.Clear();
            Dictionary<int, System.Windows.Forms.DataVisualization.Charting.Series> SerieOfShift = new Dictionary<int, System.Windows.Forms.DataVisualization.Charting.Series>();
            foreach (var shift in ShiftToListView.Keys)
            {
                var lvi = ShiftToListView.Where(kv => kv.Key.MaintenanceShift == shift.MaintenanceShift).FirstOrDefault().Value;
                lvi.BackColor = SystemColors.Window;
                if (lvi.Checked)
                {
                    var serie = new System.Windows.Forms.DataVisualization.Charting.Series()
                    {
                        ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,
                        BorderWidth = 3,
                        XValueType = ChartValueType.Date,
                        MarkerStyle = (MarkerStyle)shift.MaintenanceShift,
                        MarkerSize = 15
                    };
                    chBalanceGraph.Series.Add(serie);
                    SerieOfShift.Add(shift.MaintenanceShift, serie);
                    chBalanceGraph.ApplyPaletteColors();
                    if (lvi != null)
                    {
                        lvi.BackColor = ControlPaint.Light(serie.Color);
                    }
                }
            }

            Dictionary<int, double> shiftEfficiency = new Dictionary<int, double>();
            Dictionary<int, int> shiftEfficiencyCounter = new Dictionary<int, int>();
            foreach (var shiftDateResult in results.ShiftDateStatistics)
            {
                if (shiftDateResult.Value.Lines.Count > 0)
                {
                    var maintananceShift = new MaintananceShift(shiftDateResult.Value.Lines.First().ShiftDate);
                    var shiftNumber = maintananceShift.ShiftNumber;
                    if (SerieOfShift.ContainsKey(shiftNumber))
                    {
                        if (shiftEfficiency.ContainsKey(shiftNumber))
                        {
                            shiftEfficiency[shiftNumber] += shiftDateResult.Value.BalancePercent;
                            shiftEfficiencyCounter[shiftNumber]++;
                        }
                        else
                        {
                            shiftEfficiency.Add(shiftNumber, shiftDateResult.Value.BalancePercent);
                            shiftEfficiencyCounter.Add(shiftNumber, 1);
                        }
                        var currentSerie = SerieOfShift[shiftNumber];
                        var X = shiftDateResult.Key.ToOADate();
                        currentSerie.Points.Add(new DataPoint(X, shiftDateResult.Value.BalancePercent));

                        /*
                        foreach (var serie in SerieOfShift)
                        { 
                            if (serie.Value != currentSerie)
                            {
                                serie.Value.Points.Add(new DataPoint(X, double.NaN) { IsEmpty = true });
                            }
                        }
                        */
                    }
                }
            }
            foreach (var se in shiftEfficiency) {
                var lvi = ShiftToListView.Where(kv => kv.Key.MaintenanceShift == se.Key).FirstOrDefault().Value;
                if (lvi != null)
                {
                    var counter = shiftEfficiencyCounter[se.Key];
                    lvi.SubItems[2].Text = (se.Value/counter).ToString("F2");
                }
            }
        }
        
    }
}
