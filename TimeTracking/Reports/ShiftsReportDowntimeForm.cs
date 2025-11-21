using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TimeTrackingDB;
using TimeTrackingLib;

namespace TimeTracking.Reports
{
    public partial class ShiftsReportDowntimeForm : TimeTrackingDataForm
    {
        int[] ActiveMachines = new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        int MaxMachines = 20;
        public ShiftsReportDowntimeForm()
        {
            InitializeComponent();
            FillFilers();
        }

        private List<EquipmentIdle> GetListForReport(List<int> EquipmentNumbers, List<int> Shifts, DateTime Start, DateTime End)
        {
            Start = Start.Date;
            End = End.Date;
            //Start = new Shift(Start.Date, false).ShiftStartsAt();
            //End = new Shift(End.Date, true).ShiftEndsAt();
            List<EquipmentIdle> res;
            using (var db = new DB())
            {
                if (EquipmentNumbers != null && EquipmentNumbers.Count > 0)
                    res = db.EquipmentIdles.Where(ei => ei.ShiftStart >= Start && ei.ShiftStart <= End && EquipmentNumbers.Contains(ei.EquipmentNumber)).ToList();
                else
                    res = db.EquipmentIdles.Where(ei => ei.ShiftStart >= Start && ei.ShiftStart <= End).ToList();

                if (Shifts != null && Shifts.Count > 0)
                {
                    res = res.FindAll(ei => ei.ShiftStart.HasValue && Shifts.Contains(Shift.GetShiftNumber(ei.ShiftStart.Value, ei.IsNightShift)));
                }
            }

            return res.FindAll(r => ActiveMachines.Contains(r.EquipmentNumber));
        }

        private void CreateReport()
        {

            SortedSet<int> ShiftsChecked = new SortedSet<int>();
            foreach (ListViewItem item in lvShifts.Items)
            {
                if (item.Checked)
                    ShiftsChecked.Add((int)item.Tag);
            }
            SortedSet<int> MachinesChecked = new SortedSet<int>();
            foreach (ListViewItem item in lvMachines.Items)
            {
                if (item.Checked)
                    MachinesChecked.Add((int)item.Tag);
            }

        }

