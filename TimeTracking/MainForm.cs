using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using TimeTracking.DictionaryForms;
using TimeTracking.ShiftsForms;
using TimeTrackingDB;

namespace TimeTracking
{
    public partial class MainForm : Form
    {
        User LoggedUser;
        FormData CurrentData = new FormData();
        public MainForm(User User)
        {
            LoggedUser = User;
            InitializeComponent();
            foreach (ToolStripMenuItem tsmi in menuStrip1.Items)
            {
                int tag = 0;
                try
                {
                    if (tsmi.Tag!=null)
                        tag = Convert.ToInt32(tsmi.Tag);
                }
                catch (Exception ex)
                {

                }
                if ((tag & (int)LoggedUser.Rights) != 0 || (LoggedUser.Rights & UserRight.Administrator) == UserRight.Administrator)
                {
                    tsmi.Visible = true;
                }
                else
                    tsmi.Visible = false;
            }

            CurrentData.CurrentUser = LoggedUser;


            /*var MainPeriod = new TimePeriod(new DateTime(2023, 2, 1), new DateTime(2023, 2, 28), "Февраль");
            var list1 = new List<TimePeriod>() {MainPeriod};


            var test00 = new TimePeriod(new DateTime(2023, 1, 1), new DateTime(2023, 1, 15), "Test00");
            var test01 = new TimePeriod(new DateTime(2023, 1, 1), new DateTime(2023, 2, 1), "Test01");
            var test02 = new TimePeriod(new DateTime(2023, 1, 1), new DateTime(2023, 2, 15), "Test02");
            var test03 = new TimePeriod(new DateTime(2023, 1, 1), new DateTime(2023, 2, 28), "Test03");
            var test04 = new TimePeriod(new DateTime(2023, 1, 1), new DateTime(2023, 3, 15), "Test04");
            var test05 = new TimePeriod(new DateTime(2023, 2, 1), new DateTime(2023, 2, 15), "Test05");
            var test06 = new TimePeriod(new DateTime(2023, 2, 1), new DateTime(2023, 2, 28), "Test06");
            var test07 = new TimePeriod(new DateTime(2023, 2, 1), new DateTime(2023, 3, 15), "Test07");
            var test08 = new TimePeriod(new DateTime(2023, 2, 15), new DateTime(2023, 2, 20), "Test08");
            var test09 = new TimePeriod(new DateTime(2023, 2, 15), new DateTime(2023, 2, 28), "Test09");
            var test10 = new TimePeriod(new DateTime(2023, 2, 15), new DateTime(2023, 3, 15), "Test10");
            var test11 = new TimePeriod(new DateTime(2023, 2, 28), new DateTime(2023, 3, 15), "Test11");
            var test12 = new TimePeriod(new DateTime(2023, 3, 10), new DateTime(2023, 3, 15), "Test12");

            var list2 = new List<TimePeriod>() { test02, test08, test12 };

           
            var res=TimePeriod.CrossPeriods(list1, list2, new List<TimePeriod>());
            */

        }

        private void ShowNewForm<T>(object data = null) where T : TimeTrackingDataForm, new()
        {
            /*var openForms = new List<Form>();
            foreach (Form form in Application.OpenForms)
            {
                openForms.Add(form);
            }
            foreach (Form form in openForms)
            {
                if (form.Parent == pnlMainForm)
                {
                    form.Close();
                    try
                    {
                        if (form.Parent != null)
                            form.Parent = null;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            try
            {
                var newForm = new T();
                newForm.Tag = data;
                newForm.TopLevel = false;
                newForm.Parent = pnlMainForm;
                newForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                newForm.StartPosition = FormStartPosition.CenterParent;
                var iDictPrep = newForm as IDictionaryPrepare;
                if (iDictPrep != null) iDictPrep.Prepare();
                newForm.SetCurrentData(CurrentData);
                newForm.Visible = true;
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message+(ex.InnerException!=null?"("+ex.InnerException.Message+")":""), "Ошибка");
            }
            */
            this.IsMdiContainer = true;

            CloseWindow<T>();
            /*foreach (var form in this.MdiChildren)
            {
                if (form is T) form.Close();
            }*/
            OpenWindow<T>(data);
            
        }

        private void CloseWindow<T>() where T : TimeTrackingDataForm, new()
        {
            foreach (var form in this.MdiChildren)
            {
                if (form is T)
                {
                    RemoveWindowMenuElement(form);
                    form.Close();
                }
            }
        }
        private void OpenWindow<T>(object data = null) where T: TimeTrackingDataForm, new()
        {
            var newForm = new T();
            newForm.Tag = data;
            newForm.TopLevel = false;
            var iDictPrep = newForm as IDictionaryPrepare;
            if (iDictPrep != null) iDictPrep.Prepare();
            newForm.SetCurrentData(CurrentData);
            newForm.MdiParent = this;
            newForm.Visible = true;
            newForm.WindowState = FormWindowState.Normal;
            newForm.WindowState = FormWindowState.Maximized;
            newForm.FormClosed += ChildForm_FormClosed;
            AddWindowMenuElement(newForm);
        }