        public void FillFilers()
        {
            // смены
            for (int shift = 0; shift < 5; shift++)
            {
                string ItemText;
                if (shift == 0)
                    ItemText = "Все";
                else
                    ItemText = $"Смена {shift}";
                var lvItem = new ListViewItem(ItemText);
                lvItem.Tag = shift;
                lvShifts.Items.Add(lvItem);
            }
            lvShifts.Items[0].Checked = true;
            lvShifts.ItemChecked += ListViews_ItemChecked;

            for (int machine = 0; machine <= MaxMachines; machine++)
            {
                string ItemText;
                if (machine == 0)
                    ItemText = "Все";
                else
                    ItemText = $"Машина {machine}";
                var lvItem = new ListViewItem(ItemText);
                lvItem.Tag = machine;
                lvMachines.Items.Add(lvItem);
            }
            lvMachines.Items[0].Checked = true;
            lvMachines.ItemChecked += ListViews_ItemChecked;
        }
        private void ListViews_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var LV = sender as ListView;
            if (LV == null) return;
            if ((int)e.Item.Tag == 0)
            {
                if (e.Item.Checked)
                {
                    foreach (ListViewItem item in LV.Items)
                    {
                        if (item != e.Item)
                        {
                            item.Checked = false;
                        }
                    }
                }
            }
            else
            {
                if (e.Item.Checked)
                {
                    LV.Items[0].Checked = false;
                }
            }
        }
        private void CreateEquipmentExcelReport(DateTime FromDate, DateTime ToDate, List<int> Machines, List<int> Shifts)
        {
            var excelApp = new Microsoft.Office.Interop.Excel.Application();

            // Make the object visible.
            //excelApp.Visible = true;

            // Create a new, empty workbook and add it to the collection returned
            // by property Workbooks. The new workbook becomes the active workbook.
            // Add has an optional parameter for specifying a praticular template.
            // Because no argument is sent in this example, Add creates a new workbook.
            var workbook = excelApp.Workbooks.Add();
            // This example uses a single workSheet. The explicit type casting is
            // removed in a later procedure.
            Microsoft.Office.Interop.Excel._Worksheet workSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelApp.ActiveSheet;
            workSheet.Name = "Простои";

            workSheet.Cells[1, "A"] = "Дата";
            workSheet.Cells[1, "B"] = "";
            workSheet.Cells[1, "C"] = "Смена";
            workSheet.Cells[1, "D"] = "Остан.";
            workSheet.Cells[1, "E"] = "Запуск";
            workSheet.Cells[1, "F"] = "Прост.";
            workSheet.Cells[1, "G"] = "Вид";
            workSheet.Cells[1, "H"] = "Профиль";
            workSheet.Cells[1, "I"] = "Узел";
            workSheet.Cells[1, "J"] = "Элемент";
            workSheet.Cells[1, "K"] = "Неисправность";
            workSheet.Cells[1, "L"] = "Примечания";

            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[1, "B"]].Merge();

            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[1, "L"]].Font.Bold = true;

            int StartRow = 2;
            int row = StartRow;
            var TotalPeriod = (ToDate - FromDate).TotalMinutes;
            //List<ReportData> reportList = new List<ReportData>();
            var lst = GetListForReport(Machines, Shifts, FromDate, ToDate);
            Dictionary<int, MalfunctionReasonType> reasonTypes;
            Dictionary<int, MalfunctionReasonProfile> profiles;
            Dictionary<int, MalfunctionReasonNode> nodes;
            Dictionary<int, MalfunctionReasonElement> elements;
            Dictionary<int, MalfunctionReasonMalfunctionText> malfunction;
            using (var db = new DB())
            {
                reasonTypes = db.MalfunctionReasonTypes.OrderBy(rt => rt.MalfunctionReasonTypeID).ToDictionary(x => x.MalfunctionReasonTypeID);// ToList();
                profiles = db.MalfunctionReasonProfiles.ToDictionary(x => x.MalfunctionReasonProfileID);// ToList();
                nodes = db.MalfunctionReasonNodes.ToDictionary(x => x.MalfunctionReasonNodeID);//ToList();
                elements = db.MalfunctionReasonElements.ToDictionary(x => x.MalfunctionReasonElementID);//ToList();
                malfunction = db.MalfunctionReasonMalfunctionTexts.ToDictionary(x => x.MalfunctionReasonMalfunctionTextID);//ToList();
            }

            var ListGroupedByEquipmentNumber = lst.OrderBy(e => e.EquipmentNumber).GroupBy(e => e.EquipmentNumber);
            excelApp.Visible = true;
            foreach (var eqN in ListGroupedByEquipmentNumber)
            {
                workSheet.Cells[row, "A"] = "Тех. комплекс " + eqN.Key;
                workSheet.Cells[row, "A"].Font.Bold = true;
                row++;
                var ListOfIdlesForThisEquipment = eqN.ToList();
                foreach (var idle in ListOfIdlesForThisEquipment)
                {
                    if (!idle.ShiftStart.HasValue) continue;
                    workSheet.Cells[row, "A"] = idle.ShiftStart.Value.ToString("dd-MM-yyyy");
                    workSheet.Cells[row, "B"] = idle.IsNightShift ? "Н" : "Д";
                    workSheet.Cells[row, "C"] = Shift.GetShiftNumber(idle.ShiftStart.Value, idle.IsNightShift); ;
                    workSheet.Cells[row, "D"] = idle.IdleStart.HasValue ? idle.IdleStart.Value.ToString("HH\\:mm") : "";
                    workSheet.Cells[row, "E"] = idle.IdleEnd.HasValue ? (idle.IdleDuration.TotalHours <= 12 ? idle.IdleEnd.Value.ToString("HH\\:mm") : idle.IdleEnd.Value.ToString("dd.MM.yyyy HH\\:mm")) : "";
                    workSheet.Cells[row, "F"] = idle.IdleStart.HasValue && idle.IdleEnd.HasValue ? $"{(int)idle.IdleDuration.TotalHours}:{(int)idle.IdleDuration.Minutes:D2}" : "";// "Прост.";
                    workSheet.Cells[row, "G"] = idle.MalfunctionReasonTypeID.HasValue && reasonTypes.ContainsKey(idle.MalfunctionReasonTypeID.Value) ? reasonTypes[idle.MalfunctionReasonTypeID.Value].MalfunctionReasonTypeName : "";// "Вид";
                    workSheet.Cells[row, "H"] = idle.MalfunctionReasonProfileID.HasValue && profiles.ContainsKey(idle.MalfunctionReasonProfileID.Value) ? profiles[idle.MalfunctionReasonProfileID.Value].MalfunctionReasonProfileName : ""; //"Профиль";
                    workSheet.Cells[row, "I"] = idle.MalfunctionReasonNodeID.HasValue && nodes.ContainsKey(idle.MalfunctionReasonNodeID.Value) ? nodes[idle.MalfunctionReasonNodeID.Value].MalfunctionReasonNodeName : "";// "Узел";
                    workSheet.Cells[row, "J"] = idle.MalfunctionReasonElementID.HasValue && elements.ContainsKey(idle.MalfunctionReasonElementID.Value) ? elements[idle.MalfunctionReasonElementID.Value].MalfunctionReasonElementName : "";// "Элемент";
                    workSheet.Cells[row, "K"] = idle.MalfunctionReasonMalfunctionTextID.HasValue ? (malfunction.ContainsKey(idle.MalfunctionReasonMalfunctionTextID.Value) ? malfunction[idle.MalfunctionReasonMalfunctionTextID.Value].MalfunctionReasonMalfunctionTextName : "") : "";// "Неисправность";
                    workSheet.Cells[row, "L"] = idle.MalfunctionReasonMalfunctionTextComment;
                    row++;
                }
            }


            row++;
            var MachinesReasonsRow = row;

            workSheet.Cells[row, "A"] = "Вид оборудования";
            workSheet.Cells[row, "A"].Font.Bold = true;
            workSheet.Cells[row, "C"] = "Причина";
            workSheet.Cells[row, "C"].Font.Bold = true;
            workSheet.Cells[row, "H"] = "Время простоя";
            workSheet.Cells[row, "H"].Font.Bold = true;

            workSheet.Range[workSheet.Cells[row, "A"], workSheet.Cells[row, "B"]].Merge();
            workSheet.Range[workSheet.Cells[row, "C"], workSheet.Cells[row, "G"]].Merge();
            workSheet.Range[workSheet.Cells[row, "H"], workSheet.Cells[row, "I"]].Merge();

            row++;
            var ReportMachines = lst.OrderBy(rd => rd.EquipmentNumber).GroupBy(rd => new { rd.EquipmentNumber });
            TimeSpan totalIdleTime = new TimeSpan();

            foreach (var rmr in ReportMachines)
            {
                workSheet.Cells[row, "A"] = "Тех.комплекс " + rmr.Key.EquipmentNumber;
                workSheet.Cells[row, "A"].Font.Bold = true;
                var reasons = rmr.ToList();
                TimeSpan totalEqIdleTime = new TimeSpan();
                reasons.ForEach(ril => totalEqIdleTime = totalEqIdleTime.Add(ril.IdleDuration));
                totalIdleTime = totalIdleTime.Add(totalEqIdleTime);
                var reasonsGrp = reasons.OrderBy(e => e.MalfunctionReasonTypeID).GroupBy(rg => rg.MalfunctionReasonTypeID);
                row++;
                foreach (var rg in reasonsGrp)
                {
                    var reasonIdlelst = rg.ToList();
                    var reasonName = rg.Key.HasValue ? (reasonTypes.ContainsKey(rg.Key.Value) ? reasonTypes[rg.Key.Value].MalfunctionReasonTypeName : rg.Key.ToString()) : "Неизвестно";
                    TimeSpan totalReasonIdleTime = new TimeSpan();
                    foreach (var ril in reasonIdlelst)
                    {
                        totalReasonIdleTime = totalReasonIdleTime.Add(ril.IdleDuration);
                    }
                    workSheet.Cells[row, "C"] = reasonName;
                    workSheet.Cells[row, "H"].NumberFormat = "@";
                    workSheet.Cells[row, "H"] = totalReasonIdleTime.ToHMString();//.ToString("mm\\:ss");
                    workSheet.Cells[row, "H"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                    workSheet.Cells[row, "I"] = (totalReasonIdleTime.TotalMinutes / TotalPeriod * 100).ToString("F2") + "%";
                    workSheet.Range[workSheet.Cells[row, "A"], workSheet.Cells[row, "B"]].Merge();
                    workSheet.Range[workSheet.Cells[row, "C"], workSheet.Cells[row, "G"]].Merge();
                    row++;
                }
                workSheet.Cells[row, "D"] = "Итого по тех.комплексу " + rmr.Key.EquipmentNumber;
                workSheet.Range[workSheet.Cells[row, "D"], workSheet.Cells[row, "G"]].Merge();
                workSheet.Cells[row, "D"].Font.Bold = true;
                workSheet.Cells[row, "H"] = totalEqIdleTime.ToHMString();
                workSheet.Cells[row, "I"] = (totalEqIdleTime.TotalMinutes / TotalPeriod * 100).ToString("F2") + "%";
                row++;
            }

            workSheet.Cells[row, "D"] = "Итого тех.комплексам";
            workSheet.Range[workSheet.Cells[row, "D"], workSheet.Cells[row, "G"]].Merge();
            workSheet.Cells[row, "D"].Font.Italic = true;
            var totalTimeString = totalIdleTime.ToHMString(); ;
            workSheet.Cells[row, "H"].NumberFormat = "@";
            workSheet.Cells[row, "H"] = totalTimeString;
            workSheet.Cells[row, "H"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
            workSheet.Cells[row, "I"] = (totalIdleTime.TotalMinutes / (TotalPeriod * ReportMachines.Count()) * 100).ToString("F2") + "%";
            row++;
            row++;




            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[2, "O"]]).Cells.HorizontalAlignment =
                Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[2, "O"]]).Cells.VerticalAlignment =
                 Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "C"]]).Cells.HorizontalAlignment =
                Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "A"]]).EntireColumn.NumberFormat = "MM/DD/YYYY";

            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "C"]]).Cells.VerticalAlignment =
                 Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[StartRow - 1, "O"]].Font.Size = 14;
            workSheet.Range[workSheet.Cells[StartRow, "A"], workSheet.Cells[row, "O"]].Font.Size = 13;
            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "O"]].Font.Name = "Arial Cyr";
            for (int cols = 1; cols < 16; cols++)
            {
                workSheet.Columns[cols].AutoFit();
            }
            workSheet.Columns[4].AutoFit();


            var rowTab1 = 1;
            var xlSheets = workbook.Sheets as Microsoft.Office.Interop.Excel.Sheets;
            var TableSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlSheets.Add(Type.Missing, xlSheets[1], Type.Missing, Type.Missing);
            TableSheet.Name = "Таблица";

            //Dictionary<int, char> ReasonTypeColumn = new Dictionary<int, char>();

            //for (int i = 0; i < reasonTypes.Count; i++)
            foreach (var rt in reasonTypes)
            {
                //var reason = reasonTypes.Where(r => r.Value.MalfunctionReasonTypeID == i).FirstOrDefault().Value;
                var reason = rt.Value;
                if (reason != null)
                {
                    var col = ((char)(65 + reason.MalfunctionReasonTypeID)).ToString();
                    //ReasonTypeColumn.Add(reason.MalfunctionReasonTypeID, col);
                    TableSheet.Cells[rowTab1, col] = reason.MalfunctionReasonTypeName;
                }

            }
            TableSheet.Range[TableSheet.Cells[rowTab1, "B"], TableSheet.Cells[rowTab1, "G"]].Font.Bold = true;
            Dictionary<int, TimeSpan> ReasonTimes = new Dictionary<int, TimeSpan>();
            rowTab1++;
            foreach (var rmr in ReportMachines)
            {
                TableSheet.Cells[rowTab1, "A"] = rmr.Key.EquipmentNumber;
                TableSheet.Cells[rowTab1, "A"].Font.Bold = true;
                var reasons = rmr.ToList();
                TimeSpan totalEqIdleTime = new TimeSpan();
                reasons.ForEach(ril => totalEqIdleTime = totalEqIdleTime.Add(ril.IdleDuration));
                totalIdleTime = totalIdleTime.Add(totalEqIdleTime);
                var reasonsGrp = reasons.OrderBy(e => e.MalfunctionReasonTypeID).GroupBy(rg => rg.MalfunctionReasonTypeID);
                foreach (var rg in reasonsGrp)
                {
                    if (rg.Key == null || rg.Key == 0) continue;
                    var col = (char)(65 + rg.Key);
                    var reasonIdlelst = rg.ToList();
                    var reasonName = rg.Key.HasValue ? (reasonTypes.ContainsKey(rg.Key.Value) ? reasonTypes[rg.Key.Value].MalfunctionReasonTypeName : rg.Key.ToString()) : "Неизвестно";
                    TimeSpan totalReasonIdleTime = new TimeSpan();
                    reasonIdlelst.ForEach(ril => totalReasonIdleTime = totalReasonIdleTime.Add(ril.IdleDuration));
                    //TableSheet.Cells[rowTab1, "C"] = reasonName;
                    TableSheet.Cells[rowTab1, col.ToString()].NumberFormat = "@";
                    TableSheet.Cells[rowTab1, col.ToString()] = totalReasonIdleTime.ToHMSString();//.ToString("mm\\:ss");
                    TableSheet.Cells[rowTab1, col.ToString()].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                    if (!ReasonTimes.ContainsKey(rg.Key.Value))
                    {
                        ReasonTimes.Add(rg.Key.Value, totalReasonIdleTime);
                    }
                    else
                    {
                        ReasonTimes[rg.Key.Value] = ReasonTimes[rg.Key.Value].Add(totalReasonIdleTime);
                    }
                }
                rowTab1++;
            }

            TableSheet.Cells[rowTab1, "A"] = "Общее";
            TableSheet.Cells[rowTab1, "A"].Interior.Color = XlRgbColor.rgbLightGray;
            foreach (var rt in reasonTypes)
            {
                if (!ReasonTimes.ContainsKey(rt.Key)) continue;
                var reason = rt.Value;
                if (reason != null)
                {
                    var col = ((char)(65 + reason.MalfunctionReasonTypeID)).ToString();
                    TableSheet.Cells[rowTab1, col].NumberFormat = "@";
                    TableSheet.Cells[rowTab1, col] = ReasonTimes[rt.Key].ToHMSString();//.ToString("mm\\:ss");
                    TableSheet.Cells[rowTab1, col].Interior.Color = XlRgbColor.rgbLightGray;
                    //TableSheet.Cells[rowTab1, col] = ReasonTimes[rt.Key];
                }

            }

            ((Microsoft.Office.Interop.Excel.Range)TableSheet.Range[TableSheet.Cells[1, "A"], TableSheet.Cells[rowTab1, "O"]]).Cells.HorizontalAlignment =
                Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ((Microsoft.Office.Interop.Excel.Range)TableSheet.Range[TableSheet.Cells[1, "A"], TableSheet.Cells[rowTab1, "O"]]).Cells.VerticalAlignment =
                 Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

            TableSheet.Range[TableSheet.Cells[1, "A"], TableSheet.Cells[1, "O"]].Font.Size = 14;
            TableSheet.Range[TableSheet.Cells[StartRow, "A"], TableSheet.Cells[row, "O"]].Font.Size = 13;
            TableSheet.Range[TableSheet.Cells[1, "A"], TableSheet.Cells[row, "O"]].Font.Name = "Arial Cyr";
            for (int cols = 1; cols < 16; cols++)
            {
                TableSheet.Columns[cols].AutoFit();
            }
            //TableSheet.Columns[4].AutoFit();

            workSheet.Activate();

            rowTab1++;


            excelApp.Visible = true;

        }
        private void CreateShiftExcelReport(DateTime FromDate, DateTime ToDate, List<int> Machines, List<int> Shifts)
        {
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            // Make the object visible.

            // Create a new, empty workbook and add it to the collection returned
            // by property Workbooks. The new workbook becomes the active workbook.
            // Add has an optional parameter for specifying a praticular template.
            // Because no argument is sent in this example, Add creates a new workbook.
            excelApp.Workbooks.Add();
            // This example uses a single workSheet. The explicit type casting is
            // removed in a later procedure.
            Microsoft.Office.Interop.Excel._Worksheet workSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelApp.ActiveSheet;

            workSheet.Cells[1, "A"] = "Дата";
            workSheet.Cells[1, "B"] = "Машина";
            workSheet.Cells[1, "C"] = "Смена";
            workSheet.Cells[1, "D"] = "Остан.";
            workSheet.Cells[1, "E"] = "Запуск";
            workSheet.Cells[1, "F"] = "Прост.";
            workSheet.Cells[1, "G"] = "Вид";
            workSheet.Cells[1, "H"] = "Профиль";
            workSheet.Cells[1, "I"] = "Узел";
            workSheet.Cells[1, "J"] = "Элемент";
            workSheet.Cells[1, "K"] = "Неисправность";
            workSheet.Cells[1, "L"] = "Примечания";

            //workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[1, "B"]].Merge();

            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[1, "L"]].Font.Bold = true;

            int StartRow = 2;
            int row = StartRow;
            var TotalPeriod = (ToDate.AddDays(1) - FromDate).TotalMinutes * ActiveMachines.Length;
            //List<ReportData> reportList = new List<ReportData>();
            var lst = GetListForReport(Machines, Shifts, FromDate, ToDate);
            Dictionary<int, MalfunctionReasonType> reasonTypes;
            Dictionary<int, MalfunctionReasonProfile> profiles;
            Dictionary<int, MalfunctionReasonNode> nodes;
            Dictionary<int, MalfunctionReasonElement> elements;
            Dictionary<int, MalfunctionReasonMalfunctionText> malfunction;
            using (var db = new DB())
            {
                reasonTypes = db.MalfunctionReasonTypes.ToDictionary(x => x.MalfunctionReasonTypeID);// ToList();
                profiles = db.MalfunctionReasonProfiles.ToDictionary(x => x.MalfunctionReasonProfileID);// ToList();
                nodes = db.MalfunctionReasonNodes.ToDictionary(x => x.MalfunctionReasonNodeID);//ToList();
                elements = db.MalfunctionReasonElements.ToDictionary(x => x.MalfunctionReasonElementID);//ToList();
                malfunction = db.MalfunctionReasonMalfunctionTexts.ToDictionary(x => x.MalfunctionReasonMalfunctionTextID);//ToList();
            }

            var ListGroupedByShift = lst.OrderBy(e => e.ShiftStart).ThenBy(e1 => e1.IsNightShift).ThenBy(e2 => e2.EquipmentNumber).GroupBy(e => new { e.ShiftStart, e.IsNightShift });

            foreach (var shift in ListGroupedByShift)
            {
                //row++;
                var GroupByEq = shift.ToList().GroupBy(e => e.EquipmentNumber);
                foreach (var eqN in GroupByEq)
                {
                    //row++;
                    var ListOfIdlesForThisEquipment = eqN.ToList();
                    foreach (var idle in ListOfIdlesForThisEquipment)
                    {
                        if (!idle.ShiftStart.HasValue) continue;
                        workSheet.Cells[row, "A"] = shift.Key.ShiftStart.Value.ToString("dd-MM-yyyy");//.ToString("dd-MM-yyyy") + " " + (shift.Key.IsNightShift ? "ночная" : "дневная") + " " + Shift.GetShiftNumber(shift.Key.ShiftStart.Value, shift.Key.IsNightShift);
                        workSheet.Cells[row, "A"].Font.Bold = true;
                        workSheet.Cells[row, "B"] = $"тех. комплекс {eqN.Key}";
                        workSheet.Cells[row, "C"] = Shift.GetShiftNumber(shift.Key.ShiftStart.Value, shift.Key.IsNightShift);
                        workSheet.Cells[row, "D"] = idle.IdleStart.HasValue ? idle.IdleStart.Value.ToString("HH\\:mm") : "";
                        workSheet.Cells[row, "E"] = idle.IdleEnd.HasValue ? idle.IdleEnd.Value.ToString("HH\\:mm") : "";
                        workSheet.Cells[row, "F"] = idle.IdleStart.HasValue && idle.IdleEnd.HasValue ? $"{(int)idle.IdleDuration.TotalHours}:{(int)idle.IdleDuration.Minutes:D2}" : "";// "Прост.";
                        workSheet.Cells[row, "G"] = idle.MalfunctionReasonTypeID.HasValue && reasonTypes.ContainsKey(idle.MalfunctionReasonTypeID.Value) ? reasonTypes[idle.MalfunctionReasonTypeID.Value].MalfunctionReasonTypeName : "";// "Вид";
                        workSheet.Cells[row, "H"] = idle.MalfunctionReasonProfileID.HasValue && profiles.ContainsKey(idle.MalfunctionReasonProfileID.Value) ? profiles[idle.MalfunctionReasonProfileID.Value].MalfunctionReasonProfileName : ""; //"Профиль";
                        workSheet.Cells[row, "I"] = idle.MalfunctionReasonNodeID.HasValue && nodes.ContainsKey(idle.MalfunctionReasonNodeID.Value) ? nodes[idle.MalfunctionReasonNodeID.Value].MalfunctionReasonNodeName : "";// "Узел";
                        workSheet.Cells[row, "J"] = idle.MalfunctionReasonElementID.HasValue && elements.ContainsKey(idle.MalfunctionReasonElementID.Value) ? elements[idle.MalfunctionReasonElementID.Value].MalfunctionReasonElementName : "";// "Элемент";
                        workSheet.Cells[row, "K"] = idle.MalfunctionReasonMalfunctionTextID.HasValue ? (malfunction.ContainsKey(idle.MalfunctionReasonMalfunctionTextID.Value) ? malfunction[idle.MalfunctionReasonMalfunctionTextID.Value].MalfunctionReasonMalfunctionTextName : "") : "";// "Неисправность";
                        workSheet.Cells[row, "L"] = idle.MalfunctionReasonMalfunctionTextComment;
                        row++;
                    }
                }
            }
             ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "A"]]).EntireColumn.NumberFormat = "MM/DD/YYYY";


            row++;

            var MachinesReasonsRow = row;

            workSheet.Cells[row, "A"] = "Смена";
            workSheet.Cells[row, "A"].Font.Bold = true;
            workSheet.Cells[row, "C"] = "Причина";
            workSheet.Cells[row, "C"].Font.Bold = true;
            workSheet.Cells[row, "H"] = "Время простоя";
            workSheet.Cells[row, "H"].Font.Bold = true;

            workSheet.Range[workSheet.Cells[row, "A"], workSheet.Cells[row, "B"]].Merge();
            workSheet.Range[workSheet.Cells[row, "C"], workSheet.Cells[row, "G"]].Merge();
            workSheet.Range[workSheet.Cells[row, "H"], workSheet.Cells[row, "I"]].Merge();

            row++;
            var ReportShifts = lst.OrderBy(e => e.ShiftNumber).GroupBy(e => e.ShiftNumber);
            TimeSpan totalIdleTime = new TimeSpan();
            //excelApp.Visible = true;
            foreach (var rmr in ReportShifts)
            {
                workSheet.Cells[row, "A"] = "Смена " + rmr.Key;
                workSheet.Cells[row, "A"].Font.Bold = true;
                workSheet.Range[workSheet.Cells[row, "A"], workSheet.Cells[row, "B"]].Merge();
                workSheet.Cells[row, "C"] = "";
                workSheet.Range[workSheet.Cells[row, "C"], workSheet.Cells[row, "G"]].Merge();
                workSheet.Range[workSheet.Cells[row, "C"], workSheet.Cells[row, "G"]].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                var reasons = rmr.ToList();
                TimeSpan totalShiftIdleTime = new TimeSpan();
                reasons.ForEach(ril => totalShiftIdleTime = totalShiftIdleTime.Add(ril.IdleDuration));
                totalIdleTime = totalIdleTime.Add(totalShiftIdleTime);
                var dategroup = rmr.GroupBy(r => new { r.ShiftStart, r.IsNightShift });
                var totalShiftTime = dategroup.Count() * 720 * ActiveMachines.Length;

                var reasonsGrp = reasons.GroupBy(rg => rg.MalfunctionReasonTypeID);
                row++;
                foreach (var rg in reasonsGrp)
                {
                    var reasonIdlelst = rg.ToList();


                    var reasonName = rg.Key.HasValue ? (reasonTypes.ContainsKey(rg.Key.Value) ? reasonTypes[rg.Key.Value].MalfunctionReasonTypeName : rg.Key.ToString()) : "Неизвестно";
                    TimeSpan totalReasonIdleTime = new TimeSpan();
                    reasonIdlelst.ForEach(ril => totalReasonIdleTime = totalReasonIdleTime.Add(ril.IdleDuration));
                    workSheet.Cells[row, "C"] = reasonName;
                    workSheet.Cells[row, "H"].NumberFormat = "@";
                    workSheet.Cells[row, "H"] = totalReasonIdleTime.ToHMString();//.ToString("mm\\:ss");
                    workSheet.Cells[row, "H"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                    workSheet.Cells[row, "I"] = (totalReasonIdleTime.TotalMinutes / totalShiftTime * 100).ToString("F2") + "%";
                    workSheet.Range[workSheet.Cells[row, "A"], workSheet.Cells[row, "B"]].Merge();
                    workSheet.Range[workSheet.Cells[row, "C"], workSheet.Cells[row, "G"]].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                    workSheet.Range[workSheet.Cells[row, "C"], workSheet.Cells[row, "G"]].Merge();
                    workSheet.Range[workSheet.Cells[row, "C"], workSheet.Cells[row, "G"]].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                    row++;
                }
                workSheet.Cells[row, "D"] = "Итого по смене " + rmr.Key;
                workSheet.Range[workSheet.Cells[row, "D"], workSheet.Cells[row, "G"]].Merge();
                workSheet.Cells[row, "D"].Font.Bold = true;
                workSheet.Cells[row, "H"].NumberFormat = "@";
                workSheet.Cells[row, "H"] = totalShiftIdleTime.ToHMString();
                workSheet.Cells[row, "H"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                workSheet.Cells[row, "I"] = (totalShiftIdleTime.TotalMinutes / TotalPeriod * 100).ToString("F2") + "%";
                row++;
            }

            workSheet.Cells[row, "D"] = "Итого сменам";
            workSheet.Range[workSheet.Cells[row, "D"], workSheet.Cells[row, "G"]].Merge();
            workSheet.Cells[row, "D"].Font.Italic = true;
            var totalTimeString = totalIdleTime.ToHMString(); ;
            workSheet.Cells[row, "H"].NumberFormat = "@";
            workSheet.Cells[row, "H"] = totalTimeString;
            workSheet.Cells[row, "H"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
            workSheet.Cells[row, "I"] = (totalIdleTime.TotalMinutes / (TotalPeriod) * 100).ToString("F2") + "%";
            row++;

            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[2, "O"]]).Cells.HorizontalAlignment =
                Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[2, "O"]]).Cells.VerticalAlignment =
                 Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

            //((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "C"]]).Cells.HorizontalAlignment =
            //    Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "C"]]).Cells.VerticalAlignment =
                 Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[StartRow - 1, "O"]].Font.Size = 14;
            workSheet.Range[workSheet.Cells[StartRow, "A"], workSheet.Cells[row, "O"]].Font.Size = 13;
            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "O"]].Font.Name = "Arial Cyr";
            for (int cols = 1; cols < 16; cols++)
            {
                workSheet.Columns[cols].AutoFit();
            }
            workSheet.Columns[4].AutoFit();
            excelApp.Visible = true;

        }
        // TODO: Отчет по незаполненным полям - дает возможность создать запрос по незаполненным полям за период по операторам и по машинам. Про какие поля речь? Незакрытые простои? Или без значение причины простоя? Пишу без причины простоя
        private void CreateNoReasonExcelReport(DateTime FromDate, DateTime ToDate, List<int> Machines, List<int> Shifts)
        {
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            // Make the object visible.

            // Create a new, empty workbook and add it to the collection returned
            // by property Workbooks. The new workbook becomes the active workbook.
            // Add has an optional parameter for specifying a praticular template.
            // Because no argument is sent in this example, Add creates a new workbook.
            excelApp.Workbooks.Add();
            // This example uses a single workSheet. The explicit type casting is
            // removed in a later procedure.
            Microsoft.Office.Interop.Excel._Worksheet workSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelApp.ActiveSheet;

            workSheet.Cells[1, "A"] = "Дата";
            workSheet.Cells[1, "B"] = "";
            workSheet.Cells[1, "C"] = "Смена";
            workSheet.Cells[1, "D"] = "Остан.";
            workSheet.Cells[1, "E"] = "Запуск";
            workSheet.Cells[1, "F"] = "Прост.";
            workSheet.Cells[1, "G"] = "Вид";
            workSheet.Cells[1, "H"] = "Профиль";
            workSheet.Cells[1, "I"] = "Узел";
            workSheet.Cells[1, "J"] = "Элемент";
            workSheet.Cells[1, "K"] = "Неисправность";
            workSheet.Cells[1, "L"] = "Примечания";

            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[1, "B"]].Merge();

            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[1, "L"]].Font.Bold = true;

            int StartRow = 2;
            int row = StartRow;
            var TotalPeriod = (ToDate - FromDate).TotalMinutes;
            //List<ReportData> reportList = new List<ReportData>();
            var lst = GetListForReport(Machines, Shifts, FromDate, ToDate);
            lst = lst.FindAll(e => e.MalfunctionReasonTypeID == null);
            Dictionary<int, MalfunctionReasonType> reasonTypes;
            Dictionary<int, MalfunctionReasonProfile> profiles;
            Dictionary<int, MalfunctionReasonNode> nodes;
            Dictionary<int, MalfunctionReasonElement> elements;
            Dictionary<int, MalfunctionReasonMalfunctionText> malfunction;
            using (var db = new DB())
            {
                reasonTypes = db.MalfunctionReasonTypes.ToDictionary(x => x.MalfunctionReasonTypeID);// ToList();
                profiles = db.MalfunctionReasonProfiles.ToDictionary(x => x.MalfunctionReasonProfileID);// ToList();
                nodes = db.MalfunctionReasonNodes.ToDictionary(x => x.MalfunctionReasonNodeID);//ToList();
                elements = db.MalfunctionReasonElements.ToDictionary(x => x.MalfunctionReasonElementID);//ToList();
                malfunction = db.MalfunctionReasonMalfunctionTexts.ToDictionary(x => x.MalfunctionReasonMalfunctionTextID);//ToList();
            }

            var ListGroupedByEquipmentNumber = lst.OrderBy(e => e.EquipmentNumber).GroupBy(e => e.EquipmentNumber);

            foreach (var eqN in ListGroupedByEquipmentNumber)
            {
                workSheet.Cells[row, "A"] = "Тех. комплекс " + eqN.Key;
                workSheet.Cells[row, "A"].Font.Bold = true;
                row++;
                var ListOfIdlesForThisEquipment = eqN.ToList();
                foreach (var idle in ListOfIdlesForThisEquipment)
                {
                    if (!idle.ShiftStart.HasValue) continue;
                    workSheet.Cells[row, "A"] = idle.ShiftStart.Value.ToString("dd-MM-yyyy");
                    workSheet.Cells[row, "B"] = idle.IsNightShift ? "Н" : "Д";
                    workSheet.Cells[row, "C"] = Shift.GetShiftNumber(idle.ShiftStart.Value, idle.IsNightShift); ;
                    workSheet.Cells[row, "D"] = idle.IdleStart.HasValue ? idle.IdleStart.Value.ToString("HH\\:mm") : "";
                    workSheet.Cells[row, "E"] = idle.IdleEnd.HasValue ? idle.IdleEnd.Value.ToString("HH\\:mm") : "";
                    workSheet.Cells[row, "F"] = idle.IdleStart.HasValue && idle.IdleEnd.HasValue ? (idle.IdleEnd.Value - idle.IdleStart.Value).ToString("hh\\:mm") : "";// "Прост.";
                    workSheet.Cells[row, "G"] = idle.MalfunctionReasonTypeID.HasValue && reasonTypes.ContainsKey(idle.MalfunctionReasonTypeID.Value) ? reasonTypes[idle.MalfunctionReasonTypeID.Value].MalfunctionReasonTypeName : "";// "Вид";
                    workSheet.Cells[row, "H"] = idle.MalfunctionReasonProfileID.HasValue && profiles.ContainsKey(idle.MalfunctionReasonProfileID.Value) ? profiles[idle.MalfunctionReasonProfileID.Value].MalfunctionReasonProfileName : ""; //"Профиль";
                    workSheet.Cells[row, "I"] = idle.MalfunctionReasonNodeID.HasValue && nodes.ContainsKey(idle.MalfunctionReasonNodeID.Value) ? nodes[idle.MalfunctionReasonNodeID.Value].MalfunctionReasonNodeName : "";// "Узел";
                    workSheet.Cells[row, "J"] = idle.MalfunctionReasonElementID.HasValue && elements.ContainsKey(idle.MalfunctionReasonElementID.Value) ? elements[idle.MalfunctionReasonElementID.Value].MalfunctionReasonElementName : "";// "Элемент";
                    workSheet.Cells[row, "K"] = idle.MalfunctionReasonMalfunctionTextID.HasValue ? (malfunction.ContainsKey(idle.MalfunctionReasonMalfunctionTextID.Value) ? malfunction[idle.MalfunctionReasonMalfunctionTextID.Value].MalfunctionReasonMalfunctionTextName : "") : "";// "Неисправность";
                    workSheet.Cells[row, "L"] = idle.MalfunctionReasonMalfunctionTextComment;
                    row++;
                }
            }


            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[2, "O"]]).Cells.HorizontalAlignment =
                Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[2, "O"]]).Cells.VerticalAlignment =
                 Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "C"]]).Cells.HorizontalAlignment =
                Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "A"]]).EntireColumn.NumberFormat = "MM/DD/YYYY";

            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "C"]]).Cells.VerticalAlignment =
                 Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[StartRow - 1, "O"]].Font.Size = 14;
            workSheet.Range[workSheet.Cells[StartRow, "A"], workSheet.Cells[row, "O"]].Font.Size = 13;
            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "O"]].Font.Name = "Arial Cyr";
            for (int cols = 1; cols < 16; cols++)
            {
                workSheet.Columns[cols].AutoFit();
            }
            workSheet.Columns[4].AutoFit();
            excelApp.Visible = true;

        }
        private void CreateNotesExcelReport(DateTime FromDate, DateTime ToDate, List<int> Machines, List<int> Shifts)
        {
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            // Make the object visible.

            // Create a new, empty workbook and add it to the collection returned
            // by property Workbooks. The new workbook becomes the active workbook.
            // Add has an optional parameter for specifying a praticular template.
            // Because no argument is sent in this example, Add creates a new workbook.
            excelApp.Workbooks.Add();

            // This example uses a single workSheet. The explicit type casting is
            // removed in a later procedure.
            Microsoft.Office.Interop.Excel._Worksheet workSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelApp.ActiveSheet;

            workSheet.Cells[1, "A"] = "Дата";
            workSheet.Cells[1, "B"] = "";
            workSheet.Cells[1, "C"] = "Смена";
            workSheet.Cells[1, "D"] = "Остан.";
            workSheet.Cells[1, "E"] = "Запуск";
            workSheet.Cells[1, "F"] = "Прост.";
            workSheet.Cells[1, "G"] = "Вид";
            workSheet.Cells[1, "H"] = "Профиль";
            workSheet.Cells[1, "I"] = "Узел";
            workSheet.Cells[1, "J"] = "Элемент";
            workSheet.Cells[1, "K"] = "Неисправность";
            workSheet.Cells[1, "L"] = "Примечания";

            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[1, "B"]].Merge();

            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[1, "L"]].Font.Bold = true;

            int StartRow = 2;
            int row = StartRow;
            var TotalPeriod = (ToDate - FromDate).TotalMinutes;
            //List<ReportData> reportList = new List<ReportData>();
            var lst = GetListForReport(Machines, Shifts, FromDate, ToDate);
            lst = lst.FindAll(e => e.MalfunctionReasonMalfunctionTextComment != null && e.MalfunctionReasonTypeID == 1);
            Dictionary<int, MalfunctionReasonType> reasonTypes;
            Dictionary<int, MalfunctionReasonProfile> profiles;
            Dictionary<int, MalfunctionReasonNode> nodes;
            Dictionary<int, MalfunctionReasonElement> elements;
            Dictionary<int, MalfunctionReasonMalfunctionText> malfunction;
            using (var db = new DB())
            {
                reasonTypes = db.MalfunctionReasonTypes.ToDictionary(x => x.MalfunctionReasonTypeID);// ToList();
                profiles = db.MalfunctionReasonProfiles.ToDictionary(x => x.MalfunctionReasonProfileID);// ToList();
                nodes = db.MalfunctionReasonNodes.ToDictionary(x => x.MalfunctionReasonNodeID);//ToList();
                elements = db.MalfunctionReasonElements.ToDictionary(x => x.MalfunctionReasonElementID);//ToList();
                malfunction = db.MalfunctionReasonMalfunctionTexts.ToDictionary(x => x.MalfunctionReasonMalfunctionTextID);//ToList();
            }

            var ListGroupedByEquipmentNumber = lst.OrderBy(e => e.EquipmentNumber).GroupBy(e => e.EquipmentNumber);

            foreach (var eqN in ListGroupedByEquipmentNumber)
            {
                workSheet.Cells[row, "A"] = "Тех. комплекс " + eqN.Key;
                workSheet.Cells[row, "A"].Font.Bold = true;
                row++;
                var ListOfIdlesForThisEquipment = eqN.ToList();
                foreach (var idle in ListOfIdlesForThisEquipment)
                {
                    if (!idle.ShiftStart.HasValue) continue;
                    workSheet.Cells[row, "A"] = idle.ShiftStart.Value.ToString("dd-MM-yyyy");
                    workSheet.Cells[row, "B"] = idle.IsNightShift ? "Н" : "Д";
                    workSheet.Cells[row, "C"] = Shift.GetShiftNumber(idle.ShiftStart.Value, idle.IsNightShift); ;
                    workSheet.Cells[row, "D"] = idle.IdleStart.HasValue ? idle.IdleStart.Value.ToString("HH\\:mm") : "";
                    workSheet.Cells[row, "E"] = idle.IdleEnd.HasValue ? idle.IdleEnd.Value.ToString("HH\\:mm") : "";
                    workSheet.Cells[row, "F"] = idle.IdleStart.HasValue && idle.IdleEnd.HasValue ? (idle.IdleEnd.Value - idle.IdleStart.Value).ToString("hh\\:mm") : "";// "Прост.";
                    workSheet.Cells[row, "G"] = idle.MalfunctionReasonTypeID.HasValue && reasonTypes.ContainsKey(idle.MalfunctionReasonTypeID.Value) ? reasonTypes[idle.MalfunctionReasonTypeID.Value].MalfunctionReasonTypeName : "";// "Вид";
                    workSheet.Cells[row, "H"] = idle.MalfunctionReasonProfileID.HasValue && profiles.ContainsKey(idle.MalfunctionReasonProfileID.Value) ? profiles[idle.MalfunctionReasonProfileID.Value].MalfunctionReasonProfileName : ""; //"Профиль";
                    workSheet.Cells[row, "I"] = idle.MalfunctionReasonNodeID.HasValue && nodes.ContainsKey(idle.MalfunctionReasonNodeID.Value) ? nodes[idle.MalfunctionReasonNodeID.Value].MalfunctionReasonNodeName : "";// "Узел";
                    workSheet.Cells[row, "J"] = idle.MalfunctionReasonElementID.HasValue && elements.ContainsKey(idle.MalfunctionReasonElementID.Value) ? elements[idle.MalfunctionReasonElementID.Value].MalfunctionReasonElementName : "";// "Элемент";
                    workSheet.Cells[row, "K"] = idle.MalfunctionReasonMalfunctionTextID.HasValue ? (malfunction.ContainsKey(idle.MalfunctionReasonMalfunctionTextID.Value) ? malfunction[idle.MalfunctionReasonMalfunctionTextID.Value].MalfunctionReasonMalfunctionTextName : "") : "";// "Неисправность";
                    workSheet.Cells[row, "L"] = idle.MalfunctionReasonMalfunctionTextComment;
                    row++;
                }
            }



            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[2, "O"]]).Cells.HorizontalAlignment =
                Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[2, "O"]]).Cells.VerticalAlignment =
                 Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "C"]]).Cells.HorizontalAlignment =
                Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "A"]]).EntireColumn.NumberFormat = "MM/DD/YYYY";

            ((Microsoft.Office.Interop.Excel.Range)workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "C"]]).Cells.VerticalAlignment =
                 Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[StartRow - 1, "O"]].Font.Size = 14;
            workSheet.Range[workSheet.Cells[StartRow, "A"], workSheet.Cells[row, "O"]].Font.Size = 13;
            workSheet.Range[workSheet.Cells[1, "A"], workSheet.Cells[row, "O"]].Font.Name = "Arial Cyr";
            for (int cols = 1; cols < 16; cols++)
            {
                workSheet.Columns[cols].AutoFit();
            }
            workSheet.Columns[4].AutoFit();
            excelApp.Visible = true;

        }

        private void btnExcelEquipmentReport_Click(object sender, EventArgs e)
        {
            SortedSet<int> ShiftsChecked = new SortedSet<int>();
            foreach (ListViewItem item in lvShifts.Items)
            {
                if (item.Checked)
                    ShiftsChecked.Add((int)item.Tag);
            }
            ShiftsChecked.Remove(0);
            SortedSet<int> MachinesChecked = new SortedSet<int>();
            foreach (ListViewItem item in lvMachines.Items)
            {
                if (item.Checked)
                    MachinesChecked.Add((int)item.Tag);
            }
            MachinesChecked.Remove(0);
            CreateEquipmentExcelReport(dtpFrom.Value, dtpTo.Value, MachinesChecked.ToList(), ShiftsChecked.ToList());
        }
        private void btnExcelShiftReport_Click(object sender, EventArgs e)
        {
            SortedSet<int> ShiftsChecked = new SortedSet<int>();
            foreach (ListViewItem item in lvShifts.Items)
            {
                if (item.Checked)
                    ShiftsChecked.Add((int)item.Tag);
            }
            ShiftsChecked.Remove(0);
            SortedSet<int> MachinesChecked = new SortedSet<int>();
            foreach (ListViewItem item in lvMachines.Items)
            {
                if (item.Checked)
                    MachinesChecked.Add((int)item.Tag);
            }
            MachinesChecked.Remove(0);
            CreateShiftExcelReport(dtpFrom.Value, dtpTo.Value, MachinesChecked.ToList(), ShiftsChecked.ToList());
        }

        private void btnEmptyIdles_Click(object sender, EventArgs e)
        {
            SortedSet<int> ShiftsChecked = new SortedSet<int>();
            foreach (ListViewItem item in lvShifts.Items)
            {
                if (item.Checked)
                    ShiftsChecked.Add((int)item.Tag);
            }
            ShiftsChecked.Remove(0);
            SortedSet<int> MachinesChecked = new SortedSet<int>();
            foreach (ListViewItem item in lvMachines.Items)
            {
                if (item.Checked)
                    MachinesChecked.Add((int)item.Tag);
            }
            MachinesChecked.Remove(0);
            CreateNoReasonExcelReport(dtpFrom.Value, dtpTo.Value, MachinesChecked.ToList(), ShiftsChecked.ToList());
        }

        private void btnNotesIdle_Click(object sender, EventArgs e)
        {
            SortedSet<int> ShiftsChecked = new SortedSet<int>();
            foreach (ListViewItem item in lvShifts.Items)
            {
                if (item.Checked)
                    ShiftsChecked.Add((int)item.Tag);
            }
            ShiftsChecked.Remove(0);
            SortedSet<int> MachinesChecked = new SortedSet<int>();
            foreach (ListViewItem item in lvMachines.Items)
            {
                if (item.Checked)
                    MachinesChecked.Add((int)item.Tag);
            }
            MachinesChecked.Remove(0);
            CreateNotesExcelReport(dtpFrom.Value, dtpTo.Value, MachinesChecked.ToList(), ShiftsChecked.ToList());
        }
    }

    public static class TimeSpanExtend
    {
        public static string ToFullString(this TimeSpan t, int lastTimePart)
        {
            var TimePartsQuantity = 5;
            var TimeParts = new TimePart[] {
                new TimePart(0, ".", 1,10),
                new TimePart(0, ":", 24,2),
                new TimePart(0, ":", 60,2),
                new TimePart(0, ".", 60,2),
                new TimePart(0, "", 1000,3)
            };

            StringBuilder sb = new StringBuilder();
            bool MandatorySubElements = false;
            var time = (int)t.TotalMilliseconds;
            bool Negative = time < 0;
            time = Math.Abs(time);

            for (int i = TimePartsQuantity - 1; i >= 0; i--)
            {
                TimeParts[i].Value = time % TimeParts[i].Divisor;
                time /= TimeParts[i].Divisor;
            }
            for (int i = 0; i < TimePartsQuantity; i++)
            {
                if (TimeParts[i].Value != 0 || MandatorySubElements)
                {
                    string TimePartString;
                    if (MandatorySubElements)
                        TimePartString = TimeParts[i].Value.ToString($"D{TimeParts[i].MaxDigits}");
                    else
                        TimePartString = TimeParts[i].Value.ToString();
                    sb.Append(TimePartString);
                    if (i == lastTimePart) break;
                    sb.Append(TimeParts[i].EndsWith);
                    MandatorySubElements = true;
                }
            }
            return sb.ToString();
        }
        public static string ToHMString(this TimeSpan t)
        {
            StringBuilder sb = new StringBuilder();
            var time = (int)t.TotalMinutes;
            bool Negative = time < 0;
            time = Math.Abs(time);
            var minutes = time % 60;
            time /= 60;
            sb.Append($"{(Negative ? "-" : "")}{time}:{minutes:D2}");
            return sb.ToString();
        }
        public static string ToHMSString(this TimeSpan t)
        {
            StringBuilder sb = new StringBuilder();
            var seconds = (int)t.Seconds;
            var time = (int)t.TotalMinutes;
            bool Negative = time < 0;
            time = Math.Abs(time);
            var minutes = time % 60;
            time /= 60;
            sb.Append($"{(Negative ? "-" : "")}{time}:{minutes:D2}:{seconds:D2}");
            return sb.ToString();
        }


        public class TimePart
        {
            public int Value;
            public string EndsWith;
            public int Divisor;
            public int MaxDigits;

            public TimePart(int value, string endsWith, int divisor, int maxDigits)
            {
                Value = value;
                EndsWith = endsWith;
                Divisor = divisor;
                MaxDigits = maxDigits;
            }
        }

    }
}