        private void ChildForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            RemoveWindowMenuElement(sender as Form);
        }

        private void AddWindowMenuElement(Form form)
        {
            var newItem = new ToolStripMenuItem(form.Text);
            newItem.Click += NewItem_Click;
            newItem.Tag = form;
            tsmiOpenWindows.DropDownItems.Add(newItem);
        }

        private void RemoveWindowMenuElement(Form form)
        {
            List<ToolStripMenuItem> toRemove = new List<ToolStripMenuItem>();
            foreach(ToolStripMenuItem mi in tsmiOpenWindows.DropDownItems)
            {
                if (mi.Tag == form) toRemove.Add(mi);
            }
            foreach (var mi in toRemove)
                tsmiOpenWindows.DropDownItems.Remove(mi);
        }
        private void NewItem_Click(object sender, EventArgs e)
        {
            var form = (Form)((ToolStripMenuItem)sender).Tag;
            form.BringToFront();
            form.Focus();
        }

        private void tsmiMSStaff_Click(object sender, EventArgs e)
        {
            ShowNewForm<MaintainceServiceStaffTimeTracking>();
        }

        private void tsmiDictionaryMaintananceServiceStaff_Click(object sender, EventArgs e)
        {
            ShowNewForm<DictionaryMaintananceServiceStaff>();
        }

        private void tsmiDictionaryShiftStaff_Click(object sender, EventArgs e)
        {
            ShowNewForm<DictionaryShiftStaff>();
        }

        private void tsmiDictionaryUsers_Click(object sender, EventArgs e)
        {
            ShowNewForm<DictionaryUsers>();
        }

        private void tsmiShiftsDowntime_Click(object sender, EventArgs e)
        {
            ShowNewForm<ShiftsForms.ShiftsDowntimeForm>();
        }

        //TODO: Нужны ли отдельные замечания по ППР или можно внести все в том же окне, где вносятся причины простоев?
        //TODO: Как выбирается оператор и как он хранится? Когда вносится это значение в автоматических операциях?
        private void tsmiShiftsSPMNotice_Click(object sender, EventArgs e)
        {
            ShowNewForm<ShiftsForms.ShiftsNotesForm>();
        }

        private void tsmiShiftsReportDowntime_Click(object sender, EventArgs e)
        {
            ShowNewForm<Reports.ShiftsReportDowntimeForm>();
        }

        private void tsmiDictionaryIdleReasons_Click(object sender, EventArgs e)
        {
            ShowNewForm<DictionaryForms.DictionaryIdleReasonsForm>();
        }

        private void tsmiEmployeePosition_Click(object sender, EventArgs e)
        {
            //ShowNewForm<DictionaryEmployeePosition>();
            ShowNewForm<DictionaryPlainList>(new DictionaryPlainList.Parameters() { Caption = "Должности", TypeName= "TimeTrackingDB.EmployeePosition" });
        }

        private void tsmiMSSTT_Click(object sender, EventArgs e)
        {
            ShowNewForm<MaintainceServiceStaffTimeTracking>();
        }

        private void tsmiSTTReport_Click(object sender, EventArgs e)
        {
            ShowNewForm<Reports.StaffTimeTrackingReport>();
        }

        private void tsmiLaborCostShift_Click(object sender, EventArgs e)
        {
            ShowNewForm<Reports.LaborCostReportForm>();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Программа учета времени {VersionLabel}"); 
        }
        public string VersionLabel
        {
            get
            {
                Version ver;
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    ver = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
                }
                else
                {
                    ver = Assembly.GetExecutingAssembly().GetName().Version;
                }
                return ver.ToString();
            }
        }

        private void tsmiEquipmentBlocking_Click(object sender, EventArgs e)
        {
            ShowNewForm<EquipmentBlockingForm>();
        }

        private void tsmiBalanceTimeGraph_Click(object sender, EventArgs e)
        {
            ShowNewForm<TimeTracking.Reports.ShiftEffectivenessPeriodForm>();
        }

        private void tsmiMaintenanceIdleEquipment_Click(object sender, EventArgs e)
        {
            ShowNewForm<ShiftsForms.ShiftsDowntimeForm>();
        }

        private void простоиОборудованияЗаСменуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowNewForm<ShiftsForms.ShiftsDowntimeForm>();
        }
    }
}
